// 'Stunt Car Racer - The Sharp Kind' - Andy Hawkins 2026.
// 'Stunt Car Racer Remake' - sourceforge.net/projects/stuntcarremake.
// Stunt Car Racer (C) Geoff Crammond / MicroStyle / MicroProse 1989.

namespace StuntCarRacerLib.Tests;

// One entry in a scripted input timeline, e.g. "S at tick 2" is
// new(2, ConsoleKey.S, KeyScriptAction.Tap) and "hold accelerate from tick
// 10" is new(10, ConsoleKey.UpArrow, KeyScriptAction.Hold).
internal readonly record struct KeyScriptEvent(
    int Tick,
    ConsoleKey Key,
    KeyScriptAction Action,
    ConsoleModifiers Modifiers = ConsoleModifiers.None);
