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

namespace Elite.Engine
{
    using System.Diagnostics;
    using System.Numerics;
    using Elite.Common.Enums;
    using Elite.Engine.Config;
    using Elite.Engine.Enums;
    using Elite.Engine.Missions;
    using Elite.Engine.Save;
    using Elite.Engine.Ships;
    using Elite.Engine.Types;
    using Elite.Engine.Views;

    public class elite
    {
        private readonly IGfx _gfx;
        private readonly Audio _audio;
        internal static IKeyboard keyboard;
        internal static Scanner scanner;
        private readonly Mission _mission;
        private readonly space _space;
        private readonly Stars _stars;
        private readonly threed _threed;
        private readonly pilot _pilot;
        private readonly swat _swat;
        private readonly trade _trade;

        internal const int PULSE_LASER = 0x0F;     //  15
        internal const int BEAM_LASER = 0x8F;      // 143
        internal const int MILITARY_LASER = 0x97;  // 151
        internal const int MINING_LASER = 0x32;    //  50

        internal const int MAX_UNIV_OBJECTS = 20;

        internal static galaxy_seed docked_planet;
        internal static galaxy_seed hyperspace_planet;
        internal static planet_data current_planet_data = new();

        internal static int carry_flag = 0;
        internal static bool witchspace;

        public static ConfigSettings config = new();

        internal static Vector2 scanner_centre = new(253, 63 + 385);
        internal static Vector2 compass_centre = new(382, 22 + 385);

        internal static bool docked;
        internal static bool finished;
        internal static float flight_speed;
        internal static float flight_roll;
        internal static float flight_climb;
        internal static float front_shield;
        internal static float aft_shield;
        internal static float energy;
        internal static float laser_temp;
        internal static bool detonate_bomb;
        internal static bool auto_pilot;

#if DEBUG
        internal static Commander saved_cmdr = CommanderFactory.Max();
#else
		internal static Commander saved_cmdr = CommanderFactory.Jameson();
#endif

        internal static Commander cmdr;
        internal static player_ship myship = new();
        internal static Draw draw;

        private int breakPatternCount = 0;


        readonly long oneSec = TimeSpan.FromSeconds(1).Ticks;

        FC lockObj = new();
        TimeSpan timeout = TimeSpan.FromMilliseconds(1000 / (config.Fps * 2));
        internal static State _state = new();
        private static Dictionary<SCR, IView> _views = new();

        internal class FC
        {
            internal int drawn = 0;
            internal int missed = 0;
            internal List<long> framesDrawn = new();
        }

        internal class State
        {
            internal bool initialised = false;
            internal SCR currentScreen = SCR.SCR_NONE;
            internal IView currentView;
        }

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
            cmdr = (Commander)saved_cmdr.Clone();
            docked_planet = Planet.find_planet(new(cmdr.ShipLocationX, cmdr.ShipLocationY));
            planetName = Planet.name_planet(docked_planet, false);
            hyperspace_planet = (galaxy_seed)docked_planet.Clone();
            current_planet_data = Planet.generate_planet_data(docked_planet);
            trade.generate_stock_market();
            trade.set_stock_quantities(cmdr.station_stock);
        }

        internal static Vector2 cross = new(0, 0);
        private static int draw_lasers;
        internal static int mcount;
        private static int message_count;
        private static string message_string;
        private static bool rolling;
        private static bool climbing;
        private static bool game_paused;
        //private static bool have_joystick;
        internal static string planetName;
        internal static float distanceToPlanet;

        /*
		 * Initialise the game parameters.
		 */
        private void initialise_game()
        {
            if (_state.initialised)
            {
                return;
            }

            _state.initialised = true;

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
            witchspace = false;
            game_paused = false;
            auto_pilot = false;

            Stars.create_new_stars();
            swat.clear_universe();

            cross = new(-1, -1);

            myship.max_speed = 40;      /* 0.27 Light Mach */
            myship.max_roll = 31;
            myship.max_climb = 8;       /* CF 8 */
            myship.max_fuel = 7;        // 7.0 Light Years

            dock_player();

            scanner.update_console();

            SetView(SCR.SCR_INTRO_ONE);
        }

        internal static void FinishGame()
        {
            finished = true;
        }

        private void draw_laser_sights()
        {
            int laser = 0;

            switch (_state.currentScreen)
            {
                case SCR.SCR_FRONT_VIEW:
                    _gfx.DrawTextCentre(32, "Front View", 120, GFX_COL.GFX_COL_WHITE);
                    laser = cmdr.front_laser;
                    break;

                case SCR.SCR_REAR_VIEW:
                    _gfx.DrawTextCentre(32, "Rear View", 120, GFX_COL.GFX_COL_WHITE);
                    laser = cmdr.rear_laser;
                    break;

                case SCR.SCR_LEFT_VIEW:
                    _gfx.DrawTextCentre(32, "Left View", 120, GFX_COL.GFX_COL_WHITE);
                    laser = cmdr.left_laser;
                    break;

                case SCR.SCR_RIGHT_VIEW:
                    _gfx.DrawTextCentre(32, "Right View", 120, GFX_COL.GFX_COL_WHITE);
                    laser = cmdr.right_laser;
                    break;
            }

            if (laser != 0)
            {
                float x1 = 128 * gfx.GFX_SCALE;
                float y1 = (96 - 8) * gfx.GFX_SCALE;
                float y2 = (96 - 16) * gfx.GFX_SCALE;

                _gfx.DrawLine(new(x1 - 1, y1), new(x1 - 1, y2), GFX_COL.GFX_COL_GREY_1);
                _gfx.DrawLine(new(x1, y1), new(x1, y2), GFX_COL.GFX_COL_WHITE);
                _gfx.DrawLine(new(x1 + 1, y1), new(x1 + 1, y2), GFX_COL.GFX_COL_GREY_1);

                y1 = (96 + 8) * gfx.GFX_SCALE;
                y2 = (96 + 16) * gfx.GFX_SCALE;

                _gfx.DrawLine(new(x1 - 1, y1), new(x1 - 1, y2), GFX_COL.GFX_COL_GREY_1);
                _gfx.DrawLine(new(x1, y1), new(x1, y2), GFX_COL.GFX_COL_WHITE);
                _gfx.DrawLine(new(x1 + 1, y1), new(x1 + 1, y2), GFX_COL.GFX_COL_GREY_1);

                x1 = (128f - 8f) * gfx.GFX_SCALE;
                y1 = 96f * gfx.GFX_SCALE;
                float x2 = (128 - 16) * gfx.GFX_SCALE;

                _gfx.DrawLine(new(x1, y1 - 1), new(x2, y1 - 1), GFX_COL.GFX_COL_GREY_1);
                _gfx.DrawLine(new(x1, y1), new(x2, y1), GFX_COL.GFX_COL_WHITE);
                _gfx.DrawLine(new(x1, y1 + 1), new(x2, y1 + 1), GFX_COL.GFX_COL_GREY_1);

                x1 = (128 + 8) * gfx.GFX_SCALE;
                x2 = (128 + 16) * gfx.GFX_SCALE;

                _gfx.DrawLine(new(x1, y1 - 1), new(x2, y1 - 1), GFX_COL.GFX_COL_GREY_1);
                _gfx.DrawLine(new(x1, y1), new(x2, y1), GFX_COL.GFX_COL_WHITE);
                _gfx.DrawLine(new(x1, y1 + 1), new(x2, y1 + 1), GFX_COL.GFX_COL_GREY_1);
            }
        }

        private static void arrow_right()
        {
            switch (_state.currentScreen)
            {
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
            switch (_state.currentScreen)
            {
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
            switch (_state.currentScreen)
            {
                case SCR.SCR_EQUIP_SHIP:
                    Equipment.select_previous_equip();
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
            switch (_state.currentScreen)
            {
                case SCR.SCR_EQUIP_SHIP:
                    Equipment.select_next_equip();
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
            switch (_state.currentScreen)
            {
                case SCR.SCR_EQUIP_SHIP:
                    Equipment.buy_equip();
                    break;
            }
        }

        private void d_pressed()
        {
            switch (_state.currentScreen)
            {
                case SCR.SCR_FRONT_VIEW:
                case SCR.SCR_REAR_VIEW:
                case SCR.SCR_RIGHT_VIEW:
                case SCR.SCR_LEFT_VIEW:
                    if (auto_pilot)
                    {
                        _pilot.disengage_auto_pilot();
                    }

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

        private void run_escape_sequence()
        {
            Vector3[] rotmat = new Vector3[3];
            SetView(SCR.SCR_ESCAPE_POD);
            flight_speed = 1;
            flight_roll = 0;
            flight_climb = 0;

            VectorMaths.set_init_matrix(ref rotmat);
            rotmat[2].Z = 1.0f;

            int newship = swat.add_new_ship(SHIP.SHIP_COBRA3, new(0, 0, 200), rotmat, -127, -127);
            space.universe[newship].velocity = 7;
            _audio.PlayEffect(SoundEffect.Launch);

            for (int i = 0; i < 90; i++)
            {
                if (i == 40)
                {
                    space.universe[newship].flags |= FLG.FLG_DEAD;
                    _audio.PlayEffect(SoundEffect.Explode);
                }

                _gfx.SetClipRegion(1, 1, 510, 383);
                draw.ClearDisplay();
                _stars.update_starfield();
                _space.update_universe();

                space.universe[newship].location.X = 0;
                space.universe[newship].location.Y = 0;
                space.universe[newship].location.Z += 2;

                _gfx.DrawTextCentre(358, "Escape pod launched - Ship auto-destuct initiated.", 120, GFX_COL.GFX_COL_WHITE);

                scanner.update_console();
                _gfx.ScreenUpdate();
            }


            while ((space.ship_count[SHIP.SHIP_CORIOLIS] == 0) && (space.ship_count[SHIP.SHIP_DODEC] == 0))
            {
                auto_dock();

                if ((MathF.Abs(flight_roll) < 3) && (MathF.Abs(flight_climb) < 3))
                {
                    for (int i = 0; i < MAX_UNIV_OBJECTS; i++)
                    {
                        if (space.universe[i].type != 0)
                        {
                            space.universe[i].location.Z -= 1500;
                        }
                    }
                }

                Stars.warp_stars = true;
                _gfx.SetClipRegion(1, 1, 510, 383);
                draw.ClearDisplay();
                _stars.update_starfield();
                _space.update_universe();
                scanner.update_console();
                _gfx.ScreenUpdate();
            }

            abandon_ship();
        }

        private void handle_flight_keys()
        {
            //if (docked &&
            //    ((current_screen == SCR.SCR_MARKET_PRICES) ||
            //     (current_screen == SCR.SCR_OPTIONS) ||
            //     (current_screen == SCR.SCR_SETTINGS) ||
            //     (current_screen == SCR.SCR_EQUIP_SHIP)))
            //{
            //    keyboard.kbd_read_key();
            //}

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
                if (docked)
                {
                    _space.launch_player();
                }
                else
                {
                    if (_state.currentScreen != SCR.SCR_FRONT_VIEW)
                    {
                        SetView(SCR.SCR_FRONT_VIEW);
                        Stars.flip_stars();
                    }
                }
            }

            if (keyboard.IsKeyPressed(CommandKey.F2))
            {
                if (!docked)
                {
                    if (_state.currentScreen != SCR.SCR_REAR_VIEW)
                    {
                        SetView(SCR.SCR_REAR_VIEW);
                        Stars.flip_stars();
                    }
                }
            }

            if (keyboard.IsKeyPressed(CommandKey.F3))
            {
                if (!docked)
                {
                    if (_state.currentScreen != SCR.SCR_LEFT_VIEW)
                    {
                        SetView(SCR.SCR_LEFT_VIEW);
                        Stars.flip_stars();
                    }
                }
            }

            if (keyboard.IsKeyPressed(CommandKey.F4))
            {
                if (docked)
                {
                    Equipment.equip_ship();
                }
                else
                {
                    if (_state.currentScreen != SCR.SCR_RIGHT_VIEW)
                    {
                        SetView(SCR.SCR_RIGHT_VIEW);
                        Stars.flip_stars();
                    }
                }
            }

            if (keyboard.IsKeyPressed(CommandKey.F5))
            {
                SetView(SCR.SCR_GALACTIC_CHART);
            }

            if (keyboard.IsKeyPressed(CommandKey.F6))
            {
                SetView(SCR.SCR_SHORT_RANGE);
            }

            if (keyboard.IsKeyPressed(CommandKey.F7))
            {
                SetView(SCR.SCR_PLANET_DATA);
            }

            if (keyboard.IsKeyPressed(CommandKey.F8) && (!witchspace))
            {
                SetView(SCR.SCR_MARKET_PRICES);
            }

            if (keyboard.IsKeyPressed(CommandKey.F9))
            {
                SetView(SCR.SCR_CMDR_STATUS);
            }

            if (keyboard.IsKeyPressed(CommandKey.F10))
            {
                SetView(SCR.SCR_INVENTORY);
            }

            if (keyboard.IsKeyPressed(CommandKey.F11))
            {
                SetView(SCR.SCR_OPTIONS);
            }

            if (keyboard.IsKeyPressed(CommandKey.Fire))
            {
                if ((!docked) && (draw_lasers == 0))
                {
                    draw_lasers = _swat.fire_laser();
                }
            }

            if (keyboard.IsKeyPressed(CommandKey.Dock))
            {
                if (!docked && cmdr.docking_computer)
                {
                    if (config.InstantDock)
                    {
                        _space.engage_docking_computer();
                    }
                    else
                    {
                        _pilot.engage_auto_pilot();
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
                    _swat.activate_ecm(true);
                }
            }

            if (keyboard.IsKeyPressed(CommandKey.Hyperspace) && (!docked))
            {
                if (keyboard.IsKeyPressed(CommandKey.CTRL))
                {
                    _space.start_galactic_hyperspace();
                }
                else
                {
                    _space.start_hyperspace();
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
                    _swat.fire_missile();
                }
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
                    _swat.unarm_missile();
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
            SetView(SCR.SCR_SAVE_CMDR);
            int key;
            string name = cmdr.name;

            do
            {
                draw.DrawSaveCommander(name);

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

            cmdr.name = name;
            cmdr.ShipLocationX = docked_planet.d;
            cmdr.ShipLocationY = docked_planet.b;
            bool success = SaveFile.SaveCommanderAsync(cmdr).Result;
            draw.DrawSaveCommander(name, success);

            if (success)
            {
                saved_cmdr = (Commander)cmdr.Clone();
            }
            else
            {
                cmdr.name = saved_cmdr.name;
            }

            do
            {
                key = keyboard.ReadKey();
            } while (key != (int)CommandKey.Space);

            SetView(SCR.SCR_OPTIONS);
        }

        internal static void load_commander_screen()
        {
            SetView(SCR.SCR_LOAD_CMDR);
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

            saved_cmdr = (Commander)cmdr.Clone();
            restore_saved_commander();
            scanner.update_console();
        }

        /*
		 * Draw the game over sequence. 
		 */
        private void run_game_over_screen()
        {
            int i;
            int newship;
            Vector3[] rotmat = new Vector3[3];
            SHIP type;

            SetView(SCR.SCR_GAME_OVER);
            _gfx.SetClipRegion(1, 1, 510, 383);

            flight_speed = 6;
            flight_roll = 0;
            flight_climb = 0;
            swat.clear_universe();

            VectorMaths.set_init_matrix(ref rotmat);

            newship = swat.add_new_ship(SHIP.SHIP_COBRA3, new(0, 0, -400), rotmat, 0, 0);
            space.universe[newship].flags |= FLG.FLG_DEAD;

            for (i = 0; i < 5; i++)
            {
                type = RNG.TrueOrFalse() ? SHIP.SHIP_CARGO : SHIP.SHIP_ALLOY;
                newship = swat.add_new_ship(type, new(RNG.Random(-32, 31), RNG.Random(-32, 31), -400), rotmat, 0, 0);
                space.universe[newship].rotz = ((RNG.Random(255) * 2) & 255) - 128;
                space.universe[newship].rotx = ((RNG.Random(255) * 2) & 255) - 128;
                space.universe[newship].velocity = RNG.Random(15);
            }


            for (i = 0; i < 100; i++)
            {
                draw.ClearDisplay();
                _stars.update_starfield();
                _space.update_universe();
                _gfx.DrawTextCentre(190, "GAME OVER", 140, GFX_COL.GFX_COL_GOLD);
                _gfx.ScreenUpdate();
            }
        }

        /*
		 * Draw a break pattern (for launching, docking and hyperspacing).
		 * Just draw a very simple one for the moment.
		 */
        private void display_break_pattern()
        {
            _gfx.SetClipRegion(1, 1, 510, 383);
            draw.ClearDisplay();

            for (int i = 0; i < breakPatternCount; i++)
            {
                _gfx.DrawCircle(new(256, 192), 30 + (i * 15), GFX_COL.GFX_COL_WHITE);
            }

            _gfx.ScreenUpdate();

            breakPatternCount++;

            if (breakPatternCount == 20)
            {
                breakPatternCount = 0;

                if (docked)
                {
                    SetView(SCR.SCR_MISSION);
                }
                else
                {
                    SetView(SCR.SCR_FRONT_VIEW);
                }
            }
        }

        private void DisplayMission()
        {

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

            //have_joystick = false;

            //if (install_joystick(JOY_TYPE_AUTODETECT) == 0)
            //{
            //	have_joystick = (num_joysticks > 0);
            //}
        }

        public elite(ref IGfx alg_gfx, ref ISound sound, ref IKeyboard keyboard)
        {
            _gfx = alg_gfx;
            _audio = new Audio(sound);
            _audio.LoadSounds();

            elite.keyboard = keyboard;

            draw = new(_gfx);
            draw.LoadImages();
            draw.DrawBorder();

            scanner = new Scanner(_gfx, space.universe, space.ship_count);

            initialise_allegro();
            config = ConfigFile.ReadConfigAsync().Result;

            _threed = new(_gfx);
            _stars = new(_gfx);
            _pilot = new(_audio);
            _swat = new(this, _audio);
            _trade = new(this, _swat);
            _space = new(this, _gfx, _threed, _audio, _pilot, _swat, _trade);
            _mission = new Mission();
            _views.Add(SCR.SCR_INTRO_ONE, new Intro1(_gfx, _audio, keyboard));
            _views.Add(SCR.SCR_INTRO_TWO, new Intro2(_gfx, _audio, keyboard, _stars, _space));
            _views.Add(SCR.SCR_GALACTIC_CHART, new GalacticChart(_gfx, keyboard));
            _views.Add(SCR.SCR_SHORT_RANGE, new ShortRangeChart(_gfx, keyboard));
            _views.Add(SCR.SCR_PLANET_DATA, new PlanetData(_gfx, _mission));
            _views.Add(SCR.SCR_MARKET_PRICES, new Market(_gfx, keyboard));
            _views.Add(SCR.SCR_CMDR_STATUS, new CommanderStatus(_gfx));
            _views.Add(SCR.SCR_OPTIONS, new Options(_gfx, keyboard));
            _views.Add(SCR.SCR_QUIT, new Quit(_gfx, keyboard));
            _views.Add(SCR.SCR_SETTINGS, new Settings(_gfx, keyboard));
            _views.Add(SCR.SCR_INVENTORY, new Inventory(_gfx));

            finished = false;
            auto_pilot = false;

            long startTicks = DateTime.UtcNow.Ticks;
            long interval = (long)(100000 / config.Fps); // *10^-5

            do
            {
                long runtime = DateTime.UtcNow.Ticks - startTicks;

                if ((runtime / 100 % interval) == 0)
                {
                    //Task.Run(() => DrawFrame());
                    DrawFrame();
                }
            } while (!finished);

            Environment.Exit(0);



            //        if (!docked)
            //        {
            //            _gfx.ScreenAcquire();

            //            if (current_screen is
            //                SCR.SCR_FRONT_VIEW or SCR.SCR_REAR_VIEW or
            //                SCR.SCR_LEFT_VIEW or SCR.SCR_RIGHT_VIEW or
            //                SCR.SCR_INTRO_ONE or SCR.SCR_INTRO_TWO or
            //                SCR.SCR_GAME_OVER)
            //            {
            //                draw.ClearDisplay();
            //                _stars.update_starfield();
            //            }

            //            if (auto_pilot)
            //            {
            //                auto_dock();
            //                if ((mcount & 127) == 0)
            //                {
            //                    info_message("Docking Computers On");
            //                }
            //            }

            //            _space.update_universe();

            //            if (docked)
            //            {
            //                scanner.update_console();
            //                _gfx.ScreenRelease();
            //                continue;
            //            }

            //            if (current_screen is
            //                SCR.SCR_FRONT_VIEW or SCR.SCR_REAR_VIEW or
            //                SCR.SCR_LEFT_VIEW or SCR.SCR_RIGHT_VIEW)
            //            {
            //                if (draw_lasers != 0)
            //                {
            //                    draw.DrawLaserLines();
            //                    draw_lasers--;
            //                }

            //                draw_laser_sights();
            //            }

            //            if (message_count > 0)
            //            {
            //                _gfx.DrawTextCentre(358, message_string, 120, GFX_COL.GFX_COL_WHITE);
            //            }

            //            if (space.hyper_ready)
            //            {
            //                _space.display_hyper_status();
            //                if ((mcount & 3) == 0)
            //                {
            //                    _space.countdown_hyperspace();
            //                }
            //            }

            //            _gfx.ScreenRelease();

            //            mcount--;
            //            if (mcount < 0)
            //            {
            //                mcount = 255;
            //            }

            //            if ((mcount & 7) == 0)
            //            {
            //                space.regenerate_shields();
            //            }

            //            if ((mcount & 31) == 10)
            //            {
            //                if (energy < 50)
            //                {
            //                    info_message("ENERGY LOW");
            //                    _audio.PlayEffect(SoundEffect.Beep);
            //                }

            //                _space.update_altitude();
            //            }

            //            if ((mcount & 31) == 20)
            //            {
            //                _space.update_cabin_temp();
            //            }

            //            if ((mcount == 0) && (!witchspace))
            //            {
            //                swat.random_encounter();
            //            }

            //            swat.cool_laser();
            //            _swat.time_ecm();

            //            scanner.update_console();
            //        }

            //        if (current_screen == SCR.SCR_BREAK_PATTERN)
            //        {
            //            display_break_pattern();
            //        }

            //        if (current_screen == SCR.SCR_MISSION)
            //        {
            //            //IMission? mission = _missions.check_mission_brief();
            //            //mission?.Brief();

            //            ConstrictorMission mission = new(_gfx, _space);
            //            mission.DrawBrief();
            //            mission.Update();

            //            if (elite.keyboard.IsKeyPressed(CommandKey.Space))
            //            {
            //                //elite.current_screen = SCR.SCR_FRONT_VIEW;
            //                CommanderStatus.display_commander_status();
            //                scanner.update_console();
            //            }
            //        }

            //        if (current_screen == SCR.SCR_MISSION)
            //        {
            //            DisplayMission();
            //        }


            //    }

            //    if (!finish)
            //    {
            //        run_game_over_screen();
            //    }
            //}
        }

        private void DrawFrameElite()
        {
            initialise_game();

            _audio.UpdateSound();
            _gfx.SetClipRegion(1, 1, 510, 383);

            rolling = false;
            climbing = false;

            handle_flight_keys();

            if (game_paused)
            {
                return;
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


            draw.ClearDisplay();

            _state.currentView.UpdateUniverse();
            _space.update_universe();
            _state.currentView.Draw();
            _state.currentView.HandleInput();

#if DEBUG
            DrawFps();
#endif

            _gfx.ScreenUpdate();
        }

        internal static void SetView(SCR screen)
        {
            lock (_state)
            {
                _state.currentScreen = screen;
                _state.currentView = _views[screen];
                _state.currentView.Reset();
            }
        }

        private void DrawFrame()
        {
            bool lockTaken = false;
            long now = DateTime.UtcNow.Ticks;

            try
            {
                Monitor.TryEnter(lockObj, timeout, ref lockTaken);
                if (lockTaken)
                {
                    // The critical section.
                    DrawFrameElite();
                    lockObj.drawn++;
                    lockObj.framesDrawn.Add(now);
                }
                else
                {
                    // The lock was not acquired.
                    lockObj.missed++;
                    //Console.WriteLine($"Frames: drawn: {lockObj.drawn}, missed: {lockObj.missed}, total: {lockObj.drawn + lockObj.missed}");
                }

                //Console.WriteLine($"Frames: drawn: {lockObj.drawn}, missed: {lockObj.missed}, total: {lockObj.drawn + lockObj.missed}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception" + ex.Message);
            }
            finally
            {
                // Ensure that the lock is released.
                if (lockTaken)
                {
                    Monitor.Exit(lockObj);
                }
            }
        }

        /// <summary>
        /// Dock the player into the space station.
        /// </summary>
        internal void dock_player()
        {
            _pilot.disengage_auto_pilot();
            docked = true;
            flight_speed = 0;
            flight_roll = 0;
            flight_climb = 0;
            front_shield = 255;
            aft_shield = 255;
            energy = 255;
            myship.altitude = 255;
            myship.cabtemp = 30;
            swat.reset_weapons();
        }

        internal void decrease_energy(float amount)
        {
            energy += amount;

            if (energy <= 0)
            {
                do_game_over();
            }
        }

        /// <summary>
        /// Deplete the shields.  Drain the energy banks if the shields fail.
        /// </summary>
        /// <param name="damage"></param>
        /// <param name="front"></param>
        internal void damage_ship(int damage, bool front)
        {
            if (damage <= 0)    /* sanity check */
            {
                return;
            }

            float shield = front ? front_shield : aft_shield;

            shield -= damage;
            if (shield < 0)
            {
                decrease_energy(shield);
                shield = 0;
            }

            if (front)
            {
                front_shield = shield;
            }
            else
            {
                aft_shield = shield;
            }
        }

        internal void abandon_ship()
        {
            cmdr.escape_pod = false;
            cmdr.legal_status = 0;
            cmdr.fuel = myship.max_fuel;

            for (int i = 0; i < trade.stock_market.Length; i++)
            {
                cmdr.current_cargo[i] = 0;
            }

            _audio.PlayEffect(SoundEffect.Dock);
            dock_player();
            SetView(SCR.SCR_BREAK_PATTERN);
        }

        /// <summary>
        /// Game Over...
        /// </summary>
        internal void do_game_over()
        {
            _audio.PlayEffect(SoundEffect.Gameover);
        }

        private void DrawFps()
        {
            long secondAgo = DateTime.UtcNow.Ticks - oneSec;

            if (lockObj.framesDrawn.Count > 0)
            {
                int i;
                for (i = 0; i < lockObj.framesDrawn.Count; i++)
                {
                    if (lockObj.framesDrawn[i] > secondAgo)
                    {
                        break;
                    }
                }
                lockObj.framesDrawn.RemoveRange(0, i);
            }

            _gfx.DrawTextLeft(450, 10, $"FPS: {lockObj.framesDrawn.Count}", GFX_COL.GFX_COL_WHITE);
        }
    }
}