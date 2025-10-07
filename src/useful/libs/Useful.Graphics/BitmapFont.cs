// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Diagnostics;

namespace Useful.Graphics;

public class BitmapFont
{
    public BitmapFont(FastBitmap fontImage)
    {
        Guard.ArgumentNull(fontImage);

        Debug.Assert(fontImage.Width == 513, "Font bitmap is not the correct width.");
        Debug.Assert(fontImage.Height == 193, "Font bitmap is not the correct height.");

        Image = fontImage;
    }

    public static int CharSize => 32;

    public FastBitmap Image { get; }
}
