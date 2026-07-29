// 'Useful Libraries' - Andy Hawkins 2023-2026.

using Useful.Assets;

namespace Useful.Abstraction.Config;

/// <summary>
/// Settings shared by every game, stored under the config file's <c>engine</c> element.
/// Game-specific settings live alongside it under <c>game</c>; see
/// <see cref="ConfigSettings{TGameSettings}"/>.
/// </summary>
public sealed class EngineConfigSettings
{
    public bool EffectsOn { get; set; } = true;

    // Maximum render frame rate. The game speed is independent of it.
    public float Fps { get; set; } = 60f;

    // Which IAbstraction backend renders/plays the game: Software (default)
    // or Hardware (SDL-accelerated).
    public GraphicsBackend GraphicsBackend { get; set; } = GraphicsBackend.Software;

    public bool MusicOn { get; set; } = true;

    // Which machine's look the game reproduces: picks the asset set and,
    // with it, the render resolution and scale. See docs/asset-structure.md.
    public SystemTier Tier { get; set; } = SystemTier.SixteenBit;
}
