// 'Stunt Car Racer - The Sharp Kind' - Andy Hawkins 2026.
// 'Stunt Car Racer Remake' - sourceforge.net/projects/stuntcarremake.
// Stunt Car Racer (C) Geoff Crammond / MicroStyle / MicroProse 1989.

using StuntCarRacerLib.Fakes;
using StuntCarRacerLib.Screens;
using Useful.Assets;
using Useful.Fakes.Controls;
using Useful.Graphics;

namespace StuntCarRacerLib.Tests;

// Drives the real StuntCarRacerMain against a real SoftwareGraphics with no
// SDL window (generalising StuntCarRacerMainTests.StartRace's menu->race
// key sequence into a scripted timeline any test can replay), for tests
// that need several ticks of real gameplay and, occasionally, a rendered
// frame to eyeball.
internal sealed class HeadlessGameHarness : IDisposable
{
    private readonly SoftwareGraphics _graphics;
    private FastBitmap? _lastFrame;

    public HeadlessGameHarness(int width = 640, int height = 400)
    {
        _graphics = SoftwareGraphics.Create(width, height, b => _lastFrame = b, AssetLocator.Create());
        FakeAbstraction abstraction = new(_graphics);
        Keyboard = (FakeKeyboard)abstraction.Keyboard;
        Game = new(abstraction);
    }

    public StuntCarRacerMain Game { get; }

    public FakeKeyboard Keyboard { get; }

    public int Tick { get; private set; }

    public GameStateSummary State => new(
        Game.Screens.CurrentId,
        Game.Screens.CurrentId is GameMode.GameInProgress or GameMode.GameOver,
        Game.Race.Car.CurrentPiece,
        Game.Race.Opponent.CurrentPiece,
        Game.Race.Opponent.DistanceToPlayer());

    // Advances one tick, applying any scripted key events due at the
    // current tick before calling Update() and releasing any single-tick
    // taps afterwards.
    public GameStateSummary Step(IReadOnlyList<KeyScriptEvent> script)
    {
        List<(ConsoleKey Key, ConsoleModifiers Modifiers)>? taps = null;
        foreach (KeyScriptEvent scriptEvent in script)
        {
            if (scriptEvent.Tick != Tick)
            {
                continue;
            }

            switch (scriptEvent.Action)
            {
                case KeyScriptAction.Tap:
                    Keyboard.KeyDown(scriptEvent.Key, scriptEvent.Modifiers);
                    (taps ??= []).Add((scriptEvent.Key, scriptEvent.Modifiers));
                    break;

                case KeyScriptAction.Hold:
                    Keyboard.KeyDown(scriptEvent.Key, scriptEvent.Modifiers);
                    break;

                case KeyScriptAction.Release:
                    Keyboard.KeyUp(scriptEvent.Key, scriptEvent.Modifiers);
                    break;
            }
        }

        Game.Update();

        if (taps is not null)
        {
            foreach ((ConsoleKey key, ConsoleModifiers modifiers) in taps)
            {
                Keyboard.KeyUp(key, modifiers);
            }
        }

        Tick++;
        return State;
    }

    public GameStateSummary Run(int ticks, IReadOnlyList<KeyScriptEvent> script)
    {
        GameStateSummary state = State;
        for (int i = 0; i < ticks; i++)
        {
            state = Step(script);
        }

        return state;
    }

    // Renders the whole game (screens and HUD included, unlike
    // VisualDumpTests' hand-composed scenes) and saves it as a BMP.
    public void SaveFrame(string path)
    {
        Game.Draw();
        if (_lastFrame is null)
        {
            throw new InvalidOperationException("No frame has been rendered yet.");
        }

        BitmapWriter.Write(_lastFrame, path);
    }

    public void Dispose() => _graphics.Dispose();
}
