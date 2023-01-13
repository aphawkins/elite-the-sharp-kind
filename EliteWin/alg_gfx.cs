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
		private Bitmap _imageScanner;
		private readonly Dictionary<IMG, Bitmap> _images = new();

        private volatile int frame_count;
        private object frameCountLock = new();
        private const int MAX_POLYS = 100;
        private int start_poly;
        private int total_polys;

        private System.Windows.Forms.Timer _frameTimer;

        private readonly Dictionary<GFX_COL, Pen> _pens = new()
            {
                { GFX_COL.GFX_COL_BLACK, Pens.Black },
                { GFX_COL.GFX_COL_WHITE, Pens.White },
                { GFX_COL.GFX_COL_WHITE_2, Pens.WhiteSmoke },
                { GFX_COL.GFX_COL_GOLD, Pens.Gold },
                { GFX_COL.GFX_COL_CYAN, Pens.Cyan },
                { GFX_COL.GFX_COL_GREY_1, Pens.DimGray },
                { GFX_COL.GFX_COL_GREY_2, Pens.LightGray },
                { GFX_COL.GFX_COL_GREY_3, Pens.Gray },
                { GFX_COL.GFX_COL_GREY_4, Pens.DarkGray },
                { GFX_COL.GFX_COL_BLUE_1, Pens.LightBlue },
                { GFX_COL.GFX_COL_BLUE_2, Pens.MediumBlue },
                { GFX_COL.GFX_COL_BLUE_3, Pens.Blue },
                { GFX_COL.GFX_COL_BLUE_4, Pens.DarkBlue },
                { GFX_COL.GFX_COL_RED, Pens.Red },
                { GFX_COL.GFX_COL_RED_3, Pens.PaleVioletRed },
                { GFX_COL.GFX_COL_RED_4, Pens.MediumVioletRed },
                { GFX_COL.GFX_COL_DARK_RED, Pens.DarkRed },
                { GFX_COL.GFX_COL_YELLOW_1, Pens.Yellow },
                { GFX_COL.GFX_COL_YELLOW_3, Pens.LightGoldenrodYellow },
                { GFX_COL.GFX_COL_YELLOW_4, Pens.LightYellow },
                { GFX_COL.GFX_COL_YELLOW_5, Pens.YellowGreen },
                { GFX_COL.GFX_COL_ORANGE_1, Pens.Orange },
                { GFX_COL.GFX_COL_ORANGE_2, Pens.OrangeRed },
                { GFX_COL.GFX_COL_ORANGE_3, Pens.DarkOrange },
                { GFX_COL.GFX_COL_GREEN_1, Pens.LightGreen },
                { GFX_COL.GFX_COL_GREEN_2, Pens.Green },
                { GFX_COL.GFX_COL_GREEN_3, Pens.DarkGreen },
                { GFX_COL.GFX_COL_PINK_1, Pens.Pink },
                { GFX_COL.UNKNOWN_1, Pens.Orange },
                { GFX_COL.UNKNOWN_2, Pens.DarkOrange }
            };

        private readonly Dictionary<GFX_COL, Brush> _brushes = new()
            {
                { GFX_COL.GFX_COL_BLACK, Brushes.Black },
                { GFX_COL.GFX_COL_WHITE, Brushes.White },
                { GFX_COL.GFX_COL_WHITE_2, Brushes.WhiteSmoke },
                { GFX_COL.GFX_COL_GOLD, Brushes.Gold },
                { GFX_COL.GFX_COL_CYAN, Brushes.Cyan },
                { GFX_COL.GFX_COL_GREY_1, Brushes.DimGray },
                { GFX_COL.GFX_COL_GREY_2, Brushes.LightGray },
                { GFX_COL.GFX_COL_GREY_3, Brushes.Gray },
                { GFX_COL.GFX_COL_GREY_4, Brushes.DarkGray },
                { GFX_COL.GFX_COL_BLUE_1, Brushes.LightBlue },
                { GFX_COL.GFX_COL_BLUE_2, Brushes.MediumBlue },
                { GFX_COL.GFX_COL_BLUE_3, Brushes.Blue },
                { GFX_COL.GFX_COL_BLUE_4, Brushes.DarkBlue },
                { GFX_COL.GFX_COL_RED, Brushes.Red },
                { GFX_COL.GFX_COL_RED_3, Brushes.PaleVioletRed },
                { GFX_COL.GFX_COL_RED_4, Brushes.MediumVioletRed },
                { GFX_COL.GFX_COL_DARK_RED, Brushes.DarkRed },
                { GFX_COL.GFX_COL_YELLOW_1, Brushes.Yellow },
                { GFX_COL.GFX_COL_YELLOW_3, Brushes.LightYellow },
                { GFX_COL.GFX_COL_YELLOW_4, Brushes.YellowGreen },
                { GFX_COL.GFX_COL_YELLOW_5, Brushes.GreenYellow },
                { GFX_COL.GFX_COL_ORANGE_1, Brushes.Orange },
                { GFX_COL.GFX_COL_ORANGE_2, Brushes.OrangeRed },
                { GFX_COL.GFX_COL_ORANGE_3, Brushes.DarkOrange },
                { GFX_COL.GFX_COL_GREEN_1, Brushes.LightGreen },
                { GFX_COL.GFX_COL_GREEN_2, Brushes.Green },
                { GFX_COL.GFX_COL_GREEN_3, Brushes.DarkGreen },
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
        }

        private struct poly_data
		{
			internal int z;
            internal GFX_COL face_colour;
			internal Point[] point_list;
            internal int next;
		};

        private static readonly poly_data[] poly_chain = new poly_data[MAX_POLYS];
        private bool disposedValue;

		public bool GraphicsStartup()
		{
			Debug.WriteLine(nameof(GraphicsStartup));

			bool imagesLoaded = LoadImages();

            if (!imagesLoaded)
			{
				//allegro_message("Error reading scanner bitmap file.\n");
				return false;
			}

            _screenBufferGraphics.Clear(Color.Black);

			DrawScanner();

			// Draw border
            DrawLine(0, 0, 0, 384);
			DrawLine(0, 0, 511, 0);
			DrawLine(511, 0, 511, 384);

            // Install a timer to regulate the speed of the game...
            lock (frameCountLock)
            {
                frame_count = 0;
            }

            _frameTimer = new()
            {
                // Approx matxh the speed of the TNK
                Interval = 5000 / elite.speed_cap
            };
            _frameTimer.Tick += _frameTimer_Tick;
            _frameTimer.Start();

            return true;
		}

        private void _frameTimer_Tick(object? sender, EventArgs e)
        {
            lock (frameCountLock)
            {
                frame_count++;
            }
        }

        private bool LoadImages()
		{
			string subFolder = "gfx";

			// TODO: load image from resource
			try
			{
				_imageScanner = (Bitmap)Image.FromFile(Path.Combine(subFolder, "scanner.bmp"));
				_images[IMG.IMG_GREEN_DOT] = (Bitmap)Image.FromFile(Path.Combine(subFolder, "greendot.bmp"));
                _images[IMG.IMG_RED_DOT] = (Bitmap)Image.FromFile(Path.Combine(subFolder, "reddot.bmp"));
                _images[IMG.IMG_BIG_S] = (Bitmap)Image.FromFile(Path.Combine(subFolder, "safe.bmp"));
                _images[IMG.IMG_ELITE_TXT] = (Bitmap)Image.FromFile(Path.Combine(subFolder, "elitetx3.bmp"));
                _images[IMG.IMG_BIG_E] = (Bitmap)Image.FromFile(Path.Combine(subFolder, "ecm.bmp"));
                _images[IMG.IMG_MISSILE_GREEN] = (Bitmap)Image.FromFile(Path.Combine(subFolder, "missgrn.bmp"));
                _images[IMG.IMG_MISSILE_YELLOW] = (Bitmap)Image.FromFile(Path.Combine(subFolder, "missyell.bmp"));
                _images[IMG.IMG_MISSILE_RED] = (Bitmap)Image.FromFile(Path.Combine(subFolder, "missred.bmp"));
                _images[IMG.IMG_BLAKE] = (Bitmap)Image.FromFile(Path.Combine(subFolder, "blake.bmp"));
				return true;
			}
			catch
			{
				return false;
			}
		}

		public void GraphicsShutdown()
		{
            Debug.WriteLine(nameof(GraphicsShutdown));

            //destroy_bitmap(scanner_image);
            //destroy_bitmap(gfx_screen);
            //unload_datafile(datafile);
        }

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

		public void PlotPixelFast(Vector2 position, GFX_COL col)
		{
            PlotPixel(position, col);
        }

        public void PlotPixel(Vector2 position, GFX_COL col)
		{
			_screenBuffer.SetPixel((int)(position.X + gfx.GFX_X_OFFSET), (int)(position.Y + gfx.GFX_Y_OFFSET), _pens[col].Color);
        }

        public void DrawCircleFilled(Vector2 centre, float radius, GFX_COL colour)
		{
            _screenBufferGraphics.FillEllipse(_brushes[colour], centre.X + gfx.GFX_X_OFFSET - radius, centre.Y + gfx.GFX_Y_OFFSET - radius, 2 * radius, 2 * radius);
        }

        public virtual void DrawCircle(Vector2 centre, float radius, GFX_COL colour)
		{
            _screenBufferGraphics.DrawEllipse(_pens[colour], centre.X + gfx.GFX_X_OFFSET - radius, centre.Y + gfx.GFX_Y_OFFSET - radius, 2 * radius, 2 * radius);
        }

        public virtual void DrawLine(float x1, float y1, float x2, float y2)
		{
			DrawLine(x1, y1, x2, y2, GFX_COL.GFX_COL_WHITE);
        }

		public void DrawLine(float x1, float y1, float x2, float y2, GFX_COL line_colour)
		{
			_screenBufferGraphics.DrawLine(_pens[line_colour], x1 + gfx.GFX_X_OFFSET, y1 + gfx.GFX_Y_OFFSET, x2 + gfx.GFX_X_OFFSET, y2 + gfx.GFX_Y_OFFSET);
		}

        public void DrawLineXor(float x1, float y1, float x2, float y2, GFX_COL line_colour)
		{
            // .NET has deprecated GDI XOR drawing
            //xor_mode(true);
            DrawLine(x1, y1, x2, y2, line_colour);
            //xor_mode(false);
        }

        public void DrawTriangle(int x1, int y1, int x2, int y2, int x3, int y3, GFX_COL colour)
		{
            Debug.WriteLine(nameof(DrawTriangle));

            Point[] points = new Point[3]
            {
                new Point(x1 + gfx.GFX_X_OFFSET, y1 + gfx.GFX_Y_OFFSET),
                new Point(x2 + gfx.GFX_X_OFFSET, y2 + gfx.GFX_Y_OFFSET),
                new Point(x3 + gfx.GFX_X_OFFSET, y3 + gfx.GFX_Y_OFFSET),
            };

            gfx_polygon(points, colour);
		}

		public void DrawText(int x, int y, string text)
		{
			DrawText(x, y, text, GFX_COL.GFX_COL_WHITE);
        }

		public void DrawText(int x, int y, string text, GFX_COL colour)
		{
            PointF point = new((x / (2 / gfx.GFX_SCALE)) + gfx.GFX_X_OFFSET, (y / (2 / gfx.GFX_SCALE)) + gfx.GFX_Y_OFFSET);
            _screenBufferGraphics.DrawString(text, _fontSmall, _brushes[colour], point);
        }

		public void DrawTextCentre(int y, string text, int psize, GFX_COL colour)
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

		public void ClearDisplay()
		{
            _screenBufferGraphics.FillRectangle(Brushes.Black, gfx.GFX_X_OFFSET + 1, gfx.GFX_Y_OFFSET + 1, 510 + gfx.GFX_X_OFFSET, 383 + gfx.GFX_Y_OFFSET);
		}

		public void ClearTextArea()
		{
            _screenBufferGraphics.FillRectangle(Brushes.Black, gfx.GFX_X_OFFSET + 1, gfx.GFX_Y_OFFSET + 340, 510 + gfx.GFX_X_OFFSET, 43 + gfx.GFX_Y_OFFSET);
        }

		public void ClearArea(int x, int y, int width, int height)
		{
			_screenBufferGraphics.FillRectangle(Brushes.Black, x + gfx.GFX_X_OFFSET, y + gfx.GFX_Y_OFFSET, width + gfx.GFX_X_OFFSET, height + gfx.GFX_Y_OFFSET);
        }

		public void DrawRectangle(int x, int y, int width, int height, GFX_COL colour)
		{
			_screenBufferGraphics.FillRectangle(_brushes[colour], x + gfx.GFX_X_OFFSET, y + gfx.GFX_Y_OFFSET, width + gfx.GFX_X_OFFSET, height + gfx.GFX_Y_OFFSET);
        }

		public void DrawTextPretty(int x, int y, int width, int height, string text)
		{
			int i = 0;
			int maxlen = (width - x) / 8;
            int previous = i;

            while (i < text.Length)
			{
                i += maxlen;
                i = Math.Clamp(i, 0, text.Length - 1);

                while (text[i] is not ' ' and not ',' and not '.')
				{
					i--;
				}

                i++;
                DrawText(x + gfx.GFX_X_OFFSET, y + gfx.GFX_Y_OFFSET, text[previous..i], GFX_COL.GFX_COL_WHITE);
                previous = i;
                y += 8 * gfx.GFX_SCALE;
			}
		}

		public void DrawScanner()
		{
            _screenBufferGraphics.DrawImage(_imageScanner, gfx.GFX_X_OFFSET, 385 + gfx.GFX_Y_OFFSET);
        }

		public void SetClipRegion(int x, int y, int width, int height)
		{
            _screenBufferGraphics.Clip = new Region(new Rectangle(x + gfx.GFX_X_OFFSET, y + gfx.GFX_Y_OFFSET, width + gfx.GFX_X_OFFSET, height + gfx.GFX_Y_OFFSET));
        }

		public void RenderStart()
		{
			start_poly = 0;
			total_polys = 0;
		}

		public void DrawPolygon(Vector2[] point_list, GFX_COL face_colour, int zavg)
		{
			int i;
			int nx;

			if (total_polys == MAX_POLYS)
			{
				return;
			}

			int x = total_polys;
			total_polys++;

			poly_chain[x].face_colour = face_colour;
			poly_chain[x].z = zavg;
			poly_chain[x].next = -1;
            poly_chain[x].point_list = new Point[point_list.Length];

            for (i = 0; i < point_list.Length; i++)
			{
				poly_chain[x].point_list[i].X = (int)point_list[i].X;
                poly_chain[x].point_list[i].Y = (int)point_list[i].Y;
            }

			if (x == 0)
			{
				return;
			}

			if (zavg > poly_chain[start_poly].z)
			{
				poly_chain[x].next = start_poly;
				start_poly = x;
				return;
			}

			for (i = start_poly; poly_chain[i].next != -1; i = poly_chain[i].next)
			{
				nx = poly_chain[i].next;

				if (zavg > poly_chain[nx].z)
				{
					poly_chain[i].next = x;
					poly_chain[x].next = nx;
					return;
				}
			}

			poly_chain[i].next = x;
		}

		public void DrawLine(float x1, float y1, float x2, float y2, int dist, GFX_COL col)
		{
			Vector2[] point_list = new Vector2[2];

			point_list[0].X = x1;
			point_list[0].Y = y1;
			point_list[1].X = x2;
			point_list[1].Y = y2;

			DrawPolygon(point_list, col, dist);
		}

		public void RenderFinish()
		{
			if (total_polys == 0)
            {
                return;
            }

            for (int i = start_poly; i != -1; i = poly_chain[i].next)
			{
                Point[] points = poly_chain[i].point_list;
				GFX_COL col = poly_chain[i].face_colour;

				if (points.Length == 2)
				{
					DrawLine(points[0].X, points[0].Y, points[1].X, points[1].Y, col);
					continue;
				}

				gfx_polygon(points, col);
			};
		}

		private void gfx_polygon(Point[] points, GFX_COL face_colour)
		{
            for (int i = 0; i < points.Length; i++)
			{
				points[i].X += gfx.GFX_X_OFFSET;
				points[i].Y += gfx.GFX_Y_OFFSET;
            }

			_screenBufferGraphics.FillPolygon(_brushes[face_colour], points);
		}

		public void DrawSprite(IMG spriteImgage, int x, int y)
        {
            Bitmap sprite = _images[spriteImgage];

            if (x == -1)
            {
                x = ((256 * gfx.GFX_SCALE) - sprite.Width) / 2;
            }

            _screenBufferGraphics.DrawImage(sprite, x + gfx.GFX_X_OFFSET, y + gfx.GFX_Y_OFFSET);
        }

        public bool RequestFile(string title, string path, string ext)
		{
            Debug.WriteLine(nameof(RequestFile));

			bool okay = false;

			//show_mouse(screen);
			//okay = file_select(title, path, ext);
			//show_mouse(null);

			return okay;
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
					_imageScanner.Dispose();
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