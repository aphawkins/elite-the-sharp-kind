// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Diagnostics;
using System.Runtime.CompilerServices;
using EliteSharpLib.Audio;
using EliteSharpLib.Conflict;
using EliteSharpLib.Graphics;
using EliteSharpLib.Save;
using EliteSharpLib.Ships;
using EliteSharpLib.Views;
using Useful.Abstraction;
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

public sealed class EliteMain : IGame, IGameApp
{
    // The rate the game logic ticks at, approximately the speed of Elite
    // The New Kind. Render runs at the same rate (see Run()): decoupling it
    // from the tick rate meant most presents just redisplayed an unchanged
    // frame, and since 60/13.5 isn't a whole number the repeat count varied
    // frame to frame, producing judder.
    private const float GameTickRate = 13.5f;

    private readonly uint _colorText;
    private readonly IAbstraction _abstraction;
    private readonly IGraphics _graphics;
    private readonly IKeyboard _keyboard;

    private readonly AudioController _audio;
    private readonly Combat _combat;
    private readonly IEliteDraw _draw;
    private readonly List<long> _framesDrawn = [];
    private readonly Pilot _pilot;
    private readonly SaveFile _save;
    private readonly Scanner _scanner;
    private readonly PlayerShip _ship;
    private readonly Space _space;
    private readonly Stars _stars;
    private readonly Universe _universe;

    internal EliteMain(
        IAbstraction abstraction,
        GameState gameState,
        PlayerShip ship,
        IEliteDraw draw,
        Universe universe,
        Stars stars,
        Pilot pilot,
        Combat combat,
        SaveFile save,
        Space space,
        Scanner scanner,
        AudioController audio)
    {
        ArgumentNullException.ThrowIfNull(abstraction);
        ArgumentNullException.ThrowIfNull(gameState);
        ArgumentNullException.ThrowIfNull(ship);
        ArgumentNullException.ThrowIfNull(draw);
        ArgumentNullException.ThrowIfNull(universe);
        ArgumentNullException.ThrowIfNull(stars);
        ArgumentNullException.ThrowIfNull(pilot);
        ArgumentNullException.ThrowIfNull(combat);
        ArgumentNullException.ThrowIfNull(save);
        ArgumentNullException.ThrowIfNull(space);
        ArgumentNullException.ThrowIfNull(scanner);
        ArgumentNullException.ThrowIfNull(audio);

        _abstraction = abstraction;
        _graphics = abstraction.Graphics;
        _keyboard = abstraction.Keyboard;
        _audio = audio;
        State = gameState;
        _ship = ship;
        _draw = draw;
        _colorText = _draw.Palette["White"];
        _universe = universe;
        _stars = stars;
        _pilot = pilot;
        _combat = combat;
        _save = save;
        _space = space;
        _scanner = scanner;
    }

    public bool IsRunning => !State.ExitGame;

    // Exposed (GameState itself stays internal) for headless test harnesses
    // that need to observe screen/docked/game-over state without a rendered
    // frame.
    internal GameState State { get; }

    // The game composes a new frame only once per tick but presents at
    // Config.Fps, which is usually higher and rarely a whole multiple of
    // GameTickRate. That used to mean judder (an uneven number of presents
    // per tick) and, on Hardware/SDL, flicker (a multi-buffered swap chain
    // going stale between redraws) - both fixed at the rendering layer
    // (SDLGraphics now redraws its persistent frame texture on every
    // present), so this can render at the configured rate directly.
    public void Run() => GameHost.Run(_abstraction, this, GameTickRate, State.Config.Engine.Graphics.Fps);

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

        if (State.IsGamePaused)
        {
            // leave the framebuffer untouched so the pause screen persists
            if (_keyboard.IsPressed(ConsoleKey.R))
            {
                State.IsGamePaused = false;
            }

            return;
        }

        _draw.SetFullScreenClipRegion();
        _graphics.Clear();
        _draw.DrawBorder();
        _draw.SetViewClipRegion();

        if (_ship.Energy < 0)
        {
            State.GameOver();
        }

        if (State.MessageCount > 0)
        {
            State.MessageCount--;
        }

        _ship.LevelOut();

        if (_pilot.IsAutoPilotOn)
        {
            _pilot.AutoDock();
            if ((State.MCount & 127) == 0)
            {
                State.InfoMessage("Docking Computers On");
            }
        }

        State.CurrentView.Update();
        _space.UpdateUniverse();
        State.CurrentView.Draw();
#if DEBUG
        DrawFps();
#endif

        if (!State.IsDocked && !State.IsGameOver)
        {
            UpdateInFlight();
        }

        _draw.SetFullScreenClipRegion();

        _scanner.UpdateConsole();
        State.CurrentView.HandleInput();
    }

    // Present the frame composed by the last update. Runs at GameTickRate,
    // once per tick.
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

    // The part of a tick that only applies while flying: laser cooling,
    // messages, the hyperspace countdown and the MCount-driven housekeeping.
    private void UpdateInFlight()
    {
        _combat.CoolLaser();

        if (State.MessageCount > 0)
        {
            _graphics.DrawTextCentre(_draw.ScannerTop - 40, State.MessageString, nameof(FontType.Small), _colorText);
        }

        if (_space.IsHyperspaceReady)
        {
            _draw.DrawHyperspaceCountdown(_space.HyperCountdown);
            if ((State.MCount & 3) == 0)
            {
                _space.CountdownHyperspace();
            }
        }

        State.MCount--;
        if (State.MCount < 0)
        {
            State.MCount = 255;
        }

        if ((State.MCount & 7) == 0)
        {
            _ship.RegenerateShields();
        }

        if ((State.MCount & 31) == 10)
        {
            if (_ship.IsEnergyLow())
            {
                State.InfoMessage("ENERGY LOW");
                _audio.PlayEffect(nameof(SoundEffect.Beep));
            }

            _space.UpdateAltitude();
        }

        if ((State.MCount & 31) == 20)
        {
            _space.UpdateCabinTemp();
        }

        if ((State.MCount == 0) && (!State.InWitchspace))
        {
            _combat.RandomEncounter();
        }

        _combat.TimeECM();
    }

    private void HandleViewKeys()
    {
        HandleFlightViewKeys();
        HandleChartViewKeys();
        HandleStatusViewKeys();
    }

    // F1 - F4: the cockpit views, which double as the docked screens
    private void HandleFlightViewKeys()
    {
        if (_keyboard.IsPressed(ConsoleKey.F1) &&
            State.CurrentScreen is not Screen.IntroOne and not Screen.IntroTwo)
        {
            if (State.IsDocked)
            {
                State.SetView(Screen.Undocking);
            }
            else
            {
                State.SetView(Screen.FrontView);
            }
        }

        if (_keyboard.IsPressed(ConsoleKey.F2) &&
            !State.IsDocked)
        {
            State.SetView(Screen.RearView);
        }

        if (_keyboard.IsPressed(ConsoleKey.F3) &&
            !State.IsDocked)
        {
            State.SetView(Screen.LeftView);
        }

        if (_keyboard.IsPressed(ConsoleKey.F4))
        {
            if (State.IsDocked)
            {
                State.SetView(Screen.EquipShip);
            }
            else
            {
                State.SetView(Screen.RightView);
            }
        }
    }

    // F5 - F8: the charts and market
    private void HandleChartViewKeys()
    {
        if (_keyboard.IsPressed(ConsoleKey.F5))
        {
            State.SetView(Screen.GalacticChart);
        }

        if (_keyboard.IsPressed(ConsoleKey.F6))
        {
            State.SetView(Screen.ShortRangeChart);
        }

        if (_keyboard.IsPressed(ConsoleKey.F7))
        {
            State.SetView(Screen.PlanetData);
        }

        if (_keyboard.IsPressed(ConsoleKey.F8) && (!State.InWitchspace))
        {
            State.SetView(Screen.MarketPrices);
        }
    }

    // F9 - F11: commander status, inventory and options
    private void HandleStatusViewKeys()
    {
        if (_keyboard.IsPressed(ConsoleKey.F9))
        {
            State.SetView(Screen.CommanderStatus);
        }

        if (_keyboard.IsPressed(ConsoleKey.F10))
        {
            State.SetView(Screen.Inventory);
        }

        if (_keyboard.IsPressed(ConsoleKey.F11))
        {
            State.SetView(Screen.Options);
        }
    }

    /// <summary>
    /// Initialise the game parameters.
    /// </summary>
    private void InitialiseGame()
    {
        if (State.IsInitialised)
        {
            return;
        }

        State.Reset();
        _pilot.Reset();
        _ship.Reset();
        _combat.Reset();
        _save.GetLastSave();

        _ship.Speed = 1;
        _space.IsHyperspaceReady = false;
        State.IsGamePaused = false;

        _stars.CreateNewStars();
        _universe.ClearUniverse();
        _space.DockPlayer();

        State.SetView(Screen.IntroOne);
    }
}
