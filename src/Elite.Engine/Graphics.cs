// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

namespace Elite.Engine
{
    public static class Graphics
    {
#if RES_512_512
        public const float GFX_SCALE = 2;
        public const float GFX_X_OFFSET = 0;
        public const float GFX_Y_OFFSET = 0;
        public const float GFX_X_CENTRE = 256;
        public const float GFX_Y_CENTRE = 192;

        public const float GFX_VIEW_TX = 1;
        public const float GFX_VIEW_TY = 1;
        public const float GFX_VIEW_BX = 509;
        public const float GFX_VIEW_BY = 381;
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
