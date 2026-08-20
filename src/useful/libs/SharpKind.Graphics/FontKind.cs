// 'SharpKind Libraries' - Andy Hawkins 2023-2026.

namespace SharpKind.Graphics;

/// <summary>
/// Which kind of font the game draws its text with. Both backends honour
/// this, so the choice changes how the text looks and not which machine is
/// drawing it.
/// </summary>
public enum FontKind
{
    /// <summary>
    /// The rendition's own font sheets - the artwork it ships, and the look
    /// each rendition was drawn for.
    /// </summary>
    Bitmap = 0,

    /// <summary>
    /// A Windows .fon bitmap font. Falls back to <see cref="Bitmap"/> in a
    /// rendition that declares none.
    /// </summary>
    Fon = 1,

    /// <summary>
    /// A TrueType face, rendered without antialiasing. Falls back to
    /// <see cref="Bitmap"/> in a rendition that declares none.
    /// </summary>
    TrueType = 2,
}
