// 'SharpKind Libraries' - Andy Hawkins 2023-2026.

namespace SharpKind.Assets;

/// <summary>
/// One strike of a Windows bitmap font: the glyphs drawn at a single pixel
/// height. A .fon file may hold several, since a bitmap face cannot be scaled
/// and so has to be drawn again at every size it is wanted at.
/// </summary>
public sealed class FonFont
{
    private readonly FonGlyph[] _glyphs;

    /// <summary>
    /// Initializes a new instance of the <see cref="FonFont"/> class, from its
    /// glyphs, one per character from
    /// <paramref name="firstChar"/> to <paramref name="lastChar"/> inclusive.
    /// </summary>
    /// <param name="pixelHeight">The height every glyph is drawn at.</param>
    /// <param name="maxWidth">The width of the widest glyph.</param>
    /// <param name="firstChar">The first character the strike covers.</param>
    /// <param name="lastChar">The last character the strike covers.</param>
    /// <param name="isProportional">Whether the glyphs vary in width.</param>
    /// <param name="glyphs">One glyph per character in the range, in order.</param>
    public FonFont(int pixelHeight, int maxWidth, char firstChar, char lastChar, bool isProportional, FonGlyph[] glyphs)
    {
        ArgumentNullException.ThrowIfNull(glyphs);

        // A strike that covers a range has a glyph for every character in it.
        // Anything else would leave Glyph() returning a hole for a character
        // the font said it holds.
        if (lastChar < firstChar || glyphs.Length != lastChar - firstChar + 1)
        {
            throw new SharpKindException(
                $"A strike covering {(int)firstChar}-{(int)lastChar} needs that many {nameof(glyphs)}, not {glyphs.Length}.");
        }

        PixelHeight = pixelHeight;
        MaxWidth = maxWidth;
        FirstChar = firstChar;
        LastChar = lastChar;
        IsProportional = isProportional;
        _glyphs = glyphs;
    }

    public int PixelHeight { get; }

    // The widest glyph in the strike. A fixed-pitch strike's glyphs are all
    // this wide; a proportional one's are at most.
    public int MaxWidth { get; }

    public char FirstChar { get; }

    public char LastChar { get; }

    public bool IsProportional { get; }

    /// <summary>
    /// The glyph for <paramref name="letter"/>, or <see langword="null"/> when
    /// the strike does not cover it. A font declares the range it holds, and a
    /// character outside it has no glyph to fall back on - what to draw
    /// instead is the caller's decision, not the file's.
    /// </summary>
    public FonGlyph? Glyph(char letter)
        => letter < FirstChar || letter > LastChar ? null : _glyphs[letter - FirstChar];
}
