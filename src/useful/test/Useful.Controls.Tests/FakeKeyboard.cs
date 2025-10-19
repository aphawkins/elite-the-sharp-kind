// 'Useful Libraries' - Andy Hawkins 2025.

namespace Useful.Controls.Tests;

// Minimal in-test fake implementation of IKeyboard to validate contract behavior.
internal sealed class FakeKeyboard : IKeyboard
{
    private readonly HashSet<(ConsoleKey Key, ConsoleModifiers Modifiers)> _pressed = [];
    private (ConsoleKey Key, ConsoleModifiers Modifiers)? _last;

    public bool Close { get; private set; }

    public void ClearPressed() => _pressed.Clear();

    public bool IsPressed(ConsoleKey key) => _pressed.Any(p => p.Key == key);

    public bool IsPressed(ConsoleModifiers modifiers) => _pressed.Any(p => p.Modifiers == modifiers);

    public void KeyDown(ConsoleKey key, ConsoleModifiers modifiers)
    {
        _pressed.Add((key, modifiers));
        _last = (key, modifiers);
    }

    public void KeyUp(ConsoleKey key, ConsoleModifiers modifiers) => _pressed.Remove((key, modifiers));

    public (ConsoleKey Key, ConsoleModifiers Modifiers) LastPressed()
        => _last ?? (default(ConsoleKey), default(ConsoleModifiers));

    public void Poll()
    {
        // No-op for the fake. Real implementations may update internal state here.
    }

    // helper for tests
    public void SetClose(bool value) => Close = value;
}
