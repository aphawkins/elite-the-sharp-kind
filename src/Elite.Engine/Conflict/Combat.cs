// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Diagnostics;
using System.Numerics;
using Elite.Common.Enums;
using Elite.Engine.Enums;
using Elite.Engine.Ships;
using Elite.Engine.Trader;
using Elite.Engine.Types;

namespace Elite.Engine.Conflict
{
    internal sealed class Combat
    {
        internal bool InBattle { get; set; }
        internal int _isMISSILE_ARMED = -1;
        internal int IsMissileUnarmed { get; private set; } = -2;
        internal int MissileTarget { get; private set; }
        private readonly Audio _audio;
        private readonly GameState _gameState;
        private readonly PlayerShip _ship;
        private readonly Trade _trade;
        private readonly ShipFlags[] _initialFlags = new ShipFlags[34]
        {
            0,											// NULL,
			0,											// missile 
			0,											// coriolis
			ShipFlags.Slow | ShipFlags.FlyToPlanet,				// escape
			ShipFlags.Inactive,								// alloy
			ShipFlags.Inactive,								// cargo
			ShipFlags.Inactive,								// boulder
			ShipFlags.Inactive,								// asteroid
			ShipFlags.Inactive,								// rock
			ShipFlags.FlyToPlanet | ShipFlags.Slow,				// shuttle
			ShipFlags.FlyToPlanet | ShipFlags.Slow,				// transporter
			0,											// cobra3
			0,											// python
			0,											// boa
			ShipFlags.Slow,									// anaconda
			ShipFlags.Slow,									// hermit
			ShipFlags.Bold | ShipFlags.Police,						// viper
			ShipFlags.Bold | ShipFlags.Angry,						// sidewinder
			ShipFlags.Bold | ShipFlags.Angry,						// mamba
			ShipFlags.Bold | ShipFlags.Angry,						// krait
			ShipFlags.Bold | ShipFlags.Angry,						// adder
			ShipFlags.Bold | ShipFlags.Angry,						// gecko
			ShipFlags.Bold | ShipFlags.Angry,						// cobra1
			ShipFlags.Slow | ShipFlags.Angry,						// worm
			ShipFlags.Bold | ShipFlags.Angry,						// cobra3
			ShipFlags.Bold | ShipFlags.Angry,						// asp2
			ShipFlags.Bold | ShipFlags.Angry,						// python
			ShipFlags.Police,									// fer_de_lance
			ShipFlags.Bold | ShipFlags.Angry,						// moray
			ShipFlags.Bold | ShipFlags.Angry,						// thargoid
			ShipFlags.Angry,									// tharlet
			ShipFlags.Angry,									// constrictor
			ShipFlags.Police | ShipFlags.Cloaked,					// cougar
			0											// dodec
		};

        private bool _isEcmOurs;
        private int _laserCounter;
        private int _laserStrength;
        private LaserType _laserType;

        internal Combat(GameState gameState, Audio audio, PlayerShip ship, Trade trade)
        {
            _gameState = gameState;
            _audio = audio;
            _ship = ship;
            _trade = trade;
        }

        internal void ActivateECM(bool ours)
        {
            if (_ship.EcmActive == 0)
            {
                _ship.EcmActive = 32;
                _isEcmOurs = ours;
                _audio.PlayEffect(SoundEffect.Ecm);
            }
        }

        internal int AddNewShip(ShipType ship_type, Vector3 location, Vector3[] rotmat, float rotx, float rotz)
        {
            Debug.Assert(rotmat != null);
            for (int i = 0; i < EliteMain.MAX_UNIV_OBJECTS; i++)
            {
                if (Space.s_universe[i].Type == ShipType.None)
                {
                    Space.s_universe[i].Type = ship_type;
                    Space.s_universe[i].Location = location;
                    Space.s_universe[i].Rotmat = rotmat;
                    Space.s_universe[i].RotX = rotx;
                    Space.s_universe[i].RotZ = rotz;
                    Space.s_universe[i].Velocity = 0;
                    Space.s_universe[i].Acceleration = 0;
                    Space.s_universe[i].Bravery = 0;
                    Space.s_universe[i].Target = 0;
                    Space.s_universe[i].Flags = _initialFlags[(int)ship_type < 0 ? 0 : (int)ship_type];

                    if (ship_type is not ShipType.Planet and not ShipType.Sun)
                    {
                        Space.s_universe[i].Energy = _gameState.ShipList[ship_type].EnergyMax;
                        Space.s_universe[i].Missiles = _gameState.ShipList[ship_type].MissilesMax;
                        Space.s_ship_count[ship_type]++;
                    }

                    return i;
                }
            }

            return -1;
        }

        internal void AddNewStation(Vector3 position, Vector3[] rotmat)
        {
            ShipType station = _gameState.CurrentPlanetData.TechLevel >= 10 ? ShipType.Dodec : ShipType.Coriolis;
            Space.s_universe[1].Type = ShipType.None;
            AddNewShip(station, position, rotmat, 0, -127);
        }

        internal void ArmMissile()
        {
            if (_ship.MissileCount != 0 && MissileTarget == IsMissileUnarmed)
            {
                MissileTarget = _isMISSILE_ARMED;
            }
        }

        internal void CheckTarget(int un, ref UniverseObject flip)
        {
            //univ_object univ = space.universe[un];

            if (IsInTarget(Space.s_universe[un].Type, flip.Location.X, flip.Location.Y, flip.Location.Z))
            {
                if (MissileTarget == _isMISSILE_ARMED && Space.s_universe[un].Type >= 0)
                {
                    MissileTarget = un;
                    _gameState.InfoMessage("Target Locked");
                    _audio.PlayEffect(SoundEffect.Beep);
                }

                if (_laserStrength > 0)
                {
                    _audio.PlayEffect(SoundEffect.HitEnemy);

                    if (Space.s_universe[un].Type is not ShipType.Coriolis and not ShipType.Dodec)
                    {
                        if (Space.s_universe[un].Type is ShipType.Constrictor or ShipType.Cougar)
                        {
                            if (_laserType == LaserType.Military)
                            {
                                Space.s_universe[un].Energy -= _laserStrength / 4;
                            }
                        }
                        else
                        {
                            Space.s_universe[un].Energy -= _laserStrength;
                        }
                    }

                    if (Space.s_universe[un].Energy <= 0)
                    {
                        ExplodeObject(un);

                        if (Space.s_universe[un].Type == ShipType.Asteroid)
                        {
                            if (_laserType is LaserType.Mining or LaserType.Pulse)
                            {
                                LaunchLoot(un, ShipType.Rock);
                            }
                        }
                        else
                        {
                            LaunchLoot(un, ShipType.Alloy);
                            LaunchLoot(un, ShipType.Cargo);
                        }
                    }

                    MakeAngry(un);
                }
            }
        }

        internal void ClearUniverse()
        {
            for (int i = 0; i < EliteMain.MAX_UNIV_OBJECTS; i++)
            {
                Space.s_universe[i] = new()
                {
                    Type = 0
                };
            }

            foreach (ShipType ship in _gameState.ShipList.Keys.ToList())
            {
                Space.s_ship_count[ship] = 0;
            }

            InBattle = false;
        }
        internal void CoolLaser()
        {
            _laserStrength = 0;
            _laserType = LaserType.None;
            _gameState.DrawLasers = false;

            if (_gameState.LaserTemp > 0)
            {
                _gameState.LaserTemp--;
            }

            _laserCounter = Math.Clamp(_laserCounter - 2, 0, _laserCounter);
        }

        internal void CreateThargoid()
        {
            int newship = CreateOtherShip(ShipType.Thargoid);
            if (newship != -1)
            {
                Space.s_universe[newship].Flags = ShipFlags.Angry | ShipFlags.HasECM;
                Space.s_universe[newship].Bravery = 113;

                if (RNG.Random(255) > 64)
                {
                    LaunchEnemy(newship, ShipType.Tharglet, ShipFlags.Angry | ShipFlags.HasECM, 96);
                }
            }
        }

        internal void ExplodeObject(int un)
        {
            _gameState.Cmdr.Score++;

            if ((_gameState.Cmdr.Score & 255) == 0)
            {
                _gameState.InfoMessage("Right On Commander!");
            }

            _audio.PlayEffect(SoundEffect.Explode);
            Space.s_universe[un].Flags |= ShipFlags.Dead;

            if (Space.s_universe[un].Type == ShipType.Constrictor)
            {
                _gameState.Cmdr.Mission = 2;
            }
        }

        internal bool FireLaser()
        {
            if (_gameState.IsDocked ||
                _gameState.DrawLasers ||
                _laserCounter != 0 ||
                _gameState.LaserTemp >= 242)
            {
                return false;
            }

            _laserStrength = _gameState.CurrentScreen switch
            {
                SCR.SCR_FRONT_VIEW => _ship.LaserFront.Strength,
                SCR.SCR_REAR_VIEW => _ship.LaserRear.Strength,
                SCR.SCR_RIGHT_VIEW => _ship.LaserRight.Strength,
                SCR.SCR_LEFT_VIEW => _ship.LaserLeft.Strength,
                SCR.SCR_NONE => throw new NotImplementedException(),
                SCR.SCR_INTRO_ONE => throw new NotImplementedException(),
                SCR.SCR_INTRO_TWO => throw new NotImplementedException(),
                SCR.SCR_GALACTIC_CHART => throw new NotImplementedException(),
                SCR.SCR_SHORT_RANGE => throw new NotImplementedException(),
                SCR.SCR_PLANET_DATA => throw new NotImplementedException(),
                SCR.SCR_MARKET_PRICES => throw new NotImplementedException(),
                SCR.SCR_CMDR_STATUS => throw new NotImplementedException(),
                SCR.SCR_DOCKING => throw new NotImplementedException(),
                SCR.SCR_INVENTORY => throw new NotImplementedException(),
                SCR.SCR_EQUIP_SHIP => throw new NotImplementedException(),
                SCR.SCR_OPTIONS => throw new NotImplementedException(),
                SCR.SCR_LOAD_CMDR => throw new NotImplementedException(),
                SCR.SCR_SAVE_CMDR => throw new NotImplementedException(),
                SCR.SCR_QUIT => throw new NotImplementedException(),
                SCR.SCR_GAME_OVER => throw new NotImplementedException(),
                SCR.SCR_SETTINGS => throw new NotImplementedException(),
                SCR.SCR_ESCAPE_CAPSULE => throw new NotImplementedException(),
                SCR.SCR_MISSION_1 => throw new NotImplementedException(),
                SCR.SCR_MISSION_2 => throw new NotImplementedException(),
                SCR.SCR_UNDOCKING => throw new NotImplementedException(),
                SCR.SCR_HYPERSPACE => throw new NotImplementedException(),
                _ => 0,
            };

            if (_laserStrength == 0)
            {
                return false;
            }

            _laserType = _gameState.CurrentScreen switch
            {
                SCR.SCR_FRONT_VIEW => _ship.LaserFront.Type,
                SCR.SCR_REAR_VIEW => _ship.LaserRear.Type,
                SCR.SCR_RIGHT_VIEW => _ship.LaserRight.Type,
                SCR.SCR_LEFT_VIEW => _ship.LaserLeft.Type,
                SCR.SCR_NONE => throw new NotImplementedException(),
                SCR.SCR_INTRO_ONE => throw new NotImplementedException(),
                SCR.SCR_INTRO_TWO => throw new NotImplementedException(),
                SCR.SCR_GALACTIC_CHART => throw new NotImplementedException(),
                SCR.SCR_SHORT_RANGE => throw new NotImplementedException(),
                SCR.SCR_PLANET_DATA => throw new NotImplementedException(),
                SCR.SCR_MARKET_PRICES => throw new NotImplementedException(),
                SCR.SCR_CMDR_STATUS => throw new NotImplementedException(),
                SCR.SCR_DOCKING => throw new NotImplementedException(),
                SCR.SCR_INVENTORY => throw new NotImplementedException(),
                SCR.SCR_EQUIP_SHIP => throw new NotImplementedException(),
                SCR.SCR_OPTIONS => throw new NotImplementedException(),
                SCR.SCR_LOAD_CMDR => throw new NotImplementedException(),
                SCR.SCR_SAVE_CMDR => throw new NotImplementedException(),
                SCR.SCR_QUIT => throw new NotImplementedException(),
                SCR.SCR_GAME_OVER => throw new NotImplementedException(),
                SCR.SCR_SETTINGS => throw new NotImplementedException(),
                SCR.SCR_ESCAPE_CAPSULE => throw new NotImplementedException(),
                SCR.SCR_MISSION_1 => throw new NotImplementedException(),
                SCR.SCR_MISSION_2 => throw new NotImplementedException(),
                SCR.SCR_UNDOCKING => throw new NotImplementedException(),
                SCR.SCR_HYPERSPACE => throw new NotImplementedException(),
                _ => LaserType.None,
            };

            _laserCounter = _laserStrength > 127 ? 0 : _laserStrength & 250;
            _laserStrength &= 127;

            _audio.PlayEffect(SoundEffect.Pulse);
            _gameState.LaserTemp += 8;
            if (_ship.Energy > 1)
            {
                _ship.Energy--;
            }

            return true;
        }

        internal void FireMissile()
        {
            if (MissileTarget < 0)
            {
                return;
            }

            Vector3[] rotmat = VectorMaths.GetInitialMatrix();
            rotmat[2].Z = 1;
            rotmat[0].X = -1;

            int newship = AddNewShip(ShipType.Missile, new(0, -28, 14), rotmat, 0, 0);

            if (newship == -1)
            {
                _gameState.InfoMessage("Missile Jammed");
                return;
            }

            Space.s_universe[newship].Velocity = _ship.Speed * 2;
            Space.s_universe[newship].Flags = ShipFlags.Angry;
            Space.s_universe[newship].Target = MissileTarget;

            if (Space.s_universe[MissileTarget].Type > ShipType.Rock)
            {
                Space.s_universe[MissileTarget].Flags |= ShipFlags.Angry;
            }

            _ship.MissileCount--;
            MissileTarget = IsMissileUnarmed;

            _audio.PlayEffect(SoundEffect.Missile);
        }

        internal void RandomEncounter()
        {
            if (Space.s_ship_count[ShipType.Coriolis] != 0 || Space.s_ship_count[ShipType.Dodec] != 0)
            {
                return;
            }

            if (RNG.Random(255) == 136)
            {
                if (((int)Space.s_universe[0].Location.Z & 0x3e) != 0)
                {
                    CreateThargoid();
                }
                else
                {
                    CreateCougar();
                }

                return;
            }

            if (RNG.Random(7) == 0)
            {
                CreateTrader();
                return;
            }

            CheckForAsteroids();

            CheckForPolice();

            if (Space.s_ship_count[ShipType.Viper] != 0)
            {
                return;
            }

            if (InBattle)
            {
                return;
            }

            if (_gameState.Cmdr.Mission == 5 && RNG.Random(255) >= 200)
            {
                CreateThargoid();
            }

            CheckForOthers();
        }

        internal void RemoveShip(int un)
        {
            ShipType type = Space.s_universe[un].Type;

            if (type == ShipType.None)
            {
                return;
            }

            if (type > ShipType.None)
            {
                Space.s_ship_count[type]--;
            }

            Space.s_universe[un].Type = ShipType.None;

            CheckMissiles(un);

            if (type is ShipType.Coriolis or ShipType.Dodec)
            {
                Vector3 position = Space.s_universe[un].Location;
                position.Y = (int)position.Y & 0xFFFF;
                position.Y = (int)position.Y | 0x60000;
                AddNewShip(ShipType.Sun, position, VectorMaths.GetInitialMatrix(), 0, 0);
            }
        }

        internal void ResetWeapons()
        {
            _gameState.LaserTemp = 0;
            _laserCounter = 0;
            _laserStrength = 0;
            _laserType = LaserType.None;
            _ship.EcmActive = 0;
            MissileTarget = IsMissileUnarmed;
        }

        internal void ScoopItem(int un)
        {
            if (Space.s_universe[un].Flags.HasFlag(ShipFlags.Dead))
            {
                return;
            }

            ShipType type = Space.s_universe[un].Type;

            if (type == ShipType.Missile)
            {
                return;
            }

            if (!_ship.HasFuelScoop || Space.s_universe[un].Location.Y >= 0 ||
                _trade.TotalCargoTonnage() == _ship.CargoCapacity)
            {
                ExplodeObject(un);
                _ship.DamageShip(128 + (Space.s_universe[un].Energy / 2), Space.s_universe[un].Location.Z > 0);
                return;
            }

            if (type == ShipType.Cargo)
            {
                StockType trade = (StockType)RNG.Random(1, 8);
                _trade.AddCargo(trade);
                _gameState.InfoMessage(_trade._stockMarket[trade].Name);
                RemoveShip(un);
                return;
            }

            if (_gameState.ShipList[type].ScoopedType != StockType.None)
            {
                StockType trade = _gameState.ShipList[type].ScoopedType;
                _trade.AddCargo(trade);
                _gameState.InfoMessage(_trade._stockMarket[trade].Name);
                RemoveShip(un);
                return;
            }

            ExplodeObject(un);
            _ship.DamageShip(Space.s_universe[un].Energy / 2, Space.s_universe[un].Location.Z > 0);
        }

        internal void Tactics(int un)
        {
            int energy;
            int maxeng;
            Vector3 nvec;
            float cnt2 = 0.223f;
            float direction;
            int attacking;

            UniverseObject ship = Space.s_universe[un];
            ShipType type = ship.Type;
            ShipFlags flags = ship.Flags;

            if (type is ShipType.Planet or ShipType.Sun)
            {
                return;
            }

            if (flags.HasFlag(ShipFlags.Dead))
            {
                return;
            }

            if (flags.HasFlag(ShipFlags.Inactive))
            {
                return;
            }

            if (type == ShipType.Missile)
            {
                if (flags.HasFlag(ShipFlags.Angry))
                {
                    MissileTactics(ship);
                }

                return;
            }

            if (((un ^ _gameState.mcount) & 7) != 0)
            {
                return;
            }

            if (type is ShipType.Coriolis or ShipType.Dodec)
            {
                if (flags.HasFlag(ShipFlags.Angry))
                {
                    if (RNG.Random(255) < 240)
                    {
                        return;
                    }

                    if (Space.s_ship_count[ShipType.Viper] >= 4)
                    {
                        return;
                    }

                    LaunchEnemy(un, ShipType.Viper, ShipFlags.Angry | ShipFlags.HasECM, 113);
                    return;
                }

                LaunchShuttle();
                return;
            }

            if (type == ShipType.Hermit)
            {
                if (RNG.Random(255) > 200)
                {
                    LaunchEnemy(un, ShipType.Sidewinder + RNG.Random(3), ShipFlags.Angry | ShipFlags.HasECM, 113);
                    ship.Flags |= ShipFlags.Inactive;
                }

                return;
            }

            if (ship.Energy < _gameState.ShipList[type].EnergyMax)
            {
                ship.Energy++;
            }

            if (type == ShipType.Tharglet && Space.s_ship_count[ShipType.Thargoid] == 0)
            {
                ship.Flags = 0;
                ship.Velocity /= 2;
                return;
            }

            if (flags.HasFlag(ShipFlags.Slow))
            {
                if (RNG.Random(255) > 50)
                {
                    return;
                }
            }

            if (flags.HasFlag(ShipFlags.Police))
            {
                if (_gameState.Cmdr.LegalStatus >= 64)
                {
                    flags |= ShipFlags.Angry;
                    ship.Flags = flags;
                }
            }

            if (!flags.HasFlag(ShipFlags.Angry))
            {
                if (flags.HasFlag(ShipFlags.FlyToPlanet) || flags.HasFlag(ShipFlags.FlyToStation))
                {
                    Pilot.AutoPilotShip(ref Space.s_universe[un]);
                }

                return;
            }

            // If we get to here then the ship is angry so start attacking...
            if (Space.s_ship_count[ShipType.Coriolis] != 0 || Space.s_ship_count[ShipType.Dodec] != 0)
            {
                if (!flags.HasFlag(ShipFlags.Bold))
                {
                    ship.Bravery = 0;
                }
            }

            if (type == ShipType.Anaconda)
            {
                if (RNG.Random(255) > 200)
                {
                    LaunchEnemy(un, RNG.Random(255) > 100 ? ShipType.Worm : ShipType.Sidewinder, ShipFlags.Angry | ShipFlags.HasECM, 113);
                    return;
                }
            }

            if (RNG.Random(255) >= 250)
            {
                ship.RotZ = RNG.Random(255) | 0x68;
                if (ship.RotZ > 127)
                {
                    ship.RotZ = -((int)ship.RotZ & 127);
                }
            }

            maxeng = _gameState.ShipList[type].EnergyMax;
            energy = ship.Energy;

            if (energy < maxeng / 2)
            {
                if (energy < maxeng / 8 && RNG.Random(255) > 230 && type != ShipType.Thargoid)
                {
                    ship.Flags &= ~ShipFlags.Angry;
                    ship.Flags |= ShipFlags.Inactive;
                    LaunchEnemy(un, ShipType.EscapeCapsule, 0, 126);
                    return;
                }

                if (ship.Missiles != 0 && _ship.EcmActive == 0 && ship.Missiles >= RNG.Random(31))
                {
                    ship.Missiles--;
                    if (type == ShipType.Thargoid)
                    {
                        LaunchEnemy(un, ShipType.Tharglet, ShipFlags.Angry, ship.Bravery);
                    }
                    else
                    {
                        LaunchEnemy(un, ShipType.Missile, ShipFlags.Angry, 126);
                        _gameState.InfoMessage("INCOMING MISSILE");
                    }
                    return;
                }
            }

            nvec = VectorMaths.UnitVector(Space.s_universe[un].Location);
            direction = VectorMaths.VectorDotProduct(nvec, ship.Rotmat[2]);

            if (ship.Location.Length() < 8192 && direction <= -0.833 &&
                 _gameState.ShipList[type].LaserStrength != 0)
            {
                if (direction <= -0.917)
                {
                    ship.Flags |= ShipFlags.Firing | ShipFlags.Hostile;
                }

                if (direction <= -0.972)
                {
                    _ship.DamageShip(_gameState.ShipList[type].LaserStrength, ship.Location.Z >= 0.0);
                    ship.Acceleration--;
                    if ((ship.Location.Z >= 0.0 && _ship.ShieldFront == 0) ||
                        (ship.Location.Z < 0.0 && _ship.ShieldRear == 0))
                    {
                        _audio.PlayEffect(SoundEffect.IncomingFire2);
                    }
                    else
                    {
                        _audio.PlayEffect(SoundEffect.IncomingFire1);
                    }
                }
                else
                {
                    nvec.X = -nvec.X;
                    nvec.Y = -nvec.Y;
                    nvec.Z = -nvec.Z;
                    direction = -direction;
                    TrackObject(ref Space.s_universe[un], direction, nvec);
                }

                //		if ((fabs(ship.location.z) < 768) && (ship.bravery <= ((random.rand255() & 127) | 64)))
                if (MathF.Abs(ship.Location.Z) < 768f)
                {
                    ship.RotX = RNG.Random(135);
                    if (ship.RotX > 127)
                    {
                        ship.RotX = -((int)ship.RotX & 127);
                    }

                    ship.Acceleration = 3;
                    return;
                }

                ship.Acceleration = ship.Location.Length() < 8192 ? -1 : 3;

                return;
            }

            attacking = 0;

            if (MathF.Abs(ship.Location.Z) >= 768f ||
                MathF.Abs(ship.Location.X) >= 512f ||
                MathF.Abs(ship.Location.Y) >= 512f)
            {
                if (ship.Bravery > RNG.Random(127))
                {
                    attacking = 1;
                    nvec.X = -nvec.X;
                    nvec.Y = -nvec.Y;
                    nvec.Z = -nvec.Z;
                    direction = -direction;
                }
            }

            TrackObject(ref Space.s_universe[un], direction, nvec);

            if (attacking == 1 && ship.Location.Length() < 2048)
            {
                if (direction >= cnt2)
                {
                    ship.Acceleration = -1;
                    return;
                }

                if (ship.Velocity < 6)
                {
                    ship.Acceleration = 3;
                }
                else if (RNG.Random(255) >= 200)
                {
                    ship.Acceleration = -1;
                }

                return;
            }

            if (direction <= -0.167)
            {
                ship.Acceleration = -1;
                return;
            }

            if (direction >= cnt2)
            {
                ship.Acceleration = 3;
                return;
            }

            if (ship.Velocity < 6)
            {
                ship.Acceleration = 3;
            }
            else if (RNG.Random(255) >= 200)
            {
                ship.Acceleration = -1;
            }
        }

        internal void TimeECM()
        {
            if (_ship.EcmActive != 0)
            {
                _ship.EcmActive--;
                if (_isEcmOurs)
                {
                    _ship.DecreaseEnergy(-1);
                }
            }
        }

        internal void UnarmMissile()
        {
            MissileTarget = IsMissileUnarmed;
            _audio.PlayEffect(SoundEffect.Boop);
        }

        private bool IsInTarget(ShipType type, float x, float y, float z)
        {
            if (z < 0)
            {
                return false;
            }

            float size = _gameState.ShipList[type].Size;

            return (x * x) + (y * y) <= size;
        }

        private static void MakeAngry(int un)
        {
            ShipType type = Space.s_universe[un].Type;
            ShipFlags flags = Space.s_universe[un].Flags;

            if (flags.HasFlag(ShipFlags.Inactive))
            {
                return;
            }

            if (type is ShipType.Coriolis or ShipType.Dodec)
            {
                Space.s_universe[un].Flags |= ShipFlags.Angry;
                return;
            }

            if (type > ShipType.Rock)
            {
                Space.s_universe[un].RotX = 4;
                Space.s_universe[un].Acceleration = 2;
                Space.s_universe[un].Flags |= ShipFlags.Angry;
            }
        }

        private static void TrackObject(ref UniverseObject ship, float direction, Vector3 nvec)
        {
            int rat = 3;
            float rat2 = 0.111f;
            float dir = VectorMaths.VectorDotProduct(nvec, ship.Rotmat[1]);

            if (direction < -0.861)
            {
                ship.RotX = dir < 0 ? 7 : -7;
                ship.RotZ = 0;
                return;
            }

            ship.RotX = 0;

            if (MathF.Abs(dir) * 2 >= rat2)
            {
                ship.RotX = dir < 0 ? rat : -rat;
            }

            if (MathF.Abs(ship.RotZ) < 16)
            {
                dir = VectorMaths.VectorDotProduct(nvec, ship.Rotmat[0]);

                ship.RotZ = 0;

                if (MathF.Abs(dir) * 2 > rat2)
                {
                    ship.RotZ = dir < 0 ? rat : -rat;

                    if (ship.RotX < 0)
                    {
                        ship.RotZ = -ship.RotZ;
                    }
                }
            }
        }

        /// <summary>
        /// Check for a random asteroid encounter.
        /// </summary>
        private void CheckForAsteroids()
        {
            ShipType type;

            if (RNG.Random(255) >= 35 || Space.s_ship_count[ShipType.Asteroid] >= 3)
            {
                return;
            }

            type = RNG.Random(255) > 253 ? ShipType.Hermit : ShipType.Asteroid;

            int newship = CreateOtherShip(type);

            if (newship != -1)
            {
                //		space.universe[newship].velocity = (random.rand255() & 31) | 16; 
                Space.s_universe[newship].Velocity = 8;
                Space.s_universe[newship].RotZ = RNG.TrueOrFalse() ? -127 : 127;
                Space.s_universe[newship].RotX = 16;
            }
        }

        private void CheckForOthers()
        {
            int gov = _gameState.CurrentPlanetData.Government;
            int rnd = RNG.Random(255);

            if (gov != 0 && (rnd >= 90 || (rnd & 7) < gov))
            {
                return;
            }

            if (RNG.Random(255) < 100)
            {
                CreateLoneHunter();
                return;
            }

            // Pack hunters...
            Vector3 position = new()
            {
                Z = 12000,
                X = 1000 + RNG.Random(8191),
                Y = 1000 + RNG.Random(8191),
            };

            if (RNG.TrueOrFalse())
            {
                position.X = -position.X;
            }

            if (RNG.TrueOrFalse())
            {
                position.Y = -position.Y;
            }

            rnd = RNG.Random(3);

            for (int i = 0; i <= rnd; i++)
            {
                ShipType type = ShipType.Sidewinder + (RNG.Random(255) & RNG.Random(7));
                int newship = AddNewShip(type, position, VectorMaths.GetInitialMatrix(), 0, 0);
                if (newship != -1)
                {
                    Space.s_universe[newship].Flags = ShipFlags.Angry;
                    if (RNG.Random(255) > 245)
                    {
                        Space.s_universe[newship].Flags |= ShipFlags.HasECM;
                    }

                    Space.s_universe[newship].Bravery = ((RNG.Random(255) * 2) | 64) & 127;
                    InBattle = true;
                }
            }
        }

        private void CheckForPolice()
        {
            int offense = _trade.IsCarryingContraband() * 2;
            if (Space.s_ship_count[ShipType.Viper] == 0)
            {
                offense |= _gameState.Cmdr.LegalStatus;
            }

            if (RNG.Random(255) >= offense)
            {
                return;
            }

            int newship = CreateOtherShip(ShipType.Viper);

            if (newship != -1)
            {
                Space.s_universe[newship].Flags = ShipFlags.Angry;
                if (RNG.Random(255) > 245)
                {
                    Space.s_universe[newship].Flags |= ShipFlags.HasECM;
                }

                Space.s_universe[newship].Bravery = ((RNG.Random(255) * 2) | 64) & 127;
            }
        }

        private void CheckMissiles(int un)
        {
            if (MissileTarget == un)
            {
                MissileTarget = IsMissileUnarmed;
                _gameState.InfoMessage("Target Lost");
            }

            for (int i = 0; i < EliteMain.MAX_UNIV_OBJECTS; i++)
            {
                if (Space.s_universe[i].Type == ShipType.Missile && Space.s_universe[i].Target == un)
                {
                    Space.s_universe[i].Flags |= ShipFlags.Dead;
                }
            }
        }
        private void CreateCougar()
        {
            int newship;

            if (Space.s_ship_count[ShipType.Cougar] != 0)
            {
                return;
            }

            newship = CreateOtherShip(ShipType.Cougar);
            if (newship != -1)
            {
                Space.s_universe[newship].Flags = ShipFlags.HasECM; // | FLG_CLOAKED;
                Space.s_universe[newship].Bravery = 121;
                Space.s_universe[newship].Velocity = 18;
            }
        }

        private void CreateLoneHunter()
        {
            int rnd;
            ShipType type;
            int newship;

            if (_gameState.Cmdr.Mission == 1 && _gameState.Cmdr.GalaxyNumber == 1 &&
                _gameState.DockedPlanet.D == 144 && _gameState.DockedPlanet.B == 33 &&
                Space.s_ship_count[ShipType.Constrictor] == 0)
            {
                type = ShipType.Constrictor;
            }
            else
            {
                rnd = RNG.Random(255);
                type = ShipType.CobraMk3Lone + (rnd & 3) + (rnd > 127 ? 1 : 0);
            }

            newship = CreateOtherShip(type);

            if (newship != -1)
            {
                Space.s_universe[newship].Flags = ShipFlags.Angry;
                if (RNG.Random(255) > 200 || type == ShipType.Constrictor)
                {
                    Space.s_universe[newship].Flags |= ShipFlags.HasECM;
                }

                Space.s_universe[newship].Bravery = ((RNG.Random(255) * 2) | 64) & 127;
                InBattle = true;
            }
        }

        private int CreateOtherShip(ShipType type)
        {
            Vector3 position = new()
            {
                X = 1000 + RNG.Random(8191),
                Y = 1000 + RNG.Random(8191),
                Z = 12000,
            };

            if (RNG.Random(255) > 127)
            {
                position.X = -position.X;
            }

            if (RNG.Random(255) > 127)
            {
                position.Y = -position.Y;
            }

            int newship = AddNewShip(type, position, VectorMaths.GetInitialMatrix(), 0, 0);
            return newship;
        }

        private void CreateTrader()
        {
            ShipType type = ShipType.CobraMk3 + RNG.Random(3);
            int newship = CreateOtherShip(type);

            if (newship != -1)
            {
                Space.s_universe[newship].Rotmat[2].Z = -1.0f;
                Space.s_universe[newship].RotZ = RNG.Random(7);
                Space.s_universe[newship].Velocity = RNG.Random(31) | 16;
                Space.s_universe[newship].Bravery = RNG.Random(127);

                if (RNG.TrueOrFalse())
                {
                    Space.s_universe[newship].Flags |= ShipFlags.HasECM;
                }

                //		if (rnd & 2)
                //			space.universe[newship].flags |= FLG.FLG_ANGRY; 
            }
        }

        private void LaunchEnemy(int un, ShipType type, ShipFlags flags, int bravery)
        {
            Debug.Assert(Space.s_universe[un].Rotmat != null);
            Vector3[] rotmat = Space.s_universe[un].Rotmat.Cloner();
            int newship = AddNewShip(type, Space.s_universe[un].Location, rotmat, Space.s_universe[un].RotX, Space.s_universe[un].RotZ);

            if (newship == -1)
            {
                return;
            }

            UniverseObject ns = Space.s_universe[newship];

            if (Space.s_universe[un].Type is ShipType.Coriolis or ShipType.Dodec)
            {
                ns.Velocity = 32;
                ns.Location = new(ns.Location.X + (ns.Rotmat[2].X * 2), ns.Location.Y + (ns.Rotmat[2].Y * 2), ns.Location.Z + (ns.Rotmat[2].Z * 2));
            }

            ns.Flags |= flags;
            ns.RotZ /= 2;
            ns.RotZ *= 2;
            ns.Bravery = bravery;

            if (type is ShipType.Cargo or ShipType.Alloy or ShipType.Rock)
            {
                ns.RotZ = ((RNG.Random(255) * 2) & 255) - 128;
                ns.RotX = ((RNG.Random(255) * 2) & 255) - 128;
                ns.Velocity = RNG.Random(15);
            }
        }

        private void LaunchLoot(int un, ShipType loot)
        {
            int i, cnt;

            if (loot == ShipType.Rock)
            {
                cnt = RNG.Random(3);
            }
            else
            {
                cnt = RNG.Random(255);
                if (cnt >= 128)
                {
                    return;
                }

                cnt &= _gameState.ShipList[Space.s_universe[un].Type].LootMax;
                cnt &= 15;
            }

            for (i = 0; i < cnt; i++)
            {
                LaunchEnemy(un, loot, 0, 0);
            }
        }
        private void LaunchShuttle()
        {
            if (Space.s_ship_count[ShipType.Transporter] != 0 ||
                Space.s_ship_count[ShipType.Shuttle] != 0 ||
                RNG.Random(255) < 253 || _gameState.IsAutoPilotOn)
            {
                return;
            }

            ShipType type = RNG.TrueOrFalse() ? ShipType.Shuttle : ShipType.Transporter;
            LaunchEnemy(1, type, ShipFlags.HasECM | ShipFlags.FlyToPlanet, 113);
        }

        private void MissileTactics(UniverseObject missile)
        {
            UniverseObject target;
            Vector3 vec;
            Vector3 nvec;
            float direction;
            float cnt2 = 0.223f;

            if (_ship.EcmActive != 0)
            {
                _audio.PlayEffect(SoundEffect.Explode);

                missile.Flags |= ShipFlags.Dead;
                return;
            }

            if (missile.Target == 0)
            {
                if (missile.Location.Length() < 512)
                {
                    missile.Flags |= ShipFlags.Dead;
                    _audio.PlayEffect(SoundEffect.Explode);
                    _ship.DamageShip(250, missile.Location.Z >= 0.0);
                    return;
                }

                vec = missile.Location;
            }
            else
            {
                target = Space.s_universe[missile.Target];
                vec = missile.Location - target.Location;

                if (vec.Length() < 512)
                {
                    missile.Flags |= ShipFlags.Dead;

                    if (target.Type is not ShipType.Coriolis and not ShipType.Dodec)
                    {
                        ExplodeObject(missile.Target);
                    }
                    else
                    {
                        _audio.PlayEffect(SoundEffect.Explode);
                    }

                    return;
                }

                if (RNG.Random(255) < 16 && target.Flags.HasFlag(ShipFlags.HasECM))
                {
                    ActivateECM(false);
                    return;
                }
            }

            nvec = VectorMaths.UnitVector(vec);
            direction = VectorMaths.VectorDotProduct(nvec, missile.Rotmat[2]);
            nvec.X = -nvec.X;
            nvec.Y = -nvec.Y;
            nvec.Z = -nvec.Z;
            direction = -direction;

            TrackObject(ref missile, direction, nvec);

            if (direction <= -0.167)
            {
                missile.Acceleration = -2;
                return;
            }

            if (direction >= cnt2)
            {
                missile.Acceleration = 3;
                return;
            }

            if (missile.Velocity < 6)
            {
                missile.Acceleration = 3;
            }
            else if (RNG.Random(255) >= 200)
            {
                missile.Acceleration = -2;
            }

            return;
        }
    }
}