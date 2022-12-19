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
 * space.c
 *
 * This module handles all the flight system and management of the space universe.
 */

//# include <stdio.h>
//# include <string.h>
//# include <math.h>
//# include <stdlib.h>

//# include "vector.h"

//# include "alg_data.h"

//# include "config.h"
//# include "elite.h"
//# include "gfx.h"
//# include "docked.h"
//# include "intro.h"
//# include "shipdata.h"
//# include "shipface.h"
//# include "space.h" 
//# include "threed.h"
//# include "sound.h"
//# include "main.h"
//# include "swat.h"
//# include "random.h"
//# include "trade.h"
//# include "stars.h"
//# include "pilot.h"

namespace Elite
{
	using Elite.Enums;
	using Elite.Ships;
	using Elite.Structs;

	internal static class space
	{
		static galaxy_seed destination_planet;
		internal static bool hyper_ready;
		static int hyper_countdown;
		static string hyper_name;
		static int hyper_distance;
		static bool hyper_galactic;
		internal static univ_object[] universe = new univ_object[elite.MAX_UNIV_OBJECTS];
		internal static int[] ship_count = new int[shipdata.NO_OF_SHIPS + 1];  /* many */

		static void rotate_x_first(ref double a, ref double b, int direction)
		{
			double fx = a;
			double ux = b;

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

		static void rotate_vec(ref Vector vec, double alpha, double beta)
		{
			double x = vec.x;
			double y = vec.y;
			double z = vec.z;

			y = y - alpha * x;
			x = x + alpha * y;
			y = y - beta * z;
			z = z + beta * y;

			vec.x = x;
			vec.y = y;
			vec.z = z;
		}

		/*
		 * Update an objects location in the universe.
		 */
		static void move_univ_object(ref univ_object obj)
		{
			double x, y, z;
			double k2;
			double alpha;
			double beta;
			int rotx, rotz;
			double speed;

			alpha = elite.flight_roll / 256.0;
			beta = elite.flight_climb / 256.0;

			x = obj.location.x;
			y = obj.location.y;
			z = obj.location.z;

			if (!obj.flags.HasFlag(FLG.FLG_DEAD))
			{
				if (obj.velocity != 0)
				{
					speed = obj.velocity;
					speed *= 1.5;
					x += obj.rotmat[2].x * speed;
					y += obj.rotmat[2].y * speed;
					z += obj.rotmat[2].z * speed;
				}

				if (obj.acceleration != 0)
				{
					obj.velocity += obj.acceleration;
					obj.acceleration = 0;
					if (obj.velocity > elite.ship_list[(int)obj.type].velocity)
					{
						obj.velocity = elite.ship_list[(int)obj.type].velocity;
					}

					if (obj.velocity <= 0)
					{
						obj.velocity = 1;
					}
				}
			}

			k2 = y - alpha * x;
			z = z + beta * k2;
			y = k2 - z * beta;
			x = x + alpha * y;

			z = z - elite.flight_speed;

			obj.location.x = x;
			obj.location.y = y;
			obj.location.z = z;

			obj.distance = (int)Math.Sqrt(x * x + y * y + z * z);

			if (obj.type == SHIP.SHIP_PLANET)
			{
				beta = 0.0;
			}

			rotate_vec(ref obj.rotmat[2], alpha, beta);
			rotate_vec(ref obj.rotmat[1], alpha, beta);
			rotate_vec(ref obj.rotmat[0], alpha, beta);

			if (obj.flags.HasFlag(FLG.FLG_DEAD))
			{
				return;
			}

			rotx = obj.rotx;
			rotz = obj.rotz;

			/* If necessary rotate the object around the X axis... */

			if (rotx != 0)
			{
				rotate_x_first(ref obj.rotmat[2].x, ref obj.rotmat[1].x, rotx);
				rotate_x_first(ref obj.rotmat[2].y, ref obj.rotmat[1].y, rotx);
				rotate_x_first(ref obj.rotmat[2].z, ref obj.rotmat[1].z, rotx);

				if ((rotx != 127) && (rotx != -127))
					obj.rotx -= (rotx < 0) ? -1 : 1;
			}


			/* If necessary rotate the object around the Z axis... */

			if (rotz != 0)
			{
				rotate_x_first(ref obj.rotmat[0].x, ref obj.rotmat[1].x, rotz);
				rotate_x_first(ref obj.rotmat[0].y, ref obj.rotmat[1].y, rotz);
				rotate_x_first(ref obj.rotmat[0].z, ref obj.rotmat[1].z, rotz);

				if ((rotz != 127) && (rotz != -127))
				{
					obj.rotz -= (rotz < 0) ? -1 : 1;
				}
			}


			/* Orthonormalize the rotation matrix... */

			VectorMaths.tidy_matrix(obj.rotmat);
		}

		/*
		 * Dock the player into the space station.
		 */
		internal static void dock_player()
		{
			pilot.disengage_auto_pilot();
			elite.docked = true;
			elite.flight_speed = 0;
			elite.flight_roll = 0;
			elite.flight_climb = 0;
			elite.front_shield = 255;
			elite.aft_shield = 255;
			elite.energy = 255;
			elite.myship.altitude = 255;
			elite.myship.cabtemp = 30;
			swat.reset_weapons();
		}

		/*
		 * Check if we are correctly aligned to dock.
		 */
		static bool is_docking(int sn)
		{
			Vector vec;
			double fz;
			double ux;

			if (elite.auto_pilot)     // Don't want it to kill anyone!
			{
				return true;
			}

			fz = universe[sn].rotmat[2].z;

			if (fz > -0.90)
			{
				return false;
			}

			vec = VectorMaths.unit_vector(universe[sn].location);

			if (vec.z < 0.927)
			{
				return false;
			}

			ux = universe[sn].rotmat[1].x;
			if (ux < 0)
			{
				ux = -ux;
			}

			if (ux < 0.84)
			{
				return false;
			}

			return true;
		}

		/*
		 * Game Over...
		 */
		static void do_game_over()
		{
			sound.snd_play_sample(SND.SND_GAMEOVER);
			elite.game_over = true;
		}

		static void update_altitude()
		{
			elite.myship.altitude = 255;

			if (elite.witchspace)
			{
				return;
			}

			double x = Math.Abs(universe[0].location.x);
			double y = Math.Abs(universe[0].location.y);
			double z = Math.Abs(universe[0].location.z);

			if ((x > 65535) || (y > 65535) || (z > 65535))
			{
				return;
			}

			x /= 256;
			y /= 256;
			z /= 256;

			double dist = (x * x) + (y * y) + (z * z);

			if (dist > 65535)
			{
				return;
			}

			dist -= 9472;
			if (dist < 1)
			{
				elite.myship.altitude = 0;
				do_game_over();
				return;
			}

			dist = Math.Sqrt(dist);
			if (dist < 1)
			{
				elite.myship.altitude = 0;
				do_game_over();
				return;
			}

			elite.myship.altitude = (int)dist;
		}


		static void update_cabin_temp()
		{
			int x, y, z;
			int dist;

			elite.myship.cabtemp = 30;

			if (elite.witchspace)
			{
				return;
			}

			if (ship_count[(int)SHIP.SHIP_CORIOLIS] != 0 || ship_count[(int)SHIP.SHIP_DODEC] != 0)
			{
				return;
			}

			x = Math.Abs((int)universe[1].location.x);
			y = Math.Abs((int)universe[1].location.y);
			z = Math.Abs((int)universe[1].location.z);

			if ((x > 65535) || (y > 65535) || (z > 65535))
			{
				return;
			}

			x /= 256;
			y /= 256;
			z /= 256;

			dist = ((x * x) + (y * y) + (z * z)) / 256;

			if (dist > 255)
				return;

			dist ^= 255;

			elite.myship.cabtemp = dist + 30;

			if (elite.myship.cabtemp > 255)
			{
				elite.myship.cabtemp = 255;
				do_game_over();
				return;
			}

			if ((elite.myship.cabtemp < 224) || (!elite.cmdr.fuel_scoop))
			{
				return;
			}

			elite.cmdr.fuel += elite.flight_speed / 2;
			if (elite.cmdr.fuel > elite.myship.max_fuel)
			{
				elite.cmdr.fuel = elite.myship.max_fuel;
			}

			alg_main.info_message("Fuel Scoop On");
		}



		/*
		 * Regenerate the shields and the energy banks.
		 */

		static void regenerate_shields()
		{
			if (elite.energy > 127)
			{
				if (elite.front_shield < 255)
				{
					elite.front_shield++;
					elite.energy--;
				}

				if (elite.aft_shield < 255)
				{
					elite.aft_shield++;
					elite.energy--;
				}
			}

			elite.energy++;
			elite.energy += elite.cmdr.energy_unit;
			if (elite.energy > 255)
			{
				elite.energy = 255;
			}
		}

		internal static void decrease_energy(int amount)
		{
			elite.energy += amount;

			if (elite.energy <= 0)
			{
				do_game_over();
			}
		}

		/*
		 * Deplete the shields.  Drain the energy banks if the shields fail.
		 */
		internal static void damage_ship(int damage, bool front)
		{
			int shield;

			if (damage <= 0)    /* sanity check */
			{
				return;
			}

			shield = front ? elite.front_shield : elite.aft_shield;

			shield -= damage;
			if (shield < 0)
			{
				decrease_energy(shield);
				shield = 0;
			}

			if (front)
			{
				elite.front_shield = shield;
			}
			else
			{
				elite.aft_shield = shield;
			}
		}

		static void make_station_appear()
		{
			double px, py, pz;
			double sx, sy, sz;
			Vector vec;
			Vector[] rotmat = new Vector[3];

			px = universe[0].location.x;
			py = universe[0].location.y;
			pz = universe[0].location.z;

			vec.x = (random.rand() & 32767) - 16384;
			vec.y = (random.rand() & 32767) - 16384;
			vec.z = random.rand() & 32767;

			vec = VectorMaths.unit_vector(vec);

			sx = px - vec.x * 65792;
			sy = py - vec.y * 65792;
			sz = pz - vec.z * 65792;

			//	VectorMaths.set_init_matrix (rotmat);

			rotmat[0].x = 1.0;
			rotmat[0].y = 0.0;
			rotmat[0].z = 0.0;

			rotmat[1].x = vec.x;
			rotmat[1].y = vec.z;
			rotmat[1].z = -vec.y;

			rotmat[2].x = vec.x;
			rotmat[2].y = vec.y;
			rotmat[2].z = vec.z;

			VectorMaths.tidy_matrix(rotmat);

			swat.add_new_station(sx, sy, sz, rotmat);
		}

		static void check_docking(int i)
		{
			if (is_docking(i))
			{
				sound.snd_play_sample(SND.SND_DOCK);
				dock_player();
				elite.current_screen = SCR.SCR_BREAK_PATTERN;
				return;
			}

			if (elite.flight_speed >= 5)
			{
				do_game_over();
				return;
			}

			elite.flight_speed = 1;
			damage_ship(5, universe[i].location.z > 0);
			sound.snd_play_sample(SND.SND_CRASH);
		}

		static void switch_to_view(ref univ_object flip)
		{
			double tmp;

			if ((elite.current_screen == SCR.SCR_REAR_VIEW) ||
				(elite.current_screen == SCR.SCR_GAME_OVER))
			{
				flip.location.x = -flip.location.x;
				flip.location.z = -flip.location.z;

				flip.rotmat[0].x = -flip.rotmat[0].x;
				flip.rotmat[0].z = -flip.rotmat[0].z;

				flip.rotmat[1].x = -flip.rotmat[1].x;
				flip.rotmat[1].z = -flip.rotmat[1].z;

				flip.rotmat[2].x = -flip.rotmat[2].x;
				flip.rotmat[2].z = -flip.rotmat[2].z;
				return;
			}

			if (elite.current_screen == SCR.SCR_LEFT_VIEW)
			{
				tmp = flip.location.x;
				flip.location.x = flip.location.z;
				flip.location.z = -tmp;

				if (flip.type < 0)
					return;

				tmp = flip.rotmat[0].x;
				flip.rotmat[0].x = flip.rotmat[0].z;
				flip.rotmat[0].z = -tmp;

				tmp = flip.rotmat[1].x;
				flip.rotmat[1].x = flip.rotmat[1].z;
				flip.rotmat[1].z = -tmp;

				tmp = flip.rotmat[2].x;
				flip.rotmat[2].x = flip.rotmat[2].z;
				flip.rotmat[2].z = -tmp;
				return;
			}

			if (elite.current_screen == SCR.SCR_RIGHT_VIEW)
			{
				tmp = flip.location.x;
				flip.location.x = -flip.location.z;
				flip.location.z = tmp;

				if (flip.type < 0)
					return;

				tmp = flip.rotmat[0].x;
				flip.rotmat[0].x = -flip.rotmat[0].z;
				flip.rotmat[0].z = tmp;

				tmp = flip.rotmat[1].x;
				flip.rotmat[1].x = -flip.rotmat[1].z;
				flip.rotmat[1].z = tmp;

				tmp = flip.rotmat[2].x;
				flip.rotmat[2].x = -flip.rotmat[2].z;
				flip.rotmat[2].z = tmp;

			}
		}

		/*
		 * Update all the objects in the universe and render them.
		 */
		internal static void update_universe()
		{
			SHIP type;
			int bounty;
			string str;
			univ_object flip;

			alg_gfx.gfx_start_render();

			for (int i = 0; i < elite.MAX_UNIV_OBJECTS; i++)
			{
				type = universe[i].type;

				if (type != 0)
				{
					if (universe[i].flags.HasFlag(FLG.FLG_REMOVE))
					{
						if (type == SHIP.SHIP_VIPER)
						{
							elite.cmdr.legal_status |= 64;
						}

						bounty = elite.ship_list[(int)type].bounty;

						if ((bounty != 0) && (!elite.witchspace))
						{
							elite.cmdr.credits += bounty;
							str = $"{elite.cmdr.credits / 10:d}.{elite.cmdr.credits % 10:d} CR";
							alg_main.info_message(str);
						}

						swat.remove_ship(i);
						continue;
					}

					if (elite.detonate_bomb &&
						(!universe[i].flags.HasFlag(FLG.FLG_DEAD)) &&
						(type != SHIP.SHIP_PLANET) &&
						(type != SHIP.SHIP_SUN) &&
						(type != SHIP.SHIP_CONSTRICTOR) &&
						(type != SHIP.SHIP_COUGAR) &&
						(type != SHIP.SHIP_CORIOLIS) &&
						(type != SHIP.SHIP_DODEC))
					{
						sound.snd_play_sample(SND.SND_EXPLODE);
						universe[i].flags |= FLG.FLG_DEAD;
					}

					if ((elite.current_screen != SCR.SCR_INTRO_ONE) &&
						(elite.current_screen != SCR.SCR_INTRO_TWO) &&
						(elite.current_screen != SCR.SCR_GAME_OVER) &&
						(elite.current_screen != SCR.SCR_ESCAPE_POD))
					{
						swat.tactics(i);
					}

					move_univ_object(ref universe[i]);

					flip = universe[i];
					switch_to_view(ref flip);

					if (type == SHIP.SHIP_PLANET)
					{
						if ((ship_count[(int)SHIP.SHIP_CORIOLIS] == 0) &&
							(ship_count[(int)SHIP.SHIP_DODEC] == 0) &&
							(universe[i].distance < 65792)) // was 49152
						{
							make_station_appear();
						}

						threed.draw_ship(ref flip);
						continue;
					}

					if (type == SHIP.SHIP_SUN)
					{
						threed.draw_ship(ref flip);
						continue;
					}


					if (universe[i].distance < 170)
					{
						if ((type == SHIP.SHIP_CORIOLIS) || (type == SHIP.SHIP_DODEC))
						{
							check_docking(i);
						}
						else
							trade.scoop_item(i);

						continue;
					}

					if (universe[i].distance > 57344)
					{
						swat.remove_ship(i);
						continue;
					}

					threed.draw_ship(ref flip);

					universe[i].flags = flip.flags;
					universe[i].exp_seed = flip.exp_seed;
					universe[i].exp_delta = flip.exp_delta;

					universe[i].flags &= ~FLG.FLG_FIRING;

					if (universe[i].flags.HasFlag(FLG.FLG_DEAD))
					{
						continue;
					}

					swat.check_target(i, ref flip);
				}
			}

			alg_gfx.gfx_finish_render();
			elite.detonate_bomb = false;
		}

		/*
		 * Update the scanner and draw all the lollipops.
		 */
		static void update_scanner()
		{
			int i;
			int x, y, z;
			int x1, y1, y2;
			int colour;

			for (i = 0; i < elite.MAX_UNIV_OBJECTS; i++)
			{
				if ((universe[i].type <= 0) ||
					universe[i].flags.HasFlag(FLG.FLG_DEAD) ||
					universe[i].flags.HasFlag(FLG.FLG_CLOAKED))
				{
					continue;
				}

				x = (int)(universe[i].location.x / 256);
				y = (int)(universe[i].location.y / 256);
				z = (int)(universe[i].location.z / 256);

				x1 = x;
				y1 = -z / 4;
				y2 = y1 - y / 2;

				if ((y2 < -28) || (y2 > 28) ||
					(x1 < -50) || (x1 > 50))
					continue;

				x1 += elite.scanner_cx;
				y1 += elite.scanner_cy;
				y2 += elite.scanner_cy;

				colour = universe[i].flags.HasFlag(FLG.FLG_HOSTILE) ? gfx.GFX_COL_YELLOW_5 : gfx.GFX_COL_WHITE;

				switch (universe[i].type)
				{
					case SHIP.SHIP_MISSILE:
						colour = 137;
						break;

					case SHIP.SHIP_DODEC:
					case SHIP.SHIP_CORIOLIS:
						colour = gfx.GFX_COL_GREEN_1;
						break;

					case SHIP.SHIP_VIPER:
						colour = 252;
						break;
				}

				alg_gfx.gfx_draw_colour_line(x1 + 2, y2, x1 - 3, y2, colour);
				alg_gfx.gfx_draw_colour_line(x1 + 2, y2 + 1, x1 - 3, y2 + 1, colour);
				alg_gfx.gfx_draw_colour_line(x1 + 2, y2 + 2, x1 - 3, y2 + 2, colour);
				alg_gfx.gfx_draw_colour_line(x1 + 2, y2 + 3, x1 - 3, y2 + 3, colour);


				alg_gfx.gfx_draw_colour_line(x1, y1, x1, y2, colour);
				alg_gfx.gfx_draw_colour_line(x1 + 1, y1, x1 + 1, y2, colour);
				alg_gfx.gfx_draw_colour_line(x1 + 2, y1, x1 + 2, y2, colour);
			}
		}

		/*
		 * Update the compass which tracks the space station / planet.
		 */
		static void update_compass()
		{
			Vector dest;
			int compass_x;
			int compass_y;
			int un = 0;

			if (elite.witchspace)
			{
				return;
			}

			if (ship_count[(int)SHIP.SHIP_CORIOLIS] != 0 || ship_count[(int)SHIP.SHIP_DODEC] != 0)
			{
				un = 1;
			}

			dest = VectorMaths.unit_vector(universe[un].location);

			compass_x = (int)(elite.compass_centre_x + (dest.x * 16));
			compass_y = (int)(elite.compass_centre_y + (dest.y * -16));

			if (dest.z < 0)
			{
				alg_gfx.gfx_draw_sprite(gfx.IMG_RED_DOT, compass_x, compass_y);
			}
			else
			{
				alg_gfx.gfx_draw_sprite(gfx.IMG_GREEN_DOT, compass_x, compass_y);
			}

		}

		/*
		 * Display the speed bar.
		 */
		static void display_speed()
		{
			int sx, sy;
			int i;
			int len;
			int colour;

			sx = 417;
			sy = 384 + 9;

			len = ((elite.flight_speed * 64) / elite.myship.max_speed) - 1;

			colour = (elite.flight_speed > (elite.myship.max_speed * 2 / 3)) ? gfx.GFX_COL_DARK_RED : gfx.GFX_COL_GOLD;

			for (i = 0; i < 6; i++)
			{
				alg_gfx.gfx_draw_colour_line(sx, sy + i, sx + len, sy + i, colour);
			}
		}

		/*
		 * Draw an indicator bar.
		 * Used for shields and energy banks.
		 */
		static void display_dial_bar(int len, int x, int y)
		{
			int i = 0;

			alg_gfx.gfx_draw_colour_line(x, y + 384, x + len, y + 384, gfx.GFX_COL_GOLD);
			i++;
			alg_gfx.gfx_draw_colour_line(x, y + i + 384, x + len, y + i + 384, gfx.GFX_COL_GOLD);

			for (i = 2; i < 7; i++)
				alg_gfx.gfx_draw_colour_line(x, y + i + 384, x + len, y + i + 384, gfx.GFX_COL_YELLOW_1);

			alg_gfx.gfx_draw_colour_line(x, y + i + 384, x + len, y + i + 384, gfx.GFX_COL_DARK_RED);
		}

		/*
		 * Display the current shield strengths.
		 */
		static void display_shields()
		{
			if (elite.front_shield > 3)
			{
				display_dial_bar(elite.front_shield / 4, 31, 7);
			}

			if (elite.aft_shield > 3)
			{
				display_dial_bar(elite.aft_shield / 4, 31, 23);
			}
		}

		static void display_altitude()
		{
			if (elite.myship.altitude > 3)
			{
				display_dial_bar(elite.myship.altitude / 4, 31, 92);
			}
		}

		static void display_cabin_temp()
		{
			if (elite.myship.cabtemp > 3)
			{
				display_dial_bar(elite.myship.cabtemp / 4, 31, 60);
			}
		}

		static void display_laser_temp()
		{
			if (elite.laser_temp > 0)
			{
				display_dial_bar(elite.laser_temp / 4, 31, 76);
			}
		}

		/*
		 * Display the energy banks.
		 */
		static void display_energy()
		{
			int e1, e2, e3, e4;

			e1 = elite.energy > 64 ? 64 : elite.energy;
			e2 = elite.energy > 128 ? 64 : elite.energy - 64;
			e3 = elite.energy > 192 ? 64 : elite.energy - 128;
			e4 = elite.energy - 192;

			if (e4 > 0)
			{
				display_dial_bar(e4, 416, 61);
			}

			if (e3 > 0)
			{
				display_dial_bar(e3, 416, 79);
			}

			if (e2 > 0)
			{
				display_dial_bar(e2, 416, 97);
			}

			if (e1 > 0)
			{
				display_dial_bar(e1, 416, 115);
			}
		}

		static void display_flight_roll()
		{
			int sx, sy;
			int i;
			int pos;

			sx = 416;
			sy = 384 + 9 + 14;

			pos = sx - ((elite.flight_roll * 28) / elite.myship.max_roll);
			pos += 32;

			for (i = 0; i < 4; i++)
			{
				alg_gfx.gfx_draw_colour_line(pos + i, sy, pos + i, sy + 7, gfx.GFX_COL_GOLD);
			}
		}

		static void display_flight_climb()
		{
			int sx, sy;
			int i;
			int pos;

			sx = 416;
			sy = 384 + 9 + 14 + 16;

			pos = sx + ((elite.flight_climb * 28) / elite.myship.max_climb);
			pos += 32;

			for (i = 0; i < 4; i++)
			{
				alg_gfx.gfx_draw_colour_line(pos + i, sy, pos + i, sy + 7, gfx.GFX_COL_GOLD);
			}
		}

		static void display_fuel()
		{
			if (elite.cmdr.fuel > 0)
			{
				display_dial_bar((elite.cmdr.fuel * 64) / elite.myship.max_fuel, 31, 44);
			}
		}

		static void display_missiles()
		{
			if (elite.cmdr.missiles == 0)
			{
				return;
			}

			int nomiss = elite.cmdr.missiles > 4 ? 4 : elite.cmdr.missiles;

			int x = (4 - nomiss) * 16 + 35;
			int y = 113 + 385;

			if (swat.missile_target != swat.MISSILE_UNARMED)
			{
				alg_gfx.gfx_draw_sprite((swat.missile_target < 0) ? gfx.IMG_MISSILE_YELLOW : gfx.IMG_MISSILE_RED, x, y);
				x += 16;
				nomiss--;
			}

			for (; nomiss > 0; nomiss--)
			{
				alg_gfx.gfx_draw_sprite(gfx.IMG_MISSILE_GREEN, x, y);
				x += 16;
			}
		}

		internal static void update_console()
		{
			alg_gfx.gfx_set_clip_region(0, 0, 512, 512);
			alg_gfx.gfx_draw_scanner();

			display_speed();
			display_flight_climb();
			display_flight_roll();
			display_shields();
			display_altitude();
			display_energy();
			display_cabin_temp();
			display_laser_temp();
			display_fuel();
			display_missiles();

			if (elite.docked)
			{
				return;
			}

			update_scanner();
			update_compass();

			if (ship_count[(int)SHIP.SHIP_CORIOLIS] != 0 || ship_count[(int)SHIP.SHIP_DODEC] != 0)
			{
				alg_gfx.gfx_draw_sprite(gfx.IMG_BIG_S, 387, 490);
			}

			if (swat.ecm_active != 0)
			{
				alg_gfx.gfx_draw_sprite(gfx.IMG_BIG_E, 115, 490);
			}
		}

		static void increase_flight_roll()
		{
			if (elite.flight_roll < elite.myship.max_roll)
			{
				elite.flight_roll++;
			}
		}

		static void decrease_flight_roll()
		{
			if (elite.flight_roll > -elite.myship.max_roll)
			{
				elite.flight_roll--;
			}
		}

		static void increase_flight_climb()
		{
			if (elite.flight_climb < elite.myship.max_climb)
			{
				elite.flight_climb++;
			}
		}

		static void decrease_flight_climb()
		{
			if (elite.flight_climb > -elite.myship.max_climb)
			{
				elite.flight_climb--;
			}
		}

		static void start_hyperspace()
		{
			if (hyper_ready)
			{
				return;
			}

			hyper_distance = Docked.calc_distance_to_planet(elite.docked_planet, elite.hyperspace_planet);

			if ((hyper_distance == 0) || (hyper_distance > elite.cmdr.fuel))
			{
				return;
			}

			destination_planet = elite.hyperspace_planet;
			hyper_name = Planet.name_planet(ref destination_planet);
			hyper_name = Planet.capitalise_name(hyper_name);

			hyper_ready = true;
			hyper_countdown = 15;
			hyper_galactic = false;

			pilot.disengage_auto_pilot();
		}

		static void start_galactic_hyperspace()
		{
			if (hyper_ready)
			{
				return;
			}

			if (!elite.cmdr.galactic_hyperdrive)
			{
				return;
			}

			hyper_ready = true;
			hyper_countdown = 2;
			hyper_galactic = true;
			pilot.disengage_auto_pilot();
		}

		static void display_hyper_status()
		{
			string str = $"{hyper_countdown:d}";

			if ((elite.current_screen == SCR.SCR_FRONT_VIEW) || (elite.current_screen == SCR.SCR_REAR_VIEW) ||
				(elite.current_screen == SCR.SCR_LEFT_VIEW) || (elite.current_screen == SCR.SCR_RIGHT_VIEW))
			{
				alg_gfx.gfx_display_text(5, 5, str);
				if (hyper_galactic)
				{
					alg_gfx.gfx_display_centre_text(358, "Galactic Hyperspace", 120, gfx.GFX_COL_WHITE);
				}
				else
				{
					str = "Hyperspace - " + hyper_name;
					alg_gfx.gfx_display_centre_text(358, str, 120, gfx.GFX_COL_WHITE);
				}
			}
			else
			{
				alg_gfx.gfx_clear_area(5, 5, 25, 34);
				alg_gfx.gfx_display_text(5, 5, str);
			}
		}

		static int rotate_byte_left(int x)
		{
			return ((x << 1) | (x >> 7)) & 255;
		}

		static void enter_next_galaxy()
		{
			elite.cmdr.galaxy_number++;
			elite.cmdr.galaxy_number &= 7;

			elite.cmdr.galaxy.a = rotate_byte_left(elite.cmdr.galaxy.a);
			elite.cmdr.galaxy.b = rotate_byte_left(elite.cmdr.galaxy.b);
			elite.cmdr.galaxy.c = rotate_byte_left(elite.cmdr.galaxy.c);
			elite.cmdr.galaxy.d = rotate_byte_left(elite.cmdr.galaxy.d);
			elite.cmdr.galaxy.e = rotate_byte_left(elite.cmdr.galaxy.e);
			elite.cmdr.galaxy.f = rotate_byte_left(elite.cmdr.galaxy.f);

			elite.docked_planet = Planet.find_planet(0x60, 0x60);
			elite.hyperspace_planet = elite.docked_planet;
		}

		static void enter_witchspace()
		{
			int i;
			int nthg;

			elite.witchspace = true;
			elite.docked_planet.b ^= 31;
			swat.in_battle = true;

			elite.flight_speed = 12;
			elite.flight_roll = 0;
			elite.flight_climb = 0;
			Stars.create_new_stars();
			swat.clear_universe();

			nthg = (random.randint() & 3) + 1;

			for (i = 0; i < nthg; i++)
			{
				swat.create_thargoid();
			}

			elite.current_screen = SCR.SCR_BREAK_PATTERN;
			sound.snd_play_sample(SND.SND_HYPERSPACE);
		}

		static void complete_hyperspace()
		{
			Vector[] rotmat = new Vector[3];
			int px, py, pz;

			hyper_ready = false;
			elite.witchspace = false;

			if (hyper_galactic)
			{
				elite.cmdr.galactic_hyperdrive = false;
				enter_next_galaxy();
				elite.cmdr.legal_status = 0;
			}
			else
			{
				elite.cmdr.fuel -= hyper_distance;
				elite.cmdr.legal_status /= 2;

				if ((random.rand255() > 253) || (elite.flight_climb == elite.myship.max_climb))
				{
					enter_witchspace();
					return;
				}

				elite.docked_planet = destination_planet;
			}

			elite.cmdr.market_rnd = random.rand255();
			Planet.generate_planet_data(ref elite.current_planet_data, elite.docked_planet);
			trade.generate_stock_market();

			elite.flight_speed = 12;
			elite.flight_roll = 0;
			elite.flight_climb = 0;
			Stars.create_new_stars();
			swat.clear_universe();

			threed.generate_landscape(elite.docked_planet.a * 251 + elite.docked_planet.b);
			VectorMaths.set_init_matrix(ref rotmat);

			pz = (((elite.docked_planet.b) & 7) + 7) / 2;
			px = pz / 2;
			py = px;

			px <<= 16;
			py <<= 16;
			pz <<= 16;

			if ((elite.docked_planet.b & 1) == 0)
			{
				px = -px;
				py = -py;
			}

			swat.add_new_ship(SHIP.SHIP_PLANET, px, py, pz, rotmat, 0, 0);


			pz = -(((elite.docked_planet.d & 7) | 1) << 16);
			px = ((elite.docked_planet.f & 3) << 16) | ((elite.docked_planet.f & 3) << 8);

			swat.add_new_ship(SHIP.SHIP_SUN, px, py, pz, rotmat, 0, 0);

			elite.current_screen = SCR.SCR_BREAK_PATTERN;
			sound.snd_play_sample(SND.SND_HYPERSPACE);
		}

		static void countdown_hyperspace()
		{
			if (hyper_countdown == 0)
			{
				complete_hyperspace();
				return;
			}

			hyper_countdown--;
		}

		static void jump_warp()
		{
			int i;
			SHIP type;
			int jump;

			for (i = 0; i < elite.MAX_UNIV_OBJECTS; i++)
			{
				type = universe[i].type;

				if ((type > 0) && (type != SHIP.SHIP_ASTEROID) && (type != SHIP.SHIP_CARGO) &&
					(type != SHIP.SHIP_ALLOY) && (type != SHIP.SHIP_ROCK) &&
					(type != SHIP.SHIP_BOULDER) && (type != SHIP.SHIP_ESCAPE_CAPSULE))
				{
					alg_main.info_message("Mass Locked");
					return;
				}
			}

			if ((universe[0].distance < 75001) || (universe[1].distance < 75001))
			{
				alg_main.info_message("Mass Locked");
				return;
			}

			if (universe[0].distance < universe[1].distance)
			{
				jump = universe[0].distance - 75000;
			}
			else
			{
				jump = universe[1].distance - 75000;
			}

			if (jump > 1024)
			{
				jump = 1024;
			}

			for (i = 0; i < elite.MAX_UNIV_OBJECTS; i++)
			{
				if (universe[i].type != 0)
				{
					universe[i].location.z -= jump;
				}
			}

			Stars.warp_stars = true;
			alg_main.mcount &= 63;
			swat.in_battle = false;
		}

		static void launch_player()
		{
			Vector[] rotmat = new Vector[3];

			elite.docked = false;
			elite.flight_speed = 12;
			elite.flight_roll = -15;
			elite.flight_climb = 0;
			elite.cmdr.legal_status |= trade.carrying_contraband();
			Stars.create_new_stars();
			swat.clear_universe();
			threed.generate_landscape(elite.docked_planet.a * 251 + elite.docked_planet.b);
			VectorMaths.set_init_matrix(ref rotmat);
			swat.add_new_ship(SHIP.SHIP_PLANET, 0, 0, 65536, rotmat, 0, 0);

			rotmat[2].x = -rotmat[2].x;
			rotmat[2].y = -rotmat[2].y;
			rotmat[2].z = -rotmat[2].z;
			swat.add_new_station(0, 0, -256, rotmat);

			elite.current_screen = SCR.SCR_BREAK_PATTERN;
			sound.snd_play_sample(SND.SND_LAUNCH);
		}

		/*
		 * Engage the docking computer.
		 * For the moment we just do an instant dock if we are in the safe zone.
		 */
		static void engage_docking_computer()
		{
			if (ship_count[(int)SHIP.SHIP_CORIOLIS] != 0 || ship_count[(int)SHIP.SHIP_DODEC] != 0)
			{
				sound.snd_play_sample(SND.SND_DOCK);
				dock_player();
				elite.current_screen = SCR.SCR_BREAK_PATTERN;
			}
		}
	}
}