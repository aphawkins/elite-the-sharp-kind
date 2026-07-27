// 'Useful Libraries' - Andy Hawkins 2023-2026.

namespace Useful.Assets;

//// JSON serializable

public class AssetManifest
{
    public string Palette { get; set; } = string.Empty;

    public Dictionary<string, string> Images { get; init; } = [];

    public Dictionary<string, string> Sfx { get; init; } = [];

    public Dictionary<string, string> Music { get; init; } = [];

    public Dictionary<string, string> SoundFonts { get; init; } = [];

    public Dictionary<string, string> FontsBitmap { get; init; } = [];

    public Dictionary<string, string> FontsTrueType { get; init; } = [];

    public Dictionary<string, string> Models { get; init; } = [];
}
