// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Diagnostics;
using Elite.Common.Enums;
using Elite.Engine.Config;
using Elite.Engine.Conflict;
using Elite.Engine.Enums;
using Elite.Engine.Save;
using Elite.Engine.Ships;
using Elite.Engine.Trader;
using Elite.Engine.Views;

namespace Elite.Engine
{
    public sealed class EliteMain
    {
        internal const int MaxUniverseObjects = 20;
        private readonly Audio _audio;
        private readonly Combat _combat;
        private readonly ConfigFile _configFile;
        private readonly Draw _draw;
        private readonly GameState _gameState;
        private readonly IGraphics _graphics;
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
        private readonly Universe _universe;
        private readonly Dictionary<Screen, IView> _views = new();
        private bool _isGamePaused;

        public EliteMain(IGraphics alggraphics, ISound sound, IKeyboard keyboard)
        {
            _graphics = alggraphics;
            _audio = new(sound);
            _audio.LoadSounds();
            _keyboard = keyboard;
            _gameState = new(_keyboard, _views);
            _universe = new();
            _ship = new();
            _trade = new(_gameState, _ship);
            _planet = new(_gameState);
            _draw = new(_graphics);
            _draw.LoadImages();
            _draw.DrawBorder();
            _threed = new(_gameState, _graphics, _draw);
            _stars = new(_gameState, _graphics, _ship);
            _pilot = new(_audio, _universe, _ship);
            _combat = new(_gameState, _audio, _ship, _trade, _pilot, _universe);
            _save = new(_gameState, _ship, _trade, _planet);
            _space = new(_gameState, _graphics, _threed, _audio, _pilot, _combat, _trade, _ship, _planet, _stars, _universe);
            _scanner = new(_gameState, _graphics, _draw, _universe, _ship, _combat);
            _configFile = new();

            _gameState.Config = _configFile.ReadConfigAsync().Result;

            _views.Add(Screen.IntroOne, new Intro1View(_gameState, _graphics, _audio, keyboard, _ship, _combat, _universe));
            _views.Add(Screen.IntroTwo, new Intro2View(_gameState, _graphics, _audio, keyboard, _stars, _ship, _combat, _universe));
            _views.Add(Screen.GalacticChart, new GalacticChartView(_gameState, _graphics, _draw, keyboard, _planet, _ship));
            _views.Add(Screen.ShortRangeChart, new ShortRangeChartView(_gameState, _graphics, _draw, keyboard, _planet, _ship));
            _views.Add(Screen.PlanetData, new PlanetDataView(_gameState, _graphics, _draw, _planet));
            _views.Add(Screen.MarketPrices, new MarketView(_gameState, _graphics, _draw, keyboard, _trade, _planet));
            _views.Add(Screen.CommanderStatus, new CommanderStatusView(_gameState, _graphics, _draw, _ship, _trade, _planet, _universe));
            _views.Add(Screen.FrontView, new PilotFrontView(_gameState, _graphics, keyboard, _stars, _pilot, _ship, _space));
            _views.Add(Screen.RearView, new PilotRearView(_gameState, _graphics, keyboard, _stars, _pilot, _ship, _space));
            _views.Add(Screen.LeftView, new PilotLeftView(_gameState, _graphics, keyboard, _stars, _pilot, _ship, _space));
            _views.Add(Screen.RightView, new PilotRightView(_gameState, _graphics, keyboard, _stars, _pilot, _ship, _space));
            _views.Add(Screen.Docking, new DockingView(_gameState, _graphics, _audio, _space, _combat));
            _views.Add(Screen.Undocking, new LaunchView(_gameState, _graphics, _audio, _space, _combat));
            _views.Add(Screen.Hyperspace, new HyperspaceView(_gameState, _graphics, _audio));
            _views.Add(Screen.Inventory, new InventoryView(_graphics, _draw, _ship, _trade));
            _views.Add(Screen.EquipShip, new EquipmentView(_gameState, _graphics, _draw, keyboard, _ship, _trade, _scanner));
            _views.Add(Screen.Options, new OptionsView(_gameState, _graphics, _draw, keyboard));
            _views.Add(Screen.LoadCommander, new LoadCommanderView(_gameState, _graphics, _draw, keyboard, _save));
            _views.Add(Screen.SaveCommander, new SaveCommanderView(_gameState, _graphics, _draw, keyboard, _save));
            _views.Add(Screen.Quit, new QuitView(_gameState, _graphics, _draw, keyboard));
            _views.Add(Screen.Settings, new SettingsView(_gameState, _graphics, _draw, keyboard, _configFile));
            _views.Add(Screen.MissionOne, new ConstrictorMissionView(_gameState, _graphics, _draw, keyboard, _ship, _trade, _combat, _universe));
            _views.Add(Screen.MissionTwo, new ThargoidMissionView(_gameState, _graphics, _draw, keyboard, _ship));
            _views.Add(Screen.EscapeCapsule, new EscapeCapsuleView(_gameState, _graphics, _audio, _stars, _ship, _trade, _combat, _universe, _pilot));
            _views.Add(Screen.GameOver, new GameOverView(_gameState, _graphics, _audio, _stars, _ship, _combat, _universe));

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
            }
            while (!_gameState.ExitGame);

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

            _graphics.DrawTextLeft(450, 10, $"FPS: {_lockObj.FramesDrawn.Count}", Colour.White);
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
                Debug.WriteLine("Exception" + ex.Message);
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
            _graphics.SetClipRegion(1, 1, 510, 383);

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

            if (_pilot.IsAutoPilotOn)
            {
                _pilot.AutoDock();
                if ((_gameState.MCount & 127) == 0)
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

            if (!_gameState.IsDocked && !_gameState.IsGameOver)
            {
                _combat.CoolLaser();

                if (_gameState.MessageCount > 0)
                {
                    _graphics.DrawTextCentre(358, _gameState.MessageString, 120, Colour.White);
                }

                if (_space.IsHyperspaceReady)
                {
                    _space.DisplayHyperStatus();
                    if ((_gameState.MCount & 3) == 0)
                    {
                        _space.CountdownHyperspace();
                    }
                }

                _gameState.MCount--;
                if (_gameState.MCount < 0)
                {
                    _gameState.MCount = 255;
                }

                if ((_gameState.MCount & 7) == 0)
                {
                    _ship.RegenerateShields();
                }

                if ((_gameState.MCount & 31) == 10)
                {
                    if (_ship.IsEnergyLow())
                    {
                        _gameState.InfoMessage("ENERGY LOW");
                        _audio.PlayEffect(SoundEffect.Beep);
                    }

                    _space.UpdateAltitude();
                }

                if ((_gameState.MCount & 31) == 20)
                {
                    _space.UpdateCabinTemp();
                }

                if ((_gameState.MCount == 0) && (!_gameState.InWitchspace))
                {
                    _combat.RandomEncounter();
                }

                _combat.TimeECM();
            }

            _graphics.ScreenUpdate();
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

            if (_keyboard.IsKeyPressed(CommandKey.F1) &&
                _gameState.CurrentScreen is not Screen.IntroOne and not Screen.IntroTwo)
            {
                if (_gameState.IsDocked)
                {
                    _gameState.SetView(Screen.Undocking);
                }
                else
                {
                    _gameState.SetView(Screen.FrontView);
                }
            }

            if (_keyboard.IsKeyPressed(CommandKey.F2) &&
                !_gameState.IsDocked)
            {
                _gameState.SetView(Screen.RearView);
            }

            if (_keyboard.IsKeyPressed(CommandKey.F3) &&
                !_gameState.IsDocked)
            {
                _gameState.SetView(Screen.LeftView);
            }

            if (_keyboard.IsKeyPressed(CommandKey.F4))
            {
                if (_gameState.IsDocked)
                {
                    _gameState.SetView(Screen.EquipShip);
                }
                else
                {
                    _gameState.SetView(Screen.RightView);
                }
            }

            if (_keyboard.IsKeyPressed(CommandKey.F5))
            {
                _gameState.SetView(Screen.GalacticChart);
            }

            if (_keyboard.IsKeyPressed(CommandKey.F6))
            {
                _gameState.SetView(Screen.ShortRangeChart);
            }

            if (_keyboard.IsKeyPressed(CommandKey.F7))
            {
                _gameState.SetView(Screen.PlanetData);
            }

            if (_keyboard.IsKeyPressed(CommandKey.F8) && (!_gameState.InWitchspace))
            {
                _gameState.SetView(Screen.MarketPrices);
            }

            if (_keyboard.IsKeyPressed(CommandKey.F9))
            {
                _gameState.SetView(Screen.CommanderStatus);
            }

            if (_keyboard.IsKeyPressed(CommandKey.F10))
            {
                _gameState.SetView(Screen.Inventory);
            }

            if (_keyboard.IsKeyPressed(CommandKey.F11))
            {
                _gameState.SetView(Screen.Options);
            }

            if (_keyboard.IsKeyPressed(CommandKey.Fire))
            {
                _gameState.DrawLasers = _combat.FireLaser();
            }

            if (_keyboard.IsKeyPressed(CommandKey.DockingComputerOn) &&
                !_gameState.IsDocked && _ship.HasDockingComputer)
            {
                if (_gameState.Config.InstantDock)
                {
                    _space.EngageDockingComputer();
                }
                else if (!_gameState.InWitchspace && !_space.IsHyperspaceReady)
                {
                    _pilot.EngageAutoPilot();
                }
            }

            if (_keyboard.IsKeyPressed(CommandKey.ECM) &&
                !_gameState.IsDocked && _ship.HasECM)
            {
                _combat.ActivateECM(true);
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

            if (_keyboard.IsKeyPressed(CommandKey.Jump) &&
                (!_gameState.IsDocked) && (!_gameState.InWitchspace))
            {
                _space.JumpWarp();
            }

            if (_keyboard.IsKeyPressed(CommandKey.FireMissile) &&
                !_gameState.IsDocked)
            {
                _combat.FireMissile();
            }

            if (_keyboard.IsKeyPressed(CommandKey.Pause))
            {
                _isGamePaused = true;
            }

            if (_keyboard.IsKeyPressed(CommandKey.TargetMissile) &&
                !_gameState.IsDocked)
            {
                _combat.ArmMissile();
            }

            if (_keyboard.IsKeyPressed(CommandKey.UnarmMissile) &&
                !_gameState.IsDocked)
            {
                _combat.UnarmMissile();
            }

            if (_keyboard.IsKeyPressed(CommandKey.IncreaseSpeed) &&
                !_gameState.IsDocked)
            {
                _ship.IncreaseSpeed();
            }

            if (_keyboard.IsKeyPressed(CommandKey.DecreaseSpeed) &&
                !_gameState.IsDocked)
            {
                _ship.DecreaseSpeed();
            }

            if (_keyboard.IsKeyPressed(CommandKey.EnergyBomb) &&
                (!_gameState.IsDocked) && _ship.HasEnergyBomb)
            {
                _gameState.DetonateBomb = true;
                _ship.HasEnergyBomb = false;
            }

            if (_keyboard.IsKeyPressed(CommandKey.EscapeCapsule) &&
                (!_gameState.IsDocked) && _ship.HasEscapeCapsule && (!_gameState.InWitchspace))
            {
                _gameState.SetView(Screen.EscapeCapsule);
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
            _pilot.Reset();
            _ship.Reset();
            _save.GetLastSave();

            _ship.Speed = 1;
            _space.IsHyperspaceReady = false;
            _isGamePaused = false;

            _stars.CreateNewStars();
            _combat.ClearUniverse();
            _space.DockPlayer();

            _gameState.SetView(Screen.IntroOne);
        }
    }
}
