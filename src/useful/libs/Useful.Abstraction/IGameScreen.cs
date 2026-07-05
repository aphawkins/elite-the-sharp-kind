// 'Useful Libraries' - Andy Hawkins 2025.

namespace Useful.Abstraction;

/// <summary>
/// One screen (menu, view, game mode) of an <see cref="IGame"/>, managed by
/// a <see cref="ScreenManager{TId, TScreen}"/>: <see cref="Reset"/> runs when
/// the screen becomes current, <see cref="Update"/> advances it one
/// fixed-rate tick and <see cref="Draw"/> renders its current state.
/// </summary>
public interface IGameScreen
{
    public void Reset();

    public void Update();

    public void Draw();
}
