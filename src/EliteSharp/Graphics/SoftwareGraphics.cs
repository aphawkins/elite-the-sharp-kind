// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Diagnostics;
using System.Numerics;

namespace EliteSharp.Graphics
{
    public sealed class SoftwareGraphics : IGraphics
    {
        ////private readonly ConcurrentDictionary<ImageType, EBitmap> _images = new();
        private readonly FastBitmap _screen;
        private readonly Action<FastBitmap> _screenUpdate;
        private bool _isDisposed;

        public SoftwareGraphics(float screenWidth, float screenHeight, Action<FastBitmap> screenUpdate)
        {
            ScreenWidth = screenWidth;
            ScreenHeight = screenHeight;
            _screen = new((int)screenWidth, (int)screenHeight);
            _screenUpdate = screenUpdate;
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

        public void DrawCircle(Vector2 centre, float radius, FastColor colour)
        {
            float diameter = radius * 2;
            float x = MathF.Floor(radius);
            float y = 0;
            float tx = 1;
            float ty = 1;
            float error = tx - diameter;

            while (x >= y)
            {
                DrawPixel(new(centre.X + x, centre.Y + y), colour);
                DrawPixel(new(centre.X + x, centre.Y - y), colour);
                DrawPixel(new(centre.X - x, centre.Y + y), colour);
                DrawPixel(new(centre.X - x, centre.Y - y), colour);
                DrawPixel(new(centre.X + y, centre.Y + x), colour);
                DrawPixel(new(centre.X + y, centre.Y - x), colour);
                DrawPixel(new(centre.X - y, centre.Y + x), colour);
                DrawPixel(new(centre.X - y, centre.Y - x), colour);

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

        public void DrawCircleFilled(Vector2 centre, float radius, FastColor colour)
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
                DrawLine(new(centre.X - y, centre.Y - x), new(centre.X + y, centre.Y - x), colour);

                // Bottom of top half
                DrawLine(new(centre.X - x, centre.Y - y), new(centre.X + x, centre.Y - y), colour);

                // Top of bottom half
                DrawLine(new(centre.X - x, centre.Y + y), new(centre.X + x, centre.Y + y), colour);

                // Bottom of bottom half
                DrawLine(new(centre.X - y, centre.Y + x), new(centre.X + y, centre.Y + x), colour);

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
        }

        public void DrawImageCentre(ImageType image, float y)
        {
        }

        public void DrawLine(Vector2 lineStart, Vector2 lineEnd, FastColor colour)
        {
            for (int i = 0; i < 100; i++)
            {
                DrawPixel(new(i, i), colour);
            }

            ////float dx = MathF.Abs(lineStart.X - lineEnd.X);
            ////float dy = MathF.Abs(lineStart.Y - lineEnd.Y);
            ////int sx = lineStart.X <= lineEnd.X ? 1 : -1;
            ////int sy = lineStart.Y <= lineEnd.Y ? 1 : -1;
            ////float err = dx - dy;

            ////while (true)
            ////{
            ////    DrawPixel(lineStart, colour);

            ////    if ((int)lineStart.X == (int)lineEnd.X && (int)lineStart.Y == (int)lineEnd.Y)
            ////    {
            ////        break;
            ////    }

            ////    float err2 = 2 * err;
            ////    if (err2 > -dy)
            ////    {
            ////        err -= dy;
            ////        lineStart.X += sx;
            ////    }

            ////    if (err2 < dx)
            ////    {
            ////        err += dx;
            ////        lineStart.Y += sy;
            ////    }
            ////}
        }

        public void DrawPixel(Vector2 position, FastColor colour)
        {
            if (position.X < 0 || position.Y < 0 || position.X >= ScreenWidth || position.Y >= ScreenHeight)
            {
                return;
            }

            _screen.SetPixel((int)position.X, (int)position.Y, colour);
        }

        public void DrawPolygon(Vector2[] points, FastColor lineColour)
        {
        }

        public void DrawPolygonFilled(Vector2[] points, FastColor faceColour)
        {
        }

        public void DrawRectangle(Vector2 position, float width, float height, FastColor colour)
        {
        }

        public void DrawRectangleCentre(float y, float width, float height, FastColor colour)
        {
        }

        public void DrawRectangleFilled(Vector2 position, float width, float height, FastColor colour)
        {
        }

        public void DrawTextCentre(float y, string text, FontSize fontSize, FastColor colour)
        {
        }

        public void DrawTextLeft(Vector2 position, string text, FastColor colour)
        {
        }

        public void DrawTextRight(Vector2 position, string text, FastColor colour)
        {
        }

        public void DrawTriangle(Vector2 a, Vector2 b, Vector2 c, FastColor colour)
        {
        }

        public void DrawTriangleFilled(Vector2 a, Vector2 b, Vector2 c, FastColor colour)
        {
        }

        public void LoadBitmap(ImageType imgType, string bitmapPath)
        {
            ////using MemoryStream memStream = new();
            ////using FileStream stream = new(bitmapPath, FileMode.Open);
            ////stream.CopyToAsync(memStream).ConfigureAwait(false);
            ////memStream.Position = 0;
            ////_images[imgType] = new(memStream.ToArray());
        }

        public void ScreenUpdate() => _screenUpdate(_screen);

        public void SetClipRegion(Vector2 position, float width, float height)
        {
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

        // override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~SoftwareGraphics()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }
    }
}
