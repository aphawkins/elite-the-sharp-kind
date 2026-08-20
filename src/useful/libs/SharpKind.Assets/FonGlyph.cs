// 'SharpKind Libraries' - Andy Hawkins 2023-2026.

namespace SharpKind.Assets;

/// <summary>
/// One glyph of a <see cref="FonFont"/> strike: its advance width and its ink,
/// decoded out of the column-major packing the FNT format stores it in and
/// into a pixel per element, so a consumer never has to know that packing.
/// </summary>
public sealed class FonGlyph
{
    private readonly bool[] _ink;

    /// <summary>
    /// Initializes a new instance of the <see cref="FonGlyph"/> class, from its
    /// ink, a pixel per element, row by row.
    /// </summary>
    /// <param name="width">The glyph's width in pixels.</param>
    /// <param name="height">The glyph's height in pixels.</param>
    /// <param name="ink">True where the glyph has ink, row-major, width * height long.</param>
    public FonGlyph(int width, int height, bool[] ink)
    {
        ArgumentNullException.ThrowIfNull(ink);

        if (ink.Length != width * height)
        {
            throw new SharpKindException($"A {width}x{height} glyph needs {width * height} pixels, not {ink.Length}.");
        }

        Width = width;
        Height = height;
        _ink = ink;
    }

    public int Width { get; }

    public int Height { get; }

    // True where the glyph has ink. A FON glyph is one bit per pixel - there
    // is no antialiasing to carry - so the colour is the caller's to choose.
    public bool this[int x, int y] => _ink[(y * Width) + x];
}
