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

namespace Elite
{
	using Elite.Enums;

	internal static class Stars
	{
		internal static bool warp_stars;

		struct star
		{
			internal double x;
			internal double y;
			internal double z;
		};

		static star[] stars = new star[20];

		internal static void create_new_stars()
		{
			int i;
			int nstars;

			nstars = elite.witchspace ? 3 : 12;

			for (i = 0; i < nstars; i++)
			{
				stars[i].x = (random.rand255() - 128) | 8;
				stars[i].y = (random.rand255() - 128) | 4;
				stars[i].z = random.rand255() | 0x90;
			}

			Stars.warp_stars = false;
		}

		static void front_starfield()
		{
			int i;
			double Q;
			double delta;
			double alpha = 0;
			double beta = 0;
			double xx, yy, zz;
			int sx;
			int sy;
			int nstars;

			nstars = elite.witchspace ? 3 : 12;

			delta = Stars.warp_stars ? 50 : elite.flight_speed;
			alpha = (double)elite.flight_roll;
			beta = (double)elite.flight_climb;

			alpha /= 256.0;
			delta /= 2.0;

			for (i = 0; i < nstars; i++)
			{
				/* Plot the stars in their current locations... */

				sy = (int)stars[i].y;
				sx = (int)stars[i].x;
				zz = stars[i].z;

				sx += 128;
				sy += 96;

				sx *= gfx.GFX_SCALE;
				sy *= gfx.GFX_SCALE;

				if ((!Stars.warp_stars) &&
					(sx >= gfx.GFX_VIEW_TX) && (sx <= gfx.GFX_VIEW_BX) &&
					(sy >= gfx.GFX_VIEW_TY) && (sy <= gfx.GFX_VIEW_BY))
				{
					alg_gfx.gfx_plot_pixel(sx, sy, gfx.GFX_COL_WHITE);

					if (zz < 0xC0)
					{
						alg_gfx.gfx_plot_pixel(sx + 1, sy, gfx.GFX_COL_WHITE);
					}

					if (zz < 0x90)
					{
						alg_gfx.gfx_plot_pixel(sx, sy + 1, gfx.GFX_COL_WHITE);
						alg_gfx.gfx_plot_pixel(sx + 1, sy + 1, gfx.GFX_COL_WHITE);
					}
				}


				/* Move the stars to their new locations...*/

				Q = delta / stars[i].z;

				stars[i].z -= delta;
				yy = stars[i].y + (stars[i].y * Q);
				xx = stars[i].x + (stars[i].x * Q);
				zz = stars[i].z;

				yy = yy + (xx * alpha);
				xx = xx - (yy * alpha);

				/*
						tx = yy * beta;
						xx = xx + (tx * tx * 2);
				*/
				yy = yy + beta;

				stars[i].y = yy;
				stars[i].x = xx;


				if (Stars.warp_stars)
				{
					alg_gfx.gfx_draw_line(sx, sy, (int)(xx + 128) * gfx.GFX_SCALE, (int)(yy + 96) * gfx.GFX_SCALE);
				}

				sx = (int)xx;
				sy = (int)yy;

				if ((sx > 120) || (sx < -120) ||
					(sy > 120) || (sy < -120) || (zz < 16))
				{
					stars[i].x = (random.rand255() - 128) | 8;
					stars[i].y = (random.rand255() - 128) | 4;
					stars[i].z = random.rand255() | 0x90;
					continue;
				}

			}

			Stars.warp_stars = false;
		}

		static void rear_starfield()
		{
			int i;
			double Q;
			double delta;
			double alpha = 0;
			double beta = 0;
			double xx, yy, zz;
			int sx, sy;
			int ex, ey;
			int nstars;

			nstars = elite.witchspace ? 3 : 12;

			delta = Stars.warp_stars ? 50 : elite.flight_speed;
			alpha = -elite.flight_roll;
			beta = -elite.flight_climb;

			alpha /= 256.0;
			delta /= 2.0;

			for (i = 0; i < nstars; i++)
			{
				/* Plot the stars in their current locations... */

				sy = (int)stars[i].y;
				sx = (int)stars[i].x;
				zz = stars[i].z;

				sx += 128;
				sy += 96;

				sx *= gfx.GFX_SCALE;
				sy *= gfx.GFX_SCALE;

				if ((!Stars.warp_stars) &&
					(sx >= gfx.GFX_VIEW_TX) && (sx <= gfx.GFX_VIEW_BX) &&
					(sy >= gfx.GFX_VIEW_TY) && (sy <= gfx.GFX_VIEW_BY))
				{
					alg_gfx.gfx_plot_pixel(sx, sy, gfx.GFX_COL_WHITE);

					if (zz < 0xC0)
					{
						alg_gfx.gfx_plot_pixel(sx + 1, sy, gfx.GFX_COL_WHITE);
					}

					if (zz < 0x90)
					{
						alg_gfx.gfx_plot_pixel(sx, sy + 1, gfx.GFX_COL_WHITE);
						alg_gfx.gfx_plot_pixel(sx + 1, sy + 1, gfx.GFX_COL_WHITE);
					}
				}


				/* Move the stars to their new locations...*/

				Q = delta / stars[i].z;

				stars[i].z += delta;
				yy = stars[i].y - (stars[i].y * Q);
				xx = stars[i].x - (stars[i].x * Q);
				zz = stars[i].z;

				yy = yy + (xx * alpha);
				xx = xx - (yy * alpha);

				/*
						tx = yy * beta;
						xx = xx + (tx * tx * 2);
				*/
				yy = yy + beta;

				if (Stars.warp_stars)
				{
					ey = (int)yy;
					ex = (int)xx;
					ex = (ex + 128) * gfx.GFX_SCALE;
					ey = (ey + 96) * gfx.GFX_SCALE;

					if ((sx >= gfx.GFX_VIEW_TX) && (sx <= gfx.GFX_VIEW_BX) &&
					   (sy >= gfx.GFX_VIEW_TY) && (sy <= gfx.GFX_VIEW_BY) &&
					   (ex >= gfx.GFX_VIEW_TX) && (ex <= gfx.GFX_VIEW_BX) &&
					   (ey >= gfx.GFX_VIEW_TY) && (ey <= gfx.GFX_VIEW_BY))
					{
						alg_gfx.gfx_draw_line(sx, sy, (int)(xx + 128) * gfx.GFX_SCALE, (int)(yy + 96) * gfx.GFX_SCALE);
					}
				}

				stars[i].y = yy;
				stars[i].x = xx;

				if ((zz >= 300) || (Math.Abs(yy) >= 110))
				{
					stars[i].z = (random.rand255() & 127) + 51;

					if ((random.rand255() & 1) == 1)
					{
						stars[i].x = random.rand255() - 128;
						stars[i].y = ((random.rand255() & 1) == 1) ? -115 : 115;
					}
					else
					{
						stars[i].x = ((random.rand255() & 1) == 1) ? -126 : 126;
						stars[i].y = random.rand255() - 128;
					}
				}

			}

			Stars.warp_stars = false;
		}

		static void side_starfield()
		{
			int i;
			double delta;
			double alpha;
			double beta;
			double xx, yy, zz;
			int sx;
			int sy;
			double delt8;
			int nstars;

			nstars = elite.witchspace ? 3 : 12;

			delta = Stars.warp_stars ? 50 : elite.flight_speed;
			alpha = elite.flight_roll;
			beta = elite.flight_climb;

			if (elite.current_screen == SCR.SCR_LEFT_VIEW)
			{
				delta = -delta;
				alpha = -alpha;
				beta = -beta;
			}

			for (i = 0; i < nstars; i++)
			{
				sy = (int)stars[i].y;
				sx = (int)stars[i].x;
				zz = stars[i].z;

				sx += 128;
				sy += 96;

				sx *= gfx.GFX_SCALE;
				sy *= gfx.GFX_SCALE;

				if ((!Stars.warp_stars) &&
					(sx >= gfx.GFX_VIEW_TX) && (sx <= gfx.GFX_VIEW_BX) &&
					(sy >= gfx.GFX_VIEW_TY) && (sy <= gfx.GFX_VIEW_BY))
				{
					alg_gfx.gfx_plot_pixel(sx, sy, gfx.GFX_COL_WHITE);

					if (zz < 0xC0)
					{
						alg_gfx.gfx_plot_pixel(sx + 1, sy, gfx.GFX_COL_WHITE);
					}

					if (zz < 0x90)
					{
						alg_gfx.gfx_plot_pixel(sx, sy + 1, gfx.GFX_COL_WHITE);
						alg_gfx.gfx_plot_pixel(sx + 1, sy + 1, gfx.GFX_COL_WHITE);
					}
				}

				yy = stars[i].y;
				xx = stars[i].x;
				zz = stars[i].z;

				delt8 = delta / (zz / 32);
				xx = xx + delt8;

				xx += (yy * (beta / 256));
				yy -= (xx * (beta / 256));

				xx += ((yy / 256) * (alpha / 256)) * (-xx);
				yy += ((yy / 256) * (alpha / 256)) * (yy);

				yy += alpha;

				stars[i].y = yy;
				stars[i].x = xx;

				if (Stars.warp_stars)
				{
					alg_gfx.gfx_draw_line(sx, sy, (int)(xx + 128) * gfx.GFX_SCALE, (int)(yy + 96) * gfx.GFX_SCALE);
				}

				if (Math.Abs(stars[i].x) >= 116)
				{
					stars[i].y = random.rand255() - 128;
					stars[i].x = (elite.current_screen == SCR.SCR_LEFT_VIEW) ? 115 : -115;
					stars[i].z = random.rand255() | 8;
				}
				else if (Math.Abs(stars[i].y) >= 116)
				{
					stars[i].x = random.rand255() - 128;
					stars[i].y = (alpha > 0) ? -110 : 110;
					stars[i].z = random.rand255() | 8;
				}

			}

			Stars.warp_stars = false;
		}

		/*
		 * When we change view, flip the stars over so they look like other stars.
		 */
		static void flip_stars()
		{
			int i;
			int nstars;
			int sx;
			int sy;

			nstars = elite.witchspace ? 3 : 12;
			for (i = 0; i < nstars; i++)
			{
				sy = (int)stars[i].y;
				sx = (int)stars[i].x;
				stars[i].x = sy;
				stars[i].y = sx;
			}
		}

		static void update_starfield()
		{
			switch (elite.current_screen)
			{
				case SCR.SCR_FRONT_VIEW:
				case SCR.SCR_INTRO_ONE:
				case SCR.SCR_INTRO_TWO:
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