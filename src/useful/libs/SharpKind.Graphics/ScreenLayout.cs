// 'SharpKind Libraries' - Andy Hawkins 2023-2026.

using System.Numerics;

namespace SharpKind.Graphics;

/// <summary>
/// The render target's size, for callers that lay out against the screen.
/// It is not on <see cref="IGraphics"/>, which is a set of drawing
/// operations: where a thing goes is the caller's decision, and a caller
/// that needs the screen's extent to make it says so by asking for this.
/// </summary>
/// <param name="ScreenWidth">The native render width in pixels.</param>
/// <param name="ScreenHeight">The native render height in pixels.</param>
public record ScreenLayout(float ScreenWidth, float ScreenHeight)
{
    public Vector2 ScreenSize => new(ScreenWidth, ScreenHeight);

    public Vector2 ScreenCentre => new(ScreenWidth / 2, ScreenHeight / 2);
}
