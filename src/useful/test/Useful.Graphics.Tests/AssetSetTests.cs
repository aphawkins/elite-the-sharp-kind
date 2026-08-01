// 'Useful Libraries' - Andy Hawkins 2023-2026.

using System.Globalization;
using System.Text;
using System.Text.Json;
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
    public void ThrowsOnPixelsWhoseAlphaIsNeitherFullyOpaqueNorFullyTransparent()
    {
        // Arrange: the renderer has no middle ground for alpha, so a
        // half-transparent pixel is an authoring mistake, not a style.
        using TempImageFile image = TempImageFile.From(Bmp(0x80FF0000, 0xFF00FF00));
        IAssetLocator locator = Locator(SystemTier.SixteenBit, ("Image", image));

        // Act / Assert
        Assert.Throws<UsefulException>(() => AssetSet.Load(locator));
    }

    [Theory]
    [InlineData(SystemTier.EightBit, 16)]
    [InlineData(SystemTier.SixteenBit, 4096)]
    public void CapsEachTierAtItsOwnColourBudget(SystemTier tier, int expectedCap)
        => Assert.Equal(expectedCap, AssetColourBudget.MaxColours(tier));

    [Fact]
    public void ThrowsWhenAnEightBitSetExceedsSixteenColours()
    {
        // Arrange: 17 distinct colours against the 8-bit cap of 16.
        uint[] colours = [.. Enumerable.Range(0, 17).Select(i => 0xFF000000u | (uint)i)];
        using TempImageFile image = TempImageFile.From(Bmp(colours));
        IAssetLocator locator = Locator(SystemTier.EightBit, ("Image", image));

        // Act / Assert
        Assert.Throws<UsefulException>(() => AssetSet.Load(locator));
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
    public void AllowsTheSameSetUnderTheRoomierSixteenBitCap()
    {
        // Arrange: 17 colours, which break the 8-bit cap but are fine here, so
        // the failure above is the cap doing its job rather than the loader
        // rejecting the file. They sit on the 16-bit grid, because that tier
        // enforces channel depth as well as the cap.
        using TempImageFile image = TempImageFile.From(Bmp(OnGrid(17)));

        // Act
        AssetSet assets = AssetSet.Load(Locator(SystemTier.SixteenBit, ("Image", image)));

        // Assert
        Assert.Single(assets.Images);
        Assert.Equal(17, assets.Budget.ColourCount);
    }

    [Theory]
    [InlineData(SystemTier.EightBit, true)]
    [InlineData(SystemTier.SixteenBit, false)]
    public void TreatsThePaletteAsTheWholeColourSetOnlyOnIndexedColourTiers(SystemTier tier, bool expected)
        => Assert.Equal(expected, AssetColourBudget.PaletteNamesEveryColour(tier));

    [Fact]
    public void ThrowsWhenAnEightBitBitmapUsesAColourThePaletteDoesNotName()
    {
        // Arrange: 8-bit hardware was indexed-colour, so a pixel that is not a
        // palette entry is a colour the machine could not have shown - well
        // inside the 16-colour cap, so only the palette rule can catch it.
        using TempImageFile palette = Palette(("Red", 0xFFFF0000));
        using TempImageFile image = TempImageFile.From(Bmp(0xFFFF0000, 0xFF00FF00));
        IAssetLocator locator = Locator(SystemTier.EightBit, palette, ("Image", image));

        // Act
        UsefulException exception = Assert.Throws<UsefulException>(() => AssetSet.Load(locator));

        // Assert: the message has to name the file and the colour, or the fix
        // is a hunt through every bitmap in the tier.
        Assert.Contains("Image", exception.Message, StringComparison.Ordinal);
        Assert.Contains("FF00FF00", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void AllowsAnEightBitSetWhoseColoursThePaletteAllNames()
    {
        // Arrange: every pixel is a palette entry, and the palette names one
        // colour no bitmap uses - the rule is a subset test, not equality.
        using TempImageFile palette = Palette(("Red", 0xFFFF0000), ("Green", 0xFF00FF00), ("Blue", 0xFF0000FF));
        using TempImageFile image = TempImageFile.From(Bmp(0xFFFF0000, 0xFF00FF00));

        // Act
        AssetSet assets = AssetSet.Load(Locator(SystemTier.EightBit, palette, ("Image", image)));

        // Assert
        Assert.True(assets.Budget.IsWithinPalette);
        Assert.Empty(assets.Budget.OutsidePalette);
    }

    [Fact]
    public void IgnoresTransparentPixelsWhenCheckingAgainstThePalette()
    {
        // Arrange: a transparent pixel carries no colour, so the palette has
        // no reason to name whatever ARGB it happens to hold.
        using TempImageFile palette = Palette(("Red", 0xFFFF0000));
        using TempImageFile image = TempImageFile.From(Bmp(0xFFFF0000, 0x00123456));

        // Act
        AssetSet assets = AssetSet.Load(Locator(SystemTier.EightBit, palette, ("Image", image)));

        // Assert
        Assert.True(assets.Budget.IsWithinPalette);
    }

    [Fact]
    public void ReportsButAllowsColoursOutsideTheSixteenBitPalette()
    {
        // Arrange: 16-bit hardware is direct-colour, so its palette is only a
        // set of names the geometry draws with - bitmaps are free of it. The
        // same set that fails on 8-bit has to load here, and the breakdown is
        // still recorded so it can be logged.
        using TempImageFile palette = Palette(("Red", 0xFFFF0000));
        using TempImageFile image = TempImageFile.From(Bmp(0xFFFF0000, 0xFF00FF00));

        // Act
        AssetSet assets = AssetSet.Load(Locator(SystemTier.SixteenBit, palette, ("Image", image)));

        // Assert
        Assert.True(assets.Budget.IsWithinPalette);
        Assert.Equal([0xFF00FF00], assets.Budget.OutsidePalette["Image"]);
    }

    [Fact]
    public void ChecksBitmapFontsAgainstThePaletteToo()
    {
        // Arrange: the font is part of the tier's set, so its colours have to
        // be palette colours like any other asset's.
        using TempImageFile palette = Palette(("Red", 0xFFFF0000));
        using TempImageFile image = TempImageFile.From(Bmp(0xFFFF0000, 0xFFFF0000));
        using TempImageFile font = TempImageFile.From(FontBmp(0xFFFF0000, 0xFF00FF00));
        IAssetLocator locator = Locator(SystemTier.EightBit, palette, [("Image", image)], [("Small", font)]);

        // Act
        UsefulException exception = Assert.Throws<UsefulException>(() => AssetSet.Load(locator));

        // Assert
        Assert.Contains("Small", exception.Message, StringComparison.Ordinal);
    }

    [Theory]
    [InlineData(SystemTier.EightBit, 8)]
    [InlineData(SystemTier.SixteenBit, 4)]
    public void DrivesEachTierAtItsOwnChannelDepth(SystemTier tier, int expectedBits)
        => Assert.Equal(expectedBits, AssetColourBudget.ChannelBits(tier));

    [Theory]
    [InlineData(0xFF000000u, true)]
    [InlineData(0xFFFFFFFFu, true)]
    [InlineData(0xFF112233u, true)]
    [InlineData(0xFFEE6622u, true)]
    [InlineData(0xFF808080u, false)] // the midpoint no 4-bit channel can hold
    [InlineData(0xFFF0F0F0u, false)] // a left shift rather than a replication
    [InlineData(0xFF11FF34u, false)] // one stray channel is enough
    [InlineData(0x80112233u, true)] // alpha is not a channel of the DAC
    public void PlacesAColourOnTheTwelveBitGridOnlyWhenEveryChannelIsALevel(uint argb, bool expected)
        => Assert.Equal(expected, AssetColourBudget.IsOnGrid(argb, 4));

    [Fact]
    public void ThrowsWhenASixteenBitColourSitsBetweenTheTiersLevels()
    {
        // Arrange: 0x808080 is the classic mid grey, and the one no 12-bit DAC
        // can produce - 0x77 and 0x88 are the levels either side of it.
        using TempImageFile image = TempImageFile.From(Bmp(0xFF112233, 0xFF808080));
        IAssetLocator locator = Locator(SystemTier.SixteenBit, ("Image", image));

        // Act
        UsefulException exception = Assert.Throws<UsefulException>(() => AssetSet.Load(locator));

        // Assert: naming the file and the colour is what makes the failure
        // actionable rather than a hunt through the tier's art.
        Assert.Contains("Image", exception.Message, StringComparison.Ordinal);
        Assert.Contains("FF808080", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void AllowsASixteenBitSetWhoseColoursAllSitOnTheGrid()
    {
        // Arrange
        using TempImageFile image = TempImageFile.From(Bmp(0xFF112233, 0xFFEE6622));

        // Act
        AssetSet assets = AssetSet.Load(Locator(SystemTier.SixteenBit, ("Image", image)));

        // Assert
        Assert.True(assets.Budget.IsOnColourGrid);
        Assert.Empty(assets.Budget.OffGrid);
    }

    [Fact]
    public void ChecksTheNamedPaletteAgainstTheGridAsWellAsTheBitmaps()
    {
        // Arrange: the geometry draws with palette names, so a colour the tier
        // cannot show reaches the screen whether it arrives as a pixel or a
        // name. The bitmap here is clean, so only the palette can be at fault.
        using TempImageFile palette = Palette(("Grey", 0xFF808080));
        using TempImageFile image = TempImageFile.From(Bmp(0xFF112233, 0xFF112233));
        IAssetLocator locator = Locator(SystemTier.SixteenBit, palette, ("Image", image));

        // Act
        UsefulException exception = Assert.Throws<UsefulException>(() => AssetSet.Load(locator));

        // Assert
        Assert.Contains("Palette", exception.Message, StringComparison.Ordinal);
        Assert.Contains("FF808080", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void LeavesTheEightBitTierFreeOfTheChannelDepthRule()
    {
        // Arrange: the same colour that fails on 16-bit, on the tier whose
        // limit is its 16-entry palette rather than its channel depth.
        using TempImageFile image = TempImageFile.From(Bmp(0xFF808080, 0xFF808080));

        // Act
        AssetSet assets = AssetSet.Load(Locator(SystemTier.EightBit, ("Image", image)));

        // Assert
        Assert.True(assets.Budget.IsOnColourGrid);
    }

    [Fact]
    public void ChecksBitmapFontsAgainstTheGridToo()
    {
        // Arrange: the font is part of the tier's set, so its channels have to
        // land on the tier's levels like any other asset's.
        using TempImageFile image = TempImageFile.From(Bmp(0xFF112233, 0xFF112233));
        using TempImageFile font = TempImageFile.From(FontBmp(0xFF112233, 0xFF808080));
        IAssetLocator locator = Locator(SystemTier.SixteenBit, [("Image", image)], [("Small", font)]);

        // Act
        UsefulException exception = Assert.Throws<UsefulException>(() => AssetSet.Load(locator));

        // Assert
        Assert.Contains("Small", exception.Message, StringComparison.Ordinal);
    }

    // Distinct colours that all sit on the 12-bit grid. One channel only
    // carries 16 of them, so the count spills into the next.
    private static uint[] OnGrid(int count)
        => [.. Enumerable.Range(0, count).Select(i => 0xFF000000u | ((uint)(i % 16) * 0x11u) | ((uint)(i / 16) * 0x1100u))];

    private static TempImageFile Palette(params (string Name, uint Argb)[] colours)
        => TempImageFile.From(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(
            colours.ToDictionary(x => x.Name, x => x.Argb.ToString("X8", CultureInfo.InvariantCulture)))));

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
        => Locator(tier, null, images, []);

    private static IAssetLocator Locator(
        SystemTier tier,
        TempImageFile palette,
        params (string Name, TempImageFile File)[] images)
        => Locator(tier, palette, images, []);

    private static IAssetLocator Locator(
        SystemTier tier,
        (string Name, TempImageFile File)[] images,
        (string Name, TempImageFile File)[] fonts)
        => Locator(tier, null, images, fonts);

    private static IAssetLocator Locator(
        SystemTier tier,
        TempImageFile? palette,
        (string Name, TempImageFile File)[] images,
        (string Name, TempImageFile File)[] fonts)
    {
        Mock<IAssetLocator> locator = new();
        locator.SetupGet(x => x.Tier).Returns(tier);
        locator.SetupGet(x => x.PalettePath).Returns(palette?.Path ?? string.Empty);
        locator.SetupGet(x => x.ImagePaths).Returns(images.ToDictionary(x => x.Name, x => x.File.Path));
        locator.SetupGet(x => x.FontBitmaps).Returns(fonts.ToDictionary(
            x => x.Name,
            x => new BitmapFontAsset(
                x.File.Path,
                new BitmapFontEntry { File = x.File.Path, CellWidth = 32, CellHeight = 32, Columns = 16 })));
        return locator.Object;
    }
}
