// 'Stunt Car Racer - The Sharp Kind' - Andy Hawkins 2026.
// 'Stunt Car Racer Remake' - sourceforge.net/projects/stuntcarremake.
// Stunt Car Racer (C) Geoff Crammond / MicroStyle / MicroProse 1989.

using StuntCarRacerSharpLib.Config;
using Useful.Abstraction;
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
        Assert.True(config.Engine.Sound.Music);
        Assert.True(config.Engine.Sound.Effects);
    }

    [Fact]
    public void WriteConfigThenReadConfigRoundTrips()
    {
        // Arrange
        ConfigFile<ScrConfig> configFile = new(CreateTempDirectory(), ConfigFileName);
        ScrConfig written = new() { Engine = new() { Sound = new() { Music = false, Effects = false } } };

        // Act
        configFile.WriteConfig(written);
        ScrConfig read = configFile.ReadConfig();

        // Assert
        Assert.False(read.Engine.Sound.Music);
        Assert.False(read.Engine.Sound.Effects);
    }

    [Fact]
    public void ReadConfigWithAMistypedValueReturnsDefaultsInsteadOfThrowing()
    {
        // Arrange: a hand-edited/corrupt file where a bool field holds a
        // non-boolean string - Microsoft.Extensions.Configuration.Binder
        // wraps this as InvalidOperationException, not FormatException.
        string directory = CreateTempDirectory();
        Directory.CreateDirectory(directory);
        File.WriteAllText(
            Path.Combine(directory, ConfigFileName),
            /*lang=json,strict*/ "{\"engine\": {\"sound\": {\"music\": \"hello!\"}}}");
        ConfigFile<ScrConfig> configFile = new(directory, ConfigFileName);

        // Act
        ScrConfig config = configFile.ReadConfig();

        // Assert
        Assert.True(config.Engine.Sound.Music);
    }

    // Stunt Car Racer used to pass no validation at all, so a bad engine
    // setting sat in the file until it broke something at startup.
    [Fact]
    public void ReadConfigRepairsAnOutOfRangeBackend()
    {
        // Arrange
        string directory = CreateTempDirectory();
        Directory.CreateDirectory(directory);
        File.WriteAllText(
            Path.Combine(directory, ConfigFileName),
            /*lang=json,strict*/ "{\"engine\": {\"backend\": 7, \"sound\": {\"music\": false}}}");
        ConfigFile<ScrConfig> configFile = new(directory, ConfigFileName, StuntCarRacerServiceCollectionExtensions.RepairConfig);

        // Act
        ScrConfig config = configFile.ReadConfig();

        // Assert: the backend is back at its default, the sound setting either
        // side of it survives.
        Assert.Equal(Backend.Software, config.Engine.Backend);
        Assert.False(config.Engine.Sound.Music);
    }

    private static string CreateTempDirectory()
        => Path.Combine(Path.GetTempPath(), "ScrConfigFileTests_" + Guid.NewGuid().ToString("N"));
}
