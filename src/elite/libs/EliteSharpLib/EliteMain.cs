// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Diagnostics;
using System.Runtime.CompilerServices;
using EliteSharpLib.Audio;
using EliteSharpLib.Config;
using EliteSharpLib.Conflict;
using EliteSharpLib.Graphics;
using EliteSharpLib.Save;
using EliteSharpLib.Ships;
using EliteSharpLib.Trader;
using EliteSharpLib.Views;
using Useful;
using Useful.Abstraction;
using Useful.Assets;
using Useful.Audio;
using Useful.Controls;
using Useful.Graphics;

[assembly: CLSCompliant(false)]

// For unit testing
[assembly: InternalsVisibleTo("EliteSharpLib.Tests")]
[assembly: InternalsVisibleTo("EliteSharpLib.Fakes")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

// For benchmarking
[assembly: InternalsVisibleTo("EliteSharpLib.Benchmarks")]

// For test renderering
[assembly: InternalsVisibleTo("EliteSharpLib.Renderer")]

namespace EliteSharpLib;

public sealed class EliteMain : IGame
{
    // The rate the game logic ticks at, approximately the speed of Elite
    // The New Kind. The render rate (Config.Fps) is independent.
    private const float GameTickRate = 13.5f;

    private readonly uint _colorText;
    private readonly IAbstraction _abstraction;
    private readonly IGraphics _graphics;
    private readonly IKeyboard _keyboard;

    private readonly AudioController _audio;
    private readonly Combat _combat;
    private readonly EliteDraw _draw;
    private readonly GameState _gameState;
    private readonly List<long> _framesDrawn = [];
    private readonly Pilot _pilot;
    private readonly SaveFile _save;
    private readonly Scanner _scanner;
    private readonly PlayerShip _ship;
    private readonly Space _space;
    private readonly Stars _stars;
    private readonly Universe _universe;

    public EliteMain(IAbstraction abstraction)
    {
        Guard.ArgumentNull(abstraction);

        AssetLocator assetLocator = AssetLocator.Create();

        _abstraction = abstraction;
        _graphics = abstraction.Graphics;
        ISound sound = abstraction.Sound;
        _keyboard = abstraction.Keyboard;

        // TODO: improve this
        Dictionary<string, SfxSample> sfx = new()
        {
            { nameof(SoundEffect.Launch), new(32) },
            { nameof(SoundEffect.Crash), new(7) },
            { nameof(SoundEffect.Dock), new(36) },
            { nameof(SoundEffect.Gameover), new(24) },
            { nameof(SoundEffect.Pulse), new(4) },
            { nameof(SoundEffect.HitEnemy), new(4) },
            { nameof(SoundEffect.Explode), new(23) },
            { nameof(SoundEffect.Ecm), new(23) },
            { nameof(SoundEffect.Missile), new(25) },
            { nameof(SoundEffect.Hyperspace), new(37) },
            { nameof(SoundEffect.IncomingFire1), new(4) },
            { nameof(SoundEffect.IncomingFire2), new(5) },
            { nameof(SoundEffect.Beep), new(2) },
            { nameof(SoundEffect.Boop), new(7) },
        };
        _audio = new(sound, sfx);
        ConfigFile configFile = new();
        ScreenManager<Screen, IView> views = new(_keyboard);
        _gameState = new(views)
        {
            Config = configFile.ReadConfig(),
        };

        _ship = new();
        Trade trade = new(_gameState, _ship);
        PlanetController planet = new(_gameState);
        _draw = new(_gameState, _graphics, assetLocator);
        _colorText = _draw.Palette["White"];
        IShipFactory shipFactory = ShipFactory.Create(assetLocator, _draw);
        _universe = new(shipFactory);
        _stars = new(_gameState, _draw, _ship);
        _pilot = new(_draw, _audio, _universe, _ship);
        _combat = new(_gameState, _audio, _ship, trade, _pilot, _universe, _draw, shipFactory);
        _save = new(_gameState, _ship, trade, planet);
        _space = new(_gameState, _audio, _pilot, _combat, trade, _ship, planet, _stars, _universe, _draw);
        _scanner = new(_gameState, _draw, _universe, _ship, _combat);

        views.Add(Screen.IntroOne, new Intro1View(_gameState, _audio, _keyboard, _ship, _combat, _universe, _draw, shipFactory));
        views.Add(Screen.IntroTwo, new Intro2View(_gameState, _audio, _keyboard, _stars, _ship, _combat, _universe, _draw, shipFactory));
        views.Add(Screen.GalacticChart, new GalacticChartView(_gameState, _draw, _keyboard, planet, _ship));
        views.Add(Screen.ShortRangeChart, new ShortRangeChartView(_gameState, _draw, _keyboard, planet, _ship));
        views.Add(Screen.PlanetData, new PlanetDataView(_gameState, _draw, planet));
        views.Add(Screen.MarketPrices, new MarketView(_gameState, _draw, _keyboard, trade, planet));
        views.Add(Screen.CommanderStatus, new CommanderStatusView(_gameState, _draw, _ship, trade, planet, _universe));
        views.Add(Screen.FrontView, new PilotFrontView(_gameState, _keyboard, _stars, _pilot, _ship, _space, _draw, _combat));
        views.Add(Screen.RearView, new PilotRearView(_gameState, _keyboard, _stars, _pilot, _ship, _space, _draw, _combat));
        views.Add(Screen.LeftView, new PilotLeftView(_gameState, _keyboard, _stars, _pilot, _ship, _space, _draw, _combat));
        views.Add(Screen.RightView, new PilotRightView(_gameState, _keyboard, _stars, _pilot, _ship, _space, _draw, _combat));
        views.Add(Screen.Docking, new DockingView(_gameState, _audio, _space, _combat, _universe, _draw));
        views.Add(Screen.Undocking, new LaunchView(_gameState, _audio, _space, _combat, _universe, _draw));
        views.Add(Screen.Hyperspace, new HyperspaceView(_gameState, _audio, _draw));
        views.Add(Screen.Inventory, new InventoryView(_draw, _ship, trade));
        views.Add(Screen.EquipShip, new EquipmentView(_gameState, _draw, _keyboard, _ship, trade, _scanner));
        views.Add(Screen.Options, new OptionsView(_gameState, _draw, _keyboard));
        views.Add(Screen.LoadCommander, new LoadCommanderView(_gameState, _draw, _keyboard, _save));
        views.Add(Screen.SaveCommander, new SaveCommanderView(_gameState, _draw, _keyboard, _save));
        views.Add(Screen.Quit, new QuitView(_gameState, _draw, _keyboard));
        views.Add(Screen.Settings, new SettingsView(_gameState, _draw, _keyboard, configFile));
        views.Add(
            Screen.MissionOne,
            new ConstrictorMissionView(_gameState, _draw, _keyboard, _ship, trade, _combat, _universe, shipFactory));
        views.Add(Screen.MissionTwo, new ThargoidMissionView(_gameState, _draw, _keyboard, _ship));
        views.Add(
            Screen.EscapeCapsule,
            new EscapeCapsuleView(_gameState, _audio, _stars, _ship, trade, _universe, _pilot, _draw, shipFactory));
        views.Add(Screen.GameOver, new GameOverView(_gameState, _audio, _stars, _ship, _combat, _universe, _draw, shipFactory));
    }

    public bool IsRunning => !_gameState.ExitGame;

    public void Run()
    {
        GameHost.Run(_abstraction, this, GameTickRate, _gameState.Config.Fps);

        Environment.Exit(0);
    }

    // One fixed-rate game tick. Elite's update draws the universe as it
    // moves it (as The New Kind did), so this composes the whole frame into
    // the framebuffer and Draw only presents it.
    public void Update()
    {
        InitialiseGame();
        _audio.UpdateSound();
        _ship.IsRolling = false;
        _ship.IsClimbing = false;
        HandleViewKeys();

        if (_gameState.IsGamePaused)
        {
            // leave the framebuffer untouched so the pause screen persists
            if (_keyboard.IsPressed(ConsoleKey.R))
            {
                _gameState.IsGamePaused = false;
            }

            return;
        }

        _draw.SetFullScreenClipRegion();
        _graphics.Clear();
        _draw.DrawBorder();
        _draw.SetViewClipRegion();

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

        _gameState.CurrentView!.Update();
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
                _graphics.DrawTextCentre(_draw.ScannerTop - 40, _gameState.MessageString, nameof(FontType.Small), _colorText);
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
                    _audio.PlayEffect(nameof(SoundEffect.Beep));
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
            _gameState.CurrentView!.HandleInput();
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.ToString());
        }
#pragma warning restore CA1031
    }

    // Present the frame composed by the last update. Runs at the render
    // rate (up to Config.Fps), independently of the game tick rate.
    public void Draw()
    {
        // keep only the presents from the last second, for the FPS display
        int stale = 0;
        long oneSecondAgo = Stopwatch.GetTimestamp() - Stopwatch.Frequency;
        while (stale < _framesDrawn.Count && _framesDrawn[stale] <= oneSecondAgo)
        {
            stale++;
        }

        _framesDrawn.RemoveRange(0, stale);
        _framesDrawn.Add(Stopwatch.GetTimestamp());

        _graphics.ScreenUpdate();
    }

#if DEBUG

    private void DrawFps()
        => _graphics.DrawTextLeft(
            new(_draw.Right - 65, _draw.Top + 3),
            $"FPS: {_framesDrawn.Count}",
            nameof(FontType.Small),
            _colorText);

#endif

    private void HandleViewKeys()
    {
        if (_keyboard.IsPressed(ConsoleKey.F1) &&
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

        if (_keyboard.IsPressed(ConsoleKey.F2) &&
            !_gameState.IsDocked)
        {
            _gameState.SetView(Screen.RearView);
        }

        if (_keyboard.IsPressed(ConsoleKey.F3) &&
            !_gameState.IsDocked)
        {
            _gameState.SetView(Screen.LeftView);
        }

        if (_keyboard.IsPressed(ConsoleKey.F4))
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

        if (_keyboard.IsPressed(ConsoleKey.F5))
        {
            _gameState.SetView(Screen.GalacticChart);
        }

        if (_keyboard.IsPressed(ConsoleKey.F6))
        {
            _gameState.SetView(Screen.ShortRangeChart);
        }

        if (_keyboard.IsPressed(ConsoleKey.F7))
        {
            _gameState.SetView(Screen.PlanetData);
        }

        if (_keyboard.IsPressed(ConsoleKey.F8) && (!_gameState.InWitchspace))
        {
            _gameState.SetView(Screen.MarketPrices);
        }

        if (_keyboard.IsPressed(ConsoleKey.F9))
        {
            _gameState.SetView(Screen.CommanderStatus);
        }

        if (_keyboard.IsPressed(ConsoleKey.F10))
        {
            _gameState.SetView(Screen.Inventory);
        }

        if (_keyboard.IsPressed(ConsoleKey.F11))
        {
            _gameState.SetView(Screen.Options);
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
        _gameState.IsGamePaused = false;

        _stars.CreateNewStars();
        _universe.ClearUniverse();
        _space.DockPlayer();

        _gameState.SetView(Screen.IntroOne);
    }
}
