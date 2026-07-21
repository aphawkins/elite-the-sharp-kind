// 'Useful Libraries' - Andy Hawkins 2025.

namespace Useful.Controls;

/// <summary>
/// Producer-facing side of the keyboard: written to by input backends (e.g.
/// <c>SDLInput</c>) as raw key events arrive. Game code should depend on
/// <see cref="IKeyboard"/> instead, so it can't inject key events.
/// </summary>
public interface IKeyboardSink
{
    public bool Close { get; set; }

    public void KeyDown(ConsoleKey key, ConsoleModifiers modifiers);

    public void KeyUp(ConsoleKey key, ConsoleModifiers modifiers);
}
