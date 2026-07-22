// 'Stunt Car Racer - The Sharp Kind' - Andy Hawkins 2026.
// 'Stunt Car Racer Remake' - sourceforge.net/projects/stuntcarremake.
// Stunt Car Racer (C) Geoff Crammond / MicroStyle / MicroProse 1989.

namespace StuntCarRacerLib.Tests;

// Whether a scripted key event presses and releases within its one tick, or
// starts/ends a hold spanning several ticks.
internal enum KeyScriptAction
{
    Tap = 0,
    Hold = 1,
    Release = 2,
}
