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
	public static class gfx
	{
#if RES_512_512
		public const int GFX_SCALE = 2;
		public const int GFX_X_OFFSET = 0;
		public const int GFX_Y_OFFSET = 0;
		public const int GFX_X_CENTRE = 256;
		public const int GFX_Y_CENTRE = 192;

		public const int GFX_VIEW_TX = 1;
		public const int GFX_VIEW_TY = 1;
		public const int GFX_VIEW_BX = 509;
		public const int GFX_VIEW_BY = 381;
#endif

#if RES_800_600
		public const int GFX_SCALE = 2;
		public const int GFX_X_OFFSET = 144;
		public const int GFX_Y_OFFSET = 44;
		public const int GFX_X_CENTRE = 256;
		public const int GFX_Y_CENTRE = 192;

		public const int GFX_VIEW_TX = 1;
		public const int GFX_VIEW_TY = 1;
		public const int GFX_VIEW_BX = 509;
		public const int GFX_VIEW_BY = 381;
#endif

#if GFX_SCALE
		public const int GFX_SCALE = 1;
		public const int GFX_X_OFFSET = 0;
		public const int GFX_Y_OFFSET = 0;
		public const int GFX_X_CENTRE = 128;
		public const int GFX_Y_CENTRE = 96;

		public const int GFX_VIEW_TX = 1;
		public const int GFX_VIEW_TY = 1;
		public const int GFX_VIEW_BX = 253;
		public const int GFX_VIEW_BY = 191;
#endif

		public const int GFX_COL_BLACK = 0;
		public const int GFX_COL_DARK_RED = 28;
		public const int GFX_COL_WHITE = 255;
		public const int GFX_COL_GOLD = 39;
		public const int GFX_COL_RED = 49;
		public const int GFX_COL_CYAN = 11;

		public const int GFX_COL_GREY_1 = 248;
		public const int GFX_COL_GREY_2 = 235;
		public const int GFX_COL_GREY_3 = 234;
		public const int GFX_COL_GREY_4 = 237;

		public const int GFX_COL_BLUE_1 = 45;
		public const int GFX_COL_BLUE_2 = 46;
		public const int GFX_COL_BLUE_3 = 133;
		public const int GFX_COL_BLUE_4 = 4;

		public const int GFX_COL_RED_3 = 1;
		public const int GFX_COL_RED_4 = 71;

		public const int GFX_COL_WHITE_2 = 242;

		public const int GFX_COL_YELLOW_1 = 37;
		public const int GFX_COL_YELLOW_2 = 39;
		public const int GFX_COL_YELLOW_3 = 89;
		public const int GFX_COL_YELLOW_4 = 160;
		public const int GFX_COL_YELLOW_5 = 251;

		public const int GFX_ORANGE_1 = 76;
		public const int GFX_ORANGE_2 = 77;
		public const int GFX_ORANGE_3 = 122;

		public const int GFX_COL_GREEN_1 = 2;
		public const int GFX_COL_GREEN_2 = 17;
		public const int GFX_COL_GREEN_3 = 86;

		public const int GFX_COL_PINK_1 = 183;

		public const int IMG_GREEN_DOT = 1;
		public const int IMG_RED_DOT = 2;
		public const int IMG_BIG_S = 3;
		public const int IMG_ELITE_TXT = 4;
		public const int IMG_BIG_E = 5;
		public const int IMG_DICE = 6;
		public const int IMG_MISSILE_GREEN = 7;
		public const int IMG_MISSILE_YELLOW = 8;
		public const int IMG_MISSILE_RED = 9;
		public const int IMG_BLAKE = 10;
	}
}