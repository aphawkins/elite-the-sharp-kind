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

//# include <math.h>
//# include <stdlib.h>
//# include <string.h>
//# include "config.h"
//# include "gfx.h"
//# include "elite.h"
//# include "vector.h"
//# include "main.h"
//# include "space.h"
//# include "sound.h"

namespace Elite
{
	using Elite.Enums;
	using Elite.Structs;

	internal static class pilot
	{
		/*
		 * Fly to a given point in space.
		 */
		static void fly_to_vector(ref univ_object ship, Vector vec)
		{
			Vector nvec;
			double direction;
			double dir;
			int rat;
			double rat2;
			double cnt2;

			rat = 3;
			rat2 = 0.1666;
			cnt2 = 0.8055;

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

			if ((Math.Abs(dir) * 2) >= rat2)
			{
				ship.rotx = (dir < 0) ? rat : -rat;
			}

			if (Math.Abs(ship.rotz) < 16)
			{
				dir = VectorMaths.vector_dot_product(nvec, ship.rotmat[0]);

				ship.rotz = 0;

				if ((Math.Abs(dir) * 2) >= rat2)
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
		static void fly_to_planet(ref univ_object ship)
		{
			Vector vec;

			vec.x = space.universe[0].location.x - ship.location.x;
			vec.y = space.universe[0].location.y - ship.location.y;
			vec.z = space.universe[0].location.z - ship.location.z;

			fly_to_vector(ref ship, vec);
		}

		/*
		 * Fly to a point in front of the station docking bay.
		 * Done prior to the final stage of docking.
		 */
		static void fly_to_station_front(ref univ_object ship)
		{
			Vector vec;

			vec.x = space.universe[1].location.x - ship.location.x;
			vec.y = space.universe[1].location.y - ship.location.y;
			vec.z = space.universe[1].location.z - ship.location.z;

			vec.x += space.universe[1].rotmat[2].x * 768;
			vec.y += space.universe[1].rotmat[2].y * 768;
			vec.z += space.universe[1].rotmat[2].z * 768;

			fly_to_vector(ref ship, vec);
		}

		/*
		 * Fly towards the space station.
		 */
		static void fly_to_station(ref univ_object ship)
		{
			Vector vec;

			vec.x = space.universe[1].location.x - ship.location.x;
			vec.y = space.universe[1].location.y - ship.location.y;
			vec.z = space.universe[1].location.z - ship.location.z;

			fly_to_vector(ref ship, vec);
		}

		/*
		 * Final stage of docking.
		 * Fly into the docking bay.
		 */
		static void fly_to_docking_bay(ref univ_object ship)
		{
			Vector diff;
			double dir;

			diff.x = ship.location.x - space.universe[1].location.x;
			diff.y = ship.location.y - space.universe[1].location.y;
			diff.z = ship.location.z - space.universe[1].location.z;

			Vector vec = VectorMaths.unit_vector(diff);

			ship.rotx = 0;

			if (ship.type < 0)
			{
				ship.rotz = 1;
				if (((vec.x >= 0) && (vec.y >= 0)) ||
					 ((vec.x < 0) && (vec.y < 0)))
				{
					ship.rotz = -ship.rotz;
				}

				if (Math.Abs(vec.x) >= 0.0625)
				{
					ship.acceleration = 0;
					ship.velocity = 1;
					return;
				}

				if (Math.Abs(vec.y) > 0.002436)
				{
					ship.rotx = (vec.y < 0) ? -1 : 1;
				}

				if (Math.Abs(vec.y) >= 0.0625)
				{
					ship.acceleration = 0;
					ship.velocity = 1;
					return;
				}
			}

			ship.rotz = 0;

			dir = VectorMaths.vector_dot_product(ship.rotmat[0], space.universe[1].rotmat[1]);

			if (Math.Abs(dir) >= 0.9166)
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
			Vector diff;
			Vector vec;
			double dist;
			double dir;

			if (ship.flags.HasFlag(FLG.FLG_FLY_TO_PLANET) ||
				((space.ship_count[(int)SHIP.SHIP_CORIOLIS] == 0) && (space.ship_count[(int)SHIP.SHIP_DODEC] == 0)))
			{
				fly_to_planet(ref ship);
				return;
			}

			diff.x = ship.location.x - space.universe[1].location.x;
			diff.y = ship.location.y - space.universe[1].location.y;
			diff.z = ship.location.z - space.universe[1].location.z;

			dist = Math.Sqrt(diff.x * diff.x + diff.y * diff.y + diff.z * diff.z);

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

		internal static void engage_auto_pilot()
		{
			if (elite.auto_pilot || elite.witchspace || space.hyper_ready)
			{
				return;
			}

			elite.auto_pilot = true;
			elite.sound.PlayMidi(SND.SND_BLUE_DANUBE, true);
		}

		internal static void disengage_auto_pilot()
		{
			if (elite.auto_pilot)
			{
				elite.auto_pilot = false;
				elite.sound.StopMidi();
			}
		}
	}
}