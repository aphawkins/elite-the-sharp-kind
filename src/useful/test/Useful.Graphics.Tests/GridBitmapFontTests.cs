// 'Useful Libraries' - Andy Hawkins 2023-2026.

using Moq;
using Useful.Assets;

namespace Useful.Graphics.Tests;

// The monospaced sheet shape the 8-bit BBC Micro font uses: no magenta
// markers, every glyph filling its cell, ink recoloured and the sheet's
// black background treated as transparent.
public class GridBitmapFontTests
{
    [Fact]
    public void DrawsAMonospacedGlyphRecolouredWithTheBackgroundTransparent()
    {
        // Arrange: a 1-column sheet of 2x2 cells. Cell 0 is space (all
        // background); cell 1 is '!' with ink down its left column.
        uint black = BaseColors.Black.Argb;
        uint white = BaseColors.White.Argb;
        uint[] cells =
        [
            black, black,
            black, black,
            white, black,
            white, black,
        ];
        using TempImageFile sheet = TempImageFile.From(Sheet(cells));

        uint[] frame = [];
        using SoftwareGraphics graphics = SoftwareGraphics.Create(
            4,
            4,
            b => frame = Capture(b),
            Locator(sheet.Path));

        // Act
        graphics.DrawTextLeft(new(0, 0), "!", "TestFont", BaseColors.Red.Argb);
        graphics.ScreenUpdate();

        // Assert - ink took the requested colour, background stayed clear.
        Assert.Equal(BaseColors.Red.Argb, frame[0]);
        Assert.Equal(BaseColors.Red.Argb, frame[4]);
        Assert.Equal(BaseColors.Black.Argb, frame[1]);
    }

    [Fact]
    public void AdvancesByTheCellWidthForEveryGlyph()
    {
        // Arrange: both glyphs are fully inked, so a monospaced advance puts
        // the second one exactly one cell to the right.
        uint white = BaseColors.White.Argb;
        uint[] cells =
        [
            white, white,
            white, white,
            white, white,
            white, white,
        ];
        using TempImageFile sheet = TempImageFile.From(Sheet(cells));

        uint[] frame = [];
        using SoftwareGraphics graphics = SoftwareGraphics.Create(
            4,
            4,
            b => frame = Capture(b),
            Locator(sheet.Path));

        // Act - space then '!', both inked, so all four columns are covered.
        graphics.DrawTextLeft(new(0, 0), " !", "TestFont", BaseColors.Red.Argb);
        graphics.ScreenUpdate();

        // Assert
        Assert.Equal(BaseColors.Red.Argb, frame[0]);
        Assert.Equal(BaseColors.Red.Argb, frame[1]);
        Assert.Equal(BaseColors.Red.Argb, frame[2]);
        Assert.Equal(BaseColors.Red.Argb, frame[3]);
    }

    private static uint[] Capture(FastBitmap bitmap)
    {
        uint[] pixels = new uint[bitmap.Width * bitmap.Height];

        for (int y = 0; y < bitmap.Height; y++)
        {
            for (int x = 0; x < bitmap.Width; x++)
            {
                pixels[x + (y * bitmap.Width)] = bitmap.GetPixel(x, y);
            }
        }

        return pixels;
    }

    // Takes rows top-down, the way the sheet reads on screen, and flips them
    // into the bottom-up order a BMP stores.
    private static byte[] Sheet(uint[] pixels)
    {
        const int width = 2;
        int height = pixels.Length / width;
        byte[] data = new byte[pixels.Length * 4];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                uint colour = pixels[x + ((height - 1 - y) * width)];
                int i = x + (y * width);
                data[i * 4] = (byte)(colour & 0xFF);
                data[(i * 4) + 1] = (byte)((colour >> 8) & 0xFF);
                data[(i * 4) + 2] = (byte)((colour >> 16) & 0xFF);
                data[(i * 4) + 3] = (byte)(colour >> 24);
            }
        }

        return BmpBuilder.Build(width, height, 32, data);
    }

    private static IAssetLocator Locator(string sheetPath)
    {
        Mock<IAssetLocator> locator = new();
        locator.SetupGet(x => x.Tier).Returns(SystemTier.EightBit);
        locator.SetupGet(x => x.ImagePaths).Returns(new Dictionary<string, string>());
        BitmapFontEntry entry = new()
        {
            File = sheetPath,
            CellWidth = 2,
            CellHeight = 2,
            Columns = 1,
        };

        Dictionary<string, BitmapFontAsset> fonts = new() { { "TestFont", new(sheetPath, entry) } };
        locator.SetupGet(x => x.FontBitmaps).Returns(fonts);

        return locator.Object;
    }
}
