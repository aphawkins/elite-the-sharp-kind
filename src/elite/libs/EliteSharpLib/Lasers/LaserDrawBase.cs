// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using EliteSharpLib.Graphics;
using Useful;

namespace EliteSharpLib.Lasers;

/// <summary>
/// The laser beams and crosshairs. The geometry is shared - it is already
/// derived from the tier's scale and scanner edges - but the beam colours are
/// each tier's own, so those are left to the subclass.
/// <para>
/// It reads no game state: whether the beams are outlined and where they
/// converge both arrive on the model, the same as everything else a view
/// draws.
/// </para>
/// </summary>
internal abstract class LaserDrawBase(IEliteDraw draw)
{
    protected IEliteDraw Draw { get; } = draw;

    /// <summary>
    /// Draws the two beams converging ahead of the ship.
    /// </summary>
    /// <param name="laserType">The mount that is firing.</param>
    /// <param name="aim">
    /// Where the beams meet, relative to the viewport centre and in the
    /// original's coordinates - the frame's jitter, which the game rolls
    /// rather than the view.
    /// </param>
    /// <param name="wireframe">Whether the beams are outlined rather than filled.</param>
    internal void DrawLaserLines(LaserType laserType, Vector2 aim, bool wireframe)
    {
        FastColor color = BeamColor(laserType);
        float scale = Draw.Layout.Scale;

        Vector2 target = Draw.Layout.ViewportCentre + (aim * scale);

        Vector2 leftA = new(Draw.Layout.ViewportLeft + (32 * scale), Draw.Layout.ViewportHeight);
        Vector2 leftB = new(Draw.Layout.ViewportLeft + (48 * scale), Draw.Layout.ViewportHeight);

        Vector2 rightA = new(Draw.Layout.ViewportRight - (32 * scale), Draw.Layout.ViewportHeight);
        Vector2 rightB = new(Draw.Layout.ViewportRight - (48 * scale), Draw.Layout.ViewportHeight);

        if (wireframe)
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
        Draw.Graphics.DrawImage(image, Draw.Layout.ViewportCentre - (Draw.Graphics.ImageSize(image) / 2));
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
