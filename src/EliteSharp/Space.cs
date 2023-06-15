// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Diagnostics;
using System.Numerics;
using EliteSharp.Audio;
using EliteSharp.Conflict;
using EliteSharp.Graphics;
using EliteSharp.Ships;
using EliteSharp.Trader;
using EliteSharp.Types;
using EliteSharp.Views;

namespace EliteSharp
{
    /// <summary>
    /// This module handles all the flight system and management of the space universe.
    /// </summary>
    internal sealed class Space
    {
        private readonly AudioController _audio;
        private readonly Combat _combat;
        private readonly GameState _gameState;
        private readonly IGraphics _graphics;
        private readonly Pilot _pilot;
        private readonly PlanetController _planet;
        private readonly PlayerShip _ship;
        private readonly Stars _stars;
        private readonly Threed _threed;
        private readonly Trade _trade;
        private readonly Universe _universe;
        private GalaxySeed _destinationPlanet = new();
        private float _hyperDistance;

        internal Space(
            GameState gameState,
            IGraphics graphics,
            Threed threed,
            AudioController audio,
            Pilot pilot,
            Combat combat,
            Trade trade,
            PlayerShip ship,
            PlanetController planet,
            Stars stars,
            Universe universe)
        {
            _gameState = gameState;
            _graphics = graphics;
            _threed = threed;
            _audio = audio;
            _pilot = pilot;
            _combat = combat;
            _trade = trade;
            _ship = ship;
            _planet = planet;
            _stars = stars;
            _universe = universe;
        }

        internal int HyperCountdown { get; private set; }

        internal bool HyperGalactic { get; private set; }

        internal string HyperName { get; private set; } = string.Empty;

        internal bool IsHyperspaceReady { get; set; }

        internal void CountdownHyperspace()
        {
            if (HyperCountdown == 0)
            {
                CompleteHyperspace();
                return;
            }

            HyperCountdown--;
        }

        internal void DisplayHyperStatus() => _graphics.DrawTextRight(22, 5, $"{HyperCountdown}", Colour.White);

        /// <summary>
        /// Dock the player into the space station.
        /// </summary>
        internal void DockPlayer()
        {
            _pilot.DisengageAutoPilot();
            _gameState.IsDocked = true;
            _gameState.Reset();
            _ship.Reset();
            _combat.ResetWeapons();
        }

        /// <summary>
        /// Engage the docking computer. For the moment we just do an instant dock if we are in the safe zone.
        /// </summary>
        internal void EngageDockingComputer()
        {
            if (_universe.IsStationPresent)
            {
                _gameState.SetView(Screen.Docking);
            }
        }

        internal void JumpWarp()
        {
            ShipType type;
            float jump;

            foreach (IObject universeObj in _universe.GetAllObjects())
            {
                type = universeObj.Type;

                if (type is > 0 and not ShipType.Asteroid and not ShipType.Cargo and
                    not ShipType.Alloy and not ShipType.Rock and
                    not ShipType.Boulder and not ShipType.EscapeCapsule)
                {
                    _gameState.InfoMessage("Mass Locked");
                    return;
                }
            }

            if ((_universe.Planet!.Location.Length() < 75001) || (_universe.StationOrSun!.Location.Length() < 75001))
            {
                _gameState.InfoMessage("Mass Locked");
                return;
            }

            jump = _universe.Planet.Location.Length() < _universe.StationOrSun.Location.Length() ?
                _universe.Planet.Location.Length() - 75000 : _universe.StationOrSun.Location.Length() - 75000;

            if (jump > 1024)
            {
                jump = 1024;
            }

            foreach (IObject universeObj in _universe.GetAllObjects())
            {
                if (universeObj.Type != 0)
                {
                    universeObj.Location = new(universeObj.Location.X, universeObj.Location.Y, universeObj.Location.Z - jump);
                }
            }

            _stars.WarpStars = true;
            _gameState.MCount &= 63;
            _combat.InBattle = false;
        }

        internal void LaunchPlayer()
        {
            _ship.Speed = 12;

            // Rotate in the same direction that the station is spinning
            _ship.Roll = 15;
            _ship.Climb = 0;
            _gameState.Cmdr.LegalStatus |= _trade.IsCarryingContraband();
            _stars.CreateNewStars();
            _threed.GenerateLandscape((_gameState.DockedPlanet.A * 251) + _gameState.DockedPlanet.B);
            if (!_universe.AddNewShip(new Planet(), new(0, 0, 65536), VectorMaths.GetInitialMatrix(), 0, 0))
            {
                Debug.WriteLine("Failed to create Planet");
            }

            Vector3[] rotmat = VectorMaths.GetInitialMatrix();
            rotmat[2].X = -rotmat[2].X;
            rotmat[2].Y = -rotmat[2].Y;
            rotmat[2].Z = -rotmat[2].Z;
            _universe.AddNewStation(_gameState.CurrentPlanetData.TechLevel, new(0, 0, -256), rotmat);

            _gameState.IsDocked = false;
        }

        internal void StartGalacticHyperspace()
        {
            if (IsHyperspaceReady)
            {
                return;
            }

            if (!_ship.HasGalacticHyperdrive)
            {
                return;
            }

            IsHyperspaceReady = true;
            HyperCountdown = 2;
            HyperGalactic = true;
            _pilot.DisengageAutoPilot();
        }

        internal void StartHyperspace()
        {
            if (IsHyperspaceReady)
            {
                return;
            }

            _hyperDistance = PlanetController.CalculateDistanceToPlanet(_gameState.DockedPlanet, _gameState.HyperspacePlanet);

            if ((_hyperDistance == 0) || (_hyperDistance > _ship.Fuel))
            {
                return;
            }

            _destinationPlanet = new(_gameState.HyperspacePlanet);
            HyperName = _planet.NamePlanet(_destinationPlanet).CapitaliseFirstLetter();
            IsHyperspaceReady = true;
            HyperCountdown = 15;
            HyperGalactic = false;

            _pilot.DisengageAutoPilot();
        }

        internal void UpdateAltitude()
        {
            _ship.Altitude = 255;

            if (_gameState.InWitchspace)
            {
                return;
            }

            if (_universe.Planet == null)
            {
                return;
            }

            Vector3 vec = Vector3.Abs(_universe.Planet.Location);

            if ((vec.X == 0 && vec.Y == 0 && vec.Z == 0) ||
                vec.X > 65535 || vec.Y > 65535 || vec.Z > 65535)
            {
                return;
            }

            vec /= 256;
            vec *= vec;

            float dist = vec.X + vec.Y + vec.Z;

            if (dist > 65535)
            {
                return;
            }

            dist -= 9472;
            if (dist < 1)
            {
                _ship.Altitude = 0;
                _gameState.GameOver();
                return;
            }

            dist = MathF.Sqrt(dist);
            if (dist < 1)
            {
                _ship.Altitude = 0;
                _gameState.GameOver();
                return;
            }

            _ship.Altitude = dist;
        }

        internal void UpdateCabinTemp()
        {
            _ship.CabinTemperature = 30;

            if (_gameState.InWitchspace)
            {
                return;
            }

            if (_universe.IsStationPresent)
            {
                return;
            }

            Vector3 vec = Vector3.Abs(_universe.StationOrSun!.Location);

            if ((vec.X == 0 && vec.Y == 0 && vec.Z == 0) ||
                vec.X > 65535 || vec.Y > 65535 || vec.Z > 65535)
            {
                return;
            }

            vec /= 256;
            vec *= vec;

            float dist = (vec.X + vec.Y + vec.Z) / 256;

            if (dist > 255)
            {
                return;
            }

            dist = (int)dist ^ 255;

            _ship.CabinTemperature = dist + 30;

            if (_ship.CabinTemperature > 255)
            {
                _ship.CabinTemperature = 255;
                _gameState.GameOver();
                return;
            }

            if ((_ship.CabinTemperature < 224) || (!_ship.HasFuelScoop))
            {
                return;
            }

            _ship.Fuel += _ship.Speed / 2;
            if (_ship.Fuel > _ship.MaxFuel)
            {
                _ship.Fuel = _ship.MaxFuel;
            }

            _gameState.InfoMessage("Fuel Scoop On");
        }

        /// <summary>
        /// Update all the objects in the universe and render them.
        /// </summary>
        internal void UpdateUniverse()
        {
            _threed.RenderStart();
            int i = -1;

            foreach (IObject universeObj in _universe.GetAllObjects())
            {
                i++;

                ShipType type = universeObj.Type;

                if (type == ShipType.None)
                {
                    continue;
                }

                if (universeObj.Flags.HasFlag(ShipFlags.Remove))
                {
                    if (type == ShipType.Viper)
                    {
                        _gameState.Cmdr.LegalStatus |= 64;
                    }

                    float bounty = universeObj.Bounty;

                    if ((bounty != 0) && (!_gameState.InWitchspace))
                    {
                        _trade.Credits += bounty;
                        _gameState.InfoMessage($"{_trade.Credits:N1} Credits");
                    }

                    _combat.RemoveShip(universeObj);
                    continue;
                }

                if (_gameState.DetonateBomb &&
                    (!universeObj.Flags.HasFlag(ShipFlags.Dead)) &&
                    (type != ShipType.Planet) &&
                    (type != ShipType.Sun) &&
                    (type != ShipType.Constrictor) &&
                    (type != ShipType.Cougar) &&
                    (type != ShipType.Coriolis) &&
                    (type != ShipType.Dodec))
                {
                    _audio.PlayEffect(SoundEffect.Explode);
                    universeObj.Flags |= ShipFlags.Dead;
                }

                if (_gameState.CurrentScreen is
                    not Screen.IntroOne and
                    not Screen.IntroTwo and
                    not Screen.GameOver and
                    not Screen.EscapeCapsule)
                {
                    _combat.Tactics(universeObj, i);
                }

                MoveUniverseObject(universeObj);

                IObject flip = new NullObject(universeObj);
                SwitchToView(flip);

                if (type == ShipType.Planet)
                {
                    if (!_universe.IsStationPresent && (universeObj.Location.Length() < 65792 /* was 49152 */))
                    {
                        MakeStationAppear();
                    }

                    _threed.DrawObject(flip);
                    continue;
                }

                if (type == ShipType.Sun)
                {
                    _threed.DrawObject(flip);
                    continue;
                }

                if (universeObj.Location.Length() < 170)
                {
                    if (type is ShipType.Coriolis or ShipType.Dodec)
                    {
                        CheckDocking(universeObj);
                    }
                    else
                    {
                        _combat.ScoopItem(universeObj);
                    }

                    continue;
                }

                if (universeObj.Location.Length() > 57344)
                {
                    _combat.RemoveShip(universeObj);
                    continue;
                }

                _threed.DrawObject(flip);

                universeObj.Flags = flip.Flags;
                universeObj.ExpDelta = flip.ExpDelta;

                universeObj.Flags &= ~ShipFlags.Firing;

                if (universeObj.Flags.HasFlag(ShipFlags.Dead))
                {
                    continue;
                }

                _combat.CheckTarget(universeObj, flip);
            }

            _threed.RenderEnd();
            _gameState.DetonateBomb = false;
        }

        private static int RotateByteLeft(int x) => ((x << 1) | (x >> 7)) & 255;

        private static void RotateXFirst(ref float a, ref float b, float direction)
        {
            float fx = a;
            float ux = b;

            if (direction < 0)
            {
                a = fx - (fx / 512) + (ux / 19);
                b = ux - (ux / 512) - (fx / 19);
            }
            else
            {
                a = fx - (fx / 512) - (ux / 19);
                b = ux - (ux / 512) + (fx / 19);
            }
        }

        private void CheckDocking(IObject obj)
        {
            if (_gameState.IsDocked)
            {
                return;
            }

            if (IsDocking(obj))
            {
                _gameState.SetView(Screen.Docking);
                return;
            }

            if (_ship.Speed >= 5)
            {
                _gameState.GameOver();
                return;
            }

            _ship.Speed = 1;
            _ship.DamageShip(5, obj.Location.Z > 0);
            _audio.PlayEffect(SoundEffect.Crash);
        }

        private void CompleteHyperspace()
        {
            IsHyperspaceReady = false;
            _gameState.InWitchspace = false;

            if (HyperGalactic)
            {
                _ship.HasGalacticHyperdrive = false;
                HyperGalactic = false;
                EnterNextGalaxy();
                _gameState.Cmdr.LegalStatus = 0;
            }
            else
            {
                _ship.Fuel -= _hyperDistance;
                _gameState.Cmdr.LegalStatus /= 2;

                if ((RNG.Random(255) > 253) || (_ship.Climb >= _ship.MaxClimb))
                {
                    EnterWitchspace();
                    return;
                }

                _gameState.DockedPlanet = new(_destinationPlanet);
            }

            _trade.MarketRandomiser = RNG.Random(255);
            _gameState.CurrentPlanetData = PlanetController.GeneratePlanetData(_gameState.DockedPlanet);
            _trade.GenerateStockMarket();

            _ship.Speed = 12;
            _ship.Roll = 0;
            _ship.Climb = 0;
            _stars.CreateNewStars();
            _combat.Reset();
            _universe.ClearUniverse();

            _threed.GenerateLandscape((_gameState.DockedPlanet.A * 251) + _gameState.DockedPlanet.B);

            Vector3 position = new()
            {
                Z = ((_gameState.DockedPlanet.B & 7) + 7) / 2,
            };
            position.X = position.Z / 2;
            position.Y = position.X;

            position.X *= 65536;
            position.Y *= 65536;
            position.Z *= 65536;

            if ((_gameState.DockedPlanet.B & 1) == 0)
            {
                position.X = -position.X;
                position.Y = -position.Y;
            }

            if (!_universe.AddNewShip(new Planet(), position, VectorMaths.GetInitialMatrix(), 0, 0))
            {
                Debug.WriteLine("Failed to create Planet");
            }

            position.Z = -(((_gameState.DockedPlanet.D & 7) | 1) << 16);
            position.X = ((_gameState.DockedPlanet.F & 3) << 16) | ((_gameState.DockedPlanet.F & 3) << 8);

            if (!_universe.AddNewShip(new Sun(), position, VectorMaths.GetInitialMatrix(), 0, 0))
            {
                Debug.WriteLine("Failed to create Sun");
            }

            _gameState.SetView(Screen.Hyperspace);
        }

        private void EnterNextGalaxy()
        {
            _gameState.Cmdr.GalaxyNumber++;
            _gameState.Cmdr.GalaxyNumber &= 7;

            _gameState.Cmdr.Galaxy = new()
            {
                A = RotateByteLeft(_gameState.Cmdr.Galaxy.A),
                B = RotateByteLeft(_gameState.Cmdr.Galaxy.B),
                C = RotateByteLeft(_gameState.Cmdr.Galaxy.C),
                D = RotateByteLeft(_gameState.Cmdr.Galaxy.D),
                E = RotateByteLeft(_gameState.Cmdr.Galaxy.E),
                F = RotateByteLeft(_gameState.Cmdr.Galaxy.F),
            };

            _gameState.DockedPlanet = _planet.FindPlanet(_gameState.Cmdr.Galaxy, new(0x60, 0x60));
            _gameState.HyperspacePlanet = new(_gameState.DockedPlanet);
        }

        private void EnterWitchspace()
        {
            _gameState.InWitchspace = true;
            _gameState.DockedPlanet.B ^= 31;
            _combat.InBattle = true;

            _ship.Speed = 12;
            _ship.Roll = 0;
            _ship.Climb = 0;
            _stars.CreateNewStars();
            _combat.Reset();
            _universe.ClearUniverse();

            int nthg = RNG.Random(1, 4);

            for (int i = 0; i < nthg; i++)
            {
                _combat.CreateThargoid();
            }

            _gameState.SetView(Screen.Hyperspace);
        }

        /// <summary>
        /// Check if we are correctly aligned to dock.
        /// </summary>
        private bool IsDocking(IObject obj)
        {
            Vector3 vec;
            float fz;
            float ux;

            // Don't want it to kill anyone!
            if (_pilot.IsAutoPilotOn)
            {
                return true;
            }

            fz = obj.Rotmat[2].Z;

            if (fz > -0.90)
            {
                return false;
            }

            vec = VectorMaths.UnitVector(obj.Location);

            if (vec.Z < 0.927)
            {
                return false;
            }

            ux = obj.Rotmat[1].X;
            if (ux < 0)
            {
                ux = -ux;
            }

            return ux >= 0.84;
        }

        private void MakeStationAppear()
        {
            Vector3 location = _universe.Planet!.Location;
            Vector3 vec = new(RNG.Random(-16384, 16383), RNG.Random(-16384, 16383), RNG.Random(32767));
            vec = VectorMaths.UnitVector(vec);
            Vector3 position = location - (vec * 65792);

            //  VectorMaths.set_init_matrix (rotmat);
            Vector3[] rotmat = new Vector3[3];

            rotmat[0].X = 1;
            rotmat[0].Y = 0;
            rotmat[0].Z = 0;

            rotmat[1].X = vec.X;
            rotmat[1].Y = vec.Z;
            rotmat[1].Z = -vec.Y;

            rotmat[2].X = vec.X;
            rotmat[2].Y = vec.Y;
            rotmat[2].Z = vec.Z;

            VectorMaths.TidyMatrix(rotmat);

            _universe.AddNewStation(_gameState.CurrentPlanetData.TechLevel, position, rotmat);
        }

        /// <summary>
        /// Update an objects location in the universe.
        /// </summary>
        /// <param name="obj"></param>
        private void MoveUniverseObject(IObject obj)
        {
            float x, y, z;
            float k2;
            float alpha;
            float beta;
            float rotx, rotz;
            float speed;

            alpha = _ship.Roll / 256;
            beta = _ship.Climb / 256;

            x = obj.Location.X;
            y = obj.Location.Y;
            z = obj.Location.Z;

            if (!obj.Flags.HasFlag(ShipFlags.Dead))
            {
                if (obj.Velocity != 0)
                {
                    speed = obj.Velocity;
                    speed *= 1.5f;
                    x += obj.Rotmat[2].X * speed;
                    y += obj.Rotmat[2].Y * speed;
                    z += obj.Rotmat[2].Z * speed;
                }

                if (obj.Acceleration != 0)
                {
                    obj.Velocity += obj.Acceleration;
                    obj.Acceleration = 0;
                    if (obj.Velocity > obj.VelocityMax)
                    {
                        obj.Velocity = obj.VelocityMax;
                    }

                    if (obj.Velocity <= 0)
                    {
                        obj.Velocity = 1;
                    }
                }
            }

            k2 = y - (alpha * x);
            z += beta * k2;
            y = k2 - (z * beta);
            x += alpha * y;

            z -= _ship.Speed;

            obj.Location = new(x, y, z);

            if (obj.Type == ShipType.Planet)
            {
                beta = 0.0f;
            }

            obj.Rotmat = VectorMaths.RotateVector(obj.Rotmat, alpha, beta);

            if (obj.Flags.HasFlag(ShipFlags.Dead))
            {
                return;
            }

            rotx = obj.RotX;
            rotz = obj.RotZ;

            // If necessary rotate the object around the X axis...
            if (rotx != 0)
            {
                RotateXFirst(ref obj.Rotmat[2].X, ref obj.Rotmat[1].X, rotx);
                RotateXFirst(ref obj.Rotmat[2].Y, ref obj.Rotmat[1].Y, rotx);
                RotateXFirst(ref obj.Rotmat[2].Z, ref obj.Rotmat[1].Z, rotx);

                if (rotx is not 127 and not -127)
                {
                    obj.RotX -= (rotx < 0) ? -1 : 1;
                }
            }

            // If necessary rotate the object around the Z axis...
            if (rotz != 0)
            {
                RotateXFirst(ref obj.Rotmat[0].X, ref obj.Rotmat[1].X, rotz);
                RotateXFirst(ref obj.Rotmat[0].Y, ref obj.Rotmat[1].Y, rotz);
                RotateXFirst(ref obj.Rotmat[0].Z, ref obj.Rotmat[1].Z, rotz);

                if (rotz is not 127 and not -127)
                {
                    obj.RotZ -= (rotz < 0) ? -1 : 1;
                }
            }

            // Orthonormalize the rotation matrix...
            VectorMaths.TidyMatrix(obj.Rotmat);
        }

        private void SwitchToView(IObject flip)
        {
            float tmp;

            if (_gameState.CurrentScreen is Screen.RearView or Screen.GameOver)
            {
                flip.Location = new(-flip.Location.X, flip.Location.Y, -flip.Location.Z);

                flip.Rotmat[0].X = -flip.Rotmat[0].X;
                flip.Rotmat[0].Z = -flip.Rotmat[0].Z;

                flip.Rotmat[1].X = -flip.Rotmat[1].X;
                flip.Rotmat[1].Z = -flip.Rotmat[1].Z;

                flip.Rotmat[2].X = -flip.Rotmat[2].X;
                flip.Rotmat[2].Z = -flip.Rotmat[2].Z;
                return;
            }

            if (_gameState.CurrentScreen == Screen.LeftView)
            {
                tmp = flip.Location.X;
                flip.Location = new(flip.Location.Z, flip.Location.Y, -tmp);

                if (flip.Type < 0)
                {
                    return;
                }

                tmp = flip.Rotmat[0].X;
                flip.Rotmat[0].X = flip.Rotmat[0].Z;
                flip.Rotmat[0].Z = -tmp;

                tmp = flip.Rotmat[1].X;
                flip.Rotmat[1].X = flip.Rotmat[1].Z;
                flip.Rotmat[1].Z = -tmp;

                tmp = flip.Rotmat[2].X;
                flip.Rotmat[2].X = flip.Rotmat[2].Z;
                flip.Rotmat[2].Z = -tmp;
                return;
            }

            if (_gameState.CurrentScreen == Screen.RightView)
            {
                tmp = flip.Location.X;
                flip.Location = new(-flip.Location.Z, flip.Location.Y, tmp);

                if (flip.Type < 0)
                {
                    return;
                }

                tmp = flip.Rotmat[0].X;
                flip.Rotmat[0].X = -flip.Rotmat[0].Z;
                flip.Rotmat[0].Z = tmp;

                tmp = flip.Rotmat[1].X;
                flip.Rotmat[1].X = -flip.Rotmat[1].Z;
                flip.Rotmat[1].Z = tmp;

                tmp = flip.Rotmat[2].X;
                flip.Rotmat[2].X = -flip.Rotmat[2].Z;
                flip.Rotmat[2].Z = tmp;
            }
        }
    }
}
