// 'Stunt Car Racer - The Sharp Kind' - Andy Hawkins 2026.
// 'Stunt Car Racer Remake' - sourceforge.net/projects/stuntcarremake.
// Stunt Car Racer (C) Geoff Crammond / MicroStyle / MicroProse 1989.

using System.Runtime.CompilerServices;
using StuntCarRacerSharpLib.Rendering;
using StuntCarRacerSharpLib.Screens;
using StuntCarRacerSharpLib.Tracks;
using Useful;
using Useful.Abstraction;
using Useful.Audio;
using Useful.Controls;
using Useful.Graphics;

[assembly: CLSCompliant(false)]
[assembly: InternalsVisibleTo("StuntCarRacerSharpLib.Tests")]

namespace StuntCarRacerSharpLib;

public sealed class StuntCarRacerMain : IGame
{
    // The original remake ticks OnFrameMove at 50Hz: input and the engine
    // sound run at the full tick rate, while the car physics only steps
    // every FrameGap ticks (DEFAULT_FRAME_GAP = 4, i.e. 12.5Hz; the Amiga
    // original used MIN.FRAMES = 6).
    internal const int TickRate = 50;

    internal const string SmallFont = "Small";
    internal const string LargeFont = "Large";

    private readonly IAbstraction _abstraction;

    private bool _sceneryKeyDown;

    public StuntCarRacerMain(IAbstraction abstraction)
        : this(abstraction, TrackId.LittleRamp, new(), new RandomSource(new Random()))
    {
    }

    public StuntCarRacerMain(IAbstraction abstraction, TrackId trackId)
        : this(abstraction, trackId, new(), new RandomSource(new Random()))
    {
    }

    public StuntCarRacerMain(IAbstraction abstraction, AudioOptions audioOptions)
        : this(abstraction, TrackId.LittleRamp, audioOptions, new RandomSource(new Random()))
    {
    }

    public StuntCarRacerMain(IAbstraction abstraction, AudioOptions audioOptions, IRandomSource randomSource)
        : this(abstraction, TrackId.LittleRamp, audioOptions, randomSource)
    {
    }

    private StuntCarRacerMain(IAbstraction abstraction, TrackId trackId, AudioOptions audioOptions, IRandomSource randomSource)
    {
        Guard.ArgumentNull(abstraction);
        Guard.ArgumentNull(audioOptions);
        Guard.ArgumentNull(randomSource);

        _abstraction = abstraction;
        Graphics = abstraction.Graphics;
        Keyboard = abstraction.Keyboard;
        Sound = abstraction.Sound;
        Palette = new();

        // Effect cooldowns in physics frames (12.5Hz), sized to the sample
        // lengths so a repeated trigger doesn't stack in the mixer. OffRoad
        // and Wreck share a cooldown as they shared a buffer in the
        // original. Pan (and HitCar's fixed volume) mirror DSSetMode
        // (`StuntCarRacer.cpp:166-230`): engine and Smash on the left,
        // everything else on the right; Grounded/Creak/OffRoad/Wreck's
        // volume/pitch are set per-play instead (see CarPhysics).
        SfxSample offRoadOrWreck = new(10, pan: 1f);
        AudioController audio = new(
            Sound,
            new Dictionary<string, SfxSample>
            {
                { "Grounded", new(4, pan: 1f) },
                { "Creak", new(6, pan: 1f) },
                { "Smash", new(11, pan: -1f) },
                { "OffRoad", offRoadOrWreck },
                { "Wreck", offRoadOrWreck },
                { "HitCar", new(9, volume: 56f / 64f, pan: 1f) },
            },
            audioOptions);

        Race = new(Graphics, Palette, Sound, audio, trackId, randomSource);

        Screens = new(Keyboard);
        Screens.Add(GameMode.TrackMenu, new TrackMenuScreen(Race, Keyboard, Screens, Graphics, Palette));
        Screens.Add(GameMode.TrackPreview, new TrackPreviewScreen(Race, Keyboard, Screens, Graphics, Palette));
        Screens.Add(GameMode.GameInProgress, new RaceScreen(Race, Keyboard, Screens));
        Screens.Add(GameMode.GameOver, new GameOverScreen(Race, Keyboard, Sound, Screens));
        Screens.Set(GameMode.TrackMenu);
    }

    public bool IsRunning { get; private set; } = true;

    // The game mode state machine: TrackMenu -> TrackPreview ->
    // GameInProgress -> GameOver.
    internal ScreenManager<GameMode, IGameScreen> Screens { get; }

    internal IKeyboard Keyboard { get; }

    internal ISound Sound { get; }

    internal IGraphics Graphics { get; }

    internal ScrPalette Palette { get; }

    internal Race Race { get; }

    public void Run() => GameHost.Run(_abstraction, this, TickRate, TickRate);

    // One 50Hz tick (the original OnFrameMove).
    public void Update()
    {
        Race.FrameMoved = false;

        if (Keyboard.IsPressed(ConsoleKey.Escape))
        {
            IsRunning = false;
            return;
        }

        // N cycles the scenery type, as the original menu option
        if (Keyboard.IsPressed(ConsoleKey.N))
        {
            if (!_sceneryKeyDown)
            {
                Race.NextScenery();
                _sceneryKeyDown = true;
            }
        }
        else
        {
            _sceneryKeyDown = false;
        }

        Screens.Current!.Update();
    }

    public void Draw()
    {
        Screens.Current!.Draw();
        Graphics.ScreenUpdate();
    }
}
