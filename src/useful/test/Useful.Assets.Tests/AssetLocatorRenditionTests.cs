// 'Useful Libraries' - Andy Hawkins 2023-2026.

using System.Text;
using System.Text.Json;
using Xunit;

namespace Useful.Assets.Tests;

public class AssetLocatorRenditionTests : IDisposable
{
    private readonly string _tempRoot;
    private readonly string _assetsRoot;
    private bool _isDisposed;

    public AssetLocatorRenditionTests()
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
        AssetLocator locator = Locate("SixteenBit");

        // Act
        string path = locator.ImagePaths["Logo"];

        // Assert
        Assert.Equal(Path.Combine(_assetsRoot, "Images", "SixteenBit", "logo.bmp"), path);
    }

    [Fact]
    public void FallsBackToTheSharedFolderWhenTheTierHasNoCopy()
    {
        // Arrange: the file sits directly under Images, with no rendition folder,
        // which is what keeps rendition-neutral assets from needing duplicating.
        GivenAsset("Images", null, "logo.bmp");
        AssetLocator locator = Locate("SixteenBit");

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
        string eightBit = Locate("EightBit").ImagePaths["Logo"];
        string sixteenBit = Locate("SixteenBit").ImagePaths["Logo"];

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
        AssetLocator locator = Locate("SixteenBit");

        // Assert
        Assert.Equal(Path.Combine(_assetsRoot, "FontsBitmap", "SixteenBit", "font1.bmp"), locator.FontBitmaps["Small"].Path);
        Assert.Equal(Path.Combine(_assetsRoot, "Palette", "SixteenBit", "palette.json"), locator.PalettePath);
    }

    [Fact]
    public void ResolvesModelsByTier()
    {
        // Arrange: a model's 'usemtl' names are resolved through the rendition's
        // palette, and the two palettes name different colours, so the models
        // are rendition-varying like the images are.
        GivenAsset("Models", "SixteenBit", "ship.obj");
        AssetLocator locator = Locate("SixteenBit");

        // Act
        string path = locator.ModelPaths["Ship"];

        // Assert
        Assert.Equal(Path.Combine(_assetsRoot, "Models", "SixteenBit", "ship.obj"), path);
    }

    [Fact]
    public void LeavesTierNeutralCategoriesOutsideTheTierFolder()
    {
        // Arrange: audio and TrueType fonts are resolution-independent and must
        // not gain a rendition segment even when a rendition folder exists.
        GivenAsset("SFX", "SixteenBit", "beep.wav");
        AssetLocator locator = Locate("SixteenBit");

        // Act
        string path = locator.SfxPaths["Beep"];

        // Assert
        Assert.Equal(Path.Combine(_assetsRoot, "SFX", "beep.wav"), path);
    }

    [Fact]
    public void ATierManifestReplacesTheEntriesItNames()
    {
        // Arrange: the 8-bit set uses a different file for the same logical name.
        GivenAsset("Images", "EightBit", "logo-small.bmp");
        object renditionManifest = new { Images = new Dictionary<string, string> { { "Logo", "logo-small.bmp" } } };

        // Act
        string path = LocateWithRenditionManifest("EightBit", Manifest(), renditionManifest).ImagePaths["Logo"];

        // Assert
        Assert.Equal(Path.Combine(_assetsRoot, "Images", "EightBit", "logo-small.bmp"), path);
    }

    [Fact]
    public void ATierManifestLeavesEntriesItDoesNotNameAlone()
    {
        // Arrange: a rendition manifest that only replaces the font must not
        // disturb the images, models or palette the base manifest declares.
        GivenAsset("Images", "EightBit", "logo.bmp");
        object renditionManifest = new
        {
            FontsBitmap = new Dictionary<string, object>
            {
                { "Small", new { File = "bbc.bmp", CellWidth = 10, CellHeight = 10, Columns = 12 } },
            },
        };

        // Act
        AssetLocator locator = LocateWithRenditionManifest("EightBit", Manifest(), renditionManifest);

        // Assert
        Assert.Equal(Path.Combine(_assetsRoot, "Images", "EightBit", "logo.bmp"), locator.ImagePaths["Logo"]);
        Assert.Equal(Path.Combine(_assetsRoot, "Models", "ship.obj"), locator.ModelPaths["Ship"]);
        Assert.Equal(10, locator.FontBitmaps["Small"].CellWidth);
        Assert.Equal(12, locator.FontBitmaps["Small"].Columns);
    }

    [Fact]
    public void DefaultsToTheSixteenBitTier()
    {
        // Arrange
        GivenAsset("Images", "SixteenBit", "logo.bmp");
        using MemoryStream stream = ToStream(Manifest());

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

    private static object Manifest() => new
    {
        Palette = "palette.json",
        FontsBitmap = new Dictionary<string, object>
        {
            { "Small", new { File = "font1.bmp", CellWidth = 8, CellHeight = 8, Columns = 16 } },
        },
        Images = new Dictionary<string, string> { { "Logo", "logo.bmp" } },
        Models = new Dictionary<string, string> { { "Ship", "ship.obj" } },
        Sfx = new Dictionary<string, string> { { "Beep", "beep.wav" } },
    };

    private void GivenAsset(string category, string? rendition, string filename)
    {
        string directory = rendition is null
            ? Path.Combine(_assetsRoot, category)
            : Path.Combine(_assetsRoot, category, rendition);
        Directory.CreateDirectory(directory);
        File.WriteAllText(Path.Combine(directory, filename), string.Empty);
    }

    private AssetLocator Locate(string rendition)
        => Locate(rendition, Manifest());

    private AssetLocator Locate(string rendition, object manifest)
    {
        using MemoryStream stream = ToStream(manifest);
        return AssetLocator.Create(stream, _tempRoot, rendition);
    }

    private AssetLocator LocateWithRenditionManifest(string rendition, object manifest, object renditionManifest)
    {
        using MemoryStream stream = ToStream(manifest);
        using MemoryStream renditionStream = ToStream(renditionManifest);
        return AssetLocator.Create(stream, renditionStream, _tempRoot, rendition);
    }
}
