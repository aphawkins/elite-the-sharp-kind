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
    private readonly IEliteDraw _draw;

    internal BaseView8Bit(IEliteDraw draw)
    {
        ArgumentNullException.ThrowIfNull(draw);

        _draw = draw;
        Graphics = draw.Graphics;
        Layout = draw.Layout;
        _colorWhite = draw.Palette["White"];
        _colorYellow = draw.Palette["Yellow"];
    }

    public IGraphics Graphics { get; }

    public ViewLayout Layout { get; }

    // The border frames the viewport, so it lies outside it: the screens all
    // draw inside the view clip region, and this has to step out of it and
    // back for its own rectangle to survive.
    public void DrawBorder()
    {
        _draw.SetFullScreenClipRegion();

        for (int i = 0; i < Layout.BorderWidth; i++)
        {
            Graphics.DrawRectangle(
                new(i, i),
                Layout.ScreenWidth - (2 * i),
                Layout.ScannerTop - (2 * i),
                _colorWhite);
        }

        _draw.SetViewClipRegion();
    }

    // Right-aligned, so the text's own width sets where it starts rather than
    // this having to estimate it from the 8x8 font.
    public void DrawFps(int fps)
        => Graphics.DrawTextRight(
            new(Layout.ViewportRight - 1, Layout.ViewportTop + 1),
            $"FPS: {fps}",
            nameof(FontType.Small),
            _colorWhite);

    public void DrawHyperspaceCountdown(int countdown)
        => Graphics.DrawTextRight(
            new(Layout.ViewportLeft + 16, Layout.ViewportTop + 1),
            $"{countdown}",
            nameof(FontType.Small),
            _colorWhite);

    public void DrawViewHeader(string title)
    {
        Graphics.DrawTextCentre(Layout.ViewportTop + 1, title, nameof(FontType.Large), _colorYellow);
        Graphics.DrawLine(
            new(Layout.ViewportLeft, Layout.ViewportTop + 9),
            new(Layout.ViewportRight, Layout.ViewportTop + 9),
            _colorWhite);
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
