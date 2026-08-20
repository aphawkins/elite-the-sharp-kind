// 'SharpKind Libraries' - Andy Hawkins 2023-2026.

using System.Numerics;

namespace SharpKind.Graphics;

/// <summary>
/// Turns a string into pixels. Every font kind the engine supports - bitmap
/// sheets, Windows .fon strikes, TrueType faces - arrives through this, and
/// every backend draws text by presenting what it returns: the software
/// renderer blits the bitmap, the hardware one uploads it as a texture.
/// <para>
/// That is what keeps the two backends drawing the same text. They used to
/// rasterise separately - sheets in software, TrueType in hardware - so the
/// same game on the same rendition read differently depending on which was
/// running. Sharing the rasteriser makes matching output structural rather
/// than something two code paths have to be kept agreeing on.
/// </para>
/// </summary>
public interface IFontRasteriser
{
    /// <summary>
    /// Draws <paramref name="text"/> into a new bitmap, sized to the text and
    /// transparent where there is no ink. The caller owns the result and
    /// disposes it - both backends cache what they get back, and they cache
    /// it in different forms.
    /// </summary>
    /// <param name="text">The text to draw.</param>
    /// <param name="fontType">Which of the rendition's fonts to draw it in.</param>
    /// <param name="color">The colour the ink takes.</param>
    /// <returns>A new bitmap holding the drawn text.</returns>
    public FastBitmap Rasterise(string text, string fontType, FastColor color);

    /// <summary>
    /// The size <paramref name="text"/> would occupy, without drawing it.
    /// Whitespace-only text measures as the font's line height by zero width,
    /// matching what <see cref="Rasterise"/> would put on screen for it.
    /// </summary>
    /// <param name="text">The text to measure.</param>
    /// <param name="fontType">Which of the rendition's fonts to measure it in.</param>
    /// <returns>The width and height the text would occupy.</returns>
    public Vector2 Measure(string text, string fontType);
}
