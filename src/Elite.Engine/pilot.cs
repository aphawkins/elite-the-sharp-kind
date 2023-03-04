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

	internal class pilot
	{
		private readonly GameState _gameState;
        private readonly Audio _audio;

		internal pilot(GameState gameState, Audio audio)
		{
			_gameState = gameState;
			_audio = audio;
		}

        /*
		 * Fly to a given point in space.
		 */
        private static void fly_to_vector(ref univ_object ship, Vector3 vec)
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

			nvec = VectorMaths.unit_vector(vec);
			direction = VectorMaths.vector_dot_product(nvec, ship.rotmat[2]);

			if (direction < -0.6666)
			{
				rat2 = 0;
			}

			dir = VectorMaths.vector_dot_product(nvec, ship.rotmat[1]);

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
				dir = VectorMaths.vector_dot_product(nvec, ship.rotmat[0]);
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

        /*
		 * Fly towards the planet.
		 */
        private static void fly_to_planet(ref univ_object ship)
		{
			Vector3 vec;

			vec.X = space.universe[0].location.X - ship.location.X;
			vec.Y = space.universe[0].location.Y - ship.location.Y;
			vec.Z = space.universe[0].location.Z - ship.location.Z;

			fly_to_vector(ref ship, vec);
		}

        /*
		 * Fly to a point in front of the station docking bay.
		 * Done prior to the final stage of docking.
		 */
        private static void fly_to_station_front(ref univ_object ship)
		{
			Vector3 vec;

			vec.X = space.universe[1].location.X - ship.location.X;
			vec.Y = space.universe[1].location.Y - ship.location.Y;
			vec.Z = space.universe[1].location.Z - ship.location.Z;

			vec.X += space.universe[1].rotmat[2].X * 768;
			vec.Y += space.universe[1].rotmat[2].Y * 768;
			vec.Z += space.universe[1].rotmat[2].Z * 768;

			fly_to_vector(ref ship, vec);
		}

        /*
		 * Fly towards the space station.
		 */
        private static void fly_to_station(ref univ_object ship)
		{
			Vector3 vec;

			vec.X = space.universe[1].location.X - ship.location.X;
			vec.Y = space.universe[1].location.Y - ship.location.Y;
			vec.Z = space.universe[1].location.Z - ship.location.Z;

			fly_to_vector(ref ship, vec);
		}

        /*
		 * Final stage of docking.
		 * Fly into the docking bay.
		 */
        private static void fly_to_docking_bay(ref univ_object ship)
		{
			Vector3 diff;
			float dir;

			diff.X = ship.location.X - space.universe[1].location.X;
			diff.Y = ship.location.Y - space.universe[1].location.Y;
			diff.Z = ship.location.Z - space.universe[1].location.Z;

			Vector3 vec = VectorMaths.unit_vector(diff);

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

			dir = VectorMaths.vector_dot_product(ship.rotmat[0], space.universe[1].rotmat[1]);

			if (MathF.Abs(dir) >= 0.9166f)
			{
				ship.acceleration++;
				ship.rotz = 127;
				return;
			}

			ship.acceleration = 0;
			ship.rotz = 0;
		}

		/*
		 * Fly a ship to the planet or to the space station and dock it.
		 */
		internal static void auto_pilot_ship(ref univ_object ship)
		{
			Vector3 diff;
			Vector3 vec;
			float dist;
			float dir;

			if (ship.flags.HasFlag(FLG.FLG_FLY_TO_PLANET) ||
				((space.ship_count[SHIP.SHIP_CORIOLIS] == 0) && (space.ship_count[SHIP.SHIP_DODEC] == 0)))
			{
				fly_to_planet(ref ship);
				return;
			}

			diff.X = ship.location.X - space.universe[1].location.X;
			diff.Y = ship.location.Y - space.universe[1].location.Y;
			diff.Z = ship.location.Z - space.universe[1].location.Z;

			dist = MathF.Sqrt((diff.X * diff.X) + (diff.Y * diff.Y) + (diff.Z * diff.Z));

			if (dist < 160)
			{
				ship.flags |= FLG.FLG_REMOVE;       // Ship has docked.
				return;
			}

			vec = VectorMaths.unit_vector(diff);
			dir = VectorMaths.vector_dot_product(space.universe[1].rotmat[2], vec);

			if (dir < 0.9722)
			{
				fly_to_station_front(ref ship);
				return;
			}

			dir = VectorMaths.vector_dot_product(ship.rotmat[2], vec);

			if (dir < -0.9444)
			{
				fly_to_docking_bay(ref ship);
				return;
			}

			fly_to_station(ref ship);
		}

		internal void engage_auto_pilot()
		{
			if (elite.auto_pilot || _gameState.witchspace || space.hyper_ready)
			{
				return;
			}

			elite.auto_pilot = true;
			_audio.PlayMusic(Music.BlueDanube, true);
		}

		internal void disengage_auto_pilot()
		{
			if (elite.auto_pilot)
			{
				elite.auto_pilot = false;
				_audio.StopMusic();
			}
		}
	}
}