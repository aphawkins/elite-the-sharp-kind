// 'SharpKind Libraries' - Andy Hawkins 2023-2026.

namespace SharpKind.Assets;

//// JSON serializable

// A Windows .fon font's declaration. The file is a container: it may hold
// several strikes of the same face, each rasterised for one size, so the
// height wanted has to be named alongside it. Nothing else about the layout
// is declared - unlike a bitmap sheet, a FON carries its own cell metrics.
public class FonFontEntry
{
    public string File { get; set; } = string.Empty;

    // The strike to use, in pixels. The nearest strike the file holds is
    // taken, since a bitmap face has only the sizes it was drawn at.
    public int PixelHeight { get; set; }
}
