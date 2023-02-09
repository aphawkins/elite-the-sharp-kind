/*
 * Elite - The New Kind.
 *
 * Reverse engineered from the BBC disk version of Elite.
 * Additional material by C.J.Pinder.
 *
 * The original Elite code is (C) I.Bell & D.Braben 1984.
 * This version re-engineered in C by C.J.Pinder 1999-2001.
 *
 * email: <christian@newkind.co.uk>
 *
 *
 */

namespace Elite.Engine
{
    using System.Numerics;
	using Elite.Engine.Enums;

	internal class Stars
	{
		internal static bool warp_stars;
        private static readonly Vector3[] stars = new Vector3[20];
		private readonly IGfx _gfx;

		internal Stars(IGfx gfx)
		{
			_gfx = gfx;
        }

		internal static void create_new_stars()
		{
			for (int i = 0; i < stars.Length; i++)
			{
				stars[i] = new()
				{
					X = RNG.Random(-128, 127) | 8,
					Y = RNG.Random(-128, 127) | 4,
					Z = RNG.Random(255) | 0x90,
				};
			}

            warp_stars = false;
		}

        internal void front_starfield()
		{
			float delta = warp_stars ? 50 : elite.flight_speed;
			float alpha = elite.flight_roll;
			float beta = elite.flight_climb;

			alpha /= 256;
			delta /= 2;

			for (int i = 0; i < stars.Length; i++)
			{
                /* Plot the stars in their current locations... */
                Vector2 star = new()
                {
                    Y = stars[i].Y,
                    X = stars[i].X
                };
                float zz = stars[i].Z;

                star.X += 128;
                star.Y += 96;

                star.X *= gfx.GFX_SCALE;
                star.Y *= gfx.GFX_SCALE;

				if ((!warp_stars) &&
					(star.X >= gfx.GFX_VIEW_TX) && (star.X <= gfx.GFX_VIEW_BX) &&
					(star.Y >= gfx.GFX_VIEW_TY) && (star.Y <= gfx.GFX_VIEW_BY))
				{
                    _gfx.DrawPixel(star, GFX_COL.GFX_COL_WHITE);

					if (zz < 0xC0)
					{
                        _gfx.DrawPixel(new(star.X + 1, star.Y), GFX_COL.GFX_COL_WHITE);
					}

					if (zz < 0x90)
					{
                        _gfx.DrawPixel(new(star.X, star.Y + 1), GFX_COL.GFX_COL_WHITE);
                        _gfx.DrawPixel(new(star.X + 1, star.Y + 1), GFX_COL.GFX_COL_WHITE);
					}
				}


				/* Move the stars to their new locations...*/
				float Q = delta / stars[i].Z;

				stars[i].Z -= delta;
				float yy = stars[i].Y + (stars[i].Y * Q);
				float xx = stars[i].X + (stars[i].X * Q);
				zz = stars[i].Z;

				yy += xx * alpha;
				xx -= yy * alpha;

				/*
						tx = yy * beta;
						xx = xx + (tx * tx * 2);
				*/
				yy += beta;

				stars[i].Y = yy;
				stars[i].X = xx;


				if (warp_stars)
				{
                    _gfx.DrawLine(star, new((xx + 128) * gfx.GFX_SCALE, (yy + 96) * gfx.GFX_SCALE));
				}

                star.X = xx;
                star.Y = yy;

				if ((star.X > 120) || (star.X < -120) ||
					(star.Y > 120) || (star.Y < -120) || (zz < 16))
				{
					stars[i] = new()
					{
						X = RNG.Random(-128, 127) | 8,
						Y = RNG.Random(-128, 127) | 4,
						Z = RNG.Random(255) | 144,
					};
					continue;
				}

			}

            warp_stars = false;
		}

        private void rear_starfield()
		{
			float delta = warp_stars ? 50 : elite.flight_speed;
			float alpha = -elite.flight_roll;
			float beta = -elite.flight_climb;

			alpha /= 256;
			delta /= 2;

			for (int i = 0; i < stars.Length; i++)
			{
                /* Plot the stars in their current locations... */
                Vector2 star = new()
                {
                    Y = stars[i].Y,
                    X = stars[i].X
                };
                float zz = stars[i].Z;

                star.X += 128;
                star.Y += 96;

                star.X *= gfx.GFX_SCALE;
                star.Y *= gfx.GFX_SCALE;

				if ((!warp_stars) &&
					(star.X >= gfx.GFX_VIEW_TX) && (star.X <= gfx.GFX_VIEW_BX) &&
					(star.Y >= gfx.GFX_VIEW_TY) && (star.Y <= gfx.GFX_VIEW_BY))
				{
                    _gfx.DrawPixel(star, GFX_COL.GFX_COL_WHITE);

					if (zz < 0xC0)
					{
                        _gfx.DrawPixel(new(star.X + 1, star.Y), GFX_COL.GFX_COL_WHITE);
					}

					if (zz < 0x90)
					{
                        _gfx.DrawPixel(new(star.X, star.Y + 1), GFX_COL.GFX_COL_WHITE);
                        _gfx.DrawPixel(new(star.X + 1, star.Y + 1), GFX_COL.GFX_COL_WHITE);
					}
				}

				/* Move the stars to their new locations...*/

				float Q = delta / stars[i].Z;

				stars[i].Z += delta;
				float yy = stars[i].Y - (stars[i].Y * Q);
				float xx = stars[i].X - (stars[i].X * Q);
				zz = stars[i].Z;

				yy += xx * alpha;
				xx -= yy * alpha;

				/*
						tx = yy * beta;
						xx = xx + (tx * tx * 2);
				*/
				yy += beta;

				if (warp_stars)
				{
					float ey = yy;
					float ex = xx;
					ex = (ex + 128) * gfx.GFX_SCALE;
					ey = (ey + 96) * gfx.GFX_SCALE;

					if ((star.X >= gfx.GFX_VIEW_TX) && (star.X <= gfx.GFX_VIEW_BX) &&
					   (star.Y >= gfx.GFX_VIEW_TY) && (star.Y <= gfx.GFX_VIEW_BY) &&
					   (ex >= gfx.GFX_VIEW_TX) && (ex <= gfx.GFX_VIEW_BX) &&
					   (ey >= gfx.GFX_VIEW_TY) && (ey <= gfx.GFX_VIEW_BY))
					{
                        _gfx.DrawLine(star, new((xx + 128) * gfx.GFX_SCALE, (yy + 96) * gfx.GFX_SCALE));
					}
				}

				stars[i].Y = yy;
				stars[i].X = xx;

				if ((zz >= 300) || (MathF.Abs(yy) >= 110f))
				{
					stars[i].Z = RNG.Random(51, 178);

					if (RNG.TrueOrFalse())
					{
						stars[i].X = RNG.Random(-128, 127);
						stars[i].Y = RNG.TrueOrFalse() ? -115 : 115;
					}
					else
					{
						stars[i].X = RNG.TrueOrFalse() ? -126 : 126;
						stars[i].Y = RNG.Random(-128, 127);
					}
				}
			}

            warp_stars = false;
		}

        private void side_starfield()
		{
			float delta = warp_stars ? 50 : elite.flight_speed;
			float alpha = elite.flight_roll;
			float beta = elite.flight_climb;

			if (elite._state.currentScreen == SCR.SCR_LEFT_VIEW)
			{
				delta = -delta;
				alpha = -alpha;
				beta = -beta;
			}

			for (int i = 0; i < stars.Length; i++)
			{
                Vector2 star = new()
                {
                    Y = stars[i].Y,
                    X = stars[i].X
                };
                float zz = stars[i].Z;

				star.X += 128;
                star.Y += 96;

                star.X *= gfx.GFX_SCALE;
                star.Y *= gfx.GFX_SCALE;

				if ((!warp_stars) &&
					(star.X >= gfx.GFX_VIEW_TX) && (star.X <= gfx.GFX_VIEW_BX) &&
					(star.Y >= gfx.GFX_VIEW_TY) && (star.Y <= gfx.GFX_VIEW_BY))
				{
                    _gfx.DrawPixel(star, GFX_COL.GFX_COL_WHITE);

					if (zz < 0xC0)
					{
                        _gfx.DrawPixel(new(star.X + 1, star.Y), GFX_COL.GFX_COL_WHITE);
					}

					if (zz < 0x90)
					{
                        _gfx.DrawPixel(new(star.X, star.Y + 1), GFX_COL.GFX_COL_WHITE);
                        _gfx.DrawPixel(new(star.X + 1, star.Y + 1), GFX_COL.GFX_COL_WHITE);
					}
				}

				float yy = stars[i].Y;
				float xx = stars[i].X;
				zz = stars[i].Z;

				float delt8 = delta / (zz / 32);
				xx += delt8;

				xx += yy * (beta / 256);
				yy -= xx * (beta / 256);

				xx += yy / 256 * (alpha / 256) * (-xx);
				yy += yy / 256 * (alpha / 256) * yy;

				yy += alpha;

				stars[i].Y = yy;
				stars[i].X = xx;

				if (warp_stars)
				{
                    _gfx.DrawLine(star, new((xx + 128) * gfx.GFX_SCALE, (yy + 96) * gfx.GFX_SCALE));
				}

				if (MathF.Abs(stars[i].X) >= 116f)
				{
					stars[i].Y = RNG.Random(-128, 127);
					stars[i].X = (elite._state.currentScreen == SCR.SCR_LEFT_VIEW) ? 115 : -115;
					stars[i].Z = RNG.Random(255) | 8;
				}
				else if (MathF.Abs(stars[i].Y) >= 116f)
				{
					stars[i].X = RNG.Random(-128, 127);
					stars[i].Y = (alpha > 0) ? -110 : 110;
					stars[i].Z = RNG.Random(255) | 8;
				}

			}

            warp_stars = false;
		}

		/// <summary>
		/// When we change view, flip the stars over so they look like other stars.
		/// </summary>
		internal static void flip_stars()
		{
			for (int i = 0; i < stars.Length; i++)
			{
                float y = stars[i].Y;
                float x = stars[i].X;
                stars[i].X = y;
                stars[i].Y = x;
			}
		}

		internal void update_starfield()
		{
			switch (elite._state.currentScreen)
			{
				case SCR.SCR_FRONT_VIEW:
				case SCR.SCR_ESCAPE_POD:
					front_starfield();
					break;

				case SCR.SCR_REAR_VIEW:
				case SCR.SCR_GAME_OVER:
					rear_starfield();
					break;

				case SCR.SCR_LEFT_VIEW:
				case SCR.SCR_RIGHT_VIEW:
					side_starfield();
					break;
			}
		}
	}
}