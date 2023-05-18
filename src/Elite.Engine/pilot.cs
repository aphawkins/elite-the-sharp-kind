// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using Elite.Common.Enums;
using Elite.Engine.Ships;
using Elite.Engine.Types;

namespace Elite.Engine
{
    /// <summary>
    /// The auto-pilot code.  Used for docking computers and for flying other ships to and from the space station.
    /// </summary>
    internal sealed class Pilot
    {
        private readonly Audio _audio;

        internal Pilot(Audio audio) => _audio = audio;

        internal bool IsAutoPilotOn { get; private set; }

        internal void Reset() => IsAutoPilotOn = false;

        /// <summary>
        /// Fly a ship to the planet or to the space station and dock it.
        /// </summary>
        /// <param name="ship"></param>
        internal static void AutoPilotShip(ref UniverseObject ship)
        {
            Vector3 diff;
            Vector3 vec;
            float dist;
            float dir;

            if (ship.Flags.HasFlag(ShipFlags.FlyToPlanet) ||
                ((Space.s_ship_count[ShipType.Coriolis] == 0) && (Space.s_ship_count[ShipType.Dodec] == 0)))
            {
                FlyToPlanet(ref ship);
                return;
            }

            diff.X = ship.Location.X - Space.s_universe[1].Location.X;
            diff.Y = ship.Location.Y - Space.s_universe[1].Location.Y;
            diff.Z = ship.Location.Z - Space.s_universe[1].Location.Z;

            dist = MathF.Sqrt((diff.X * diff.X) + (diff.Y * diff.Y) + (diff.Z * diff.Z));

            if (dist < 160)
            {
                ship.Flags |= ShipFlags.Remove;       // Ship has docked.
                return;
            }

            vec = VectorMaths.UnitVector(diff);
            dir = VectorMaths.VectorDotProduct(Space.s_universe[1].Rotmat[2], vec);

            if (dir < 0.9722)
            {
                FlyToStationFront(ref ship);
                return;
            }

            dir = VectorMaths.VectorDotProduct(ship.Rotmat[2], vec);

            if (dir < -0.9444)
            {
                FlyToDockingBay(ref ship);
                return;
            }

            FlyToStation(ref ship);
        }

        internal void DisengageAutoPilot()
        {
            if (IsAutoPilotOn)
            {
                IsAutoPilotOn = false;
                _audio.StopMusic();
            }
        }

        internal void EngageAutoPilot()
        {
            if (!IsAutoPilotOn)
            {
                IsAutoPilotOn = true;
                _audio.PlayMusic(Music.BlueDanube, true);
            }
        }

        /// <summary>
        /// Final stage of docking. Fly into the docking bay.
        /// </summary>
        /// <param name="ship"></param>
        private static void FlyToDockingBay(ref UniverseObject ship)
        {
            Vector3 diff;
            float dir;

            diff.X = ship.Location.X - Space.s_universe[1].Location.X;
            diff.Y = ship.Location.Y - Space.s_universe[1].Location.Y;
            diff.Z = ship.Location.Z - Space.s_universe[1].Location.Z;

            Vector3 vec = VectorMaths.UnitVector(diff);

            ship.RotX = 0;

            if (ship.Type < 0)
            {
                ship.RotZ = 1;
                if (((vec.X >= 0) && (vec.Y >= 0)) ||
                     ((vec.X < 0) && (vec.Y < 0)))
                {
                    ship.RotZ = -ship.RotZ;
                }

                if (MathF.Abs(vec.X) >= 0.0625f)
                {
                    ship.Acceleration = 0;
                    ship.Velocity = 1;
                    return;
                }

                if (MathF.Abs(vec.Y) > 0.002436f)
                {
                    ship.RotX = (vec.Y < 0) ? -1 : 1;
                }

                if (MathF.Abs(vec.Y) >= 0.0625f)
                {
                    ship.Acceleration = 0;
                    ship.Velocity = 1;
                    return;
                }
            }

            ship.RotZ = 0;

            dir = VectorMaths.VectorDotProduct(ship.Rotmat[0], Space.s_universe[1].Rotmat[1]);

            if (MathF.Abs(dir) >= 0.9166f)
            {
                ship.Acceleration++;
                ship.RotZ = 127;
                return;
            }

            ship.Acceleration = 0;
            ship.RotZ = 0;
        }

        /// <summary>
        /// Fly towards the planet.
        /// </summary>
        /// <param name="ship"></param>
        private static void FlyToPlanet(ref UniverseObject ship)
        {
            Vector3 vec;

            vec.X = Space.s_universe[0].Location.X - ship.Location.X;
            vec.Y = Space.s_universe[0].Location.Y - ship.Location.Y;
            vec.Z = Space.s_universe[0].Location.Z - ship.Location.Z;

            FlyToVector(ref ship, vec);
        }

        /// <summary>
        /// Fly towards the space station.
        /// </summary>
        /// <param name="ship"></param>
        private static void FlyToStation(ref UniverseObject ship)
        {
            Vector3 vec;

            vec.X = Space.s_universe[1].Location.X - ship.Location.X;
            vec.Y = Space.s_universe[1].Location.Y - ship.Location.Y;
            vec.Z = Space.s_universe[1].Location.Z - ship.Location.Z;

            FlyToVector(ref ship, vec);
        }

        /// <summary>
        /// Fly to a point in front of the station docking bay. Done prior to the final stage of docking.
        /// </summary>
        /// <param name="ship"></param>
        private static void FlyToStationFront(ref UniverseObject ship)
        {
            Vector3 vec;

            vec.X = Space.s_universe[1].Location.X - ship.Location.X;
            vec.Y = Space.s_universe[1].Location.Y - ship.Location.Y;
            vec.Z = Space.s_universe[1].Location.Z - ship.Location.Z;

            vec.X += Space.s_universe[1].Rotmat[2].X * 768;
            vec.Y += Space.s_universe[1].Rotmat[2].Y * 768;
            vec.Z += Space.s_universe[1].Rotmat[2].Z * 768;

            FlyToVector(ref ship, vec);
        }

        /// <summary>
        /// Fly to a given point in space.
        /// </summary>
        /// <param name="ship"></param>
        /// <param name="vec"></param>
        private static void FlyToVector(ref UniverseObject ship, Vector3 vec)
        {
            Vector3 nvec;
            float direction;
            float dir;
            int rat;
            float rat2;
            float cnt2;

            rat = 3;
            rat2 = 0.1666f;
            cnt2 = 0.8055f;

            nvec = VectorMaths.UnitVector(vec);
            direction = VectorMaths.VectorDotProduct(nvec, ship.Rotmat[2]);

            if (direction < -0.6666)
            {
                rat2 = 0;
            }

            dir = VectorMaths.VectorDotProduct(nvec, ship.Rotmat[1]);

            if (direction < -0.861)
            {
                ship.RotX = (dir < 0) ? 7 : -7;
                ship.RotZ = 0;
                return;
            }

            ship.RotX = 0;

            if ((MathF.Abs(dir) * 2) >= rat2)
            {
                ship.RotX = (dir < 0) ? rat : -rat;
            }

            if (MathF.Abs(ship.RotZ) < 16)
            {
                dir = VectorMaths.VectorDotProduct(nvec, ship.Rotmat[0]);
                ship.RotZ = 0;

                if ((MathF.Abs(dir) * 2) >= rat2)
                {
                    ship.RotZ = (dir < 0) ? rat : -rat;

                    if (ship.RotX < 0)
                    {
                        ship.RotZ = -ship.RotZ;
                    }
                }
            }

            if (direction <= -0.167)
            {
                ship.Acceleration = -1;
                return;
            }

            if (direction >= cnt2)
            {
                ship.Acceleration = 3;
            }
        }
    }
}
