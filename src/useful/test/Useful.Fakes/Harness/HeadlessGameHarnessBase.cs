// 'Useful Libraries' - Andy Hawkins 2025.

using Useful.Assets;
using Useful.Controls;
using Useful.Fakes.Controls;
using Useful.Graphics;

namespace Useful.Fakes.Harness;

// Drives a game's Update()/Draw() directly against a real SoftwareGraphics
// with no SDL window, applying a scripted KeyScriptEvent timeline to a
// FakeKeyboard each tick - shared machinery behind both StuntCarRacerLib and
// EliteSharpLib's headless game harnesses. Only building each game's own
// object graph and its state snapshot differ between them; those stay in
// each game's own derived harness.
public abstract class HeadlessGameHarnessBase<TState> : IDisposable
{
    private FastBitmap? _lastFrame;

    protected HeadlessGameHarnessBase(int width, int height, IAssetLocator assetLocator)
        => Graphics = SoftwareGraphics.Create(width, height, b => _lastFrame = b, assetLocator);

    public FakeKeyboard Keyboard { get; protected set; } = null!;

    public int Tick { get; private set; }

    public abstract TState State { get; }

    protected SoftwareGraphics Graphics { get; }

    // Advances one tick, applying any scripted key events due at the
    // current tick before calling the game's Update() and releasing any
    // single-tick taps afterwards.
    public TState Step(IReadOnlyList<KeyScriptEvent> script)
    {
        Guard.ArgumentNull(script);

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

                case KeyScriptAction.SaveFrame:
                    // No-op here: headless callers save frames explicitly, via this
                    // class's own SaveFrame method. This action only matters to
                    // KeyScriptPlayer, the real-app counterpart driven by GameHost.
                    break;
            }
        }

        UpdateGame();

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

    public TState Run(int ticks, IReadOnlyList<KeyScriptEvent> script)
    {
        TState state = State;
        for (int i = 0; i < ticks; i++)
        {
            state = Step(script);
        }

        return state;
    }

    // Renders the whole game (screens and HUD included) and saves it as a
    // BMP.
    public void SaveFrame(string path)
    {
        DrawGame();
        if (_lastFrame is null)
        {
            throw new InvalidOperationException("No frame has been rendered yet.");
        }

        BitmapWriter.Write(_lastFrame, path);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    // One game tick: composes the whole frame (StuntCarRacerMain.Update) or
    // just advances state where drawing is a separate call (EliteMain).
    protected abstract void UpdateGame();

    // Presents/renders the frame SaveFrame will capture.
    protected abstract void DrawGame();

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            Graphics.Dispose();
        }
    }
}
