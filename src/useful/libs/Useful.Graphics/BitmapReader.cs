// 'Useful Libraries' - Andy Hawkins 2025.

using System.Buffers.Binary;

namespace Useful.Graphics;

public static class BitmapReader
{
    internal const int HeaderLength = 54;
    internal const int ExtraHeaderLength = 96;

    private const int BitDepth = 32;
    private const int WidthOffset = 18;
    private const int HeightOffset = 22;
    private const int BitsPerPixelOffset = 28;

    public static FastBitmap Read(string path)
    {
        byte[] bytes = File.ReadAllBytes(path);

        if (bytes.Length == 0)
        {
            return new(0, 0);
        }

        if (bytes[0] != 'B' || bytes[1] != 'M')
        {
            throw new UsefulException("Identifier is incorrect: not a BMP file.");
        }

        int width = BinaryPrimitives.ReadInt32LittleEndian(bytes.AsSpan(WidthOffset));
        int height = BinaryPrimitives.ReadInt32LittleEndian(bytes.AsSpan(HeightOffset));

        short fileBitDepth = BinaryPrimitives.ReadInt16LittleEndian(bytes.AsSpan(BitsPerPixelOffset));
        if (fileBitDepth != BitDepth)
        {
            throw new UsefulException($"Bit Depth is incorrect. Found: {fileBitDepth}. Should be: {BitDepth}");
        }

        uint[] pixels = new uint[width * height];
        const int pixelDataOffset = HeaderLength + ExtraHeaderLength;

        for (int y = 0; y < height; y++)
        {
            int rowOffset = pixelDataOffset + (y * width * 4);

            // Flip image vertically: the file stores rows bottom-up.
            int destinationRow = (height - y - 1) * width;

            for (int x = 0; x < width; x++)
            {
                pixels[destinationRow + x] = (uint)BinaryPrimitives.ReadInt32LittleEndian(bytes.AsSpan(rowOffset + (x * 4)));
            }
        }

        return new(width, height, pixels);
    }
}
