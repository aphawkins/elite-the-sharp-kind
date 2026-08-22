// 'SharpKind Libraries' - Andy Hawkins 2023-2026.

namespace SharpKind.Input;

/// <summary>
/// Consumer-facing side of the gamepad, with the same pressed-vs-held
/// semantics <see cref="IKeyboard"/> documents. Covers both target device
/// classes: XInput-style pads with analog axes, and generic-HID digital
/// sticks whose 8-way stick and hat report axes as exactly -1, 0 or +1.
/// </summary>
public interface IGamepad
{
    /// <summary>
    /// Gets a value indicating whether any device is currently attached.
    /// Nothing is pressed and every axis reads 0 while this is <c>false</c>,
    /// so callers only need this to decide whether to show gamepad hints.
    /// </summary>
    public bool IsConnected { get; }

    public void ClearPressed();

    /// <summary>
    /// One-shot "was this button just pressed" check: a single physical
    /// button-down is consumed (and reported) at most once, even while the
    /// button remains held. Suited to menu/UI actions. For continuous
    /// controls polled every tick, use <see cref="IsHeld(GamepadButton)"/>.
    /// </summary>
    public bool IsPressed(GamepadButton button);

    /// <summary>
    /// Continuous "is this button currently down" check, with no consuming
    /// side effect, so repeated polls see the button stay held for as long
    /// as it physically is.
    /// </summary>
    public bool IsHeld(GamepadButton button);

    /// <summary>
    /// The axis position, -1 to +1 (triggers: 0 to +1). A digital device
    /// reports only -1, 0 or +1. Reading an axis never consumes it.
    /// </summary>
    public float Axis(GamepadAxis axis);

    public void Poll();
}
