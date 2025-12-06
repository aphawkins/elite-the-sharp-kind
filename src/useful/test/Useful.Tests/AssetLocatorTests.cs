// 'Useful Libraries' - Andy Hawkins 2025.

using System.Text;
using System.Text.Json;
using Useful.Assets;
using Xunit;

namespace Useful.Tests;

public class AssetLocatorTests
{
    [Fact]
    public void CreateFromStreamBuildsPaths()
    {
        // Arrange
        string tempRoot = Path.Combine(Path.GetTempPath(), "Useful.Assets.Tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempRoot);
        string assetsRoot = Path.Combine(tempRoot, "Assets");
        Directory.CreateDirectory(assetsRoot);
        Directory.CreateDirectory(Path.Combine(assetsRoot, "Palette"));
        Directory.CreateDirectory(Path.Combine(assetsRoot, "FontsBitmap"));
        Directory.CreateDirectory(Path.Combine(assetsRoot, "FontsTrueType"));
        Directory.CreateDirectory(Path.Combine(assetsRoot, "Images"));
        Directory.CreateDirectory(Path.Combine(assetsRoot, "Music"));
        Directory.CreateDirectory(Path.Combine(assetsRoot, "SFX"));
        Directory.CreateDirectory(Path.Combine(assetsRoot, "Models"));
        Directory.CreateDirectory(Path.Combine(assetsRoot, "SoundFonts"));

        string manifestPath = Path.Combine(assetsRoot, "AssetManifest.json");

        object manifestObject = new
        {
            Palette = "palette.png",
            FontsBitmap = new Dictionary<string, string>
            {
                { "Arial", "arial.png" },
                { "Vera", "vera.png" },
            },
            FontsTrueType = new Dictionary<string, string>
            {
                { "Roboto", "roboto.ttf" },
            },
            Images = new Dictionary<string, string>
            {
                { "Logo", "logo.png" },
            },
            Music = new Dictionary<string, string>
            {
                { "Theme", "theme.mp3" },
            },
            Sfx = new Dictionary<string, string>
            {
                { "Click", "click.wav" },
            },
            Models = new Dictionary<string, string>
            {
                { "Ship", "ship.model" },
            },
            SoundFonts = new Dictionary<string, string>
            {
                { "Fontella", "fontella.sf2" },
            },
        };

        string manifestJson = JsonSerializer.Serialize(manifestObject);
        File.WriteAllText(manifestPath, manifestJson);

        try
        {
            using FileStream stream = File.OpenRead(manifestPath);

            // Act
            AssetLocator locator = AssetLocator.Create(stream, tempRoot);

            // Assert - Palette
            string expectedPalette = Path.Combine(assetsRoot, "Palette", "palette.png");
            Assert.Equal(expectedPalette, locator.PalettePath);

            // Assert - FontsBitmap
            IDictionary<int, string> fontBitmapPaths = locator.FontBitmapPaths;
            Assert.Equal(2, fontBitmapPaths.Count);
            Assert.Contains(Path.Combine(assetsRoot, "FontsBitmap", "arial.png"), fontBitmapPaths.Values);
            Assert.Contains(Path.Combine(assetsRoot, "FontsBitmap", "vera.png"), fontBitmapPaths.Values);

            // Assert - FontsTrueType
            IDictionary<int, string> fontTrueTypePaths = locator.FontTrueTypePaths;
            Assert.Single(fontTrueTypePaths);
            Assert.Equal(Path.Combine(assetsRoot, "FontsTrueType", "roboto.ttf"), fontTrueTypePaths[0]);

            // Assert - Images
            IDictionary<int, string> imagePaths = locator.ImagePaths;
            Assert.Single(imagePaths);
            Assert.Equal(Path.Combine(assetsRoot, "Images", "logo.png"), imagePaths[0]);

            // Assert - Music
            IDictionary<int, string> musicPaths = locator.MusicPaths;
            Assert.Single(musicPaths);
            Assert.Equal(Path.Combine(assetsRoot, "Music", "theme.mp3"), musicPaths[0]);

            // Assert - Sfx
            IDictionary<int, string> sfxPaths = locator.SfxPaths;
            Assert.Single(sfxPaths);
            Assert.Equal(Path.Combine(assetsRoot, "SFX", "click.wav"), sfxPaths[0]);

            // Assert - Models
            IDictionary<string, string> modelPaths = locator.ModelPaths;
            Assert.Single(modelPaths);
            Assert.Equal(Path.Combine(assetsRoot, "Models", "ship.model"), modelPaths["Ship"]);

            // Assert - SoundFonts
            IDictionary<int, string> soundFontPaths = locator.SoundFontPaths;
            Assert.Single(soundFontPaths);
            Assert.Equal(Path.Combine(assetsRoot, "SoundFonts", "fontella.sf2"), soundFontPaths[0]);
        }
        finally
        {
            try
            {
                if (Directory.Exists(tempRoot))
                {
                    Directory.Delete(tempRoot, true);
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
    }

    [Fact]
    public void CreateDefaultThrowsWhenManifestMissing()
    {
        // Arrange
        string baseDir = Path.GetDirectoryName(AppContext.BaseDirectory) ?? string.Empty;
        string assetsRoot = Path.Combine(baseDir, "Assets");
        string manifestPath = Path.Combine(assetsRoot, "AssetManifest.json");

        if (File.Exists(manifestPath))
        {
            File.Delete(manifestPath);
        }

        // Ensure Assets dir exists but manifest missing
        Directory.CreateDirectory(assetsRoot);

        try
        {
            // Act & Assert
            Assert.Throws<UsefulException>(AssetLocator.Create);
        }
        finally
        {
            try
            {
                if (Directory.Exists(assetsRoot))
                {
                    Directory.Delete(assetsRoot, true);
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
    }

    [Fact]
    public void CreateFromStreamNullStreamThrowsArgumentNullException()
    {
        // Arrange
        Stream? nullStream = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => AssetLocator.Create(nullStream!, "ignored"));
    }

    [Fact]
    public void CreateFromStreamInvalidJsonThrowsUsefulException()
    {
        // Arrange
        string tempRoot = Path.Combine(Path.GetTempPath(), "Useful.Assets.Tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempRoot);
        string assetsRoot = Path.Combine(tempRoot, "Assets");
        Directory.CreateDirectory(assetsRoot);

        try
        {
            using MemoryStream ms = new();
            using StreamWriter sw = new(ms, Encoding.UTF8, 1024, true);
            sw.Write("{ invalid json");
            sw.Flush();
            ms.Position = 0;

            // Act & Assert
            Assert.Throws<UsefulException>(() => AssetLocator.Create(ms, tempRoot));
        }
        finally
        {
            try
            {
                if (Directory.Exists(tempRoot))
                {
                    Directory.Delete(tempRoot, true);
                }
            }
            catch (IOException)
            {
                // ignore cleanup errors
            }
            catch (UnauthorizedAccessException)
            {
                // ignore cleanup errors
            }
        }
    }

    [Fact]
    public void CreateFromStreamNullJsonThrowsUsefulException()
    {
        // Arrange
        string tempRoot = Path.Combine(Path.GetTempPath(), "Useful.Assets.Tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempRoot);
        string assetsRoot = Path.Combine(tempRoot, "Assets");
        Directory.CreateDirectory(assetsRoot);

        try
        {
            using MemoryStream ms = new();
            using StreamWriter sw = new(ms, Encoding.UTF8, 1024, true);
            sw.Write("null");
            sw.Flush();
            ms.Position = 0;

            // Act & Assert
            Assert.Throws<UsefulException>(() => AssetLocator.Create(ms, tempRoot));
        }
        finally
        {
            try
            {
                if (Directory.Exists(tempRoot))
                {
                    Directory.Delete(tempRoot, true);
                }
            }
            catch (IOException)
            {
                // ignore cleanup errors
            }
            catch (UnauthorizedAccessException)
            {
                // ignore cleanup errors
            }
        }
    }

    [Fact]
    public void CreateDefaultReadsManifestFileReturnsLocator()
    {
        // Arrange
        string baseDir = Path.GetDirectoryName(AppContext.BaseDirectory) ?? string.Empty;
        string assetsRoot = Path.Combine(baseDir, "Assets");
        Directory.CreateDirectory(assetsRoot);
        Directory.CreateDirectory(Path.Combine(assetsRoot, "Palette"));
        Directory.CreateDirectory(Path.Combine(assetsRoot, "FontsBitmap"));
        Directory.CreateDirectory(Path.Combine(assetsRoot, "FontsTrueType"));
        Directory.CreateDirectory(Path.Combine(assetsRoot, "Images"));
        Directory.CreateDirectory(Path.Combine(assetsRoot, "Music"));
        Directory.CreateDirectory(Path.Combine(assetsRoot, "SFX"));
        Directory.CreateDirectory(Path.Combine(assetsRoot, "Models"));
        Directory.CreateDirectory(Path.Combine(assetsRoot, "SoundFonts"));

        string manifestPath = Path.Combine(assetsRoot, "AssetManifest.json");

        object manifestObject = new
        {
            Palette = "palette.png",
            FontsBitmap = new Dictionary<string, string>
            {
                { "Arial", "arial.png" },
            },
            FontsTrueType = new Dictionary<string, string>
            {
                { "Roboto", "roboto.ttf" },
            },
            Images = new Dictionary<string, string>
            {
                { "Logo", "logo.png" },
            },
            Music = new Dictionary<string, string>
            {
                { "Theme", "theme.mp3" },
            },
            Sfx = new Dictionary<string, string>
            {
                { "Click", "click.wav" },
            },
            Models = new Dictionary<string, string>
            {
                { "Ship", "ship.model" },
            },
            SoundFonts = new Dictionary<string, string>
            {
                { "Fontella", "fontella.sf2" },
            },
        };

        string manifestJson = JsonSerializer.Serialize(manifestObject);
        File.WriteAllText(manifestPath, manifestJson);

        try
        {
            // Act
            AssetLocator locator = AssetLocator.Create();

            // Assert - simple sanity checks
            Assert.Equal(Path.Combine(assetsRoot, "Palette", "palette.png"), locator.PalettePath);
            Assert.Single(locator.FontBitmapPaths);
            Assert.Single(locator.FontTrueTypePaths);
            Assert.Single(locator.ImagePaths);
            Assert.Single(locator.MusicPaths);
            Assert.Single(locator.SfxPaths);
            Assert.Single(locator.ModelPaths);
            Assert.Single(locator.SoundFontPaths);
        }
        finally
        {
            try
            {
                if (File.Exists(manifestPath))
                {
                    File.Delete(manifestPath);
                }

                if (Directory.Exists(assetsRoot))
                {
                    Directory.Delete(assetsRoot, true);
                }
            }
            catch (IOException)
            {
                // ignore cleanup errors
            }
            catch (UnauthorizedAccessException)
            {
                // ignore cleanup errors
            }
        }
    }
}
