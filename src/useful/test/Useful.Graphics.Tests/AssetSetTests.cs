// 'Useful Libraries' - Andy Hawkins 2023-2026.

using Moq;
using Useful.Assets;

namespace Useful.Graphics.Tests;

public class AssetSetTests
{
    [Fact]
    public void CountsDistinctOpaqueColoursAcrossTheWholeSet()
    {
        // Arrange: two images sharing one colour, so the union is 3 rather
        // than the 4 a per-image count would give.
        using TempImageFile red = TempImageFile.From(Bmp(0xFFFF0000, 0xFF00FF00));
        using TempImageFile blue = TempImageFile.From(Bmp(0xFF0000FF, 0xFF00FF00));

        // Act
        AssetSet assets = AssetSet.Load(Locator(SystemTier.SixteenBit, ("Red", red), ("Blue", blue)));

        // Assert
        Assert.Equal(3, assets.Budget.ColourCount);
        Assert.Equal(2, assets.Budget.PerAsset["Red"]);
        Assert.Equal(2, assets.Budget.PerAsset["Blue"]);
    }

    [Fact]
    public void ExcludesFullyTransparentPixelsFromTheCount()
    {
        // Arrange: one opaque colour and one transparent pixel.
        using TempImageFile image = TempImageFile.From(Bmp(0xFFFF0000, 0x00123456));

        // Act
        AssetSet assets = AssetSet.Load(Locator(SystemTier.SixteenBit, ("Image", image)));

        // Assert
        Assert.Equal(1, assets.Budget.ColourCount);
    }

    [Fact]
    public void CountsBitmapFontsAgainstTheBudgetToo()
    {
        // Arrange: the font is part of the tier's set even for backends that
        // draw text with TrueType fonts instead.
        using TempImageFile image = TempImageFile.From(Bmp(0xFFFF0000, 0xFFFF0000));
        using TempImageFile font = TempImageFile.From(FontBmp(0xFF00FF00, 0xFF0000FF));

        // Act
        AssetSet assets = AssetSet.Load(Locator(SystemTier.SixteenBit, [("Image", image)], [("Small", font)]));

        // Assert
        Assert.Equal(3, assets.Budget.ColourCount);
        Assert.Equal(2, assets.Budget.PerAsset["Small"]);
    }

    [Fact]
    public void FlagsPixelsWhoseAlphaIsNeitherFullyOpaqueNorFullyTransparent()
    {
        // Arrange
        using TempImageFile image = TempImageFile.From(Bmp(0x80FF0000, 0xFF00FF00));

        // Act
        AssetSet assets = AssetSet.Load(Locator(SystemTier.SixteenBit, ("Image", image)));

        // Assert
        Assert.Equal(1, assets.Budget.PartialAlphaCount);
    }

    [Theory]
    [InlineData(SystemTier.EightBit, 16)]
    [InlineData(SystemTier.SixteenBit, 4096)]
    public void CapsEachTierAtItsOwnColourBudget(SystemTier tier, int expectedCap)
        => Assert.Equal(expectedCap, AssetColourBudget.MaxColours(tier));

    [Fact]
    public void ReportsAnEightBitSetOfSeventeenColoursAsOverBudget()
    {
        // Arrange: 17 distinct colours against the 8-bit cap of 16.
        uint[] colours = [.. Enumerable.Range(0, 17).Select(i => 0xFF000000u | (uint)i)];
        using TempImageFile image = TempImageFile.From(Bmp(colours));

        // Act
        AssetSet assets = AssetSet.Load(Locator(SystemTier.EightBit, ("Image", image)));

        // Assert
        Assert.Equal(17, assets.Budget.ColourCount);
        Assert.Equal(16, assets.Budget.Cap);
        Assert.False(assets.Budget.IsWithinBudget);
    }

    [Fact]
    public void ReportsAnEightBitSetOfSixteenColoursAsWithinBudget()
    {
        // Arrange: exactly at the cap, which must pass.
        uint[] colours = [.. Enumerable.Range(0, 16).Select(i => 0xFF000000u | (uint)i)];
        using TempImageFile image = TempImageFile.From(Bmp(colours));

        // Act
        AssetSet assets = AssetSet.Load(Locator(SystemTier.EightBit, ("Image", image)));

        // Assert
        Assert.True(assets.Budget.IsWithinBudget);
    }

    [Fact]
    public void LoadsAnOverBudgetSetAnywayForNow()
    {
        // Arrange: the validator is warn-only until the committed 16-bit
        // assets are posterised, so an over-budget set must still load.
        uint[] colours = [.. Enumerable.Range(0, 17).Select(i => 0xFF000000u | (uint)i)];
        using TempImageFile image = TempImageFile.From(Bmp(colours));

        // Act
        AssetSet assets = AssetSet.Load(Locator(SystemTier.EightBit, ("Image", image)));

        // Assert
        Assert.Single(assets.Images);
        Assert.Equal(17, assets.Images["Image"].Width);
    }

    private static byte[] Bmp(params uint[] colours)
    {
        byte[] pixels = new byte[colours.Length * 4];

        for (int i = 0; i < colours.Length; i++)
        {
            pixels[i * 4] = (byte)(colours[i] & 0xFF);
            pixels[(i * 4) + 1] = (byte)((colours[i] >> 8) & 0xFF);
            pixels[(i * 4) + 2] = (byte)((colours[i] >> 16) & 0xFF);
            pixels[(i * 4) + 3] = (byte)(colours[i] >> 24);
        }

        return BmpBuilder.Build(colours.Length, 1, 32, pixels);
    }

    // BitmapFont only accepts the 513x193 sheet the real fonts use, so a
    // font fixture has to be that size whatever colours it carries.
    private static byte[] FontBmp(uint first, uint rest)
    {
        const int width = 513;
        const int height = 193;
        byte[] pixels = new byte[width * height * 4];

        for (int i = 0; i < width * height; i++)
        {
            uint colour = i == 0 ? first : rest;
            pixels[i * 4] = (byte)(colour & 0xFF);
            pixels[(i * 4) + 1] = (byte)((colour >> 8) & 0xFF);
            pixels[(i * 4) + 2] = (byte)((colour >> 16) & 0xFF);
            pixels[(i * 4) + 3] = (byte)(colour >> 24);
        }

        return BmpBuilder.Build(width, height, 32, pixels);
    }

    private static IAssetLocator Locator(SystemTier tier, params (string Name, TempImageFile File)[] images)
        => Locator(tier, images, []);

    private static IAssetLocator Locator(
        SystemTier tier,
        (string Name, TempImageFile File)[] images,
        (string Name, TempImageFile File)[] fonts)
    {
        Mock<IAssetLocator> locator = new();
        locator.SetupGet(x => x.Tier).Returns(tier);
        locator.SetupGet(x => x.ImagePaths).Returns(images.ToDictionary(x => x.Name, x => x.File.Path));
        locator.SetupGet(x => x.FontBitmapPaths).Returns(fonts.ToDictionary(x => x.Name, x => x.File.Path));
        return locator.Object;
    }
}
