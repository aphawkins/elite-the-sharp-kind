// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using EliteSharpLib.Graphics;
using Useful.Graphics;

namespace EliteSharpLib.Views.SixteenBit;

/// <summary>
/// The 16-bit tier's shared chrome and text helpers. Keeps the spacing the
/// 16-bit screens have always used, against its proportional font and the
/// tier's larger canvas.
/// </summary>
internal class BaseView16Bit : IBaseView
{
    // The 16-bit font is proportional, so this is the nominal character width
    // the word-wrap estimates against rather than a measured one.
    private const int CharacterWidth = 8;

    private readonly uint _colorGold;
    private readonly uint _colorWhite;
    private readonly uint _colorYellow;
    private readonly float _rowHeight;

    internal BaseView16Bit(IEliteDraw draw)
    {
        ArgumentNullException.ThrowIfNull(draw);

        Graphics = draw.Graphics;
        Layout = draw.Layout;
        _rowHeight = 8 * draw.Layout.Scale;
        _colorGold = draw.Palette["Gold"];
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
            new(Layout.Left + 21, Layout.Top + 4),
            $"{countdown}",
            nameof(FontType.Small),
            _colorWhite);

    public void DrawViewHeader(string title)
    {
        Graphics.DrawTextCentre(Layout.Top + 6, title, nameof(FontType.Large), _colorGold);
        Graphics.DrawLine(new(Layout.Left, 36), new(Layout.Right, 36), _colorWhite);

        // Vertical lines
        Graphics.DrawLine(new(Layout.ScannerLeft, Layout.Top + 37), new(Layout.ScannerLeft, Layout.ScannerTop), _colorYellow);
        Graphics.DrawLine(new(Layout.ScannerRight, Layout.Top + 37), new(Layout.ScannerRight, Layout.ScannerTop), _colorYellow);
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
            position.Y += _rowHeight;
        }
    }
}
