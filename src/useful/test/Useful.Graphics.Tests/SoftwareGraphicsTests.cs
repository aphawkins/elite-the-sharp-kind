// 'Useful Libraries' - Andy Hawkins 2025.

using System.Numerics;
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
        using SoftwareGraphics graphics = SoftwareGraphics.Create(5, 5, DoAssert, moqAssetLocator.Object);

        // Act
        graphics.DrawPixel(new(2, 2), BaseColors.White.Argb);
        graphics.Clear();
        graphics.ScreenUpdate();

        // Assert
        static void DoAssert(FastBitmap bmp) => Assert.Equal(BaseColors.Black.Argb, bmp.GetPixel(2, 2));
    }

    [Fact]
    public void DrawCircleFilledInBounds()
    {
        // Arrange
        Mock<IAssetLocator> moqAssetLocator = ArrangeAssets();
        using SoftwareGraphics graphics = SoftwareGraphics.Create(5, 5, DoAssert, moqAssetLocator.Object);

        // Act
        graphics.DrawCircleFilled(new(2, 2), 2, BaseColors.White.Argb);
        graphics.ScreenUpdate();

        // Assert
        static void DoAssert(FastBitmap bmp)
        {
            Assert.Equal(BaseColors.White.Argb, bmp.GetPixel(0, 2));
            Assert.Equal(BaseColors.White.Argb, bmp.GetPixel(4, 2));
            Assert.Equal(BaseColors.White.Argb, bmp.GetPixel(2, 0));
            Assert.Equal(BaseColors.White.Argb, bmp.GetPixel(2, 4));
            Assert.Equal(BaseColors.White.Argb, bmp.GetPixel(2, 2));
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
        using SoftwareGraphics graphics = SoftwareGraphics.Create(5, 5, DoAssert, moqAssetLocator.Object);

        // Act
        graphics.DrawCircleFilled(new(x, y), radius, BaseColors.White.Argb);
        graphics.ScreenUpdate();

        // Assert
        void DoAssert(FastBitmap bmp)
        {
            for (int screenY = 0; screenY < 5; screenY++)
            {
                for (int screenX = 0; screenX < 5; screenX++)
                {
                    Assert.Equal(centreColor, bmp.GetPixel(screenX, screenY));
                }
            }
        }
    }

    [Fact]
    public void DrawCircleInBounds()
    {
        // Arrange
        Mock<IAssetLocator> moqAssetLocator = ArrangeAssets();
        using SoftwareGraphics graphics = SoftwareGraphics.Create(5, 5, DoAssert, moqAssetLocator.Object);

        // Act
        graphics.DrawCircle(new(2, 2), 2, BaseColors.White.Argb);
        graphics.ScreenUpdate();

        // Assert
        static void DoAssert(FastBitmap bmp)
        {
            Assert.Equal(BaseColors.White.Argb, bmp.GetPixel(0, 2));
            Assert.Equal(BaseColors.White.Argb, bmp.GetPixel(4, 2));
            Assert.Equal(BaseColors.White.Argb, bmp.GetPixel(2, 0));
            Assert.Equal(BaseColors.White.Argb, bmp.GetPixel(2, 4));
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
        using SoftwareGraphics graphics = SoftwareGraphics.Create(5, 5, DoAssert, moqAssetLocator.Object);

        // Act
        graphics.DrawCircle(new(x, y), radius, BaseColors.White.Argb);
        graphics.ScreenUpdate();

        // Assert
        static void DoAssert(FastBitmap bmp)
        {
            for (int screenY = 0; screenY < 5; screenY++)
            {
                for (int screenX = 0; screenX < 5; screenX++)
                {
                    Assert.Equal(BaseColors.Black.Argb, bmp.GetPixel(screenX, screenY));
                }
            }
        }
    }

    [Fact]
    public void DrawCirclePartialInBounds()
    {
        // Arrange
        Mock<IAssetLocator> moqAssetLocator = ArrangeAssets();
        using SoftwareGraphics graphics = SoftwareGraphics.Create(5, 5, DoAssert, moqAssetLocator.Object);

        // Act
        graphics.DrawCircle(new(0, 0), 4, BaseColors.White.Argb);
        graphics.ScreenUpdate();

        // Assert
        static void DoAssert(FastBitmap bmp)
        {
            Assert.Equal(BaseColors.White.Argb, bmp.GetPixel(0, 4));
            Assert.Equal(BaseColors.White.Argb, bmp.GetPixel(4, 0));
        }
    }

    [Theory]
    [InlineData(2, 2, "2x2redtopleft.bmp", 0, 0)]
    [InlineData(4, 4, "2x2redtopleft.bmp", 2, 2)]
    public void DrawImage(int width, int height, string filename, int imageX, int imageY)
    {
        // Arrange
        Mock<IAssetLocator> moqAssetLocator = ArrangeAssets(filename);
        using SoftwareGraphics graphics = SoftwareGraphics.Create(width, height, DoAssert, moqAssetLocator.Object);

        // Act
        graphics.DrawImage(123, new(imageX, imageY));
        graphics.ScreenUpdate();

        // Assert
        void DoAssert(FastBitmap bmp) => Assert.Equal(BaseColors.Red.Argb, bmp.GetPixel(imageX, imageY));
    }

    [Theory]
    [InlineData("2x2redtopleft.bmp", 2, 2)]
    public void DrawImageOutOfBounds(string filename, int imageWidth, int imageHeight)
    {
        // Arrange
        Mock<IAssetLocator> moqAssetLocator = ArrangeAssets(filename);
        using SoftwareGraphics graphics = SoftwareGraphics.Create(imageWidth, imageHeight, DoAssert, moqAssetLocator.Object);

        // Act
        graphics.DrawImage(123, new(1, 1));
        graphics.ScreenUpdate();

        // Assert
        static void DoAssert(FastBitmap bmp) => Assert.Equal(BaseColors.Red.Argb, bmp.GetPixel(1, 1));
    }

    [Theory]
    [InlineData("2x2redtopleft.bmp")]
    public void DrawImageTransparent(string filename)
    {
        // Arrange
        Mock<IAssetLocator> moqAssetLocator = ArrangeAssets(filename);
        using SoftwareGraphics graphics = SoftwareGraphics.Create(2, 2, DoAssert, moqAssetLocator.Object);
        graphics.DrawPixel(new(1, 1), BaseColors.White.Argb);

        // Act
        graphics.DrawImage(123, new(0, 0));
        graphics.ScreenUpdate();

        // Assert
        static void DoAssert(FastBitmap bmp)
        {
            Assert.Equal(BaseColors.Red.Argb, bmp.GetPixel(0, 0));
            Assert.Equal(BaseColors.Black.Argb, bmp.GetPixel(0, 1));
            Assert.Equal(BaseColors.Black.Argb, bmp.GetPixel(1, 0));
            Assert.Equal(BaseColors.White.Argb, bmp.GetPixel(1, 1));
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
        using SoftwareGraphics graphics = SoftwareGraphics.Create(5, 5, DoAssert, moqAssetLocator.Object);

        // Act
        graphics.DrawLine(new(startX, startY), new(endX, endY), BaseColors.White.Argb);
        graphics.ScreenUpdate();

        // Assert
        void DoAssert(FastBitmap bmp)
        {
            Assert.Equal(BaseColors.White.Argb, bmp.GetPixel((int)startX, (int)startY));
            Assert.Equal(BaseColors.White.Argb, bmp.GetPixel((int)endX, (int)endY));
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
        using SoftwareGraphics graphics = SoftwareGraphics.Create(5, 5, DoAssert, moqAssetLocator.Object);

        // Act
        graphics.DrawLine(new(startX, startY), new(endX, endY), BaseColors.White.Argb);
        graphics.ScreenUpdate();

        // Assert
        static void DoAssert(FastBitmap bmp) => Assert.Equal(BaseColors.White.Argb, bmp.GetPixel(2, 2));
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(2, 2)]
    [InlineData(4, 4)]
    public void DrawPixelInBounds(float x, float y)
    {
        // Arrange
        Mock<IAssetLocator> moqAssetLocator = ArrangeAssets();
        using SoftwareGraphics graphics = SoftwareGraphics.Create(5, 5, DoAssert, moqAssetLocator.Object);

        // Act
        graphics.DrawPixel(new(x, y), BaseColors.White.Argb);
        graphics.ScreenUpdate();

        // Assert
        void DoAssert(FastBitmap bmp) => Assert.Equal(BaseColors.White.Argb, bmp.GetPixel((int)x, (int)y));
    }

    [Theory]
    [InlineData(-9, -9)]
    [InlineData(9, 9)]
    public void DrawPixelOutOfBounds(float x, float y)
    {
        // Arrange
        Mock<IAssetLocator> moqAssetLocator = ArrangeAssets();
        using SoftwareGraphics graphics = SoftwareGraphics.Create(5, 5, (_) => { }, moqAssetLocator.Object);

        // Act
        graphics.DrawPixel(new(x, y), BaseColors.White.Argb);
        graphics.ScreenUpdate();

        // Assert
    }

    [Theory]
    [InlineData("2x2redtopleft.bmp", 2, 2)]
    public void LoadImage(string filename, int width, int height)
    {
        // Arrange
        Mock<IAssetLocator> moqAssetLocator = ArrangeAssets(filename);
        using SoftwareGraphics graphics = SoftwareGraphics.Create(width, height, (_) => { }, moqAssetLocator.Object);

        // Act
        graphics.ScreenUpdate();

        // Assert
    }

    [Fact]
    public void PropertiesSetAndDimensions()
    {
        // Arrange
        Mock<IAssetLocator> moqAssetLocator = ArrangeAssets();

        // Act
        using SoftwareGraphics graphics = SoftwareGraphics.Create(3, 4, (_) => { }, moqAssetLocator.Object);

        // Assert initial state
        Assert.Equal(3f, graphics.ScreenWidth);
        Assert.Equal(4f, graphics.ScreenHeight);
        Assert.Equal(2f, graphics.Scale);
    }

    [Fact]
    public void DrawImageCentrePlacesImageAtCalculatedX()
    {
        // Arrange: use known 2x2 image
        Mock<IAssetLocator> moqAssetLocator = ArrangeAssets("2x2redtopleft.bmp");
        using SoftwareGraphics graphics = SoftwareGraphics.Create(4, 4, DoAssert, moqAssetLocator.Object);

        // Act: centre X should be (4 - 2) / 2 = 1
        graphics.DrawImageCentre(123, 1);
        graphics.ScreenUpdate();

        // Assert
        static void DoAssert(FastBitmap bmp) => Assert.Equal(BaseColors.Red.Argb, bmp.GetPixel(1, 1));
    }

    [Fact]
    public void DrawPolygonDrawsEdges()
    {
        // Arrange
        Mock<IAssetLocator> moqAssetLocator = ArrangeAssets();
        using SoftwareGraphics graphics = SoftwareGraphics.Create(5, 5, DoAssert, moqAssetLocator.Object);

        Vector2[] square = [new(0, 0), new(4, 0), new(4, 4), new(0, 4)];

        // Act
        graphics.DrawPolygon(square, BaseColors.White.Argb);
        graphics.ScreenUpdate();

        // Assert: corners drawn
        static void DoAssert(FastBitmap bmp)
        {
            Assert.Equal(BaseColors.White.Argb, bmp.GetPixel(0, 0));
            Assert.Equal(BaseColors.White.Argb, bmp.GetPixel(4, 0));
            Assert.Equal(BaseColors.White.Argb, bmp.GetPixel(4, 4));
            Assert.Equal(BaseColors.White.Argb, bmp.GetPixel(0, 4));
        }
    }

    [Fact]
    public void DrawPolygonFilledFillsTriangle()
    {
        // Arrange
        Mock<IAssetLocator> moqAssetLocator = ArrangeAssets();
        using SoftwareGraphics graphics = SoftwareGraphics.Create(5, 5, DoAssert, moqAssetLocator.Object);

        Vector2[] tri = [new(1, 1), new(3, 1), new(2, 3)];

        // Act
        graphics.DrawPolygonFilled(tri, BaseColors.White.Argb);
        graphics.ScreenUpdate();

        // Assert: interior pixel set
        static void DoAssert(FastBitmap bmp) => Assert.Equal(BaseColors.White.Argb, bmp.GetPixel(2, 2));
    }

    [Fact]
    public void DrawRectangleBehaviour()
    {
        // Arrange
        Mock<IAssetLocator> moqAssetLocator = ArrangeAssets();
        using SoftwareGraphics graphics = SoftwareGraphics.Create(6, 6, DoAssert, moqAssetLocator.Object);

        // Act - outline
        graphics.DrawRectangle(new(1, 1), 3, 3, BaseColors.White.Argb);
        graphics.ScreenUpdate();

        // Assert outline
        static void DoAssert(FastBitmap bmp)
        {
            Assert.Equal(BaseColors.Black.Argb, bmp.GetPixel(1, 0));
            Assert.Equal(BaseColors.Black.Argb, bmp.GetPixel(3, 0));
            Assert.Equal(BaseColors.Black.Argb, bmp.GetPixel(0, 1));
            Assert.Equal(BaseColors.White.Argb, bmp.GetPixel(1, 1));
            Assert.Equal(BaseColors.White.Argb, bmp.GetPixel(2, 1));
            Assert.Equal(BaseColors.White.Argb, bmp.GetPixel(3, 1));
            Assert.Equal(BaseColors.Black.Argb, bmp.GetPixel(4, 1));
            Assert.Equal(BaseColors.White.Argb, bmp.GetPixel(1, 2));
            Assert.Equal(BaseColors.Black.Argb, bmp.GetPixel(2, 2));
            Assert.Equal(BaseColors.White.Argb, bmp.GetPixel(3, 2));
            Assert.Equal(BaseColors.Black.Argb, bmp.GetPixel(0, 3));
            Assert.Equal(BaseColors.White.Argb, bmp.GetPixel(1, 3));
            Assert.Equal(BaseColors.White.Argb, bmp.GetPixel(2, 3));
            Assert.Equal(BaseColors.White.Argb, bmp.GetPixel(3, 3));
            Assert.Equal(BaseColors.Black.Argb, bmp.GetPixel(4, 3));
            Assert.Equal(BaseColors.Black.Argb, bmp.GetPixel(1, 4));
            Assert.Equal(BaseColors.Black.Argb, bmp.GetPixel(3, 4));
        }
    }

    [Fact]
    public void DrawRectangleFilledBehaviour()
    {
        // Arrange
        Mock<IAssetLocator> moqAssetLocator = ArrangeAssets();
        using SoftwareGraphics graphics = SoftwareGraphics.Create(6, 6, DoAssert, moqAssetLocator.Object);

        // Act - filled
        using SoftwareGraphics graphics2 = SoftwareGraphics.Create(6, 6, DoAssert, moqAssetLocator.Object);
        graphics2.DrawRectangleFilled(new(1, 1), 3, 3, BaseColors.White.Argb);
        graphics2.ScreenUpdate();

        // Assert - interior should be white
        static void DoAssert(FastBitmap bmp)
        {
            Assert.Equal(BaseColors.Black.Argb, bmp.GetPixel(1, 0));
            Assert.Equal(BaseColors.Black.Argb, bmp.GetPixel(3, 0));
            Assert.Equal(BaseColors.Black.Argb, bmp.GetPixel(0, 1));
            Assert.Equal(BaseColors.White.Argb, bmp.GetPixel(1, 1));
            Assert.Equal(BaseColors.White.Argb, bmp.GetPixel(2, 1));
            Assert.Equal(BaseColors.White.Argb, bmp.GetPixel(3, 1));
            Assert.Equal(BaseColors.Black.Argb, bmp.GetPixel(4, 1));
            Assert.Equal(BaseColors.White.Argb, bmp.GetPixel(1, 2));
            Assert.Equal(BaseColors.White.Argb, bmp.GetPixel(2, 2));
            Assert.Equal(BaseColors.White.Argb, bmp.GetPixel(3, 2));
            Assert.Equal(BaseColors.Black.Argb, bmp.GetPixel(0, 3));
            Assert.Equal(BaseColors.White.Argb, bmp.GetPixel(1, 3));
            Assert.Equal(BaseColors.White.Argb, bmp.GetPixel(2, 3));
            Assert.Equal(BaseColors.White.Argb, bmp.GetPixel(3, 3));
            Assert.Equal(BaseColors.Black.Argb, bmp.GetPixel(4, 3));
            Assert.Equal(BaseColors.Black.Argb, bmp.GetPixel(1, 4));
            Assert.Equal(BaseColors.Black.Argb, bmp.GetPixel(3, 4));
        }
    }

    [Fact]
    public void DrawTextWhitespaceNoChange()
    {
        // Arrange
        Mock<IAssetLocator> moqAssetLocator = ArrangeAssets();
        using SoftwareGraphics graphics = SoftwareGraphics.Create(5, 5, DoAssert, moqAssetLocator.Object);

        // Preserve a pixel
        graphics.DrawPixel(new(2, 2), BaseColors.White.Argb);

        // Act - whitespace should do nothing
        graphics.DrawTextLeft(new(0, 0), "   ", 0, BaseColors.White.Argb);
        graphics.DrawTextCentre(0, " ", 0, BaseColors.White.Argb);
        graphics.DrawTextRight(new(0, 0), Environment.NewLine, 0, BaseColors.White.Argb);
        graphics.ScreenUpdate();

        // Assert preserved
        static void DoAssert(FastBitmap bmp) => Assert.Equal(BaseColors.White.Argb, bmp.GetPixel(2, 2));
    }

    [Fact]
    public void DrawTriangleNoThrowAndDraws()
    {
        // Arrange
        Mock<IAssetLocator> moqAssetLocator = ArrangeAssets();
        using SoftwareGraphics graphics = SoftwareGraphics.Create(5, 5, DoAssert, moqAssetLocator.Object);

        Vector2 a = new(1, 1);
        Vector2 b = new(3, 1);
        Vector2 c = new(2, 3);

        // Act - outline
        graphics.DrawTriangle(a, b, c, BaseColors.White.Argb);
        graphics.ScreenUpdate();

        // Assert some outline points exist
        static void DoAssert(FastBitmap bmp)
        {
            Assert.Equal(BaseColors.White.Argb, bmp.GetPixel(1, 1));
            Assert.Equal(BaseColors.White.Argb, bmp.GetPixel(3, 1));
        }
    }

    [Fact]
    public void DrawTriangleFilledNoThrowAndDraws()
    {
        // Arrange
        Mock<IAssetLocator> moqAssetLocator = ArrangeAssets();
        using SoftwareGraphics graphics = SoftwareGraphics.Create(5, 5, DoAssert, moqAssetLocator.Object);

        Vector2 a = new(1, 1);
        Vector2 b = new(3, 1);
        Vector2 c = new(2, 3);

        // Act - filled
        graphics.DrawTriangleFilled(a, b, c, BaseColors.White.Argb);
        graphics.ScreenUpdate();

        static void DoAssert(FastBitmap bmp) => Assert.Equal(BaseColors.White.Argb, bmp.GetPixel(2, 2));
    }

    [Fact]
    public void SetClipRegionNoThrow()
    {
        // Arrange
        Mock<IAssetLocator> moqAssetLocator = ArrangeAssets();
        using SoftwareGraphics graphics = SoftwareGraphics.Create(5, 5, (_) => { }, moqAssetLocator.Object);

        // Act / Assert - should not throw
        graphics.SetClipRegion(new Vector2(1, 1), 2, 2);
    }

    [Fact]
    public void DisposeCanBeCalledMultipleTimes()
    {
        // Arrange
        Mock<IAssetLocator> moqAssetLocator = ArrangeAssets();
        SoftwareGraphics graphics = SoftwareGraphics.Create(5, 5, (_) => { }, moqAssetLocator.Object);

        // Act
        graphics.Dispose();
        graphics.Dispose(); // second call should be safe

        // Assert - no exception thrown (implicit)
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
