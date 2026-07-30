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
    /// Gets the width of the frame drawn around the view area.
    /// </summary>
    public float BorderWidth { get; } = 1;

    public float Left => BorderWidth;

    public float Top => BorderWidth;

    public float Right => ScreenWidth - BorderWidth;

    public float Bottom => ScreenHeight - ScannerSize.Y;

    public Vector2 Centre => new(ScreenWidth / 2, (ScannerTop / 2) + BorderWidth);

    public float ScannerTop => ScreenHeight - ScannerSize.Y;

    public float ScannerLeft => Centre.X - (ScannerSize.X / 2);

    public float ScannerRight => ScannerLeft + ScannerSize.X - 1;

    /// <summary>
    /// Gets the x of the scanner's left edge, which the absolute-positioned
    /// screens lay out from so their content stays with the HUD rather than
    /// with the screen edge as the tier widens.
    /// </summary>
    public float Offset => ScannerLeft;

    // DrawBorder's rectangle draws its far edge at position+size-1 (last
    // inclusive pixel), one short of Right/Bottom, so the view clip must stop
    // one pixel earlier still or content lands on top of the border line
    // itself instead of stopping short of it.
    public float Height => Bottom - BorderWidth - 1;

    public float Width => ScreenWidth - (2 * BorderWidth) - 1;
}
