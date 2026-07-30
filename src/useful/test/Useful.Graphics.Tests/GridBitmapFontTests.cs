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
        FastColor black = BaseColors.Black;
        FastColor white = BaseColors.White;
        FastColor[] cells =
        [
            black, black,
            black, black,
            white, black,
            white, black,
        ];
        using TempImageFile sheet = TempImageFile.From(Sheet(cells));

        FastColor[] frame = [];
        using SoftwareGraphics graphics = SoftwareGraphics.Create(
            4,
            4,
            b => frame = Capture(b),
            Locator(sheet.Path));

        // Act
        graphics.DrawTextLeft(new(0, 0), "!", "TestFont", BaseColors.Red);
        graphics.ScreenUpdate();

        // Assert - ink took the requested colour, background stayed clear.
        Assert.Equal(BaseColors.Red, frame[0]);
        Assert.Equal(BaseColors.Red, frame[4]);
        Assert.Equal(BaseColors.Black, frame[1]);
    }

    [Fact]
    public void AdvancesByTheCellWidthForEveryGlyph()
    {
        // Arrange: both glyphs are fully inked, so a monospaced advance puts
        // the second one exactly one cell to the right.
        FastColor white = BaseColors.White;
        FastColor[] cells =
        [
            white, white,
            white, white,
            white, white,
            white, white,
        ];
        using TempImageFile sheet = TempImageFile.From(Sheet(cells));

        FastColor[] frame = [];
        using SoftwareGraphics graphics = SoftwareGraphics.Create(
            4,
            4,
            b => frame = Capture(b),
            Locator(sheet.Path));

        // Act - space then '!', both inked, so all four columns are covered.
        graphics.DrawTextLeft(new(0, 0), " !", "TestFont", BaseColors.Red);
        graphics.ScreenUpdate();

        // Assert
        Assert.Equal(BaseColors.Red, frame[0]);
        Assert.Equal(BaseColors.Red, frame[1]);
        Assert.Equal(BaseColors.Red, frame[2]);
        Assert.Equal(BaseColors.Red, frame[3]);
    }

    private static FastColor[] Capture(FastBitmap bitmap)
    {
        FastColor[] pixels = new FastColor[bitmap.Width * bitmap.Height];

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
    private static byte[] Sheet(FastColor[] pixels)
    {
        const int width = 2;
        int height = pixels.Length / width;
        byte[] data = new byte[pixels.Length * 4];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                FastColor colour = pixels[x + ((height - 1 - y) * width)];
                int i = x + (y * width);
                data[i * 4] = colour.B;
                data[(i * 4) + 1] = colour.G;
                data[(i * 4) + 2] = colour.R;
                data[(i * 4) + 3] = colour.A;
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
