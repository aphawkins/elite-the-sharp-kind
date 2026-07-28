// 'Useful Libraries' - Andy Hawkins 2023-2026.

using Useful.Assets;

namespace Useful.Abstraction.Config;

/// <summary>
/// Settings shared by every game's config file. Game-specific config types derive from
/// this and add their own properties.
/// </summary>
public abstract class BaseConfigSettings
{
    public bool EffectsOn { get; set; } = true;

    // Which IAbstraction backend renders/plays the game: Software (default)
    // or Hardware (SDL-accelerated).
    public GraphicsBackend GraphicsBackend { get; set; } = GraphicsBackend.Software;

    public bool MusicOn { get; set; } = true;

    // Which machine's look the game reproduces: picks the asset set and,
    // with it, the render resolution and scale. See docs/asset-structure.md.
    public SystemTier Tier { get; set; } = SystemTier.SixteenBit;
}
