// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using Elite.Common.Enums;
using Elite.Engine.Conflict;
using Elite.Engine.Enums;
using Elite.Engine.Ships;
using Elite.Engine.Trader;
using Elite.Engine.Types;

namespace Elite.Engine
{
    /// <summary>
    /// This module handles all the flight system and management of the space universe.
    /// </summary>
    internal sealed class Space
    {
        internal static int s_hyper_countdown;
        internal static bool s_hyper_galactic;
        internal static string s_hyper_name = string.Empty;
        internal static bool s_hyper_ready;
        internal static Dictionary<ShipType, int> s_ship_count = new();
        internal static UniverseObject[] s_universe = new UniverseObject[EliteMain.MAX_UNIV_OBJECTS];
        private static GalaxySeed s_destination_planet = new();
        private static float s_hyper_distance;
        private readonly Audio _audio;
        private readonly Combat _combat;
        private readonly GameState _gameState;
        private readonly IGraphics _graphics;
        private readonly Pilot _pilot;
        private readonly Planet _planet;
        private readonly PlayerShip _ship;
        private readonly Stars _stars;
        private readonly Threed _threed;
        private readonly Trade _trade;

        internal Space(
            GameState gameState,
            IGraphics graphics,
            Threed threed,
            Audio audio,
            Pilot pilot,
            Combat combat,
            Trade trade,
            PlayerShip ship,
            Planet planet,
            Stars stars)
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
        }

        internal void CountdownHyperspace()
        {
            if (s_hyper_countdown == 0)
            {
                CompleteHyperspace();
                return;
            }

            s_hyper_countdown--;
        }

        internal void DisplayHyperStatus() => _graphics.DrawTextRight(22, 5, $"{s_hyper_countdown}", Colour.White1);

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
            if (s_ship_count[ShipType.Coriolis] != 0 || s_ship_count[ShipType.Dodec] != 0)
            {
                _gameState.SetView(SCR.SCR_DOCKING);
            }
        }

        internal void JumpWarp()
        {
            int i;
            ShipType type;
            float jump;

            for (i = 0; i < EliteMain.MAX_UNIV_OBJECTS; i++)
            {
                type = s_universe[i].Type;

                if (type is > 0 and not ShipType.Asteroid and not ShipType.Cargo and
                    not ShipType.Alloy and not ShipType.Rock and
                    not ShipType.Boulder and not ShipType.EscapeCapsule)
                {
                    _gameState.InfoMessage("Mass Locked");
                    return;
                }
            }

            if ((s_universe[0].Location.Length() < 75001) || (s_universe[1].Location.Length() < 75001))
            {
                _gameState.InfoMessage("Mass Locked");
                return;
            }

            jump = s_universe[0].Location.Length() < s_universe[1].Location.Length() ? s_universe[0].Location.Length() - 75000f : s_universe[1].Location.Length() - 75000f;

            if (jump > 1024)
            {
                jump = 1024;
            }

            for (i = 0; i < EliteMain.MAX_UNIV_OBJECTS; i++)
            {
                if (s_universe[i].Type != 0)
                {
                    s_universe[i].Location = new(s_universe[i].Location.X, s_universe[i].Location.Y, s_universe[i].Location.Z - jump);
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
            _combat.AddNewShip(ShipType.Planet, new(0, 0, 65536), VectorMaths.GetInitialMatrix(), 0, 0);

            Vector3[] rotmat = VectorMaths.GetInitialMatrix();
            rotmat[2].X = -rotmat[2].X;
            rotmat[2].Y = -rotmat[2].Y;
            rotmat[2].Z = -rotmat[2].Z;
            _combat.AddNewStation(new(0, 0, -256), rotmat);

            _gameState.IsDocked = false;
        }

        internal void StartGalacticHyperspace()
        {
            if (s_hyper_ready)
            {
                return;
            }

            if (!_ship.HasGalacticHyperdrive)
            {
                return;
            }

            s_hyper_ready = true;
            s_hyper_countdown = 2;
            s_hyper_galactic = true;
            _pilot.DisengageAutoPilot();
        }

        internal void StartHyperspace()
        {
            if (s_hyper_ready)
            {
                return;
            }

            s_hyper_distance = Planet.CalculateDistanceToPlanet(_gameState.DockedPlanet, _gameState.HyperspacePlanet);

            if ((s_hyper_distance == 0) || (s_hyper_distance > _ship.Fuel))
            {
                return;
            }

            s_destination_planet = (GalaxySeed)_gameState.HyperspacePlanet.Clone();
            s_hyper_name = _planet.NamePlanet(s_destination_planet, true);
            s_hyper_ready = true;
            s_hyper_countdown = 15;
            s_hyper_galactic = false;

            _pilot.DisengageAutoPilot();
        }

        internal void UpdateAltitude()
        {
            _ship.Altitude = 255;

            if (_gameState.InWitchspace)
            {
                return;
            }

            float x = MathF.Abs(s_universe[0].Location.X);
            float y = MathF.Abs(s_universe[0].Location.Y);
            float z = MathF.Abs(s_universe[0].Location.Z);

            if ((x == 0 && y == 0 && z == 0) ||
                x > 65535 || y > 65535 || z > 65535)
            {
                return;
            }

            x /= 256;
            y /= 256;
            z /= 256;

            float dist = (x * x) + (y * y) + (z * z);

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

            if (s_ship_count[ShipType.Coriolis] != 0 || s_ship_count[ShipType.Dodec] != 0)
            {
                return;
            }

            float x = MathF.Abs(s_universe[1].Location.X);
            float y = MathF.Abs(s_universe[1].Location.Y);
            float z = MathF.Abs(s_universe[1].Location.Z);

            if ((x == 0 && y == 0 && z == 0) ||
                x > 65535 || y > 65535 || z > 65535)
            {
                return;
            }

            x /= 256;
            y /= 256;
            z /= 256;

            float dist = ((x * x) + (y * y) + (z * z)) / 256;

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

            for (int i = 0; i < EliteMain.MAX_UNIV_OBJECTS; i++)
            {
                ShipType type = s_universe[i].Type;

                if (type == ShipType.None)
                {
                    continue;
                }

                if (s_universe[i].Flags.HasFlag(ShipFlags.Remove))
                {
                    if (type == ShipType.Viper)
                    {
                        _gameState.Cmdr.LegalStatus |= 64;
                    }

                    float bounty = _gameState.ShipList[type].Bounty;

                    if ((bounty != 0) && (!_gameState.InWitchspace))
                    {
                        _trade._credits += bounty;
                        _gameState.InfoMessage($"{_trade._credits:N1} Credits");
                    }

                    _combat.RemoveShip(i);
                    continue;
                }

                if (_gameState.DetonateBomb &&
                    (!s_universe[i].Flags.HasFlag(ShipFlags.Dead)) &&
                    (type != ShipType.Planet) &&
                    (type != ShipType.Sun) &&
                    (type != ShipType.Constrictor) &&
                    (type != ShipType.Cougar) &&
                    (type != ShipType.Coriolis) &&
                    (type != ShipType.Dodec))
                {
                    _audio.PlayEffect(SoundEffect.Explode);
                    s_universe[i].Flags |= ShipFlags.Dead;
                }

                if (_gameState.CurrentScreen is
                    not SCR.SCR_INTRO_ONE and
                    not SCR.SCR_INTRO_TWO and
                    not SCR.SCR_GAME_OVER and
                    not SCR.SCR_ESCAPE_CAPSULE)
                {
                    _combat.Tactics(i);
                }

                MoveUniverseObject(ref s_universe[i]);

                UniverseObject flip = new(s_universe[i]);
                SwitchToView(ref flip);

                if (type == ShipType.Planet)
                {
                    if ((s_ship_count[ShipType.Coriolis] == 0) &&
                        (s_ship_count[ShipType.Dodec] == 0) &&
                        (s_universe[i].Location.Length() < 65792 /* was 49152 */))
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

                if (s_universe[i].Location.Length() < 170)
                {
                    if (type is ShipType.Coriolis or ShipType.Dodec)
                    {
                        CheckDocking(i);
                    }
                    else
                    {
                        _combat.ScoopItem(i);
                    }

                    continue;
                }

                if (s_universe[i].Location.Length() > 57344)
                {
                    _combat.RemoveShip(i);
                    continue;
                }

                _threed.DrawObject(flip);

                s_universe[i].Flags = flip.Flags;
                s_universe[i].ExpDelta = flip.ExpDelta;

                s_universe[i].Flags &= ~ShipFlags.Firing;

                if (s_universe[i].Flags.HasFlag(ShipFlags.Dead))
                {
                    continue;
                }

                _combat.CheckTarget(i, ref flip);
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

        private void CheckDocking(int i)
        {
            if (_gameState.IsDocked)
            {
                return;
            }

            if (IsDocking(i))
            {
                _gameState.SetView(SCR.SCR_DOCKING);
                return;
            }

            if (_ship.Speed >= 5)
            {
                _gameState.GameOver();
                return;
            }

            _ship.Speed = 1;
            _ship.DamageShip(5, s_universe[i].Location.Z > 0);
            _audio.PlayEffect(SoundEffect.Crash);
        }

        private void CompleteHyperspace()
        {
            s_hyper_ready = false;
            _gameState.InWitchspace = false;

            if (s_hyper_galactic)
            {
                _ship.HasGalacticHyperdrive = false;
                s_hyper_galactic = false;
                EnterNextGalaxy();
                _gameState.Cmdr.LegalStatus = 0;
            }
            else
            {
                _ship.Fuel -= s_hyper_distance;
                _gameState.Cmdr.LegalStatus /= 2;

                if ((RNG.Random(255) > 253) || (_ship.Climb >= _ship.MaxClimb))
                {
                    EnterWitchspace();
                    return;
                }

                _gameState.DockedPlanet = (GalaxySeed)s_destination_planet.Clone();
            }

            _trade._marketRandomiser = RNG.Random(255);
            _gameState.CurrentPlanetData = Planet.GeneratePlanetData(_gameState.DockedPlanet);
            _trade.GenerateStockMarket(_gameState.CurrentPlanetData);

            _ship.Speed = 12;
            _ship.Roll = 0;
            _ship.Climb = 0;
            _stars.CreateNewStars();
            _combat.ClearUniverse();

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

            _combat.AddNewShip(ShipType.Planet, position, VectorMaths.GetInitialMatrix(), 0, 0);

            position.Z = -(((_gameState.DockedPlanet.D & 7) | 1) << 16);
            position.X = ((_gameState.DockedPlanet.F & 3) << 16) | ((_gameState.DockedPlanet.F & 3) << 8);

            _combat.AddNewShip(ShipType.Sun, position, VectorMaths.GetInitialMatrix(), 0, 0);

            _gameState.SetView(SCR.SCR_HYPERSPACE);
        }

        private void EnterNextGalaxy()
        {
            _gameState.Cmdr.GalaxyNumber++;
            _gameState.Cmdr.GalaxyNumber &= 7;

            GalaxySeed glx = new()
            {
                A = RotateByteLeft(_gameState.Cmdr.Galaxy.A),
                B = RotateByteLeft(_gameState.Cmdr.Galaxy.B),
                C = RotateByteLeft(_gameState.Cmdr.Galaxy.C),
                D = RotateByteLeft(_gameState.Cmdr.Galaxy.D),
                E = RotateByteLeft(_gameState.Cmdr.Galaxy.E),
                F = RotateByteLeft(_gameState.Cmdr.Galaxy.F),
            };
            _gameState.Cmdr.Galaxy = glx;

            _gameState.DockedPlanet = _planet.FindPlanet(_gameState.Cmdr.Galaxy, new(0x60, 0x60));
            _gameState.HyperspacePlanet = (GalaxySeed)_gameState.DockedPlanet.Clone();
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
            _combat.ClearUniverse();

            int nthg = RNG.Random(1, 4);

            for (int i = 0; i < nthg; i++)
            {
                _combat.CreateThargoid();
            }

            _gameState.SetView(SCR.SCR_HYPERSPACE);
        }

        /// <summary>
        /// Check if we are correctly aligned to dock.
        /// </summary>
        /// <param name="sn"></param>
        /// <returns></returns>
        private bool IsDocking(int sn)
        {
            Vector3 vec;
            float fz;
            float ux;

            // Don't want it to kill anyone!
            if (_gameState.IsAutoPilotOn)
            {
                return true;
            }

            fz = s_universe[sn].Rotmat[2].Z;

            if (fz > -0.90)
            {
                return false;
            }

            vec = VectorMaths.UnitVector(s_universe[sn].Location);

            if (vec.Z < 0.927)
            {
                return false;
            }

            ux = s_universe[sn].Rotmat[1].X;
            if (ux < 0)
            {
                ux = -ux;
            }

            return ux >= 0.84;
        }

        private void MakeStationAppear()
        {
            Vector3 location = s_universe[0].Location;
            Vector3 vec;
            vec.X = RNG.Random(-16384, 16383);
            vec.Y = RNG.Random(-16384, 16383);
            vec.Z = RNG.Random(32767);

            vec = VectorMaths.UnitVector(vec);

            Vector3 position = new()
            {
                X = location.X - (vec.X * 65792),
                Y = location.Y - (vec.Y * 65792),
                Z = location.Z - (vec.Z * 65792),
            };

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

            _combat.AddNewStation(position, rotmat);
        }

        /// <summary>
        /// Update an objects location in the universe.
        /// </summary>
        /// <param name="obj"></param>
        private void MoveUniverseObject(ref UniverseObject obj)
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
                    if (obj.Velocity > _gameState.ShipList[obj.Type].VelocityMax)
                    {
                        obj.Velocity = _gameState.ShipList[obj.Type].VelocityMax;
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

        private void SwitchToView(ref UniverseObject flip)
        {
            float tmp;

            if (_gameState.CurrentScreen is SCR.SCR_REAR_VIEW or SCR.SCR_GAME_OVER)
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

            if (_gameState.CurrentScreen == SCR.SCR_LEFT_VIEW)
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

            if (_gameState.CurrentScreen == SCR.SCR_RIGHT_VIEW)
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
