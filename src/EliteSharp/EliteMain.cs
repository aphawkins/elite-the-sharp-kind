// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Diagnostics;
using System.Runtime.CompilerServices;
using EliteSharp.Assets.Fonts;
using EliteSharp.Audio;
using EliteSharp.Config;
using EliteSharp.Conflict;
using EliteSharp.Controls;
using EliteSharp.Graphics;
using EliteSharp.Save;
using EliteSharp.Ships;
using EliteSharp.Trader;
using EliteSharp.Views;

[assembly: CLSCompliant(false)]

// For unit testing
[assembly: InternalsVisibleTo("EliteSharp.Tests")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

// For benchmarking
[assembly: InternalsVisibleTo("EliteSharp.Benchmarks")]

// For test renderering
[assembly: InternalsVisibleTo("EliteSharp.Renderer")]

namespace EliteSharp
{
    public sealed class EliteMain
    {
        private readonly AudioController _audio;
        private readonly Combat _combat;
        private readonly Draw _draw;
        private readonly GameState _gameState;
        private readonly IGraphics _graphics;
        private readonly IKeyboard _keyboard;
        private readonly FrameCounter _lockObj = new();
#if DEBUG
        private readonly long _oneSecondinTicks = TimeSpan.FromSeconds(1).Ticks;
#endif
        private readonly Pilot _pilot;
        private readonly SaveFile _save;
        private readonly Scanner _scanner;
        private readonly PlayerShip _ship;
        private readonly Space _space;
        private readonly Stars _stars;
        private readonly TimeSpan _timeout;
        private readonly Universe _universe;
        private readonly Dictionary<Screen, IView> _views = [];
        private bool _isGamePaused;

        public EliteMain(IGraphics graphics, ISound sound, IKeyboard keyboard)
        {
            _graphics = graphics;
            _audio = new(sound);
            _keyboard = keyboard;
            ConfigFile configFile = new();
            _gameState = new(_keyboard, _views)
            {
                Config = configFile.ReadConfig(),
            };

            _ship = new();
            Trade trade = new(_gameState, _ship);
            PlanetController planet = new(_gameState);
            _draw = new Draw(_gameState, _graphics);
            _universe = new(_draw);
            _stars = new(_gameState, _draw, _ship);
            _pilot = new(_draw, _audio, _universe, _ship);
            _combat = new(_gameState, _audio, _ship, trade, _pilot, _universe, _draw);
            _save = new(_gameState, _ship, trade, planet);
            _space = new(_gameState, _audio, _pilot, _combat, trade, _ship, planet, _stars, _universe, _draw);
            _scanner = new(_gameState, _draw, _universe, _ship, _combat);

            _views.Add(Screen.IntroOne, new Intro1View(_gameState, _audio, keyboard, _ship, _combat, _universe, _draw));
            _views.Add(Screen.IntroTwo, new Intro2View(_gameState, _audio, keyboard, _stars, _ship, _combat, _universe, _draw));
            _views.Add(Screen.GalacticChart, new GalacticChartView(_gameState, _draw, keyboard, planet, _ship));
            _views.Add(Screen.ShortRangeChart, new ShortRangeChartView(_gameState, _draw, keyboard, planet, _ship));
            _views.Add(Screen.PlanetData, new PlanetDataView(_gameState, _draw, planet));
            _views.Add(Screen.MarketPrices, new MarketView(_gameState, _draw, keyboard, trade, planet));
            _views.Add(Screen.CommanderStatus, new CommanderStatusView(_gameState, _draw, _ship, trade, planet, _universe));
            _views.Add(Screen.FrontView, new PilotFrontView(_gameState, keyboard, _stars, _pilot, _ship, _space, _draw));
            _views.Add(Screen.RearView, new PilotRearView(_gameState, keyboard, _stars, _pilot, _ship, _space, _draw));
            _views.Add(Screen.LeftView, new PilotLeftView(_gameState, keyboard, _stars, _pilot, _ship, _space, _draw));
            _views.Add(Screen.RightView, new PilotRightView(_gameState, keyboard, _stars, _pilot, _ship, _space, _draw));
            _views.Add(Screen.Docking, new DockingView(_gameState, _audio, _space, _combat, _universe, _draw));
            _views.Add(Screen.Undocking, new LaunchView(_gameState, _audio, _space, _combat, _universe, _draw));
            _views.Add(Screen.Hyperspace, new HyperspaceView(_gameState, _audio, _draw));
            _views.Add(Screen.Inventory, new InventoryView(_draw, _ship, trade));
            _views.Add(Screen.EquipShip, new EquipmentView(_gameState, _draw, keyboard, _ship, trade, _scanner));
            _views.Add(Screen.Options, new OptionsView(_gameState, _draw, keyboard));
            _views.Add(Screen.LoadCommander, new LoadCommanderView(_gameState, _draw, keyboard, _save));
            _views.Add(Screen.SaveCommander, new SaveCommanderView(_gameState, _draw, keyboard, _save));
            _views.Add(Screen.Quit, new QuitView(_gameState, _draw, keyboard));
            _views.Add(Screen.Settings, new SettingsView(_gameState, _draw, keyboard, configFile));
            _views.Add(Screen.MissionOne, new ConstrictorMissionView(_gameState, _draw, keyboard, _ship, trade, _combat, _universe));
            _views.Add(Screen.MissionTwo, new ThargoidMissionView(_gameState, _draw, keyboard, _ship));
            _views.Add(Screen.EscapeCapsule, new EscapeCapsuleView(_gameState, _audio, _stars, _ship, trade, _universe, _pilot, _draw));
            _views.Add(Screen.GameOver, new GameOverView(_gameState, _audio, _stars, _ship, _combat, _universe, _draw));

            _timeout = TimeSpan.FromMilliseconds(1000 / (_gameState.Config.Fps * 2));
        }

        public void Run()
        {
            long startTicks = Stopwatch.GetTimestamp();
            long intervalTicks = (long)(10000000 / _gameState.Config.Fps); // *1000 = ms; *10000 = ticks

            do
            {
                long nowTicks = Stopwatch.GetTimestamp();

                if (((nowTicks - startTicks) % intervalTicks) == 0)
                {
                    _keyboard.Poll();
                    DrawFrame(nowTicks);
                }
            }
            while (!_gameState.ExitGame && !_keyboard.Close);

            Environment.Exit(0);
        }

#if DEBUG
        private void DrawFps()
        {
            _graphics.DrawTextLeft(new(_draw.Right - 65, _draw.Top + 3), $"FPS: {_lockObj.FramesDrawn.Count}", EliteColors.White);
            _graphics.DrawTextLeft(new(_draw.Right - 65, _draw.Top + 18), $"DROP: {_lockObj.Dropped}", EliteColors.White);

            if (_lockObj.FramesDrawn.Count > 0)
            {
                int i;
                for (i = 0; i < _lockObj.FramesDrawn.Count; i++)
                {
                    if (_lockObj.FramesDrawn[i] > Stopwatch.GetTimestamp() - _oneSecondinTicks)
                    {
                        break;
                    }
                }

                _lockObj.FramesDrawn.RemoveRange(0, i);
            }
        }
#endif

        private void DrawFrame(long ticks)
        {
            bool lockTaken = false;

            try
            {
                Monitor.TryEnter(_lockObj, _timeout, ref lockTaken);
                if (lockTaken)
                {
                    // The critical section.
                    DrawFrameElite();
                    _lockObj.Drawn++;
                    _lockObj.FramesDrawn.Add(ticks);
                }
                else
                {
                    // The lock was not acquired.
                    _lockObj.Dropped++;
                }
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
            _draw.SetFullScreenClipRegion();
            _graphics.Clear();
            _audio.UpdateSound();
            _draw.DrawBorder();
            _draw.SetViewClipRegion();
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

            _gameState.CurrentView!.UpdateUniverse();
            _space.UpdateUniverse();
            _gameState.CurrentView.Draw();
#if DEBUG
            DrawFps();
#endif

            if (!_gameState.IsDocked && !_gameState.IsGameOver)
            {
                _combat.CoolLaser();

                if (_gameState.MessageCount > 0)
                {
                    _graphics.DrawTextCentre(_draw.ScannerTop - 40, _gameState.MessageString, FontType.Small, EliteColors.White);
                }

                if (_space.IsHyperspaceReady)
                {
                    _draw.DrawHyperspaceCountdown(_space.HyperCountdown);
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

            _draw.SetFullScreenClipRegion();

#pragma warning disable CA1031
            try
            {
                _scanner.UpdateConsole();
                _gameState.CurrentView.HandleInput();
                _graphics.ScreenUpdate();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
#pragma warning restore CA1031
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
                !_gameState.IsDocked
                && _ship.HasDockingComputer)
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
                !_gameState.IsDocked
                && _ship.HasECM)
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
                (!_gameState.IsDocked)
                && (!_gameState.InWitchspace))
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
                (!_gameState.IsDocked)
                && _ship.HasEnergyBomb)
            {
                _gameState.DetonateBomb = true;
                _ship.HasEnergyBomb = false;
            }

            if (_keyboard.IsKeyPressed(CommandKey.EscapeCapsule) &&
                (!_gameState.IsDocked)
                && _ship.HasEscapeCapsule
                && (!_gameState.InWitchspace))
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
            _combat.Reset();
            _save.GetLastSave();

            _ship.Speed = 1;
            _space.IsHyperspaceReady = false;
            _isGamePaused = false;

            _stars.CreateNewStars();
            _universe.ClearUniverse();
            _space.DockPlayer();

            _gameState.SetView(Screen.IntroOne);
        }
    }
}
