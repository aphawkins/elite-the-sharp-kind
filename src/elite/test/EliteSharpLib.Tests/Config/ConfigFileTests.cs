// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Config;
using EliteSharpLib.Suns;
using Useful.Abstraction.Config;
using Useful.Assets;
using Useful.Config;

namespace EliteSharpLib.Tests.Config;

public class ConfigFileTests
{
    private const string ConfigFileName = "elite.sharp";

    [Fact]
    public void ReadConfigWithoutAFileReturnsDefaults()
    {
        // Arrange
        ConfigFile<EliteConfig> configFile = new(CreateTempDirectory(), ConfigFileName);

        // Act
        EliteConfig config = configFile.ReadConfig();

        // Assert
        Assert.Equal(60f, config.Engine.Graphics.Fps);
        Assert.False(config.Engine.Graphics.ShowFps);
        Assert.Equal(1, config.Engine.WindowScale);
        Assert.True(config.Engine.Sound.Music);
        Assert.True(config.Engine.Sound.Effects);
    }

    [Fact]
    public void WriteConfigThenReadConfigRoundTrips()
    {
        // Arrange
        ConfigFile<EliteConfig> configFile = new(CreateTempDirectory(), ConfigFileName);
        EliteConfig written = new()
        {
            Engine = new() { Sound = new() { Music = false, Effects = false } },
            Game = new() { InstantDock = true },
        };

        // Act
        configFile.WriteConfig(written);
        EliteConfig read = configFile.ReadConfig();

        // Assert
        Assert.False(read.Engine.Sound.Music);
        Assert.False(read.Engine.Sound.Effects);
        Assert.True(read.Game.InstantDock);
    }

    [Fact]
    public void ReadConfigWithAMistypedValueReturnsDefaultsInsteadOfThrowing()
    {
        // Arrange: a hand-edited/corrupt file where a bool field holds a
        // non-boolean string - Microsoft.Extensions.Configuration.Binder
        // wraps this as InvalidOperationException, not FormatException.
        string directory = CreateTempDirectory();
        Directory.CreateDirectory(directory);
        File.WriteAllText(Path.Combine(directory, ConfigFileName), /*lang=json,strict*/ "{\"game\": {\"instantDock\": \"hello!\"}}");
        ConfigFile<EliteConfig> configFile = new(directory, ConfigFileName);

        // Act
        EliteConfig config = configFile.ReadConfig();

        // Assert
        Assert.False(config.Game.InstantDock);
    }

    [Fact]
    public void ReadConfigWithInvalidFpsRepairsOnlyTheFps()
    {
        // Arrange: exercises AddEliteConfig's actual repair, not just the
        // generic ConfigFile<T> plumbing. The unreadable fps must not cost
        // the user the settings either side of it.
        EliteConfig config = ReadWritten(
            /*lang=json,strict*/ "{\"engine\": {\"graphics\": {\"fps\": 0}}, \"game\": {\"instantDock\": true}}");

        // Assert
        Assert.Equal(60f, config.Engine.Graphics.Fps);
        Assert.True(config.Game.InstantDock);
    }

    [Fact]
    public void ReadConfigHonoursShowFps()
    {
        EliteConfig config = ReadWritten(
            /*lang=json,strict*/ "{\"engine\": {\"graphics\": {\"showFps\": true}}}");

        Assert.True(config.Engine.Graphics.ShowFps);
    }

    [Fact]
    public void ReadConfigWithAnOutOfRangeEnumRepairsThatSettingAlone()
    {
        EliteConfig config = ReadWritten(
            /*lang=json,strict*/ "{\"engine\": {\"tier\": 7}, \"game\": {\"sunStyle\": \"Solid\"}}");

        Assert.Equal(SystemTier.SixteenBit, config.Engine.Tier);
        Assert.Equal(SunType.Solid, config.Game.SunStyle);
    }

    // The limit of repairing in place: a value the binder cannot even parse
    // (a misspelt enum name, a string where a number belongs) fails the whole
    // bind, so there is nothing to repair and the defaults stand. The file
    // itself is kept as .bad, which is the only reason that is survivable.
    [Fact]
    public void ReadConfigWithAnUnparseableValueFallsBackToDefaultsAndKeepsTheFile()
    {
        string directory = CreateTempDirectory();
        Directory.CreateDirectory(directory);
        File.WriteAllText(
            Path.Combine(directory, ConfigFileName),
            /*lang=json,strict*/ "{\"engine\": {\"tier\": \"ThirtyTwoBit\"}, \"game\": {\"instantDock\": true}}");
        ConfigFile<EliteConfig> configFile = new(directory, ConfigFileName, EliteServiceCollectionExtensions.RepairConfig);

        EliteConfig config = configFile.ReadConfig();

        Assert.Equal(SystemTier.SixteenBit, config.Engine.Tier);
        Assert.False(config.Game.InstantDock);
        Assert.True(File.Exists(Path.Combine(directory, ConfigFileName + ".bad")));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-2)]
    [InlineData(5)]
    public void ReadConfigWithAnUnusableWindowScaleRepairsThatSettingAlone(int scale)
    {
        EliteConfig config = ReadWritten(
            $"{{\"engine\": {{\"windowScale\": {scale}, \"tier\": \"EightBit\"}}}}");

        Assert.Equal(1, config.Engine.WindowScale);
        Assert.Equal(SystemTier.EightBit, config.Engine.Tier);
    }

    [Fact]
    public void ReadConfigKeepsAWindowScaleItCanHonour()
    {
        // The scale is independent of the tier: a magnified 8-bit window is
        // the point of the setting, not a contradiction to repair away.
        EliteConfig config = ReadWritten(
            /*lang=json,strict*/ "{\"engine\": {\"windowScale\": 3, \"tier\": \"EightBit\"}}");

        Assert.Equal(3, config.Engine.WindowScale);
        Assert.Equal(SystemTier.EightBit, config.Engine.Tier);
    }

    [Fact]
    public void ReadConfigStampsTheCurrentSchemaVersion()
    {
        // A file from before versioning has no version at all, and one from a
        // later build claims a version this one cannot honour; both are
        // brought back to what this build writes.
        Assert.Equal(ConfigSchema.CurrentVersion, ReadWritten(/*lang=json,strict*/ "{\"game\": {}}").Version);
        Assert.Equal(ConfigSchema.CurrentVersion, ReadWritten(/*lang=json,strict*/ "{\"version\": 99}").Version);
    }

    private static EliteConfig ReadWritten(string json)
    {
        string directory = CreateTempDirectory();
        Directory.CreateDirectory(directory);
        File.WriteAllText(Path.Combine(directory, ConfigFileName), json);
        ConfigFile<EliteConfig> configFile = new(directory, ConfigFileName, EliteServiceCollectionExtensions.RepairConfig);

        return configFile.ReadConfig();
    }

    private static string CreateTempDirectory()
        => Path.Combine(Path.GetTempPath(), "ConfigFileTests_" + Guid.NewGuid().ToString("N"));
}
