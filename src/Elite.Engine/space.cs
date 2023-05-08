/*
 * Elite - The New Kind.
 *
 * Reverse engineered from the BBC disk version of Elite.
 * Additional material by C.J.Pinder.
 *
 * The original Elite code is (C) I.Bell & D.Braben 1984.
 * This version re-engineered in C by C.J.Pinder 1999-2001.
 *
 * email: <christian@newkind.co.uk>
 *
 *
 */

using System.Numerics;
using Elite.Common.Enums;
using Elite.Engine.Enums;
using Elite.Engine.Ships;
using Elite.Engine.Types;

/*
 * space.c
 *
 * This module handles all the flight system and management of the space universe.
 */

namespace Elite.Engine
{
    internal class Space
    {
        private readonly GameState _gameState;
        private readonly IGfx _gfx;
        private readonly Threed _threed;
        private readonly Audio _audio;
        private readonly Pilot _pilot;
        private readonly Combat _combat;
        private readonly Trade _trade;
        private readonly PlayerShip _ship;
        private readonly Planet _planet;

        private static GalaxySeed destination_planet = new();
        internal static bool hyper_ready;
        internal static int hyper_countdown;
        internal static string hyper_name = string.Empty;
        private static float hyper_distance;
        internal static bool hyper_galactic;
        internal static UniverseObject[] universe = new UniverseObject[EliteMain.MAX_UNIV_OBJECTS];
        internal static Dictionary<ShipType, int> ship_count = new();

        internal Space(GameState gameState, IGfx gfx, Threed threed, Audio audio, Pilot pilot, Combat combat, Trade trade, PlayerShip ship, Planet planet)
        {
            _gameState = gameState;
            _gfx = gfx;
            _threed = threed;
            _audio = audio;
            _pilot = pilot;
            _combat = combat;
            _trade = trade;
            _ship = ship;
            _planet = planet;
        }

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

            x = obj.location.X;
            y = obj.location.Y;
            z = obj.location.Z;

            if (!obj.flags.HasFlag(FLG.FLG_DEAD))
            {
                if (obj.velocity != 0)
                {
                    speed = obj.velocity;
                    speed *= 1.5f;
                    x += obj.Rotmat[2].X * speed;
                    y += obj.Rotmat[2].Y * speed;
                    z += obj.Rotmat[2].Z * speed;
                }

                if (obj.acceleration != 0)
                {
                    obj.velocity += obj.acceleration;
                    obj.acceleration = 0;
                    if (obj.velocity > _gameState.ShipList[obj.type].VelocityMax)
                    {
                        obj.velocity = _gameState.ShipList[obj.type].VelocityMax;
                    }

                    if (obj.velocity <= 0)
                    {
                        obj.velocity = 1;
                    }
                }
            }

            k2 = y - (alpha * x);
            z += beta * k2;
            y = k2 - (z * beta);
            x += alpha * y;

            z -= _ship.Speed;

            obj.location = new(x, y, z);

            if (obj.type == ShipType.Planet)
            {
                beta = 0.0f;
            }

            VectorMaths.RotateVector(ref obj.Rotmat, alpha, beta);

            if (obj.flags.HasFlag(FLG.FLG_DEAD))
            {
                return;
            }

            rotx = obj.rotx;
            rotz = obj.rotz;

            /* If necessary rotate the object around the X axis... */

            if (rotx != 0)
            {
                RotateXFirst(ref obj.Rotmat[2].X, ref obj.Rotmat[1].X, rotx);
                RotateXFirst(ref obj.Rotmat[2].Y, ref obj.Rotmat[1].Y, rotx);
                RotateXFirst(ref obj.Rotmat[2].Z, ref obj.Rotmat[1].Z, rotx);

                if (rotx is not 127 and not (-127))
                {
                    obj.rotx -= (rotx < 0) ? -1 : 1;
                }
            }


            /* If necessary rotate the object around the Z axis... */

            if (rotz != 0)
            {
                RotateXFirst(ref obj.Rotmat[0].X, ref obj.Rotmat[1].X, rotz);
                RotateXFirst(ref obj.Rotmat[0].Y, ref obj.Rotmat[1].Y, rotz);
                RotateXFirst(ref obj.Rotmat[0].Z, ref obj.Rotmat[1].Z, rotz);

                if (rotz is not 127 and not (-127))
                {
                    obj.rotz -= (rotz < 0) ? -1 : 1;
                }
            }


            /* Orthonormalize the rotation matrix... */

            VectorMaths.TidyMatrix(obj.Rotmat);
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

            if (_gameState.IsAutoPilotOn)     // Don't want it to kill anyone!
            {
                return true;
            }

            fz = universe[sn].Rotmat[2].Z;

            if (fz > -0.90)
            {
                return false;
            }

            vec = VectorMaths.UnitVector(universe[sn].location);

            if (vec.Z < 0.927)
            {
                return false;
            }

            ux = universe[sn].Rotmat[1].X;
            if (ux < 0)
            {
                ux = -ux;
            }

            return ux >= 0.84;
        }

        internal void UpdateAltitude()
        {
            _ship.Altitude = 255;

            if (_gameState.InWitchspace)
            {
                return;
            }

            float x = MathF.Abs(universe[0].location.X);
            float y = MathF.Abs(universe[0].location.Y);
            float z = MathF.Abs(universe[0].location.Z);

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

            if (ship_count[ShipType.Coriolis] != 0 || ship_count[ShipType.Dodec] != 0)
            {
                return;
            }

            float x = MathF.Abs(universe[1].location.X);
            float y = MathF.Abs(universe[1].location.Y);
            float z = MathF.Abs(universe[1].location.Z);

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

        private void MakeStationAppear()
        {
            Vector3 location = universe[0].location;
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

            //	VectorMaths.set_init_matrix (rotmat);
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
            _ship.DamageShip(5, universe[i].location.Z > 0);
            _audio.PlayEffect(SoundEffect.Crash);
        }

        private void SwitchToView(ref UniverseObject flip)
        {
            float tmp;

            if (_gameState.CurrentScreen is SCR.SCR_REAR_VIEW or SCR.SCR_GAME_OVER)
            {
                flip.location.X = -flip.location.X;
                flip.location.Z = -flip.location.Z;

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
                tmp = flip.location.X;
                flip.location.X = flip.location.Z;
                flip.location.Z = -tmp;

                if (flip.type < 0)
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
                tmp = flip.location.X;
                flip.location.X = -flip.location.Z;
                flip.location.Z = tmp;

                if (flip.type < 0)
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

        /// <summary>
        /// Update all the objects in the universe and render them.
        /// </summary>
        internal void UpdateUniverse()
        {
            _threed.RenderStart();

            for (int i = 0; i < EliteMain.MAX_UNIV_OBJECTS; i++)
            {
                ShipType type = universe[i].type;

                if (type == ShipType.None)
                {
                    continue;
                }

                if (universe[i].flags.HasFlag(FLG.FLG_REMOVE))
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
                    (!universe[i].flags.HasFlag(FLG.FLG_DEAD)) &&
                    (type != ShipType.Planet) &&
                    (type != ShipType.Sun) &&
                    (type != ShipType.Constrictor) &&
                    (type != ShipType.Cougar) &&
                    (type != ShipType.Coriolis) &&
                    (type != ShipType.Dodec))
                {
                    _audio.PlayEffect(SoundEffect.Explode);
                    universe[i].flags |= FLG.FLG_DEAD;
                }

                if (_gameState.CurrentScreen is
                    not SCR.SCR_INTRO_ONE and
                    not SCR.SCR_INTRO_TWO and
                    not SCR.SCR_GAME_OVER and
                    not SCR.SCR_ESCAPE_CAPSULE)
                {
                    _combat.Tactics(i);
                }

                MoveUniverseObject(ref universe[i]);

                UniverseObject flip = new(universe[i]);
                SwitchToView(ref flip);

                if (type == ShipType.Planet)
                {
                    if ((ship_count[ShipType.Coriolis] == 0) &&
                        (ship_count[ShipType.Dodec] == 0) &&
                        (universe[i].location.Length() < 65792)) // was 49152
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


                if (universe[i].location.Length() < 170)
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

                if (universe[i].location.Length() > 57344)
                {
                    _combat.RemoveShip(i);
                    continue;
                }

                _threed.DrawObject(flip);

                universe[i].flags = flip.flags;
                universe[i].exp_delta = flip.exp_delta;

                universe[i].flags &= ~FLG.FLG_FIRING;

                if (universe[i].flags.HasFlag(FLG.FLG_DEAD))
                {
                    continue;
                }

                _combat.CheckTarget(i, ref flip);
            }

            _threed.RenderEnd();
            _gameState.DetonateBomb = false;
        }

        internal void StartHyperspace()
        {
            if (hyper_ready)
            {
                return;
            }

            hyper_distance = Planet.CalculateDistanceToPlanet(_gameState.DockedPlanet, _gameState.HyperspacePlanet);

            if ((hyper_distance == 0) || (hyper_distance > _ship.Fuel))
            {
                return;
            }

            destination_planet = (GalaxySeed)_gameState.HyperspacePlanet.Clone();
            hyper_name = _planet.NamePlanet(destination_planet, true);
            hyper_ready = true;
            hyper_countdown = 15;
            hyper_galactic = false;

            _pilot.DisengageAutoPilot();
        }

        internal void StartGalacticHyperspace()
        {
            if (hyper_ready)
            {
                return;
            }

            if (!_ship.hasGalacticHyperdrive)
            {
                return;
            }

            hyper_ready = true;
            hyper_countdown = 2;
            hyper_galactic = true;
            _pilot.DisengageAutoPilot();
        }

        internal void DisplayHyperStatus() => _gfx.DrawTextRight(22, 5, $"{hyper_countdown}", GFX_COL.GFX_COL_WHITE);

        private static int RotateByteLeft(int x) => ((x << 1) | (x >> 7)) & 255;

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
                F = RotateByteLeft(_gameState.Cmdr.Galaxy.F)
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
            Stars.CreateNewStars();
            _combat.ClearUniverse();

            int nthg = RNG.Random(1, 4);

            for (int i = 0; i < nthg; i++)
            {
                _combat.CreateThargoid();
            }

            _gameState.SetView(SCR.SCR_HYPERSPACE);
        }

        private void CompleteHyperspace()
        {
            hyper_ready = false;
            _gameState.InWitchspace = false;

            if (hyper_galactic)
            {
                _ship.hasGalacticHyperdrive = false;
                hyper_galactic = false;
                EnterNextGalaxy();
                _gameState.Cmdr.LegalStatus = 0;
            }
            else
            {
                _ship.Fuel -= hyper_distance;
                _gameState.Cmdr.LegalStatus /= 2;

                if ((RNG.Random(255) > 253) || (_ship.Climb >= _ship.MaxClimb))
                {
                    EnterWitchspace();
                    return;
                }

                _gameState.DockedPlanet = (GalaxySeed)destination_planet.Clone();
            }

            _trade._marketRandomiser = RNG.Random(255);
            _gameState.CurrentPlanetData = Planet.GeneratePlanetData(_gameState.DockedPlanet);
            _trade.GenerateStockMarket(_gameState.CurrentPlanetData);

            _ship.Speed = 12;
            _ship.Roll = 0;
            _ship.Climb = 0;
            Stars.CreateNewStars();
            _combat.ClearUniverse();

            _threed.GenerateLandscape((_gameState.DockedPlanet.A * 251) + _gameState.DockedPlanet.B);

            Vector3 position = new()
            {
                Z = (((_gameState.DockedPlanet.B) & 7) + 7) / 2
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

        internal void CountdownHyperspace()
        {
            if (hyper_countdown == 0)
            {
                CompleteHyperspace();
                return;
            }

            hyper_countdown--;
        }

        internal void JumpWarp()
        {
            int i;
            ShipType type;
            float jump;

            for (i = 0; i < EliteMain.MAX_UNIV_OBJECTS; i++)
            {
                type = universe[i].type;

                if (type is > 0 and not ShipType.Asteroid and not ShipType.Cargo and
                    not ShipType.Alloy and not ShipType.Rock and
                    not ShipType.Boulder and not ShipType.EscapeCapsule)
                {
                    _gameState.InfoMessage("Mass Locked");
                    return;
                }
            }

            if ((universe[0].location.Length() < 75001) || (universe[1].location.Length() < 75001))
            {
                _gameState.InfoMessage("Mass Locked");
                return;
            }

            jump = universe[0].location.Length() < universe[1].location.Length() ? universe[0].location.Length() - 75000f : universe[1].location.Length() - 75000f;

            if (jump > 1024)
            {
                jump = 1024;
            }

            for (i = 0; i < EliteMain.MAX_UNIV_OBJECTS; i++)
            {
                if (universe[i].type != 0)
                {
                    universe[i].location.Z -= jump;
                }
            }

            Stars.warp_stars = true;
            _gameState.mcount &= 63;
            _combat.InBattle = false;
        }

        internal void LaunchPlayer()
        {
            _ship.Speed = 12;
            // Rotate in the same direction that the station is spinning
            _ship.Roll = 15;
            _ship.Climb = 0;
            _gameState.Cmdr.LegalStatus |= _trade.IsCarryingContraband();
            Stars.CreateNewStars();
            _threed.GenerateLandscape((_gameState.DockedPlanet.A * 251) + _gameState.DockedPlanet.B);
            _combat.AddNewShip(ShipType.Planet, new(0, 0, 65536), VectorMaths.GetInitialMatrix(), 0, 0);

            Vector3[] rotmat = VectorMaths.GetInitialMatrix();
            rotmat[2].X = -rotmat[2].X;
            rotmat[2].Y = -rotmat[2].Y;
            rotmat[2].Z = -rotmat[2].Z;
            _combat.AddNewStation(new(0, 0, -256), rotmat);

            _gameState.IsDocked = false;
        }

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
            if (ship_count[ShipType.Coriolis] != 0 || ship_count[ShipType.Dodec] != 0)
            {
                _gameState.SetView(SCR.SCR_DOCKING);
            }
        }
    }
}
