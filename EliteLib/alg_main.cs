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

namespace Elite
{
	using System.Diagnostics;
	using System.Drawing;
	/*
* alg_main.c
*
* Allegro version of the main game handler.
*/


	//# include <stdio.h>
	//# include <string.h>
	//# include <math.h> 
	//# include <ctype.h>
	//# include <time.h>
	//# include <stdlib.h>

	//# include "allegro.h"

	//# include "config.h"
	//# include "gfx.h"
	//# include "main.h"
	//# include "vector.h"
	//# include "alg_data.h"
	//# include "elite.h"
	//# include "docked.h"
	//# include "intro.h"
	//# include "shipdata.h"
	//# include "shipface.h"
	//# include "space.h"
	//# include "sound.h"
	//# include "threed.h"
	//# include "swat.h"
	//# include "random.h"
	//# include "options.h"
	//# include "stars.h"
	//# include "missions.h"
	//# include "pilot.h"
	//# include "file.h"
	//# include "keyboard.h"

	using Elite.Enums;
	using Elite.Structs;

	public static class alg_main
	{
		static int old_cross_x, old_cross_y;
		static int cross_timer;
		static int draw_lasers;
		internal static int mcount;
		static int message_count;
		static string message_string;
		static bool rolling;
		static bool climbing;
		static bool game_paused;
		static bool have_joystick;
		static bool find_input;
		static string find_name;

		/*
		 * Initialise the game parameters.
		 */
		static void initialise_game()
		{
			random.set_rand_seed((int)DateTime.UtcNow.Ticks);
			elite.current_screen = SCR.SCR_INTRO_ONE;

			elite.restore_saved_commander();

			elite.flight_speed = 1;
			elite.flight_roll = 0;
			elite.flight_climb = 0;
			elite.docked = true;
			elite.front_shield = 255;
			elite.aft_shield = 255;
			elite.energy = 255;
			draw_lasers = 0;
			mcount = 0;
			space.hyper_ready = false;
			elite.detonate_bomb = false;
			find_input = false;
			elite.witchspace = false;
			game_paused = false;
			elite.auto_pilot = false;

			Stars.create_new_stars();
			swat.clear_universe();

			Docked.cross_x = -1;
			Docked.cross_y = -1;
			cross_timer = 0;


			elite.myship.max_speed = 40;      /* 0.27 Light Mach */
			elite.myship.max_roll = 31;
			elite.myship.max_climb = 8;       /* CF 8 */
			elite.myship.max_fuel = 70;       /* 7.0 Light Years */
		}

		static void finish_game()
		{
			elite.finish = true;
			elite.game_over = true;
		}

		/*
		 * Move the planet chart cross hairs to specified position.
		 */
		static void move_cross(int dx, int dy)
		{
			cross_timer = 5;

			if (elite.current_screen == SCR.SCR_SHORT_RANGE)
			{
				Docked.cross_x += (dx * 4);
				Docked.cross_y += (dy * 4);
				return;
			}

			if (elite.current_screen == SCR.SCR_GALACTIC_CHART)
			{
				Docked.cross_x += (dx * 2);
				Docked.cross_y += (dy * 2);

				if (Docked.cross_x < 1)
				{
					Docked.cross_x = 1;
				}

				if (Docked.cross_x > 510)
				{
					Docked.cross_x = 510;
				}

				if (Docked.cross_y < 37)
				{
					Docked.cross_y = 37;
				}

				if (Docked.cross_y > 293)
				{
					Docked.cross_y = 293;
				}
			}
		}

		/*
		 * Draw the cross hairs at the specified position.
		 */
		static void draw_cross(int cx, int cy)
		{
			if (elite.current_screen == SCR.SCR_SHORT_RANGE)
			{
                elite.alg_gfx.gfx_set_clip_region(1, 37, 510, 339);
                elite.alg_gfx.gfx_draw_colour_line_xor(cx - 16, cy, cx + 16, cy, GFX_COL.GFX_COL_RED);
                elite.alg_gfx.gfx_draw_colour_line_xor(cx, cy - 16, cx, cy + 16, GFX_COL.GFX_COL_RED);
                elite.alg_gfx.gfx_set_clip_region(1, 1, 510, 383);
				return;
			}

			if (elite.current_screen == SCR.SCR_GALACTIC_CHART)
			{
                elite.alg_gfx.gfx_set_clip_region(1, 37, 510, 293);
                elite.alg_gfx.gfx_draw_colour_line_xor(cx - 8, cy, cx + 8, cy, GFX_COL.GFX_COL_RED);
                elite.alg_gfx.gfx_draw_colour_line_xor(cx, cy - 8, cx, cy + 8, GFX_COL.GFX_COL_RED);
                elite.alg_gfx.gfx_set_clip_region(1, 1, 510, 383);
			}
		}

		static void draw_laser_sights()
		{
			int laser = 0;
			int x1, y1, x2, y2;

			switch (elite.current_screen)
			{
				case SCR.SCR_FRONT_VIEW:
					elite.alg_gfx.gfx_display_centre_text(32, "Front View", 120, GFX_COL.GFX_COL_WHITE);
					laser = elite.cmdr.front_laser;
					break;

				case SCR.SCR_REAR_VIEW:
					elite.alg_gfx.gfx_display_centre_text(32, "Rear View", 120, GFX_COL.GFX_COL_WHITE);
					laser = elite.cmdr.rear_laser;
					break;

				case SCR.SCR_LEFT_VIEW:
					elite.alg_gfx.gfx_display_centre_text(32, "Left View", 120, GFX_COL.GFX_COL_WHITE);
					laser = elite.cmdr.left_laser;
					break;

				case SCR.SCR_RIGHT_VIEW:
					elite.alg_gfx.gfx_display_centre_text(32, "Right View", 120, GFX_COL.GFX_COL_WHITE);
					laser = elite.cmdr.right_laser;
					break;
			}

			if (laser != 0)
			{
				x1 = 128 * gfx.GFX_SCALE;
				y1 = (96 - 8) * gfx.GFX_SCALE;
				y2 = (96 - 16) * gfx.GFX_SCALE;

				elite.alg_gfx.gfx_draw_colour_line(x1 - 1, y1, x1 - 1, y2, GFX_COL.GFX_COL_GREY_1);
				elite.alg_gfx.gfx_draw_colour_line(x1, y1, x1, y2, GFX_COL.GFX_COL_WHITE);
				elite.alg_gfx.gfx_draw_colour_line(x1 + 1, y1, x1 + 1, y2, GFX_COL.GFX_COL_GREY_1);

				y1 = (96 + 8) * gfx.GFX_SCALE;
				y2 = (96 + 16) * gfx.GFX_SCALE;

                elite.alg_gfx.gfx_draw_colour_line(x1 - 1, y1, x1 - 1, y2, GFX_COL.GFX_COL_GREY_1);
                elite.alg_gfx.gfx_draw_colour_line(x1, y1, x1, y2, GFX_COL.GFX_COL_WHITE);
                elite.alg_gfx.gfx_draw_colour_line(x1 + 1, y1, x1 + 1, y2, GFX_COL.GFX_COL_GREY_1);

				x1 = (128 - 8) * gfx.GFX_SCALE;
				y1 = 96 * gfx.GFX_SCALE;
				x2 = (128 - 16) * gfx.GFX_SCALE;

                elite.alg_gfx.gfx_draw_colour_line(x1, y1 - 1, x2, y1 - 1, GFX_COL.GFX_COL_GREY_1);
                elite.alg_gfx.gfx_draw_colour_line(x1, y1, x2, y1, GFX_COL.GFX_COL_WHITE);
                elite.alg_gfx.gfx_draw_colour_line(x1, y1 + 1, x2, y1 + 1, GFX_COL.GFX_COL_GREY_1);

				x1 = (128 + 8) * gfx.GFX_SCALE;
				x2 = (128 + 16) * gfx.GFX_SCALE;

                elite.alg_gfx.gfx_draw_colour_line(x1, y1 - 1, x2, y1 - 1, GFX_COL.GFX_COL_GREY_1);
                elite.alg_gfx.gfx_draw_colour_line(x1, y1, x2, y1, GFX_COL.GFX_COL_WHITE);
                elite.alg_gfx.gfx_draw_colour_line(x1, y1 + 1, x2, y1 + 1, GFX_COL.GFX_COL_GREY_1);
			}
		}

		static void arrow_right()
		{
			switch (elite.current_screen)
			{
				case SCR.SCR_MARKET_PRICES:
					Docked.buy_stock();
					break;

				case SCR.SCR_SETTINGS:
					options.select_right_setting();
					break;

				case SCR.SCR_SHORT_RANGE:
				case SCR.SCR_GALACTIC_CHART:
					move_cross(1, 0);
					break;

				case SCR.SCR_FRONT_VIEW:
				case SCR.SCR_REAR_VIEW:
				case SCR.SCR_RIGHT_VIEW:
				case SCR.SCR_LEFT_VIEW:
					if (elite.flight_roll > 0)
					{
						elite.flight_roll = 0;
					}
					else
					{
						space.decrease_flight_roll();
						space.decrease_flight_roll();
						rolling = true;
					}
					break;
			}
		}

		static void arrow_left()
		{
			switch (elite.current_screen)
			{
				case SCR.SCR_MARKET_PRICES:
					Docked.sell_stock();
					break;

				case SCR.SCR_SETTINGS:
					options.select_left_setting();
					break;

				case SCR.SCR_SHORT_RANGE:
				case SCR.SCR_GALACTIC_CHART:
					move_cross(-1, 0);
					break;

				case SCR.SCR_FRONT_VIEW:
				case SCR.SCR_REAR_VIEW:
				case SCR.SCR_RIGHT_VIEW:
				case SCR.SCR_LEFT_VIEW:
					if (elite.flight_roll < 0)
					{
						elite.flight_roll = 0;
					}
					else
					{
						space.increase_flight_roll();
						space.increase_flight_roll();
						rolling = true;
					}
					break;
			}
		}

		static void arrow_up()
		{
			switch (elite.current_screen)
			{
				case SCR.SCR_MARKET_PRICES:
					Docked.select_previous_stock();
					break;

				case SCR.SCR_EQUIP_SHIP:
					Docked.select_previous_equip();
					break;

				case SCR.SCR_OPTIONS:
					options.select_previous_option();
					break;

				case SCR.SCR_SETTINGS:
					options.select_up_setting();
					break;

				case SCR.SCR_SHORT_RANGE:
				case SCR.SCR_GALACTIC_CHART:
					move_cross(0, -1);
					break;

				case SCR.SCR_FRONT_VIEW:
				case SCR.SCR_REAR_VIEW:
				case SCR.SCR_RIGHT_VIEW:
				case SCR.SCR_LEFT_VIEW:
					if (elite.flight_climb > 0)
					{
						elite.flight_climb = 0;
					}
					else
					{
						space.decrease_flight_climb();
					}
					climbing = true;
					break;
			}
		}

		static void arrow_down()
		{
			switch (elite.current_screen)
			{
				case SCR.SCR_MARKET_PRICES:
					Docked.select_next_stock();
					break;

				case SCR.SCR_EQUIP_SHIP:
					Docked.select_next_equip();
					break;

				case SCR.SCR_OPTIONS:
					options.select_next_option();
					break;

				case SCR.SCR_SETTINGS:
					options.select_down_setting();
					break;

				case SCR.SCR_SHORT_RANGE:
				case SCR.SCR_GALACTIC_CHART:
					move_cross(0, 1);
					break;

				case SCR.SCR_FRONT_VIEW:
				case SCR.SCR_REAR_VIEW:
				case SCR.SCR_RIGHT_VIEW:
				case SCR.SCR_LEFT_VIEW:
					if (elite.flight_climb < 0)
					{
						elite.flight_climb = 0;
					}
					else
					{
						space.increase_flight_climb();
					}
					climbing = true;
					break;
			}
		}

		static void return_pressed()
		{
			switch (elite.current_screen)
			{
				case SCR.SCR_EQUIP_SHIP:
					Docked.buy_equip();
					break;

				case SCR.SCR_OPTIONS:
					options.do_option();
					break;

				case SCR.SCR_SETTINGS:
					options.toggle_setting();
					break;
			}
		}

		static void y_pressed()
		{
			switch (elite.current_screen)
			{
				case SCR.SCR_QUIT:
					finish_game();
					break;
			}
		}

		static void n_pressed()
		{
			switch (elite.current_screen)
			{
				case SCR.SCR_QUIT:
					if (elite.docked)
					{
						Docked.display_commander_status();
					}
					else
					{
						elite.current_screen = SCR.SCR_FRONT_VIEW;
					}

					break;
			}
		}


		static void d_pressed()
		{
			switch (elite.current_screen)
			{
				case SCR.SCR_GALACTIC_CHART:
				case SCR.SCR_SHORT_RANGE:
					Docked.show_distance_to_planet();
					break;

				case SCR.SCR_FRONT_VIEW:
				case SCR.SCR_REAR_VIEW:
				case SCR.SCR_RIGHT_VIEW:
				case SCR.SCR_LEFT_VIEW:
					if (elite.auto_pilot)
					{
						pilot.disengage_auto_pilot();
					}

					break;
			}
		}

		static void f_pressed()
		{
			if ((elite.current_screen == SCR.SCR_GALACTIC_CHART) ||
				(elite.current_screen == SCR.SCR_SHORT_RANGE))
			{
				find_input = true;
				find_name = string.Empty;
                elite.alg_gfx.gfx_clear_text_area();
                elite.alg_gfx.gfx_display_text(16, 340, "Planet Name?");
			}
		}

		static void add_find_char(char letter)
		{
			if (find_name.Length == 16)
			{
				return;
			}

			string str = char.ToUpper(letter).ToString();
			find_name += str;

			str = "Planet Name? " + find_name;
            elite.alg_gfx.gfx_clear_text_area();
            elite.alg_gfx.gfx_display_text(16, 340, str);
		}

		static void delete_find_char()
		{
			string str;

			int len = find_name.Length;
			if (len == 0)
			{
				return;
			}

			find_name = find_name[..^1];

			str = "Planet Name? " + find_name;
            elite.alg_gfx.gfx_clear_text_area();
            elite.alg_gfx.gfx_display_text(16, 340, str);
		}

		static void o_pressed()
		{
			switch (elite.current_screen)
			{
				case SCR.SCR_GALACTIC_CHART:
				case SCR.SCR_SHORT_RANGE:
					Docked.move_cursor_to_origin();
					break;
			}
		}


		static void auto_dock()
		{
			univ_object ship = new univ_object();
			ship.rotmat = new Vector[3];
			ship.location.x = 0;
			ship.location.y = 0;
			ship.location.z = 0;

			VectorMaths.set_init_matrix(ref ship.rotmat);
			ship.rotmat[2].z = 1;
			ship.rotmat[0].x = -1;
			ship.type = (SHIP)(-96);
			ship.velocity = elite.flight_speed;
			ship.acceleration = 0;
			ship.bravery = 0;
			ship.rotz = 0;
			ship.rotx = 0;

			pilot.auto_pilot_ship(ref ship);

			if (ship.velocity > 22)
			{
				elite.flight_speed = 22;
			}
			else
			{
				elite.flight_speed = ship.velocity;
			}

			if (ship.acceleration > 0)
			{
				elite.flight_speed++;
				if (elite.flight_speed > 22)
				{
					elite.flight_speed = 22;
				}
			}

			if (ship.acceleration < 0)
			{
				elite.flight_speed--;
				if (elite.flight_speed < 1)
				{
					elite.flight_speed = 1;
				}
			}

			if (ship.rotx == 0)
			{
				elite.flight_climb = 0;
			}

			if (ship.rotx < 0)
			{
				space.increase_flight_climb();

				if (ship.rotx < -1)
				{
					space.increase_flight_climb();
				}
			}

			if (ship.rotx > 0)
			{
				space.decrease_flight_climb();

				if (ship.rotx > 1)
				{
					space.decrease_flight_climb();
				}
			}

			if (ship.rotz == 127)
			{
				elite.flight_roll = -14;
			}
			else
			{
				if (ship.rotz == 0)
				{
					elite.flight_roll = 0;
				}

				if (ship.rotz > 0)
				{
					space.increase_flight_roll();

					if (ship.rotz > 1)
					{
						space.increase_flight_roll();
					}
				}

				if (ship.rotz < 0)
				{
					space.decrease_flight_roll();

					if (ship.rotz < -1)
					{
						space.decrease_flight_roll();
					}
				}
			}
		}


		static void run_escape_sequence()
		{
			int i;
			Vector[] rotmat = new Vector[3];

			elite.current_screen = SCR.SCR_ESCAPE_POD;

			elite.flight_speed = 1;
			elite.flight_roll = 0;
			elite.flight_climb = 0;

			VectorMaths.set_init_matrix(ref rotmat);
			rotmat[2].z = 1.0;

			int newship = swat.add_new_ship(SHIP.SHIP_COBRA3, 0, 0, 200, rotmat, -127, -127);
			space.universe[newship].velocity = 7;
			sound.snd_play_sample(SND.SND_LAUNCH);

			for (i = 0; i < 90; i++)
			{
				if (i == 40)
				{
					space.universe[newship].flags |= FLG.FLG_DEAD;
					sound.snd_play_sample(SND.SND_EXPLODE);
				}

                elite.alg_gfx.gfx_set_clip_region(1, 1, 510, 383);
                elite.alg_gfx.gfx_clear_display();
				Stars.update_starfield();
				space.update_universe();

				space.universe[newship].location.x = 0;
				space.universe[newship].location.y = 0;
				space.universe[newship].location.z += 2;

                elite.alg_gfx.gfx_display_centre_text(358, "Escape pod launched - Ship auto-destuct initiated.", 120, GFX_COL.GFX_COL_WHITE);

				space.update_console();
                elite.alg_gfx.gfx_update_screen();
			}


			while ((space.ship_count[(int)SHIP.SHIP_CORIOLIS] == 0) && (space.ship_count[(int)SHIP.SHIP_DODEC] == 0))
			{
				auto_dock();

				if ((Math.Abs(elite.flight_roll) < 3) && (Math.Abs(elite.flight_climb) < 3))
				{
					for (i = 0; i < elite.MAX_UNIV_OBJECTS; i++)
					{
						if (space.universe[i].type != 0)
						{
							space.universe[i].location.z -= 1500;
						}
					}
				}

				Stars.warp_stars = true;
                elite.alg_gfx.gfx_set_clip_region(1, 1, 510, 383);
                elite.alg_gfx.gfx_clear_display();
				Stars.update_starfield();
				space.update_universe();
				space.update_console();
                elite.alg_gfx.gfx_update_screen();
			}

			swat.abandon_ship();
		}

		static void handle_flight_keys()
		{
			char keyasc;

			if (elite.docked &&
				((elite.current_screen == SCR.SCR_MARKET_PRICES) ||
				 (elite.current_screen == SCR.SCR_OPTIONS) ||
				 (elite.current_screen == SCR.SCR_SETTINGS) ||
				 (elite.current_screen == SCR.SCR_EQUIP_SHIP)))
			{
				keyboard.kbd_read_key();
			}

			keyboard.kbd_poll_keyboard();

			//if (have_joystick)
			//{
			//	poll_joystick();

			//	if (joy[0].stick[0].axis[1].d1)
			//		arrow_up();

			//	if (joy[0].stick[0].axis[1].d2)
			//		arrow_down();

			//	if (joy[0].stick[0].axis[0].d1)
			//		arrow_left();

			//	if (joy[0].stick[0].axis[0].d2)
			//		arrow_right();

			//	if (joy[0].button[0].b)
			//		keyboard.kbd_fire_pressed = true;

			//	if (joy[0].button[1].b)
			//		keyboard.kbd_inc_speed_pressed = true;

			//	if (joy[0].button[2].b)
			//		keyboard.kbd_dec_speed_pressed = true;
			//}

			if (game_paused)
			{
				if (keyboard.kbd_resume_pressed)
				{
					game_paused = false;
				}

				return;
			}

			if (keyboard.kbd_F1_pressed)
			{
				find_input = false;

				if (elite.docked)
				{
					space.launch_player();
				}
				else
				{
					if (elite.current_screen != SCR.SCR_FRONT_VIEW)
					{
						elite.current_screen = SCR.SCR_FRONT_VIEW;
						Stars.flip_stars();
					}
				}
			}

			if (keyboard.kbd_F2_pressed)
			{
				find_input = false;

				if (!elite.docked)
				{
					if (elite.current_screen != SCR.SCR_REAR_VIEW)
					{
						elite.current_screen = SCR.SCR_REAR_VIEW;
						Stars.flip_stars();
					}
				}
			}

			if (keyboard.kbd_F3_pressed)
			{
				find_input = false;

				if (!elite.docked)
				{
					if (elite.current_screen != SCR.SCR_LEFT_VIEW)
					{
						elite.current_screen = SCR.SCR_LEFT_VIEW;
						Stars.flip_stars();
					}
				}
			}

			if (keyboard.kbd_F4_pressed)
			{
				find_input = false;

				if (elite.docked)
				{
					Docked.equip_ship();
				}
				else
				{
					if (elite.current_screen != SCR.SCR_RIGHT_VIEW)
					{
						elite.current_screen = SCR.SCR_RIGHT_VIEW;
						Stars.flip_stars();
					}
				}
			}

			if (keyboard.kbd_F5_pressed)
			{
				find_input = false;
				old_cross_x = -1;
				Docked.display_galactic_chart();
			}

			if (keyboard.kbd_F6_pressed)
			{
				find_input = false;
				old_cross_x = -1;
				Docked.display_short_range_chart();
			}

			if (keyboard.kbd_F7_pressed)
			{
				find_input = false;
				Docked.display_data_on_planet();
			}

			if (keyboard.kbd_F8_pressed && (!elite.witchspace))
			{
				find_input = false;
				Docked.display_market_prices();
			}

			if (keyboard.kbd_F9_pressed)
			{
				find_input = false;
				Docked.display_commander_status();
			}

			if (keyboard.kbd_F10_pressed)
			{
				find_input = false;
				Docked.display_inventory();
			}

			if (keyboard.kbd_F11_pressed)
			{
				find_input = false;
				options.display_options();
			}

			if (find_input)
			{
				keyasc = keyboard.kbd_read_key();

				if (keyboard.kbd_enter_pressed)
				{
					find_input = false;
					Docked.find_planet_by_name(find_name);
					return;
				}

				if (keyboard.kbd_backspace_pressed)
				{
					delete_find_char();
					return;
				}

				if (char.IsLetter(keyasc))
				{
					add_find_char(keyasc);
				}

				return;
			}

			if (keyboard.kbd_y_pressed)
			{
				y_pressed();
			}

			if (keyboard.kbd_n_pressed)
			{
				n_pressed();
			}

			if (keyboard.kbd_fire_pressed)
			{
				if ((!elite.docked) && (draw_lasers == 0))
				{
					draw_lasers = swat.fire_laser();
				}
			}

			if (keyboard.kbd_dock_pressed)
			{
				if (!elite.docked && elite.cmdr.docking_computer)
				{
					if (elite.instant_dock)
					{
						space.engage_docking_computer();
					}
					else
					{
						pilot.engage_auto_pilot();
					}
				}
			}

			if (keyboard.kbd_d_pressed)
			{
				d_pressed();
			}

			if (keyboard.kbd_ecm_pressed)
			{
				if (!elite.docked && elite.cmdr.ecm)
				{
					swat.activate_ecm(true);
				}
			}

			if (keyboard.kbd_find_pressed)
			{
				f_pressed();
			}

			if (keyboard.kbd_hyperspace_pressed && (!elite.docked))
			{
				if (keyboard.kbd_ctrl_pressed)
				{
					space.start_galactic_hyperspace();
				}
				else
				{
					space.start_hyperspace();
				}
			}

			if (keyboard.kbd_jump_pressed && (!elite.docked) && (!elite.witchspace))
			{
				space.jump_warp();
			}

			if (keyboard.kbd_fire_missile_pressed)
			{
				if (!elite.docked)
				{
					swat.fire_missile();
				}
			}

			if (keyboard.kbd_origin_pressed)
			{
				o_pressed();
			}

			if (keyboard.kbd_pause_pressed)
			{
				game_paused = true;
			}

			if (keyboard.kbd_target_missile_pressed)
			{
				if (!elite.docked)
				{
					swat.arm_missile();
				}
			}

			if (keyboard.kbd_unarm_missile_pressed)
			{
				if (!elite.docked)
				{
					swat.unarm_missile();
				}
			}

			if (keyboard.kbd_inc_speed_pressed)
			{
				if (!elite.docked)
				{
					if (elite.flight_speed < elite.myship.max_speed)
					{
						elite.flight_speed++;
					}
				}
			}

			if (keyboard.kbd_dec_speed_pressed)
			{
				if (!elite.docked)
				{
					if (elite.flight_speed > 1)
					{
						elite.flight_speed--;
					}
				}
			}

			if (keyboard.kbd_up_pressed)
			{
				arrow_up();
			}

			if (keyboard.kbd_down_pressed)
			{
				arrow_down();
			}

			if (keyboard.kbd_left_pressed)
			{
				arrow_left();
			}

			if (keyboard.kbd_right_pressed)
			{
				arrow_right();
			}

			if (keyboard.kbd_enter_pressed)
			{
				return_pressed();
			}

			if (keyboard.kbd_energy_bomb_pressed)
			{
				if ((!elite.docked) && (elite.cmdr.energy_bomb))
				{
					elite.detonate_bomb = true;
					elite.cmdr.energy_bomb = false;
				}
			}

			if (keyboard.kbd_escape_pressed)
			{
				if ((!elite.docked) && (elite.cmdr.escape_pod) && (!elite.witchspace))
				{
					run_escape_sequence();
				}
			}
		}

		static void set_commander_name(string path)
		{
			string fname = Path.GetFileNameWithoutExtension(path);
			string cname = elite.cmdr.name;

			foreach (char c in fname)
			{
				if (char.IsLetter(c))
				{
					cname += char.ToUpper(c);
				}
			}

			elite.cmdr.name = cname;
		}

		internal static void save_commander_screen()
		{
			elite.current_screen = SCR.SCR_SAVE_CMDR;

            elite.alg_gfx.gfx_clear_display();
            elite.alg_gfx.gfx_display_centre_text(10, "SAVE COMMANDER", 140, GFX_COL.GFX_COL_GOLD);
            elite.alg_gfx.gfx_draw_line(0, 36, 511, 36);
            elite.alg_gfx.gfx_update_screen();

			string path = elite.cmdr.name;
			path = ".nkc";

			bool okay = elite.alg_gfx.gfx_request_file("Save Commander", path, "nkc");

			if (!okay)
			{
				options.display_options();
				return;
			}

			bool rv = File.save_commander_file(path);

			if (rv)
			{
                elite.alg_gfx.gfx_display_centre_text(175, "Error Saving Commander!", 140, GFX_COL.GFX_COL_GOLD);
				return;
			}

            elite.alg_gfx.gfx_display_centre_text(175, "Commander Saved.", 140, GFX_COL.GFX_COL_GOLD);

			set_commander_name(path);
			elite.saved_cmdr = elite.cmdr;
			elite.saved_cmdr.ship_x = elite.docked_planet.d;
			elite.saved_cmdr.ship_y = elite.docked_planet.b;
		}

		internal static void load_commander_screen()
		{
            elite.alg_gfx.gfx_clear_display();
            elite.alg_gfx.gfx_display_centre_text(10, "LOAD COMMANDER", 140, GFX_COL.GFX_COL_GOLD);
            elite.alg_gfx.gfx_draw_line(0, 36, 511, 36);
            elite.alg_gfx.gfx_update_screen();

			string path = "jameson.nkc";

			bool rv = elite.alg_gfx.gfx_request_file("Load Commander", path, "nkc");

			if (!rv)
			{
				return;
			}

			rv = File.load_commander_file(path);

			if (rv)
			{
				elite.saved_cmdr = elite.cmdr;
                elite.alg_gfx.gfx_display_centre_text(175, "Error Loading Commander!", 140, GFX_COL.GFX_COL_GOLD);
                elite.alg_gfx.gfx_display_centre_text(200, "Press any key to continue.", 140, GFX_COL.GFX_COL_GOLD);
                elite.alg_gfx.gfx_update_screen();
				//TODO: Fix this
				//readkey();
				Debug.WriteLine("call to readkey()");
				return;
			}

			elite.restore_saved_commander();
			set_commander_name(path);
			elite.saved_cmdr = elite.cmdr;
			space.update_console();
		}

		static void run_first_intro_screen()
		{
			elite.current_screen = SCR.SCR_INTRO_ONE;

			sound.snd_play_midi(SND.SND_ELITE_THEME, true);

			intro.initialise_intro1();

			for (; ; )
			{
				intro.update_intro1();

                elite.alg_gfx.gfx_update_screen();

				keyboard.kbd_poll_keyboard();

				if (keyboard.kbd_y_pressed)
				{
					sound.snd_stop_midi();
					load_commander_screen();
					break;
				}

				if (keyboard.kbd_n_pressed)
				{
					sound.snd_stop_midi();
					break;
				}
			}

		}

		static void run_second_intro_screen()
		{
			elite.current_screen = SCR.SCR_INTRO_TWO;

			sound.snd_play_midi(SND.SND_BLUE_DANUBE, true);

			intro.initialise_intro2();

			elite.flight_speed = 3;
			elite.flight_roll = 0;
			elite.flight_climb = 0;

			for (; ; )
			{
				intro.update_intro2();

                elite.alg_gfx.gfx_update_screen();

				keyboard.kbd_poll_keyboard();

				if (keyboard.kbd_space_pressed)
				{
					break;
				}
			}

			sound.snd_stop_midi();
		}

		/*
		 * Draw the game over sequence. 
		 */
		static void run_game_over_screen()
		{
			int i;
			int newship;
			Vector[] rotmat = new Vector[3];
			SHIP type;

			elite.current_screen = SCR.SCR_GAME_OVER;
            elite.alg_gfx.gfx_set_clip_region(1, 1, 510, 383);

			elite.flight_speed = 6;
			elite.flight_roll = 0;
			elite.flight_climb = 0;
			swat.clear_universe();

			VectorMaths.set_init_matrix(ref rotmat);

			newship = swat.add_new_ship(SHIP.SHIP_COBRA3, 0, 0, -400, rotmat, 0, 0);
			space.universe[newship].flags |= FLG.FLG_DEAD;

			for (i = 0; i < 5; i++)
			{
				type = ((random.rand255() & 1) == 1) ? SHIP.SHIP_CARGO : SHIP.SHIP_ALLOY;
				newship = swat.add_new_ship(type, (random.rand255() & 63) - 32, (random.rand255() & 63) - 32, -400, rotmat, 0, 0);
				space.universe[newship].rotz = ((random.rand255() * 2) & 255) - 128;
				space.universe[newship].rotx = ((random.rand255() * 2) & 255) - 128;
				space.universe[newship].velocity = random.rand255() & 15;
			}


			for (i = 0; i < 100; i++)
			{
                elite.alg_gfx.gfx_clear_display();
				Stars.update_starfield();
				space.update_universe();
                elite.alg_gfx.gfx_display_centre_text(190, "GAME OVER", 140, GFX_COL.GFX_COL_GOLD);
                elite.alg_gfx.gfx_update_screen();
			}
		}

		/*
		 * Draw a break pattern (for launching, docking and hyperspacing).
		 * Just draw a very simple one for the moment.
		 */
		static void display_break_pattern()
		{
			int i;

            elite.alg_gfx.gfx_set_clip_region(1, 1, 510, 383);
            elite.alg_gfx.gfx_clear_display();

			for (i = 0; i < 20; i++)
			{
                elite.alg_gfx.gfx_draw_circle(256, 192, 30 + i * 15, GFX_COL.GFX_COL_WHITE);
                elite.alg_gfx.gfx_update_screen();
			}

			if (elite.docked)
			{
				missions.check_mission_brief();
				Docked.display_commander_status();
				space.update_console();
			}
			else
				elite.current_screen = SCR.SCR_FRONT_VIEW;
		}

		internal static void info_message(string message)
		{
			message_string = message;
			message_count = 37;
			//	sound.snd_play_sample (SND_BEEP);
		}

		static void initialise_allegro()
		{
			// allegro_init();
			// install_keyboard();
			// install_timer();
			// install_mouse();

			have_joystick = false;

			//if (install_joystick(JOY_TYPE_AUTODETECT) == 0)
			//{
			//	have_joystick = (num_joysticks > 0);
			//}
		}

		public static int main(ref IGfx alg_gfx)
		{
            elite.alg_gfx = alg_gfx;

            initialise_allegro();
			File.read_config_file();

			if (elite.alg_gfx.gfx_graphics_startup() == 1)
			{
				return 1;
			}

			/* Start the sound system... */
			sound.snd_sound_startup();

			/* Do any setup necessary for the keyboard... */
			keyboard.kbd_keyboard_startup();

			elite.finish = false;
			elite.auto_pilot = false;

			while (!elite.finish)
			{
				elite.game_over = false;
				initialise_game();
				space.dock_player();

				space.update_console();

				elite.current_screen = SCR.SCR_FRONT_VIEW;
				run_first_intro_screen();
				run_second_intro_screen();

				old_cross_x = -1;
				old_cross_y = -1;

				space.dock_player();
				Docked.display_commander_status();

				while (!elite.game_over)
				{
					sound.snd_update_sound();
                    elite.alg_gfx.gfx_update_screen();
                    elite.alg_gfx.gfx_set_clip_region(1, 1, 510, 383);

					rolling = false;
					climbing = false;

					handle_flight_keys();

					if (game_paused)
					{
						continue;
					}

					if (message_count > 0)
					{
						message_count--;
					}

					if (!rolling)
					{
						if (elite.flight_roll > 0)
						{
							space.decrease_flight_roll();
						}

						if (elite.flight_roll < 0)
						{
							space.increase_flight_roll();
						}
					}

					if (!climbing)
					{
						if (elite.flight_climb > 0)
						{
							space.decrease_flight_climb();
						}

						if (elite.flight_climb < 0)
						{
							space.increase_flight_climb();
						}
					}

					if (!elite.docked)
					{
                        elite.alg_gfx.gfx_acquire_screen();

						if ((elite.current_screen == SCR.SCR_FRONT_VIEW) || (elite.current_screen == SCR.SCR_REAR_VIEW) ||
							(elite.current_screen == SCR.SCR_LEFT_VIEW) || (elite.current_screen == SCR.SCR_RIGHT_VIEW) ||
							(elite.current_screen == SCR.SCR_INTRO_ONE) || (elite.current_screen == SCR.SCR_INTRO_TWO) ||
							(elite.current_screen == SCR.SCR_GAME_OVER))
						{
                            elite.alg_gfx.gfx_clear_display();
							Stars.update_starfield();
						}

						if (elite.auto_pilot)
						{
							auto_dock();
							if ((mcount & 127) == 0)
								info_message("Docking Computers On");
						}

						space.update_universe();

						if (elite.docked)
						{
							space.update_console();
                            elite.alg_gfx.gfx_release_screen();
							continue;
						}

						if ((elite.current_screen == SCR.SCR_FRONT_VIEW) || (elite.current_screen == SCR.SCR_REAR_VIEW) ||
							(elite.current_screen == SCR.SCR_LEFT_VIEW) || (elite.current_screen == SCR.SCR_RIGHT_VIEW))
						{
							if (draw_lasers != 0)
							{
								swat.draw_laser_lines();
								draw_lasers--;
							}

							draw_laser_sights();
						}

						if (message_count > 0)
						{
                            elite.alg_gfx.gfx_display_centre_text(358, message_string, 120, GFX_COL.GFX_COL_WHITE);
						}

						if (space.hyper_ready)
						{
							space.display_hyper_status();
							if ((mcount & 3) == 0)
							{
								space.countdown_hyperspace();
							}
						}

                        elite.alg_gfx.gfx_release_screen();

						mcount--;
						if (mcount < 0)
						{
							mcount = 255;
						}

						if ((mcount & 7) == 0)
						{
							space.regenerate_shields();
						}

						if ((mcount & 31) == 10)
						{
							if (elite.energy < 50)
							{
								info_message("ENERGY LOW");
								sound.snd_play_sample(SND.SND_BEEP);
							}

							space.update_altitude();
						}

						if ((mcount & 31) == 20)
						{
							space.update_cabin_temp();
						}

						if ((mcount == 0) && (!elite.witchspace))
						{
							swat.random_encounter();
						}

						swat.cool_laser();
						swat.time_ecm();

						space.update_console();
					}

					if (elite.current_screen == SCR.SCR_BREAK_PATTERN)
					{
						display_break_pattern();
					}

					if (cross_timer > 0)
					{
						cross_timer--;
						if (cross_timer == 0)
						{
							Docked.show_distance_to_planet();
						}
					}

					if ((Docked.cross_x != old_cross_x) ||
						(Docked.cross_y != old_cross_y))
					{
						if (old_cross_x != -1)
						{
							draw_cross(old_cross_x, old_cross_y);
						}

						old_cross_x = Docked.cross_x;
						old_cross_y = Docked.cross_y;

						draw_cross(old_cross_x, old_cross_y);
					}
				}

				if (!elite.finish)
				{
					run_game_over_screen();
				}
			}

			sound.snd_sound_shutdown();

            elite.alg_gfx.gfx_graphics_shutdown();

			return 0;
		}

		//END_OF_MAIN();
	}
}