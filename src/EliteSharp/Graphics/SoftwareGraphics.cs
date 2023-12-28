// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Diagnostics;
using System.Numerics;
using EliteSharp.Assets;
using EliteSharp.Assets.Fonts;

namespace EliteSharp.Graphics
{
    public sealed class SoftwareGraphics : IGraphics
    {
        private readonly int _fontHeight = 20;
        private readonly FastBitmap _fontLarge;
        private readonly Dictionary<FontType, FastBitmap> _fonts;
        private readonly FastBitmap _fontSmall;
        private readonly int _fontWidth = 12;
        private readonly Dictionary<ImageType, FastBitmap> _images;
        private readonly FastBitmap _screen;
        private readonly Action<FastBitmap> _screenUpdate;
        private bool _isDisposed;

        public SoftwareGraphics(float screenWidth, float screenHeight, SoftwareAssetLoader assetLoader, Action<FastBitmap> screenUpdate)
        {
            Guard.ArgumentNull(assetLoader);

            ScreenWidth = screenWidth;
            ScreenHeight = screenHeight;
            _screen = new((int)screenWidth, (int)screenHeight);
            _screenUpdate = screenUpdate;
            _images = assetLoader.LoadImages();
            _fonts = assetLoader.LoadFonts();
            _fontLarge = _fonts[FontType.Large];
            _fontSmall = _fonts[FontType.Small];
            Clear();
        }

        public float Scale { get; } = 2;

        public float ScreenHeight { get; }

        public float ScreenWidth { get; }

        public void Clear() => _screen.Clear();

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public void DrawCircle(Vector2 centre, float radius, FastColor color)
        {
            float diameter = radius * 2;
            float x = MathF.Floor(radius);
            float y = 0;
            float tx = 1;
            float ty = 1;
            float error = tx - diameter;

            while (x >= y)
            {
                DrawPixel(new(centre.X + x, centre.Y + y), color);
                DrawPixel(new(centre.X + x, centre.Y - y), color);
                DrawPixel(new(centre.X - x, centre.Y + y), color);
                DrawPixel(new(centre.X - x, centre.Y - y), color);
                DrawPixel(new(centre.X + y, centre.Y + x), color);
                DrawPixel(new(centre.X + y, centre.Y - x), color);
                DrawPixel(new(centre.X - y, centre.Y + x), color);
                DrawPixel(new(centre.X - y, centre.Y - x), color);

                if (error <= 0)
                {
                    y++;
                    error += ty;
                    ty += 2;
                }

                if (error > 0)
                {
                    x--;
                    tx += 2;
                    error += tx - diameter;
                }
            }
        }

        public void DrawCircleFilled(Vector2 centre, float radius, FastColor color)
        {
            float diameter = MathF.Floor(radius) * 2;
            float x = MathF.Floor(radius);
            float y = 0;
            float tx = 1;
            float ty = 1;
            float error = tx - diameter;

            while (x >= y)
            {
                Debug.WriteLine($"{x},{y}");

                // Top of top half
                DrawLine(new(centre.X - y, centre.Y - x), new(centre.X + y, centre.Y - x), color);

                // Bottom of top half
                DrawLine(new(centre.X - x, centre.Y - y), new(centre.X + x, centre.Y - y), color);

                // Top of bottom half
                DrawLine(new(centre.X - x, centre.Y + y), new(centre.X + x, centre.Y + y), color);

                // Bottom of bottom half
                DrawLine(new(centre.X - y, centre.Y + x), new(centre.X + y, centre.Y + x), color);

                if (error <= 0)
                {
                    y++;
                    error += ty;
                    ty += 2;
                }

                if (error > 0)
                {
                    x--;
                    tx += 2;
                    error += tx - diameter;
                }
            }
        }

        public void DrawImage(ImageType image, Vector2 position)
        {
            FastBitmap bitmap = _images[image];
            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    FastColor color = bitmap.GetPixel(x, y);
                    if (color.A != 0)
                    {
                        // TODO: should mix the transparent colors correctly here
                        // but the only transparency being used is transparent or opaque
                        DrawPixel((int)(position.X + x), (int)(position.Y + y), color);
                    }
                }
            }
        }

        public void DrawImageCentre(ImageType image, float y)
        {
            float x = (ScreenWidth - _images[image].Width) / 2;
            DrawImage(image, new(x, y));
        }

        public void DrawLine(Vector2 lineStart, Vector2 lineEnd, FastColor color)
            => DrawLineInt(
                (int)MathF.Floor(lineStart.X),
                (int)MathF.Floor(lineStart.Y),
                (int)MathF.Floor(lineEnd.X),
                (int)MathF.Floor(lineEnd.Y),
                color);

        public void DrawPixel(Vector2 position, FastColor color)
        {
            if (position.X < 0 || position.Y < 0 || position.X >= ScreenWidth || position.Y >= ScreenHeight)
            {
                return;
            }

            _screen.SetPixel((int)position.X, (int)position.Y, color);
        }

        public void DrawPolygon(Vector2[] points, FastColor lineColor)
        {
            if (points == null)
            {
                return;
            }

            for (int i = 0; i < points.Length - 1; i++)
            {
                DrawLine(points[i], points[i + 1], lineColor);
            }

            DrawLine(points[0], points[^1], lineColor);
        }

        public void DrawPolygonFilled(Vector2[] points, FastColor faceColor)
        {
            if (points == null)
            {
                return;
            }

            // Create triangles of which each share the first vertex
            for (int i = 1; i < points.Length - 1; i++)
            {
                DrawTriangleFilled(points[0], points[i], points[i + 1], faceColor);
            }
        }

        public void DrawRectangle(Vector2 position, float width, float height, FastColor color)
            => DrawRectangleInt(
                (int)MathF.Floor(position.X),
                (int)MathF.Floor(position.Y),
                (int)MathF.Floor(width),
                (int)MathF.Floor(height),
                color);

        public void DrawRectangleCentre(float y, float width, float height, FastColor color)
            => DrawRectangle(new((ScreenWidth - width) / Scale, y), width, height, color);

        public void DrawRectangleFilled(Vector2 position, float width, float height, FastColor color)
            => DrawRectangleFilledInt(
                (int)MathF.Floor(position.X),
                (int)MathF.Floor(position.Y),
                (int)MathF.Floor(width),
                (int)MathF.Floor(height),
                color);

        public void DrawTextCentre(float y, string text, FontSize fontSize, FastColor color)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return;
            }

            int i = 0;
            foreach (char letter in text)
            {
                int fontRow = (letter >> 4) - 2;
                int fontColumn = letter & 0xF;
                int x = (int)((ScreenWidth / 2) - (text.Length * _fontWidth / 2));

                for (int fontY = 0; fontY < _fontHeight; fontY++)
                {
                    for (int fontX = 0; fontX < _fontWidth; fontX++)
                    {
                        FastColor pixelColor = _fontLarge.GetPixel(
                            fontX + (32 * fontColumn) + 1,
                            fontY + (32 * fontRow) + 1);

                        if (pixelColor.A != 0)
                        {
                            // TODO: should mix the transparent colors correctly here
                            // but the only transparency being used is transparent or opaque
                            DrawPixel(
                                x + fontX + (i * _fontWidth),
                                (int)y + fontY,
                                pixelColor);
                        }
                    }
                }

                i++;
            }
        }

        public void DrawTextLeft(Vector2 position, string text, FastColor color)
        {
        }

        public void DrawTextRight(Vector2 position, string text, FastColor color)
        {
        }

        public void DrawTriangle(Vector2 a, Vector2 b, Vector2 c, FastColor color)
        {
            DrawLine(a, b, color);
            DrawLine(b, c, color);
            DrawLine(c, a, color);
        }

        public void DrawTriangleFilled(Vector2 a, Vector2 b, Vector2 c, FastColor color)
        {
            // Sort the points so that a <= b <= c
            (a, b, c) = SortPointsByY(a, b, c);

            // Compute the x coordinates of the triangle edges
            int[] ab = Interpolate(a.Y, a.X, b.Y, b.X);
            int[] bc = Interpolate(b.Y, b.X, c.Y, c.X);
            int[] ac = Interpolate(a.Y, a.X, c.Y, c.X);

            // Concatenate the short sides
            ab = ab.Length > 0 ? ab[..^1] : ab;  // all items in the array except the last
            int[] abc = [.. ab, .. bc];

            // Determine which is left and which is right
            int m = abc.Length / 2;
            int[] leftX = abc;
            int[] rightX = ac;

            if (ac.Length > m && abc.Length > m && ac[m] < abc[m])
            {
                (leftX, rightX) = (rightX, leftX);
            }

            // Draw the horizontal segments
            int ay = (int)MathF.Floor(a.Y);
            int cy = (int)Math.Floor(c.Y);

            for (int y = ay; y <= cy; y++)
            {
                if (leftX.Length > y - ay && rightX.Length > y - ay)
                {
                    for (int x = leftX[y - ay]; x <= rightX[y - ay]; x++)
                    {
                        DrawPixel(x, y, color);
                    }
                }
            }
        }

        public void ScreenUpdate() => _screenUpdate(_screen);

        public void SetClipRegion(Vector2 position, float width, float height)
        {
        }

        private static (Vector2 A, Vector2 B, Vector2 C) SortPointsByY(Vector2 a, Vector2 b, Vector2 c)
        {
            Vector2[] sorted = [a, b, c];
            Array.Sort(sorted, (i, j) => i.Y.CompareTo(j.Y));
            return (sorted[0], sorted[1], sorted[2]);
        }

        private void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    // dispose managed state (managed objects)
                    _screen?.Dispose();
                }

                // free unmanaged resources (unmanaged objects) and override finalizer
                // set large fields to null
                _isDisposed = true;
            }
        }

        private void DrawLineInt(int startX, int startY, int endX, int endY, in FastColor color)
        {
            if (Math.Abs(endX - startX) > Math.Abs(endY - startY))
            {
                // Line is horizontal-ish
                // Make sure startX < endX
                if (startX > endX)
                {
                    (endX, startX) = (startX, endX);
                    (endY, startY) = (startY, endY);
                }

                int[] ys = Interpolate(startX, startY, endX, endY);
                for (int x = startX; x <= endX; x++)
                {
                    if ((x - startX) < ys.Length)
                    {
                        DrawPixel(x, ys[x - startX], color);
                    }
                }
            }
            else
            {
                // Line is vertical-ish
                // Make sure startY < endY
                if (startY > endY)
                {
                    (endX, startX) = (startX, endX);
                    (endY, startY) = (startY, endY);
                }

                int[] xs = Interpolate(startY, startX, endY, endX);
                for (int y = startY; y <= endY; y++)
                {
                    if ((y - startY) < xs.Length)
                    {
                        DrawPixel(xs[y - startY], y, color);
                    }
                }
            }
        }

        private void DrawPixel(int x, int y, in FastColor color)
        {
            if (x < 0 || y < 0 || x >= ScreenWidth || y >= ScreenHeight)
            {
                return;
            }

            _screen.SetPixel(x, y, color);
        }

        private void DrawRectangleFilledInt(int startX, int startY, int width, int height, in FastColor color)
        {
            startX = Math.Min(Math.Max(startX, 0), (int)ScreenWidth);
            startY = Math.Min(Math.Max(startY, 0), (int)ScreenWidth);
            int endX = Math.Min(Math.Max(startX + width, 0), (int)ScreenWidth);
            int endY = Math.Min(Math.Max(startY + height, 0), (int)ScreenWidth);

            // Draw horizontal lined
            for (int x = startX; x <= endX; x++)
            {
                for (int y = startY; y <= endY; y++)
                {
                    _screen.SetPixel(x, y, color);
                }
            }
        }

        private void DrawRectangleInt(int startX, int startY, int width, int height, in FastColor color)
        {
            startX = Math.Min(Math.Max(startX, 0), (int)ScreenWidth);
            startY = Math.Min(Math.Max(startY, 0), (int)ScreenWidth);
            int endX = Math.Min(Math.Max(startX + width, 0), (int)ScreenWidth);
            int endY = Math.Min(Math.Max(startY + height, 0), (int)ScreenWidth);

            // Draw horizontal lines
            for (int x = startX; x <= endX; x++)
            {
                _screen.SetPixel(x, startY, color);
                _screen.SetPixel(x, endY, color);
            }

            for (int y = startY; y <= endY; y++)
            {
                _screen.SetPixel(startX, y, color);
                _screen.SetPixel(endX, y, color);
            }
        }

        private int[] Interpolate(float i0, float d0, float i1, float d1)
        {
            if ((int)MathF.Floor(i0) == (int)MathF.Floor(i1))
            {
                return [(int)MathF.Floor(d0)];
            }

            List<int> values = [];

            float a = (d1 - d0) / (i1 - i0);
            float d = d0;
            for (int i = (int)MathF.Floor(i0); i <= (int)MathF.Floor(i1); i++)
            {
                values.Add((int)MathF.Floor(d));

                d += a;
            }

            return [.. values];
        }
    }
}
