// 'Useful Libraries' - Andy Hawkins 2025.

namespace Useful.Controls;

public interface IKeyboard
{
    public bool Close { get; set; }

    public void ClearPressed();

    public bool IsPressed(ConsoleKey key);

    public bool IsPressed(ConsoleModifiers modifiers);

    public void KeyDown(ConsoleKey key, ConsoleModifiers modifiers);

    public void KeyUp(ConsoleKey key, ConsoleModifiers modifiers);

    public (ConsoleKey Key, ConsoleModifiers Modifiers) LastPressed();

    public void Poll();
}
