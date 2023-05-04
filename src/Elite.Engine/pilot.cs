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

/*
 * pilot.c
 *
 * The auto-pilot code.  Used for docking computers and for
 * flying other ships to and from the space station.
 */

/*
 * In the original Elite this code was mixed in with the tactics routines.
 * I have split it out to make it more understandable and easier to maintain.
 */

namespace Elite.Engine
{
    using System.Numerics;
    using Elite.Common.Enums;
	using Elite.Engine.Enums;
	using Elite.Engine.Types;

	internal class Pilot
	{
		private readonly GameState _gameState;
        private readonly Audio _audio;

		internal Pilot(GameState gameState, Audio audio)
		{
			_gameState = gameState;
			_audio = audio;
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
			direction = VectorMaths.VectorDotProduct(nvec, ship.rotmat[2]);

			if (direction < -0.6666)
			{
				rat2 = 0;
			}

			dir = VectorMaths.VectorDotProduct(nvec, ship.rotmat[1]);

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
				dir = VectorMaths.VectorDotProduct(nvec, ship.rotmat[0]);
				ship.rotz = 0;

				if ((MathF.Abs(dir) * 2) >= rat2)
				{
					ship.rotz = (dir < 0) ? rat : -rat;

					if (ship.rotx < 0)
					{
						ship.rotz = -ship.rotz;
					}
				}
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
		}

		/// <summary>
		/// Fly towards the planet.
		/// </summary>
		/// <param name="ship"></param>
        private static void FlyToPlanet(ref UniverseObject ship)
		{
			Vector3 vec;

			vec.X = Space.universe[0].location.X - ship.location.X;
			vec.Y = Space.universe[0].location.Y - ship.location.Y;
			vec.Z = Space.universe[0].location.Z - ship.location.Z;

			FlyToVector(ref ship, vec);
		}

		/// <summary>
		/// Fly to a point in front of the station docking bay. Done prior to the final stage of docking.
		/// </summary>
		/// <param name="ship"></param>
        private static void FlyToStationFront(ref UniverseObject ship)
		{
			Vector3 vec;

			vec.X = Space.universe[1].location.X - ship.location.X;
			vec.Y = Space.universe[1].location.Y - ship.location.Y;
			vec.Z = Space.universe[1].location.Z - ship.location.Z;

			vec.X += Space.universe[1].rotmat[2].X * 768;
			vec.Y += Space.universe[1].rotmat[2].Y * 768;
			vec.Z += Space.universe[1].rotmat[2].Z * 768;

			FlyToVector(ref ship, vec);
		}

        /// <summary>
        /// Fly towards the space station.
        /// </summary>
        /// <param name="ship"></param>
        private static void FlyToStation(ref UniverseObject ship)
		{
			Vector3 vec;

			vec.X = Space.universe[1].location.X - ship.location.X;
			vec.Y = Space.universe[1].location.Y - ship.location.Y;
			vec.Z = Space.universe[1].location.Z - ship.location.Z;

			FlyToVector(ref ship, vec);
		}

        /// <summary>
        /// Final stage of docking. Fly into the docking bay.
        /// </summary>
        /// <param name="ship"></param>
        private static void FlyToDockingBay(ref UniverseObject ship)
		{
			Vector3 diff;
			float dir;

			diff.X = ship.location.X - Space.universe[1].location.X;
			diff.Y = ship.location.Y - Space.universe[1].location.Y;
			diff.Z = ship.location.Z - Space.universe[1].location.Z;

			Vector3 vec = VectorMaths.UnitVector(diff);

			ship.rotx = 0;

			if (ship.type < 0)
			{
				ship.rotz = 1;
				if (((vec.X >= 0) && (vec.Y >= 0)) ||
					 ((vec.X < 0) && (vec.Y < 0)))
				{
					ship.rotz = -ship.rotz;
				}

				if (MathF.Abs(vec.X) >= 0.0625f)
				{
					ship.acceleration = 0;
					ship.velocity = 1;
					return;
				}

				if (MathF.Abs(vec.Y) > 0.002436f)
				{
					ship.rotx = (vec.Y < 0) ? -1 : 1;
				}

				if (MathF.Abs(vec.Y) >= 0.0625f)
				{
					ship.acceleration = 0;
					ship.velocity = 1;
					return;
				}
			}

			ship.rotz = 0;

			dir = VectorMaths.VectorDotProduct(ship.rotmat[0], Space.universe[1].rotmat[1]);

			if (MathF.Abs(dir) >= 0.9166f)
			{
				ship.acceleration++;
				ship.rotz = 127;
				return;
			}

			ship.acceleration = 0;
			ship.rotz = 0;
		}

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

			if (ship.flags.HasFlag(FLG.FLG_FLY_TO_PLANET) ||
				((Space.ship_count[ShipType.Coriolis] == 0) && (Space.ship_count[ShipType.Dodec] == 0)))
			{
				FlyToPlanet(ref ship);
				return;
			}

			diff.X = ship.location.X - Space.universe[1].location.X;
			diff.Y = ship.location.Y - Space.universe[1].location.Y;
			diff.Z = ship.location.Z - Space.universe[1].location.Z;

			dist = MathF.Sqrt((diff.X * diff.X) + (diff.Y * diff.Y) + (diff.Z * diff.Z));

			if (dist < 160)
			{
				ship.flags |= FLG.FLG_REMOVE;       // Ship has docked.
				return;
			}

			vec = VectorMaths.UnitVector(diff);
			dir = VectorMaths.VectorDotProduct(Space.universe[1].rotmat[2], vec);

			if (dir < 0.9722)
			{
				FlyToStationFront(ref ship);
				return;
			}

			dir = VectorMaths.VectorDotProduct(ship.rotmat[2], vec);

			if (dir < -0.9444)
			{
				FlyToDockingBay(ref ship);
				return;
			}

			FlyToStation(ref ship);
		}

		internal void EngageAutoPilot()
		{
			if (EliteMain.auto_pilot || _gameState.witchspace || Space.hyper_ready)
			{
				return;
			}

			EliteMain.auto_pilot = true;
			_audio.PlayMusic(Music.BlueDanube, true);
		}

		internal void DisengageAutoPilot()
		{
			if (EliteMain.auto_pilot)
			{
				EliteMain.auto_pilot = false;
				_audio.StopMusic();
			}
		}
	}
}