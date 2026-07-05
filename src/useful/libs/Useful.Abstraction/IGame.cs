// 'Useful Libraries' - Andy Hawkins 2025.

namespace Useful.Abstraction;

/// <summary>
/// A game hosted by <see cref="GameHost"/>: <see cref="Update"/> advances
/// the game one fixed-rate tick and <see cref="Draw"/> presents the current
/// state, independently of the update rate.
/// </summary>
public interface IGame
{
    public bool IsRunning { get; }

    public void Update();

    public void Draw();
}
