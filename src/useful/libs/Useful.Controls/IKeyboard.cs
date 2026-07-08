// 'Useful Libraries' - Andy Hawkins 2025.

namespace Useful.Controls;

public interface IKeyboard
{
    public bool Close { get; set; }

    public void ClearPressed();

    /// <summary>
    /// One-shot "was this key just pressed" check: a single physical
    /// key-down is consumed (and reported) at most once, even while the
    /// key remains held. Suited to menu/UI actions. For continuous
    /// movement controls polled every tick (e.g. steering/accelerating),
    /// use <see cref="IsHeld(ConsoleKey)"/> instead.
    /// </summary>
    public bool IsPressed(ConsoleKey key);

    public bool IsPressed(ConsoleModifiers modifiers);

    /// <summary>
    /// Continuous "is this key currently down" check: reflects the
    /// physical key state with no consuming side effect, so repeated
    /// polls (e.g. once per physics tick) see the key stay held for as
    /// long as it physically is, even alongside other held keys.
    /// </summary>
    public bool IsHeld(ConsoleKey key);

    public void KeyDown(ConsoleKey key, ConsoleModifiers modifiers);

    public void KeyUp(ConsoleKey key, ConsoleModifiers modifiers);

    public (ConsoleKey Key, ConsoleModifiers Modifiers) LastPressed();

    public void Poll();
}
