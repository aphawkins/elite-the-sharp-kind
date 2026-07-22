// 'Useful Libraries' - Andy Hawkins 2025.

namespace Useful.Controls;

// One entry in a scripted input timeline, e.g. "S at tick 2" is
// new(2, ConsoleKey.S, KeyScriptAction.Tap) and "hold accelerate from tick
// 10" is new(10, ConsoleKey.UpArrow, KeyScriptAction.Hold). SaveFrame events
// carry ConsoleKey.None; only the tick matters.
public readonly record struct KeyScriptEvent(
    int Tick,
    ConsoleKey Key,
    KeyScriptAction Action,
    ConsoleModifiers Modifiers = ConsoleModifiers.None);
