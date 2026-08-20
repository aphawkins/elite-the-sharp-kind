// 'SharpKind Libraries' - Andy Hawkins 2023-2026.

namespace SharpKind.Assets;

//// JSON serializable

// The fonts an asset set declares, grouped by the kind of font they are, and
// keyed within each kind by the font types the game asks for - Small and
// Large. The three names match the FontKind members the engine setting
// chooses between, so a manifest section and a setting value read the same.
//
// A kind a set declares nothing for is simply absent: only the sheets are
// certain to exist, since they are what a rendition ships.
public class FontManifest
{
    public Dictionary<string, BitmapFontEntry> Bitmap { get; init; } = [];

    public Dictionary<string, FonFontEntry> Fon { get; init; } = [];

    public Dictionary<string, TrueTypeFontEntry> TrueType { get; init; } = [];
}
