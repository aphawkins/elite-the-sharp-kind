using System.Diagnostics;
using System.Numerics;
using Elite.Common.Enums;
using Elite.Engine.Enums;
using Elite.Engine.Ships;
using Elite.Engine.Types;

namespace Elite.Engine
{
    internal class Combat
    {
        internal bool InBattle { get; set; }
        internal int _isMISSILE_ARMED = -1;
        internal int IsMissileUnarmed { get; private set; } = -2;
        internal int MissileTarget { get; private set; }
        private readonly Audio _audio;
        private readonly GameState _gameState;
        private readonly PlayerShip _ship;
        private readonly Trade _trade;
        private readonly FLG[] _initialFlags = new FLG[34]
        {
            0,											// NULL,
			0,											// missile 
			0,											// coriolis
			FLG.FLG_SLOW | FLG.FLG_FLY_TO_PLANET,				// escape
			FLG.FLG_INACTIVE,								// alloy
			FLG.FLG_INACTIVE,								// cargo
			FLG.FLG_INACTIVE,								// boulder
			FLG.FLG_INACTIVE,								// asteroid
			FLG.FLG_INACTIVE,								// rock
			FLG.FLG_FLY_TO_PLANET | FLG.FLG_SLOW,				// shuttle
			FLG.FLG_FLY_TO_PLANET | FLG.FLG_SLOW,				// transporter
			0,											// cobra3
			0,											// python
			0,											// boa
			FLG.FLG_SLOW,									// anaconda
			FLG.FLG_SLOW,									// hermit
			FLG.FLG_BOLD | FLG.FLG_POLICE,						// viper
			FLG.FLG_BOLD | FLG.FLG_ANGRY,						// sidewinder
			FLG.FLG_BOLD | FLG.FLG_ANGRY,						// mamba
			FLG.FLG_BOLD | FLG.FLG_ANGRY,						// krait
			FLG.FLG_BOLD | FLG.FLG_ANGRY,						// adder
			FLG.FLG_BOLD | FLG.FLG_ANGRY,						// gecko
			FLG.FLG_BOLD | FLG.FLG_ANGRY,						// cobra1
			FLG.FLG_SLOW | FLG.FLG_ANGRY,						// worm
			FLG.FLG_BOLD | FLG.FLG_ANGRY,						// cobra3
			FLG.FLG_BOLD | FLG.FLG_ANGRY,						// asp2
			FLG.FLG_BOLD | FLG.FLG_ANGRY,						// python
			FLG.FLG_POLICE,									// fer_de_lance
			FLG.FLG_BOLD | FLG.FLG_ANGRY,						// moray
			FLG.FLG_BOLD | FLG.FLG_ANGRY,						// thargoid
			FLG.FLG_ANGRY,									// tharlet
			FLG.FLG_ANGRY,									// constrictor
			FLG.FLG_POLICE | FLG.FLG_CLOAKED,					// cougar
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
                if (Space.universe[i].type == ShipType.None)
                {
                    Space.universe[i].type = ship_type;
                    Space.universe[i].location = location;
                    Space.universe[i].Rotmat = rotmat;
                    Space.universe[i].rotx = rotx;
                    Space.universe[i].rotz = rotz;
                    Space.universe[i].velocity = 0;
                    Space.universe[i].acceleration = 0;
                    Space.universe[i].bravery = 0;
                    Space.universe[i].target = 0;
                    Space.universe[i].flags = _initialFlags[(int)ship_type < 0 ? 0 : (int)ship_type];

                    if (ship_type is not ShipType.Planet and not ShipType.Sun)
                    {
                        Space.universe[i].energy = _gameState.ShipList[ship_type].EnergyMax;
                        Space.universe[i].missiles = _gameState.ShipList[ship_type].MissilesMax;
                        Space.ship_count[ship_type]++;
                    }

                    return i;
                }
            }

            return -1;
        }

        internal void AddNewStation(Vector3 position, Vector3[] rotmat)
        {
            ShipType station = (_gameState.CurrentPlanetData.TechLevel >= 10) ? ShipType.Dodec : ShipType.Coriolis;
            Space.universe[1].type = ShipType.None;
            AddNewShip(station, position, rotmat, 0, -127);
        }

        internal void ArmMissile()
        {
            if ((_ship.MissileCount != 0) && (MissileTarget == IsMissileUnarmed))
            {
                MissileTarget = _isMISSILE_ARMED;
            }
        }

        internal void CheckTarget(int un, ref UniverseObject flip)
        {
            //univ_object univ = space.universe[un];

            if (IsInTarget(Space.universe[un].type, flip.location.X, flip.location.Y, flip.location.Z))
            {
                if ((MissileTarget == _isMISSILE_ARMED) && (Space.universe[un].type >= 0))
                {
                    MissileTarget = un;
                    _gameState.InfoMessage("Target Locked");
                    _audio.PlayEffect(SoundEffect.Beep);
                }

                if (_laserStrength > 0)
                {
                    _audio.PlayEffect(SoundEffect.HitEnemy);

                    if (Space.universe[un].type is not ShipType.Coriolis and not ShipType.Dodec)
                    {
                        if (Space.universe[un].type is ShipType.Constrictor or ShipType.Cougar)
                        {
                            if (_laserType == LaserType.Military)
                            {
                                Space.universe[un].energy -= _laserStrength / 4;
                            }
                        }
                        else
                        {
                            Space.universe[un].energy -= _laserStrength;
                        }
                    }

                    if (Space.universe[un].energy <= 0)
                    {
                        ExplodeObject(un);

                        if (Space.universe[un].type == ShipType.Asteroid)
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
                Space.universe[i] = new()
                {
                    type = 0
                };
            }

            foreach (ShipType ship in _gameState.ShipList.Keys.ToList())
            {
                Space.ship_count[ship] = 0;
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
                Space.universe[newship].flags = FLG.FLG_ANGRY | FLG.FLG_HAS_ECM;
                Space.universe[newship].bravery = 113;

                if (RNG.Random(255) > 64)
                {
                    LaunchEnemy(newship, ShipType.Tharglet, FLG.FLG_ANGRY | FLG.FLG_HAS_ECM, 96);
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
            Space.universe[un].flags |= FLG.FLG_DEAD;

            if (Space.universe[un].type == ShipType.Constrictor)
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
                _ => LaserType.None,
            };

            _laserCounter = (_laserStrength > 127) ? 0 : (_laserStrength & 250);
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

            Space.universe[newship].velocity = _ship.Speed * 2;
            Space.universe[newship].flags = FLG.FLG_ANGRY;
            Space.universe[newship].target = MissileTarget;

            if (Space.universe[MissileTarget].type > ShipType.Rock)
            {
                Space.universe[MissileTarget].flags |= FLG.FLG_ANGRY;
            }

            _ship.MissileCount--;
            MissileTarget = IsMissileUnarmed;

            _audio.PlayEffect(SoundEffect.Missile);
        }

        internal void RandomEncounter()
        {
            if ((Space.ship_count[ShipType.Coriolis] != 0) || (Space.ship_count[ShipType.Dodec] != 0))
            {
                return;
            }

            if (RNG.Random(255) == 136)
            {
                if (((int)Space.universe[0].location.Z & 0x3e) != 0)
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

            if (Space.ship_count[ShipType.Viper] != 0)
            {
                return;
            }

            if (InBattle)
            {
                return;
            }

            if ((_gameState.Cmdr.Mission == 5) && (RNG.Random(255) >= 200))
            {
                CreateThargoid();
            }

            CheckForOthers();
        }

        internal void RemoveShip(int un)
        {
            ShipType type = Space.universe[un].type;

            if (type == ShipType.None)
            {
                return;
            }

            if (type > ShipType.None)
            {
                Space.ship_count[type]--;
            }

            Space.universe[un].type = ShipType.None;

            CheckMissiles(un);

            if (type is ShipType.Coriolis or ShipType.Dodec)
            {
                Vector3 position = Space.universe[un].location;
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
            if (Space.universe[un].flags.HasFlag(FLG.FLG_DEAD))
            {
                return;
            }

            ShipType type = Space.universe[un].type;

            if (type == ShipType.Missile)
            {
                return;
            }

            if ((!_ship.HasFuelScoop) || (Space.universe[un].location.Y >= 0) ||
                (_trade.TotalCargoTonnage() == _ship.CargoCapacity))
            {
                ExplodeObject(un);
                _ship.DamageShip(128 + (Space.universe[un].energy / 2), Space.universe[un].location.Z > 0);
                return;
            }

            if (type == ShipType.Cargo)
            {
                StockType trade = (StockType)RNG.Random(1, 8);
                _trade.AddCargo(trade);
                _gameState.InfoMessage(_trade._stockMarket[trade].name);
                RemoveShip(un);
                return;
            }

            if (_gameState.ShipList[type].ScoopedType != StockType.None)
            {
                StockType trade = _gameState.ShipList[type].ScoopedType;
                _trade.AddCargo(trade);
                _gameState.InfoMessage(_trade._stockMarket[trade].name);
                RemoveShip(un);
                return;
            }

            ExplodeObject(un);
            _ship.DamageShip(Space.universe[un].energy / 2, Space.universe[un].location.Z > 0);
        }

        internal void Tactics(int un)
        {
            int energy;
            int maxeng;
            Vector3 nvec;
            float cnt2 = 0.223f;
            float direction;
            int attacking;

            UniverseObject ship = Space.universe[un];
            ShipType type = ship.type;
            FLG flags = ship.flags;

            if (type is ShipType.Planet or ShipType.Sun)
            {
                return;
            }

            if (flags.HasFlag(FLG.FLG_DEAD))
            {
                return;
            }

            if (flags.HasFlag(FLG.FLG_INACTIVE))
            {
                return;
            }

            if (type == ShipType.Missile)
            {
                if (flags.HasFlag(FLG.FLG_ANGRY))
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
                if (flags.HasFlag(FLG.FLG_ANGRY))
                {
                    if (RNG.Random(255) < 240)
                    {
                        return;
                    }

                    if (Space.ship_count[ShipType.Viper] >= 4)
                    {
                        return;
                    }

                    LaunchEnemy(un, ShipType.Viper, FLG.FLG_ANGRY | FLG.FLG_HAS_ECM, 113);
                    return;
                }

                LaunchShuttle();
                return;
            }

            if (type == ShipType.Hermit)
            {
                if (RNG.Random(255) > 200)
                {
                    LaunchEnemy(un, ShipType.Sidewinder + RNG.Random(3), FLG.FLG_ANGRY | FLG.FLG_HAS_ECM, 113);
                    ship.flags |= FLG.FLG_INACTIVE;
                }

                return;
            }

            if (ship.energy < _gameState.ShipList[type].EnergyMax)
            {
                ship.energy++;
            }

            if ((type == ShipType.Tharglet) && (Space.ship_count[ShipType.Thargoid] == 0))
            {
                ship.flags = 0;
                ship.velocity /= 2;
                return;
            }

            if (flags.HasFlag(FLG.FLG_SLOW))
            {
                if (RNG.Random(255) > 50)
                {
                    return;
                }
            }

            if (flags.HasFlag(FLG.FLG_POLICE))
            {
                if (_gameState.Cmdr.LegalStatus >= 64)
                {
                    flags |= FLG.FLG_ANGRY;
                    ship.flags = flags;
                }
            }

            if (!flags.HasFlag(FLG.FLG_ANGRY))
            {
                if (flags.HasFlag(FLG.FLG_FLY_TO_PLANET) || flags.HasFlag(FLG.FLG_FLY_TO_STATION))
                {
                    Pilot.AutoPilotShip(ref Space.universe[un]);
                }

                return;
            }


            /* If we get to here then the ship is angry so start attacking... */

            if (Space.ship_count[ShipType.Coriolis] != 0 || Space.ship_count[ShipType.Dodec] != 0)
            {
                if (!flags.HasFlag(FLG.FLG_BOLD))
                {
                    ship.bravery = 0;
                }
            }

            if (type == ShipType.Anaconda)
            {
                if (RNG.Random(255) > 200)
                {
                    LaunchEnemy(un, RNG.Random(255) > 100 ? ShipType.Worm : ShipType.Sidewinder, FLG.FLG_ANGRY | FLG.FLG_HAS_ECM, 113);
                    return;
                }
            }

            if (RNG.Random(255) >= 250)
            {
                ship.rotz = RNG.Random(255) | 0x68;
                if (ship.rotz > 127)
                {
                    ship.rotz = -((int)ship.rotz & 127);
                }
            }

            maxeng = _gameState.ShipList[type].EnergyMax;
            energy = ship.energy;

            if (energy < (maxeng / 2))
            {
                if ((energy < (maxeng / 8)) && (RNG.Random(255) > 230) && (type != ShipType.Thargoid))
                {
                    ship.flags &= ~FLG.FLG_ANGRY;
                    ship.flags |= FLG.FLG_INACTIVE;
                    LaunchEnemy(un, ShipType.EscapeCapsule, 0, 126);
                    return;
                }

                if ((ship.missiles != 0) && (_ship.EcmActive == 0) && (ship.missiles >= RNG.Random(31)))
                {
                    ship.missiles--;
                    if (type == ShipType.Thargoid)
                    {
                        LaunchEnemy(un, ShipType.Tharglet, FLG.FLG_ANGRY, ship.bravery);
                    }
                    else
                    {
                        LaunchEnemy(un, ShipType.Missile, FLG.FLG_ANGRY, 126);
                        _gameState.InfoMessage("INCOMING MISSILE");
                    }
                    return;
                }
            }

            nvec = VectorMaths.UnitVector(Space.universe[un].location);
            direction = VectorMaths.VectorDotProduct(nvec, ship.Rotmat[2]);

            if ((ship.location.Length() < 8192) && (direction <= -0.833) &&
                 (_gameState.ShipList[type].LaserStrength != 0))
            {
                if (direction <= -0.917)
                {
                    ship.flags |= FLG.FLG_FIRING | FLG.FLG_HOSTILE;
                }

                if (direction <= -0.972)
                {
                    _ship.DamageShip(_gameState.ShipList[type].LaserStrength, ship.location.Z >= 0.0);
                    ship.acceleration--;
                    if (((ship.location.Z >= 0.0) && (_ship.ShieldFront == 0)) ||
                        ((ship.location.Z < 0.0) && (_ship.ShieldRear == 0)))
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
                    TrackObject(ref Space.universe[un], direction, nvec);
                }

                //		if ((fabs(ship.location.z) < 768) && (ship.bravery <= ((random.rand255() & 127) | 64)))
                if (MathF.Abs(ship.location.Z) < 768f)
                {
                    ship.rotx = RNG.Random(135);
                    if (ship.rotx > 127)
                    {
                        ship.rotx = -((int)ship.rotx & 127);
                    }

                    ship.acceleration = 3;
                    return;
                }

                ship.acceleration = ship.location.Length() < 8192 ? -1 : 3;

                return;
            }

            attacking = 0;

            if ((MathF.Abs(ship.location.Z) >= 768f) ||
                (MathF.Abs(ship.location.X) >= 512f) ||
                (MathF.Abs(ship.location.Y) >= 512f))
            {
                if (ship.bravery > RNG.Random(127))
                {
                    attacking = 1;
                    nvec.X = -nvec.X;
                    nvec.Y = -nvec.Y;
                    nvec.Z = -nvec.Z;
                    direction = -direction;
                }
            }

            TrackObject(ref Space.universe[un], direction, nvec);

            if ((attacking == 1) && (ship.location.Length() < 2048))
            {
                if (direction >= cnt2)
                {
                    ship.acceleration = -1;
                    return;
                }

                if (ship.velocity < 6)
                {
                    ship.acceleration = 3;
                }
                else if (RNG.Random(255) >= 200)
                {
                    ship.acceleration = -1;
                }

                return;
            }

            if (direction <= -0.167)
            {
                ship.acceleration = -1;
                return;
            }

            if (direction >= cnt2)
            {
                ship.acceleration = 3;
                return;
            }

            if (ship.velocity < 6)
            {
                ship.acceleration = 3;
            }
            else if (RNG.Random(255) >= 200)
            {
                ship.acceleration = -1;
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

            return ((x * x) + (y * y)) <= size;
        }

        private static void MakeAngry(int un)
        {
            ShipType type = Space.universe[un].type;
            FLG flags = Space.universe[un].flags;

            if (flags.HasFlag(FLG.FLG_INACTIVE))
            {
                return;
            }

            if (type is ShipType.Coriolis or ShipType.Dodec)
            {
                Space.universe[un].flags |= FLG.FLG_ANGRY;
                return;
            }

            if (type > ShipType.Rock)
            {
                Space.universe[un].rotx = 4;
                Space.universe[un].acceleration = 2;
                Space.universe[un].flags |= FLG.FLG_ANGRY;
            }
        }

        private static void TrackObject(ref UniverseObject ship, float direction, Vector3 nvec)
        {
            int rat = 3;
            float rat2 = 0.111f;
            float dir = VectorMaths.VectorDotProduct(nvec, ship.Rotmat[1]);

            if (direction < -0.861)
            {
                ship.rotx = (dir < 0) ? 7 : -7;
                ship.rotz = 0;
                return;
            }

            ship.rotx = 0;

            if ((MathF.Abs(dir) * 2) >= rat2)
            {
                ship.rotx = (dir < 0) ? rat : -rat;
            }

            if (MathF.Abs(ship.rotz) < 16)
            {
                dir = VectorMaths.VectorDotProduct(nvec, ship.Rotmat[0]);

                ship.rotz = 0;

                if ((MathF.Abs(dir) * 2) > rat2)
                {
                    ship.rotz = (dir < 0) ? rat : -rat;

                    if (ship.rotx < 0)
                    {
                        ship.rotz = -ship.rotz;
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

            if ((RNG.Random(255) >= 35) || (Space.ship_count[ShipType.Asteroid] >= 3))
            {
                return;
            }

            type = RNG.Random(255) > 253 ? ShipType.Hermit : ShipType.Asteroid;

            int newship = CreateOtherShip(type);

            if (newship != -1)
            {
                //		space.universe[newship].velocity = (random.rand255() & 31) | 16; 
                Space.universe[newship].velocity = 8;
                Space.universe[newship].rotz = RNG.TrueOrFalse() ? -127 : 127;
                Space.universe[newship].rotx = 16;
            }
        }

        private void CheckForOthers()
        {
            int gov = _gameState.CurrentPlanetData.Government;
            int rnd = RNG.Random(255);

            if ((gov != 0) && ((rnd >= 90) || ((rnd & 7) < gov)))
            {
                return;
            }

            if (RNG.Random(255) < 100)
            {
                CreateLoneHunter();
                return;
            }

            /* Pack hunters... */
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
                    Space.universe[newship].flags = FLG.FLG_ANGRY;
                    if (RNG.Random(255) > 245)
                    {
                        Space.universe[newship].flags |= FLG.FLG_HAS_ECM;
                    }

                    Space.universe[newship].bravery = ((RNG.Random(255) * 2) | 64) & 127;
                    InBattle = true;
                }
            }
        }

        private void CheckForPolice()
        {
            int offense = _trade.IsCarryingContraband() * 2;
            if (Space.ship_count[ShipType.Viper] == 0)
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
                Space.universe[newship].flags = FLG.FLG_ANGRY;
                if (RNG.Random(255) > 245)
                {
                    Space.universe[newship].flags |= FLG.FLG_HAS_ECM;
                }

                Space.universe[newship].bravery = ((RNG.Random(255) * 2) | 64) & 127;
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
                if ((Space.universe[i].type == ShipType.Missile) && (Space.universe[i].target == un))
                {
                    Space.universe[i].flags |= FLG.FLG_DEAD;
                }
            }
        }
        private void CreateCougar()
        {
            int newship;

            if (Space.ship_count[ShipType.Cougar] != 0)
            {
                return;
            }

            newship = CreateOtherShip(ShipType.Cougar);
            if (newship != -1)
            {
                Space.universe[newship].flags = FLG.FLG_HAS_ECM; // | FLG_CLOAKED;
                Space.universe[newship].bravery = 121;
                Space.universe[newship].velocity = 18;
            }
        }

        private void CreateLoneHunter()
        {
            int rnd;
            ShipType type;
            int newship;

            if ((_gameState.Cmdr.Mission == 1) && (_gameState.Cmdr.GalaxyNumber == 1) &&
                (_gameState.DockedPlanet.D == 144) && (_gameState.DockedPlanet.B == 33) &&
                (Space.ship_count[ShipType.Constrictor] == 0))
            {
                type = ShipType.Constrictor;
            }
            else
            {
                rnd = RNG.Random(255);
                type = ShipType.CobraMk3Lone + (rnd & 3) + ((rnd > 127) ? 1 : 0);
            }

            newship = CreateOtherShip(type);

            if (newship != -1)
            {
                Space.universe[newship].flags = FLG.FLG_ANGRY;
                if ((RNG.Random(255) > 200) || (type == ShipType.Constrictor))
                {
                    Space.universe[newship].flags |= FLG.FLG_HAS_ECM;
                }

                Space.universe[newship].bravery = ((RNG.Random(255) * 2) | 64) & 127;
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
                Space.universe[newship].Rotmat[2].Z = -1.0f;
                Space.universe[newship].rotz = RNG.Random(7);
                Space.universe[newship].velocity = RNG.Random(31) | 16;
                Space.universe[newship].bravery = RNG.Random(127);

                if (RNG.TrueOrFalse())
                {
                    Space.universe[newship].flags |= FLG.FLG_HAS_ECM;
                }

                //		if (rnd & 2)
                //			space.universe[newship].flags |= FLG.FLG_ANGRY; 
            }
        }

        private void LaunchEnemy(int un, ShipType type, FLG flags, int bravery)
        {
            Debug.Assert(Space.universe[un].Rotmat != null);
            Vector3[] rotmat = Space.universe[un].Rotmat.Cloner();
            int newship = AddNewShip(type, Space.universe[un].location, rotmat, Space.universe[un].rotx, Space.universe[un].rotz);

            if (newship == -1)
            {
                return;
            }

            UniverseObject ns = Space.universe[newship];

            if (Space.universe[un].type is ShipType.Coriolis or ShipType.Dodec)
            {
                ns.velocity = 32;
                ns.location.X += ns.Rotmat[2].X * 2;
                ns.location.Y += ns.Rotmat[2].Y * 2;
                ns.location.Z += ns.Rotmat[2].Z * 2;
            }

            ns.flags |= flags;
            ns.rotz /= 2;
            ns.rotz *= 2;
            ns.bravery = bravery;

            if (type is ShipType.Cargo or ShipType.Alloy or ShipType.Rock)
            {
                ns.rotz = ((RNG.Random(255) * 2) & 255) - 128;
                ns.rotx = ((RNG.Random(255) * 2) & 255) - 128;
                ns.velocity = RNG.Random(15);
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

                cnt &= _gameState.ShipList[Space.universe[un].type].LootMax;
                cnt &= 15;
            }

            for (i = 0; i < cnt; i++)
            {
                LaunchEnemy(un, loot, 0, 0);
            }
        }
        private void LaunchShuttle()
        {
            if ((Space.ship_count[ShipType.Transporter] != 0) ||
                (Space.ship_count[ShipType.Shuttle] != 0) ||
                (RNG.Random(255) < 253) || _gameState.IsAutoPilotOn)
            {
                return;
            }

            ShipType type = RNG.TrueOrFalse() ? ShipType.Shuttle : ShipType.Transporter;
            LaunchEnemy(1, type, FLG.FLG_HAS_ECM | FLG.FLG_FLY_TO_PLANET, 113);
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

                missile.flags |= FLG.FLG_DEAD;
                return;
            }

            if (missile.target == 0)
            {
                if (missile.location.Length() < 512)
                {
                    missile.flags |= FLG.FLG_DEAD;
                    _audio.PlayEffect(SoundEffect.Explode);
                    _ship.DamageShip(250, missile.location.Z >= 0.0);
                    return;
                }

                vec = missile.location;
            }
            else
            {
                target = Space.universe[missile.target];
                vec = missile.location - target.location;

                if (vec.Length() < 512)
                {
                    missile.flags |= FLG.FLG_DEAD;

                    if (target.type is not ShipType.Coriolis and not ShipType.Dodec)
                    {
                        ExplodeObject(missile.target);
                    }
                    else
                    {
                        _audio.PlayEffect(SoundEffect.Explode);
                    }

                    return;
                }

                if ((RNG.Random(255) < 16) && target.flags.HasFlag(FLG.FLG_HAS_ECM))
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
                missile.acceleration = -2;
                return;
            }

            if (direction >= cnt2)
            {
                missile.acceleration = 3;
                return;
            }

            if (missile.velocity < 6)
            {
                missile.acceleration = 3;
            }
            else if (RNG.Random(255) >= 200)
            {
                missile.acceleration = -2;
            }

            return;
        }
    }
}
