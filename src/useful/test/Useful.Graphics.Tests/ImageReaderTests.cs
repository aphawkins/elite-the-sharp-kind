// 'Useful Libraries' - Andy Hawkins 2023-2026.

namespace Useful.Graphics.Tests;

public class ImageReaderTests
{
    [Fact]
    public void ReadsABmpRegardlessOfItsExtension()
    {
        // Arrange: TempImageFile writes everything as .img, so a match here
        // can only have come from the file's magic bytes.
        using TempImageFile file = TempImageFile.From(BmpBuilder.Build(1, 1, 32, [0x00, 0x00, 0xFF, 0xFF]));

        // Act
        FastBitmap bitmap = ImageReader.Read(file.Path);

        // Assert
        Assert.Equal(0xFFFF0000, bitmap.GetPixel(0, 0));
    }

    [Fact]
    public void ReadsAPngRegardlessOfItsExtension()
    {
        // Arrange
        using TempImageFile file = TempImageFile.From(PngBuilder.Build(1, 1, 8, 2, [0, 0xFF, 0x00, 0x00]));

        // Act
        FastBitmap bitmap = ImageReader.Read(file.Path);

        // Assert
        Assert.Equal(0xFFFF0000, bitmap.GetPixel(0, 0));
    }

    [Fact]
    public void ReturnsAnEmptyBitmapForAnEmptyFile()
    {
        // Arrange
        using TempImageFile file = TempImageFile.From([]);

        // Act
        FastBitmap bitmap = ImageReader.Read(file.Path);

        // Assert
        Assert.Equal(0, bitmap.Width);
        Assert.Equal(0, bitmap.Height);
    }

    [Fact]
    public void ThrowsOnAnUnrecognisedFormat()
    {
        // Arrange: a GIF header, which nothing here decodes.
        using TempImageFile file = TempImageFile.From([0x47, 0x49, 0x46, 0x38, 0x39, 0x61]);

        // Act / Assert
        Assert.Throws<UsefulException>(() => ImageReader.Read(file.Path));
    }
}
