// 'SharpKind Libraries' - Andy Hawkins 2023-2026.

namespace SharpKind.Input;

public class SoftwareGamepad : IGamepad, IGamepadSink
{
    private readonly Dictionary<GamepadButton, bool> _heldButtons = [];
    private readonly Dictionary<GamepadButton, bool> _pressedButtons = [];
    private readonly Dictionary<GamepadAxis, float> _axes = [];
    private readonly IInput _input;
    private int _deviceCount;

    public SoftwareGamepad(IInput input)
    {
        ArgumentNullException.ThrowIfNull(input);

        input.Register(this);
        _input = input;
    }

    public bool IsConnected => _deviceCount > 0;

    public void ClearPressed()
    {
        _heldButtons.Clear();
        _pressedButtons.Clear();
        _axes.Clear();
    }

    public bool IsPressed(GamepadButton button)
    {
        if (button == GamepadButton.None)
        {
            return false;
        }

        if (_pressedButtons.TryGetValue(button, out bool value) && value)
        {
            _pressedButtons[button] = false;
            return true;
        }

        return false;
    }

    public bool IsHeld(GamepadButton button)
        => button != GamepadButton.None && _heldButtons.TryGetValue(button, out bool value) && value;

    public float Axis(GamepadAxis axis) => _axes.TryGetValue(axis, out float value) ? value : 0f;

    public void Connected() => _deviceCount++;

    public void Disconnected()
    {
        if (_deviceCount > 0)
        {
            _deviceCount--;
        }

        if (_deviceCount == 0)
        {
            // A button held as the device is unplugged would otherwise stay
            // held forever, leaving the car steering with no way to stop it.
            ClearPressed();
        }
    }

    public void ButtonDown(GamepadButton button)
    {
        if (button == GamepadButton.None)
        {
            return;
        }

        _heldButtons[button] = true;
        _pressedButtons[button] = true;
    }

    public void ButtonUp(GamepadButton button)
    {
        _heldButtons[button] = false;
        _pressedButtons[button] = false;
    }

    public void AxisMoved(GamepadAxis axis, float value) => _axes[axis] = Math.Clamp(value, -1f, 1f);

    public void Poll() => _input.Poll();
}
