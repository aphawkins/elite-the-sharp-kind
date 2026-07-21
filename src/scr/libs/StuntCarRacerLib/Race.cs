// 'Stunt Car Racer - The Sharp Kind' - Andy Hawkins 2026.
// 'Stunt Car Racer Remake' - sourceforge.net/projects/stuntcarremake.
// Stunt Car Racer (C) Geoff Crammond / MicroStyle / MicroProse 1989.

using System.Diagnostics.CodeAnalysis;
using StuntCarRacerLib.Cars;
using StuntCarRacerLib.Rendering;
using StuntCarRacerLib.Tracks;
using Useful;
using Useful.Audio;
using Useful.Graphics;

namespace StuntCarRacerLib;

// The track/car/opponent state and its world/HUD rendering: the part of the
// game screens actually drive, split out of StuntCarRacerMain (which keeps
// only the run loop, screen wiring and audio setup) so screens depend on
// this instead of the whole game object.
internal sealed class Race
{
    private const int DefaultFrameGap = 4;

    private readonly IGraphics _graphics;
    private readonly ScrPalette _palette;
    private readonly ISound _sound;
    private readonly AudioController _audio;
    private readonly BackdropRenderer _backdrop;
    private readonly HudRenderer _hud;
    private readonly CarMesh _carMesh;
    private readonly RoadTextures _roadTextures;
    private readonly List<WorldPolygon> _worldPolygons = [];

    private TrackRenderer _renderer;
    private OpponentRenderer _opponentRenderer;
    private int _frameCount;

    internal Race(IGraphics graphics, ScrPalette palette, ISound sound, AudioController audio, TrackId trackId)
    {
        _graphics = graphics;
        _palette = palette;
        _sound = sound;
        _audio = audio;
        _backdrop = new(graphics, palette);
        _hud = new(graphics);
        _carMesh = new(palette);
        _roadTextures = new(palette);

        LoadTrack(trackId);
    }

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

    internal void NextScenery() => _backdrop.NextSceneryType();

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

    [MemberNotNull(
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
        _renderer = new(Track, _graphics, _palette, _roadTextures);
        _opponentRenderer = new(Opponent, _carMesh, _palette);
    }

    // The world common to every screen: backdrop, track and (outside the
    // track menu) the opponent car. Road lines draw around the player's
    // position, as the original did in every mode.
    internal void DrawWorld(bool showOpponent)
    {
        _graphics.Clear();
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
            _audio.PlayEffect("Grounded", Car.GroundedVolume, pitch: 1.0);
        }

        if (Car.CreakSoundTriggered)
        {
            _audio.PlayEffect("Creak", Car.CreakVolume, pitch: 1.0);
        }

        if (Car.SmashSoundTriggered)
        {
            _audio.PlayEffect("Smash");
        }

        if (Car.OffRoadSoundTriggered)
        {
            _audio.PlayEffect("OffRoad", volume: null, Car.OffRoadPitch);
        }
        else if (Car.WreckSoundTriggered)
        {
            _audio.PlayEffect("Wreck", volume: null, Car.WreckPitch);
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
        _sound.PlayLoop(sample, frequency / sampleRate);
    }

    // The in-game display: the cockpit overlay (wheels, engine, damage
    // crack/holes, speed bar, lap/boost/distance read-outs) plus the
    // remake's text overlays (opponent name at race start, race result).
    internal void DrawHud(bool gameOver)
    {
        FastColor white = _palette.Colour(Track.ScrBaseColour + 15);
        float height = _graphics.ScreenHeight;

        _hud.Draw(new(
            Car.LeftWheelFrame,
            Car.RightWheelFrame,
            Car.LeftWheelBounce,
            Car.RightWheelBounce,
            Car.BoostActivated != 0,
            Car.NewDamage,
            Car.SmashHoles,
            Car.DisplaySpeed,
            Car.LapNumber,
            Car.BoostReserve,
            Opponent.DistanceToPlayer(),
            Car.OnChains,
            Car.WaitingToReleaseChains,
            Car.ZAngle,
            Car.CurrentLapTicks,
            Car.BestLapTicks));

        // output the opponent's name for four seconds at race start
        if (RaceTick < 4 * StuntCarRacerMain.TickRate)
        {
            _graphics.DrawTextCentre(height - 300, $"Opponent: {Opponent.Name}", StuntCarRacerMain.SmallFont, white);
        }

        if (!RaceFinished)
        {
            return;
        }

        if (gameOver)
        {
            _graphics.DrawTextCentre(height - 300, "GAME OVER: Press 'M' for track menu", StuntCarRacerMain.LargeFont, white);
        }
        else
        {
            // the result text flashes white/black, changing every half second
            int flash = (RaceTick - RaceFinishedTick) % StuntCarRacerMain.TickRate;
            FastColor colour = flash < StuntCarRacerMain.TickRate / 2 ? white : _palette.Colour(Track.ScrBaseColour);
            _graphics.DrawTextCentre(height - 300, RaceWon ? "RACE WON" : "RACE LOST", StuntCarRacerMain.LargeFont, colour);
        }
    }
}
