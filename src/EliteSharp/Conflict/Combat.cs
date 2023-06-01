// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Diagnostics;
using System.Numerics;
using EliteSharp.Audio;
using EliteSharp.Enums;
using EliteSharp.Lasers;
using EliteSharp.Ships;
using EliteSharp.Trader;

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
        private bool _isEcmOurs;
        private int _laserCounter;
        private int _laserStrength;
        private LaserType _laserType;

        internal Combat(GameState gameState, AudioController audio, PlayerShip ship, Trade trade, Pilot pilot, Universe universe)
        {
            _gameState = gameState;
            _audio = audio;
            _ship = ship;
            _trade = trade;
            _pilot = pilot;
            _universe = universe;
        }

        internal bool InBattle { get; set; }

        internal bool IsMissileArmed { get; private set; }

        internal IObject? MissileTarget { get; private set; } = new NullObject();

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

        internal void CheckTarget(IObject obj, IObject flip)
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

                    if (obj.Type is not ShipType.Coriolis and not ShipType.Dodec)
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
            IObject newship = _universe.AddNewShip(ShipType.Thargoid);

            if (newship.Type == ShipType.Thargoid)
            {
                newship.Flags = ShipFlags.Angry | ShipFlags.HasECM;
                newship.Bravery = 113;

                if (RNG.Random(255) > 64)
                {
                    LaunchEnemy(newship, ShipType.Tharglet, ShipFlags.Angry | ShipFlags.HasECM, 96);
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

            IObject newship = _universe.AddNewShip(ShipType.Missile, new(0, -28, 14), rotmat, 0, 0);

            if (newship.Type == ShipType.None)
            {
                _gameState.InfoMessage("Missile Jammed");
                return;
            }

            newship.Velocity = _ship.Speed * 2;
            newship.Flags = ShipFlags.Angry;
            newship.Target = MissileTarget;

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

            if (RNG.Random(255) == 136)
            {
                if (((int)_universe.Planet.Location.Z & 0x3e) != 0)
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

            if (_universe.PoliceCount != 0)
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

        internal void RemoveShip(IObject ship)
        {
            if (ship.Type == ShipType.None)
            {
                return;
            }

            CheckMissiles(ship);

            if (ship.Type is ShipType.Coriolis or ShipType.Dodec)
            {
                Vector3 position = ship.Location;
                position.Y = (int)position.Y & 0xFFFF;
                position.Y = (int)position.Y | 0x60000;
                _universe.AddNewShip(ShipType.Sun, position, VectorMaths.GetInitialMatrix(), 0, 0);
            }

            _universe.RemoveShip(ship);
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

        internal void ScoopItem(IObject obj)
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
                StockType trade = (StockType)RNG.Random(1, 8);
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

        internal void Tactics(IObject ship, int un)
        {
            Vector3 nvec;
            const float cnt2 = 0.223f;
            float direction;
            int attacking;

            ShipFlags flags = ship.Flags;

            if (ship.Type is ShipType.Planet or ShipType.Sun)
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

            if (ship.Type is ShipType.Coriolis or ShipType.Dodec)
            {
                if (flags.HasFlag(ShipFlags.Angry))
                {
                    if (RNG.Random(255) < 240)
                    {
                        return;
                    }

                    if (_universe.PoliceCount >= 4)
                    {
                        return;
                    }

                    LaunchEnemy(ship, ShipType.Viper, ShipFlags.Angry | ShipFlags.HasECM, 113);
                    return;
                }

                LaunchShuttle();
                return;
            }

            if (ship.Type == ShipType.Hermit)
            {
                if (RNG.Random(255) > 200)
                {
                    LaunchEnemy(ship, ShipType.Sidewinder + RNG.Random(3), ShipFlags.Angry | ShipFlags.HasECM, 113);
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
                RNG.Random(255) > 50)
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

            if (ship.Type == ShipType.Anaconda && RNG.Random(255) > 200)
            {
                LaunchEnemy(ship, RNG.Random(255) > 100 ? ShipType.Worm : ShipType.Sidewinder, ShipFlags.Angry | ShipFlags.HasECM, 113);
                return;
            }

            if (RNG.Random(255) >= 250)
            {
                ship.RotZ = RNG.Random(255) | 0x68;
                if (ship.RotZ > 127)
                {
                    ship.RotZ = -((int)ship.RotZ & 127);
                }
            }

            if (ship.Energy < ship.EnergyMax / 2)
            {
                if (ship.Energy < ship.EnergyMax / 8 && RNG.Random(255) > 230 && ship.Type != ShipType.Thargoid)
                {
                    ship.Flags &= ~ShipFlags.Angry;
                    ship.Flags |= ShipFlags.Inactive;
                    LaunchEnemy(ship, ShipType.EscapeCapsule, 0, 126);
                    return;
                }

                if (ship.Missiles != 0 && _ship.EcmActive == 0 && ship.Missiles >= RNG.Random(31))
                {
                    ship.Missiles--;
                    if (ship.Type == ShipType.Thargoid)
                    {
                        LaunchEnemy(ship, ShipType.Tharglet, ShipFlags.Angry, ship.Bravery);
                    }
                    else
                    {
                        LaunchEnemy(ship, ShipType.Missile, ShipFlags.Angry, 126);
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
            MissileTarget = null;
            IsMissileArmed = false;
            _audio.PlayEffect(SoundEffect.Boop);
        }

        private static bool IsInTarget(IObject ship, Vector3 position) =>
            position.Z >= 0 && (position.X * position.X) + (position.Y * position.Y) <= ship.Size;

        private static void MakeAngry(IObject ship)
        {
            if (ship.Flags.HasFlag(ShipFlags.Inactive))
            {
                return;
            }

            if (ship.Type is ShipType.Coriolis or ShipType.Dodec)
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

        private static void TrackObject(IObject ship, float direction, Vector3 nvec)
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
            ShipType type;

            if (RNG.Random(255) >= 35 || _universe.ShipCount(ShipType.Asteroid) >= 3)
            {
                return;
            }

            type = RNG.Random(255) > 253 ? ShipType.Hermit : ShipType.Asteroid;

            IObject asteroid = _universe.AddNewShip(type);

            if (asteroid.Type == ShipType.Asteroid)
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
                IObject newship = _universe.AddNewShip(type, position, VectorMaths.GetInitialMatrix(), 0, 0);
                if (newship.Type == type)
                {
                    newship.Flags = ShipFlags.Angry;
                    if (RNG.Random(255) > 245)
                    {
                        newship.Flags |= ShipFlags.HasECM;
                    }

                    newship.Bravery = ((RNG.Random(255) * 2) | 64) & 127;
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

            if (RNG.Random(255) >= offense)
            {
                return;
            }

            IObject newship = _universe.AddNewShip(ShipType.Viper);

            if (newship.Type == ShipType.Viper)
            {
                newship.Flags = ShipFlags.Angry;
                if (RNG.Random(255) > 245)
                {
                    newship.Flags |= ShipFlags.HasECM;
                }

                newship.Bravery = ((RNG.Random(255) * 2) | 64) & 127;
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
                if (universeObj.Type == ShipType.Missile && universeObj.Target == obj)
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

            IObject newship = _universe.AddNewShip(ShipType.Cougar);
            if (newship.Type == ShipType.Cougar)
            {
                newship.Flags = ShipFlags.HasECM; // | FLG_CLOAKED;
                newship.Bravery = 121;
                newship.Velocity = 18;
            }
            else
            {
                Debug.Fail("Failed to create Cougar");
            }
        }

        private void CreateLoneHunter()
        {
            ShipType type;

            if (_gameState.Cmdr.Mission == 1 && _gameState.Cmdr.GalaxyNumber == 1 &&
                _gameState.DockedPlanet.D == 144 && _gameState.DockedPlanet.B == 33 &&
                _universe.ShipCount(ShipType.Constrictor) == 0)
            {
                type = ShipType.Constrictor;
            }
            else
            {
                int rnd = RNG.Random(255);
                type = ShipType.CobraMk3Lone + (rnd & 3) + (rnd > 127 ? 1 : 0);
            }

            IObject newship = _universe.AddNewShip(type);

            if (newship.Type == type)
            {
                newship.Flags = ShipFlags.Angry;
                if (RNG.Random(255) > 200 || type == ShipType.Constrictor)
                {
                    newship.Flags |= ShipFlags.HasECM;
                }

                newship.Bravery = ((RNG.Random(255) * 2) | 64) & 127;
                InBattle = true;
            }
            else
            {
                Debug.Fail("Failed to create Lone Hunter");
            }
        }

        private void CreateTrader()
        {
            ShipType type = ShipType.CobraMk3 + RNG.Random(3);
            IObject newship = _universe.AddNewShip(type);

            if (newship.Type == type)
            {
                newship.Rotmat[2].Z = -1.0f;
                newship.RotZ = RNG.Random(7);
                newship.Velocity = RNG.Random(31) | 16;
                newship.Bravery = RNG.Random(127);

                if (RNG.TrueOrFalse())
                {
                    newship.Flags |= ShipFlags.HasECM;
                }

                //      if (rnd & 2)
                //          space.universe[newship].flags |= FLG.FLG_ANGRY;
            }
            else
            {
                Debug.Fail("Failed to create Trader");
            }
        }

        private void LaunchEnemy(IObject sourceShip, ShipType newType, ShipFlags flags, int bravery)
        {
            Debug.Assert(sourceShip.Rotmat != null, "Rotation matrix should not be null.");
            Vector3[] rotmat = sourceShip.Rotmat.Cloner();
            IObject newship = _universe.AddNewShip(newType, sourceShip.Location, rotmat, sourceShip.RotX, sourceShip.RotZ);

            if (newship.Type == ShipType.None)
            {
                return;
            }

            if (sourceShip.Type is ShipType.Coriolis or ShipType.Dodec)
            {
                newship.Velocity = 32;
                newship.Location = new(
                    newship.Location.X + (newship.Rotmat[2].X * 2),
                    newship.Location.Y + (newship.Rotmat[2].Y * 2),
                    newship.Location.Z + (newship.Rotmat[2].Z * 2));
            }

            newship.Flags |= flags;
            newship.RotZ /= 2;
            newship.RotZ *= 2;
            newship.Bravery = bravery;

            if (newType is ShipType.Cargo or ShipType.Alloy or ShipType.Rock)
            {
                newship.RotZ = ((RNG.Random(255) * 2) & 255) - 128;
                newship.RotX = ((RNG.Random(255) * 2) & 255) - 128;
                newship.Velocity = RNG.Random(15);
            }
        }

        private void LaunchLoot(IObject obj, ShipType loot)
        {
            int cnt;

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

                cnt &= obj.LootMax;
                cnt &= 15;
            }

            for (int i = 0; i < cnt; i++)
            {
                LaunchEnemy(obj, loot, 0, 0);
            }
        }

        private void LaunchShuttle()
        {
            if (_universe.ShipCount(ShipType.Transporter) != 0 ||
                _universe.ShipCount(ShipType.Shuttle) != 0 ||
                RNG.Random(255) < 253 || _pilot.IsAutoPilotOn)
            {
                return;
            }

            ShipType type = RNG.TrueOrFalse() ? ShipType.Shuttle : ShipType.Transporter;
            IObject station = _universe.StationOrSun;
            LaunchEnemy(station, type, ShipFlags.HasECM | ShipFlags.FlyToPlanet, 113);
        }

        private void MissileTactics(IObject missile)
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

                    if (missile.Target.Type is not ShipType.Coriolis and not ShipType.Dodec)
                    {
                        ExplodeObject(missile.Target);
                    }
                    else
                    {
                        _audio.PlayEffect(SoundEffect.Explode);
                    }

                    return;
                }

                if (RNG.Random(255) < 16 && missile.Target.Flags.HasFlag(ShipFlags.HasECM))
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
            else if (RNG.Random(255) >= 200)
            {
                missile.Acceleration = -2;
            }
        }
    }
}