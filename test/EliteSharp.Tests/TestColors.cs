// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Graphics;

namespace EliteSharp.Tests
{
    public static class TestColors
    {
        public static FastColor OpaqueBlack => new(0xFF000000);

        public static FastColor TransparentBlack => new(0x00000000);

        public static FastColor OpaqueWhite => new(0xFFFFFFFF);

        public static FastColor TransparentWhite => new(0x00FFFFFF);

        public static FastColor OpaqueRed => new(0xFFFF0000);
    }
}
