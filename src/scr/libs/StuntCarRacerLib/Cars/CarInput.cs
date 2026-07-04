// 'Stunt Car Racer - The Sharp Kind' - Andy Hawkins 2026.
// 'Stunt Car Racer Remake' - sourceforge.net/projects/stuntcarremake.
// Stunt Car Racer (C) Geoff Crammond / MicroStyle / MicroProse 1989.

namespace StuntCarRacerLib.Cars;

// Player control flags, following the original KEY_P1_* definitions.
// Keyboard controls in the original: S = left, D = right,
// RETURN = accelerate + boost, SPACE = brake/reverse + boost,
// HASH = brake/reverse.
[Flags]
public enum CarInput
{
    None = 0,

    Left = 1,

    Right = 2,

    // Brake/reverse (original HASH key).
    Hash = 4,

    // Brake/reverse plus boost (original SPACE key).
    BrakeBoost = 8,

    // Accelerate plus boost (original RETURN key).
    AccelBoost = 16,
}
