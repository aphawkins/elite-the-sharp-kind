// 'Useful Libraries' - Andy Hawkins 2023-2026.

namespace Useful.Assets;

//// JSON serializable

// Per-tier filename replacements, for the cases where a tier's set genuinely
// differs from the others - a merged sprite sheet, or a bitmap with no
// equivalent at that tier. Logical names not mentioned here keep the
// manifest's own filename.
public class AssetTierOverride
{
    public string Palette { get; set; } = string.Empty;

    public Dictionary<string, string> Images { get; init; } = [];

    public Dictionary<string, string> FontsBitmap { get; init; } = [];
}
