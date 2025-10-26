// 'Useful Libraries' - Andy Hawkins 2025.

namespace Useful.Controls;

public class SoftwareKeyboard : IKeyboard
{
    private readonly Dictionary<ConsoleKey, bool> _pressedKeys = [];
    private readonly IInput _input;
    private ConsoleKey _lastKeyPressed;
    private ConsoleModifiers _lastModifierPressed;
    private ConsoleModifiers _pressedModifiers = ConsoleModifiers.None;

    public SoftwareKeyboard(IInput input)
    {
        Guard.ArgumentNull(input);

        input.Register(this);
        _input = input;
    }

    public bool Close { get; set; }

    public void ClearPressed()
    {
        _lastKeyPressed = ConsoleKey.None;
        _pressedKeys.Clear();
        _lastModifierPressed = ConsoleModifiers.None;
        _pressedModifiers = ConsoleModifiers.None;
    }

    public bool IsPressed(ConsoleKey key)
    {
        if (key == ConsoleKey.None)
        {
            return false;
        }

        if (_lastKeyPressed == key)
        {
            _lastKeyPressed = ConsoleKey.None;
            ClearPressed(key);
            return true;
        }

        if (_pressedKeys.TryGetValue(key, out bool value) && value)
        {
            ClearPressed(key);
            return true;
        }

        return false;
    }

    public bool IsPressed(ConsoleModifiers modifiers)
    {
        if (_lastModifierPressed.HasFlag(modifiers))
        {
            _lastModifierPressed &= ~modifiers;
            ClearPressed(modifiers);
            return true;
        }

        if (_pressedModifiers.HasFlag(modifiers))
        {
            ClearPressed(modifiers);
            return true;
        }

        return false;
    }

    public void KeyDown(ConsoleKey key, ConsoleModifiers modifiers)
    {
        _lastKeyPressed = key;
        _lastModifierPressed = modifiers;
        _pressedKeys[key] = true;
        _pressedModifiers |= modifiers;
    }

    public void KeyUp(ConsoleKey key, ConsoleModifiers modifiers)
    {
        _pressedKeys[key] = false;
        ClearPressed(key);
        if (_lastKeyPressed == key)
        {
            _lastKeyPressed = ConsoleKey.None;
        }

        ClearPressed(modifiers);
        if (_lastModifierPressed.HasFlag(modifiers))
        {
            _lastModifierPressed &= ~modifiers;
        }
    }

    public (ConsoleKey Key, ConsoleModifiers Modifiers) LastPressed()
    {
        if ((_lastKeyPressed == ConsoleKey.None) && (_lastModifierPressed == ConsoleModifiers.None))
        {
            return (ConsoleKey.None, ConsoleModifiers.None);
        }

        (ConsoleKey Key, ConsoleModifiers Modifiers) keys = (_lastKeyPressed, _lastModifierPressed);
        _lastKeyPressed = ConsoleKey.None;
        _lastModifierPressed = ConsoleModifiers.None;
        return keys;
    }

    public void Poll() => _input.Poll();

    private void ClearPressed(ConsoleKey key) => _pressedKeys[key] = false;

    private void ClearPressed(ConsoleModifiers modifiers) => _pressedModifiers &= ~modifiers;
}
