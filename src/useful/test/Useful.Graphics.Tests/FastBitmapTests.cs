// 'Useful Libraries' - Andy Hawkins 2025.

namespace Useful.Graphics.Tests;

public class FastBitmapTests
{
    [Fact]
    public void ResizeByOneRowCopiesExistingPixels()
    {
        // Arrange
        using FastBitmap bitmap = new(2, 2);
        bitmap.SetPixel(0, 0, BaseColors.White.Argb);
        bitmap.SetPixel(1, 1, BaseColors.Red.Argb);

        // Act
        using FastBitmap resized = bitmap.Resize(2, 3);

        // Assert
        Assert.Equal(BaseColors.White.Argb, resized.GetPixel(0, 0));
        Assert.Equal(BaseColors.Red.Argb, resized.GetPixel(1, 1));
        Assert.Equal(BaseColors.TransparentBlack.Argb, resized.GetPixel(0, 2));
        Assert.Equal(BaseColors.TransparentBlack.Argb, resized.GetPixel(1, 2));
    }

    [Fact]
    public void ResizeByOneColumnCopiesExistingPixels()
    {
        // Arrange
        using FastBitmap bitmap = new(2, 2);
        bitmap.SetPixel(0, 0, BaseColors.White.Argb);
        bitmap.SetPixel(1, 1, BaseColors.Red.Argb);

        // Act
        using FastBitmap resized = bitmap.Resize(3, 2);

        // Assert
        Assert.Equal(BaseColors.White.Argb, resized.GetPixel(0, 0));
        Assert.Equal(BaseColors.Red.Argb, resized.GetPixel(1, 1));
        Assert.Equal(BaseColors.TransparentBlack.Argb, resized.GetPixel(2, 0));
        Assert.Equal(BaseColors.TransparentBlack.Argb, resized.GetPixel(2, 1));
    }
}
