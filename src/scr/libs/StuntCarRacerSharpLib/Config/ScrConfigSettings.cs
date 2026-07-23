// 'Stunt Car Racer - The Sharp Kind' - Andy Hawkins 2026.
// 'Stunt Car Racer Remake' - sourceforge.net/projects/stuntcarremake.
// Stunt Car Racer (C) Geoff Crammond / MicroStyle / MicroProse 1989.

using Useful.Abstraction;

namespace StuntCarRacerSharpLib.Config;

internal sealed class ScrConfigSettings
{
    public bool EffectsOn { get; set; } = true;

    // Which IAbstraction backend renders/plays the game: Software (default)
    // or Hardware (SDL-accelerated).
    public GraphicsBackend GraphicsBackend { get; set; } = GraphicsBackend.Software;

    public bool MusicOn { get; set; } = true;
}
