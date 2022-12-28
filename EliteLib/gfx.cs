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


/**
 *
 * Elite - The New Kind.
 *
 * The code in this file has not been derived from the original Elite code.
 * Written by C.J.Pinder 1999/2000.
 *
 **/

namespace Elite
{
	internal static class gfx
	{
#if RES_512_512
		internal const int GFX_SCALE = 2;
		internal const int GFX_X_OFFSET = 0;
		internal const int GFX_Y_OFFSET = 0;
		internal const int GFX_X_CENTRE = 256;
		internal const int GFX_Y_CENTRE = 192;

		internal const int GFX_VIEW_TX = 1;
		internal const int GFX_VIEW_TY = 1;
		internal const int GFX_VIEW_BX = 509;
		internal const int GFX_VIEW_BY = 381;
#endif

#if RES_800_600
		internal const int GFX_SCALE = 2;
		internal const int GFX_X_OFFSET = 144;
		internal const int GFX_Y_OFFSET = 44;
		internal const int GFX_X_CENTRE = 256;
		internal const int GFX_Y_CENTRE = 192;

		internal const int GFX_VIEW_TX = 1;
		internal const int GFX_VIEW_TY = 1;
		internal const int GFX_VIEW_BX = 509;
		internal const int GFX_VIEW_BY = 381;
#endif

#if GFX_SCALE
		internal const int GFX_SCALE = 1;
		internal const int GFX_X_OFFSET = 0;
		internal const int GFX_Y_OFFSET = 0;
		internal const int GFX_X_CENTRE = 128;
		internal const int GFX_Y_CENTRE = 96;

		internal const int GFX_VIEW_TX = 1;
		internal const int GFX_VIEW_TY = 1;
		internal const int GFX_VIEW_BX = 253;
		internal const int GFX_VIEW_BY = 191;
#endif

		internal const int GFX_COL_BLACK = 0;
		internal const int GFX_COL_DARK_RED = 28;
		internal const int GFX_COL_WHITE = 255;
		internal const int GFX_COL_GOLD = 39;
		internal const int GFX_COL_RED = 49;
		internal const int GFX_COL_CYAN = 11;

		internal const int GFX_COL_GREY_1 = 248;
		internal const int GFX_COL_GREY_2 = 235;
		internal const int GFX_COL_GREY_3 = 234;
		internal const int GFX_COL_GREY_4 = 237;

		internal const int GFX_COL_BLUE_1 = 45;
		internal const int GFX_COL_BLUE_2 = 46;
		internal const int GFX_COL_BLUE_3 = 133;
		internal const int GFX_COL_BLUE_4 = 4;

		internal const int GFX_COL_RED_3 = 1;
		internal const int GFX_COL_RED_4 = 71;

		internal const int GFX_COL_WHITE_2 = 242;

		internal const int GFX_COL_YELLOW_1 = 37;
		internal const int GFX_COL_YELLOW_2 = 39;
		internal const int GFX_COL_YELLOW_3 = 89;
		internal const int GFX_COL_YELLOW_4 = 160;
		internal const int GFX_COL_YELLOW_5 = 251;

		internal const int GFX_ORANGE_1 = 76;
		internal const int GFX_ORANGE_2 = 77;
		internal const int GFX_ORANGE_3 = 122;

		internal const int GFX_COL_GREEN_1 = 2;
		internal const int GFX_COL_GREEN_2 = 17;
		internal const int GFX_COL_GREEN_3 = 86;

		internal const int GFX_COL_PINK_1 = 183;

		internal const int IMG_GREEN_DOT = 1;
		internal const int IMG_RED_DOT = 2;
		internal const int IMG_BIG_S = 3;
		internal const int IMG_ELITE_TXT = 4;
		internal const int IMG_BIG_E = 5;
		internal const int IMG_DICE = 6;
		internal const int IMG_MISSILE_GREEN = 7;
		internal const int IMG_MISSILE_YELLOW = 8;
		internal const int IMG_MISSILE_RED = 9;
		internal const int IMG_BLAKE = 10;
	}
}