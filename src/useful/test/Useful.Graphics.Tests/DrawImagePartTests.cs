// 'Useful Libraries' - Andy Hawkins 2025.

using Moq;
using Useful.Assets;

namespace Useful.Graphics.Tests;

public class DrawImagePartTests
{
    // golden/2x2redtopleft.bmp: red at (0,0), transparent elsewhere.
    private const string TestImage = "Test";

    [Fact]
    public void DrawsScaledSourceRectangle()
    {
        using SoftwareGraphics graphics = CreateGraphics(DoAssert);

        // scale the red source pixel up to a 10x10 block at (20,20)
        graphics.DrawImagePart(TestImage, new(20, 20), new(10, 10), new(0, 0), new(1, 1));
        graphics.ScreenUpdate();

        static void DoAssert(FastBitmap bmp)
        {
            for (int y = 20; y < 30; y++)
            {
                for (int x = 20; x < 30; x++)
                {
                    Assert.Equal(BaseColors.Red.Argb, bmp.GetPixel(x, y));
                }
            }

            // outside the destination stays untouched
            Assert.Equal(BaseColors.Black.Argb, bmp.GetPixel(31, 20));
            Assert.Equal(BaseColors.Black.Argb, bmp.GetPixel(20, 31));
        }
    }

    [Fact]
    public void SkipsTransparentPixels()
    {
        using SoftwareGraphics graphics = CreateGraphics(DoAssert);

        // the whole 2x2 image: only the top-left source quadrant is opaque
        graphics.DrawImagePart(TestImage, new(10, 10), new(20, 20), new(0, 0), new(2, 2));
        graphics.ScreenUpdate();

        static void DoAssert(FastBitmap bmp)
        {
            Assert.Equal(BaseColors.Red.Argb, bmp.GetPixel(12, 12));

            // the transparent quadrants leave the background alone
            Assert.Equal(BaseColors.Black.Argb, bmp.GetPixel(28, 12));
            Assert.Equal(BaseColors.Black.Argb, bmp.GetPixel(12, 28));
            Assert.Equal(BaseColors.Black.Argb, bmp.GetPixel(28, 28));
        }
    }

    [Fact]
    public void NegativeSourceWidthMirrorsHorizontally()
    {
        using SoftwareGraphics graphics = CreateGraphics(DoAssert);

        // mirrored, the red top-left source pixel lands on the right half
        graphics.DrawImagePart(TestImage, new(10, 10), new(20, 20), new(0, 0), new(-2, 2));
        graphics.ScreenUpdate();

        static void DoAssert(FastBitmap bmp)
        {
            Assert.Equal(BaseColors.Red.Argb, bmp.GetPixel(28, 12));
            Assert.Equal(BaseColors.Black.Argb, bmp.GetPixel(12, 12));
        }
    }

    [Fact]
    public void ClipsToScreenBoundsWithoutThrowing()
    {
        using SoftwareGraphics graphics = CreateGraphics(_ => { });

        graphics.DrawImagePart(TestImage, new(-5, -5), new(20, 20), new(0, 0), new(2, 2));
        graphics.DrawImagePart(TestImage, new(95, 95), new(20, 20), new(0, 0), new(2, 2));
        graphics.DrawImagePart(TestImage, new(10, 10), new(0, 0), new(0, 0), new(2, 2));
        graphics.ScreenUpdate();
    }

    private static SoftwareGraphics CreateGraphics(Action<FastBitmap> screenUpdate)
    {
        Dictionary<string, string> images = new()
        {
            { TestImage, Path.Combine("golden", "2x2redtopleft.bmp") },
        };

        Mock<IAssetLocator> assets = new();
        assets.Setup(a => a.ImagePaths).Returns(images);
        assets.Setup(a => a.FontBitmapPaths).Returns(new Dictionary<string, string>());

        return SoftwareGraphics.Create(100, 100, screenUpdate, assets.Object);
    }
}
