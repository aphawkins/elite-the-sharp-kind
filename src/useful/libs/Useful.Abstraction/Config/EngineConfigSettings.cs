// 'Useful Libraries' - Andy Hawkins 2023-2026.

using Useful.Assets;

namespace Useful.Abstraction.Config;

/// <summary>
/// Settings shared by every game, stored under the config file's <c>engine</c>
/// element. Graphics and sound have a group each; what's left at the top sits
/// across both. Game-specific settings live alongside under <c>game</c>; see
/// <see cref="ConfigSettings{TGameSettings}"/>.
/// </summary>
public sealed class EngineConfigSettings
{
    // Past this the window is larger than any display the game could be
    // shown on, so it is a typo rather than an intention.
    private const int MaxWindowScale = 4;

    private const int DefaultWindowScale = 1;

    // Which IAbstraction runs the game: Software (default) or Hardware
    // (SDL-accelerated). It picks the mixer as well as the rasteriser, which
    // is why it sits here rather than under Graphics.
    public Backend Backend { get; set; } = Backend.Software;

    public GraphicsConfigSettings Graphics { get; set; } = new();

    public SoundConfigSettings Sound { get; set; } = new();

    // Which machine's look the game reproduces: picks the asset set and,
    // with it, the render resolution and scale. The asset set covers music
    // and effects as well as the artwork, so this isn't graphics-only
    // either. See docs/asset-structure.md.
    public SystemTier Tier { get; set; } = SystemTier.SixteenBit;

    // How many window pixels each rendered pixel occupies. Independent of
    // Tier: the game always renders at the tier's native resolution and is
    // magnified only at presentation, so scale 2 fills a window twice the
    // size with the same pixels doubled rather than with more detail.
    // Integer only - a fractional scale cannot double pixels evenly.
    public int WindowScale { get; set; } = DefaultWindowScale;

    /// <summary>
    /// Replaces any engine value that cannot be honoured with its default, in
    /// place. Every game validates the same way through this, rather than each
    /// one repeating (or, as Stunt Car Racer used to, skipping) the checks.
    /// </summary>
    /// <returns><see langword="true"/> if anything had to be replaced.</returns>
    public bool Repair()
    {
        bool repaired = false;

        if (!Enum.IsDefined(Backend))
        {
            Backend = Backend.Software;
            repaired = true;
        }

        if (!Enum.IsDefined(Tier))
        {
            Tier = SystemTier.SixteenBit;
            repaired = true;
        }

        if (WindowScale is < 1 or > MaxWindowScale)
        {
            WindowScale = DefaultWindowScale;
            repaired = true;
        }

        return Graphics.Repair() || repaired;
    }
}
