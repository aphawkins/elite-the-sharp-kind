// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;

namespace EliteSharp.Graphics
{
    internal class SoftwareGraphics : IGraphics
    {
        private readonly int[,] _buffer;

        internal SoftwareGraphics(int[,] buffer)
        {
            _buffer = buffer;
            ScreenWidth = buffer.GetLength(0);
            ScreenHeight = buffer.GetLength(1);
            Scale = 2;
        }

        public float ScreenHeight { get; }

        public float Scale { get; }

        public float ScreenWidth { get; }

        public void ClearArea(Vector2 position, float width, float height) => throw new NotImplementedException();

        public void Dispose()
        {
        }

        public void DrawCircle(Vector2 centre, float radius, Colour colour)
        {
            int diameter = (int)(radius * 2);
            int x = (int)(radius - 1);
            int y = 0;
            int tx = 1;
            int ty = 1;
            int error = tx - diameter;

            while (x >= y)
            {
                _buffer[(int)(radius + x + centre.X), (int)(radius + y + centre.Y)] = (int)colour;
                _buffer[(int)(radius + x + centre.X), (int)(radius - y + centre.Y)] = (int)colour;
                _buffer[(int)(radius - x + centre.X), (int)(radius + y + centre.Y)] = (int)colour;
                _buffer[(int)(radius - x + centre.X), (int)(radius - y + centre.Y)] = (int)colour;
                _buffer[(int)(radius + y + centre.X), (int)(radius + x + centre.Y)] = (int)colour;
                _buffer[(int)(radius + y + centre.X), (int)(radius - x + centre.Y)] = (int)colour;
                _buffer[(int)(radius - y + centre.X), (int)(radius + x + centre.Y)] = (int)colour;
                _buffer[(int)(radius - y + centre.X), (int)(radius - x + centre.Y)] = (int)colour;

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

        public void DrawCircleFilled(Vector2 centre, float radius, Colour colour)
        {
            int diameter = (int)(radius * 2);
            int x = (int)(radius - 1);
            int y = 0;
            int tx = 1;
            int ty = 1;
            int error = tx - diameter;

            while (x >= y)
            {
                for (int i = y; i <= x; i++)
                {
                    _buffer[(int)(radius + i + centre.X), (int)(radius + y + centre.Y)] = (byte)colour;
                    _buffer[(int)(radius + i + centre.X), (int)(radius - y + centre.Y)] = (byte)colour;
                    _buffer[(int)(radius + y + centre.X), (int)(radius + i + centre.Y)] = (byte)colour;
                    _buffer[(int)(radius - y + centre.X), (int)(radius + i + centre.Y)] = (byte)colour;
                    _buffer[(int)(radius - i + centre.X), (int)(radius + y + centre.Y)] = (byte)colour;
                    _buffer[(int)(radius - i + centre.X), (int)(radius - y + centre.Y)] = (byte)colour;
                    _buffer[(int)(radius + y + centre.X), (int)(radius - i + centre.Y)] = (byte)colour;
                    _buffer[(int)(radius - y + centre.X), (int)(radius - i + centre.Y)] = (byte)colour;
                }

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

        public void DrawImage(Image image, Vector2 position) => throw new NotImplementedException();

        public void DrawImageCentre(Image image, float y) => throw new NotImplementedException();

        public void DrawLine(Vector2 lineStart, Vector2 lineEnd, Colour colour) => throw new NotImplementedException();

        public void DrawLine(Vector2 lineStart, Vector2 lineEnd) => throw new NotImplementedException();

        public void DrawPixel(Vector2 position, Colour colour) => _buffer[(int)position.X, (int)position.Y] = (int)colour;

        public void DrawPixelFast(Vector2 position, Colour colour) => _buffer[(int)position.X, (int)position.Y] = (int)colour;

        public void DrawPolygon(Vector2[] pointList, Colour lineColour) => throw new NotImplementedException();

        public void DrawPolygonFilled(Vector2[] pointList, Colour faceColour) => throw new NotImplementedException();

        public void DrawRectangle(Vector2 position, float width, float height, Colour colour) => throw new NotImplementedException();

        public void DrawRectangleCentre(float y, float width, float height, Colour colour) => throw new NotImplementedException();

        public void DrawRectangleFilled(Vector2 position, float width, float height, Colour colour) => throw new NotImplementedException();

        public void DrawTextCentre(float y, string text, FontSize fontSize, Colour colour) => throw new NotImplementedException();

        public void DrawTextLeft(Vector2 position, string text, Colour colour) => throw new NotImplementedException();

        public void DrawTextRight(float x, float y, string text, Colour colour) => throw new NotImplementedException();

        public void DrawTriangle(Vector2 a, Vector2 b, Vector2 c, Colour colour) => throw new NotImplementedException();

        public void DrawTriangleFilled(Vector2 a, Vector2 b, Vector2 c, Colour colour) => throw new NotImplementedException();

        public void LoadBitmap(Image imgType, byte[] bitmapBytes) => throw new NotImplementedException();

        public void ScreenAcquire() => throw new NotImplementedException();

        public void ScreenRelease() => throw new NotImplementedException();

        public void ScreenUpdate() => throw new NotImplementedException();

        public void SetClipRegion(Vector2 position, float width, float height) => throw new NotImplementedException();
    }
}
