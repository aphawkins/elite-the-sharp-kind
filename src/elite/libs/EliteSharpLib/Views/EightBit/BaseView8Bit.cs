// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using EliteSharpLib.Graphics;
using Useful;
using Useful.Graphics;

namespace EliteSharpLib.Views.EightBit;

/// <summary>
/// The 8-bit tier's shared chrome and text helpers, authored for the 320x256
/// canvas and its fixed 8x8 font. The 8-bit palette is the machine's 16
/// colours under their web names, so there is no Gold here: the header is
/// Yellow, as the hardware this stands in for would have had it.
/// </summary>
internal class BaseView8Bit : IBaseView
{
    // The 8-bit font is a monospaced 8x8 sheet, so a row is 8 pixels and a
    // character is 8 wide - both fixed, unlike the 16-bit proportional font.
    private const int CharacterWidth = 8;
    private const int RowHeight = 8;

    private readonly FastColor _colorWhite;
    private readonly FastColor _colorYellow;

    internal BaseView8Bit(IEliteDraw draw)
    {
        ArgumentNullException.ThrowIfNull(draw);

        Graphics = draw.Graphics;
        Layout = draw.Layout;
        _colorWhite = draw.Palette["White"];
        _colorYellow = draw.Palette["Yellow"];
    }

    public IGraphics Graphics { get; }

    public ViewLayout Layout { get; }

    public void DrawBorder()
    {
        for (int i = 0; i < Layout.BorderWidth; i++)
        {
            Graphics.DrawRectangle(
                new(i, i),
                Layout.ScreenWidth - 1 - (2 * i),
                Layout.Bottom - (2 * i),
                _colorWhite);
        }
    }

    public void DrawHyperspaceCountdown(int countdown)
        => Graphics.DrawTextRight(
            new(Layout.Left + 11, Layout.Top + 2),
            $"{countdown}",
            nameof(FontType.Small),
            _colorWhite);

    public void DrawViewHeader(string title)
    {
        Graphics.DrawTextCentre(Layout.Top + 3, title, nameof(FontType.Large), _colorYellow);
        Graphics.DrawLine(new(Layout.Left, 18), new(Layout.Right, 18), _colorWhite);

        // Vertical lines
        Graphics.DrawLine(new(Layout.ScannerLeft, Layout.Top + 19), new(Layout.ScannerLeft, Layout.ScannerTop), _colorYellow);
        Graphics.DrawLine(new(Layout.ScannerRight, Layout.Top + 19), new(Layout.ScannerRight, Layout.ScannerTop), _colorYellow);
    }

    public void DrawTextPretty(Vector2 position, float width, string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        int i = 0;
        float maxlen = width / CharacterWidth;
        int previous = i;

        while (i < text.Length)
        {
            i += (int)maxlen;
            i = Math.Clamp(i, 0, text.Length - 1);
            int breakPoint = i;

            while (i > previous && text[i] is not ' ' and not ',' and not '.')
            {
                i--;
            }

            // No space/comma/period found within the line width: hard-break the word.
            i = i > previous ? i + 1 : breakPoint + 1;

            Graphics.DrawTextLeft(position, text[previous..i], nameof(FontType.Small), _colorWhite);
            previous = i;
            position.Y += RowHeight;
        }
    }
}
