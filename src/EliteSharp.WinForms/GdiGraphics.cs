// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Collections.Concurrent;
using System.Numerics;
using EliteSharp.Graphics;

namespace EliteSharp.WinForms
{
    internal sealed class GdiGraphics : IGraphics
    {
        private const int ScannerHeight = 128;
        private readonly Font _fontLarge = new("Arial", 18, FontStyle.Bold, GraphicsUnit.Pixel);
        private readonly Font _fontSmall = new("Arial", 12, FontStyle.Bold, GraphicsUnit.Pixel);
        private readonly ConcurrentDictionary<Graphics.Image, Bitmap> _images = new();
        private readonly Dictionary<Colour, Pen> _pens = new();
        private readonly Bitmap _screen;
        private readonly Bitmap _screenBuffer;
        private readonly System.Drawing.Graphics _screenBufferGraphics;
        private readonly System.Drawing.Graphics _screenGraphics;
        private readonly object _screenLock = new();
        private readonly Color _transparentColour = Color.FromArgb(0, 255, 0, 255);
        private RectangleF _clipRegion;
        private bool _isDisposed;

        public GdiGraphics(Bitmap screen)
        {
            _screen = screen;
            _screenGraphics = System.Drawing.Graphics.FromImage(_screen);
            _screenBuffer = new Bitmap(screen.Width, screen.Height);
            _screenBufferGraphics = System.Drawing.Graphics.FromImage(_screenBuffer);
            _screenBufferGraphics.Clear(Color.Black);
            Centre = new(screen.Width / 2, (screen.Height - ScannerHeight) / 2);
            Width = screen.Width;
            Height = screen.Height;

            foreach (Colour colour in Enum.GetValues<Colour>())
            {
                Pen pen = new(Color.FromArgb((int)colour | unchecked((int)0xFF000000)));
                _pens.Add(colour, pen);
            }
        }

        public Vector2 Centre { get; init; }

        public int Height { get; }

        public Vector2 Offset { get; private set; } = new(0, 0);

        public float Scale { get; } = 2;

        public Vector2 ViewB { get; private set; } = new(509, 381);

        public Vector2 ViewT { get; private set; } = new(1, 1);

        public int Width { get; }

        public void ClearArea(Vector2 position, float width, float height) => _screenBufferGraphics.FillRectangle(Brushes.Black, position.X + Offset.X, position.Y + Offset.Y, width + Offset.X, height + Offset.Y);

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public void DrawCircle(Vector2 centre, float radius, Colour colour) => _screenBufferGraphics.DrawEllipse(_pens[colour], centre.X + Offset.X - radius, centre.Y + Offset.Y - radius, 2 * radius, 2 * radius);

        public void DrawCircleFilled(Vector2 centre, float radius, Colour colour) => _screenBufferGraphics.FillEllipse(_pens[colour].Brush, centre.X + Offset.X - radius, centre.Y + Offset.Y - radius, 2 * radius, 2 * radius);

        public void DrawImage(Graphics.Image spriteImgage, Vector2 location)
        {
            Bitmap sprite = _images[spriteImgage];

            if (location.X < 0)
            {
                location.X = ((256 * Scale) - sprite.Width) / 2;
            }

            _screenBufferGraphics.DrawImage(sprite, location.X + Offset.X, location.Y + Offset.Y);
        }

        public void DrawLine(Vector2 lineStart, Vector2 lineEnd) => _screenBufferGraphics.DrawLine(_pens[Colour.White], lineStart.X + Offset.X, lineStart.Y + Offset.Y, lineEnd.X + Offset.X, lineEnd.Y + Offset.Y);

        public void DrawLine(Vector2 lineStart, Vector2 lineEnd, Colour colour) => _screenBufferGraphics.DrawLine(_pens[colour], lineStart.X + Offset.X, lineStart.Y + Offset.Y, lineEnd.X + Offset.X, lineEnd.Y + Offset.Y);

        public void DrawPixel(Vector2 position, Colour colour)
        {
            // Prevent SetPixel from drawing outside of the clip region
            if (position.X < _clipRegion.Left ||
                position.X > _clipRegion.Right ||
                position.Y < _clipRegion.Top ||
                position.Y > _clipRegion.Bottom)
            {
                return;
            }

            _screenBuffer.SetPixel((int)(position.X + Offset.X), (int)(position.Y + Offset.Y), _pens[colour].Color);
        }

        public void DrawPixelFast(Vector2 position, Colour colour) =>

            // Is there a faster way of doing this?
            DrawPixel(position, colour);

        public void DrawPolygon(Vector2[] pointList, Colour lineColour)
        {
            PointF[] points = new PointF[pointList.Length];

            for (int i = 0; i < pointList.Length; i++)
            {
                points[i] = new PointF(pointList[i].X + Offset.X, pointList[i].Y + Offset.Y);
            }

            _screenBufferGraphics.DrawPolygon(_pens[lineColour], points);
        }

        public void DrawPolygonFilled(Vector2[] pointList, Colour faceColour)
        {
            PointF[] points = new PointF[pointList.Length];

            for (int i = 0; i < pointList.Length; i++)
            {
                points[i] = new PointF(pointList[i].X + Offset.X, pointList[i].Y + Offset.Y);
            }

            _screenBufferGraphics.FillPolygon(_pens[faceColour].Brush, points);
        }

        public void DrawRectangle(Vector2 position, float width, float height, Colour colour) =>
            _screenBufferGraphics.DrawRectangle(_pens[colour], position.X + Offset.X, position.Y + Offset.Y, width + Offset.X, height + Offset.Y);

        public void DrawRectangleFilled(Vector2 position, float width, float height, Colour colour) =>
            _screenBufferGraphics.FillRectangle(_pens[colour].Brush, position.X + Offset.X, position.Y + Offset.Y, width + Offset.X, height + Offset.Y);

        public void DrawTextCentre(float y, string text, FontSize fontSize, Colour colour)
        {
            using StringFormat stringFormat = new()
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center,
            };

            PointF point = new((128 * Scale) + Offset.X, (y / (2 / Scale)) + Offset.Y);
            _screenBufferGraphics.DrawString(
                text,
                fontSize == FontSize.Large ? _fontLarge : _fontSmall,
                _pens[colour].Brush,
                point,
                stringFormat);
        }

        public void DrawTextLeft(float x, float y, string text, Colour colour)
        {
            PointF point = new((x / (2 / Scale)) + Offset.X, (y / (2 / Scale)) + Offset.Y);
            _screenBufferGraphics.DrawString(text, _fontSmall, _pens[colour].Brush, point);
        }

        public void DrawTextRight(float x, float y, string text, Colour colour)
        {
            using StringFormat stringFormat = new()
            {
                Alignment = StringAlignment.Far,
            };

            PointF point = new((x / (2 / Scale)) + Offset.X, (y / (2 / Scale)) + Offset.Y);
            _screenBufferGraphics.DrawString(text, _fontSmall, _pens[colour].Brush, point, stringFormat);
        }

        public void DrawTriangle(Vector2 a, Vector2 b, Vector2 c, Colour colour)
        {
            PointF[] points = new PointF[3]
            {
                new(a.X += Offset.X, a.Y += Offset.Y),
                new(b.X += Offset.X, b.Y += Offset.Y),
                new(c.X += Offset.X, c.Y += Offset.Y),
            };

            _screenBufferGraphics.DrawLines(_pens[colour], points);
        }

        public void DrawTriangleFilled(Vector2 a, Vector2 b, Vector2 c, Colour colour)
        {
            PointF[] points = new PointF[3]
            {
                new(a.X += Offset.X, a.Y += Offset.Y),
                new(b.X += Offset.X, b.Y += Offset.Y),
                new(c.X += Offset.X, c.Y += Offset.Y),
            };

            _screenBufferGraphics.FillPolygon(_pens[colour].Brush, points);
        }

        public void LoadBitmap(Graphics.Image imgType, byte[] bitmapBytes)
        {
            _images[imgType] = (Bitmap)System.Drawing.Image.FromStream(new MemoryStream(bitmapBytes));
            _images[imgType].MakeTransparent(_transparentColour);
        }

        public void ScreenAcquire()
        {
            //acquire_bitmap(graphics_screen);
        }

        public void ScreenRelease()
        {
            //release_bitmap(graphics_screen);
        }

        /// <summary>
        /// Blit the back buffer to the screen.
        /// </summary>
        public void ScreenUpdate()
        {
            lock (_screenLock)
            {
                _screenGraphics.DrawImage(_screenBuffer, Offset.X, Offset.Y);
            }
        }

        public void SetClipRegion(Vector2 position, float width, float height)
        {
            _clipRegion = new RectangleF(position.X + Offset.X, position.Y + Offset.Y, width + Offset.X, height + Offset.Y);
            _screenBufferGraphics.Clip = new Region(_clipRegion);
        }

        private void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    // dispose managed state (managed objects)
                    _screenBufferGraphics.Dispose();
                    _screenBuffer.Dispose();
                    _screenGraphics.Dispose();
                    _screen.Dispose();
                    _fontSmall.Dispose();
                    _fontLarge.Dispose();

                    // Images
                    foreach (KeyValuePair<Graphics.Image, Bitmap> image in _images)
                    {
                        image.Value.Dispose();
                    }
                }

                // free unmanaged resources (unmanaged objects) and override finalizer
                // set large fields to null
                _isDisposed = true;
            }
        }

        // // Override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~alg_graphics()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }
    }
}
