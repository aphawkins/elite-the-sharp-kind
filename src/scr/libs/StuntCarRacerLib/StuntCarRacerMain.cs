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
    private readonly SceneCamera _camera = new();
    private readonly TrackRenderer _renderer;

    private bool _exitGame;

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
        _renderer = new(_track, _graphics);

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
        _car.Update(ReadInput());
        _car.UpdateLapData();
        _camera.FollowCar(_car);
    }

    internal void DrawFrame()
    {
        _graphics.Clear();
        _renderer.Draw(_camera);
        DrawHud();
        _graphics.ScreenUpdate();
    }

    private void StartRace()
    {
        _car.StartRace();
        _car.BoostReserve = _track.StandardBoost;
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

    // Simple speed/boost bars until fonts and the original dashboard are in.
    private void DrawHud()
    {
        float width = _graphics.ScreenWidth;
        float height = _graphics.ScreenHeight;

        // speed bar along the bottom (display speed range is roughly 0-170)
        float speedWidth = _car.DisplaySpeed * (width - 20) / 170;
        _graphics.DrawRectangleFilled(new(10, height - 20), speedWidth, 8, ScrPalette.Colour(Track.ScrBaseColour + 3));

        // boost reserve bar above it
        float boostWidth = _car.BoostReserve * (width - 20) / Math.Max(1, _track.StandardBoost);
        _graphics.DrawRectangleFilled(new(10, height - 34), boostWidth, 8, ScrPalette.Colour(Track.ScrBaseColour + 10));
    }
}
