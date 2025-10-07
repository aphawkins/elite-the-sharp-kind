// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Diagnostics;

namespace Useful.Graphics;

public static class BitmapFile
{
    private const int BitDepth = 32;
    private const int HeaderLength = 54;
    private const int ExtraHeaderLength = 96;

    public static FastBitmap Read(string path)
    {
        byte[] bytes = File.ReadAllBytes(path);

        if (bytes == null || bytes.Length == 0)
        {
            return new(0, 0);
        }

        // Identifier
        Debug.Assert(bytes[0] == 'B', "Identifier is incorrect");
        Debug.Assert(bytes[1] == 'M', "Identifier is incorrect");

        // Width
        byte[] widthBytes = new byte[4];
        Array.Copy(bytes, 18, widthBytes, 0, 4);
        int width = BitConverter.ToInt32(widthBytes, 0);

        // Height
        byte[] heightBytes = new byte[4];
        Array.Copy(bytes, 22, heightBytes, 0, 4);
        int height = BitConverter.ToInt32(heightBytes, 0);

        // Bits Per Pixel
        byte[] bppBytes = new byte[2];
        Array.Copy(bytes, 28, bppBytes, 0, 2);
        short fileBitDepth = BitConverter.ToInt16(bppBytes, 0);
        if (fileBitDepth != BitDepth)
        {
            throw new UsefulException($"Bit Depth is incorrect. Found: {fileBitDepth}. Should be: {BitDepth}");
        }

        ////int imageLength = width * height * BitDepth / 8;

        ////// File Size
        ////byte[] lengthBytes = new byte[4];
        ////Array.Copy(bytes, 2, lengthBytes, 0, 4);
        ////Debug.Assert(
        ////    BitConverter.ToInt32(lengthBytes, 0) == HeaderLength + _imageLength,
        ////    "File Size is incorrect");

        uint[] pixels = new uint[width * height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                byte[] colorBytes = new byte[4];
                Array.Copy(bytes, HeaderLength + ExtraHeaderLength + (((y * width) + x) * 4), colorBytes, 0, 4);
                if (!BitConverter.IsLittleEndian)
                {
                    Array.Reverse(colorBytes);
                }

                // Flip image horizontally
                pixels[((height - y - 1) * width) + x] = (uint)BitConverter.ToInt32(colorBytes, 0);
            }
        }

        return new(width, height, pixels);
    }
}
