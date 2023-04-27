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
    using Elite.Engine.Save;
    using Elite.Engine.Ships;
    using Elite.Engine.Types;
    using Elite.Engine.Views;

    public class elite
    {
        private readonly IGfx _gfx;
        private readonly Audio _audio;
        private readonly IKeyboard _keyboard;
        internal static Scanner scanner;
        private readonly space _space;
        private readonly Stars _stars;
        private readonly threed _threed;
        private readonly pilot _pilot;
        private readonly swat _swat;
        private readonly trade _trade;
        private readonly Planet _planet;
        private readonly SaveFile _save;
        private readonly PlayerShip _ship;

        internal const int MAX_UNIV_OBJECTS = 20;
        internal static int carry_flag = 0;
        internal static ConfigSettings config = new();
        internal static Vector2 scanner_centre = new(253, 63 + 385);
        internal static Vector2 compass_centre = new(382, 22 + 385);
        internal static bool docked;
        internal static bool exitGame;
        internal static float laser_temp;
        internal static bool detonate_bomb;
        internal static bool auto_pilot;
        private readonly Draw _draw;
        readonly long oneSec = TimeSpan.FromSeconds(1).Ticks;
        readonly FC lockObj = new();
        readonly TimeSpan timeout = TimeSpan.FromMilliseconds(1000 / (config.Fps * 2));
        private readonly GameState _gameState;
        private static readonly Dictionary<SCR, IView> _views = new();
        internal static Vector2 cross = new(0, 0);
        internal static bool drawLasers;
        internal static int mcount;
        private static int message_count;
        private static string message_string;
        private static bool game_paused;
        //private static bool have_joystick;
        internal static float distanceToPlanet;

        internal class FC
        {
            internal int drawn = 0;
            internal int missed = 0;
            internal List<long> framesDrawn = new();
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

        /*
		 * Initialise the game parameters.
		 */
        private void initialise_game()
        {
            if (_gameState.IsInitialised)
            {
                return;
            }

            _gameState.Reset();
            _ship.Reset();
            _save.GetLastSave();
            _gameState.restore_saved_commander();

            _ship.speed = 1;
            docked = true;
            drawLasers = false;
            mcount = 0;
            space.hyper_ready = false;
            detonate_bomb = false;
            game_paused = false;
            auto_pilot = false;

            Stars.create_new_stars();
            swat.clear_universe();

            cross = new(-1, -1);

            _space.dock_player();

            _gameState.SetView(SCR.SCR_INTRO_ONE);
        }

        internal static void ExitGame()
        {
            exitGame = true;
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
                if (_keyboard.IsKeyPressed(CommandKey.Resume))
                {
                    game_paused = false;
                }

                return;
            }

            if (_keyboard.IsKeyPressed(CommandKey.F1))
            {
                if (_gameState.currentScreen is not SCR.SCR_INTRO_ONE and not SCR.SCR_INTRO_TWO)
                {
                    if (docked)
                    {
                        _gameState.SetView(SCR.SCR_UNDOCKING);
                    }
                    else
                    {
                        _gameState.SetView(SCR.SCR_FRONT_VIEW);
                    }
                }
            }

            if (_keyboard.IsKeyPressed(CommandKey.F2))
            {
                if (!docked)
                {
                    _gameState.SetView(SCR.SCR_REAR_VIEW);
                }
            }

            if (_keyboard.IsKeyPressed(CommandKey.F3))
            {
                if (!docked)
                {
                    _gameState.SetView(SCR.SCR_LEFT_VIEW);
                }
            }

            if (_keyboard.IsKeyPressed(CommandKey.F4))
            {
                if (docked)
                {
                    _gameState.SetView(SCR.SCR_EQUIP_SHIP);
                }
                else
                {
                    _gameState.SetView(SCR.SCR_RIGHT_VIEW);
                }
            }

            if (_keyboard.IsKeyPressed(CommandKey.F5))
            {
                _gameState.SetView(SCR.SCR_GALACTIC_CHART);
            }

            if (_keyboard.IsKeyPressed(CommandKey.F6))
            {
                _gameState.SetView(SCR.SCR_SHORT_RANGE);
            }

            if (_keyboard.IsKeyPressed(CommandKey.F7))
            {
                _gameState.SetView(SCR.SCR_PLANET_DATA);
            }

            if (_keyboard.IsKeyPressed(CommandKey.F8) && (!_gameState.witchspace))
            {
                _gameState.SetView(SCR.SCR_MARKET_PRICES);
            }

            if (_keyboard.IsKeyPressed(CommandKey.F9))
            {
                _gameState.SetView(SCR.SCR_CMDR_STATUS);
            }

            if (_keyboard.IsKeyPressed(CommandKey.F10))
            {
                _gameState.SetView(SCR.SCR_INVENTORY);
            }

            if (_keyboard.IsKeyPressed(CommandKey.F11))
            {
                _gameState.SetView(SCR.SCR_OPTIONS);
            }

            if (_keyboard.IsKeyPressed(CommandKey.Fire))
            {
                drawLasers = _swat.FireLaser();
            }

            if (_keyboard.IsKeyPressed(CommandKey.DockingComputerOn))
            {
                if (!docked && _ship.hasDockingComputer)
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

            if (_keyboard.IsKeyPressed(CommandKey.ECM))
            {
                if (!docked && _ship.hasECM)
                {
                    _swat.activate_ecm(true);
                }
            }

            if (_keyboard.IsKeyPressed(CommandKey.Hyperspace) && (!docked))
            {
                if (_keyboard.IsKeyPressed(CommandKey.Ctrl))
                {
                    _space.start_galactic_hyperspace();
                }
                else
                {
                    _space.start_hyperspace();
                }
            }

            if (_keyboard.IsKeyPressed(CommandKey.Jump) && (!docked) && (!_gameState.witchspace))
            {
                space.jump_warp();
            }

            if (_keyboard.IsKeyPressed(CommandKey.FireMissile))
            {
                if (!docked)
                {
                    _swat.fire_missile();
                }
            }

            if (_keyboard.IsKeyPressed(CommandKey.Pause))
            {
                game_paused = true;
            }

            if (_keyboard.IsKeyPressed(CommandKey.TargetMissile))
            {
                if (!docked)
                {
                    _swat.arm_missile();
                }
            }

            if (_keyboard.IsKeyPressed(CommandKey.UnarmMissile))
            {
                if (!docked)
                {
                    _swat.unarm_missile();
                }
            }

            if (_keyboard.IsKeyPressed(CommandKey.IncreaseSpeed))
            {
                if (!docked)
                {
                    _ship.IncreaseSpeed();
                }
            }

            if (_keyboard.IsKeyPressed(CommandKey.DecreaseSpeed))
            {
                if (!docked)
                {
                    _ship.DecreaseSpeed();
                }
            }

            if (_keyboard.IsKeyPressed(CommandKey.EnergyBomb))
            {
                if ((!docked) && _ship.hasEnergyBomb)
                {
                    detonate_bomb = true;
                    _ship.hasEnergyBomb = false;
                }
            }

            if (_keyboard.IsKeyPressed(CommandKey.EscapePod))
            {
                if ((!docked) && _ship.hasEscapePod && (!_gameState.witchspace))
                {
                    _gameState.SetView(SCR.SCR_ESCAPE_POD);
                }
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

            //have_joystick = false;

            //if (install_joystick(JOY_TYPE_AUTODETECT) == 0)
            //{
            //	have_joystick = (num_joysticks > 0);
            //}
        }

        public elite(IGfx alg_gfx, ISound sound, IKeyboard keyboard)
        {
            _gfx = alg_gfx;
            _audio = new Audio(sound);
            _audio.LoadSounds();
            _keyboard = keyboard;
            _gameState = new(_keyboard, _views);
            _ship = new();
            _save = new(_gameState, _ship);
            _planet = new(_gameState);
            _draw = new(_gfx);
            _draw.LoadImages();
            _draw.DrawBorder();

            scanner = new Scanner(_gameState, _gfx, _draw, space.universe, space.ship_count, _ship);

            initialise_allegro();
            config = ConfigFile.ReadConfigAsync().Result;
            
            _threed = new(_gfx, _draw);
            _stars = new(_gameState, _gfx, _ship);
            _pilot = new(_gameState, _audio);
            _swat = new(_gameState, _audio, _ship);
            _trade = new(_gameState, _swat, _ship);
            _space = new(_gameState, _gfx, _threed, _audio, _pilot, _swat, _trade, _planet, _ship);

            _views.Add(SCR.SCR_INTRO_ONE, new Intro1(_gameState, _gfx, _audio, keyboard, _ship));
            _views.Add(SCR.SCR_INTRO_TWO, new Intro2(_gameState, _gfx, _audio, keyboard, _stars, _ship));
            _views.Add(SCR.SCR_GALACTIC_CHART, new GalacticChart(_gameState, _gfx, _draw, keyboard, _planet, _ship));
            _views.Add(SCR.SCR_SHORT_RANGE, new ShortRangeChart(_gameState, _gfx, _draw, keyboard, _planet, _ship));
            _views.Add(SCR.SCR_PLANET_DATA, new PlanetData(_gameState, _gfx, _draw));
            _views.Add(SCR.SCR_MARKET_PRICES, new Market(_gameState, _gfx, _draw, keyboard, _trade, _ship));
            _views.Add(SCR.SCR_CMDR_STATUS, new CommanderStatus(_gameState, _gfx, _draw, _ship));
            _views.Add(SCR.SCR_FRONT_VIEW, new PilotFrontView(_gameState, _gfx, keyboard, _stars, _pilot, _ship));
            _views.Add(SCR.SCR_REAR_VIEW, new PilotRearView(_gameState, _gfx, keyboard, _stars, _pilot, _ship));
            _views.Add(SCR.SCR_LEFT_VIEW, new PilotLeftView(_gameState, _gfx, keyboard, _stars, _pilot, _ship));
            _views.Add(SCR.SCR_RIGHT_VIEW, new PilotRightView(_gameState, _gfx, keyboard, _stars, _pilot, _ship));
            _views.Add(SCR.SCR_DOCKING, new Docking(_gameState, _gfx, _audio, _space));
            _views.Add(SCR.SCR_UNDOCKING, new Launch(_gameState, _gfx, _audio, _space));
            _views.Add(SCR.SCR_HYPERSPACE, new Hyperspace(_gameState, _gfx, _audio));
            _views.Add(SCR.SCR_INVENTORY, new Inventory(_gameState, _gfx, _draw, _ship));
            _views.Add(SCR.SCR_EQUIP_SHIP, new Equipment(_gameState, _gfx, _draw, keyboard, _ship));
            _views.Add(SCR.SCR_OPTIONS, new Options(_gameState, _gfx, _draw, keyboard));
            _views.Add(SCR.SCR_LOAD_CMDR, new LoadCommander(_gameState, _gfx, _draw, keyboard, _planet, _save));
            _views.Add(SCR.SCR_SAVE_CMDR, new SaveCommander(_gameState, _gfx, _draw, keyboard, _save));
            _views.Add(SCR.SCR_QUIT, new Quit(_gameState, _gfx, _draw, keyboard));
            _views.Add(SCR.SCR_SETTINGS, new Settings(_gameState, _gfx, _draw, keyboard));
            _views.Add(SCR.SCR_MISSION_1, new ConstrictorMission(_gameState, _gfx, _draw, keyboard, _ship));
            _views.Add(SCR.SCR_MISSION_2, new ThargoidMission(_gameState, _gfx, _draw, keyboard, _ship));
            _views.Add(SCR.SCR_ESCAPE_POD, new EscapePod(_gameState, _gfx, _audio, _stars, _ship));
            _views.Add(SCR.SCR_GAME_OVER, new GameOverView(_gameState, _gfx, _audio, _stars, _ship));

            exitGame = false;
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
            } while (!exitGame);

            Environment.Exit(0);
        }

        private void DrawFrameElite()
        {
            initialise_game();

            _audio.UpdateSound();
            _gfx.SetClipRegion(1, 1, 510, 383);

            _ship.isRolling = false;
            _ship.isClimbing = false;

            handle_flight_keys();

            if (game_paused)
            {
                return;
            }

            if (_ship.energy < 0)
            {
                _gameState.GameOver();
            }

            if (message_count > 0)
            {
                message_count--;
            }

            _ship.LevelOut();

            if (auto_pilot)
            {
                _ship.AutoDock();
                if ((mcount & 127) == 0)
                {
                    info_message("Docking Computers On");
                }
            }

            _draw.ClearDisplay();

            _gameState.currentView.UpdateUniverse();
            _space.update_universe();
            _gameState.currentView.Draw();
            scanner.update_console();
            _gameState.currentView.HandleInput();

#if DEBUG
            DrawFps();
#endif

            if (!docked & !_gameState.IsGameOver)
            {
                swat.cool_laser();

                if (message_count > 0)
                {
                    _gfx.DrawTextCentre(358, message_string, 120, GFX_COL.GFX_COL_WHITE);
                }

                if (space.hyper_ready)
                {
                    _space.display_hyper_status();
                    if ((mcount & 3) == 0)
                    {
                        _space.countdown_hyperspace();
                    }
                }

                mcount--;
                if (mcount < 0)
                {
                    mcount = 255;
                }

                if ((mcount & 7) == 0)
                {
                    _ship.RegenerateShields();
                }

                if ((mcount & 31) == 10)
                {
                    if (_ship.IsEnergyLow())
                    {
                        info_message("ENERGY LOW");
                        _audio.PlayEffect(SoundEffect.Beep);
                    }

                    _space.update_altitude();
                }

                if ((mcount & 31) == 20)
                {
                    _space.update_cabin_temp();
                }

                if ((mcount == 0) && (!_gameState.witchspace))
                {
                    _swat.random_encounter();
                }

                _swat.time_ecm();
            }

            _gfx.ScreenUpdate();
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
                throw;
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