// 'Useful Libraries' - Andy Hawkins 2023-2026.

using Useful.Input;

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
    private TScreen? _current;

    public ScreenManager(IKeyboard keyboard)
    {
        ArgumentNullException.ThrowIfNull(keyboard);

        _keyboard = keyboard;
    }

    public TId CurrentId { get; private set; } = default!;

    /// <summary>
    /// Gets the current screen. Throws until <see cref="Set"/> has been
    /// called at least once, so callers past setup never need a
    /// null-forgiving access.
    /// </summary>
    /// <exception cref="InvalidOperationException"><see cref="Set"/> has not
    /// been called yet.</exception>
    public TScreen Current => _current
        ?? throw new InvalidOperationException($"No screen is current; call {nameof(Set)} first.");

    public void Add(TId id, TScreen screen)
    {
        ArgumentNullException.ThrowIfNull(screen);

        _screens.Add(id, screen);
    }

    public void Set(TId id)
    {
        CurrentId = id;
        _current = _screens[id];
        _keyboard.ClearPressed();
        _current.Reset();
    }
}
