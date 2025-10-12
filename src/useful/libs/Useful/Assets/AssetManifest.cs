// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

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
