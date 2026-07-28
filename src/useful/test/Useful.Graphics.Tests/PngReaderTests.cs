// 'Useful Libraries' - Andy Hawkins 2023-2026.

namespace Useful.Graphics.Tests;

public class PngReaderTests
{
    [Fact]
    public void ReadsTruecolourWithAlpha()
    {
        // Arrange: one opaque red pixel, one half-transparent blue one.
        byte[] scanlines = [0, 0xFF, 0x00, 0x00, 0xFF, 0x00, 0x00, 0xFF, 0x80];
        using TempImageFile file = TempImageFile.From(PngBuilder.Build(2, 1, 8, 6, scanlines));

        // Act
        FastBitmap bitmap = PngReader.Read(file.Path);

        // Assert
        Assert.Equal(2, bitmap.Width);
        Assert.Equal(1, bitmap.Height);
        Assert.Equal(0xFFFF0000, bitmap.GetPixel(0, 0));
        Assert.Equal(0x800000FF, bitmap.GetPixel(1, 0));
    }

    [Fact]
    public void ReadsTruecolourWithoutAlphaAsOpaque()
    {
        // Arrange
        byte[] scanlines = [0, 0x10, 0x20, 0x30];
        using TempImageFile file = TempImageFile.From(PngBuilder.Build(1, 1, 8, 2, scanlines));

        // Act
        FastBitmap bitmap = PngReader.Read(file.Path);

        // Assert
        Assert.Equal(0xFF102030, bitmap.GetPixel(0, 0));
    }

    [Fact]
    public void ReadsIndexedPixelsAndTheirPaletteTransparency()
    {
        // Arrange: palette entry 0 is marked fully transparent by tRNS,
        // entry 1 has no tRNS entry at all and so stays opaque.
        byte[] palette = [0xFF, 0x00, 0x00, 0x00, 0xFF, 0x00];
        using TempImageFile file = TempImageFile.From(
            PngBuilder.Build(2, 1, 8, 3, [0, 0x00, 0x01], palette, [0x00]));

        // Act
        FastBitmap bitmap = PngReader.Read(file.Path);

        // Assert
        Assert.Equal(0x00FF0000u, bitmap.GetPixel(0, 0));
        Assert.Equal(0xFF00FF00, bitmap.GetPixel(1, 0));
    }

    [Fact]
    public void ScalesSubByteGreyscaleSamplesAcrossTheFullRange()
    {
        // Arrange: 0x80 is 1 then 0 across the first two bits, so the pixels
        // are white then black rather than 1 and 0.
        using TempImageFile file = TempImageFile.From(PngBuilder.Build(2, 1, 1, 0, [0, 0x80]));

        // Act
        FastBitmap bitmap = PngReader.Read(file.Path);

        // Assert
        Assert.Equal(0xFFFFFFFF, bitmap.GetPixel(0, 0));
        Assert.Equal(0xFF000000, bitmap.GetPixel(1, 0));
    }

    [Fact]
    public void AppliesTheGreyscaleTransparencyKey()
    {
        // Arrange: mid-grey is keyed out, black is not.
        using TempImageFile file = TempImageFile.From(
            PngBuilder.Build(2, 1, 8, 0, [0, 0x80, 0x00], transparency: [0x00, 0x80]));

        // Act
        FastBitmap bitmap = PngReader.Read(file.Path);

        // Assert
        Assert.Equal(0x00808080u, bitmap.GetPixel(0, 0));
        Assert.Equal(0xFF000000, bitmap.GetPixel(1, 0));
    }

    [Fact]
    public void ReadsGreyscaleWithAlpha()
    {
        // Arrange
        using TempImageFile file = TempImageFile.From(PngBuilder.Build(1, 1, 8, 4, [0, 0x40, 0x80]));

        // Act
        FastBitmap bitmap = PngReader.Read(file.Path);

        // Assert
        Assert.Equal(0x80404040, bitmap.GetPixel(0, 0));
    }

    [Fact]
    public void TruncatesSixteenBitSamplesToTheirHighByte()
    {
        // Arrange: 0x10FF, 0x2000, 0x30AA.
        byte[] scanlines = [0, 0x10, 0xFF, 0x20, 0x00, 0x30, 0xAA];
        using TempImageFile file = TempImageFile.From(PngBuilder.Build(1, 1, 16, 2, scanlines));

        // Act
        FastBitmap bitmap = PngReader.Read(file.Path);

        // Assert
        Assert.Equal(0xFF102030, bitmap.GetPixel(0, 0));
    }

    [Fact]
    public void ReversesEveryScanlineFilter()
    {
        // Arrange: five rows encoded with None, Up, Sub, Average and Paeth,
        // each hand-computed to reconstruct to the same two pixels.
        byte[] scanlines =
        [
            0, 10, 20, 30, 40, 50, 60,
            2, 0, 0, 0, 0, 0, 0,
            1, 10, 20, 30, 30, 30, 30,
            3, 5, 10, 15, 15, 15, 15,
            4, 0, 0, 0, 0, 0, 0,
        ];
        using TempImageFile file = TempImageFile.From(PngBuilder.Build(2, 5, 8, 2, scanlines));

        // Act
        FastBitmap bitmap = PngReader.Read(file.Path);

        // Assert
        for (int y = 0; y < 5; y++)
        {
            Assert.Equal(0xFF0A141Eu, bitmap.GetPixel(0, y));
            Assert.Equal(0xFF28323Cu, bitmap.GetPixel(1, y));
        }
    }

    [Fact]
    public void ThrowsOnAnInterlacedImage()
    {
        // Arrange
        using TempImageFile file = TempImageFile.From(
            PngBuilder.Build(1, 1, 8, 2, [0, 0x10, 0x20, 0x30], interlace: 1));

        // Act / Assert
        Assert.Throws<UsefulException>(() => PngReader.Read(file.Path));
    }

    [Fact]
    public void ThrowsOnAnIndexedImageWithNoPalette()
    {
        // Arrange
        using TempImageFile file = TempImageFile.From(PngBuilder.Build(1, 1, 8, 3, [0, 0x00]));

        // Act / Assert
        Assert.Throws<UsefulException>(() => PngReader.Read(file.Path));
    }

    [Fact]
    public void ThrowsOnAnUnsupportedColourType()
    {
        // Arrange
        using TempImageFile file = TempImageFile.From(PngBuilder.Build(1, 1, 8, 5, [0, 0x00]));

        // Act / Assert
        Assert.Throws<UsefulException>(() => PngReader.Read(file.Path));
    }

    [Fact]
    public void ThrowsWhenPixelDataIsTruncated()
    {
        // Arrange: a 4x4 image carrying a single scanline.
        using TempImageFile file = TempImageFile.From(
            PngBuilder.Build(4, 4, 8, 2, [0, 0x10, 0x20, 0x30, 0x10, 0x20, 0x30, 0x10, 0x20, 0x30, 0x10, 0x20, 0x30]));

        // Act / Assert
        Assert.Throws<UsefulException>(() => PngReader.Read(file.Path));
    }
}
