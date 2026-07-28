// 'Useful Libraries' - Andy Hawkins 2023-2026.

using System.Text;
using System.Text.Json;
using Xunit;

namespace Useful.Assets.Tests;

public class AssetLocatorTierTests : IDisposable
{
    private readonly string _tempRoot;
    private readonly string _assetsRoot;
    private bool _isDisposed;

    public AssetLocatorTierTests()
    {
        _tempRoot = Path.Combine(Path.GetTempPath(), "Useful.Assets.Tests", Guid.NewGuid().ToString("N"));
        _assetsRoot = Path.Combine(_tempRoot, "Assets");
        Directory.CreateDirectory(_assetsRoot);
    }

    [Fact]
    public void PrefersTheTierFolderWhenTheAssetExistsThere()
    {
        // Arrange
        GivenAsset("Images", "SixteenBit", "logo.bmp");
        AssetLocator locator = Locate(SystemTier.SixteenBit);

        // Act
        string path = locator.ImagePaths["Logo"];

        // Assert
        Assert.Equal(Path.Combine(_assetsRoot, "Images", "SixteenBit", "logo.bmp"), path);
    }

    [Fact]
    public void FallsBackToTheSharedFolderWhenTheTierHasNoCopy()
    {
        // Arrange: the file sits directly under Images, with no tier folder,
        // which is what keeps tier-neutral assets from needing duplicating.
        GivenAsset("Images", null, "logo.bmp");
        AssetLocator locator = Locate(SystemTier.SixteenBit);

        // Act
        string path = locator.ImagePaths["Logo"];

        // Assert
        Assert.Equal(Path.Combine(_assetsRoot, "Images", "logo.bmp"), path);
    }

    [Fact]
    public void ResolvesEachTierToItsOwnCopyOfTheSameLogicalName()
    {
        // Arrange: one logical name, one filename, two tiers.
        GivenAsset("Images", "EightBit", "logo.bmp");
        GivenAsset("Images", "SixteenBit", "logo.bmp");

        // Act
        string eightBit = Locate(SystemTier.EightBit, SystemTier.EightBit, SystemTier.SixteenBit).ImagePaths["Logo"];
        string sixteenBit = Locate(SystemTier.SixteenBit, SystemTier.EightBit, SystemTier.SixteenBit).ImagePaths["Logo"];

        // Assert
        Assert.Equal(Path.Combine(_assetsRoot, "Images", "EightBit", "logo.bmp"), eightBit);
        Assert.Equal(Path.Combine(_assetsRoot, "Images", "SixteenBit", "logo.bmp"), sixteenBit);
    }

    [Fact]
    public void ResolvesBitmapFontsAndThePaletteByTierToo()
    {
        // Arrange
        GivenAsset("FontsBitmap", "SixteenBit", "font1.bmp");
        GivenAsset("Palette", "SixteenBit", "palette.json");
        AssetLocator locator = Locate(SystemTier.SixteenBit);

        // Assert
        Assert.Equal(Path.Combine(_assetsRoot, "FontsBitmap", "SixteenBit", "font1.bmp"), locator.FontBitmaps["Small"].Path);
        Assert.Equal(Path.Combine(_assetsRoot, "Palette", "SixteenBit", "palette.json"), locator.PalettePath);
    }

    [Fact]
    public void LeavesTierNeutralCategoriesOutsideTheTierFolder()
    {
        // Arrange: models, audio and TrueType fonts are resolution-independent
        // and must not gain a tier segment even when a tier folder exists.
        GivenAsset("Models", "SixteenBit", "ship.obj");
        AssetLocator locator = Locate(SystemTier.SixteenBit);

        // Act
        string path = locator.ModelPaths["Ship"];

        // Assert
        Assert.Equal(Path.Combine(_assetsRoot, "Models", "ship.obj"), path);
    }

    [Fact]
    public void AppliesATierOverrideToTheFilename()
    {
        // Arrange: the 8-bit set uses a different file for the same logical name.
        GivenAsset("Images", "EightBit", "logo-small.bmp");
        object manifest = Manifest(
            [SystemTier.EightBit],
            new Dictionary<string, object>
            {
                { "EightBit", new { Images = new Dictionary<string, string> { { "Logo", "logo-small.bmp" } } } },
            });

        // Act
        string path = Locate(SystemTier.EightBit, manifest).ImagePaths["Logo"];

        // Assert
        Assert.Equal(Path.Combine(_assetsRoot, "Images", "EightBit", "logo-small.bmp"), path);
    }

    [Fact]
    public void ThrowsWhenTheRequestedTierIsNotOneTheManifestShips()
    {
        // Arrange: asking for a tier with no assets fails at construction
        // rather than silently resolving to the shared fallback at first draw.
        object manifest = Manifest([SystemTier.SixteenBit], []);

        // Act / Assert
        Assert.Throws<UsefulException>(() => Locate(SystemTier.EightBit, manifest));
    }

    [Fact]
    public void DefaultsToTheSixteenBitTier()
    {
        // Arrange
        GivenAsset("Images", "SixteenBit", "logo.bmp");
        using MemoryStream stream = ToStream(Manifest([], []));

        // Act
        AssetLocator locator = AssetLocator.Create(stream, _tempRoot);

        // Assert
        Assert.Equal(Path.Combine(_assetsRoot, "Images", "SixteenBit", "logo.bmp"), locator.ImagePaths["Logo"]);
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_isDisposed)
        {
            return;
        }

        _isDisposed = true;

        try
        {
            if (disposing && Directory.Exists(_tempRoot))
            {
                Directory.Delete(_tempRoot, true);
            }
        }
        catch (IOException)
        {
            // Best-effort cleanup; swallow exceptions to avoid masking test results.
        }
        catch (UnauthorizedAccessException)
        {
            // Best-effort cleanup; swallow exceptions to avoid masking test results.
        }
    }

    private static MemoryStream ToStream(object manifest)
        => new(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(manifest)));

    private static object Manifest(SystemTier[] tiers, Dictionary<string, object> tierOverrides) => new
    {
        Tiers = tiers.Select(x => x.ToString()).ToArray(),
        TierOverrides = tierOverrides,
        Palette = "palette.json",
        FontsBitmap = new Dictionary<string, object>
        {
            { "Small", new { File = "font1.bmp", CellWidth = 8, CellHeight = 8, Columns = 16 } },
        },
        Images = new Dictionary<string, string> { { "Logo", "logo.bmp" } },
        Models = new Dictionary<string, string> { { "Ship", "ship.obj" } },
    };

    private void GivenAsset(string category, string? tier, string filename)
    {
        string directory = tier is null
            ? Path.Combine(_assetsRoot, category)
            : Path.Combine(_assetsRoot, category, tier);
        Directory.CreateDirectory(directory);
        File.WriteAllText(Path.Combine(directory, filename), string.Empty);
    }

    private AssetLocator Locate(SystemTier tier, params SystemTier[] shipped)
        => Locate(tier, Manifest(shipped, []));

    private AssetLocator Locate(SystemTier tier, object manifest)
    {
        using MemoryStream stream = ToStream(manifest);
        return AssetLocator.Create(stream, _tempRoot, tier);
    }
}
