// 'Stunt Car Racer - The Sharp Kind' - Andy Hawkins 2026.
// 'Stunt Car Racer Remake' - sourceforge.net/projects/stuntcarremake.
// Stunt Car Racer (C) Geoff Crammond / MicroStyle / MicroProse 1989.

using System.Diagnostics;
using System.Runtime.CompilerServices;
using StuntCarRacerLib.Cars;
using StuntCarRacerLib.Rendering;
using StuntCarRacerLib.Tracks;
using Useful;
using Useful.Abstraction;
using Useful.Controls;
using Useful.Graphics;

[assembly: CLSCompliant(false)]
[assembly: InternalsVisibleTo("StuntCarRacerLib.Tests")]

namespace StuntCarRacerLib;

public sealed class StuntCarRacerMain
{
    private const int Fps = 30;

    private readonly IGraphics _graphics;
    private readonly IKeyboard _keyboard;
    private readonly long _oneSecondInTicks = TimeSpan.FromSeconds(1).Ticks;
    private readonly long _timerResolution = TimeSpan.FromSeconds(1).Ticks / Stopwatch.Frequency;

    private readonly Track _track;
    private readonly CarPhysics _car;
    private readonly OpponentPhysics _opponent;
    private readonly SceneCamera _camera = new();
    private readonly TrackRenderer _renderer;
    private readonly BackdropRenderer _backdrop;
    private readonly OpponentRenderer _opponentRenderer;
    private readonly List<WorldPolygon> _worldPolygons = [];

    private bool _exitGame;
    private bool _sceneryKeyDown;
    private int _raceFrame;
    private bool _raceFinished;
    private bool _raceWon;
    private bool _gameOver;
    private int _raceFinishedFrame;

    public StuntCarRacerMain(IAbstraction abstraction)
        : this(abstraction, TrackId.LittleRamp)
    {
    }

    public StuntCarRacerMain(IAbstraction abstraction, TrackId trackId)
    {
        Guard.ArgumentNull(abstraction);

        _graphics = abstraction.Graphics;
        _keyboard = abstraction.Keyboard;

        _track = Track.Load(trackId);
        _car = new(_track);
        _opponent = new(_track, _car);
        _renderer = new(_track, _graphics);
        _backdrop = new(_graphics);
        _opponentRenderer = new(_opponent);

        StartRace();
    }

    public void Run()
    {
        long startTicks = Stopwatch.GetTimestamp() * _timerResolution;
        long intervalTicks = _oneSecondInTicks / Fps;

        do
        {
            long nowTicks = Stopwatch.GetTimestamp() * _timerResolution;
            if (((nowTicks - startTicks) % intervalTicks) == 0)
            {
                _keyboard.Poll();

                if (_keyboard.IsPressed(ConsoleKey.Escape))
                {
                    _exitGame = true;
                }

                UpdateFrame();
                DrawFrame();
            }
        }
        while (!_exitGame && !_keyboard.Close);
    }

    internal void UpdateFrame()
    {
        _raceFrame++;

        _car.Update(ReadInput());
        _opponent.Update();
        _car.UpdateLapData();
        _opponent.UpdateLapData();
        _car.UpdateDamage();
        _camera.FollowCar(_car);

        // the race finishes when either car completes the final lap
        if (!_raceFinished && (_car.RaceFinished || _opponent.LapNumber >= 4))
        {
            _raceFinished = true;
            _raceWon = _opponent.CalculateIfWinning() < 0;
            _raceFinishedFrame = _raceFrame;
        }

        // show the race result for six seconds, then it is game over
        if (_raceFinished && !_gameOver && _raceFrame - _raceFinishedFrame > 6 * Fps)
        {
            _gameOver = true;
        }

        // M restarts the race (the original returns to the track menu)
        if (_gameOver && _keyboard.IsPressed(ConsoleKey.M))
        {
            StartRace();
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
    }

    internal void DrawFrame()
    {
        _graphics.Clear();
        _backdrop.Draw(_camera);

        _worldPolygons.Clear();
        _opponentRenderer.AppendWorldPolygons(_worldPolygons);
        _renderer.Draw(_camera, _worldPolygons);

        DrawHud();
        _graphics.ScreenUpdate();
    }

    private void StartRace()
    {
        _car.StartRace();
        _car.BoostReserve = _track.StandardBoost;
        _opponent.StartRace();

        _raceFrame = 0;
        _raceFinished = false;
        _raceWon = false;
        _gameOver = false;
        _raceFinishedFrame = 0;
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

    // In-game text display, following the original GAME_IN_PROGRESS layout:
    // opponent name at race start, lap/boost/distance bottom left,
    // speed/damage bottom right, and the race result when finished.
    private void DrawHud()
    {
        const string smallFont = "Small";
        const string largeFont = "Large";
        uint white = ScrPalette.Colour(Track.ScrBaseColour + 15);
        uint yellow = ScrPalette.Colour(Track.ScrBaseColour + 3);
        float height = _graphics.ScreenHeight;
        float width = _graphics.ScreenWidth;

        // output the opponent's name for four seconds at race start
        if (_raceFrame < 4 * Fps)
        {
            _graphics.DrawTextCentre(height - 300, $"Opponent: {_opponent.Name}", smallFont, white);
        }

        string lapText = _car.LapNumber > 0 ? _car.LapNumber.ToString(System.Globalization.CultureInfo.InvariantCulture) : " ";
        _graphics.DrawTextLeft(new(2, height - 34), $"Lap: {lapText}   Boost: {_car.BoostReserve}", smallFont, yellow);
        _graphics.DrawTextLeft(new(2, height - 18), $"Opponent Distance: {_opponent.DistanceToPlayer()}", smallFont, yellow);

        _graphics.DrawTextLeft(new(width - 200, height - 34), $"Speed: {_car.DisplaySpeed}", smallFont, yellow);
        _graphics.DrawTextLeft(new(width - 200, height - 18), $"Damage: {_car.NewDamage}", smallFont, yellow);

        if (!_raceFinished)
        {
            return;
        }

        if (_gameOver)
        {
            _graphics.DrawTextCentre(height - 300, "GAME OVER: Press M to race again", largeFont, white);
        }
        else
        {
            // the result text flashes white/black, changing every half second
            int flash = (_raceFrame - _raceFinishedFrame) % Fps;
            uint colour = flash < Fps / 2 ? white : ScrPalette.Colour(Track.ScrBaseColour);
            _graphics.DrawTextCentre(height - 300, _raceWon ? "RACE WON" : "RACE LOST", largeFont, colour);
        }
    }
}
