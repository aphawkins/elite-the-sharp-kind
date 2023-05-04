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

    public class EliteMain
    {
        private readonly IGfx _gfx;
        private readonly Audio _audio;
        private readonly IKeyboard _keyboard;
        internal static Scanner scanner;
        private readonly Space _space;
        private readonly Stars _stars;
        private readonly Threed _threed;
        private readonly Pilot _pilot;
        private readonly Combat _combat;
        private readonly Trade _trade;
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
        internal static float distanceToPlanet;

        internal class FC
        {
            internal int drawn = 0;
            internal int missed = 0;
            internal List<long> framesDrawn = new();
        }

        internal static ShipData[] ship_list = new ShipData[Ship.NO_OF_SHIPS + 1]
        {
            new(),
            Ship.missile_data,
            Ship.coriolis_data,
            Ship.esccaps_data,
            Ship.alloy_data,
            Ship.cargo_data,
            Ship.boulder_data,
            Ship.asteroid_data,
            Ship.rock_data,
            Ship.orbit_data,
            Ship.transp_data,
            Ship.cobra3a_data,
            Ship.pythona_data,
            Ship.boa_data,
            Ship.anacnda_data,
            Ship.hermit_data,
            Ship.viper_data,
            Ship.sidewnd_data,
            Ship.mamba_data,
            Ship.krait_data,
            Ship.adder_data,
            Ship.gecko_data,
            Ship.cobra1_data,
            Ship.worm_data,
            Ship.cobra3b_data,
            Ship.asp2_data,
            Ship.pythonb_data,
            Ship.ferdlce_data,
            Ship.moray_data,
            Ship.thargoid_data,
            Ship.thargon_data,
            Ship.constrct_data,
            Ship.cougar_data,
            Ship.dodec_data
        };

        /// <summary>
        /// Initialise the game parameters.
        /// </summary>
        private void InitialiseGame()
        {
            if (_gameState.IsInitialised)
            {
                return;
            }

            _gameState.Reset();
            _ship.Reset();
            _save.GetLastSave();

            _ship.speed = 1;
            docked = true;
            drawLasers = false;
            mcount = 0;
            Space.hyper_ready = false;
            detonate_bomb = false;
            game_paused = false;
            auto_pilot = false;

            Stars.CreateNewStars();
            _combat.ClearUniverse();

            cross = new(-1, -1);

            _space.DockPlayer();

            _gameState.SetView(SCR.SCR_INTRO_ONE);
        }

        internal static void ExitGame()
        {
            exitGame = true;
        }

        private void HandleFlightKeys()
        {
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
                drawLasers = _combat.FireLaser();
            }

            if (_keyboard.IsKeyPressed(CommandKey.DockingComputerOn))
            {
                if (!docked && _ship.hasDockingComputer)
                {
                    if (config.InstantDock)
                    {
                        _space.EngageDockingComputer();
                    }
                    else
                    {
                        _pilot.EngageAutoPilot();
                    }
                }
            }

            if (_keyboard.IsKeyPressed(CommandKey.ECM))
            {
                if (!docked && _ship.hasECM)
                {
                    _combat.ActivateECM(true);
                }
            }

            if (_keyboard.IsKeyPressed(CommandKey.Hyperspace) && (!docked))
            {
                if (_keyboard.IsKeyPressed(CommandKey.Ctrl))
                {
                    _space.StartGalacticHyperspace();
                }
                else
                {
                    _space.StartHyperspace();
                }
            }

            if (_keyboard.IsKeyPressed(CommandKey.Jump) && (!docked) && (!_gameState.witchspace))
            {
                _space.JumpWarp();
            }

            if (_keyboard.IsKeyPressed(CommandKey.FireMissile))
            {
                if (!docked)
                {
                    _combat.FireMissile();
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
                    _combat.ArmMissile();
                }
            }

            if (_keyboard.IsKeyPressed(CommandKey.UnarmMissile))
            {
                if (!docked)
                {
                    _combat.UnarmMissile();
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

        internal static void InfoMessage(string message)
        {
            message_string = message;
            message_count = 37;
            //	sound.snd_play_sample (SND_BEEP);
        }

        public EliteMain(IGfx alg_gfx, ISound sound, IKeyboard keyboard)
        {
            _gfx = alg_gfx;
            _audio = new Audio(sound);
            _audio.LoadSounds();
            _keyboard = keyboard;
            _gameState = new(_keyboard, _views);
            _ship = new();
            _trade = new(_ship);
            _planet = new(_gameState);
            _draw = new(_gfx);
            _draw.LoadImages();
            _draw.DrawBorder();        
            _threed = new(_gfx, _draw);
            _stars = new(_gameState, _gfx, _ship);
            _pilot = new(_gameState, _audio);
            _combat = new(_gameState, _audio, _ship, _trade);
            _save = new(_gameState, _ship, _trade);
            _space = new(_gameState, _gfx, _threed, _audio, _pilot, _combat, _trade, _ship);

            scanner = new Scanner(_gameState, _gfx, _draw, Space.universe, Space.ship_count, _ship, _combat);
            config = ConfigFile.ReadConfigAsync().Result;

            _views.Add(SCR.SCR_INTRO_ONE, new Intro1(_gameState, _gfx, _audio, keyboard, _ship, _combat));
            _views.Add(SCR.SCR_INTRO_TWO, new Intro2(_gameState, _gfx, _audio, keyboard, _stars, _ship, _combat));
            _views.Add(SCR.SCR_GALACTIC_CHART, new GalacticChart(_gameState, _gfx, _draw, keyboard, _planet, _ship));
            _views.Add(SCR.SCR_SHORT_RANGE, new ShortRangeChart(_gameState, _gfx, _draw, keyboard, _planet, _ship));
            _views.Add(SCR.SCR_PLANET_DATA, new PlanetDataView(_gameState, _gfx, _draw));
            _views.Add(SCR.SCR_MARKET_PRICES, new Market(_gameState, _gfx, _draw, keyboard, _trade));
            _views.Add(SCR.SCR_CMDR_STATUS, new CommanderStatus(_gameState, _gfx, _draw, _ship, _trade));
            _views.Add(SCR.SCR_FRONT_VIEW, new PilotFrontView(_gfx, keyboard, _stars, _pilot, _ship));
            _views.Add(SCR.SCR_REAR_VIEW, new PilotRearView(_gfx, keyboard, _stars, _pilot, _ship));
            _views.Add(SCR.SCR_LEFT_VIEW, new PilotLeftView(_gfx, keyboard, _stars, _pilot, _ship));
            _views.Add(SCR.SCR_RIGHT_VIEW, new PilotRightView(_gfx, keyboard, _stars, _pilot, _ship));
            _views.Add(SCR.SCR_DOCKING, new Docking(_gameState, _gfx, _audio, _space, _combat));
            _views.Add(SCR.SCR_UNDOCKING, new Launch(_gameState, _gfx, _audio, _space, _combat));
            _views.Add(SCR.SCR_HYPERSPACE, new Hyperspace(_gameState, _gfx, _audio));
            _views.Add(SCR.SCR_INVENTORY, new Inventory(_gfx, _draw, _ship, _trade));
            _views.Add(SCR.SCR_EQUIP_SHIP, new Equipment(_gameState, _gfx, _draw, keyboard, _ship, _trade));
            _views.Add(SCR.SCR_OPTIONS, new Options(_gameState, _gfx, _draw, keyboard));
            _views.Add(SCR.SCR_LOAD_CMDR, new LoadCommander(_gameState, _gfx, _draw, keyboard, _save));
            _views.Add(SCR.SCR_SAVE_CMDR, new SaveCommander(_gameState, _gfx, _draw, keyboard, _save));
            _views.Add(SCR.SCR_QUIT, new Quit(_gameState, _gfx, _draw, keyboard));
            _views.Add(SCR.SCR_SETTINGS, new Settings(_gameState, _gfx, _draw, keyboard));
            _views.Add(SCR.SCR_MISSION_1, new ConstrictorMission(_gameState, _gfx, _draw, keyboard, _ship, _trade, _combat));
            _views.Add(SCR.SCR_MISSION_2, new ThargoidMission(_gameState, _gfx, _draw, keyboard, _ship));
            _views.Add(SCR.SCR_ESCAPE_POD, new EscapePod(_gameState, _gfx, _audio, _stars, _ship, _trade, _combat));
            _views.Add(SCR.SCR_GAME_OVER, new GameOverView(_gameState, _gfx, _audio, _stars, _ship, _combat));

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
            InitialiseGame();

            _audio.UpdateSound();
            _gfx.SetClipRegion(1, 1, 510, 383);

            _ship.isRolling = false;
            _ship.isClimbing = false;

            HandleFlightKeys();

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
                    InfoMessage("Docking Computers On");
                }
            }

            _draw.ClearDisplay();

            _gameState.currentView.UpdateUniverse();
            _space.UpdateUniverse();
            _gameState.currentView.Draw();
            scanner.UpdateConsole();
            _gameState.currentView.HandleInput();

#if DEBUG
            DrawFps();
#endif

            if (!docked & !_gameState.IsGameOver)
            {
                _combat.CoolLaser();

                if (message_count > 0)
                {
                    _gfx.DrawTextCentre(358, message_string, 120, GFX_COL.GFX_COL_WHITE);
                }

                if (Space.hyper_ready)
                {
                    _space.DisplayHyperStatus();
                    if ((mcount & 3) == 0)
                    {
                        _space.CountdownHyperspace();
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
                        InfoMessage("ENERGY LOW");
                        _audio.PlayEffect(SoundEffect.Beep);
                    }

                    _space.UpdateAltitude();
                }

                if ((mcount & 31) == 20)
                {
                    _space.UpdateCabinTemp();
                }

                if ((mcount == 0) && (!_gameState.witchspace))
                {
                    _combat.RandomEncounter();
                }

                _combat.TimeECM();
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