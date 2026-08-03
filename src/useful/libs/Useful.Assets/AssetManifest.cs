// 'Useful Libraries' - Andy Hawkins 2023-2026.

namespace Useful.Assets;

//// JSON serializable

public class AssetManifest
{
    // What this rendition says its own colours are limited to, checked at
    // load. A rendition that declares nothing is unconstrained.
    public AssetColourLimits Colours { get; set; } = new();

    public string Palette { get; set; } = string.Empty;

    public Dictionary<string, string> Images { get; init; } = [];

    public Dictionary<string, string> Sfx { get; init; } = [];

    public Dictionary<string, string> Music { get; init; } = [];

    public Dictionary<string, string> SoundFonts { get; init; } = [];

    public Dictionary<string, BitmapFontEntry> FontsBitmap { get; init; } = [];

    public Dictionary<string, TrueTypeFontEntry> FontsTrueType { get; init; } = [];

    public Dictionary<string, string> Models { get; init; } = [];
}
