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
    using Elite.Enums;
    using Elite.Structs;
    using Elite.Ships;
    using System.Numerics;
    using Elite.Config;
    using Elite.Save;
    using Elite.Views;

    public static class elite
	{
		internal static IGfx alg_gfx;
		internal static ISound sound;
        internal static IKeyboard keyboard;

        internal const int PULSE_LASER = 0x0F;     //  15
		internal const int BEAM_LASER = 0x8F;      // 143
		internal const int MILITARY_LASER = 0x97;  // 151
		internal const int MINING_LASER = 0x32;    //  50

		internal const int MAX_UNIV_OBJECTS = 20;

		internal static galaxy_seed docked_planet;
		internal static galaxy_seed hyperspace_planet;
		internal static planet_data current_planet_data = new();

		//static int curr_galaxy_num = 1;
		//static int curr_fuel = 70;
		internal static int carry_flag = 0;
		internal static SCR current_screen = 0;
		internal static bool witchspace;

        public static ConfigSettings config;

        internal static string scanner_filename;
		internal static Vector2 scanner_centre = new(253, 63 + 385);
		internal static Vector2 compass_centre = new(382, 22 + 385);

		internal static bool game_over;
		internal static bool docked;
		internal static bool finish;
		internal static int flight_speed;
		internal static int flight_roll;
		internal static int flight_climb;
		internal static int front_shield;
		internal static int aft_shield;
		internal static int energy;
		internal static int laser_temp;
		internal static bool detonate_bomb;
		internal static bool auto_pilot;

#if !DEBUG
        internal static Commander saved_cmdr = CommanderFactory.Max();
#else
		internal static Commander saved_cmdr = CommanderFactory.Jameson();
#endif

        internal static Commander cmdr;
		internal static player_ship myship = new();
		internal static Draw draw;

		internal static ship_data[] ship_list = new ship_data[shipdata.NO_OF_SHIPS + 1]
		{
			new(),
			shipdata.missile_data,
			shipdata.coriolis_data,
			shipdata.esccaps_data,
			shipdata.alloy_data,
			shipdata.cargo_data,
			shipdata.boulder_data,
			shipdata.asteroid_data,
			shipdata.rock_data,
			shipdata.orbit_data,
			shipdata.transp_data,
			shipdata.cobra3a_data,
			shipdata.pythona_data,
			shipdata.boa_data,
			shipdata.anacnda_data,
			shipdata.hermit_data,
			shipdata.viper_data,
			shipdata.sidewnd_data,
			shipdata.mamba_data,
			shipdata.krait_data,
			shipdata.adder_data,
			shipdata.gecko_data,
			shipdata.cobra1_data,
			shipdata.worm_data,
			shipdata.cobra3b_data,
			shipdata.asp2_data,
			shipdata.pythonb_data,
			shipdata.ferdlce_data,
			shipdata.moray_data,
			shipdata.thargoid_data,
			shipdata.thargon_data,
			shipdata.constrct_data,
			shipdata.cougar_data,
			shipdata.dodec_data
		};

		internal static void restore_saved_commander()
		{
			cmdr = saved_cmdr;

			docked_planet = Planet.find_planet(cmdr.shiplocation);
			hyperspace_planet = (galaxy_seed)docked_planet.Clone();

			Planet.generate_planet_data(ref current_planet_data, docked_planet);
			trade.generate_stock_market();
			trade.set_stock_quantities(cmdr.station_stock);
		}

        private static Vector2 old_cross;
        private static int cross_timer;
        private static int draw_lasers;
        internal static int mcount;
        private static int message_count;
        private static string message_string;
        private static bool rolling;
        private static bool climbing;
        private static bool game_paused;
        private static bool have_joystick;
        private static bool find_input;
        private static string find_name;

        /*
		 * Initialise the game parameters.
		 */
        private static void initialise_game()
        {
            random.rand_seed = (int)DateTime.UtcNow.Ticks;
            current_screen = SCR.SCR_INTRO_ONE;

            restore_saved_commander();

            flight_speed = 1;
            flight_roll = 0;
            flight_climb = 0;
            docked = true;
            front_shield = 255;
            aft_shield = 255;
            energy = 255;
            draw_lasers = 0;
            mcount = 0;
            space.hyper_ready = false;
            detonate_bomb = false;
            find_input = false;
            witchspace = false;
            game_paused = false;
            auto_pilot = false;

            Stars.create_new_stars();
            swat.clear_universe();

            GalacticChart.cross = new(-1f, -1f);
            cross_timer = 0;

            myship.max_speed = 40;      /* 0.27 Light Mach */
            myship.max_roll = 31;
            myship.max_climb = 8;       /* CF 8 */
            myship.max_fuel = 70;       /* 7.0 Light Years */
        }

        private static void finish_game()
        {
            finish = true;
            game_over = true;
        }

        /*
		 * Move the planet chart cross hairs to specified position.
		 */
        private static void move_cross(int dx, int dy)
        {
            cross_timer = 5;

            if (current_screen == SCR.SCR_SHORT_RANGE)
            {
                GalacticChart.cross.X += dx * 4;
                GalacticChart.cross.Y += dy * 4;
                return;
            }
            else if (current_screen == SCR.SCR_GALACTIC_CHART)
            {
                GalacticChart.cross.X += dx * 2;
                GalacticChart.cross.Y += dy * 2;

                if (GalacticChart.cross.X < 1)
                {
                    GalacticChart.cross.X = 1;
                }

                if (GalacticChart.cross.X > 510)
                {
                    GalacticChart.cross.X = 510;
                }

                if (GalacticChart.cross.Y < 37)
                {
                    GalacticChart.cross.Y = 37;
                }

                if (GalacticChart.cross.Y > 293)
                {
                    GalacticChart.cross.Y = 293;
                }
            }
        }

        /// <summary>
        /// Draw the cross hairs at the specified position.
        /// </summary>
        /// <param name="cx"></param>
        /// <param name="cy"></param>
        private static void draw_cross(Vector2 centre)
        {
            if (current_screen == SCR.SCR_SHORT_RANGE)
            {
                alg_gfx.SetClipRegion(1, 37, 510, 339);
                alg_gfx.DrawLine(new(centre.X - 16f, centre.Y), new(centre.X + 16f, centre.Y), GFX_COL.GFX_COL_RED);
                alg_gfx.DrawLine(new(centre.X, centre.Y - 16), new(centre.X, centre.Y + 16), GFX_COL.GFX_COL_RED);
                alg_gfx.SetClipRegion(1, 1, 510, 383);
                return;
            }

            if (current_screen == SCR.SCR_GALACTIC_CHART)
            {
                alg_gfx.SetClipRegion(1, 37, 510, 293);
                alg_gfx.DrawLine(new(centre.X - 8, centre.Y), new(centre.X + 8, centre.Y), GFX_COL.GFX_COL_RED);
                alg_gfx.DrawLine(new(centre.X, centre.Y - 8), new(centre.X, centre.Y + 8), GFX_COL.GFX_COL_RED);
                alg_gfx.SetClipRegion(1, 1, 510, 383);
            }
        }

        private static void draw_laser_sights()
        {
            int laser = 0;

            switch (current_screen)
            {
                case SCR.SCR_FRONT_VIEW:
                    alg_gfx.DrawTextCentre(32, "Front View", 120, GFX_COL.GFX_COL_WHITE);
                    laser = cmdr.front_laser;
                    break;

                case SCR.SCR_REAR_VIEW:
                    alg_gfx.DrawTextCentre(32, "Rear View", 120, GFX_COL.GFX_COL_WHITE);
                    laser = cmdr.rear_laser;
                    break;

                case SCR.SCR_LEFT_VIEW:
                    alg_gfx.DrawTextCentre(32, "Left View", 120, GFX_COL.GFX_COL_WHITE);
                    laser = cmdr.left_laser;
                    break;

                case SCR.SCR_RIGHT_VIEW:
                    alg_gfx.DrawTextCentre(32, "Right View", 120, GFX_COL.GFX_COL_WHITE);
                    laser = cmdr.right_laser;
                    break;
            }

            if (laser != 0)
            {
                float x1 = 128f * gfx.GFX_SCALE;
                float y1 = (96f - 8f) * gfx.GFX_SCALE;
                float y2 = (96f - 16f) * gfx.GFX_SCALE;

                alg_gfx.DrawLine(new(x1 - 1, y1), new(x1 - 1, y2), GFX_COL.GFX_COL_GREY_1);
                alg_gfx.DrawLine(new(x1, y1), new(x1, y2), GFX_COL.GFX_COL_WHITE);
                alg_gfx.DrawLine(new(x1 + 1, y1), new(x1 + 1, y2), GFX_COL.GFX_COL_GREY_1);

                y1 = (96f + 8f) * gfx.GFX_SCALE;
                y2 = (96f + 16f) * gfx.GFX_SCALE;

                alg_gfx.DrawLine(new(x1 - 1, y1), new(x1 - 1, y2), GFX_COL.GFX_COL_GREY_1);
                alg_gfx.DrawLine(new(x1, y1), new(x1, y2), GFX_COL.GFX_COL_WHITE);
                alg_gfx.DrawLine(new(x1 + 1, y1), new(x1 + 1, y2), GFX_COL.GFX_COL_GREY_1);

                x1 = (128f - 8f) * gfx.GFX_SCALE;
                y1 = 96f * gfx.GFX_SCALE;
                float x2 = (128f - 16f) * gfx.GFX_SCALE;

                alg_gfx.DrawLine(new(x1, y1 - 1), new(x2, y1 - 1), GFX_COL.GFX_COL_GREY_1);
                alg_gfx.DrawLine(new(x1, y1), new(x2, y1), GFX_COL.GFX_COL_WHITE);
                alg_gfx.DrawLine(new(x1, y1 + 1), new(x2, y1 + 1), GFX_COL.GFX_COL_GREY_1);

                x1 = (128f + 8f) * gfx.GFX_SCALE;
                x2 = (128f + 16f) * gfx.GFX_SCALE;

                alg_gfx.DrawLine(new(x1, y1 - 1), new(x2, y1 - 1), GFX_COL.GFX_COL_GREY_1);
                alg_gfx.DrawLine(new(x1, y1), new(x2, y1), GFX_COL.GFX_COL_WHITE);
                alg_gfx.DrawLine(new(x1, y1 + 1), new(x2, y1 + 1), GFX_COL.GFX_COL_GREY_1);
            }
        }

        private static void arrow_right()
        {
            switch (current_screen)
            {
                case SCR.SCR_MARKET_PRICES:
                    Market.buy_stock();
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
                    if (flight_roll > 0)
                    {
                        flight_roll = 0;
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

        private static void arrow_left()
        {
            switch (current_screen)
            {
                case SCR.SCR_MARKET_PRICES:
                    Market.sell_stock();
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
                    if (flight_roll < 0)
                    {
                        flight_roll = 0;
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

        private static void arrow_up()
        {
            switch (current_screen)
            {
                case SCR.SCR_MARKET_PRICES:
                    Market.select_previous_stock();
                    break;

                case SCR.SCR_EQUIP_SHIP:
                    Equipment.select_previous_equip();
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
                    if (flight_climb > 0)
                    {
                        flight_climb = 0;
                    }
                    else
                    {
                        space.decrease_flight_climb();
                    }
                    climbing = true;
                    break;
            }
        }

        private static void arrow_down()
        {
            switch (current_screen)
            {
                case SCR.SCR_MARKET_PRICES:
                    Market.select_next_stock();
                    break;

                case SCR.SCR_EQUIP_SHIP:
                    Equipment.select_next_equip();
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
                    if (flight_climb < 0)
                    {
                        flight_climb = 0;
                    }
                    else
                    {
                        space.increase_flight_climb();
                    }
                    climbing = true;
                    break;
            }
        }

        private static void return_pressed()
        {
            switch (current_screen)
            {
                case SCR.SCR_EQUIP_SHIP:
                    Equipment.buy_equip();
                    break;

                case SCR.SCR_OPTIONS:
                    options.do_option();
                    break;

                case SCR.SCR_SETTINGS:
                    options.toggle_setting();
                    break;
            }
        }

        private static void y_pressed()
        {
            switch (current_screen)
            {
                case SCR.SCR_QUIT:
                    finish_game();
                    break;
            }
        }

        private static void n_pressed()
        {
            switch (current_screen)
            {
                case SCR.SCR_QUIT:
                    if (docked)
                    {
                        CommanderStatus.display_commander_status();
                    }
                    else
                    {
                        current_screen = SCR.SCR_FRONT_VIEW;
                    }

                    break;
            }
        }

        private static void d_pressed()
        {
            switch (current_screen)
            {
                case SCR.SCR_GALACTIC_CHART:
                case SCR.SCR_SHORT_RANGE:
                    GalacticChart.show_distance_to_planet();
                    break;

                case SCR.SCR_FRONT_VIEW:
                case SCR.SCR_REAR_VIEW:
                case SCR.SCR_RIGHT_VIEW:
                case SCR.SCR_LEFT_VIEW:
                    if (auto_pilot)
                    {
                        pilot.disengage_auto_pilot();
                    }

                    break;
            }
        }

        private static void f_pressed()
        {
            if (current_screen is SCR.SCR_GALACTIC_CHART or SCR.SCR_SHORT_RANGE)
            {
                find_input = true;
                find_name = string.Empty;
                alg_gfx.ClearTextArea();
                alg_gfx.DrawTextLeft(16, 340, "Planet Name?", GFX_COL.GFX_COL_WHITE);
            }
        }

        private static void add_find_char(char letter)
        {
            if (find_name.Length == 16)
            {
                return;
            }

            string str = char.ToUpper(letter).ToString();
            find_name += str;

            str = "Planet Name? " + find_name;
            alg_gfx.ClearTextArea();
            alg_gfx.DrawTextLeft(16, 340, str, GFX_COL.GFX_COL_WHITE);
        }

        private static void delete_find_char()
        {
            string str;

            int len = find_name.Length;
            if (len == 0)
            {
                return;
            }

            find_name = find_name[..^1];

            str = "Planet Name? " + find_name;
            alg_gfx.ClearTextArea();
            alg_gfx.DrawTextLeft(16, 340, str, GFX_COL.GFX_COL_WHITE);
        }

        private static void o_pressed()
        {
            switch (current_screen)
            {
                case SCR.SCR_GALACTIC_CHART:
                case SCR.SCR_SHORT_RANGE:
                    GalacticChart.move_cursor_to_origin();
                    break;
            }
        }

        private static void auto_dock()
        {
            univ_object ship = new()
            {
                rotmat = new Vector3[3]
            };
            ship.location.X = 0;
            ship.location.Y = 0;
            ship.location.Z = 0;

            VectorMaths.set_init_matrix(ref ship.rotmat);
            ship.rotmat[2].Z = 1;
            ship.rotmat[0].X = -1;
            ship.type = (SHIP)(-96);
            ship.velocity = flight_speed;
            ship.acceleration = 0;
            ship.bravery = 0;
            ship.rotz = 0;
            ship.rotx = 0;

            pilot.auto_pilot_ship(ref ship);

            flight_speed = ship.velocity > 22 ? 22 : ship.velocity;

            if (ship.acceleration > 0)
            {
                flight_speed++;
                if (flight_speed > 22)
                {
                    flight_speed = 22;
                }
            }

            if (ship.acceleration < 0)
            {
                flight_speed--;
                if (flight_speed < 1)
                {
                    flight_speed = 1;
                }
            }

            if (ship.rotx == 0)
            {
                flight_climb = 0;
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
                flight_roll = -14;
            }
            else
            {
                if (ship.rotz == 0)
                {
                    flight_roll = 0;
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

        private static void run_escape_sequence()
        {
            int i;
            Vector3[] rotmat = new Vector3[3];

            current_screen = SCR.SCR_ESCAPE_POD;

            flight_speed = 1;
            flight_roll = 0;
            flight_climb = 0;

            VectorMaths.set_init_matrix(ref rotmat);
            rotmat[2].Z = 1.0f;

            int newship = swat.add_new_ship(SHIP.SHIP_COBRA3, 0, 0, 200, rotmat, -127, -127);
            space.universe[newship].velocity = 7;
            sound.PlaySample(Sfx.Launch);

            for (i = 0; i < 90; i++)
            {
                if (i == 40)
                {
                    space.universe[newship].flags |= FLG.FLG_DEAD;
                    sound.PlaySample(Sfx.Explode);
                }

                alg_gfx.SetClipRegion(1, 1, 510, 383);
                alg_gfx.ClearDisplay();
                Stars.update_starfield();
                space.update_universe();

                space.universe[newship].location.X = 0;
                space.universe[newship].location.Y = 0;
                space.universe[newship].location.Z += 2;

                alg_gfx.DrawTextCentre(358, "Escape pod launched - Ship auto-destuct initiated.", 120, GFX_COL.GFX_COL_WHITE);

                space.update_console();
                alg_gfx.ScreenUpdate();
            }


            while ((space.ship_count[(int)SHIP.SHIP_CORIOLIS] == 0) && (space.ship_count[(int)SHIP.SHIP_DODEC] == 0))
            {
                auto_dock();

                if ((Math.Abs(flight_roll) < 3) && (Math.Abs(flight_climb) < 3))
                {
                    for (i = 0; i < MAX_UNIV_OBJECTS; i++)
                    {
                        if (space.universe[i].type != 0)
                        {
                            space.universe[i].location.Z -= 1500;
                        }
                    }
                }

                Stars.warp_stars = true;
                alg_gfx.SetClipRegion(1, 1, 510, 383);
                alg_gfx.ClearDisplay();
                Stars.update_starfield();
                space.update_universe();
                space.update_console();
                alg_gfx.ScreenUpdate();
            }

            swat.abandon_ship();
        }

        private static void handle_flight_keys()
        {
            char keyasc;

            if (docked &&
                ((current_screen == SCR.SCR_MARKET_PRICES) ||
                 (current_screen == SCR.SCR_OPTIONS) ||
                 (current_screen == SCR.SCR_SETTINGS) ||
                 (current_screen == SCR.SCR_EQUIP_SHIP)))
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
                if (keyboard.IsKeyPressed(CommandKey.Resume))
                {
                    game_paused = false;
                }

                return;
            }

            if (keyboard.IsKeyPressed(CommandKey.F1))
            {
                find_input = false;

                if (docked)
                {
                    space.launch_player();
                }
                else
                {
                    if (current_screen != SCR.SCR_FRONT_VIEW)
                    {
                        current_screen = SCR.SCR_FRONT_VIEW;
                        Stars.flip_stars();
                    }
                }
            }

            if (keyboard.IsKeyPressed(CommandKey.F2))
            {
                find_input = false;

                if (!docked)
                {
                    if (current_screen != SCR.SCR_REAR_VIEW)
                    {
                        current_screen = SCR.SCR_REAR_VIEW;
                        Stars.flip_stars();
                    }
                }
            }

            if (keyboard.IsKeyPressed(CommandKey.F3))
            {
                find_input = false;

                if (!docked)
                {
                    if (current_screen != SCR.SCR_LEFT_VIEW)
                    {
                        current_screen = SCR.SCR_LEFT_VIEW;
                        Stars.flip_stars();
                    }
                }
            }

            if (keyboard.IsKeyPressed(CommandKey.F4))
            {
                find_input = false;

                if (docked)
                {
                    Equipment.equip_ship();
                }
                else
                {
                    if (current_screen != SCR.SCR_RIGHT_VIEW)
                    {
                        current_screen = SCR.SCR_RIGHT_VIEW;
                        Stars.flip_stars();
                    }
                }
            }

            if (keyboard.IsKeyPressed(CommandKey.F5))
            {
                find_input = false;
                old_cross.X = -1f;
                GalacticChart.display_galactic_chart();
            }

            if (keyboard.IsKeyPressed(CommandKey.F6))
            {
                find_input = false;
                old_cross.X = -1f;
                GalacticChart.display_short_range_chart();
            }

            if (keyboard.IsKeyPressed(CommandKey.F7))
            {
                find_input = false;
                PlanetData.display_data_on_planet();
            }

            if (keyboard.IsKeyPressed(CommandKey.F8) && (!witchspace))
            {
                find_input = false;
                Market.display_market_prices();
            }

            if (keyboard.IsKeyPressed(CommandKey.F9))
            {
                find_input = false;
                CommanderStatus.display_commander_status();
            }

            if (keyboard.IsKeyPressed(CommandKey.F10))
            {
                find_input = false;
                Market.display_inventory();
            }

            if (keyboard.IsKeyPressed(CommandKey.F11))
            {
                find_input = false;
                options.display_options();
            }

            if (find_input)
            {
                keyasc = keyboard.kbd_read_key();

                if (keyboard.IsKeyPressed(CommandKey.Enter))
                {
                    find_input = false;
                    GalacticChart.find_planet_by_name(find_name);
                    return;
                }

                if (keyboard.IsKeyPressed(CommandKey.Backspace))
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

            if (keyboard.IsKeyPressed(CommandKey.Y))
            {
                y_pressed();
            }

            if (keyboard.IsKeyPressed(CommandKey.N))
            {
                n_pressed();
            }

            if (keyboard.IsKeyPressed(CommandKey.Fire))
            {
                if ((!docked) && (draw_lasers == 0))
                {
                    draw_lasers = swat.fire_laser();
                }
            }

            if (keyboard.IsKeyPressed(CommandKey.Dock))
            {
                if (!docked && cmdr.docking_computer)
                {
                    if (config.InstantDock)
                    {
                        space.engage_docking_computer();
                    }
                    else
                    {
                        pilot.engage_auto_pilot();
                    }
                }
            }

            if (keyboard.IsKeyPressed(CommandKey.D))
            {
                d_pressed();
            }

            if (keyboard.IsKeyPressed(CommandKey.ECM))
            {
                if (!docked && cmdr.ecm)
                {
                    swat.activate_ecm(true);
                }
            }

            if (keyboard.IsKeyPressed(CommandKey.Find))
            {
                f_pressed();
            }

            if (keyboard.IsKeyPressed(CommandKey.Hyperspace) && (!docked))
            {
                if (keyboard.IsKeyPressed(CommandKey.CTRL))
                {
                    space.start_galactic_hyperspace();
                }
                else
                {
                    space.start_hyperspace();
                }
            }

            if (keyboard.IsKeyPressed(CommandKey.Jump) && (!docked) && (!witchspace))
            {
                space.jump_warp();
            }

            if (keyboard.IsKeyPressed(CommandKey.FireMissile))
            {
                if (!docked)
                {
                    swat.fire_missile();
                }
            }

            if (keyboard.IsKeyPressed(CommandKey.Origin))
            {
                o_pressed();
            }

            if (keyboard.IsKeyPressed(CommandKey.Pause))
            {
                game_paused = true;
            }

            if (keyboard.IsKeyPressed(CommandKey.TargetMissile))
            {
                if (!docked)
                {
                    swat.arm_missile();
                }
            }

            if (keyboard.IsKeyPressed(CommandKey.UnarmMissile))
            {
                if (!docked)
                {
                    swat.unarm_missile();
                }
            }

            if (keyboard.IsKeyPressed(CommandKey.IncSpeed))
            {
                if (!docked)
                {
                    if (flight_speed < myship.max_speed)
                    {
                        flight_speed++;
                    }
                }
            }

            if (keyboard.IsKeyPressed(CommandKey.DecSpeed))
            {
                if (!docked)
                {
                    if (flight_speed > 1)
                    {
                        flight_speed--;
                    }
                }
            }

            if (keyboard.IsKeyPressed(CommandKey.Up))
            {
                arrow_up();
            }

            if (keyboard.IsKeyPressed(CommandKey.Down))
            {
                arrow_down();
            }

            if (keyboard.IsKeyPressed(CommandKey.Left))
            {
                arrow_left();
            }

            if (keyboard.IsKeyPressed(CommandKey.Right))
            {
                arrow_right();
            }

            if (keyboard.IsKeyPressed(CommandKey.Enter))
            {
                return_pressed();
            }

            if (keyboard.IsKeyPressed(CommandKey.EnergyBomb))
            {
                if ((!docked) && cmdr.energy_bomb)
                {
                    detonate_bomb = true;
                    cmdr.energy_bomb = false;
                }
            }

            if (keyboard.IsKeyPressed(CommandKey.Escape))
            {
                if ((!docked) && cmdr.escape_pod && (!witchspace))
                {
                    run_escape_sequence();
                }
            }
        }

        internal static void save_commander_screen()
        {
            //elite.current_screen = SCR.SCR_SAVE_CMDR;

            //         elite.alg_gfx.ClearDisplay();
            //         elite.alg_gfx.DrawTextCentre(20, "SAVE COMMANDER", 140, GFX_COL.GFX_COL_GOLD);
            //         elite.alg_gfx.DrawLine(0, 36, 511, 36);
            //         elite.alg_gfx.ScreenUpdate();

            //string path = elite.cmdr.name;
            //path = ".nkc";

            //bool okay = elite.alg_gfx.RequestFile("Save Commander", path, "nkc");

            //if (!okay)
            //{
            //	options.display_options();
            //	return;
            //}

            //bool rv = SaveFile.save_commander_file(path);

            //if (rv)
            //{
            //             elite.alg_gfx.DrawTextCentre(175, "Error Saving Commander!", 140, GFX_COL.GFX_COL_GOLD);
            //	return;
            //}

            //         elite.alg_gfx.DrawTextCentre(175, "Commander Saved.", 140, GFX_COL.GFX_COL_GOLD);

            //set_commander_name(path);
            //elite.saved_cmdr = elite.cmdr;
            //elite.saved_cmdr.ship_x = elite.docked_planet.d;
            //elite.saved_cmdr.ship_y = elite.docked_planet.b;
        }

        internal static void load_commander_screen()
        {
            int key;
            string name = elite.cmdr.name;

            do
            {
                draw.DrawLoadCommander(false, name);

                key = keyboard.ReadKey();
                if (key is >= 'A' and <= 'Z')
                {
                    name += (char)key;
                }
                else if (key is (int)CommandKey.Backspace)
                {
                    if (!string.IsNullOrEmpty(name))
                    {
                        name = name[..^1]; ;
                    }
                }
            } while (key != (int)CommandKey.Enter);

            Commander? cmdr = SaveFile.LoadCommanderAsync(name).Result;

            if (cmdr == null)
            {
                draw.DrawLoadCommander(true, name);
                do
                {
                    key = keyboard.ReadKey();
                } while (key != (int)CommandKey.Space);

                cmdr = CommanderFactory.Jameson();
            }

            saved_cmdr = (Commander)cmdr;
            restore_saved_commander();
            space.update_console();
        }

        private static void run_first_intro_screen()
        {
            current_screen = SCR.SCR_INTRO_ONE;

            sound.PlayMidi(Music.EliteTheme, true);

            intro.initialise_intro1();

            for (; ; )
            {
                intro.update_intro1();

                alg_gfx.ScreenUpdate();

                keyboard.kbd_poll_keyboard();

                if (keyboard.IsKeyPressed(CommandKey.Y))
                {
                    sound.StopMidi();
                    load_commander_screen();
                    break;
                }

                if (keyboard.IsKeyPressed(CommandKey.N))
                {
                    sound.StopMidi();
                    break;
                }
            }

        }

        private static void run_second_intro_screen()
        {
            current_screen = SCR.SCR_INTRO_TWO;

            sound.PlayMidi(Music.BlueDanube, true);

            intro.initialise_intro2();

            flight_speed = 3;
            flight_roll = 0;
            flight_climb = 0;

            for (; ; )
            {
                intro.update_intro2();

                alg_gfx.ScreenUpdate();

                keyboard.kbd_poll_keyboard();

                if (keyboard.IsKeyPressed(CommandKey.Space))
                {
                    break;
                }
            }

            sound.StopMidi();
        }

        /*
		 * Draw the game over sequence. 
		 */
        private static void run_game_over_screen()
        {
            int i;
            int newship;
            Vector3[] rotmat = new Vector3[3];
            SHIP type;

            current_screen = SCR.SCR_GAME_OVER;
            alg_gfx.SetClipRegion(1, 1, 510, 383);

            flight_speed = 6;
            flight_roll = 0;
            flight_climb = 0;
            swat.clear_universe();

            VectorMaths.set_init_matrix(ref rotmat);

            newship = swat.add_new_ship(SHIP.SHIP_COBRA3, 0, 0, -400, rotmat, 0, 0);
            space.universe[newship].flags |= FLG.FLG_DEAD;

            for (i = 0; i < 5; i++)
            {
                type = random.rand255().IsOdd() ? SHIP.SHIP_CARGO : SHIP.SHIP_ALLOY;
                newship = swat.add_new_ship(type, (random.rand255() & 63) - 32, (random.rand255() & 63) - 32, -400, rotmat, 0, 0);
                space.universe[newship].rotz = ((random.rand255() * 2) & 255) - 128;
                space.universe[newship].rotx = ((random.rand255() * 2) & 255) - 128;
                space.universe[newship].velocity = random.rand255() & 15;
            }


            for (i = 0; i < 100; i++)
            {
                alg_gfx.ClearDisplay();
                Stars.update_starfield();
                space.update_universe();
                alg_gfx.DrawTextCentre(190, "GAME OVER", 140, GFX_COL.GFX_COL_GOLD);
                alg_gfx.ScreenUpdate();
            }
        }

        /*
		 * Draw a break pattern (for launching, docking and hyperspacing).
		 * Just draw a very simple one for the moment.
		 */
        private static void display_break_pattern()
        {
            alg_gfx.SetClipRegion(1, 1, 510, 383);
            alg_gfx.ClearDisplay();

            for (int i = 0; i < 20; i++)
            {
                alg_gfx.DrawCircle(new(256f, 192f), 30f + (i * 15f), GFX_COL.GFX_COL_WHITE);
                alg_gfx.ScreenUpdate();
            }

            if (docked)
            {
                missions.check_mission_brief();
                CommanderStatus.display_commander_status();
                space.update_console();
            }
            else
            {
                current_screen = SCR.SCR_FRONT_VIEW;
            }
        }

        internal static void info_message(string message)
        {
            message_string = message;
            message_count = 37;
            //	sound.snd_play_sample (SND_BEEP);
        }

        private static void initialise_allegro()
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

        public static int main(ref IGfx alg_gfx, ref ISound sound, ref IKeyboard keyboard)
        {
            elite.alg_gfx = alg_gfx;
            elite.sound = sound;
            elite.keyboard = keyboard;
            draw = new Draw(elite.alg_gfx);

            initialise_allegro();
            config = ConfigFile.ReadConfigAsync().Result;
            elite.alg_gfx.SpeedCap = config.SpeedCap;

            /* Do any setup necessary for the keyboard... */
            keyboard.kbd_keyboard_startup();

            finish = false;
            auto_pilot = false;

            while (!finish)
            {
                game_over = false;
                initialise_game();
                space.dock_player();

                space.update_console();

                current_screen = SCR.SCR_FRONT_VIEW;
                run_first_intro_screen();
                run_second_intro_screen();

                old_cross.X = -1f;
                old_cross.Y = -1f;

                space.dock_player();
                CommanderStatus.display_commander_status();

                while (!game_over)
                {
                    sound.UpdateSound();
                    elite.alg_gfx.ScreenUpdate();
                    elite.alg_gfx.SetClipRegion(1, 1, 510, 383);

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
                        if (flight_roll > 0)
                        {
                            space.decrease_flight_roll();
                        }

                        if (flight_roll < 0)
                        {
                            space.increase_flight_roll();
                        }
                    }

                    if (!climbing)
                    {
                        if (flight_climb > 0)
                        {
                            space.decrease_flight_climb();
                        }

                        if (flight_climb < 0)
                        {
                            space.increase_flight_climb();
                        }
                    }

                    if (!docked)
                    {
                        elite.alg_gfx.ScreenAcquire();

                        if (current_screen is
                            SCR.SCR_FRONT_VIEW or SCR.SCR_REAR_VIEW or
                            SCR.SCR_LEFT_VIEW or SCR.SCR_RIGHT_VIEW or
                            SCR.SCR_INTRO_ONE or SCR.SCR_INTRO_TWO or
                            SCR.SCR_GAME_OVER)
                        {
                            elite.alg_gfx.ClearDisplay();
                            Stars.update_starfield();
                        }

                        if (auto_pilot)
                        {
                            auto_dock();
                            if ((mcount & 127) == 0)
                            {
                                info_message("Docking Computers On");
                            }
                        }

                        space.update_universe();

                        if (docked)
                        {
                            space.update_console();
                            elite.alg_gfx.ScreenRelease();
                            continue;
                        }

                        if (current_screen is
                            SCR.SCR_FRONT_VIEW or SCR.SCR_REAR_VIEW or
                            SCR.SCR_LEFT_VIEW or SCR.SCR_RIGHT_VIEW)
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
                            elite.alg_gfx.DrawTextCentre(358, message_string, 120, GFX_COL.GFX_COL_WHITE);
                        }

                        if (space.hyper_ready)
                        {
                            space.display_hyper_status();
                            if ((mcount & 3) == 0)
                            {
                                space.countdown_hyperspace();
                            }
                        }

                        elite.alg_gfx.ScreenRelease();

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
                            if (energy < 50)
                            {
                                info_message("ENERGY LOW");
                                sound.PlaySample(Sfx.Beep);
                            }

                            space.update_altitude();
                        }

                        if ((mcount & 31) == 20)
                        {
                            space.update_cabin_temp();
                        }

                        if ((mcount == 0) && (!witchspace))
                        {
                            swat.random_encounter();
                        }

                        swat.cool_laser();
                        swat.time_ecm();

                        space.update_console();
                    }

                    if (current_screen == SCR.SCR_BREAK_PATTERN)
                    {
                        display_break_pattern();
                    }

                    if (cross_timer > 0)
                    {
                        cross_timer--;
                        if (cross_timer == 0)
                        {
                            GalacticChart.show_distance_to_planet();
                        }
                    }

                    if ((GalacticChart.cross.X != old_cross.X) ||
                        (GalacticChart.cross.Y != old_cross.Y))
                    {
                        old_cross = GalacticChart.cross;

                        if (current_screen == SCR.SCR_GALACTIC_CHART)
                        {
                            draw.DrawGalacticChart(cmdr.galaxy_number + 1, GalacticChart.planetPixels, GalacticChart.planetName, GalacticChart.distanceToPlanet);
                        }
                        else if (current_screen == SCR.SCR_SHORT_RANGE)
                        {
                            draw.DrawShortRangeChart(GalacticChart.planetNames, GalacticChart.planetSizes, GalacticChart.planetName, GalacticChart.distanceToPlanet);
                        }

                        draw_cross(old_cross);
                    }
                }

                if (!finish)
                {
                    run_game_over_screen();
                }
            }

            return 0;
        }

        //END_OF_MAIN();
    }
}