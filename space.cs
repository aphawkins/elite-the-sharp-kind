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

# include <stdio.h>
# include <string.h>
# include <math.h>
# include <stdlib.h>

# include "vector.h"

# include "alg_data.h"

# include "config.h"
# include "elite.h"
# include "gfx.h"
# include "docked.h"
# include "intro.h"
# include "shipdata.h"
# include "shipface.h"
# include "space.h" 
# include "threed.h"
# include "sound.h"
# include "main.h"
# include "swat.h"
# include "random.h"
# include "trade.h"
# include "stars.h"
# include "pilot.h"

using Elite;
using Elite.Structs;
using EliteLib;

namespace Elite
{
	using Elite.Enums;
	using Elite.Structs;

	internal static class space
	{
		extern int flight_climb;
		extern int flight_roll;
		extern int flight_speed;

		galaxy_seed destination_planet;
		internal static int hyper_ready;
		int hyper_countdown;
		char hyper_name[16];
		int hyper_distance;
		int hyper_galactic;


		internal const int MAX_UNIV_OBJECTS = 20;

		internal static univ_object[] universe = new univ_object[MAX_UNIV_OBJECTS];
		internal static int[] ship_count = new int[shipdata.NO_OF_SHIPS + 1];  /* many */

		static void rotate_x_first(double* a, double* b, int direction)
		{
			double fx, ux;

			fx = *a;
			ux = *b;

			if (direction < 0)
			{
				*a = fx - (fx / 512) + (ux / 19);
				*b = ux - (ux / 512) - (fx / 19);
			}
			else
			{
				*a = fx - (fx / 512) - (ux / 19);
				*b = ux - (ux / 512) + (fx / 19);
			}
		}


		static void rotate_vec(Vector[] vec, double alpha, double beta)
		{
			double x, y, z;

			x = vec.x;
			y = vec.y;
			z = vec.z;

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

			alpha = flight_roll / 256.0;
			beta = flight_climb / 256.0;

			x = obj.location.x;
			y = obj.location.y;
			z = obj.location.z;

			if (!(obj.flags & FLG_DEAD))
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
					if (obj.velocity > elite.ship_list[obj.type].velocity)
					{
						obj.velocity = elite.ship_list[obj.type].velocity;
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

			z = z - flight_speed;

			obj.location.x = x;
			obj.location.y = y;
			obj.location.z = z;

			obj.distance = sqrt(x * x + y * y + z * z);

			if (obj.type == SHIP_PLANET)
				beta = 0.0;

			rotate_vec(&obj.rotmat[2], alpha, beta);
			rotate_vec(&obj.rotmat[1], alpha, beta);
			rotate_vec(&obj.rotmat[0], alpha, beta);

			if (obj.flags.HasFlag(FLG.FLG_DEAD))
			{
				return;
			}

			rotx = obj.rotx;
			rotz = obj.rotz;

			/* If necessary rotate the object around the X axis... */

			if (rotx != 0)
			{
				rotate_x_first(&obj.rotmat[2].x, &obj.rotmat[1].x, rotx);
				rotate_x_first(&obj.rotmat[2].y, &obj.rotmat[1].y, rotx);
				rotate_x_first(&obj.rotmat[2].z, &obj.rotmat[1].z, rotx);

				if ((rotx != 127) && (rotx != -127))
					obj.rotx -= (rotx < 0) ? -1 : 1;
			}


			/* If necessary rotate the object around the Z axis... */

			if (rotz != 0)
			{
				rotate_x_first(&obj.rotmat[0].x, &obj.rotmat[1].x, rotz);
				rotate_x_first(&obj.rotmat[0].y, &obj.rotmat[1].y, rotz);
				rotate_x_first(&obj.rotmat[0].z, &obj.rotmat[1].z, rotz);

				if ((rotz != 127) && (rotz != -127))
					obj.rotz -= (rotz < 0) ? -1 : 1;
			}


			/* Orthonormalize the rotation matrix... */

			VectorMaths.tidy_matrix(obj.rotmat);
		}


		/*
		 * Dock the player into the space station.
		 */

		void dock_player()
		{
			disengage_auto_pilot();
			docked = 1;
			flight_speed = 0;
			flight_roll = 0;
			flight_climb = 0;
			front_shield = 255;
			aft_shield = 255;
			energy = 255;
			myship.altitude = 255;
			myship.cabtemp = 30;
			reset_weapons();
		}


		/*
		 * Check if we are correctly aligned to dock.
		 */

		int is_docking(int sn)
		{
			Vector vec;
			double fz;
			double ux;

			if (auto_pilot)     // Don't want it to kill anyone!
				return 1;

			fz = universe[sn].rotmat[2].z;

			if (fz > -0.90)
				return 0;

			vec = VectorMaths.unit_vector(universe[sn].location);

			if (vec.z < 0.927)
				return 0;

			ux = universe[sn].rotmat[1].x;
			if (ux < 0)
				ux = -ux;

			if (ux < 0.84)
				return 0;

			return 1;
		}


		/*
		 * Game Over...
		 */

		void do_game_over()
		{
			snd_play_sample(SND_GAMEOVER);
			game_over = 1;
		}


		void update_altitude()
		{
			double x, y, z;
			double dist;

			myship.altitude = 255;

			if (elite.witchspace)
			{
				return;
			}

			x = Math.Abs(universe[0].location.x);
			y = Math.Abs(universe[0].location.y);
			z = Math.Abs(universe[0].location.z);

			if ((x > 65535) || (y > 65535) || (z > 65535))
				return;

			x /= 256;
			y /= 256;
			z /= 256;

			dist = (x * x) + (y * y) + (z * z);

			if (dist > 65535)
				return;

			dist -= 9472;
			if (dist < 1)
			{
				myship.altitude = 0;
				do_game_over();
				return;
			}

			dist = sqrt(dist);
			if (dist < 1)
			{
				myship.altitude = 0;
				do_game_over();
				return;
			}

			myship.altitude = dist;
		}


		static void update_cabin_temp()
		{
			int x, y, z;
			int dist;

			myship.cabtemp = 30;

			if (elite.witchspace)
			{
				return;
			}

			if (ship_count[SHIP_CORIOLIS] || ship_count[SHIP_DODEC])
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

			myship.cabtemp = dist + 30;

			if (myship.cabtemp > 255)
			{
				myship.cabtemp = 255;
				do_game_over();
				return;
			}

			if ((myship.cabtemp < 224) || (elite.cmdr.fuel_scoop == 0))
				return;

			elite.cmdr.fuel += flight_speed / 2;
			if (elite.cmdr.fuel > myship.max_fuel)
				elite.cmdr.fuel = myship.max_fuel;

			alg_main.info_message("Fuel Scoop On");
		}



		/*
		 * Regenerate the shields and the energy banks.
		 */

		void regenerate_shields()
		{
			if (energy > 127)
			{
				if (front_shield < 255)
				{
					front_shield++;
					energy--;
				}

				if (aft_shield < 255)
				{
					aft_shield++;
					energy--;
				}
			}

			energy++;
			energy += elite.cmdr.energy_unit;
			if (energy > 255)
				energy = 255;
		}


		void decrease_energy(int amount)
		{
			energy += amount;

			if (energy <= 0)
				do_game_over();
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

			shield = front ? front_shield : aft_shield;

			shield -= damage;
			if (shield < 0)
			{
				decrease_energy(shield);
				shield = 0;
			}

			if (front)
				front_shield = shield;
			else
				aft_shield = shield;
		}




		void make_station_appear()
		{
			double px, py, pz;
			double sx, sy, sz;
			Vector vec;
			Matrix rotmat;

			px = universe[0].location.x;
			py = universe[0].location.y;
			pz = universe[0].location.z;

			vec.x = (rand() & 32767) - 16384;
			vec.y = (rand() & 32767) - 16384;
			vec.z = rand() & 32767;

			vec = VectorMaths.unit_vector(&vec);

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

			add_new_station(sx, sy, sz, rotmat);
		}



		void check_docking(int i)
		{
			if (is_docking(i))
			{
				snd_play_sample(SND_DOCK);
				dock_player();
				current_screen = SCR_BREAK_PATTERN;
				return;
			}

			if (flight_speed >= 5)
			{
				do_game_over();
				return;
			}

			flight_speed = 1;
			damage_ship(5, universe[i].location.z > 0);
			snd_play_sample(SND_CRASH);
		}


		void switch_to_view(univ_object* flip)
		{
			double tmp;

			if ((current_screen == SCR_REAR_VIEW) ||
				(current_screen == SCR_GAME_OVER))
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


			if (current_screen == SCR_LEFT_VIEW)
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

			if (current_screen == SCR_RIGHT_VIEW)
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

		void update_universe()
		{
			int i;
			int type;
			int bounty;
			char str[80];
			univ_object flip;


			gfx_start_render();

			for (i = 0; i < MAX_UNIV_OBJECTS; i++)
			{
				type = universe[i].type;

				if (type != 0)
				{
					if (universe[i].flags.HasFlag(FLG.FLG_REMOVE))
					{
						if (type == SHIP_VIPER)
						{
							elite.cmdr.legal_status |= 64;
						}

						bounty = elite.ship_list[type].bounty;

						if ((bounty != 0) && (!elite.witchspace))
						{
							elite.cmdr.credits += bounty;
							sprintf(str, "%d.%d CR", elite.cmdr.credits / 10, elite.cmdr.credits % 10);
                            alg_main.info_message(str);
						}

						swat.remove_ship(i);
						continue;
					}

					if ((detonate_bomb) && ((!universe[i].flags.HasFlag(FLG.FLG_DEAD)) &&
						(type != SHIP_PLANET) && (type != SHIP_SUN) &&
						(type != SHIP_CONSTRICTOR) && (type != SHIP_COUGAR) &&
						(type != SHIP_CORIOLIS) && (type != SHIP_DODEC))
					{
						snd_play_sample(SND_EXPLODE);
						universe[i].flags |= FLG.FLG_DEAD;
					}

					if ((current_screen != SCR_INTRO_ONE) &&
						(current_screen != SCR_INTRO_TWO) &&
						(current_screen != SCR_GAME_OVER) &&
						(current_screen != SCR_ESCAPE_POD))
					{
						tactics(i);
					}

					move_univ_object(universe[i]);

					flip = universe[i];
					switch_to_view(&flip);

					if (type == SHIP_PLANET)
					{
						if ((ship_count[SHIP_CORIOLIS] == 0) &&
							(ship_count[SHIP_DODEC] == 0) &&
							(universe[i].distance < 65792)) // was 49152
						{
							make_station_appear();
						}

						draw_ship(&flip);
						continue;
					}

					if (type == SHIP_SUN)
					{
						draw_ship(&flip);
						continue;
					}


					if (universe[i].distance < 170)
					{
						if ((type == SHIP_CORIOLIS) || (type == SHIP_DODEC))
							check_docking(i);
						else
							trade.scoop_item(i);

						continue;
					}

					if (universe[i].distance > 57344)
					{
                        swat.remove_ship(i);
						continue;
					}

					draw_ship(&flip);

					universe[i].flags = flip.flags;
					universe[i].exp_seed = flip.exp_seed;
					universe[i].exp_delta = flip.exp_delta;

					universe[i].flags &= ~FLG.FLG_FIRING;

					if (universe[i].flags & FLG.FLG_DEAD)
						continue;

					check_target(i, &flip);
				}
			}

			gfx_finish_render();
			detonate_bomb = 0;
		}




		/*
		 * Update the scanner and draw all the lollipops.
		 */

		void update_scanner()
		{
			int i;
			int x, y, z;
			int x1, y1, y2;
			int colour;

			for (i = 0; i < MAX_UNIV_OBJECTS; i++)
			{
				if ((universe[i].type <= 0) ||
					universe[i].flags.HasFlag(FLG.FLG_DEAD) ||
					universe[i].flags.HasFlag(FLG.FLG_CLOAKED))
				{
					continue;
				}

				x = universe[i].location.x / 256;
				y = universe[i].location.y / 256;
				z = universe[i].location.z / 256;

				x1 = x;
				y1 = -z / 4;
				y2 = y1 - y / 2;

				if ((y2 < -28) || (y2 > 28) ||
					(x1 < -50) || (x1 > 50))
					continue;

				x1 += scanner_cx;
				y1 += scanner_cy;
				y2 += scanner_cy;

				colour = universe[i].flags.HasFlag(FLG.FLG_HOSTILE) ? GFX_COL_YELLOW_5 : GFX_COL_WHITE;

				switch (universe[i].type)
				{
					case SHIP_MISSILE:
						colour = 137;
						break;

					case SHIP_DODEC:
					case SHIP_CORIOLIS:
						colour = GFX_COL_GREEN_1;
						break;

					case SHIP_VIPER:
						colour = 252;
						break;
				}

				gfx_draw_colour_line(x1 + 2, y2, x1 - 3, y2, colour);
				gfx_draw_colour_line(x1 + 2, y2 + 1, x1 - 3, y2 + 1, colour);
				gfx_draw_colour_line(x1 + 2, y2 + 2, x1 - 3, y2 + 2, colour);
				gfx_draw_colour_line(x1 + 2, y2 + 3, x1 - 3, y2 + 3, colour);


				gfx_draw_colour_line(x1, y1, x1, y2, colour);
				gfx_draw_colour_line(x1 + 1, y1, x1 + 1, y2, colour);
				gfx_draw_colour_line(x1 + 2, y1, x1 + 2, y2, colour);
			}
		}


		/*
		 * Update the compass which tracks the space station / planet.
		 */

		void update_compass()
		{
			Vector dest;
			int compass_x;
			int compass_y;
			int un = 0;

			if (elite.witchspace)
			{
				return;
			}

			if (ship_count[SHIP_CORIOLIS] || ship_count[SHIP_DODEC])
				un = 1;

			dest = VectorMaths.unit_vector(universe[un].location);

			compass_x = compass_centre_x + (dest.x * 16);
			compass_y = compass_centre_y + (dest.y * -16);

			if (dest.z < 0)
			{
				gfx_draw_sprite(IMG_RED_DOT, compass_x, compass_y);
			}
			else
			{
				gfx_draw_sprite(IMG_GREEN_DOT, compass_x, compass_y);
			}

		}


		/*
		 * Display the speed bar.
		 */

		void display_speed()
		{
			int sx, sy;
			int i;
			int len;
			int colour;

			sx = 417;
			sy = 384 + 9;

			len = ((flight_speed * 64) / myship.max_speed) - 1;

			colour = (flight_speed > (myship.max_speed * 2 / 3)) ? GFX_COL_DARK_RED : GFX_COL_GOLD;

			for (i = 0; i < 6; i++)
			{
				gfx_draw_colour_line(sx, sy + i, sx + len, sy + i, colour);
			}
		}


		/*
		 * Draw an indicator bar.
		 * Used for shields and energy banks.
		 */

		void display_dial_bar(int len, int x, int y)
		{
			int i = 0;

			gfx_draw_colour_line(x, y + 384, x + len, y + 384, GFX_COL_GOLD);
			i++;
			gfx_draw_colour_line(x, y + i + 384, x + len, y + i + 384, GFX_COL_GOLD);

			for (i = 2; i < 7; i++)
				gfx_draw_colour_line(x, y + i + 384, x + len, y + i + 384, GFX_COL_YELLOW_1);

			gfx_draw_colour_line(x, y + i + 384, x + len, y + i + 384, GFX_COL_DARK_RED);
		}


		/*
		 * Display the current shield strengths.
		 */

		void display_shields()
		{
			if (front_shield > 3)
				display_dial_bar(front_shield / 4, 31, 7);

			if (aft_shield > 3)
				display_dial_bar(aft_shield / 4, 31, 23);
		}


		void display_altitude()
		{
			if (myship.altitude > 3)
				display_dial_bar(myship.altitude / 4, 31, 92);
		}

		void display_cabin_temp()
		{
			if (myship.cabtemp > 3)
				display_dial_bar(myship.cabtemp / 4, 31, 60);
		}


		void display_laser_temp()
		{
			if (laser_temp > 0)
				display_dial_bar(laser_temp / 4, 31, 76);
		}


		/*
		 * Display the energy banks.
		 */

		void display_energy()
		{
			int e1, e2, e3, e4;

			e1 = energy > 64 ? 64 : energy;
			e2 = energy > 128 ? 64 : energy - 64;
			e3 = energy > 192 ? 64 : energy - 128;
			e4 = energy - 192;

			if (e4 > 0)
				display_dial_bar(e4, 416, 61);

			if (e3 > 0)
				display_dial_bar(e3, 416, 79);

			if (e2 > 0)
				display_dial_bar(e2, 416, 97);

			if (e1 > 0)
				display_dial_bar(e1, 416, 115);
		}



		void display_flight_roll()
		{
			int sx, sy;
			int i;
			int pos;

			sx = 416;
			sy = 384 + 9 + 14;

			pos = sx - ((flight_roll * 28) / myship.max_roll);
			pos += 32;

			for (i = 0; i < 4; i++)
			{
				gfx_draw_colour_line(pos + i, sy, pos + i, sy + 7, GFX_COL_GOLD);
			}
		}

		void display_flight_climb()
		{
			int sx, sy;
			int i;
			int pos;

			sx = 416;
			sy = 384 + 9 + 14 + 16;

			pos = sx + ((flight_climb * 28) / myship.max_climb);
			pos += 32;

			for (i = 0; i < 4; i++)
			{
				gfx_draw_colour_line(pos + i, sy, pos + i, sy + 7, GFX_COL_GOLD);
			}
		}


		void display_fuel()
		{
			if (elite.cmdr.fuel > 0)
				display_dial_bar((elite.cmdr.fuel * 64) / myship.max_fuel, 31, 44);
		}


		void display_missiles()
		{
			int nomiss;
			int x, y;

			if (elite.cmdr.missiles == 0)
				return;

			nomiss = elite.cmdr.missiles > 4 ? 4 : elite.cmdr.missiles;

			x = (4 - nomiss) * 16 + 35;
			y = 113 + 385;

			if (missile_target != swat.MISSILE_UNARMED)
			{
				gfx_draw_sprite((missile_target < 0) ? IMG_MISSILE_YELLOW :
														IMG_MISSILE_RED, x, y);
				x += 16;
				nomiss--;
			}

			for (; nomiss > 0; nomiss--)
			{
				gfx_draw_sprite(IMG_MISSILE_GREEN, x, y);
				x += 16;
			}
		}


		void update_console()
		{
			gfx_set_clip_region(0, 0, 512, 512);
			gfx_draw_scanner();

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

			if (docked)
				return;

			update_scanner();
			update_compass();

			if (ship_count[SHIP_CORIOLIS] || ship_count[SHIP_DODEC])
				gfx_draw_sprite(IMG_BIG_S, 387, 490);

			if (ecm_active)
				gfx_draw_sprite(IMG_BIG_E, 115, 490);
		}

		void increase_flight_roll()
		{
			if (flight_roll < myship.max_roll)
				flight_roll++;
		}


		void decrease_flight_roll()
		{
			if (flight_roll > -myship.max_roll)
				flight_roll--;
		}


		void increase_flight_climb()
		{
			if (flight_climb < myship.max_climb)
				flight_climb++;
		}

		void decrease_flight_climb()
		{
			if (flight_climb > -myship.max_climb)
				flight_climb--;
		}


		void start_hyperspace()
		{
			if (hyper_ready)
				return;

			hyper_distance = calc_distance_to_planet(docked_planet, hyperspace_planet);

			if ((hyper_distance == 0) || (hyper_distance > elite.cmdr.fuel))
				return;

			destination_planet = hyperspace_planet;
			name_planet(hyper_name, destination_planet);
			capitalise_name(hyper_name);

			hyper_ready = 1;
			hyper_countdown = 15;
			hyper_galactic = 0;

			disengage_auto_pilot();
		}

		void start_galactic_hyperspace()
		{
			if (hyper_ready)
				return;

			if (elite.cmdr.galactic_hyperdrive == 0)
				return;

			hyper_ready = 1;
			hyper_countdown = 2;
			hyper_galactic = 1;
			disengage_auto_pilot();
		}



		void display_hyper_status()
		{
			char str[80];

			sprintf(str, "%d", hyper_countdown);

			if ((current_screen == SCR_FRONT_VIEW) || (current_screen == SCR_REAR_VIEW) ||
				(current_screen == SCR_LEFT_VIEW) || (current_screen == SCR_RIGHT_VIEW))
			{
				gfx_display_text(5, 5, str);
				if (hyper_galactic)
				{
					gfx_display_centre_text(358, "Galactic Hyperspace", 120, GFX_COL_WHITE);
				}
				else
				{
					sprintf(str, "Hyperspace - %s", hyper_name);
					gfx_display_centre_text(358, str, 120, GFX_COL_WHITE);
				}
			}
			else
			{
				gfx_clear_area(5, 5, 25, 34);
				gfx_display_text(5, 5, str);
			}
		}


		int rotate_byte_left(int x)
		{
			return ((x << 1) | (x >> 7)) & 255;
		}

		void enter_next_galaxy()
		{
			elite.cmdr.galaxy_number++;
			elite.cmdr.galaxy_number &= 7;

			elite.cmdr.galaxy.a = rotate_byte_left(elite.cmdr.galaxy.a);
			elite.cmdr.galaxy.b = rotate_byte_left(elite.cmdr.galaxy.b);
			elite.cmdr.galaxy.c = rotate_byte_left(elite.cmdr.galaxy.c);
			elite.cmdr.galaxy.d = rotate_byte_left(elite.cmdr.galaxy.d);
			elite.cmdr.galaxy.e = rotate_byte_left(elite.cmdr.galaxy.e);
			elite.cmdr.galaxy.f = rotate_byte_left(elite.cmdr.galaxy.f);

			docked_planet = find_planet(0x60, 0x60);
			hyperspace_planet = docked_planet;
		}

		static void enter_witchspace()
		{
			int i;
			int nthg;

            elite.witchspace = true;
			docked_planet.b ^= 31;
			in_battle = 1;

			flight_speed = 12;
			flight_roll = 0;
			flight_climb = 0;
			create_new_stars();
			clear_universe();

			nthg = (randint() & 3) + 1;

			for (i = 0; i < nthg; i++)
				create_thargoid();

			current_screen = SCR_BREAK_PATTERN;
			snd_play_sample(SND_HYPERSPACE);
		}


		static void complete_hyperspace()
		{
			Vector[] rotmat = new Vector[3];
			int px, py, pz;

			hyper_ready = 0;
            elite.witchspace = false;

			if (hyper_galactic)
			{
				elite.cmdr.galactic_hyperdrive = 0;
				enter_next_galaxy();
				elite.cmdr.legal_status = 0;
			}
			else
			{
				elite.cmdr.fuel -= hyper_distance;
				elite.cmdr.legal_status /= 2;

				if ((random.rand255() > 253) || (flight_climb == myship.max_climb))
				{
					enter_witchspace();
					return;
				}

				docked_planet = destination_planet;
			}

			elite.cmdr.market_rnd = random.rand255();
			planet.generate_planet_data(ref elite.current_planet_data, docked_planet);
			generate_stock_market();

			flight_speed = 12;
			flight_roll = 0;
			flight_climb = 0;
			create_new_stars();
			clear_universe();

			generate_landscape(docked_planet.a * 251 + docked_planet.b);
			VectorMaths.set_init_matrix(rotmat);

			pz = (((docked_planet.b) & 7) + 7) / 2;
			px = pz / 2;
			py = px;

			px <<= 16;
			py <<= 16;
			pz <<= 16;

			if ((docked_planet.b & 1) == 0)
			{
				px = -px;
				py = -py;
			}

			add_new_ship(SHIP_PLANET, px, py, pz, rotmat, 0, 0);


			pz = -(((docked_planet.d & 7) | 1) << 16);
			px = ((docked_planet.f & 3) << 16) | ((docked_planet.f & 3) << 8);

			add_new_ship(SHIP_SUN, px, py, pz, rotmat, 0, 0);

			current_screen = SCR_BREAK_PATTERN;
			snd_play_sample(SND_HYPERSPACE);
		}


		void countdown_hyperspace()
		{
			if (hyper_countdown == 0)
			{
				complete_hyperspace();
				return;
			}

			hyper_countdown--;
		}



		void jump_warp()
		{
			int i;
			int type;
			int jump;

			for (i = 0; i < MAX_UNIV_OBJECTS; i++)
			{
				type = universe[i].type;

				if ((type > 0) && (type != shipdata.SHIP_ASTEROID) && (type != shipdata.SHIP_CARGO) &&
					(type != shipdata.SHIP_ALLOY) && (type != shipdata.SHIP_ROCK) &&
					(type != shipdata.SHIP_BOULDER) && (type != shipdata.SHIP_ESCAPE_CAPSULE))
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
				jump = universe[0].distance - 75000;
			else
				jump = universe[1].distance - 75000;

			if (jump > 1024)
				jump = 1024;

			for (i = 0; i < MAX_UNIV_OBJECTS; i++)
			{
				if (universe[i].type != 0)
					universe[i].location.z -= jump;
			}

			warp_stars = 1;
			alg_main.mcount &= 63;
			in_battle = 0;
		}


		void launch_player()
		{
			Matrix rotmat;

			docked = 0;
			flight_speed = 12;
			flight_roll = -15;
			flight_climb = 0;
			elite.cmdr.legal_status |= trade.carrying_contraband();
			create_new_stars();
			swat.clear_universe();
			generate_landscape(docked_planet.a * 251 + docked_planet.b);
			VectorMaths.set_init_matrix(rotmat);
			add_new_ship(SHIP_PLANET, 0, 0, 65536, rotmat, 0, 0);

			rotmat[2].x = -rotmat[2].x;
			rotmat[2].y = -rotmat[2].y;
			rotmat[2].z = -rotmat[2].z;
			add_new_station(0, 0, -256, rotmat);

			current_screen = SCR_BREAK_PATTERN;
			snd_play_sample(SND_LAUNCH);
		}



		/*
		 * Engage the docking computer.
		 * For the moment we just do an instant dock if we are in the safe zone.
		 */

		void engage_docking_computer()
		{
			if (ship_count[SHIP_CORIOLIS] || ship_count[SHIP_DODEC])
			{
				snd_play_sample(SND_DOCK);
				dock_player();
				current_screen = SCR_BREAK_PATTERN;
			}
		}
	}
}