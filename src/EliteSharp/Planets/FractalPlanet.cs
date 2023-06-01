// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Diagnostics.CodeAnalysis;
using EliteSharp.Enums;

namespace EliteSharp.Planets
{
    internal sealed class FractalPlanet : PlanetRenderer
    {
        internal FractalPlanet(IGraphics graphics)
            : base(graphics)
        {
        }

        /// <summary>
        /// Generate a fractal landscape. Uses midpoint displacement method.
        /// </summary>
        /// <param name="seed">Initial seed for the generation.</param>
        [SuppressMessage("Security", "CA5394:Do not use insecure randomness", Justification = "Randomness here requires seed.")]
        public override void GenerateLandscape(int seed)
        {
            const int d = LANDXMAX / 8;
            Random random = new(seed);

            for (int y = 0; y <= LANDYMAX; y += d)
            {
                for (int x = 0; x <= LANDXMAX; x += d)
                {
                    _landscape[x, y] = random.Next(255);
                }
            }

            for (int y = 0; y < LANDYMAX; y += d)
            {
                for (int x = 0; x < LANDXMAX; x += d)
                {
                    MidpointSquare(x, y, d);
                }
            }

            for (int y = 0; y <= LANDYMAX; y++)
            {
                for (int x = 0; x <= LANDXMAX; x++)
                {
                    float dist = (x * x) + (y * y);
                    bool dark = dist > 10000;
                    int h = _landscape[x, y];
                    _landscape[x, y] = h > 166
                        ? (int)(dark ? Colour.Green : Colour.LightGreen)
                        : (int)(dark ? Colour.Blue : Colour.LightBlue);
                }
            }
        }

        /// <summary>
        /// Calculate the midpoint between two given points.
        /// </summary>
        private int CalcMidpoint(int sx, int sy, int ex, int ey) =>
            Math.Clamp(((_landscape[sx, sy] + _landscape[ex, ey]) / 2) + RNG.GaussianRandom(-7, 8), 0, 255);

        /// <summary>
        /// Calculate a square on the midpoint map.
        /// </summary>
        /// <param name="tx"></param>
        /// <param name="ty"></param>
        /// <param name="w"></param>
        private void MidpointSquare(int tx, int ty, int w)
        {
            int d = w / 2;
            int mx = tx + d;
            int my = ty + d;
            int bx = tx + w;
            int by = ty + w;

            _landscape[mx, ty] = CalcMidpoint(tx, ty, bx, ty);
            _landscape[mx, by] = CalcMidpoint(tx, by, bx, by);
            _landscape[tx, my] = CalcMidpoint(tx, ty, tx, by);
            _landscape[bx, my] = CalcMidpoint(bx, ty, bx, by);
            _landscape[mx, my] = CalcMidpoint(tx, my, bx, my);

            if (d == 1)
            {
                return;
            }

            MidpointSquare(tx, ty, d);
            MidpointSquare(mx, ty, d);
            MidpointSquare(tx, my, d);
            MidpointSquare(mx, my, d);
        }
    }
}
