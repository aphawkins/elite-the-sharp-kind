// 'Stunt Car Racer - The Sharp Kind' - Andy Hawkins 2026.
// 'Stunt Car Racer Remake' - sourceforge.net/projects/stuntcarremake.
// Stunt Car Racer (C) Geoff Crammond / MicroStyle / MicroProse 1989.

using StuntCarRacerSharpLib.Config;
using Useful.Config;
using Xunit;

namespace StuntCarRacerSharpLib.Tests.Config;

public class ScrConfigFileTests
{
    private const string ConfigFileName = "stuntcarracer.sharp";

    [Fact]
    public void ReadConfigWithoutAFileReturnsDefaults()
    {
        // Arrange
        ConfigFile<ScrConfig> configFile = new(CreateTempDirectory(), ConfigFileName);

        // Act
        ScrConfig config = configFile.ReadConfig();

        // Assert
        Assert.True(config.Engine.MusicOn);
        Assert.True(config.Engine.EffectsOn);
    }

    [Fact]
    public void WriteConfigThenReadConfigRoundTrips()
    {
        // Arrange
        ConfigFile<ScrConfig> configFile = new(CreateTempDirectory(), ConfigFileName);
        ScrConfig written = new() { Engine = new() { MusicOn = false, EffectsOn = false } };

        // Act
        configFile.WriteConfig(written);
        ScrConfig read = configFile.ReadConfig();

        // Assert
        Assert.False(read.Engine.MusicOn);
        Assert.False(read.Engine.EffectsOn);
    }

    [Fact]
    public void ReadConfigWithAMistypedValueReturnsDefaultsInsteadOfThrowing()
    {
        // Arrange: a hand-edited/corrupt file where a bool field holds a
        // non-boolean string - Microsoft.Extensions.Configuration.Binder
        // wraps this as InvalidOperationException, not FormatException.
        string directory = CreateTempDirectory();
        Directory.CreateDirectory(directory);
        File.WriteAllText(Path.Combine(directory, ConfigFileName), /*lang=json,strict*/ "{\"engine\": {\"musicOn\": \"hello!\"}}");
        ConfigFile<ScrConfig> configFile = new(directory, ConfigFileName);

        // Act
        ScrConfig config = configFile.ReadConfig();

        // Assert
        Assert.True(config.Engine.MusicOn);
    }

    private static string CreateTempDirectory()
        => Path.Combine(Path.GetTempPath(), "ScrConfigFileTests_" + Guid.NewGuid().ToString("N"));
}
