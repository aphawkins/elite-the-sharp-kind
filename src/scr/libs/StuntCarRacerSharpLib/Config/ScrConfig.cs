// 'Stunt Car Racer - The Sharp Kind' - Andy Hawkins 2026.
// 'Stunt Car Racer Remake' - sourceforge.net/projects/stuntcarremake.
// Stunt Car Racer (C) Geoff Crammond / MicroStyle / MicroProse 1989.

using Useful.Abstraction.Config;

namespace StuntCarRacerSharpLib.Config;

// The root of stuntcarracer.sharp: shared engine settings plus the game's own.
internal sealed class ScrConfig : ConfigSettings<ScrConfigSettings>;
