// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using EliteSharpLib.Graphics;
using Useful;
using Useful.Graphics.Rendering;

namespace EliteSharpLib.Lasers;

/// <summary>
/// The laser beams and crosshairs. The geometry is shared - it is already
/// derived from the tier's scale and scanner edges - but the beam colours are
/// each tier's own, so those are left to the subclass.
/// </summary>
internal abstract class LaserDrawBase(GameState gameState, IEliteDraw draw, RNG rng)
{
    protected IEliteDraw Draw { get; } = draw;

    internal void DrawLaserLines(LaserType laserType)
    {
        FastColor color = BeamColor(laserType);
        float scale = Draw.Layout.Scale;

        Vector2 target = new()
        {
            X = Draw.Layout.Centre.X + (rng.Random(0, 2) * scale),
            Y = Draw.Layout.Centre.Y + (rng.Random(0, 2) * scale),
        };

        Vector2 leftA = new(Draw.Layout.ScannerLeft + (32 * scale), Draw.Layout.Bottom);
        Vector2 leftB = new(Draw.Layout.ScannerLeft + (48 * scale), Draw.Layout.Bottom);

        Vector2 rightA = new(Draw.Layout.ScannerRight - (32 * scale), Draw.Layout.Bottom);
        Vector2 rightB = new(Draw.Layout.ScannerRight - (48 * scale), Draw.Layout.Bottom);

        if (gameState.Config.Engine.Graphics.GraphicStyle == GraphicStyle.Wireframe)
        {
            // Left laser
            Draw.Graphics.DrawTriangle(leftA, target, leftB, color);

            // Right laser
            Draw.Graphics.DrawTriangle(rightA, target, rightB, color);
        }
        else
        {
            // Left laser
            Draw.Graphics.DrawTriangleFilled(leftA, target, leftB, color);

            // Right laser
            Draw.Graphics.DrawTriangleFilled(rightA, target, rightB, color);
        }
    }

    // Each laser type has its own crosshair sprite, centred on the view.
    internal void DrawLaserSights(LaserType laserType)
    {
        if (laserType == LaserType.None)
        {
            return;
        }

        string image = CrosshairImage(laserType);
        Draw.Graphics.DrawImage(image, Draw.Layout.Centre - (Draw.Graphics.ImageSize(image) / 2));
    }

    /// <summary>
    /// The beam colour for a laser type. Beam and mining match their crosshair
    /// sprite's shade; pulse and military share the default.
    /// </summary>
    protected abstract FastColor BeamColor(LaserType laserType);

    private static string CrosshairImage(LaserType laserType) => laserType switch
    {
        LaserType.Pulse => nameof(ImageType.LaserPulse),
        LaserType.Beam => nameof(ImageType.LaserBeam),
        LaserType.Military => nameof(ImageType.LaserMilitary),
        LaserType.Mining => nameof(ImageType.LaserMining),
        _ => throw new ArgumentOutOfRangeException(nameof(laserType)),
    };
}
