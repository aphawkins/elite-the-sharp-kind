// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Collections.Concurrent;
using System.Diagnostics;
using System.Numerics;

namespace EliteSharp.Graphics
{
    internal class SoftwareGraphics : IGraphics
    {
        private readonly ConcurrentDictionary<ImageType, EBitmap> _images = new();
        private readonly EBitmap _screen;

        internal SoftwareGraphics(EBitmap screen)
        {
            _screen = screen;
            Clear();
        }

        public float Scale { get; } = 2;

        public float ScreenHeight => _screen.Height;

        public float ScreenWidth => _screen.Width;

        public void Clear()
        {
            for (int y = 0; y < _screen.Height; y++)
            {
                for (int x = 0; x < _screen.Width; x++)
                {
                    _screen.SetPixel(x, y, EColors.Black);
                }
            }
        }

        public void Dispose()
        {
        }

        public void DrawCircle(Vector2 centre, float radius, EColor colour)
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

        public void DrawCircleFilled(Vector2 centre, float radius, EColor colour)
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

        public void DrawImage(ImageType image, Vector2 position) => throw new NotImplementedException();

        public void DrawImageCentre(ImageType image, float y) => throw new NotImplementedException();

        public void DrawLine(Vector2 lineStart, Vector2 lineEnd, EColor colour)
        {
            float dx = MathF.Abs(lineStart.X - lineEnd.X);
            float dy = MathF.Abs(lineStart.Y - lineEnd.Y);
            int sx = lineStart.X <= lineEnd.X ? 1 : -1;
            int sy = lineEnd.X <= lineEnd.Y ? 1 : -1;
            float err = dx - dy;

            while (true)
            {
                DrawPixel(new(lineStart.X, lineStart.Y), colour);

                if ((int)lineStart.X == (int)lineEnd.X && (int)lineStart.Y == (int)lineEnd.Y)
                {
                    break;
                }

                float err2 = 2 * err;
                if (err2 > -dy)
                {
                    err -= dy;
                    lineStart.X += sx;
                }

                if (err2 < dx)
                {
                    err += dx;
                    lineStart.Y += sy;
                }
            }
        }

        public void DrawPixel(Vector2 position, EColor colour)
        {
            if (position.X < 0 || position.Y < 0 || position.X >= ScreenWidth || position.Y >= ScreenHeight)
            {
                return;
            }

            _screen.SetPixel((int)position.X, (int)position.Y, colour);
        }

        public void DrawPixelFast(Vector2 position, EColor colour) => DrawPixel(position, colour);

        public void DrawPolygon(Vector2[] points, EColor lineColour) => throw new NotImplementedException();

        public void DrawPolygonFilled(Vector2[] points, EColor faceColour) => throw new NotImplementedException();

        public void DrawRectangle(Vector2 position, float width, float height, EColor colour) => throw new NotImplementedException();

        public void DrawRectangleCentre(float y, float width, float height, EColor colour) => throw new NotImplementedException();

        public void DrawRectangleFilled(Vector2 position, float width, float height, EColor colour) => throw new NotImplementedException();

        public void DrawTextCentre(float y, string text, FontSize fontSize, EColor colour) => throw new NotImplementedException();

        public void DrawTextLeft(Vector2 position, string text, EColor colour) => throw new NotImplementedException();

        public void DrawTextRight(Vector2 position, string text, EColor colour) => throw new NotImplementedException();

        public void DrawTriangle(Vector2 a, Vector2 b, Vector2 c, EColor colour) => throw new NotImplementedException();

        public void DrawTriangleFilled(Vector2 a, Vector2 b, Vector2 c, EColor colour) => throw new NotImplementedException();

        public void LoadBitmap(ImageType imgType, string bitmapPath)
        {
            using MemoryStream memStream = new();
            using FileStream stream = new(bitmapPath, FileMode.Open);
            stream.CopyToAsync(memStream).ConfigureAwait(false);
            memStream.Position = 0;
            _images[imgType] = new(memStream.ToArray());
        }

        public void ScreenUpdate()
        {
        }

        public void SetClipRegion(Vector2 position, float width, float height)
        {
            // TODO: implement this
        }
    }
}
