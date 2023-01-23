/**
 *
 * Elite - The New Kind.
 *
 * Allegro version of Graphics routines.
 *
 * The code in this file has not been derived from the original Elite code.
 * Written by C.J.Pinder 1999-2001.
 * email: <christian@newkind.co.uk>
 *
 * Routines for drawing anti-aliased lines and circles by T.Harte.
 *
 **/

namespace Elite
{
	using System.Diagnostics;
	using System.Drawing;
	using System.Numerics;
    using Elite.Enums;

	public class alg_gfx : IGfx, IDisposable
	{
        // Screen buffer
        private readonly Bitmap _screenBuffer;
        private readonly Graphics _screenBufferGraphics;

        // Actual screen
        private readonly Bitmap _screen;
        private readonly Graphics _screenGraphics;

		// Fonts
        private readonly Font _fontSmall = new ("Arial", 12, FontStyle.Bold, GraphicsUnit.Pixel);
        private readonly Font _fontLarge = new("Arial", 18, FontStyle.Bold, GraphicsUnit.Pixel);

		// Images
		private readonly Dictionary<IMG, Bitmap> _images = new();

        private volatile int frame_count;
        private object frameCountLock = new();
        private System.Windows.Forms.Timer _frameTimer;

        private readonly Dictionary<GFX_COL, Pen> _pens = new()
            {
                { GFX_COL.GFX_COL_BLACK, Pens.Black },
                { GFX_COL.GFX_COL_WHITE, Pens.White },
                { GFX_COL.GFX_COL_WHITE_2, Pens.WhiteSmoke },
                { GFX_COL.GFX_COL_CYAN, Pens.Cyan },
                { GFX_COL.GFX_COL_GREY_1, Pens.LightGray },
                { GFX_COL.GFX_COL_GREY_2, Pens.DimGray },
                { GFX_COL.GFX_COL_GREY_3, Pens.Gray },
                { GFX_COL.GFX_COL_GREY_4, Pens.DarkGray },
                { GFX_COL.GFX_COL_BLUE_1, Pens.DarkBlue },
                { GFX_COL.GFX_COL_BLUE_2, Pens.Blue },
                { GFX_COL.GFX_COL_BLUE_3, Pens.MediumBlue },
                { GFX_COL.GFX_COL_BLUE_4, Pens.LightBlue },
                { GFX_COL.GFX_COL_RED, Pens.Red },
                { GFX_COL.GFX_COL_RED_3, Pens.PaleVioletRed },
                { GFX_COL.GFX_COL_RED_4, Pens.MediumVioletRed },
                { GFX_COL.GFX_COL_DARK_RED, Pens.DarkRed },
                { GFX_COL.GFX_COL_YELLOW_1, Pens.Goldenrod },
                { GFX_COL.GFX_COL_GOLD, Pens.Gold },
                { GFX_COL.GFX_COL_YELLOW_3, Pens.Yellow },
                { GFX_COL.GFX_COL_YELLOW_4, Pens.LightYellow },
                { GFX_COL.GFX_COL_YELLOW_5, Pens.LightGoldenrodYellow },
                { GFX_COL.GFX_COL_ORANGE_1, Pens.DarkOrange },
                { GFX_COL.GFX_COL_ORANGE_2, Pens.OrangeRed },
                { GFX_COL.GFX_COL_ORANGE_3, Pens.Orange },
                { GFX_COL.GFX_COL_GREEN_1, Pens.DarkGreen },
                { GFX_COL.GFX_COL_GREEN_2, Pens.Green },
                { GFX_COL.GFX_COL_GREEN_3, Pens.LightGreen },
                { GFX_COL.GFX_COL_PINK_1, Pens.Pink },
                { GFX_COL.UNKNOWN_1, Pens.Orange },
                { GFX_COL.UNKNOWN_2, Pens.DarkOrange }
            };

        private readonly Dictionary<GFX_COL, Brush> _brushes = new()
            {
                { GFX_COL.GFX_COL_BLACK, Brushes.Black },
                { GFX_COL.GFX_COL_WHITE, Brushes.White },
                { GFX_COL.GFX_COL_WHITE_2, Brushes.WhiteSmoke },
                { GFX_COL.GFX_COL_CYAN, Brushes.Cyan },
                { GFX_COL.GFX_COL_GREY_1, Brushes.LightGray },
                { GFX_COL.GFX_COL_GREY_2, Brushes.DimGray },
                { GFX_COL.GFX_COL_GREY_3, Brushes.Gray },
                { GFX_COL.GFX_COL_GREY_4, Brushes.DarkGray },
                { GFX_COL.GFX_COL_BLUE_1, Brushes.DarkBlue },
                { GFX_COL.GFX_COL_BLUE_2, Brushes.Blue },
                { GFX_COL.GFX_COL_BLUE_3, Brushes.MediumBlue },
                { GFX_COL.GFX_COL_BLUE_4, Brushes.LightBlue },
                { GFX_COL.GFX_COL_RED, Brushes.Red },
                { GFX_COL.GFX_COL_RED_3, Brushes.PaleVioletRed },
                { GFX_COL.GFX_COL_RED_4, Brushes.MediumVioletRed },
                { GFX_COL.GFX_COL_DARK_RED, Brushes.DarkRed },
                { GFX_COL.GFX_COL_YELLOW_1, Brushes.Goldenrod },
                { GFX_COL.GFX_COL_GOLD, Brushes.Gold },
                { GFX_COL.GFX_COL_YELLOW_3, Brushes.Yellow },
                { GFX_COL.GFX_COL_YELLOW_4, Brushes.LightYellow },
                { GFX_COL.GFX_COL_YELLOW_5, Brushes.LightGoldenrodYellow },
                { GFX_COL.GFX_COL_ORANGE_1, Brushes.DarkOrange },
                { GFX_COL.GFX_COL_ORANGE_2, Brushes.OrangeRed },
                { GFX_COL.GFX_COL_ORANGE_3, Brushes.Orange },
                { GFX_COL.GFX_COL_GREEN_1, Brushes.DarkGreen },
                { GFX_COL.GFX_COL_GREEN_2, Brushes.Green },
                { GFX_COL.GFX_COL_GREEN_3, Brushes.LightGreen },
                { GFX_COL.GFX_COL_PINK_1, Brushes.Pink },
                { GFX_COL.UNKNOWN_1, Brushes.Orange },
                { GFX_COL.UNKNOWN_2, Brushes.DarkOrange }
            };

        public alg_gfx(ref Bitmap screen)
		{
            Debug.Assert(screen.Width == 512);
            Debug.Assert(screen.Height == 512);
            Debug.Assert(_pens.Count == Enum.GetNames(typeof(GFX_COL)).Length);
            Debug.Assert(_brushes.Count == Enum.GetNames(typeof(GFX_COL)).Length);

            _screen = screen;
            _screenGraphics = Graphics.FromImage(_screen);
            _screenBuffer = new Bitmap(screen.Width, screen.Height);
            _screenBufferGraphics = Graphics.FromImage(_screenBuffer);
            _screenBufferGraphics.Clear(Color.Black);

            // Install a timer to regulate the speed of the game...
            lock (frameCountLock)
            {
                frame_count = 0;
            }

            _frameTimer = new()
            {
                // Approx matxh the speed of the TNK
                Interval = 5000 / SpeedCap
            };
            _frameTimer.Tick += _frameTimer_Tick;
            _frameTimer.Start();
        }

        private bool disposedValue;

        private void _frameTimer_Tick(object? sender, EventArgs e)
        {
            lock (frameCountLock)
            {
                frame_count++;
            }
        }

        public void LoadBitmap(IMG imgType, Stream bitmapStream)
        {
            _images[imgType] = (Bitmap)Image.FromStream(bitmapStream);
        }

        public int SpeedCap { get; set; } = 75;

        /// <summary>
        /// Blit the back buffer to the screen.
        /// </summary>
		public void ScreenUpdate()
		{
            while (frame_count < 1)
            {
                Thread.Sleep(10);
                // TODO: find a better way of doing multithreading
                Application.DoEvents();
            }

            lock (frameCountLock)
            {
                frame_count = 0;
            }

            lock (_screen)
			{
				_screenGraphics.DrawImage(_screenBuffer, gfx.GFX_X_OFFSET, gfx.GFX_Y_OFFSET);
			}

            Application.DoEvents();
        }

        public void ScreenAcquire()
		{
            //acquire_bitmap(gfx_screen);
        }

        public void ScreenRelease()
		{
            //release_bitmap(gfx_screen);
		}

		public void DrawPixelFast(Vector2 position, GFX_COL col)
		{
            // Is there a faster way of doing this?
            DrawPixel(position, col);
        }

        public void DrawPixel(Vector2 position, GFX_COL col)
		{
            //TODO: Fix SNES planet colour issues
            Color colour = _pens.TryGetValue(col, out Pen value) ? value.Color : Color.Magenta;

            //TODO: fix bad values from explosion
            if (position.X < 0 || position.X >= 512 || position.Y < 0 || position.Y >= 512)
            {
                return;
            }

            _screenBuffer.SetPixel((int)(position.X + gfx.GFX_X_OFFSET), (int)(position.Y + gfx.GFX_Y_OFFSET), colour);
        }

        public void DrawCircleFilled(Vector2 centre, float radius, GFX_COL colour)
		{
            _screenBufferGraphics.FillEllipse(_brushes[colour], centre.X + gfx.GFX_X_OFFSET - radius, centre.Y + gfx.GFX_Y_OFFSET - radius, 2 * radius, 2 * radius);
        }

        public virtual void DrawCircle(Vector2 centre, float radius, GFX_COL colour)
		{
            _screenBufferGraphics.DrawEllipse(_pens[colour], centre.X + gfx.GFX_X_OFFSET - radius, centre.Y + gfx.GFX_Y_OFFSET - radius, 2 * radius, 2 * radius);
        }

        public virtual void DrawLine(Vector2 start, Vector2 end)
		{
            _screenBufferGraphics.DrawLine(_pens[GFX_COL.GFX_COL_WHITE], start.X + gfx.GFX_X_OFFSET, start.Y + gfx.GFX_Y_OFFSET, end.X + gfx.GFX_X_OFFSET, end.Y + gfx.GFX_Y_OFFSET);
        }

		public void DrawLine(Vector2 start, Vector2 end, GFX_COL line_colour)
		{
			_screenBufferGraphics.DrawLine(_pens[line_colour], start.X + gfx.GFX_X_OFFSET, start.Y + gfx.GFX_Y_OFFSET, end.X + gfx.GFX_X_OFFSET, end.Y + gfx.GFX_Y_OFFSET);
		}

        public void DrawTriangle(Vector2 a, Vector2 b, Vector2 c, GFX_COL colour)
        {
            PointF[] points = new PointF[3]
            {
                new(a.X += gfx.GFX_X_OFFSET, a.Y += gfx.GFX_Y_OFFSET),
                new(b.X += gfx.GFX_X_OFFSET, b.Y += gfx.GFX_Y_OFFSET),
                new(c.X += gfx.GFX_X_OFFSET, c.Y += gfx.GFX_Y_OFFSET)
            };

            _screenBufferGraphics.DrawLines(_pens[colour], points);
        }

        public void DrawTriangleFilled(Vector2 a, Vector2 b, Vector2 c, GFX_COL colour)
		{
            PointF[] points = new PointF[3] 
            { 
                new(a.X += gfx.GFX_X_OFFSET, a.Y += gfx.GFX_Y_OFFSET), 
                new(b.X += gfx.GFX_X_OFFSET, b.Y += gfx.GFX_Y_OFFSET), 
                new(c.X += gfx.GFX_X_OFFSET, c.Y += gfx.GFX_Y_OFFSET)
            };

            _screenBufferGraphics.FillPolygon(_brushes[colour], points);
        }

		public void DrawTextLeft(float x, float y, string text, GFX_COL colour)
		{
            PointF point = new((x / (2 / gfx.GFX_SCALE)) + gfx.GFX_X_OFFSET, (y / (2 / gfx.GFX_SCALE)) + gfx.GFX_Y_OFFSET);
            _screenBufferGraphics.DrawString(text, _fontSmall, _brushes[colour], point);
        }

        public void DrawTextRight(float x, float y, string text, GFX_COL colour)
        {
            StringFormat stringFormat = new()
            {
                Alignment = StringAlignment.Far,
            };

            PointF point = new((x / (2 / gfx.GFX_SCALE)) + gfx.GFX_X_OFFSET, (y / (2 / gfx.GFX_SCALE)) + gfx.GFX_Y_OFFSET);
            _screenBufferGraphics.DrawString(text, _fontSmall, _brushes[colour], point, stringFormat);
        }

        public void DrawTextCentre(float y, string text, int psize, GFX_COL colour)
		{
            StringFormat stringFormat = new()
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            PointF point = new((128 * gfx.GFX_SCALE) + gfx.GFX_X_OFFSET, (y / (2 / gfx.GFX_SCALE)) + gfx.GFX_Y_OFFSET);
            _screenBufferGraphics.DrawString(
				text,
                psize == 140 ? _fontLarge : _fontSmall, 
				_brushes[colour], 
				point, 
				stringFormat);
        }

		public void ClearArea(float x, float y, float width, float height)
		{
			_screenBufferGraphics.FillRectangle(Brushes.Black, x + gfx.GFX_X_OFFSET, y + gfx.GFX_Y_OFFSET, width + gfx.GFX_X_OFFSET, height + gfx.GFX_Y_OFFSET);
        }

		public void DrawRectangleFilled(float x, float y, float width, float height, GFX_COL colour)
		{
			_screenBufferGraphics.FillRectangle(_brushes[colour], x + gfx.GFX_X_OFFSET, y + gfx.GFX_Y_OFFSET, width + gfx.GFX_X_OFFSET, height + gfx.GFX_Y_OFFSET);
        }

        public void DrawRectangle(float x, float y, float width, float height, GFX_COL colour)
        {
            _screenBufferGraphics.DrawRectangle(_pens[colour], x + gfx.GFX_X_OFFSET, y + gfx.GFX_Y_OFFSET, width + gfx.GFX_X_OFFSET, height + gfx.GFX_Y_OFFSET);
        }

		public void SetClipRegion(float x, float y, float width, float height)
		{
            _screenBufferGraphics.Clip = new Region(new RectangleF(x + gfx.GFX_X_OFFSET, y + gfx.GFX_Y_OFFSET, width + gfx.GFX_X_OFFSET, height + gfx.GFX_Y_OFFSET));
        }

        public void DrawPolygonFilled(Vector2[] vectors, GFX_COL faceColour)
        {
            PointF[] points = new PointF[vectors.Length];

            for (int i = 0; i < vectors.Length; i++)
            {
                points[i] = new PointF(vectors[i].X + gfx.GFX_X_OFFSET, vectors[i].Y + gfx.GFX_Y_OFFSET);
            }

            _screenBufferGraphics.FillPolygon(_brushes[faceColour], points);
        }

        public void DrawPolygon(Vector2[] vectors, GFX_COL lineColour)
        {
            PointF[] points = new PointF[vectors.Length];

            for (int i = 0; i < vectors.Length; i++)
            {
                points[i] = new PointF(vectors[i].X + gfx.GFX_X_OFFSET, vectors[i].Y + gfx.GFX_Y_OFFSET);
            }

            _screenBufferGraphics.DrawPolygon(_pens[lineColour], points);
        }

        public void DrawImage(IMG image, Vector2 position)
        {
            Bitmap sprite = _images[image];

            if (position.X < 0)
            {
                position.X = ((256 * gfx.GFX_SCALE) - sprite.Width) / 2;
            }

            _screenBufferGraphics.DrawImage(sprite, position.X + gfx.GFX_X_OFFSET, position.Y + gfx.GFX_Y_OFFSET);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
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
					foreach(KeyValuePair<IMG, Bitmap> image in _images)
					{
						image.Value.Dispose();
					}
				}

                // free unmanaged resources (unmanaged objects) and override finalizer
                // set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~alg_gfx()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}