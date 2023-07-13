// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Diagnostics;
using System.Numerics;
using EliteSharp.Audio;
using EliteSharp.Graphics;
using EliteSharp.Lasers;
using EliteSharp.Ships;
using EliteSharp.Trader;
using EliteSharp.Views;

namespace EliteSharp.Conflict
{
    internal sealed class Combat
    {
        private readonly AudioController _audio;
        private readonly GameState _gameState;
        private readonly Pilot _pilot;
        private readonly PlayerShip _ship;
        private readonly Trade _trade;
        private readonly Universe _universe;
        private readonly IDraw _draw;
        private bool _isEcmOurs;
        private int _laserCounter;
        private int _laserStrength;
        private LaserType _laserType;

        internal Combat(GameState gameState, AudioController audio, PlayerShip ship, Trade trade, Pilot pilot, Universe universe, IDraw draw)
        {
            _gameState = gameState;
            _audio = audio;
            _ship = ship;
            _trade = trade;
            _pilot = pilot;
            _universe = universe;
            _draw = draw;
        }

        internal bool InBattle { get; set; }

        internal bool IsMissileArmed { get; private set; }

        internal IObject? MissileTarget { get; private set; }

        internal void ActivateECM(bool ours)
        {
            if (_ship.EcmActive == 0)
            {
                _ship.EcmActive = 32;
                _isEcmOurs = ours;
                _audio.PlayEffect(SoundEffect.Ecm);
            }
        }

        internal void ArmMissile()
        {
            if (_ship.MissileCount != 0)
            {
                IsMissileArmed = true;
            }
        }

        internal void CheckTarget(IShip obj, IObject flip)
        {
            //univ_object univ = space.universe[un];
            if (IsInTarget(obj, flip.Location))
            {
                if (MissileTarget == null && IsMissileArmed && obj.Type >= 0)
                {
                    MissileTarget = obj;
                    _gameState.InfoMessage("Target Locked");
                    _audio.PlayEffect(SoundEffect.Beep);
                }

                if (_laserStrength > 0)
                {
                    _audio.PlayEffect(SoundEffect.HitEnemy);

                    if (!obj.Flags.HasFlag(ShipFlags.Station))
                    {
                        if (obj.Type is ShipType.Constrictor or ShipType.Cougar)
                        {
                            if (_laserType == LaserType.Military)
                            {
                                obj.Energy -= _laserStrength / 4;
                            }
                        }
                        else
                        {
                            obj.Energy -= _laserStrength;
                        }
                    }

                    if (obj.Energy <= 0)
                    {
                        ExplodeObject(obj);

                        if (obj.Type == ShipType.Asteroid)
                        {
                            if (_laserType is LaserType.Mining or LaserType.Pulse)
                            {
                                LaunchLoot(obj, ShipType.Rock);
                            }
                        }
                        else
                        {
                            LaunchLoot(obj, ShipType.Alloy);
                            LaunchLoot(obj, ShipType.Cargo);
                        }
                    }

                    MakeAngry(obj);
                }
            }
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
            IShip thargoid = new Thargoid(_draw);
            if (_universe.AddNewShip(thargoid))
            {
                thargoid.Flags = ShipFlags.Angry | ShipFlags.HasECM;
                thargoid.Bravery = 113;

                if (RNG.Random(256) > 64 && !LaunchEnemy(thargoid, new Tharglet(_draw), ShipFlags.Angry | ShipFlags.HasECM, 96))
                {
                    Debug.Fail("Failed to create Tharglet");
                }
            }
            else
            {
                Debug.Fail("Failed to create Thargoid");
            }
        }

        internal void ExplodeObject(IObject obj)
        {
            _gameState.Cmdr.Score++;

            if ((_gameState.Cmdr.Score & 255) == 0)
            {
                _gameState.InfoMessage("Right On Commander!");
            }

            _audio.PlayEffect(SoundEffect.Explode);
            obj.Flags |= ShipFlags.Dead;

            if (obj.Type == ShipType.Constrictor)
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
                Screen.FrontView => _ship.LaserFront.Strength,
                Screen.RearView => _ship.LaserRear.Strength,
                Screen.RightView => _ship.LaserRight.Strength,
                Screen.LeftView => _ship.LaserLeft.Strength,
                Screen.None => throw new NotImplementedException(),
                Screen.IntroOne => throw new NotImplementedException(),
                Screen.IntroTwo => throw new NotImplementedException(),
                Screen.GalacticChart => throw new NotImplementedException(),
                Screen.ShortRangeChart => throw new NotImplementedException(),
                Screen.PlanetData => throw new NotImplementedException(),
                Screen.MarketPrices => throw new NotImplementedException(),
                Screen.CommanderStatus => throw new NotImplementedException(),
                Screen.Docking => throw new NotImplementedException(),
                Screen.Inventory => throw new NotImplementedException(),
                Screen.EquipShip => throw new NotImplementedException(),
                Screen.Options => throw new NotImplementedException(),
                Screen.LoadCommander => throw new NotImplementedException(),
                Screen.SaveCommander => throw new NotImplementedException(),
                Screen.Quit => throw new NotImplementedException(),
                Screen.GameOver => throw new NotImplementedException(),
                Screen.Settings => throw new NotImplementedException(),
                Screen.EscapeCapsule => throw new NotImplementedException(),
                Screen.MissionOne => throw new NotImplementedException(),
                Screen.MissionTwo => throw new NotImplementedException(),
                Screen.Undocking => throw new NotImplementedException(),
                Screen.Hyperspace => throw new NotImplementedException(),
                _ => 0,
            };

            if (_laserStrength == 0)
            {
                return false;
            }

            _laserType = _gameState.CurrentScreen switch
            {
                Screen.FrontView => _ship.LaserFront.Type,
                Screen.RearView => _ship.LaserRear.Type,
                Screen.RightView => _ship.LaserRight.Type,
                Screen.LeftView => _ship.LaserLeft.Type,
                Screen.None => throw new NotImplementedException(),
                Screen.IntroOne => throw new NotImplementedException(),
                Screen.IntroTwo => throw new NotImplementedException(),
                Screen.GalacticChart => throw new NotImplementedException(),
                Screen.ShortRangeChart => throw new NotImplementedException(),
                Screen.PlanetData => throw new NotImplementedException(),
                Screen.MarketPrices => throw new NotImplementedException(),
                Screen.CommanderStatus => throw new NotImplementedException(),
                Screen.Docking => throw new NotImplementedException(),
                Screen.Inventory => throw new NotImplementedException(),
                Screen.EquipShip => throw new NotImplementedException(),
                Screen.Options => throw new NotImplementedException(),
                Screen.LoadCommander => throw new NotImplementedException(),
                Screen.SaveCommander => throw new NotImplementedException(),
                Screen.Quit => throw new NotImplementedException(),
                Screen.GameOver => throw new NotImplementedException(),
                Screen.Settings => throw new NotImplementedException(),
                Screen.EscapeCapsule => throw new NotImplementedException(),
                Screen.MissionOne => throw new NotImplementedException(),
                Screen.MissionTwo => throw new NotImplementedException(),
                Screen.Undocking => throw new NotImplementedException(),
                Screen.Hyperspace => throw new NotImplementedException(),
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
            if (MissileTarget == null || !IsMissileArmed)
            {
                return;
            }

            Vector3[] rotmat = VectorMaths.GetInitialMatrix();
            rotmat[2].Z = 1;
            rotmat[0].X = -1;

            IShip missile = new Missile(_draw);
            if (!_universe.AddNewShip(missile, new(0, -28, 14), rotmat, 0, 0))
            {
                _gameState.InfoMessage("Missile Jammed");
                return;
            }

            missile.Velocity = _ship.Speed * 2;
            missile.Flags = ShipFlags.Angry;
            missile.Target = MissileTarget;

            if (MissileTarget.Type > ShipType.Rock)
            {
                MissileTarget.Flags |= ShipFlags.Angry;
            }

            _ship.MissileCount--;
            IsMissileArmed = false;
            MissileTarget = null;

            _audio.PlayEffect(SoundEffect.Missile);
        }

        internal void RandomEncounter()
        {
            if (_universe.IsStationPresent)
            {
                return;
            }

            if (RNG.Random(256) == 136)
            {
                if (((int)_universe.Planet!.Location.Z & 0x3e) != 0)
                {
                    CreateThargoid();
                }
                else
                {
                    CreateCougar();
                }

                return;
            }

            if (RNG.Random(8) == 0)
            {
                CreateTrader();
                return;
            }

            CheckForAsteroids();

            CheckForPolice();

            if (_universe.PoliceCount != 0)
            {
                return;
            }

            if (InBattle)
            {
                return;
            }

            if (_gameState.Cmdr.Mission == 5 && RNG.Random(256) >= 200)
            {
                CreateThargoid();
            }

            CheckForOthers();
        }

        internal void RemoveShip(IObject obj)
        {
            if (obj.Type == ShipType.None)
            {
                return;
            }

            CheckMissiles(obj);

            if (obj.Flags.HasFlag(ShipFlags.Station))
            {
                Vector3 position = obj.Location;
                position.Y = (int)position.Y & 0xFFFF;
                position.Y = (int)position.Y | 0x60000;
                _universe.AddNewShip(new Sun(_draw), position, VectorMaths.GetInitialMatrix(), 0, 0);
            }

            _universe.RemoveShip(obj);
        }

        internal void Reset() => InBattle = false;

        internal void ResetWeapons()
        {
            _gameState.LaserTemp = 0;
            _laserCounter = 0;
            _laserStrength = 0;
            _laserType = LaserType.None;
            _ship.EcmActive = 0;
            MissileTarget = null;
        }

        internal void ScoopItem(IShip obj)
        {
            if (obj.Flags.HasFlag(ShipFlags.Dead))
            {
                return;
            }

            ShipType type = obj.Type;

            if (type == ShipType.Missile)
            {
                return;
            }

            if (!_ship.HasFuelScoop || obj.Location.Y >= 0 ||
                _trade.TotalCargoTonnage() == _ship.CargoCapacity)
            {
                ExplodeObject(obj);
                _ship.DamageShip(128 + (obj.Energy / 2), obj.Location.Z > 0);
                return;
            }

            if (type == ShipType.Cargo)
            {
                StockType trade = (StockType)RNG.Random(1, 9);
                _trade.AddCargo(trade);
                _gameState.InfoMessage(_trade.StockMarket[trade].Name);
                RemoveShip(obj);
                return;
            }

            if (obj.ScoopedType != StockType.None)
            {
                StockType trade = obj.ScoopedType;
                _trade.AddCargo(trade);
                _gameState.InfoMessage(_trade.StockMarket[trade].Name);
                RemoveShip(obj);
                return;
            }

            ExplodeObject(obj);
            _ship.DamageShip(obj.Energy / 2, obj.Location.Z > 0);
        }

        internal void Tactics(IShip ship, int un)
        {
            Vector3 nvec;
            const float cnt2 = 0.223f;
            float direction;
            int attacking;

            ShipFlags flags = ship.Flags;

            if (ship.Type == ShipType.Missile)
            {
                if (flags.HasFlag(ShipFlags.Angry))
                {
                    MissileTactics(ship);
                }

                return;
            }

            if (((un ^ _gameState.MCount) & 7) != 0)
            {
                return;
            }

            if (ship.Flags.HasFlag(ShipFlags.Station))
            {
                if (flags.HasFlag(ShipFlags.Angry))
                {
                    if (RNG.Random(256) < 240)
                    {
                        return;
                    }

                    if (_universe.PoliceCount >= 4)
                    {
                        return;
                    }

                    if (!LaunchEnemy(ship, new Viper(_draw), ShipFlags.Angry | ShipFlags.HasECM, 113))
                    {
                        Debug.Fail("Failed to create Police");
                    }

                    return;
                }

                LaunchShuttle();
                return;
            }

            if (ship.Type == ShipType.Hermit)
            {
                if (RNG.Random(256) > 200)
                {
                    IShip pirate = new ShipFactory(_draw).CreatePirate();
                    if (!LaunchEnemy(ship, pirate, ShipFlags.Angry | ShipFlags.HasECM, 113))
                    {
                        Debug.Fail("Failed to create Hermit Pirate");
                    }

                    ship.Flags |= ShipFlags.Inactive;
                }

                return;
            }

            if (ship.Energy < ship.EnergyMax)
            {
                ship.Energy++;
            }

            if (ship.Type == ShipType.Tharglet && _universe.ShipCount(ShipType.Thargoid) == 0)
            {
                ship.Flags = 0;
                ship.Velocity /= 2;
                return;
            }

            if (flags.HasFlag(ShipFlags.Slow) &&
                RNG.Random(256) > 50)
            {
                return;
            }

            if (flags.HasFlag(ShipFlags.Police) &&
                _gameState.Cmdr.LegalStatus >= 64)
            {
                flags |= ShipFlags.Angry;
                ship.Flags = flags;
            }

            if (!flags.HasFlag(ShipFlags.Angry))
            {
                if (flags.HasFlag(ShipFlags.FlyToPlanet) || flags.HasFlag(ShipFlags.FlyToStation))
                {
                    _pilot.AutoPilotShip(ship);
                }

                return;
            }

            // If we get to here then the ship is angry so start attacking...
            if (_universe.IsStationPresent && !flags.HasFlag(ShipFlags.Bold))
            {
                ship.Bravery = 0;
            }

            if (ship.Type == ShipType.Anaconda && RNG.Random(256) > 200)
            {
                IShip anacondaHunter = RNG.Random(256) > 100 ? new Worm(_draw) : new Sidewinder(_draw);
                if (!LaunchEnemy(ship, anacondaHunter, ShipFlags.Angry | ShipFlags.HasECM, 113))
                {
                    Debug.Fail("Failed to create Anaconda Hunter");
                }

                return;
            }

            if (RNG.Random(256) >= 250)
            {
                ship.RotZ = RNG.Random(256) | 0x68;
                if (ship.RotZ > 127)
                {
                    ship.RotZ = -((int)ship.RotZ & 127);
                }
            }

            if (ship.Energy < ship.EnergyMax / 2)
            {
                if (ship.Energy < ship.EnergyMax / 8 && RNG.Random(256) > 230 && ship.Type != ShipType.Thargoid)
                {
                    ship.Flags &= ~ShipFlags.Angry;
                    ship.Flags |= ShipFlags.Inactive;
                    if (!LaunchEnemy(ship, new EscapeCapsule(_draw), 0, 126))
                    {
                        Debug.Fail("Failed to create Escape Capsule");
                    }

                    return;
                }

                if (ship.Missiles != 0 && _ship.EcmActive == 0 && ship.Missiles >= RNG.Random(32))
                {
                    ship.Missiles--;
                    if (ship.Type == ShipType.Thargoid)
                    {
                        if (!LaunchEnemy(ship, new Tharglet(_draw), ShipFlags.Angry, ship.Bravery))
                        {
                            Debug.Fail("Failed to create Tharglet");
                        }
                    }
                    else
                    {
                        if (!LaunchEnemy(ship, new Missile(_draw), ShipFlags.Angry, 126))
                        {
                            Debug.Fail("Failed to create Missile");
                        }

                        _gameState.InfoMessage("INCOMING MISSILE");
                    }

                    return;
                }
            }

            nvec = VectorMaths.UnitVector(ship.Location);
            direction = VectorMaths.VectorDotProduct(nvec, ship.Rotmat[2]);

            if (ship.Location.Length() < 8192 &&
                direction <= -0.833 &&
                ship.LaserStrength != 0)
            {
                if (direction <= -0.917)
                {
                    ship.Flags |= ShipFlags.Firing | ShipFlags.Hostile;
                }

                if (direction <= -0.972)
                {
                    _ship.DamageShip(ship.LaserStrength, ship.Location.Z >= 0.0);
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
                    TrackObject(ship, direction, nvec);
                }

                //      if ((fabs(ship.location.z) < 768) && (ship.bravery <= ((random.rand255() & 127) | 64)))
                if (MathF.Abs(ship.Location.Z) < 768f)
                {
                    ship.RotX = RNG.Random(136);
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

            if (MathF.Abs(ship.Location.Z) >= 768 ||
                MathF.Abs(ship.Location.X) >= 512 ||
                MathF.Abs(ship.Location.Y) >= 512)
            {
                if (ship.Bravery > RNG.Random(128))
                {
                    attacking = 1;
                    nvec.X = -nvec.X;
                    nvec.Y = -nvec.Y;
                    nvec.Z = -nvec.Z;
                    direction = -direction;
                }
            }

            TrackObject(ship, direction, nvec);

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
                else if (RNG.Random(256) >= 200)
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
            else if (RNG.Random(256) >= 200)
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
            MissileTarget = null;
            IsMissileArmed = false;
            _audio.PlayEffect(SoundEffect.Boop);
        }

        private static bool IsInTarget(IShip ship, Vector3 position) =>
            position.Z >= 0 && (position.X * position.X) + (position.Y * position.Y) <= ship.Size;

        private static void MakeAngry(IShip ship)
        {
            if (ship.Flags.HasFlag(ShipFlags.Inactive))
            {
                return;
            }

            if (ship.Flags.HasFlag(ShipFlags.Station))
            {
                ship.Flags |= ShipFlags.Angry;
                return;
            }

            if (ship.Type > ShipType.Rock)
            {
                ship.RotX = 4;
                ship.Acceleration = 2;
                ship.Flags |= ShipFlags.Angry;
            }
        }

        private static void TrackObject(IShip ship, float direction, Vector3 nvec)
        {
            const int rat = 3;
            const float rat2 = 0.111f;
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
            if (RNG.Random(256) >= 35 || _universe.ShipCount(ShipType.Asteroid) >= 3)
            {
                return;
            }

            IShip asteroid = new ShipFactory(_draw).CreateAsteroid();
            if (_universe.AddNewShip(asteroid))
            {
                //      space.universe[newship].velocity = (random.rand255() & 31) | 16;
                asteroid.Velocity = 8;
                asteroid.RotZ = RNG.TrueOrFalse() ? -127 : 127;
                asteroid.RotX = 16;
            }
            else
            {
                Debug.Fail("Failed to create Asteroid");
            }
        }

        private void CheckForOthers()
        {
            int gov = _gameState.CurrentPlanetData.Government;
            int rnd = RNG.Random(256);

            if (gov != 0 && (rnd >= 90 || (rnd & 7) < gov))
            {
                return;
            }

            if (RNG.Random(256) < 100)
            {
                CreateLoneWolf();
                return;
            }

            // Pack hunters...
            Vector3 position = new()
            {
                Z = 12000,
                X = 1000 + RNG.Random(8192),
                Y = 1000 + RNG.Random(8192),
            };

            if (RNG.TrueOrFalse())
            {
                position.X = -position.X;
            }

            if (RNG.TrueOrFalse())
            {
                position.Y = -position.Y;
            }

            rnd = RNG.Random(4);

            for (int i = 0; i <= rnd; i++)
            {
                IShip packHunter = new ShipFactory(_draw).CreatePackHunter();
                if (_universe.AddNewShip(packHunter, position, VectorMaths.GetInitialMatrix(), 0, 0))
                {
                    packHunter.Flags = ShipFlags.Angry;
                    if (RNG.Random(256) > 245)
                    {
                        packHunter.Flags |= ShipFlags.HasECM;
                    }

                    packHunter.Bravery = ((RNG.Random(256) * 2) | 64) & 127;
                    InBattle = true;
                }
                else
                {
                    Debug.Fail("Failed to create Pack Hunter");
                }
            }
        }

        private void CheckForPolice()
        {
            int offense = _trade.IsCarryingContraband() * 2;
            if (_universe.PoliceCount == 0)
            {
                offense |= _gameState.Cmdr.LegalStatus;
            }

            if (RNG.Random(256) >= offense)
            {
                return;
            }

            IShip police = new Viper(_draw);

            if (_universe.AddNewShip(police))
            {
                police.Flags = ShipFlags.Angry;
                if (RNG.Random(256) > 245)
                {
                    police.Flags |= ShipFlags.HasECM;
                }

                police.Bravery = ((RNG.Random(256) * 2) | 64) & 127;
            }
            else
            {
                Debug.Fail("Failed to create Police");
            }
        }

        private void CheckMissiles(IObject obj)
        {
            if (MissileTarget == obj)
            {
                MissileTarget = null;
                IsMissileArmed = false;
                _gameState.InfoMessage("Target Lost");
            }

            foreach (IObject universeObj in _universe.GetAllObjects())
            {
                if (universeObj.Type == ShipType.Missile && ((IShip)universeObj).Target == obj)
                {
                    universeObj.Flags |= ShipFlags.Dead;
                }
            }
        }

        private void CreateCougar()
        {
            if (_universe.ShipCount(ShipType.Cougar) != 0)
            {
                return;
            }

            IShip cougar = new Cougar(_draw);
            if (_universe.AddNewShip(cougar))
            {
                cougar.Flags = ShipFlags.HasECM; // | FLG_CLOAKED;
                cougar.Bravery = 121;
                cougar.Velocity = 18;
            }
            else
            {
                Debug.Fail("Failed to create Cougar");
            }
        }

        private void CreateLoneWolf()
        {
            IShip loneWolf = _gameState.Cmdr.Mission == 1 && _gameState.Cmdr.GalaxyNumber == 1 &&
                _gameState.DockedPlanet.D == 144 && _gameState.DockedPlanet.B == 33 &&
                _universe.ShipCount(ShipType.Constrictor) == 0
                ? new Constrictor(_draw)
                : new ShipFactory(_draw).CreateLoneWolf();

            if (_universe.AddNewShip(loneWolf))
            {
                loneWolf.Flags = ShipFlags.Angry;
                if (RNG.Random(256) > 200 || loneWolf.Type == ShipType.Constrictor)
                {
                    loneWolf.Flags |= ShipFlags.HasECM;
                }

                loneWolf.Bravery = ((RNG.Random(256) * 2) | 64) & 127;
                InBattle = true;
            }
            else
            {
                Debug.Fail("Failed to create Lone Hunter");
            }
        }

        private void CreateTrader()
        {
            IShip trader = new ShipFactory(_draw).CreateTrader();

            if (_universe.AddNewShip(trader))
            {
                trader.Rotmat[2].Z = -1.0f;
                trader.RotZ = RNG.Random(8);
                trader.Velocity = RNG.Random(32) | 16;
                trader.Bravery = RNG.Random(128);

                if (RNG.TrueOrFalse())
                {
                    trader.Flags |= ShipFlags.HasECM;
                }

                //      if (rnd & 2)
                //          space.universe[newship].flags |= FLG.FLG_ANGRY;
            }
            else
            {
                Debug.Fail("Failed to create Trader");
            }
        }

        private bool LaunchEnemy(IShip sourceShip, IShip newShip, ShipFlags flags, int bravery)
        {
            Debug.Assert(sourceShip.Rotmat != null, "Rotation matrix should not be null.");
            Vector3[] rotmat = sourceShip.Rotmat.Cloner();

            if (!_universe.AddNewShip(newShip, sourceShip.Location, rotmat, sourceShip.RotX, sourceShip.RotZ))
            {
                return false;
            }

            if (sourceShip.Flags.HasFlag(ShipFlags.Station))
            {
                newShip.Velocity = 32;
                newShip.Location += newShip.Rotmat[2] * 2;
            }

            newShip.Flags |= flags;
            newShip.RotZ /= 2;
            newShip.RotZ *= 2;
            newShip.Bravery = bravery;

            if (newShip.Type is ShipType.Cargo or ShipType.Alloy or ShipType.Rock)
            {
                newShip.RotZ = ((RNG.Random(256) * 2) & 255) - 128;
                newShip.RotX = ((RNG.Random(256) * 2) & 255) - 128;
                newShip.Velocity = RNG.Random(16);
            }

            return true;
        }

        private void LaunchLoot(IShip ship, ShipType lootType)
        {
            int count;

            if (lootType == ShipType.Rock)
            {
                count = RNG.Random(4);
            }
            else
            {
                count = RNG.Random(256);
                if (count >= 128)
                {
                    return;
                }

                count &= ship.LootMax;
                count &= 15;
            }

            for (int i = 0; i < count; i++)
            {
                IShip loot;
                if (lootType == ShipType.Rock)
                {
                    loot = new RockSplinter(_draw);
                }
                else if (lootType == ShipType.Alloy)
                {
                    loot = new Alloy(_draw);
                }
                else if (lootType == ShipType.Cargo)
                {
                    loot = new CargoCannister(_draw);
                }
                else
                {
                    Debug.Fail("Incorrect loot type");
                    return;
                }

                if (!LaunchEnemy(ship, loot, 0, 0))
                {
                    Debug.Fail("Failed to create Loot");
                }
            }
        }

        private void LaunchShuttle()
        {
            if (_universe.ShipCount(ShipType.Transporter) != 0 ||
                _universe.ShipCount(ShipType.Shuttle) != 0 ||
                RNG.Random(256) < 253 || _pilot.IsAutoPilotOn)
            {
                return;
            }

            IObject? station = _universe.StationOrSun;
            if (station == null)
            {
                return;
            }

            Debug.Assert(station.Flags.HasFlag(ShipFlags.Station), "Shuttle must be launched from a station");
            IShip shuttle = RNG.TrueOrFalse() ? new Shuttle(_draw) : new Transporter(_draw);
            if (!LaunchEnemy((IShip)station, shuttle, ShipFlags.HasECM | ShipFlags.FlyToPlanet, 113))
            {
                Debug.Fail("Failed to create Shuttle");
            }
        }

        private void MissileTactics(IShip missile)
        {
            Vector3 vec;
            Vector3 nvec;
            float direction;
            const float cnt2 = 0.223f;

            if (_ship.EcmActive != 0)
            {
                _audio.PlayEffect(SoundEffect.Explode);

                missile.Flags |= ShipFlags.Dead;
                return;
            }

            if (missile.Target == null)
            {
                if (missile.Location.Length() < 256)
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
                vec = missile.Location - missile.Target.Location;

                if (vec.Length() < 256)
                {
                    missile.Flags |= ShipFlags.Dead;

                    if (!missile.Target.Flags.HasFlag(ShipFlags.Station))
                    {
                        ExplodeObject(missile.Target);
                    }
                    else
                    {
                        _audio.PlayEffect(SoundEffect.Explode);
                    }

                    return;
                }

                if (RNG.Random(256) < 16 && missile.Target.Flags.HasFlag(ShipFlags.HasECM))
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

            TrackObject(missile, direction, nvec);

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
            else if (RNG.Random(256) >= 200)
            {
                missile.Acceleration = -2;
            }
        }
    }
}
