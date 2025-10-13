// 'Useful Libraries' - Andy Hawkins 2025.

using Moq;
using Useful.Assets;

namespace Useful.Graphics.Tests;

public class SoftwareGraphicsTests
{
    [Fact]
    public void Clear()
    {
        // Arrange
        Mock<IAssetLocator> moqAssetLocator = ArrangeAssets();
        using SoftwareGraphics graphics = new(5, 5, DoAssert);
        graphics.Initialize(moqAssetLocator.Object, []);

        // Act
        graphics.DrawPixel(new(2, 2), BaseColors.White);
        graphics.Clear();
        graphics.ScreenUpdate();

        // Assert
        static void DoAssert(FastBitmap bmp) => Assert.Equal(BaseColors.Black, bmp.GetPixel(2, 2));
    }

    [Fact]
    public void DrawCircleFilledInBounds()
    {
        // Arrange
        Mock<IAssetLocator> moqAssetLocator = ArrangeAssets();
        using SoftwareGraphics graphics = new(5, 5, DoAssert);
        graphics.Initialize(moqAssetLocator.Object, []);

        // Act
        graphics.DrawCircleFilled(new(2, 2), 2, BaseColors.White);
        graphics.ScreenUpdate();

        // Assert
        static void DoAssert(FastBitmap bmp)
        {
            Assert.Equal(BaseColors.White, bmp.GetPixel(0, 2));
            Assert.Equal(BaseColors.White, bmp.GetPixel(4, 2));
            Assert.Equal(BaseColors.White, bmp.GetPixel(2, 0));
            Assert.Equal(BaseColors.White, bmp.GetPixel(2, 4));
            Assert.Equal(BaseColors.White, bmp.GetPixel(2, 2));
        }
    }

    [Theory]
    [InlineData(2, 2, 3, 0xFFFFFFFF)]
    [InlineData(2, 2, 9, 0xFFFFFFFF)]
    [InlineData(-9, -9, 3, 0xFF000000)]
    [InlineData(9, 9, 3, 0xFF000000)]
    public void DrawCircleFilledOutOfBounds(float x, float y, float radius, uint centreColor)
    {
        // Arrange
        Mock<IAssetLocator> moqAssetLocator = ArrangeAssets();
        using SoftwareGraphics graphics = new(5, 5, DoAssert);
        graphics.Initialize(moqAssetLocator.Object, []);

        // Act
        graphics.DrawCircleFilled(new(x, y), radius, BaseColors.White);
        graphics.ScreenUpdate();

        // Assert
        void DoAssert(FastBitmap bmp)
        {
            for (int screenY = 0; screenY < 5; screenY++)
            {
                for (int screenX = 0; screenX < 5; screenX++)
                {
                    Assert.Equal(new FastColor(centreColor), bmp.GetPixel(screenX, screenY));
                }
            }
        }
    }

    [Fact]
    public void DrawCircleInBounds()
    {
        // Arrange
        Mock<IAssetLocator> moqAssetLocator = ArrangeAssets();
        using SoftwareGraphics graphics = new(5, 5, DoAssert);
        graphics.Initialize(moqAssetLocator.Object, []);

        // Act
        graphics.DrawCircle(new(2, 2), 2, BaseColors.White);
        graphics.ScreenUpdate();

        // Assert
        static void DoAssert(FastBitmap bmp)
        {
            Assert.Equal(BaseColors.White, bmp.GetPixel(0, 2));
            Assert.Equal(BaseColors.White, bmp.GetPixel(4, 2));
            Assert.Equal(BaseColors.White, bmp.GetPixel(2, 0));
            Assert.Equal(BaseColors.White, bmp.GetPixel(2, 4));
        }
    }

    [Theory]
    [InlineData(2, 2, 3)]
    [InlineData(2, 2, 9)]
    [InlineData(-9, -9, 3)]
    [InlineData(9, 9, 3)]
    public void DrawCircleOutOfBounds(float x, float y, float radius)
    {
        // Arrange
        Mock<IAssetLocator> moqAssetLocator = ArrangeAssets();
        using SoftwareGraphics graphics = new(5, 5, DoAssert);
        graphics.Initialize(moqAssetLocator.Object, []);

        // Act
        graphics.DrawCircle(new(x, y), radius, BaseColors.White);
        graphics.ScreenUpdate();

        // Assert
        static void DoAssert(FastBitmap bmp)
        {
            for (int screenY = 0; screenY < 5; screenY++)
            {
                for (int screenX = 0; screenX < 5; screenX++)
                {
                    Assert.Equal(BaseColors.Black, bmp.GetPixel(screenX, screenY));
                }
            }
        }
    }

    [Fact]
    public void DrawCirclePartialInBounds()
    {
        // Arrange
        Mock<IAssetLocator> moqAssetLocator = ArrangeAssets();
        using SoftwareGraphics graphics = new(5, 5, DoAssert);
        graphics.Initialize(moqAssetLocator.Object, []);

        // Act
        graphics.DrawCircle(new(0, 0), 4, BaseColors.White);
        graphics.ScreenUpdate();

        // Assert
        static void DoAssert(FastBitmap bmp)
        {
            Assert.Equal(BaseColors.White, bmp.GetPixel(0, 4));
            Assert.Equal(BaseColors.White, bmp.GetPixel(4, 0));
        }
    }

    [Theory]
    [InlineData(2, 2, "2x2redtopleft.bmp", 0, 0)]
    [InlineData(4, 4, "2x2redtopleft.bmp", 2, 2)]
    public void DrawImage(int width, int height, string filename, int imageX, int imageY)
    {
        // Arrange
        Mock<IAssetLocator> moqAssetLocator = ArrangeAssets(filename);
        using SoftwareGraphics graphics = new(width, height, DoAssert);
        graphics.Initialize(moqAssetLocator.Object, []);

        // Act
        graphics.DrawImage(123, new(imageX, imageY));
        graphics.ScreenUpdate();

        // Assert
        void DoAssert(FastBitmap bmp) => Assert.Equal(BaseColors.Red, bmp.GetPixel(imageX, imageY));
    }

    [Theory]
    [InlineData("2x2redtopleft.bmp", 2, 2)]
    public void DrawImageOutOfBounds(string filename, int imageWidth, int imageHeight)
    {
        // Arrange
        Mock<IAssetLocator> moqAssetLocator = ArrangeAssets(filename);
        using SoftwareGraphics graphics = new(imageWidth, imageHeight, DoAssert);
        graphics.Initialize(moqAssetLocator.Object, []);

        // Act
        graphics.DrawImage(123, new(1, 1));
        graphics.ScreenUpdate();

        // Assert
        static void DoAssert(FastBitmap bmp) => Assert.Equal(BaseColors.Red, bmp.GetPixel(1, 1));
    }

    [Theory]
    [InlineData("2x2redtopleft.bmp")]
    public void DrawImageTransparent(string filename)
    {
        // Arrange
        Mock<IAssetLocator> moqAssetLocator = ArrangeAssets(filename);
        using SoftwareGraphics graphics = new(2, 2, DoAssert);
        graphics.Initialize(moqAssetLocator.Object, []);
        graphics.DrawPixel(new(1, 1), BaseColors.White);

        // Act
        graphics.DrawImage(123, new(0, 0));
        graphics.ScreenUpdate();

        // Assert
        static void DoAssert(FastBitmap bmp)
        {
            Assert.Equal(BaseColors.Red, bmp.GetPixel(0, 0));
            Assert.Equal(BaseColors.Black, bmp.GetPixel(0, 1));
            Assert.Equal(BaseColors.Black, bmp.GetPixel(1, 0));
            Assert.Equal(BaseColors.White, bmp.GetPixel(1, 1));
        }
    }

    [Theory]
    [InlineData(0, 0, 4, 4)]
    [InlineData(0, 4, 4, 0)]
    [InlineData(0, 0, 4, 0)]
    [InlineData(0, 0, 0, 4)]
    public void DrawLineInBounds(float startX, float startY, float endX, float endY)
    {
        // Arrange
        Mock<IAssetLocator> moqAssetLocator = ArrangeAssets();
        using SoftwareGraphics graphics = new(5, 5, DoAssert);
        graphics.Initialize(moqAssetLocator.Object, []);

        // Act
        graphics.DrawLine(new(startX, startY), new(endX, endY), BaseColors.White);
        graphics.ScreenUpdate();

        // Assert
        void DoAssert(FastBitmap bmp)
        {
            Assert.Equal(BaseColors.White, bmp.GetPixel((int)startX, (int)startY));
            Assert.Equal(BaseColors.White, bmp.GetPixel((int)endX, (int)endY));
        }
    }

    [Theory]
    [InlineData(0, 0, 5, 5)]
    [InlineData(-1, -1, 5, 5)]
    [InlineData(-1, 5, 5, -1)]
    [InlineData(-1, 2, 5, 2)]
    public void DrawLineOutOfBounds(float startX, float startY, float endX, float endY)
    {
        // Arrange
        Mock<IAssetLocator> moqAssetLocator = ArrangeAssets();
        using SoftwareGraphics graphics = new(5, 5, DoAssert);
        graphics.Initialize(moqAssetLocator.Object, []);

        // Act
        graphics.DrawLine(new(startX, startY), new(endX, endY), BaseColors.White);
        graphics.ScreenUpdate();

        // Assert
        static void DoAssert(FastBitmap bmp) => Assert.Equal(BaseColors.White, bmp.GetPixel(2, 2));
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(2, 2)]
    [InlineData(4, 4)]
    public void DrawPixelInBounds(float x, float y)
    {
        // Arrange
        Mock<IAssetLocator> moqAssetLocator = ArrangeAssets();
        using SoftwareGraphics graphics = new(5, 5, DoAssert);
        graphics.Initialize(moqAssetLocator.Object, []);

        // Act
        graphics.DrawPixel(new(x, y), BaseColors.White);
        graphics.ScreenUpdate();

        // Assert
        void DoAssert(FastBitmap bmp) => Assert.Equal(BaseColors.White, bmp.GetPixel((int)x, (int)y));
    }

    [Theory]
    [InlineData(-9, -9)]
    [InlineData(9, 9)]
    public void DrawPixelOutOfBounds(float x, float y)
    {
        // Arrange
        Mock<IAssetLocator> moqAssetLocator = ArrangeAssets();
        using SoftwareGraphics graphics = new(5, 5, (_) => { });
        graphics.Initialize(moqAssetLocator.Object, []);

        // Act
        graphics.DrawPixel(new(x, y), BaseColors.White);
        graphics.ScreenUpdate();

        // Assert
    }

    [Theory]
    [InlineData("2x2redtopleft.bmp", 2, 2)]
    public void LoadImage(string filename, int width, int height)
    {
        // Arrange
        Mock<IAssetLocator> moqAssetLocator = ArrangeAssets(filename);
        using SoftwareGraphics graphics = new(width, height, (_) => { });
        graphics.Initialize(moqAssetLocator.Object, []);

        // Act
        graphics.ScreenUpdate();

        // Assert
    }

    private static string GraphicsFilename(string filename)
        => Path.Combine("golden", filename);

    private static Mock<IAssetLocator> ArrangeAssets(string filename = "")
    {
        Mock<IAssetLocator> moqAssetLocator = new();

        if (string.IsNullOrEmpty(filename))
        {
            moqAssetLocator.Setup(x => x.ImagePaths)
                .Returns(new Dictionary<int, string>());
        }
        else
        {
            moqAssetLocator.Setup(x => x.ImagePaths)
                .Returns(new Dictionary<int, string>() { { 123, GraphicsFilename(filename) } });
        }

        moqAssetLocator.Setup(x => x.FontBitmapPaths)
            .Returns(new Dictionary<int, string>());

        return moqAssetLocator;
    }
}
