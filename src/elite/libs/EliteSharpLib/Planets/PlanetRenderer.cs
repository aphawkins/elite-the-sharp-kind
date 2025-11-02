// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using EliteSharpLib.Graphics;
using Useful.Graphics;

namespace EliteSharpLib.Planets;

internal class PlanetRenderer
{
    internal const int LandXMax = 128;
    internal const int LandYMax = 128;
#pragma warning disable SA1401 // Fields should be private
    internal readonly FastColor[,] _landscape = new FastColor[LandXMax + 1, LandYMax + 1];
#pragma warning restore SA1401 // Fields should be private
    private readonly IEliteDraw _draw;

    internal PlanetRenderer(IEliteDraw draw) => _draw = draw;

    internal (Vector2 Position, float Radius)? GetPlanetPosition(Vector4 location)
    {
        Vector2 position = new(location.X, -location.Y);
        position *= 256 / location.Z;
        position += _draw.Centre / 2;
        position *= _draw.Graphics.Scale;

        float radius = 6291456 / location.Length();

        // Planets are BIG!
        ////  radius = 6291456 / ship_vec.z;
        radius *= _draw.Graphics.Scale;

        return (position.X + radius < _draw.Left) ||
            (position.X - radius > _draw.Right) ||
            (position.Y + radius < _draw.Top) ||
            (position.Y - radius > _draw.Bottom)
            ? null
            : (position, radius);
    }

    /// <summary>
    /// Draw a solid planet. Based on Doros circle drawing alogorithm.
    /// </summary>
    internal void Draw(Vector2 centre, float radius, Vector4[] vec)
    {
        float vx = vec[1].X * 65536;
        float vy = vec[1].Y * 65536;
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
    private void RenderPlanetLine(Vector2 centre, float x, float y, float radius, float vx, float vy)
    {
        Vector2 s = new()
        {
            Y = y + centre.Y,
        };

        if (s.Y < _draw.Top || s.Y > _draw.Bottom)
        {
            return;
        }

        s.X = centre.X - x;
        float ex = centre.X + x;

        float rx = (-x * vx) - (y * vy);
        float ry = (-x * vy) + (y * vx);
        rx += radius * 65536;
        ry += radius * 65536;

        // radius * 2 * LAND_X_MAX >> 16
        float div = radius * 1024;

        for (; s.X <= ex; s.X++)
        {
            if (s.X >= _draw.Left && s.X <= _draw.Right)
            {
                int lx = (int)Math.Clamp(MathF.Abs(rx / div), 0, LandXMax);
                int ly = (int)Math.Clamp(MathF.Abs(ry / div), 0, LandYMax);
                _draw.Graphics.DrawPixel(s, _landscape[lx, ly]);
            }

            rx += vx;
            ry += vy;
        }
    }
}
