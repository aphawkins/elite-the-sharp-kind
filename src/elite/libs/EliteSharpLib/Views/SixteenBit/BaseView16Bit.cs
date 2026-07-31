// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using EliteSharpLib.Graphics;
using Useful;
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

    // The frame drawn around the viewport, overlaying its outer pixel rather
    // than being reserved out of it.
    private const int BorderWidth = 1;

    private readonly FastColor _colorGoldenrod;
    private readonly FastColor _colorWhite;
    private readonly float _rowHeight;

    internal BaseView16Bit(IEliteDraw draw)
    {
        ArgumentNullException.ThrowIfNull(draw);

        Graphics = draw.Graphics;
        Layout = draw.Layout;
        _rowHeight = 8 * draw.Layout.Scale;
        _colorGoldenrod = draw.Palette["Goldenrod"];
        _colorWhite = draw.Palette["White"];
    }

    public IGraphics Graphics { get; }

    public ViewLayout Layout { get; }

    // Drawn over the viewport's own edge, so it needs no clip juggling: the
    // rectangle is the viewport, and the view clip region already admits it.
    public void DrawBorder()
    {
        for (int i = 0; i < BorderWidth; i++)
        {
            Graphics.DrawRectangle(
                new(Layout.ViewportLeft + i, Layout.ViewportTop + i),
                Layout.ViewportWidth - (2 * i),
                Layout.ViewportHeight - (2 * i),
                _colorWhite);
        }
    }

    // Right-aligned, so the proportional font's width never has to be
    // estimated to place this.
    public void DrawFps(int fps)
        => Graphics.DrawTextRight(
            new(Layout.ViewportRight - 4, Layout.ViewportTop + 4),
            $"FPS: {fps}",
            nameof(FontType.Small),
            _colorWhite);

    public void DrawHyperspaceCountdown(int countdown)
        => Graphics.DrawTextRight(
            new(Layout.ViewportLeft + 21, Layout.ViewportTop + 4),
            $"{countdown}",
            nameof(FontType.Small),
            _colorWhite);

    public void DrawInfoMessage(string message)
        => Graphics.DrawTextCentre(
            Layout.ViewportHeight - 40,
            message,
            nameof(FontType.Small),
            _colorWhite);

    public void DrawViewHeader(string title)
    {
        Graphics.DrawTextCentre(Layout.ViewportTop + 6, title, nameof(FontType.Large), _colorGoldenrod);
        Graphics.DrawLine(
            new(Layout.ViewportLeft, Layout.ViewportTop + 35),
            new(Layout.ViewportRight, Layout.ViewportTop + 35),
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
            position.Y += _rowHeight;
        }
    }
}
