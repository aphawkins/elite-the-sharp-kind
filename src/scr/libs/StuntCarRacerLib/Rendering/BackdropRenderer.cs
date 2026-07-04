// 'Stunt Car Racer - The Sharp Kind' - Andy Hawkins 2026.
// 'Stunt Car Racer Remake' - sourceforge.net/projects/stuntcarremake.
// Stunt Car Racer (C) Geoff Crammond / MicroStyle / MicroProse 1989.

using System.Numerics;
using StuntCarRacerLib.Cars;
using StuntCarRacerLib.Tracks;
using Useful;
using Useful.Graphics;

namespace StuntCarRacerLib.Rendering;

// Draws the horizon (sky/ground fill) and the scenery skyline, ported from
// the original Backdrop.cpp. The sky is filled first and the ground drawn
// as one large polygon below the projected horizon line, which replaces the
// original's rectangle/clipping case analysis; the scenery objects are the
// original's mountains, buildings and lake on a ring around the viewpoint.
public sealed class BackdropRenderer
{
    private const int SkyColour = Track.ScrBaseColour + 7;

    private const int GroundColour = Track.ScrBaseColour + 13;

    // Scenery type ranges 0-4 (standard, taller, snowcapped, buildings, mixed).
    private const int MaxSceneryType = 4;

    // Original: z of 0x00010000, divided by (4 * FOCUS).
    private const int SceneryScaleFactor = 64;

    private static readonly int[][] s_sceneryTypes =
    [
        BackdropData.StandardNumbers,
        BackdropData.TallerNumbers,
        BackdropData.SnowcappedNumbers,
        BackdropData.BuildingNumbers,
        BackdropData.MixedNumbers,
    ];

    private readonly IGraphics _graphics;

    private int _currentSceneryType = MaxSceneryType;

    public BackdropRenderer(IGraphics graphics)
    {
        Guard.ArgumentNull(graphics);
        _graphics = graphics;
    }

    public void NextSceneryType()
    {
        _currentSceneryType++;
        if (_currentSceneryType > MaxSceneryType)
        {
            _currentSceneryType = 0;
        }
    }

    public void Draw(SceneCamera camera)
    {
        Guard.ArgumentNull(camera);

        // the original works with the render-space viewpoint y, which is the
        // negated camera height (in track units)
        int viewpointY = -camera.Y;

        DrawHorizon(viewpointY, camera.XAngle, camera.ZAngle);
        DrawScenery(viewpointY, camera.XAngle, camera.YAngle, camera.ZAngle);
    }

    private void DrawHorizon(int viewpointY, int angleX, int angleZ)
    {
        float width = _graphics.ScreenWidth;
        float height = _graphics.ScreenHeight;

        // y adjustment depending upon the viewpoint x angle - needed because
        // only two horizon points are used rather than four (when the rotated
        // z values are negative the y values are negated, so the adjustment
        // must also change sign)
        bool rotatedZNegative = angleX is >= AmigaTrig.Degrees90 and < AmigaTrig.Degrees270;
        int adjustY = (rotatedZNegative ? viewpointY : -viewpointY) / 2;

        // two far points defining the horizon line, rotated about the x and
        // z axes then projected (the original DrawHorizon projection)
        int sinX = AmigaTrig.Sin(angleX);
        int cosX = AmigaTrig.Cos(angleX);
        int sinZ = AmigaTrig.Sin(angleZ);
        int cosZ = AmigaTrig.Cos(angleZ);

        Span<Vector2> screen = stackalloc Vector2[2];
        for (int i = 0; i < 2; i++)
        {
            int x = i == 0 ? -0x00010000 : 0x00010000;
            int y = adjustY;

            // rotate about x axis
            long transY = ((long)y * cosX) - (0x00010000L * sinX);
            long transZ = ((long)y * sinX) + (0x00010000L * cosX);

            // rotate about z axis
            y = (int)(transY >> Track.LogPrecision);
            long transX = ((long)x * cosZ) - ((long)y * sinZ);
            transY = ((long)x * sinZ) + ((long)y * cosZ);

            // perspective projection (as the original: divisor scales with height)
            float z = transZ / (height * 512f / 480f);
            if (Math.Abs(z) < 1)
            {
                z = 1; // prevent division by zero
            }

            screen[i] = new(
                (transX / z) + (width / 2),
                (transY / z) + (height / 2));
        }

        // fill the whole screen with sky, then draw the ground as one large
        // polygon on the ground side of the horizon line
        _graphics.DrawRectangleFilled(new(0, 0), width, height, ScrPalette.Colour(SkyColour));

        Vector2 direction = screen[1] - screen[0];
        float length = direction.Length();
        if (length < 1)
        {
            return; // degenerate horizon line; leave sky only
        }

        direction /= length;

        // ground is on the (-dy, dx) side of the line, which flips
        // automatically when the viewpoint is upside down
        Vector2 groundSide = new(-direction.Y, direction.X);
        float extent = (width + height) * 4;

        Vector2 p1 = screen[0] - (direction * extent);
        Vector2 p2 = screen[1] + (direction * extent);
        _graphics.DrawPolygonFilled(
            [p1, p2, p2 + (groundSide * extent), p1 + (groundSide * extent)],
            ScrPalette.Colour(GroundColour));
    }

    private void DrawScenery(int viewpointY, int angleX, int angleY, int angleZ)
    {
        float width = _graphics.ScreenWidth;
        float height = _graphics.ScreenHeight;

        int[] sceneryNumbers = s_sceneryTypes[_currentSceneryType];

        // x/z angle are fixed for all scenery objects
        int sinX = AmigaTrig.Sin(angleX);
        int cosX = AmigaTrig.Cos(angleX);
        int sinZ = AmigaTrig.Sin(angleZ);
        int cosZ = AmigaTrig.Cos(angleZ);

        Span<Vector2> screen = stackalloc Vector2[7];
        for (int m = 0; m < BackdropData.SceneryPositions.Length; m++)
        {
            (Coord3D[] coords, int[] polygons) = BackdropData.SceneryObjects[sceneryNumbers[m]];

            // y angle for this scenery object (reversed, as the original)
            int position = BackdropData.SceneryPositions[m] * 256;
            int sceneryAngle = -(angleY + position) & (Track.MaxAngle - 1);

            int sinY = AmigaTrig.Sin(sceneryAngle);
            int cosY = AmigaTrig.Cos(sceneryAngle);

            // rotate scenery about y/x/z axis and perform perspective projection
            bool visible = true;
            for (int i = 0; i < coords.Length; i++)
            {
                int x = coords[i].X * SceneryScaleFactor;
                int y = -(coords[i].Y * SceneryScaleFactor);
                int z = coords[i].Z;

                y -= viewpointY / 2;

                // prevent sky showing through between scenery and ground
                y += 2 * SceneryScaleFactor;

                // rotate about y axis
                long transX = ((long)x * cosY) + ((long)z * sinY);
                long transZ = ((long)z * cosY) - ((long)x * sinY);

                // rotate about x axis
                z = (int)(transZ >> Track.LogPrecision);
                long transY = ((long)y * cosX) - ((long)z * sinX);
                transZ = ((long)y * sinX) + ((long)z * cosX);

                // rotate about z axis
                x = (int)(transX >> Track.LogPrecision);
                y = (int)(transY >> Track.LogPrecision);
                transX = ((long)x * cosZ) - ((long)y * sinZ);
                transY = ((long)x * sinZ) + ((long)y * cosZ);

                // skip this scenery object if any z is behind the viewpoint
                if (transZ <= 0)
                {
                    visible = false;
                    break;
                }

                // perspective projection
                float projZ = transZ / (height * 512f / 480f);
                if (Math.Abs(projZ) < 1)
                {
                    projZ = 1; // prevent division by zero
                }

                screen[i] = new(
                    (transX / projZ) + (width / 2),
                    (transY / projZ) + (height / 2));
            }

            if (!visible)
            {
                continue;
            }

            // draw the scenery object's polygons (colour, sides, offsets...)
            int p = 0;
            while (p < polygons.Length)
            {
                int colour = polygons[p++];
                int sides = polygons[p++];

                Vector2[] points = new Vector2[sides];
                for (int j = 0; j < sides; j++)
                {
                    points[j] = screen[polygons[p++]];
                }

                if (sides >= 3)
                {
                    _graphics.DrawPolygonFilled(points, ScrPalette.Colour(Track.ScrBaseColour + colour));
                }
            }
        }
    }
}
