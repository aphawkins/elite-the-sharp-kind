// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Collections.Concurrent;
using System.Drawing.Imaging;
using System.Numerics;
using EliteSharp.Graphics;

namespace EliteSharp.WinForms
{
    public sealed class GDIGraphics : IGraphics
    {
        private readonly Font _fontLarge = new("Arial", 18, FontStyle.Bold, GraphicsUnit.Pixel);
        private readonly Font _fontSmall = new("Arial", 12, FontStyle.Bold, GraphicsUnit.Pixel);
        private readonly ConcurrentDictionary<ImageType, Bitmap> _images = new();
        private readonly Dictionary<FastColor, Pen> _pens = [];
        private readonly Bitmap _screen;
        private readonly Bitmap _screenBuffer;
        private readonly System.Drawing.Graphics _screenBufferGraphics;
        private readonly System.Drawing.Graphics _screenGraphics;
        private readonly object _screenLock = new();
        private readonly Action<Bitmap> _screenUpdate;
        private RectangleF _clipRegion;
        private bool _isDisposed;

        public GDIGraphics(float screenWidth, float screenHeight, Action<Bitmap> screenUpdate)
        {
            ScreenWidth = screenWidth;
            ScreenHeight = screenHeight;
            _screen = new((int)screenWidth, (int)screenHeight, PixelFormat.Format32bppArgb);
            _screenUpdate = screenUpdate;
            _screenGraphics = System.Drawing.Graphics.FromImage(_screen);
            _screenBuffer = new Bitmap(_screen.Width, _screen.Height, _screen.PixelFormat);
            _screenBufferGraphics = System.Drawing.Graphics.FromImage(_screenBuffer);
            _screenBufferGraphics.Clear(Color.Black);
            ScreenWidth = _screen.Width;
            ScreenHeight = _screen.Height;

            foreach (FastColor colour in FastColors.AllColors())
            {
                Pen pen = new(Color.FromArgb(colour.Argb));
                _pens.Add(colour, pen);
            }
        }

        public float ScreenHeight { get; }

        public float Scale { get; } = 2;

        public float ScreenWidth { get; }

        public void Clear()
        {
            if (_isDisposed)
            {
                return;
            }

            _screenBufferGraphics.Clear(Color.Black);
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public void DrawCircle(Vector2 centre, float radius, FastColor colour)
        {
            if (_isDisposed)
            {
                return;
            }

            _screenBufferGraphics.DrawEllipse(_pens[colour], centre.X - radius, centre.Y - radius, 2 * radius, 2 * radius);
        }

        public void DrawCircleFilled(Vector2 centre, float radius, FastColor colour)
        {
            if (_isDisposed)
            {
                return;
            }

            _screenBufferGraphics.FillEllipse(_pens[colour].Brush, centre.X - radius, centre.Y - radius, 2 * radius, 2 * radius);
        }

        public void DrawImage(ImageType image, Vector2 position)
        {
            if (_isDisposed)
            {
                return;
            }

            _screenBufferGraphics.DrawImage(
                _images[image],
                position.X,
                position.Y,
                _images[image].Width,
                _images[image].Height);
        }

        public void DrawImageCentre(ImageType image, float y)
        {
            float x = (ScreenWidth - _images[image].Width) / 2;
            DrawImage(image, new(x, y));
        }

        public void DrawLine(Vector2 lineStart, Vector2 lineEnd, FastColor colour)
        {
            if (_isDisposed)
            {
                return;
            }

            _screenBufferGraphics.DrawLine(_pens[colour], lineStart.X, lineStart.Y, lineEnd.X, lineEnd.Y);
        }

        public void DrawPixel(Vector2 position, FastColor colour)
        {
            if (_isDisposed)
            {
                return;
            }

            // Prevent SetPixel from drawing outside of the clip region
            if (position.X < _clipRegion.Left ||
                position.X > _clipRegion.Right ||
                position.Y < _clipRegion.Top ||
                position.Y > _clipRegion.Bottom)
            {
                return;
            }

            _screenBuffer.SetPixel((int)position.X, (int)position.Y, _pens[colour].Color);
        }

        public void DrawPixelFast(Vector2 position, FastColor colour)
            => DrawPixel(position, colour); // Is there a faster way of doing this?

        public void DrawPolygon(Vector2[] points, FastColor lineColour)
        {
            if (_isDisposed || points == null)
            {
                return;
            }

            PointF[] drawPoints = new PointF[points.Length];

            for (int i = 0; i < points.Length; i++)
            {
                drawPoints[i] = new PointF(points[i].X, points[i].Y);
            }

            _screenBufferGraphics.DrawPolygon(_pens[lineColour], drawPoints);
        }

        public void DrawPolygonFilled(Vector2[] points, FastColor faceColour)
        {
            if (_isDisposed || points == null)
            {
                return;
            }

            PointF[] drawPoints = new PointF[points.Length];

            for (int i = 0; i < points.Length; i++)
            {
                drawPoints[i] = new PointF(points[i].X, points[i].Y);
            }

            _screenBufferGraphics.FillPolygon(_pens[faceColour].Brush, drawPoints);
        }

        public void DrawRectangle(Vector2 position, float width, float height, FastColor colour)
        {
            if (_isDisposed)
            {
                return;
            }

            _screenBufferGraphics.DrawRectangle(_pens[colour], position.X, position.Y, width, height);
        }

        public void DrawRectangleCentre(float y, float width, float height, FastColor colour)
        {
            if (_isDisposed)
            {
                return;
            }

            _screenBufferGraphics.DrawRectangle(
                _pens[colour],
                (ScreenWidth - width) / 2,
                y / (2 / Scale),
                width,
                height);
        }

        public void DrawRectangleFilled(Vector2 position, float width, float height, FastColor colour)
        {
            if (_isDisposed)
            {
                return;
            }

            _screenBufferGraphics.FillRectangle(_pens[colour].Brush, position.X, position.Y, width, height);
        }

        public void DrawTextCentre(float y, string text, FontSize fontSize, FastColor colour)
        {
            if (_isDisposed)
            {
                return;
            }

            using StringFormat stringFormat = new()
            {
                Alignment = StringAlignment.Center,
            };

            _screenBufferGraphics.DrawString(
                text,
                fontSize == FontSize.Large ? _fontLarge : _fontSmall,
                _pens[colour].Brush,
                ScreenWidth / 2,
                y / (2 / Scale),
                stringFormat);
        }

        public void DrawTextLeft(Vector2 position, string text, FastColor colour)
        {
            if (_isDisposed)
            {
                return;
            }

            PointF point = new(position.X / (2 / Scale), position.Y / (2 / Scale));
            _screenBufferGraphics.DrawString(text, _fontSmall, _pens[colour].Brush, point);
        }

        public void DrawTextRight(Vector2 position, string text, FastColor colour)
        {
            if (_isDisposed)
            {
                return;
            }

            using StringFormat stringFormat = new()
            {
                Alignment = StringAlignment.Far,
            };

            _screenBufferGraphics.DrawString(
                text,
                _fontSmall,
                _pens[colour].Brush,
                position.X / (2 / Scale),
                position.Y / (2 / Scale),
                stringFormat);
        }

        public void DrawTriangle(Vector2 a, Vector2 b, Vector2 c, FastColor colour)
        {
            if (_isDisposed)
            {
                return;
            }

            PointF[] points =
            [
                new(a.X, a.Y),
                new(b.X, b.Y),
                new(c.X, c.Y),
            ];

            _screenBufferGraphics.DrawLines(_pens[colour], points);
        }

        public void DrawTriangleFilled(Vector2 a, Vector2 b, Vector2 c, FastColor colour)
        {
            if (_isDisposed)
            {
                return;
            }

            PointF[] points =
            [
                new(a.X, a.Y),
                new(b.X, b.Y),
                new(c.X, c.Y),
            ];

            _screenBufferGraphics.FillPolygon(_pens[colour].Brush, points);
        }

        public void LoadBitmap(ImageType imgType, string bitmapPath)
            => _images[imgType] = (Bitmap)Image.FromFile(bitmapPath);

        /// <summary>
        /// Blit the back buffer to the screen.
        /// </summary>
        public void ScreenUpdate()
        {
            if (_isDisposed)
            {
                return;
            }

            lock (_screenLock)
            {
                _screenGraphics.DrawImage(_screenBuffer, 0, 0);
            }

            _screenUpdate(_screen);
        }

        public void SetClipRegion(Vector2 position, float width, float height)
        {
            if (_isDisposed)
            {
                return;
            }

            _clipRegion = new RectangleF(position.X, position.Y, width, height);
            _screenBufferGraphics.Clip = new Region(_clipRegion);
        }

        private void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    // dispose managed state (managed objects)
                    _screenBufferGraphics?.Dispose();
                    _screenBuffer?.Dispose();
                    _screenGraphics?.Dispose();
                    _screen?.Dispose();
                    _fontSmall?.Dispose();
                    _fontLarge?.Dispose();

                    // Images
                    foreach (KeyValuePair<ImageType, Bitmap> image in _images)
                    {
                        image.Value.Dispose();
                    }
                }

                // free unmanaged resources (unmanaged objects) and override finalizer
                // set large fields to null
                _isDisposed = true;
            }
        }
    }
}
