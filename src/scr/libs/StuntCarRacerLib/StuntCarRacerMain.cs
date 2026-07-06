// 'Stunt Car Racer - The Sharp Kind' - Andy Hawkins 2026.
// 'Stunt Car Racer Remake' - sourceforge.net/projects/stuntcarremake.
// Stunt Car Racer (C) Geoff Crammond / MicroStyle / MicroProse 1989.

using System.Runtime.CompilerServices;
using StuntCarRacerLib.Cars;
using StuntCarRacerLib.Rendering;
using StuntCarRacerLib.Screens;
using StuntCarRacerLib.Tracks;
using Useful;
using Useful.Abstraction;
using Useful.Audio;
using Useful.Controls;
using Useful.Graphics;

[assembly: CLSCompliant(false)]
[assembly: InternalsVisibleTo("StuntCarRacerLib.Tests")]

namespace StuntCarRacerLib;

public sealed class StuntCarRacerMain : IGame
{
    // The original remake ticks OnFrameMove at 50Hz: input and the engine
    // sound run at the full tick rate, while the car physics only steps
    // every FrameGap ticks (DEFAULT_FRAME_GAP = 4, i.e. 12.5Hz; the Amiga
    // original used MIN.FRAMES = 6).
    internal const int TickRate = 50;

    internal const string SmallFont = "Small";
    internal const string LargeFont = "Large";

    private const int DefaultFrameGap = 4;

    private readonly IAbstraction _abstraction;
    private readonly BackdropRenderer _backdrop;
    private readonly HudRenderer _hud;
    private readonly List<WorldPolygon> _worldPolygons = [];
    private readonly AudioController _audio;

    private TrackRenderer _renderer;
    private OpponentRenderer _opponentRenderer;

    private bool _sceneryKeyDown;
    private int _frameCount;

    public StuntCarRacerMain(IAbstraction abstraction)
        : this(abstraction, TrackId.LittleRamp)
    {
    }

    public StuntCarRacerMain(IAbstraction abstraction, TrackId trackId)
    {
        Guard.ArgumentNull(abstraction);

        _abstraction = abstraction;
        Graphics = abstraction.Graphics;
        Keyboard = abstraction.Keyboard;
        Sound = abstraction.Sound;
        _backdrop = new(Graphics);
        _hud = new(Graphics);

        // Effect cooldowns in physics frames (12.5Hz), sized to the sample
        // lengths so a repeated trigger doesn't stack in the mixer. OffRoad
        // and Wreck share a cooldown as they shared a buffer in the
        // original.
        SfxSample offRoadOrWreck = new(10);
        _audio = new(
            Sound,
            new Dictionary<string, SfxSample>
            {
                { "Grounded", new(4) },
                { "Creak", new(6) },
                { "Smash", new(11) },
                { "OffRoad", offRoadOrWreck },
                { "Wreck", offRoadOrWreck },
                { "HitCar", new(9) },
            });

        LoadTrack(trackId);

        Screens = new(Keyboard);
        Screens.Add(GameMode.TrackMenu, new TrackMenuScreen(this));
        Screens.Add(GameMode.TrackPreview, new TrackPreviewScreen(this));
        Screens.Add(GameMode.GameInProgress, new RaceScreen(this));
        Screens.Add(GameMode.GameOver, new GameOverScreen(this));
        Screens.Set(GameMode.TrackMenu);
    }

    public bool IsRunning { get; private set; } = true;

    // The game mode state machine: TrackMenu -> TrackPreview ->
    // GameInProgress -> GameOver.
    internal ScreenManager<GameMode, IGameScreen> Screens { get; }

    internal IKeyboard Keyboard { get; }

    internal ISound Sound { get; }

    internal IGraphics Graphics { get; }

    internal SceneCamera Camera { get; } = new();

    internal Track Track { get; private set; }

    internal CarPhysics Car { get; private set; }

    internal OpponentPhysics Opponent { get; private set; }

    internal DrawBridge Bridge { get; private set; }

    // How often the physics steps, per plan pass 3 step 1: made settable so
    // the frame gap can be tuned as the original's -/+ keys did.
    internal int FrameGap { get; set; } = DefaultFrameGap;

    // Whether the last tick stepped the physics (the original bFrameMoved).
    internal bool FrameMoved { get; set; }

    // Race timing shared by the race and game-over screens: the tick count
    // since the race started, and when and how it finished.
    internal int RaceTick { get; set; }

    internal bool RaceFinished { get; set; }

    internal bool RaceWon { get; set; }

    internal int RaceFinishedTick { get; set; }

    public void Run() => GameHost.Run(_abstraction, this, TickRate, TickRate);

    // One 50Hz tick (the original OnFrameMove).
    public void Update()
    {
        FrameMoved = false;

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
                _backdrop.NextSceneryType();
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

    // The original's frameCount countdown: the physics only steps every
    // FrameGap ticks.
    internal bool PhysicsDue()
    {
        if (_frameCount > 0)
        {
            _frameCount--;
        }

        if (_frameCount == 0)
        {
            _frameCount = FrameGap;
            return true;
        }

        return false;
    }

    [System.Diagnostics.CodeAnalysis.MemberNotNull(
        nameof(Track),
        nameof(Car),
        nameof(Opponent),
        nameof(Bridge),
        nameof(_renderer),
        nameof(_opponentRenderer))]
    internal void LoadTrack(TrackId trackId)
    {
        Track = Track.Load(trackId);
        Car = new(Track);
        Opponent = new(Track, Car);
        Bridge = new(Track);
        _renderer = new(Track, Graphics);
        _opponentRenderer = new(Opponent);
    }

    // The world common to every screen: backdrop, track and (outside the
    // track menu) the opponent car. Road lines draw around the player's
    // position, as the original did in every mode.
    internal void DrawWorld(bool showOpponent)
    {
        Graphics.Clear();
        _backdrop.Draw(Camera);

        _worldPolygons.Clear();
        if (showOpponent)
        {
            _opponentRenderer.AppendWorldPolygons(_worldPolygons);
        }

        _renderer.Draw(Camera, _worldPolygons, Car.CurrentPiece, Car.CurrentSegment);
    }

    // Play the effect triggers from the physics, throttled by the shared
    // AudioController cooldowns. Runs once per physics frame.
    internal void UpdateSounds()
    {
        _audio.UpdateSound();

        if (Car.GroundedSoundTriggered)
        {
            _audio.PlayEffect("Grounded");
        }

        if (Car.CreakSoundTriggered)
        {
            _audio.PlayEffect("Creak");
        }

        if (Car.SmashSoundTriggered)
        {
            _audio.PlayEffect("Smash");
        }

        if (Car.OffRoadSoundTriggered)
        {
            _audio.PlayEffect("OffRoad");
        }
        else if (Car.WreckSoundTriggered)
        {
            _audio.PlayEffect("Wreck");
        }

        if (Opponent.HitCarSoundTriggered)
        {
            _audio.PlayEffect("HitCar");
        }
    }

    // Engine sound sample and pitch, from the original FramesWheelsEngine.
    // The samples were recorded at 11025Hz; the original played them at
    // AMIGA_PAL_HZ / period.
    internal void UpdateEngineSound()
    {
        const int amigaPalHz = 3546895;
        const int sampleRate = 11025;

        int r = Car.EngineRevs + 378;
        int period = 4800000 / r;
        int index = 6;

        if (period >= 0x3fff)
        {
            period = 0x3ffe;
        }

        period |= Car.EngineFluctuation;
        if (period < 124)
        {
            period = 124; // lowest possible period
        }

        // calculate the sound index that will give period < 256
        while (period >= 256)
        {
            period >>= 1;
            --index;

            if (index < 0)
            {
                index = 0;
            }
        }

        double frequency = (double)amigaPalHz / period;
        string sample = index == 0 ? "TickOver" : $"EnginePitch{index + 1}";
        Sound.PlayLoop(sample, frequency / sampleRate);
    }

    // The in-game display: the Amiga dashboard (chassis beam with damage
    // crack, speed bar, lap/boost/distance read-outs) plus the remake's
    // text overlays (opponent name at race start, race result).
    internal void DrawHud(bool gameOver)
    {
        uint white = ScrPalette.Colour(Track.ScrBaseColour + 15);
        float height = Graphics.ScreenHeight;

        _hud.Draw(Car.LapNumber, Car.BoostReserve, Car.NewDamage, Car.PlayerZSpeed, Opponent.DistanceToPlayer());

        // output the opponent's name for four seconds at race start
        if (RaceTick < 4 * TickRate)
        {
            Graphics.DrawTextCentre(height - 300, $"Opponent: {Opponent.Name}", SmallFont, white);
        }

        if (!RaceFinished)
        {
            return;
        }

        if (gameOver)
        {
            Graphics.DrawTextCentre(height - 300, "GAME OVER: Press 'M' for track menu", LargeFont, white);
        }
        else
        {
            // the result text flashes white/black, changing every half second
            int flash = (RaceTick - RaceFinishedTick) % TickRate;
            uint colour = flash < TickRate / 2 ? white : ScrPalette.Colour(Track.ScrBaseColour);
            Graphics.DrawTextCentre(height - 300, RaceWon ? "RACE WON" : "RACE LOST", LargeFont, colour);
        }
    }
}
