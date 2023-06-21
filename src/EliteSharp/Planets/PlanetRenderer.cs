// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using EliteSharp.Graphics;

namespace EliteSharp.Planets
{
    internal abstract class PlanetRenderer : IPlanetRenderer
    {
        protected const int LANDXMAX = 128;
        protected const int LANDYMAX = 128;
#pragma warning disable SA1401 // Fields should be private
        protected readonly int[,] _landscape = new int[LANDXMAX + 1, LANDYMAX + 1];
        protected IGraphics _graphics;
#pragma warning restore SA1401 // Fields should be private

        internal PlanetRenderer(IGraphics graphics) => _graphics = graphics;

        /// <summary>
        /// Draw a solid planet. Based on Doros circle drawing alogorithm.
        /// </summary>
        public virtual void Draw(Vector2 centre, float radius, Vector3[] vec)
        {
            float vx = vec[1].X * 65536;
            float vy = vec[1].Y * 65536;

            float s = radius;
            float x = radius;
            float y = 0;

            s -= x + x;
            while (y <= x)
            {
                // Top of top half
                RenderPlanetLine(centre, y, -MathF.Floor(x), radius, vx, vy);

                // Bottom of top half
                RenderPlanetLine(centre, x, -y, radius, vx, vy);

                // Top of bottom half
                RenderPlanetLine(centre, x, y, radius, vx, vy);

                // Bottom of bottom half
                RenderPlanetLine(centre, y, MathF.Floor(x), radius, vx, vy);

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
        protected void RenderPlanetLine(Vector2 centre, float x, float y, float radius, float vx, float vy)
        {
            Vector2 s = new()
            {
                Y = y + centre.Y,
            };

            if (s.Y < _graphics.ViewT.Y || s.Y > _graphics.ViewB.Y)
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
                if (s.X >= _graphics.ViewT.X && s.X <= _graphics.ViewB.X)
                {
                    int lx = (int)Math.Clamp(MathF.Abs(rx / div), 0, LANDXMAX);
                    int ly = (int)Math.Clamp(MathF.Abs(ry / div), 0, LANDYMAX);
                    Colour colour = (Colour)_landscape[lx, ly];
                    _graphics.DrawPixelFast(s, colour);
                }

                rx += vx;
                ry += vy;
            }
        }
    }
}
