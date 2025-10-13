// 'Useful Libraries' - Andy Hawkins 2025.

namespace Useful.Assets;

public class AssetManifest
{
#pragma warning disable CA2227 // Collection properties should be read only
    public Dictionary<string, string> Images { get; set; } = [];

    public Dictionary<string, string> Sfx { get; set; } = [];

    public Dictionary<string, string> Music { get; set; } = [];

    public Dictionary<string, string> FontsBitmap { get; set; } = [];

    public Dictionary<string, string> FontsTrueType { get; set; } = [];
}
