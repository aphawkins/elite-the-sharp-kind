// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using EliteSharp.Audio;
using EliteSharp.Graphics;
using EliteSharp.Ships;

namespace EliteSharp
{
    /// <summary>
    /// The auto-pilot code.  Used for docking computers and for flying other ships to and from the space station.
    /// </summary>
    internal sealed class Pilot
    {
        private readonly IDraw _draw;
        private readonly AudioController _audio;
        private readonly PlayerShip _ship;
        private readonly Universe _universe;

        internal Pilot(IDraw draw, AudioController audio, Universe universe, PlayerShip ship)
        {
            _draw = draw;
            _audio = audio;
            _universe = universe;
            _ship = ship;
        }

        internal bool IsAutoPilotOn { get; private set; }

        internal void AutoDock()
        {
            IShip ship = new ShipBase(_draw)
            {
                Rotmat = VectorMaths.GetInitialMatrix(),
                Location = Vector3.Zero,
                Acceleration = 0,

                //Type = (ShipType)(-96),
                Velocity = _ship.Speed,
                Bravery = 0,
                RotZ = 0,
                RotX = 0,
            };

            ship.Rotmat[0].X = -1;
            ship.Rotmat[2].Z = 1;

            AutoPilotShip(ship);

            _ship.Speed = ship.Velocity > 22 ? 22 : ship.Velocity;

            if (ship.Acceleration > 0)
            {
                _ship.Speed++;
                if (_ship.Speed > 22)
                {
                    _ship.Speed = 22;
                }
            }

            if (ship.Acceleration < 0)
            {
                _ship.Speed--;
                if (_ship.Speed < 1)
                {
                    _ship.Speed = 1;
                }
            }

            if (ship.RotX == 0)
            {
                _ship.Climb = 0;
            }

            if (ship.RotX < 0)
            {
                _ship.IncreaseClimb();

                if (ship.RotX < -1)
                {
                    _ship.IncreaseClimb();
                }
            }

            if (ship.RotX > 0)
            {
                _ship.DecreaseClimb();

                if (ship.RotX > 1)
                {
                    _ship.DecreaseClimb();
                }
            }

            if (ship.RotZ == 127)
            {
                _ship.Roll = -14;
            }
            else if (ship.RotZ == 0)
            {
                _ship.Roll = 0;
            }
            else if (ship.RotZ > 0)
            {
                _ship.IncreaseRoll();

                if (ship.RotZ > 1)
                {
                    _ship.IncreaseRoll();
                }
            }
            else if (ship.RotZ < 0)
            {
                _ship.DecreaseRoll();

                if (ship.RotZ < -1)
                {
                    _ship.DecreaseRoll();
                }
            }
        }

        /// <summary>
        /// Fly a ship to the planet or to the space station and dock it.
        /// </summary>
        internal void AutoPilotShip(IShip ship)
        {
            if (ship.Flags.HasFlag(ShipProperties.FlyToPlanet) || !_universe.IsStationPresent)
            {
                FlyToPlanet(ship);
                return;
            }

            Vector3 diff = ship.Location - _universe.StationOrSun!.Location;

            float dist = MathF.Sqrt((diff.X * diff.X) + (diff.Y * diff.Y) + (diff.Z * diff.Z));

            if (dist < 160)
            {
                ship.Flags |= ShipProperties.Remove;       // Ship has docked.
                return;
            }

            Vector3 vec = VectorMaths.UnitVector(diff);
            float dir = VectorMaths.VectorDotProduct(_universe.StationOrSun.Rotmat[2], vec);

            if (dir < 0.9722)
            {
                FlyToStationFront(ship);
                return;
            }

            dir = VectorMaths.VectorDotProduct(ship.Rotmat[2], vec);

            if (dir < -0.9444)
            {
                FlyToDockingBay(ship);
                return;
            }

            FlyToStation(ship);
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
                _audio.PlayMusic(MusicType.BlueDanube, true);
            }
        }

        internal void Reset() => IsAutoPilotOn = false;

        /// <summary>
        /// Fly to a given point in space.
        /// </summary>
        /// <param name="ship"></param>
        /// <param name="vec"></param>
        private static void FlyToVector(IShip ship, Vector3 vec)
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

        /// <summary>
        /// Final stage of docking. Fly into the docking bay.
        /// </summary>
        /// <param name="ship"></param>
        private void FlyToDockingBay(IShip ship)
        {
            Vector3 diff = ship.Location - _universe.StationOrSun!.Location;
            Vector3 vec = VectorMaths.UnitVector(diff);
            ship.RotX = 0;

            if (ship.Type == ShipType.None)
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

            float dir = VectorMaths.VectorDotProduct(ship.Rotmat[0], _universe.StationOrSun.Rotmat[1]);

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
        private void FlyToPlanet(IShip ship)
        {
            if (_universe.Planet == null)
            {
                return;
            }

            Vector3 vec = _universe.Planet.Location - ship.Location;
            FlyToVector(ship, vec);
        }

        /// <summary>
        /// Fly towards the space station.
        /// </summary>
        private void FlyToStation(IShip ship)
        {
            Vector3 vec = _universe.StationOrSun!.Location - ship.Location;
            FlyToVector(ship, vec);
        }

        /// <summary>
        /// Fly to a point in front of the station docking bay. Done prior to the final stage of docking.
        /// </summary>
        private void FlyToStationFront(IShip ship)
        {
            Vector3 vec = _universe.StationOrSun!.Location - ship.Location;

            vec.X += _universe.StationOrSun.Rotmat[2].X * 768;
            vec.Y += _universe.StationOrSun.Rotmat[2].Y * 768;
            vec.Z += _universe.StationOrSun.Rotmat[2].Z * 768;

            FlyToVector(ship, vec);
        }
    }
}
