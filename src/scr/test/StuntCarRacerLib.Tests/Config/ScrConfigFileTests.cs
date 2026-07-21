// 'Stunt Car Racer - The Sharp Kind' - Andy Hawkins 2026.
// 'Stunt Car Racer Remake' - sourceforge.net/projects/stuntcarremake.
// Stunt Car Racer (C) Geoff Crammond / MicroStyle / MicroProse 1989.

using StuntCarRacerLib.Config;
using Useful.Config;
using Xunit;

namespace StuntCarRacerLib.Tests.Config;

public class ScrConfigFileTests
{
    private const string ConfigFileName = "stuntcarracersharp.cfg";

    [Fact]
    public void ReadConfigWithoutAFileReturnsDefaults()
    {
        // Arrange
        ConfigFile<ScrConfigSettings> configFile = new(CreateTempDirectory(), ConfigFileName);

        // Act
        ScrConfigSettings config = configFile.ReadConfig();

        // Assert
        Assert.True(config.MusicOn);
        Assert.True(config.EffectsOn);
    }

    [Fact]
    public void WriteConfigThenReadConfigRoundTrips()
    {
        // Arrange
        ConfigFile<ScrConfigSettings> configFile = new(CreateTempDirectory(), ConfigFileName);
        ScrConfigSettings written = new() { MusicOn = false, EffectsOn = false };

        // Act
        configFile.WriteConfig(written);
        ScrConfigSettings read = configFile.ReadConfig();

        // Assert
        Assert.False(read.MusicOn);
        Assert.False(read.EffectsOn);
    }

    [Fact]
    public void ReadConfigWithAMistypedValueReturnsDefaultsInsteadOfThrowing()
    {
        // Arrange: a hand-edited/corrupt file where a bool field holds a
        // non-boolean string - Microsoft.Extensions.Configuration.Binder
        // wraps this as InvalidOperationException, not FormatException.
        string directory = CreateTempDirectory();
        Directory.CreateDirectory(directory);
        File.WriteAllText(Path.Combine(directory, ConfigFileName), /*lang=json,strict*/ "{\"MusicOn\": \"hello!\"}");
        ConfigFile<ScrConfigSettings> configFile = new(directory, ConfigFileName);

        // Act
        ScrConfigSettings config = configFile.ReadConfig();

        // Assert
        Assert.True(config.MusicOn);
    }

    private static string CreateTempDirectory()
        => Path.Combine(Path.GetTempPath(), "ScrConfigFileTests_" + Guid.NewGuid().ToString("N"));
}
