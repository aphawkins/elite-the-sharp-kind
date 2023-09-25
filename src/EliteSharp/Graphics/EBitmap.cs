// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Diagnostics;

namespace EliteSharp.Graphics
{
    public class EBitmap
    {
        private const int HeaderLength = 54;

        public EBitmap(int width, int height)
        {
            BitDepth = 32;
            int imageLength = width * height * BitDepth / 8;
            Bytes = new byte[HeaderLength + imageLength];

            // Identifier
            Bytes[0] = (byte)'B';
            Bytes[1] = (byte)'M';

            // File Size
            Array.Copy(BitConverter.GetBytes(Bytes.Length), 0, Bytes, 2, 4);

            // Total header size (Offset to the image bytes)
            Array.Copy(BitConverter.GetBytes(HeaderLength), 0, Bytes, 10, 4);

            // DIB Header Size
            Bytes[14] = 40;

            // Width
            Width = width;
            Array.Copy(BitConverter.GetBytes(width), 0, Bytes, 18, 4);

            // Height
            Height = height;
            Array.Copy(BitConverter.GetBytes(height), 0, Bytes, 22, 4);

            // Bits Per Pixel
            Array.Copy(BitConverter.GetBytes(BitDepth), 0, Bytes, 28, 2);

            // Pixel bytes length
            Array.Copy(BitConverter.GetBytes(imageLength), 0, Bytes, 34, 4);
        }

        public EBitmap(byte[] bytes)
        {
            Bytes = bytes;

            // Identifier
            Debug.Assert(Bytes[0] == 'B', "Identifier is correct");
            Debug.Assert(Bytes[1] == 'M', "Identifier is correct");

            // File Size
            byte[] lengthBytes = new byte[4];
            Array.Copy(Bytes, 2, lengthBytes, 0, 4);
            Debug.Assert(BitConverter.ToInt32(lengthBytes, 0) == Bytes.Length, "File Size is correct");

            // Width
            byte[] widthBytes = new byte[4];
            Array.Copy(Bytes, 18, widthBytes, 0, 4);
            Width = BitConverter.ToInt32(widthBytes, 0);

            // Height
            byte[] heightBytes = new byte[4];
            Array.Copy(Bytes, 22, heightBytes, 0, 4);
            Height = BitConverter.ToInt32(heightBytes, 0);

            // Bits Per Pixel
            byte[] bppBytes = new byte[2];
            Array.Copy(Bytes, 28, bppBytes, 0, 2);
            BitDepth = BitConverter.ToInt16(bppBytes, 0);
        }

#pragma warning disable CA1819 // Properties should not return arrays
        public byte[] Bytes { get; }
#pragma warning restore CA1819 // Properties should not return arrays

        public int Height { get; }

        public int Width { get; }

        public int BitDepth { get; }

        public EColor GetPixel(int x, int y)
        {
            if (BitDepth != 32 || x < 0 || x > Width - 1 || y < 0 || y > Height - 1)
            {
                return new(255, 0, 0, 0);
            }

            int offset = HeaderLength + ((((Height - y - 1) * Width) + x) * (BitDepth / 8));
            return new(
                Bytes[offset + 3],
                Bytes[offset + 2],
                Bytes[offset + 1],
                Bytes[offset + 0]);
        }

        public void SetPixel(int x, int y, EColor color)
        {
            if (BitDepth != 32 || x < 0 || x > Width - 1 || y < 0 || y > Height - 1)
            {
                return;
            }

            int offset = HeaderLength + ((((Height - y - 1) * Width) + x) * (BitDepth / 8));
            Bytes[offset + 0] = color.B;
            Bytes[offset + 1] = color.G;
            Bytes[offset + 2] = color.R;
            Bytes[offset + 3] = color.A;
        }
    }
}
