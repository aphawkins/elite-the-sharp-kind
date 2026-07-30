// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using EliteSharpLib.Graphics;
using Useful;

namespace EliteSharpLib.Planets;

internal class PlanetRenderer
{
    internal const int LandXMax = 128;
    internal const int LandYMax = 128;
    private readonly IEliteDraw _draw;

    internal PlanetRenderer(IEliteDraw draw) => _draw = draw;

    internal FastColor[,] Landscape { get; } = new FastColor[LandXMax + 1, LandYMax + 1];

    internal (Vector2 Position, float Radius)? GetPlanetPosition(Vector4 location)
    {
        Vector2 position = new(location.X, -location.Y);
        position *= _draw.Focus / location.Z;
        position += _draw.Layout.Centre;

        float radius = 6291456 / location.Length();

        // Planets are BIG!
        ////  radius = 6291456 / ship_vec.z;
        // The radius is in the original's 256-wide space, so it follows the
        // projection's focal length rather than Scale.
        radius *= _draw.Focus / 256;

        return (position.X + radius < _draw.Layout.Left) ||
            (position.X - radius > _draw.Layout.Right) ||
            (position.Y + radius < _draw.Layout.Top) ||
            (position.Y - radius > _draw.Layout.Bottom)
            ? null
            : (position, radius);
    }

    /// <summary>
    /// Draw a solid planet. Based on Doros circle drawing alogorithm.
    /// </summary>
    internal void Draw(Vector2 centre, float radius, Matrix4x4 rotmat)
    {
        float vx = rotmat.M21 * 65536;
        float vy = rotmat.M22 * 65536;
        float x = MathF.Floor(radius);
        float s = -x;
        float y = 0;

        while (y <= x)
        {
            // Top of top half
            RenderPlanetLine(centre, y, -x, radius, vx, vy);

            // Bottom of top half
            RenderPlanetLine(centre, x, -y, radius, vx, vy);

            // Top of bottom half
            RenderPlanetLine(centre, x, y, radius, vx, vy);

            // Bottom of bottom half
            RenderPlanetLine(centre, y, x, radius, vx, vy);

            s += y + y + 1;
            y++;
            if (s >= 0)
            {
                s -= x + x + 2;
                x--;
            }
        }
    }

    /// <summary>
    /// Draw a line of the planet with appropriate rotation.
    /// </summary>
    private void RenderPlanetLine(Vector2 centre, float offsetX, float offsetY, float radius, float vx, float vy)
    {
        Vector2 s = new()
        {
            Y = offsetY + centre.Y,
        };

        // Bottom/Right are the border line's own row/column (see EliteDraw.Height/
        // Width), so the far edge must be excluded here the same way Top/Left's
        // near edge already is, or the fill paints over the border.
        if (s.Y < _draw.Layout.Top || s.Y >= _draw.Layout.Bottom)
        {
            return;
        }

        s.X = centre.X - offsetX;
        float ex = centre.X + offsetX;

        float rx = (-offsetX * vx) - (offsetY * vy);
        float ry = (-offsetX * vy) + (offsetY * vx);
        rx += radius * 65536;
        ry += radius * 65536;

        // radius * 2 * LAND_X_MAX >> 16
        float div = radius * 1024;

        for (; s.X <= ex; s.X++)
        {
            if (s.X >= _draw.Layout.Left && s.X < _draw.Layout.Right)
            {
                int lx = (int)Math.Clamp(MathF.Abs(rx / div), 0, LandXMax);
                int ly = (int)Math.Clamp(MathF.Abs(ry / div), 0, LandYMax);
                _draw.Graphics.DrawPixel(s, Landscape[lx, ly]);
            }

            rx += vx;
            ry += vy;
        }
    }
}
