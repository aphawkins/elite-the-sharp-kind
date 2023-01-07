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
        private const int MAX_POLYS = 100;
        private int start_poly;
        private int total_polys;

		public alg_gfx(ref Bitmap screen)
		{
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

        private void frame_timer()
		{
			frame_count++;
		}
		//END_OF_FUNCTION(frame_timer);

		public bool GraphicsStartup()
		{
			Debug.WriteLine(nameof(GraphicsStartup));

			bool imagesLoaded = LoadImages();

            if (!imagesLoaded)
			{
				//set_gfx_mode(GFX_TEXT, 0, 0, 0, 0);
				//allegro_message("Error reading scanner bitmap file.\n");
				return false;
			}

            _screenBufferGraphics.Clear(Color.Black);

			DrawScanner();

			// Draw border
            DrawLine(0, 0, 0, 384);
			DrawLine(0, 0, 511, 0);
			DrawLine(511, 0, 511, 384);

			//			/* Install a timer to regulate the speed of the game... */

			//			LOCK_VARIABLE(frame_count);
			//			LOCK_FUNCTION(frame_timer);
			//			frame_count = 0;
			//			install_int(frame_timer, speed_cap);

			return true;
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

		/*
		 * Blit the back buffer to the screen.
		 */
		public void ScreenUpdate()
		{
            // TODO: Put in a better framerate handler
            Thread.Sleep(100);

            //while (frame_count < 1)
            //{
            //	Thread.Sleep(10);
            //	// rest(10);
            //}

            //frame_count = 0;

            lock (_screen)
			{
				_screenGraphics.DrawImage(_screenBuffer, gfx.GFX_X_OFFSET, gfx.GFX_Y_OFFSET);
			}

            Application.DoEvents();
        }

        public void ScreenAcquire()
		{
            Debug.WriteLine(nameof(ScreenAcquire));

            //acquire_bitmap(gfx_screen);
        }

        public void ScreenRelease()
		{
            Debug.WriteLine(nameof(ScreenRelease));

            //release_bitmap(gfx_screen);
		}

		public void PlotPixelFast(int x, int y, GFX_COL col)
		{
            Debug.WriteLine(nameof(PlotPixelFast));

            ////	_putpixel(gfx_screen, x, y, col);
            //gfx_screen.line[y][x] = col;
		}

        public void PlotPixel(int x, int y, GFX_COL col)
		{
			_screenBuffer.SetPixel(x + gfx.GFX_X_OFFSET, y + gfx.GFX_Y_OFFSET, MapColorToPen(col).Color);
        }

        public void DrawCircleFilled(int cx, int cy, int radius, GFX_COL colour)
		{
            Debug.WriteLine(nameof(DrawCircleFilled));

            _screenBufferGraphics.FillEllipse(MapColorToBrush(colour), cx + gfx.GFX_X_OFFSET - radius, cy + gfx.GFX_Y_OFFSET - radius, 2 * radius, 2 * radius);
        }


        public virtual void DrawCircle(int cx, int cy, int radius, GFX_COL colour)
		{
            _screenBufferGraphics.DrawEllipse(MapColorToPen(colour), cx + gfx.GFX_X_OFFSET - radius, cy + gfx.GFX_Y_OFFSET - radius, 2 * radius, 2 * radius);
        }

        public virtual void DrawLine(float x1, float y1, float x2, float y2)
		{
			DrawLine(x1, y1, x2, y2, GFX_COL.GFX_COL_WHITE);
        }

		public void DrawLine(float x1, float y1, float x2, float y2, GFX_COL line_colour)
		{
			_screenBufferGraphics.DrawLine(MapColorToPen(line_colour), x1 + gfx.GFX_X_OFFSET, y1 + gfx.GFX_Y_OFFSET, x2 + gfx.GFX_X_OFFSET, y2 + gfx.GFX_Y_OFFSET);
		}

        public void DrawLineXor(float x1, float y1, float x2, float y2, GFX_COL line_colour)
		{
            Debug.WriteLine(nameof(DrawLineXor));

            //xor_mode(true);
            //gfx_draw_colour_line(x1, y1, x2, y2, line_colour);
            //xor_mode(false);
        }

        public void DrawTriangle(int x1, int y1, int x2, int y2, int x3, int y3, GFX_COL col)
		{
            Debug.WriteLine(nameof(DrawTriangle));

         //   triangle(gfx_screen, x1 + gfx.GFX_X_OFFSET, y1 + gfx.GFX_Y_OFFSET, x2 + gfx.GFX_X_OFFSET, y2 + gfx.GFX_Y_OFFSET,
						   //x3 + gfx.GFX_X_OFFSET, y3 + gfx.GFX_Y_OFFSET, col);
		}

		public void DrawText(int x, int y, string text)
		{
			DrawText(x, y, text, GFX_COL.GFX_COL_WHITE);
        }

		public void DrawText(int x, int y, string text, GFX_COL colour)
		{
            PointF point = new((x / (2 / gfx.GFX_SCALE)) + gfx.GFX_X_OFFSET, (y / (2 / gfx.GFX_SCALE)) + gfx.GFX_Y_OFFSET);
            _screenBufferGraphics.DrawString(text, _fontSmall, MapColorToBrush(colour), point);
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
				MapColorToBrush(colour), 
				point, 
				stringFormat);
        }

		public void ClearDisplay()
		{
            _screenBufferGraphics.FillRectangle(Brushes.Black, gfx.GFX_X_OFFSET + 1, gfx.GFX_Y_OFFSET + 1, 510 + gfx.GFX_X_OFFSET, 383 + gfx.GFX_Y_OFFSET);
		}

		public void ClearTextArea()
		{
            _screenBufferGraphics.FillRectangle(Brushes.Black, gfx.GFX_X_OFFSET + 1, gfx.GFX_Y_OFFSET + 340, 510 + gfx.GFX_X_OFFSET, 383 + gfx.GFX_Y_OFFSET);
        }

		public void ClearArea(int tx, int ty, int bx, int by)
		{
			_screenBufferGraphics.FillRectangle(Brushes.Black, tx + gfx.GFX_X_OFFSET, ty + gfx.GFX_Y_OFFSET, bx + gfx.GFX_X_OFFSET, by + gfx.GFX_Y_OFFSET);
        }

		public void DrawRectangle(int tx, int ty, int bx, int by, GFX_COL col)
		{
			_screenBufferGraphics.FillRectangle(MapColorToBrush(col), tx + gfx.GFX_X_OFFSET, ty + gfx.GFX_Y_OFFSET, bx + gfx.GFX_X_OFFSET, by + gfx.GFX_Y_OFFSET);
        }

		public void DrawTextPretty(int tx, int ty, int bx, int by, string txt)
		{
			int i = 0;
			int maxlen = (bx - tx) / 8;
            int previous = i;

            while (i < txt.Length)
			{
                i += maxlen;
                i = Math.Clamp(i, 0, txt.Length - 1);

                while (txt[i] is not ' ' and not ',' and not '.')
				{
					i--;
				}

                i++;
                DrawText(tx + gfx.GFX_X_OFFSET, ty + gfx.GFX_Y_OFFSET, txt[previous..i], GFX_COL.GFX_COL_WHITE);
                previous = i;
                ty += 8 * gfx.GFX_SCALE;
			}
		}

		public void DrawScanner()
		{
            _screenBufferGraphics.DrawImage(_imageScanner, gfx.GFX_X_OFFSET, 385 + gfx.GFX_Y_OFFSET);
        }

		public void SetClipRegion(int tx, int ty, int bx, int by)
		{
            Debug.WriteLine(nameof(SetClipRegion));

            //set_clip(gfx_screen, tx + gfx.GFX_X_OFFSET, ty + gfx.GFX_Y_OFFSET, bx + gfx.GFX_X_OFFSET, by + gfx.GFX_Y_OFFSET);
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

		public void DrawLine(int x1, int y1, int x2, int y2, int dist, GFX_COL col)
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

			_screenBufferGraphics.FillPolygon(MapColorToBrush(face_colour), points);
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

		private static Pen MapColorToPen(GFX_COL col)
		{
			switch (col)
			{
                case GFX_COL.GFX_COL_WHITE:
                    return Pens.White;

                case GFX_COL.GFX_COL_WHITE_2:
                    return Pens.WhiteSmoke;

                case GFX_COL.GFX_COL_BLACK:
                    return Pens.Black;

                case GFX_COL.GFX_COL_GOLD:
                    return Pens.Gold;

                case GFX_COL.GFX_COL_YELLOW_1:
                    return Pens.Yellow;

                case GFX_COL.GFX_COL_RED:
                    return Pens.Red;

                case GFX_COL.GFX_COL_RED_3:
                    return Pens.PaleVioletRed;

                case GFX_COL.GFX_COL_RED_4:
                    return Pens.MediumVioletRed;

                case GFX_COL.GFX_COL_DARK_RED:
                    return Pens.DarkRed;

                case GFX_COL.GFX_COL_BLUE_1:
                    return Pens.LightBlue;

                case GFX_COL.GFX_COL_BLUE_2:
                    return Pens.MediumBlue;

                case GFX_COL.GFX_COL_BLUE_3:
                    return Pens.Blue;

                case GFX_COL.GFX_COL_BLUE_4:
                    return Pens.DarkBlue;

                case GFX_COL.GFX_COL_GREY_1:
                    return Pens.DimGray;

                case GFX_COL.GFX_COL_GREY_2:
                    return Pens.LightGray;

                case GFX_COL.GFX_COL_GREY_3:
                    return Pens.Gray;

                case GFX_COL.GFX_COL_GREY_4:
                    return Pens.DarkGray;

                case GFX_COL.GFX_COL_GREEN_1:
                    return Pens.LightGreen;

                case GFX_COL.GFX_COL_GREEN_2:
                    return Pens.Green;

                case GFX_COL.GFX_COL_GREEN_3:
                    return Pens.DarkGreen;

                case GFX_COL.GFX_COL_PINK_1:
                    return Pens.Pink;

                default:
					Debug.Assert(false);
					return Pens.Magenta;
			}
		}

        private static Brush MapColorToBrush(GFX_COL col)
        {
            switch (col)
            {
                case GFX_COL.GFX_COL_WHITE:
                    return Brushes.White;

                case GFX_COL.GFX_COL_WHITE_2:
                    return Brushes.WhiteSmoke;

                case GFX_COL.GFX_COL_BLACK:
                    return Brushes.Black;

                case GFX_COL.GFX_COL_GOLD:
                    return Brushes.Gold;

                case GFX_COL.GFX_COL_YELLOW_1:
                    return Brushes.Yellow;

                case GFX_COL.GFX_COL_RED:
                    return Brushes.Red;

                case GFX_COL.GFX_COL_RED_3:
                    return Brushes.PaleVioletRed;

                case GFX_COL.GFX_COL_RED_4:
                    return Brushes.MediumVioletRed;

                case GFX_COL.GFX_COL_DARK_RED:
                    return Brushes.DarkRed;

                case GFX_COL.GFX_COL_BLUE_1:
                    return Brushes.LightBlue;

                case GFX_COL.GFX_COL_BLUE_2:
                    return Brushes.MediumBlue;

                case GFX_COL.GFX_COL_BLUE_3:
                    return Brushes.Blue;

                case GFX_COL.GFX_COL_BLUE_4:
                    return Brushes.DarkBlue;

                case GFX_COL.GFX_COL_GREY_1:
                    return Brushes.DimGray;

                case GFX_COL.GFX_COL_GREY_2:
                    return Brushes.LightGray;

                case GFX_COL.GFX_COL_GREY_3:
                    return Brushes.Gray;

                case GFX_COL.GFX_COL_GREY_4:
                    return Brushes.DarkGray;

                case GFX_COL.GFX_COL_GREEN_1:
                    return Brushes.LightGreen;

                case GFX_COL.GFX_COL_GREEN_2:
                    return Brushes.Green;

                case GFX_COL.GFX_COL_GREEN_3:
                    return Brushes.DarkGreen;

                case GFX_COL.GFX_COL_PINK_1:
                    return Brushes.Pink;

                default:
                    Debug.Assert(false);
                    return Brushes.Magenta;
            }
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