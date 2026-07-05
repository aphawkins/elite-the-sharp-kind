// 'Useful Libraries' - Andy Hawkins 2025.

using Useful.Timing;

namespace Useful.Abstraction;

/// <summary>
/// Runs an <see cref="IGame"/> on the fixed-timestep <see cref="GameLoop"/>:
/// the keyboard is polled and the game updated at <c>updatesPerSecond</c>,
/// and the game draws at up to <c>maxFramesPerSecond</c>.
/// </summary>
public static class GameHost
{
    public static void Run(IAbstraction abstraction, IGame game, double updatesPerSecond, double maxFramesPerSecond)
    {
        Guard.ArgumentNull(abstraction);
        Guard.ArgumentNull(game);

        GameLoop loop = new(
            updatesPerSecond,
            () =>
            {
                abstraction.Keyboard.Poll();
                game.Update();
            },
            game.Draw,
            () => game.IsRunning && !abstraction.Keyboard.Close,
            maxFramesPerSecond);
        loop.Run();
    }
}
