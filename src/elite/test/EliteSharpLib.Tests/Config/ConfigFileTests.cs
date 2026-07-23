// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Config;
using Useful.Config;

namespace EliteSharpLib.Tests.Config;

public class ConfigFileTests
{
    private const string ConfigFileName = "elitesharp.cfg";

    [Fact]
    public void ReadConfigWithoutAFileReturnsDefaults()
    {
        // Arrange
        ConfigFile<EliteConfigSettings> configFile = new(CreateTempDirectory(), ConfigFileName);

        // Act
        EliteConfigSettings config = configFile.ReadConfig();

        // Assert
        Assert.Equal(60f, config.Fps);
        Assert.True(config.MusicOn);
        Assert.True(config.EffectsOn);
    }

    [Fact]
    public void WriteConfigThenReadConfigRoundTrips()
    {
        // Arrange
        ConfigFile<EliteConfigSettings> configFile = new(CreateTempDirectory(), ConfigFileName);
        EliteConfigSettings written = new() { MusicOn = false, EffectsOn = false, InstantDock = true };

        // Act
        configFile.WriteConfig(written);
        EliteConfigSettings read = configFile.ReadConfig();

        // Assert
        Assert.False(read.MusicOn);
        Assert.False(read.EffectsOn);
        Assert.True(read.InstantDock);
    }

    [Fact]
    public void ReadConfigWithAMistypedValueReturnsDefaultsInsteadOfThrowing()
    {
        // Arrange: a hand-edited/corrupt file where a bool field holds a
        // non-boolean string - Microsoft.Extensions.Configuration.Binder
        // wraps this as InvalidOperationException, not FormatException.
        string directory = CreateTempDirectory();
        Directory.CreateDirectory(directory);
        File.WriteAllText(Path.Combine(directory, ConfigFileName), /*lang=json,strict*/ "{\"ShipWireframe\": \"hello!\"}");
        ConfigFile<EliteConfigSettings> configFile = new(directory, ConfigFileName);

        // Act
        EliteConfigSettings config = configFile.ReadConfig();

        // Assert
        Assert.False(config.ShipWireframe);
    }

    [Fact]
    public void ReadConfigWithInvalidFpsFallsBackToDefaults()
    {
        // Arrange: exercises AddEliteConfig's actual validation predicate
        // (Fps > 0), not just the generic ConfigFile<T> plumbing.
        string directory = CreateTempDirectory();
        Directory.CreateDirectory(directory);
        File.WriteAllText(Path.Combine(directory, ConfigFileName), /*lang=json,strict*/ "{\"Fps\": 0}");
        ConfigFile<EliteConfigSettings> configFile = new(directory, ConfigFileName, EliteServiceCollectionExtensions.IsValidConfig);

        // Act
        EliteConfigSettings config = configFile.ReadConfig();

        // Assert
        Assert.Equal(60f, config.Fps);
    }

    private static string CreateTempDirectory()
        => Path.Combine(Path.GetTempPath(), "ConfigFileTests_" + Guid.NewGuid().ToString("N"));
}
