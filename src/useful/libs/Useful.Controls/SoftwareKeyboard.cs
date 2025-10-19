// 'Useful Libraries' - Andy Hawkins 2025.

namespace Useful.Controls;

public class SoftwareKeyboard : IKeyboard
{
    public bool Close { get; }

    public void ClearPressed()
    {
    }

    public bool IsPressed(ConsoleKey key) => false;

    public bool IsPressed(ConsoleModifiers modifiers) => false;

    public void KeyDown(ConsoleKey key, ConsoleModifiers modifiers)
    {
    }

    public void KeyUp(ConsoleKey key, ConsoleModifiers modifiers)
    {
    }

    public (ConsoleKey Key, ConsoleModifiers Modifiers) LastPressed() => (ConsoleKey.None, ConsoleModifiers.None);

    public void Poll()
    {
    }
}
