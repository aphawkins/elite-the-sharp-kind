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

using System.Drawing;
using System.Numerics;
using Elite.Engine.Enums;

namespace Elite.WinForms
{
    public class GdiGraphicsAntiAlias : GdiGraphics
    {
        //private const int AA_BITS = 3;
        //private const int AA_AND = 7;
        //private const int AA_BASE = 235;

        //#define trunc(x) ((x) & ~65535)
        //#define frac(x) ((x) & 65535)
        //#define invfrac(x) (65535-frac(x))
        //#define plot(x,y,c) putpixel(gfx_screen, (x), (y), (c)+AA_BASE)

        public GdiGraphicsAntiAlias(ref Bitmap screen) : base(ref screen)
        {
        }

        public override void DrawCircle(Vector2 centre, float radius, GFX_COL colour = GFX_COL.GFX_COL_WHITE)
        {
            //	int x, y;
            //	int s;
            //	int sx, sy;

            //	cx += gfx.GFX_X_OFFSET;
            //	cy += gfx.GFX_Y_OFFSET;

            //	radius >>= (16 - AA_BITS);

            //	x = radius;
            //	s = -radius;
            //	y = 0;

            //	while (y <= x)
            //	{
            //		//wide pixels
            //		sx = cx + (x >> AA_BITS); sy = cy + (y >> AA_BITS);

            //		plot(sx, sy, AA_AND - (x & AA_AND));
            //		plot(sx + 1, sy, x & AA_AND);

            //		sy = cy - (y >> AA_BITS);

            //		plot(sx, sy, AA_AND - (x & AA_AND));
            //		plot(sx + 1, sy, x & AA_AND);

            //		sx = cx - (x >> AA_BITS);

            //		plot(sx, sy, AA_AND - (x & AA_AND));
            //		plot(sx - 1, sy, x & AA_AND);

            //		sy = cy + (y >> AA_BITS);

            //		plot(sx, sy, AA_AND - (x & AA_AND));
            //		plot(sx - 1, sy, x & AA_AND);

            //		//tall pixels
            //		sx = cx + (y >> AA_BITS); sy = cy + (x >> AA_BITS);

            //		plot(sx, sy, AA_AND - (x & AA_AND));
            //		plot(sx, sy + 1, x & AA_AND);

            //		sy = cy - (x >> AA_BITS);

            //		plot(sx, sy, AA_AND - (x & AA_AND));
            //		plot(sx, sy - 1, x & AA_AND);

            //		sx = cx - (y >> AA_BITS);

            //		plot(sx, sy, AA_AND - (x & AA_AND));
            //		plot(sx, sy - 1, x & AA_AND);

            //		sy = cy + (x >> AA_BITS);

            //		plot(sx, sy, AA_AND - (x & AA_AND));
            //		plot(sx, sy + 1, x & AA_AND);

            //		s += AA_AND + 1 + (y << (AA_BITS + 1)) + ((1 << (AA_BITS + 2)) - 2);
            //		y += AA_AND + 1;

            //		while (s >= 0)
            //		{
            //			s -= (x << 1) + 2;
            //			x--;
            //		}
            //	}
        }

        /// <summary>
        /// Draw anti-aliased line.
        /// By T.Harte.
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        public override void DrawLine(Vector2 start, Vector2 end)
        {
            //	fixed grad, xd, yd;
            //	fixed xgap, ygap, xend, yend, xf, yf;
            //	fixed brightness1, brightness2, swap;

            //	int x, y, ix1, ix2, iy1, iy2;

            //	x1 += itofix(GFX_X_OFFSET);
            //	x2 += itofix(GFX_X_OFFSET);
            //	y1 += itofix(GFX_Y_OFFSET);
            //	y2 += itofix(GFX_Y_OFFSET);

            //	xd = x2 - x1;
            //	yd = y2 - y1;

            //	if (Math.Abs(xd) > Math.Abs(yd))
            //	{
            //		if (x1 > x2)
            //		{
            //			swap = x1; x1 = x2; x2 = swap;
            //			swap = y1; y1 = y2; y2 = swap;
            //			xd = -xd;
            //			yd = -yd;
            //		}

            //		grad = fdiv(yd, xd);

            //		//end point 1

            //		xend = trunc(x1 + 32768);
            //		yend = y1 + fmul(grad, xend - x1);

            //		xgap = invfrac(x1 + 32768);

            //		ix1 = xend >> 16;
            //		iy1 = yend >> 16;

            //		brightness1 = fmul(invfrac(yend), xgap);
            //		brightness2 = fmul(frac(yend), xgap);

            //		plot(ix1, iy1, brightness1 >> (16 - AA_BITS));
            //		plot(ix1, iy1 + 1, brightness2 >> (16 - AA_BITS));

            //		yf = yend + grad;

            //		//end point 2;

            //		xend = trunc(x2 + 32768);
            //		yend = y2 + fmul(grad, xend - x2);

            //		xgap = invfrac(x2 - 32768);

            //		ix2 = xend >> 16;
            //		iy2 = yend >> 16;

            //		brightness1 = fmul(invfrac(yend), xgap);
            //		brightness2 = fmul(frac(yend), xgap);

            //		plot(ix2, iy2, brightness1 >> (16 - AA_BITS));
            //		plot(ix2, iy2 + 1, brightness2 >> (16 - AA_BITS));

            //		for (x = ix1 + 1; x <= ix2 - 1; x++)
            //		{
            //			brightness1 = invfrac(yf);
            //			brightness2 = frac(yf);

            //			plot(x, (yf >> 16), brightness1 >> (16 - AA_BITS));
            //			plot(x, 1 + (yf >> 16), brightness2 >> (16 - AA_BITS));

            //			yf += grad;
            //		}
            //	}
            //	else
            //	{
            //		if (y1 > y2)
            //		{
            //			swap = x1; x1 = x2; x2 = swap;
            //			swap = y1; y1 = y2; y2 = swap;
            //			xd = -xd;
            //			yd = -yd;
            //		}

            //		grad = fdiv(xd, yd);

            //		//end point 1

            //		yend = trunc(y1 + 32768);
            //		xend = x1 + fmul(grad, yend - y1);

            //		ygap = invfrac(y1 + 32768);

            //		iy1 = yend >> 16;
            //		ix1 = xend >> 16;

            //		brightness1 = fmul(invfrac(xend), ygap);
            //		brightness2 = fmul(frac(xend), ygap);

            //		plot(ix1, iy1, brightness1 >> (16 - AA_BITS));
            //		plot(ix1 + 1, iy1, brightness2 >> (16 - AA_BITS));

            //		xf = xend + grad;

            //		//end point 2;

            //		yend = trunc(y2 + 32768);
            //		xend = x2 + fmul(grad, yend - y2);

            //		ygap = invfrac(y2 - 32768);

            //		ix2 = xend >> 16;
            //		iy2 = yend >> 16;

            //		brightness1 = fmul(invfrac(xend), ygap);
            //		brightness2 = fmul(frac(xend), ygap);

            //		plot(ix2, iy2, brightness1 >> (16 - AA_BITS));
            //		plot(ix2 + 1, iy2, brightness2 >> (16 - AA_BITS));

            //		for (y = iy1 + 1; y <= iy2 - 1; y++)
            //		{
            //			brightness1 = invfrac(xf);
            //			brightness2 = frac(xf);

            //			plot((xf >> 16), y, brightness1 >> (16 - AA_BITS));
            //			plot(1 + (xf >> 16), y, brightness2 >> (16 - AA_BITS));

            //			xf += grad;
            //		}
            //	}
        }

        //#undef trunc
        //#undef frac
        //#undef invfrac
        //#undef plot

        //#undef AA_BITS
        //#undef AA_AND
        //#undef AA_BASE
    }
}