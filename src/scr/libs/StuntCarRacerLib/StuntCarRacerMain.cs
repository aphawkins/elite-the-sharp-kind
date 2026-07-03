// 'Stunt Car Racer - The Sharp Kind' - Andy Hawkins 2026.
// 'Stunt Car Racer Remake' - sourceforge.net/projects/stuntcarremake.
// Stunt Car Racer (C) Geoff Crammond / MicroStyle / MicroProse 1989.

using System.Diagnostics;
using Useful;
using Useful.Abstraction;
using Useful.Controls;
using Useful.Graphics;

[assembly: CLSCompliant(false)]

namespace StuntCarRacerLib;

public sealed class StuntCarRacerMain
{
    private const int Fps = 30;

    private readonly IGraphics _graphics;
    private readonly IKeyboard _keyboard;
    private readonly long _oneSecondInTicks = TimeSpan.FromSeconds(1).Ticks;
    private readonly long _timerResolution = TimeSpan.FromSeconds(1).Ticks / Stopwatch.Frequency;
    private bool _exitGame;

    public StuntCarRacerMain(IAbstraction abstraction)
    {
        Guard.ArgumentNull(abstraction);

        _graphics = abstraction.Graphics;
        _keyboard = abstraction.Keyboard;
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

                DrawFrame();
            }
        }
        while (!_exitGame && !_keyboard.Close);
    }

    private void DrawFrame()
    {
        _graphics.Clear();

        // Placeholder scene until the track renderer is ported.
        float centreX = _graphics.ScreenWidth / 2;
        float centreY = _graphics.ScreenHeight / 2;
        _graphics.DrawRectangle(new(centreX - 100, centreY - 50), 200, 100, 0xFFFFFFFF);
        _graphics.DrawLine(new(centreX - 100, centreY - 50), new(centreX + 100, centreY + 50), 0xFF00FF00);
        _graphics.DrawLine(new(centreX - 100, centreY + 50), new(centreX + 100, centreY - 50), 0xFF00FF00);

        _graphics.ScreenUpdate();
    }
}
