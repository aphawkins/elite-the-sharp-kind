// 'Useful Libraries' - Andy Hawkins 2023-2026.

using System.Collections.ObjectModel;

namespace Useful.Assets;

//// JSON serializable

public class AssetManifest
{
    // Which tiers this game actually ships assets for. Asking for a tier
    // that isn't listed fails at startup rather than at first draw.
    public Collection<SystemTier> Tiers { get; init; } = [];

    public Dictionary<SystemTier, AssetTierOverride> TierOverrides { get; init; } = [];

    public string Palette { get; set; } = string.Empty;

    public Dictionary<string, string> Images { get; init; } = [];

    public Dictionary<string, string> Sfx { get; init; } = [];

    public Dictionary<string, string> Music { get; init; } = [];

    public Dictionary<string, string> SoundFonts { get; init; } = [];

    public Dictionary<string, BitmapFontEntry> FontsBitmap { get; init; } = [];

    public Dictionary<string, TrueTypeFontEntry> FontsTrueType { get; init; } = [];

    public Dictionary<string, string> Models { get; init; } = [];
}
