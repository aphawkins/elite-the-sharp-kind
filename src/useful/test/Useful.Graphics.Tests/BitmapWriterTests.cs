// 'Useful Libraries' - Andy Hawkins 2025.

namespace Useful.Graphics.Tests;

public class BitmapWriterTests
{
    [Fact]
    public void WriteProducesAValidBmpSignatureAndLength()
    {
        // Arrange
        using FastBitmap bitmap = new(2, 2);
        string path = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.bmp");

        try
        {
            // Act
            BitmapWriter.Write(bitmap, path);

            // Assert
            byte[] bytes = File.ReadAllBytes(path);
            Assert.Equal((byte)'B', bytes[0]);
            Assert.Equal((byte)'M', bytes[1]);
            Assert.Equal(bytes.Length, BitConverter.ToInt32(bytes, 2));
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void WriteThenReadRoundTripsPixelsIncludingAlpha()
    {
        // Arrange: a distinct, non-symmetric colour per corner so a
        // transposed or flipped round trip would be caught.
        using FastBitmap bitmap = new(2, 2);
        bitmap.SetPixel(0, 0, BaseColors.Red);
        bitmap.SetPixel(1, 0, BaseColors.TransparentBlack);
        bitmap.SetPixel(0, 1, new FastColor(0x80, 0x11, 0x22, 0x33));
        bitmap.SetPixel(1, 1, BaseColors.White);
        string path = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.bmp");

        try
        {
            // Act
            BitmapWriter.Write(bitmap, path);
            FastBitmap roundTripped = BitmapReader.Read(path);

            // Assert
            Assert.Equal(bitmap.Width, roundTripped.Width);
            Assert.Equal(bitmap.Height, roundTripped.Height);
            Assert.Equal(bitmap.GetPixel(0, 0), roundTripped.GetPixel(0, 0));
            Assert.Equal(bitmap.GetPixel(1, 0), roundTripped.GetPixel(1, 0));
            Assert.Equal(bitmap.GetPixel(0, 1), roundTripped.GetPixel(0, 1));
            Assert.Equal(bitmap.GetPixel(1, 1), roundTripped.GetPixel(1, 1));
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void WriteThenReadRoundTripsANonSquareBitmap()
    {
        // Arrange: width != height so a row/column mix-up in either the
        // writer or reader would surface as a size or pixel mismatch.
        using FastBitmap bitmap = new(3, 1);
        bitmap.SetPixel(0, 0, BaseColors.Red);
        bitmap.SetPixel(1, 0, BaseColors.White);
        bitmap.SetPixel(2, 0, BaseColors.TransparentBlack);
        string path = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.bmp");

        try
        {
            // Act
            BitmapWriter.Write(bitmap, path);
            FastBitmap roundTripped = BitmapReader.Read(path);

            // Assert
            Assert.Equal(3, roundTripped.Width);
            Assert.Equal(1, roundTripped.Height);
            Assert.Equal(bitmap.GetPixel(0, 0), roundTripped.GetPixel(0, 0));
            Assert.Equal(bitmap.GetPixel(1, 0), roundTripped.GetPixel(1, 0));
            Assert.Equal(bitmap.GetPixel(2, 0), roundTripped.GetPixel(2, 0));
        }
        finally
        {
            File.Delete(path);
        }
    }
}
