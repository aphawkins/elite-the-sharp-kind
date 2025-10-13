// 'Useful Libraries' - Andy Hawkins 2025.

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
