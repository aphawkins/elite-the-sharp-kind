// 'Useful Libraries' - Andy Hawkins 2023-2026.

namespace Useful.Abstraction;

/// <summary>
/// A game as the composition root sees it: something that can be started and
/// runs until it finishes.
/// </summary>
/// <remarks>
/// Deliberately separate from <see cref="IGame"/>, which is the other side of
/// the same object - the per-tick <c>Update</c>/<c>Draw</c> pair that
/// <c>GameHost</c> consumes while <see cref="Run"/> is executing. The host and
/// the composition root want different things from a game, so they ask for
/// them through different interfaces.
/// </remarks>
public interface IGameApp
{
    /// <summary>
    /// Runs the game to completion, returning when it stops.
    /// </summary>
    public void Run();
}
