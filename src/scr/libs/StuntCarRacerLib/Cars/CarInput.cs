// 'Stunt Car Racer - The Sharp Kind' - Andy Hawkins 2026.
// 'Stunt Car Racer Remake' - sourceforge.net/projects/stuntcarremake.
// Stunt Car Racer (C) Geoff Crammond / MicroStyle / MicroProse 1989.

namespace StuntCarRacerLib.Cars;

// Player control flags, following ptitSeb's stuntcarremake KEY_P1_*
// definitions (which replaced the original fluffyfreak remake's combined
// accelerate+boost/brake+boost keys with independent ones). Keyboard
// controls: Left/Right arrows = steer, Up = accelerate, Down = brake,
// Space = boost (applies with either accelerate or brake held).
[Flags]
public enum CarInput
{
    None = 0,

    Left = 1,

    Right = 2,

    Accelerate = 4,

    Brake = 8,

    Boost = 16,

    // Convenience combinations for driving tests ("floor it").
    AccelBoost = Accelerate | Boost,

    BrakeBoost = Brake | Boost,
}
