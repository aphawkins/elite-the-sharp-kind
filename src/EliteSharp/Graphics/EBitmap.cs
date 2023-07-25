// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

namespace EliteSharp.Graphics
{
    internal class EBitmap
    {
        private readonly EColor[,] _pixels;

        internal EBitmap(int width, int height) => _pixels = new EColor[width, height];

        internal int Height => _pixels.GetLength(1);

        internal int Width => _pixels.GetLength(0);

        internal EColor GetPixel(int x, int y) => _pixels[x, y];

        internal void SetPixel(int x, int y, EColor colour) => _pixels[x, y] = colour;
    }
}
