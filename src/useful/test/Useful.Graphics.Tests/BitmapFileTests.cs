// 'Useful Libraries' - Andy Hawkins 2025.

namespace Useful.Graphics.Tests;

public class BitmapFileTests
{
    [Theory]
    [InlineData("2x2redtopleft.bmp", 2, 2)]
    public void LoadBitmapOrientation(string filename, int width, int height)
    {
        // Arrange
        string path = Path.Combine("golden", filename);

        // Act
        FastBitmap bitmap = BitmapFile.Read(path);

        // Assert
        Assert.Equal(width, bitmap.Width);
        Assert.Equal(height, bitmap.Height);
        Assert.Equal(32, bitmap.BitsPerPixel);
        Assert.Equal(BaseColors.Red, bitmap.GetPixel(0, 0));
        Assert.Equal(BaseColors.TransparentBlack, bitmap.GetPixel(0, 1));
        Assert.Equal(BaseColors.TransparentBlack, bitmap.GetPixel(1, 0));
        Assert.Equal(BaseColors.TransparentBlack, bitmap.GetPixel(1, 1));
    }
}
