// 'Useful Libraries' - Andy Hawkins 2023-2026.

namespace Useful.Input;

public interface IKeyboard
{
    public bool Close { get; }

    public void ClearPressed();

    /// <summary>
    /// One-shot "was this key just pressed" check: a single physical
    /// key-down is consumed (and reported) at most once, even while the
    /// key remains held. Suited to menu/UI actions. For continuous
    /// movement controls polled every tick (e.g. steering/accelerating),
    /// use <see cref="IsHeld(ConsoleKey)"/> instead.
    /// </summary>
    public bool IsPressed(ConsoleKey key);

    /// <summary>
    /// One-shot "was this modifier just pressed" check, consuming in the
    /// same way as <see cref="IsPressed(ConsoleKey)"/>. Use
    /// <see cref="IsHeld(ConsoleModifiers)"/> to test a modifier that only
    /// decides how another key is read - a consuming read there takes the
    /// modifier away from whichever handler polls next.
    /// </summary>
    public bool IsPressed(ConsoleModifiers modifiers);

    /// <summary>
    /// Continuous "is this key currently down" check: reflects the
    /// physical key state with no consuming side effect, so repeated
    /// polls (e.g. once per physics tick) see the key stay held for as
    /// long as it physically is, even alongside other held keys.
    /// </summary>
    public bool IsHeld(ConsoleKey key);

    /// <summary>
    /// Continuous "is this modifier currently down" check, with no consuming
    /// side effect. Meant for guards of the form "Ctrl decides what this key
    /// does": test the modifier with this first, then consume the key, so a
    /// key press with no modifier is left for the handler it belongs to.
    /// </summary>
    public bool IsHeld(ConsoleModifiers modifiers);

    public (ConsoleKey Key, ConsoleModifiers Modifiers) LastPressed();

    public void Poll();
}
