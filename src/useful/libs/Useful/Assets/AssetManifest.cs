// 'Useful Libraries' - Andy Hawkins 2025.

using System.Diagnostics.CodeAnalysis;

namespace Useful.Assets;

//// JSON serializable

[SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "Need to be writable for serialisation.")]
public class AssetManifest
{
    public string Palette { get; set; } = string.Empty;

    public Dictionary<string, string> Images { get; set; } = [];

    public Dictionary<string, string> Sfx { get; set; } = [];

    public Dictionary<string, string> Music { get; set; } = [];

    public Dictionary<string, string> FontsBitmap { get; set; } = [];

    public Dictionary<string, string> FontsTrueType { get; set; } = [];

    public Dictionary<string, string> Models { get; set; } = [];
}
