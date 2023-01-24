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

namespace Elite.Engine
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
	}
}