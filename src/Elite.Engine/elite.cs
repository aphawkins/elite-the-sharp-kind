// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Diagnostics;
using Elite.Common.Enums;
using Elite.Engine.Enums;
using Elite.Engine.Save;
using Elite.Engine.Ships;
using Elite.Engine.Views;

namespace Elite.Engine
{
    public partial class EliteMain
    {
        internal const int MAX_UNIV_OBJECTS = 20;
        private readonly Audio _audio;
        private readonly Combat _combat;
        private readonly ConfigFile _configFile;
        private readonly Draw _draw;
        private readonly GameState _gameState;
        private readonly IGfx _gfx;
        private readonly IKeyboard _keyboard;
        private readonly FrameCounter _lockObj = new();
        private readonly long _oneSecond = TimeSpan.FromSeconds(1).Ticks;
        private readonly Pilot _pilot;
        private readonly Planet _planet;
        private readonly SaveFile _save;
        private readonly Scanner _scanner;
        private readonly PlayerShip _ship;
        private readonly Space _space;
        private readonly Stars _stars;
        private readonly Threed _threed;
        private readonly TimeSpan _timeout;
        private readonly Trade _trade;
        private readonly Dictionary<SCR, IView> _views = new();
        private bool _isGamePaused;
        public EliteMain(IGfx alg_gfx, ISound sound, IKeyboard keyboard)
        {
            _gfx = alg_gfx;
            _audio = new(sound);
            _audio.LoadSounds();
            _keyboard = keyboard;
            _gameState = new(_keyboard, _views);
            _ship = new();
            _trade = new(_ship);
            _planet = new(_gameState);
            _draw = new(_gfx);
            _draw.LoadImages();
            _draw.DrawBorder();
            _threed = new(_gameState, _gfx, _draw);
            _stars = new(_gameState, _gfx, _ship);
            _pilot = new(_gameState, _audio);
            _combat = new(_gameState, _audio, _ship, _trade);
            _save = new(_gameState, _ship, _trade, _planet);
            _space = new(_gameState, _gfx, _threed, _audio, _pilot, _combat, _trade, _ship, _planet);
            _scanner = new(_gameState, _gfx, _draw, Space.universe, Space.ship_count, _ship, _combat);
            _configFile = new();

            _gameState.Config = _configFile.ReadConfigAsync().Result;

            _views.Add(SCR.SCR_INTRO_ONE, new Intro1View(_gameState, _gfx, _audio, keyboard, _ship, _combat));
            _views.Add(SCR.SCR_INTRO_TWO, new Intro2View(_gameState, _gfx, _audio, keyboard, _stars, _ship, _combat));
            _views.Add(SCR.SCR_GALACTIC_CHART, new GalacticChartView(_gameState, _gfx, _draw, keyboard, _planet, _ship));
            _views.Add(SCR.SCR_SHORT_RANGE, new ShortRangeChartView(_gameState, _gfx, _draw, keyboard, _planet, _ship));
            _views.Add(SCR.SCR_PLANET_DATA, new PlanetDataView(_gameState, _gfx, _draw, _planet));
            _views.Add(SCR.SCR_MARKET_PRICES, new MarketView(_gameState, _gfx, _draw, keyboard, _trade, _planet));
            _views.Add(SCR.SCR_CMDR_STATUS, new CommanderStatusView(_gameState, _gfx, _draw, _ship, _trade, _planet));
            _views.Add(SCR.SCR_FRONT_VIEW, new PilotFrontView(_gameState, _gfx, keyboard, _stars, _pilot, _ship));
            _views.Add(SCR.SCR_REAR_VIEW, new PilotRearView(_gameState, _gfx, keyboard, _stars, _pilot, _ship));
            _views.Add(SCR.SCR_LEFT_VIEW, new PilotLeftView(_gameState, _gfx, keyboard, _stars, _pilot, _ship));
            _views.Add(SCR.SCR_RIGHT_VIEW, new PilotRightView(_gameState, _gfx, keyboard, _stars, _pilot, _ship));
            _views.Add(SCR.SCR_DOCKING, new DockingView(_gameState, _gfx, _audio, _space, _combat));
            _views.Add(SCR.SCR_UNDOCKING, new LaunchView(_gameState, _gfx, _audio, _space, _combat));
            _views.Add(SCR.SCR_HYPERSPACE, new HyperspaceView(_gameState, _gfx, _audio));
            _views.Add(SCR.SCR_INVENTORY, new InventoryView(_gfx, _draw, _ship, _trade));
            _views.Add(SCR.SCR_EQUIP_SHIP, new EquipmentView(_gameState, _gfx, _draw, keyboard, _ship, _trade, _scanner));
            _views.Add(SCR.SCR_OPTIONS, new OptionsView(_gameState, _gfx, _draw, keyboard));
            _views.Add(SCR.SCR_LOAD_CMDR, new LoadCommanderView(_gameState, _gfx, _draw, keyboard, _save));
            _views.Add(SCR.SCR_SAVE_CMDR, new SaveCommanderView(_gameState, _gfx, _draw, keyboard, _save));
            _views.Add(SCR.SCR_QUIT, new QuitView(_gameState, _gfx, _draw, keyboard));
            _views.Add(SCR.SCR_SETTINGS, new SettingsView(_gameState, _gfx, _draw, keyboard, _configFile));
            _views.Add(SCR.SCR_MISSION_1, new ConstrictorMissionView(_gameState, _gfx, _draw, keyboard, _ship, _trade, _combat));
            _views.Add(SCR.SCR_MISSION_2, new ThargoidMissionView(_gameState, _gfx, _draw, keyboard, _ship));
            _views.Add(SCR.SCR_ESCAPE_POD, new EscapePodView(_gameState, _gfx, _audio, _stars, _ship, _trade, _combat));
            _views.Add(SCR.SCR_GAME_OVER, new GameOverView(_gameState, _gfx, _audio, _stars, _ship, _combat));

            _timeout = TimeSpan.FromMilliseconds(1000 / (_gameState.Config.Fps * 2));

            long startTicks = DateTime.UtcNow.Ticks;
            long interval = (long)(100000 / _gameState.Config.Fps); // *10^-5

            do
            {
                long runtime = DateTime.UtcNow.Ticks - startTicks;

                if ((runtime / 100 % interval) == 0)
                {
                    //Task.Run(() => DrawFrame());
                    DrawFrame();
                }
            } while (!_gameState.ExitGame);

            Environment.Exit(0);
        }

        private void DrawFps()
        {
            long secondAgo = DateTime.UtcNow.Ticks - _oneSecond;

            if (_lockObj.FramesDrawn.Count > 0)
            {
                int i;
                for (i = 0; i < _lockObj.FramesDrawn.Count; i++)
                {
                    if (_lockObj.FramesDrawn[i] > secondAgo)
                    {
                        break;
                    }
                }
                _lockObj.FramesDrawn.RemoveRange(0, i);
            }

            _gfx.DrawTextLeft(450, 10, $"FPS: {_lockObj.FramesDrawn.Count}", GFX_COL.GFX_COL_WHITE);
        }

        private void DrawFrame()
        {
            bool lockTaken = false;
            long now = DateTime.UtcNow.Ticks;

            try
            {
                Monitor.TryEnter(_lockObj, _timeout, ref lockTaken);
                if (lockTaken)
                {
                    // The critical section.
                    DrawFrameElite();
                    _lockObj.Drawn++;
                    _lockObj.FramesDrawn.Add(now);
                }
                else
                {
                    // The lock was not acquired.
                    _lockObj.Missed++;
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
                    Monitor.Exit(_lockObj);
                }
            }
        }

        private void DrawFrameElite()
        {
            InitialiseGame();

            _audio.UpdateSound();
            _gfx.SetClipRegion(1, 1, 510, 383);

            _ship.IsRolling = false;
            _ship.IsClimbing = false;

            HandleFlightKeys();

            if (_isGamePaused)
            {
                return;
            }

            if (_ship.Energy < 0)
            {
                _gameState.GameOver();
            }

            if (_gameState.MessageCount > 0)
            {
                _gameState.MessageCount--;
            }

            _ship.LevelOut();

            if (_gameState.IsAutoPilotOn)
            {
                _ship.AutoDock();
                if ((_gameState.mcount & 127) == 0)
                {
                    _gameState.InfoMessage("Docking Computers On");
                }
            }

            _draw.ClearDisplay();

            _gameState.CurrentView!.UpdateUniverse();
            _space.UpdateUniverse();
            _gameState.CurrentView.Draw();
            _scanner.UpdateConsole();
            _gameState.CurrentView.HandleInput();

#if DEBUG
            DrawFps();
#endif

            if (!_gameState.IsDocked & !_gameState.IsGameOver)
            {
                _combat.CoolLaser();

                if (_gameState.MessageCount > 0)
                {
                    _gfx.DrawTextCentre(358, _gameState.MessageString, 120, GFX_COL.GFX_COL_WHITE);
                }

                if (Space.hyper_ready)
                {
                    _space.DisplayHyperStatus();
                    if ((_gameState.mcount & 3) == 0)
                    {
                        _space.CountdownHyperspace();
                    }
                }

                _gameState.mcount--;
                if (_gameState.mcount < 0)
                {
                    _gameState.mcount = 255;
                }

                if ((_gameState.mcount & 7) == 0)
                {
                    _ship.RegenerateShields();
                }

                if ((_gameState.mcount & 31) == 10)
                {
                    if (_ship.IsEnergyLow())
                    {
                        _gameState.InfoMessage("ENERGY LOW");
                        _audio.PlayEffect(SoundEffect.Beep);
                    }

                    _space.UpdateAltitude();
                }

                if ((_gameState.mcount & 31) == 20)
                {
                    _space.UpdateCabinTemp();
                }

                if ((_gameState.mcount == 0) && (!_gameState.InWitchspace))
                {
                    _combat.RandomEncounter();
                }

                _combat.TimeECM();
            }

            _gfx.ScreenUpdate();
        }

        private void HandleFlightKeys()
        {
            if (_isGamePaused)
            {
                if (_keyboard.IsKeyPressed(CommandKey.Resume))
                {
                    _isGamePaused = false;
                }

                return;
            }

            if (_keyboard.IsKeyPressed(CommandKey.F1))
            {
                if (_gameState.CurrentScreen is not SCR.SCR_INTRO_ONE and not SCR.SCR_INTRO_TWO)
                {
                    if (_gameState.IsDocked)
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
                if (!_gameState.IsDocked)
                {
                    _gameState.SetView(SCR.SCR_REAR_VIEW);
                }
            }

            if (_keyboard.IsKeyPressed(CommandKey.F3))
            {
                if (!_gameState.IsDocked)
                {
                    _gameState.SetView(SCR.SCR_LEFT_VIEW);
                }
            }

            if (_keyboard.IsKeyPressed(CommandKey.F4))
            {
                if (_gameState.IsDocked)
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

            if (_keyboard.IsKeyPressed(CommandKey.F8) && (!_gameState.InWitchspace))
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
                _gameState.DrawLasers = _combat.FireLaser();
            }

            if (_keyboard.IsKeyPressed(CommandKey.DockingComputerOn))
            {
                if (!_gameState.IsDocked && _ship.HasDockingComputer)
                {
                    if (_gameState.Config.InstantDock)
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
                if (!_gameState.IsDocked && _ship.HasECM)
                {
                    _combat.ActivateECM(true);
                }
            }

            if (_keyboard.IsKeyPressed(CommandKey.Hyperspace) && (!_gameState.IsDocked))
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

            if (_keyboard.IsKeyPressed(CommandKey.Jump) && (!_gameState.IsDocked) && (!_gameState.InWitchspace))
            {
                _space.JumpWarp();
            }

            if (_keyboard.IsKeyPressed(CommandKey.FireMissile))
            {
                if (!_gameState.IsDocked)
                {
                    _combat.FireMissile();
                }
            }

            if (_keyboard.IsKeyPressed(CommandKey.Pause))
            {
                _isGamePaused = true;
            }

            if (_keyboard.IsKeyPressed(CommandKey.TargetMissile))
            {
                if (!_gameState.IsDocked)
                {
                    _combat.ArmMissile();
                }
            }

            if (_keyboard.IsKeyPressed(CommandKey.UnarmMissile))
            {
                if (!_gameState.IsDocked)
                {
                    _combat.UnarmMissile();
                }
            }

            if (_keyboard.IsKeyPressed(CommandKey.IncreaseSpeed))
            {
                if (!_gameState.IsDocked)
                {
                    _ship.IncreaseSpeed();
                }
            }

            if (_keyboard.IsKeyPressed(CommandKey.DecreaseSpeed))
            {
                if (!_gameState.IsDocked)
                {
                    _ship.DecreaseSpeed();
                }
            }

            if (_keyboard.IsKeyPressed(CommandKey.EnergyBomb))
            {
                if ((!_gameState.IsDocked) && _ship.HasEnergyBomb)
                {
                    _gameState.DetonateBomb = true;
                    _ship.HasEnergyBomb = false;
                }
            }

            if (_keyboard.IsKeyPressed(CommandKey.EscapePod))
            {
                if ((!_gameState.IsDocked) && _ship.HasEscapePod && (!_gameState.InWitchspace))
                {
                    _gameState.SetView(SCR.SCR_ESCAPE_POD);
                }
            }
        }

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

            _ship.Speed = 1;
            Space.hyper_ready = false;
            _isGamePaused = false;

            Stars.CreateNewStars();
            _combat.ClearUniverse();
            _space.DockPlayer();
        }
    }
}
