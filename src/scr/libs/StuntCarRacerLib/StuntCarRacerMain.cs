// 'Stunt Car Racer - The Sharp Kind' - Andy Hawkins 2026.
// 'Stunt Car Racer Remake' - sourceforge.net/projects/stuntcarremake.
// Stunt Car Racer (C) Geoff Crammond / MicroStyle / MicroProse 1989.

using System.Globalization;
using System.Runtime.CompilerServices;
using StuntCarRacerLib.Cars;
using StuntCarRacerLib.Rendering;
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
    private const int TickRate = 50;
    private const int DefaultFrameGap = 4;

    private const string SmallFont = "Small";
    private const string LargeFont = "Large";

    private readonly IAbstraction _abstraction;
    private readonly IGraphics _graphics;
    private readonly IKeyboard _keyboard;
    private readonly ISound _sound;

    private readonly SceneCamera _camera = new();
    private readonly BackdropRenderer _backdrop;
    private readonly List<WorldPolygon> _worldPolygons = [];
    private readonly int[] _soundThrottles = new int[5];

    private Track _track;
    private CarPhysics _car;
    private OpponentPhysics _opponent;
    private DrawBridge _drawBridge;
    private TrackRenderer _renderer;
    private OpponentRenderer _opponentRenderer;

    private GameMode _mode = GameMode.TrackMenu;
    private int _menuOrbitAngle;
    private bool _sceneryKeyDown;
    private int _frameCount;
    private int _raceTick;
    private bool _raceFinished;
    private bool _raceWon;
    private int _raceFinishedTick;

    public StuntCarRacerMain(IAbstraction abstraction)
        : this(abstraction, TrackId.LittleRamp)
    {
    }

    public StuntCarRacerMain(IAbstraction abstraction, TrackId trackId)
    {
        Guard.ArgumentNull(abstraction);

        _abstraction = abstraction;
        _graphics = abstraction.Graphics;
        _keyboard = abstraction.Keyboard;
        _sound = abstraction.Sound;
        _backdrop = new(_graphics);

        LoadTrack(trackId);
    }

    private enum GameMode
    {
        TrackMenu = 0,
        TrackPreview = 1,
        GameInProgress = 2,
        GameOver = 3,
    }

    public bool IsRunning { get; private set; } = true;

    // How often the physics steps, per plan pass 3 step 1: made settable so
    // the frame gap can be tuned as the original's -/+ keys did.
    internal int FrameGap { get; set; } = DefaultFrameGap;

    // Whether the last tick stepped the physics (the original bFrameMoved).
    internal bool FrameMoved { get; private set; }

    public void Run() => GameHost.Run(_abstraction, this, TickRate, TickRate);

    // One 50Hz tick (the original OnFrameMove).
    public void Update()
    {
        FrameMoved = false;

        if (_keyboard.IsPressed(ConsoleKey.Escape))
        {
            IsRunning = false;
            return;
        }

        // N cycles the scenery type, as the original menu option
        if (_keyboard.IsPressed(ConsoleKey.N))
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

        switch (_mode)
        {
            case GameMode.TrackMenu:
                // the track menu is not throttled by the frame gap
                UpdateTrackMenu();
                break;

            case GameMode.TrackPreview:
                if (PhysicsDue())
                {
                    FrameMoved = true;
                    UpdateTrackPreview();
                }

                break;

            case GameMode.GameInProgress:
                UpdateRaceTick();
                if (_mode == GameMode.GameInProgress && PhysicsDue())
                {
                    FrameMoved = true;
                    UpdateGame();
                }

                break;

            case GameMode.GameOver:
                // the original freezes the action once the game is over;
                // M returns to the track menu
                if (_keyboard.IsPressed(ConsoleKey.M))
                {
                    _mode = GameMode.TrackMenu;
                }

                break;
        }
    }

    public void Draw()
    {
        _graphics.Clear();
        _backdrop.Draw(_camera);

        _worldPolygons.Clear();
        if (_mode != GameMode.TrackMenu)
        {
            // the original only shows the opponent outside the track menu
            _opponentRenderer.AppendWorldPolygons(_worldPolygons);
        }

        _renderer.Draw(_camera, _worldPolygons);

        switch (_mode)
        {
            case GameMode.TrackMenu:
                DrawTrackMenuText();
                break;

            case GameMode.TrackPreview:
                DrawTrackPreviewText();
                break;

            case GameMode.GameInProgress:
            case GameMode.GameOver:
                DrawHud();
                break;
        }

        _graphics.ScreenUpdate();
    }

    // The original's frameCount countdown: the physics only steps every
    // FrameGap ticks.
    private bool PhysicsDue()
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

    // The full-rate part of the game mode (the original FramesWheelsEngine
    // call plus the race-finished timing, which the original drove from the
    // wall clock).
    private void UpdateRaceTick()
    {
        _raceTick++;
        _car.ApplyEngineRevs();
        UpdateEngineSound();

        // show the race result for six seconds, then it is game over
        if (_raceFinished && _raceTick - _raceFinishedTick > 6 * TickRate)
        {
            _mode = GameMode.GameOver;
            _sound.StopLoop();
        }
    }

    // The camera circles the currently selected track (original CalcTrackMenuViewpoint).
    private void UpdateTrackMenu()
    {
        _menuOrbitAngle = (_menuOrbitAngle + 128) & (Track.MaxAngle - 1);

        const long centre = (long)Track.TrackCubes * Track.CubeSize / 2;
        const long radius = (Track.TrackCubes - 2) * (long)Track.CubeSize / AmigaTrig.Precision;

        long viewX = centre + (AmigaTrig.Sin(_menuOrbitAngle) * radius);
        const long viewY = -3L * Track.CubeSize;
        long viewZ = centre + (AmigaTrig.Cos(_menuOrbitAngle) * radius);

        _camera.LookAt(viewX, viewY, viewZ, centre, 0, centre);

        // track selection
        for (ConsoleKey key = ConsoleKey.D1; key <= ConsoleKey.D8; key++)
        {
            if (_keyboard.IsPressed(key) && _track.Id != (TrackId)(key - ConsoleKey.D1))
            {
                LoadTrack((TrackId)(key - ConsoleKey.D1));
            }
        }

        if (_keyboard.IsPressed(ConsoleKey.S))
        {
            // reset the cars so the opponent is shown during the preview
            _car.Reset();
            _opponent.StartRace();
            _mode = GameMode.TrackPreview;
        }
    }

    // The opponent drives the selected track, watched from the middle of the
    // world (original CalcTrackPreviewViewpoint).
    private void UpdateTrackPreview()
    {
        _opponent.Update();
        _drawBridge.Move(_car.CurrentPiece, _opponent.CurrentPiece, _opponent);

        const long centre = (long)Track.TrackCubes * Track.CubeSize / 2;

        // the Draw Bridge requires a higher viewpoint
        long viewY = _track.Id == TrackId.DrawBridge
            ? _opponent.Y - (5L * Track.CubeSize / 2)
            : _opponent.Y - ((long)Track.CubeSize / 2);

        long viewX = centre + ((_opponent.X - centre) / 2);
        long viewZ = centre + ((_opponent.Z - centre) / 2);

        _camera.LookAt(viewX, viewY, viewZ, _opponent.X, _opponent.Y, _opponent.Z);

        if (_keyboard.IsPressed(ConsoleKey.S))
        {
            StartRace();
            _mode = GameMode.GameInProgress;
        }
        else if (_keyboard.IsPressed(ConsoleKey.M))
        {
            _mode = GameMode.TrackMenu;
        }
    }

    // One physics frame of the race (every FrameGap ticks).
    private void UpdateGame()
    {
        _car.Update(ReadInput());
        _opponent.Update();
        _drawBridge.Move(_car.CurrentPiece, _opponent.CurrentPiece, _opponent);
        _car.UpdateLapData();
        _opponent.UpdateLapData();
        _car.UpdateDamage();
        _camera.FollowCar(_car);

        // the race finishes when either car completes the final lap
        if (!_raceFinished && (_car.RaceFinished || _opponent.LapNumber >= 4))
        {
            _raceFinished = true;
            _raceWon = _opponent.CalculateIfWinning() < 0;
            _raceFinishedTick = _raceTick;
        }

        UpdateSounds();
    }

    // Play the effect triggers from the physics (throttled so repeated
    // triggers don't stack in the mixer). Throttle counts are physics
    // frames (12.5Hz).
    private void UpdateSounds()
    {
        for (int i = 0; i < _soundThrottles.Length; i++)
        {
            if (_soundThrottles[i] > 0)
            {
                _soundThrottles[i]--;
            }
        }

        PlayThrottled(0, 4, _car.GroundedSoundTriggered, "Grounded");
        PlayThrottled(1, 6, _car.CreakSoundTriggered, "Creak");
        PlayThrottled(2, 11, _car.SmashSoundTriggered, "Smash");
        PlayThrottled(3, 10, _car.OffRoadSoundTriggered || _car.WreckSoundTriggered, _car.OffRoadSoundTriggered ? "OffRoad" : "Wreck");
        PlayThrottled(4, 9, _opponent.HitCarSoundTriggered, "HitCar");
    }

    private void PlayThrottled(int slot, int frames, bool triggered, string sfxType)
    {
        if (triggered && _soundThrottles[slot] == 0)
        {
            _sound.Play(sfxType);
            _soundThrottles[slot] = frames;
        }
    }

    // Engine sound sample and pitch, from the original FramesWheelsEngine.
    // The samples were recorded at 11025Hz; the original played them at
    // AMIGA_PAL_HZ / period.
    private void UpdateEngineSound()
    {
        const int amigaPalHz = 3546895;
        const int sampleRate = 11025;

        int r = _car.EngineRevs + 378;
        int period = 4800000 / r;
        int index = 6;

        if (period >= 0x3fff)
        {
            period = 0x3ffe;
        }

        period |= _car.EngineFluctuation;
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
        _sound.PlayLoop(sample, frequency / sampleRate);
    }

    [System.Diagnostics.CodeAnalysis.MemberNotNull(
        nameof(_track),
        nameof(_car),
        nameof(_opponent),
        nameof(_drawBridge),
        nameof(_renderer),
        nameof(_opponentRenderer))]
    private void LoadTrack(TrackId trackId)
    {
        _track = Track.Load(trackId);
        _car = new(_track);
        _opponent = new(_track, _car);
        _drawBridge = new(_track);
        _renderer = new(_track, _graphics);
        _opponentRenderer = new(_opponent);
    }

    private void StartRace()
    {
        _car.StartRace();
        _car.BoostReserve = _track.StandardBoost;
        _opponent.StartRace();
        _drawBridge.Reset(_opponent);

        _raceTick = 0;
        _raceFinished = false;
        _raceWon = false;
        _raceFinishedTick = 0;
    }

    // Original keyboard controls: S = left, D = right,
    // RETURN = accelerate + boost, SPACE = brake/reverse + boost,
    // HASH = brake/reverse (mapped to B here).
    private CarInput ReadInput()
    {
        CarInput input = CarInput.None;

        if (_keyboard.IsPressed(ConsoleKey.S))
        {
            input |= CarInput.Left;
        }

        if (_keyboard.IsPressed(ConsoleKey.D))
        {
            input |= CarInput.Right;
        }

        if (_keyboard.IsPressed(ConsoleKey.Enter))
        {
            input |= CarInput.AccelBoost;
        }

        if (_keyboard.IsPressed(ConsoleKey.Spacebar))
        {
            input |= CarInput.BrakeBoost;
        }

        if (_keyboard.IsPressed(ConsoleKey.B))
        {
            input |= CarInput.Hash;
        }

        return input;
    }

    // The track list and instructions (original HandleTrackMenu).
    private void DrawTrackMenuText()
    {
        uint yellow = ScrPalette.Colour(Track.ScrBaseColour + 3);
        uint white = ScrPalette.Colour(Track.ScrBaseColour + 15);
        float height = _graphics.ScreenHeight;

        _graphics.DrawTextLeft(new(2, 40), "Choose track :-", SmallFont, yellow);

        for (int i = 0; i < 8; i++)
        {
            string name = (TrackId)i switch
            {
                TrackId.LittleRamp => "Little Ramp",
                TrackId.SteppingStones => "Stepping Stones",
                TrackId.HumpBack => "Hump Back",
                TrackId.BigRamp => "Big Ramp",
                TrackId.SkiJump => "Ski Jump",
                TrackId.DrawBridge => "Draw Bridge",
                TrackId.HighJump => "High Jump",
                _ => "Roller Coaster",
            };

            uint colour = _track.Id == (TrackId)i ? white : yellow;
            _graphics.DrawTextLeft(
                new(2, 60 + (i * 16)),
                string.Create(CultureInfo.InvariantCulture, $"'{i + 1}' -  {name}"),
                SmallFont,
                colour);
        }

        _graphics.DrawTextLeft(
            new(2, height - 34),
            $"Current track - {_track.Name}.",
            SmallFont,
            yellow);
        _graphics.DrawTextLeft(
            new(2, height - 18),
            "Press 'S' to select, Escape to quit",
            SmallFont,
            yellow);
    }

    // The preview instructions (original HandleTrackPreview).
    private void DrawTrackPreviewText()
    {
        uint yellow = ScrPalette.Colour(Track.ScrBaseColour + 3);
        float height = _graphics.ScreenHeight;

        _graphics.DrawTextLeft(
            new(2, height - 130),
            $"Selected track - {_track.Name}.",
            SmallFont,
            yellow);
        _graphics.DrawTextLeft(
            new(2, height - 114),
            "Press 'S' to start game, 'M' for track menu, Escape to quit",
            SmallFont,
            yellow);

        _graphics.DrawTextLeft(new(2, height - 82), "Keyboard controls during game :-", SmallFont, yellow);
        _graphics.DrawTextLeft(
            new(2, height - 66),
            "  S = Steer left, D = Steer right, Enter = Accelerate, Space = Brake",
            SmallFont,
            yellow);
        _graphics.DrawTextLeft(new(2, height - 50), "  B = Brake, N = Change scenery", SmallFont, yellow);
        _graphics.DrawTextLeft(new(2, height - 34), "  M = Back to track menu, Escape = Quit", SmallFont, yellow);
    }

    // In-game text display, following the original GAME_IN_PROGRESS layout:
    // opponent name at race start, lap/boost/distance bottom left,
    // speed/damage bottom right, and the race result when finished.
    private void DrawHud()
    {
        uint white = ScrPalette.Colour(Track.ScrBaseColour + 15);
        uint yellow = ScrPalette.Colour(Track.ScrBaseColour + 3);
        float height = _graphics.ScreenHeight;
        float width = _graphics.ScreenWidth;

        // output the opponent's name for four seconds at race start
        if (_raceTick < 4 * TickRate)
        {
            _graphics.DrawTextCentre(height - 300, $"Opponent: {_opponent.Name}", SmallFont, white);
        }

        string lapText = _car.LapNumber > 0 ? _car.LapNumber.ToString(CultureInfo.InvariantCulture) : " ";
        _graphics.DrawTextLeft(new(2, height - 34), $"Lap: {lapText}   Boost: {_car.BoostReserve}", SmallFont, yellow);
        _graphics.DrawTextLeft(new(2, height - 18), $"Opponent Distance: {_opponent.DistanceToPlayer()}", SmallFont, yellow);

        _graphics.DrawTextLeft(new(width - 200, height - 34), $"Speed: {_car.DisplaySpeed}", SmallFont, yellow);
        _graphics.DrawTextLeft(new(width - 200, height - 18), $"Damage: {_car.NewDamage}", SmallFont, yellow);

        if (!_raceFinished)
        {
            return;
        }

        if (_mode == GameMode.GameOver)
        {
            _graphics.DrawTextCentre(height - 300, "GAME OVER: Press 'M' for track menu", LargeFont, white);
        }
        else
        {
            // the result text flashes white/black, changing every half second
            int flash = (_raceTick - _raceFinishedTick) % TickRate;
            uint colour = flash < TickRate / 2 ? white : ScrPalette.Colour(Track.ScrBaseColour);
            _graphics.DrawTextCentre(height - 300, _raceWon ? "RACE WON" : "RACE LOST", LargeFont, colour);
        }
    }
}
