// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;

namespace EliteSharpLib.Views;

/// <summary>
/// The tier's screen metrics, in one place, for the views to lay out against.
/// Everything but the four inputs is derived rather than stored: a screen
/// size, the scanner art's size and the coordinate scale determine the rest,
/// so no two of these can disagree.
/// <para>
/// The screen is two regions stacked: the <b>viewport</b> - everything above
/// the scanner, framed by the border, where the universe and every screen's
/// content is drawn - and the scanner below it. The viewport members describe
/// the drawable interior, inside the border; the Scanner members describe the
/// HUD art. Nothing is expressed relative to the other region.
/// </para>
/// </summary>
/// <param name="ScreenWidth">The render width in pixels.</param>
/// <param name="ScreenHeight">The render height in pixels.</param>
/// <param name="ScannerSize">
/// The scanner bitmap's size. Taken from the art rather than hardcoded, so
/// each tier's scanner defines its own HUD height and width (the 8-bit
/// scanner is 320x56 against the 16-bit 512x129).
/// </param>
/// <param name="Scale">
/// Elite's coordinate scale: the game's drawing maths is written in the
/// original's 256x256-ish space and multiplied up to the render resolution.
/// Kept whole, per the pixel-doubling rule in docs/decisions.md - a
/// fractional value would put HUD text and ship vertices on half-pixels.
/// </param>
internal sealed record ViewLayout(float ScreenWidth, float ScreenHeight, Vector2 ScannerSize, float Scale)
{
    /// <summary>
    /// Gets the width of the frame drawn around the viewport.
    /// </summary>
    public float BorderWidth { get; } = 1;

    /// <summary>
    /// Gets the y of the scanner's top edge, which is also the row the
    /// border's bottom edge is drawn on.
    /// </summary>
    public float ScannerTop => ScreenHeight - ScannerSize.Y;

    public float ScannerLeft => ViewportCentre.X - (ScannerSize.X / 2);

    public float ScannerRight => ScannerLeft + ScannerSize.X - 1;

    // The viewport interior is half-open: Left/Top are the first drawable
    // pixel inside the border, Right/Bottom are one past the last. The border
    // itself occupies x = 0 and x = ScreenWidth - 1, y = 0 and y = ScannerTop
    // - 1, so nothing drawn within these bounds can land on it.
    public float ViewportLeft => BorderWidth;

    public float ViewportTop => BorderWidth;

    public float ViewportRight => ScreenWidth - BorderWidth;

    public float ViewportBottom => ScannerTop - BorderWidth;

    public float ViewportWidth => ViewportRight - ViewportLeft;

    public float ViewportHeight => ViewportBottom - ViewportTop;

    public Vector2 ViewportCentre => new((ViewportLeft + ViewportRight) / 2, (ViewportTop + ViewportBottom) / 2);
}
