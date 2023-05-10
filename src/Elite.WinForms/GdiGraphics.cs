// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Diagnostics;
using System.Numerics;
using Elite.Engine;
using Elite.Engine.Enums;

namespace Elite.WinForms
{
    public class GdiGraphics : IGfx, IDisposable
    {
        private readonly Dictionary<Colour, Brush> _brushes = new()
            {
                { Colour.Black, Brushes.Black },
                { Colour.White1, Brushes.White },
                { Colour.White2, Brushes.WhiteSmoke },
                { Colour.Cyan, Brushes.Cyan },
                { Colour.Grey1, Brushes.LightGray },
                { Colour.Grey2, Brushes.DimGray },
                { Colour.Grey3, Brushes.Gray },
                { Colour.Grey4, Brushes.DarkGray },
                { Colour.Blue1, Brushes.DarkBlue },
                { Colour.Blue2, Brushes.Blue },
                { Colour.Blue3, Brushes.MediumBlue },
                { Colour.Blue4, Brushes.LightBlue },
                { Colour.Red1, Brushes.Red },
                { Colour.Red3, Brushes.PaleVioletRed },
                { Colour.Red4, Brushes.MediumVioletRed },
                { Colour.Red2, Brushes.DarkRed },
                { Colour.Yellow1, Brushes.Goldenrod },
                { Colour.Gold, Brushes.Gold },
                { Colour.Yellow3, Brushes.Yellow },
                { Colour.Yellow4, Brushes.LightYellow },
                { Colour.Yellow5, Brushes.LightGoldenrodYellow },
                { Colour.Orange1, Brushes.DarkOrange },
                { Colour.Orange2, Brushes.OrangeRed },
                { Colour.Orange3, Brushes.Orange },
                { Colour.Green1, Brushes.DarkGreen },
                { Colour.Green2, Brushes.Green },
                { Colour.Green3, Brushes.LightGreen },
                { Colour.Pink1, Brushes.Pink },
            };

        private readonly Font _fontLarge = new("Arial", 18, FontStyle.Bold, GraphicsUnit.Pixel);

        // Fonts
        private readonly Font _fontSmall = new("Arial", 12, FontStyle.Bold, GraphicsUnit.Pixel);

        // Images
        private readonly Dictionary<Common.Enums.Image, Bitmap> _images = new();

        private readonly Dictionary<Colour, Pen> _pens = new()
            {
                { Colour.Black, Pens.Black },
                { Colour.White1, Pens.White },
                { Colour.White2, Pens.WhiteSmoke },
                { Colour.Cyan, Pens.Cyan },
                { Colour.Grey1, Pens.LightGray },
                { Colour.Grey2, Pens.DimGray },
                { Colour.Grey3, Pens.Gray },
                { Colour.Grey4, Pens.DarkGray },
                { Colour.Blue1, Pens.DarkBlue },
                { Colour.Blue2, Pens.Blue },
                { Colour.Blue3, Pens.MediumBlue },
                { Colour.Blue4, Pens.LightBlue },
                { Colour.Red1, Pens.Red },
                { Colour.Red3, Pens.PaleVioletRed },
                { Colour.Red4, Pens.MediumVioletRed },
                { Colour.Red2, Pens.DarkRed },
                { Colour.Yellow1, Pens.Goldenrod },
                { Colour.Gold, Pens.Gold },
                { Colour.Yellow3, Pens.Yellow },
                { Colour.Yellow4, Pens.LightYellow },
                { Colour.Yellow5, Pens.LightGoldenrodYellow },
                { Colour.Orange1, Pens.DarkOrange },
                { Colour.Orange2, Pens.OrangeRed },
                { Colour.Orange3, Pens.Orange },
                { Colour.Green1, Pens.DarkGreen },
                { Colour.Green2, Pens.Green },
                { Colour.Green3, Pens.LightGreen },
                { Colour.Pink1, Pens.Pink },
            };

        // Actual screen
        private readonly Bitmap _screen;

        // Screen buffer
        private readonly Bitmap _screenBuffer;

        private readonly System.Drawing.Graphics _screenBufferGraphics;
        private readonly System.Drawing.Graphics _screenGraphics;
        private bool _isDisposed;

        //private volatile int frame_count;
        //private readonly object frameCountLock = new();
        //private readonly System.Windows.Forms.Timer _frameTimer;
        public GdiGraphics(ref Bitmap screen)
        {
            Debug.Assert(screen.Width == 512);
            Debug.Assert(screen.Height == 512);
            Debug.Assert(_pens.Count == Enum.GetNames(typeof(Colour)).Length);
            Debug.Assert(_brushes.Count == Enum.GetNames(typeof(Colour)).Length);

            _screen = screen;
            _screenGraphics = System.Drawing.Graphics.FromImage(_screen);
            _screenBuffer = new Bitmap(screen.Width, screen.Height);
            _screenBufferGraphics = System.Drawing.Graphics.FromImage(_screenBuffer);
            _screenBufferGraphics.Clear(Color.Black);
        }

        public void ClearArea(float x, float y, float width, float height) => _screenBufferGraphics.FillRectangle(Brushes.Black, x + Engine.Graphics.GFX_X_OFFSET, y + Engine.Graphics.GFX_Y_OFFSET, width + Engine.Graphics.GFX_X_OFFSET, height + Engine.Graphics.GFX_Y_OFFSET);

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public virtual void DrawCircle(Vector2 centre, float radius, Colour colour) => _screenBufferGraphics.DrawEllipse(_pens[colour], centre.X + Engine.Graphics.GFX_X_OFFSET - radius, centre.Y + Engine.Graphics.GFX_Y_OFFSET - radius, 2 * radius, 2 * radius);

        public void DrawCircleFilled(Vector2 centre, float radius, Colour colour) => _screenBufferGraphics.FillEllipse(_brushes[colour], centre.X + Engine.Graphics.GFX_X_OFFSET - radius, centre.Y + Engine.Graphics.GFX_Y_OFFSET - radius, 2 * radius, 2 * radius);

        public void DrawImage(Common.Enums.Image spriteImgage, Vector2 location)
        {
            Bitmap sprite = _images[spriteImgage];

            if (location.X < 0)
            {
                location.X = ((256 * Engine.Graphics.GFX_SCALE) - sprite.Width) / 2;
            }

            _screenBufferGraphics.DrawImage(sprite, location.X + Engine.Graphics.GFX_X_OFFSET, location.Y + Engine.Graphics.GFX_Y_OFFSET);
        }

        public virtual void DrawLine(Vector2 start, Vector2 end) => _screenBufferGraphics.DrawLine(_pens[Colour.White1], start.X + Engine.Graphics.GFX_X_OFFSET, start.Y + Engine.Graphics.GFX_Y_OFFSET, end.X + Engine.Graphics.GFX_X_OFFSET, end.Y + Engine.Graphics.GFX_Y_OFFSET);

        public void DrawLine(Vector2 start, Vector2 end, Colour colour) => _screenBufferGraphics.DrawLine(_pens[colour], start.X + Engine.Graphics.GFX_X_OFFSET, start.Y + Engine.Graphics.GFX_Y_OFFSET, end.X + Engine.Graphics.GFX_X_OFFSET, end.Y + Engine.Graphics.GFX_Y_OFFSET);

        public void DrawPixel(Vector2 position, Colour colour)
        {
            //TODO: Fix SNES planet colour issues
            Color color = _pens.TryGetValue(colour, out Pen? value) ? value.Color : Color.Magenta;

            Debug.Assert(color != Color.Magenta);

            //TODO: fix bad values from explosion
            if (position.X < 0 || position.X >= 512 || position.Y < 0 || position.Y >= 512)
            {
                return;
            }

            _screenBuffer.SetPixel((int)(position.X + Engine.Graphics.GFX_X_OFFSET), (int)(position.Y + Engine.Graphics.GFX_Y_OFFSET), color);
        }

        public void DrawPixelFast(Vector2 position, Colour colour) =>
            // Is there a faster way of doing this?
            DrawPixel(position, colour);

        public void DrawPolygon(Vector2[] pointList, Colour lineColour)
        {
            PointF[] points = new PointF[pointList.Length];

            for (int i = 0; i < pointList.Length; i++)
            {
                points[i] = new PointF(pointList[i].X + Engine.Graphics.GFX_X_OFFSET, pointList[i].Y + Engine.Graphics.GFX_Y_OFFSET);
            }

            _screenBufferGraphics.DrawPolygon(_pens[lineColour], points);
        }

        public void DrawPolygonFilled(Vector2[] pointList, Colour faceColour)
        {
            PointF[] points = new PointF[pointList.Length];

            for (int i = 0; i < pointList.Length; i++)
            {
                points[i] = new PointF(pointList[i].X + Engine.Graphics.GFX_X_OFFSET, pointList[i].Y + Engine.Graphics.GFX_Y_OFFSET);
            }

            _screenBufferGraphics.FillPolygon(_brushes[faceColour], points);
        }

        public void DrawRectangle(float x, float y, float width, float height, Colour colour) => _screenBufferGraphics.DrawRectangle(_pens[colour], x + Engine.Graphics.GFX_X_OFFSET, y + Engine.Graphics.GFX_Y_OFFSET, width + Engine.Graphics.GFX_X_OFFSET, height + Engine.Graphics.GFX_Y_OFFSET);

        public void DrawRectangleFilled(float x, float y, float width, float height, Colour colour) => _screenBufferGraphics.FillRectangle(_brushes[colour], x + Engine.Graphics.GFX_X_OFFSET, y + Engine.Graphics.GFX_Y_OFFSET, width + Engine.Graphics.GFX_X_OFFSET, height + Engine.Graphics.GFX_Y_OFFSET);

        public void DrawTextCentre(float y, string text, int psize, Colour colour)
        {
            StringFormat stringFormat = new()
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            PointF point = new((128 * Engine.Graphics.GFX_SCALE) + Engine.Graphics.GFX_X_OFFSET, (y / (2 / Engine.Graphics.GFX_SCALE)) + Engine.Graphics.GFX_Y_OFFSET);
            _screenBufferGraphics.DrawString(
                text,
                psize == 140 ? _fontLarge : _fontSmall,
                _brushes[colour],
                point,
                stringFormat);
        }

        public void DrawTextLeft(float x, float y, string text, Colour colour)
        {
            PointF point = new((x / (2 / Engine.Graphics.GFX_SCALE)) + Engine.Graphics.GFX_X_OFFSET, (y / (2 / Engine.Graphics.GFX_SCALE)) + Engine.Graphics.GFX_Y_OFFSET);
            _screenBufferGraphics.DrawString(text, _fontSmall, _brushes[colour], point);
        }

        public void DrawTextRight(float x, float y, string text, Colour colour)
        {
            StringFormat stringFormat = new()
            {
                Alignment = StringAlignment.Far,
            };

            PointF point = new((x / (2 / Engine.Graphics.GFX_SCALE)) + Engine.Graphics.GFX_X_OFFSET, (y / (2 / Engine.Graphics.GFX_SCALE)) + Engine.Graphics.GFX_Y_OFFSET);
            _screenBufferGraphics.DrawString(text, _fontSmall, _brushes[colour], point, stringFormat);
        }

        public void DrawTriangle(Vector2 a, Vector2 b, Vector2 c, Colour colour)
        {
            PointF[] points = new PointF[3]
            {
                new(a.X += Engine.Graphics.GFX_X_OFFSET, a.Y += Engine.Graphics.GFX_Y_OFFSET),
                new(b.X += Engine.Graphics.GFX_X_OFFSET, b.Y += Engine.Graphics.GFX_Y_OFFSET),
                new(c.X += Engine.Graphics.GFX_X_OFFSET, c.Y += Engine.Graphics.GFX_Y_OFFSET)
            };

            _screenBufferGraphics.DrawLines(_pens[colour], points);
        }

        public void DrawTriangleFilled(Vector2 a, Vector2 b, Vector2 c, Colour colour)
        {
            PointF[] points = new PointF[3]
            {
                new(a.X += Engine.Graphics.GFX_X_OFFSET, a.Y += Engine.Graphics.GFX_Y_OFFSET),
                new(b.X += Engine.Graphics.GFX_X_OFFSET, b.Y += Engine.Graphics.GFX_Y_OFFSET),
                new(c.X += Engine.Graphics.GFX_X_OFFSET, c.Y += Engine.Graphics.GFX_Y_OFFSET)
            };

            _screenBufferGraphics.FillPolygon(_brushes[colour], points);
        }

        public void LoadBitmap(Common.Enums.Image imgType, Stream bitmapStream) => _images[imgType] = (Bitmap)Image.FromStream(bitmapStream);

        public void ScreenAcquire()
        {
            //acquire_bitmap(gfx_screen);
        }

        public void ScreenRelease()
        {
            //release_bitmap(gfx_screen);
        }

        /// <summary>
        /// Blit the back buffer to the screen.
        /// </summary>
		public void ScreenUpdate()
        {
            // TODO: find a better way of doing multithreading
            Application.DoEvents();

            lock (_screen)
            {
                _screenGraphics.DrawImage(_screenBuffer, Engine.Graphics.GFX_X_OFFSET, Engine.Graphics.GFX_Y_OFFSET);
            }

            Application.DoEvents();
        }

        public void SetClipRegion(float x, float y, float width, float height) => _screenBufferGraphics.Clip = new Region(new RectangleF(x + Engine.Graphics.GFX_X_OFFSET, y + Engine.Graphics.GFX_Y_OFFSET, width + Engine.Graphics.GFX_X_OFFSET, height + Engine.Graphics.GFX_Y_OFFSET));

        protected virtual void Dispose(bool disposing)
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
                    foreach (KeyValuePair<Common.Enums.Image, Bitmap> image in _images)
                    {
                        image.Value.Dispose();
                    }
                }

                // free unmanaged resources (unmanaged objects) and override finalizer
                // set large fields to null
                _isDisposed = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~alg_gfx()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }
    }
}
