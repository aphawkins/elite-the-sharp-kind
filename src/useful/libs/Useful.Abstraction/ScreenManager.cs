// 'Useful Libraries' - Andy Hawkins 2025.

using Useful.Controls;

namespace Useful.Abstraction;

/// <summary>
/// A state machine over a game's screens, keyed by <typeparamref name="TId"/>
/// (typically an enum). <see cref="Set"/> makes a screen current, clearing
/// any pending key presses and resetting the screen on the way in.
/// </summary>
/// <typeparam name="TId">The key identifying each screen.</typeparam>
/// <typeparam name="TScreen">The screen type held, so games can expose
/// members beyond <see cref="IGameScreen"/> on <see cref="Current"/>.</typeparam>
public sealed class ScreenManager<TId, TScreen>
    where TId : notnull
    where TScreen : class, IGameScreen
{
    private readonly Dictionary<TId, TScreen> _screens = [];
    private readonly IKeyboard _keyboard;

    public ScreenManager(IKeyboard keyboard)
    {
        Guard.ArgumentNull(keyboard);

        _keyboard = keyboard;
    }

    public TId CurrentId { get; private set; } = default!;

    public TScreen? Current { get; private set; }

    public void Add(TId id, TScreen screen)
    {
        Guard.ArgumentNull(screen);

        _screens.Add(id, screen);
    }

    public void Set(TId id)
    {
        CurrentId = id;
        Current = _screens[id];
        _keyboard.ClearPressed();
        Current.Reset();
    }
}
