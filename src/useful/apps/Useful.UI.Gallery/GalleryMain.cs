// 'Useful Libraries' - Andy Hawkins 2023-2026.

using Useful.Abstraction;
using Useful.Abstraction.Config;

namespace Useful.UI.Gallery;

/// <summary>
/// The gallery as the composition root and the game host see it: something to
/// run, and a tick to run. There is no state to advance, so an update is only
/// the input the controls take.
/// </summary>
/// <param name="abstraction">The window, its surface and its keyboard.</param>
/// <param name="engine">The engine settings, for the frame rate.</param>
internal sealed class GalleryMain(IAbstraction abstraction, EngineConfigSettings engine) : IGameApp, IGame
{
    // Nothing here moves, so the update rate only has to be quick enough that
    // a key press feels immediate.
    private const double TicksPerSecond = 30;

    private readonly Gallery _gallery = new(abstraction.Graphics);

    public bool IsRunning { get; private set; } = true;

    public void Run() => GameHost.Run(abstraction, this, TicksPerSecond, engine.Graphics.Fps);

    public void Update()
    {
        if (abstraction.Keyboard.IsPressed(ConsoleKey.Escape))
        {
            IsRunning = false;
            return;
        }

        _gallery.HandleInput(abstraction.Keyboard);
    }

    public void Draw()
    {
        abstraction.Graphics.Clear();
        _gallery.Draw();
        abstraction.Graphics.ScreenUpdate();
    }
}
