// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Config;
using EliteSharpLib.Suns;
using Useful.Abstraction.Config;
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
    public void ReadConfigKeepsARenditionNameItDoesNotRecognise()
    {
        // A name the game has never heard of is not a mistake to repair: the
        // whole point of renditions being named rather than enumerated is
        // that the game cannot know what exists. Whether one by that name is
        // installed is settled when it is looked for, and that failure names
        // it - repairing to the default here would quietly ignore what the
        // commander asked for.
        EliteConfig config = ReadWritten(
            /*lang=json,strict*/ "{\"engine\": {\"rendition\": \"Psychedelic\"}, \"game\": {\"sunStyle\": \"Solid\"}}");

        Assert.Equal("Psychedelic", config.Engine.Rendition);
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
            /*lang=json,strict*/ "{\"engine\": {\"windowScale\": \"lots\"}, \"game\": {\"instantDock\": true}}");
        ConfigFile<EliteConfig> configFile = new(directory, ConfigFileName, EliteServiceCollectionExtensions.RepairConfig);

        EliteConfig config = configFile.ReadConfig();

        Assert.Equal("16-bit", config.Engine.Rendition);
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
            $"{{\"engine\": {{\"windowScale\": {scale}, \"tier\": \"8Bit\"}}}}");

        Assert.Equal(1, config.Engine.WindowScale);
        Assert.Equal("8-bit", config.Engine.Rendition);
    }

    [Fact]
    public void ReadConfigKeepsAWindowScaleItCanHonour()
    {
        // The scale is independent of the rendition: a magnified 8-bit window
        // is the point of the setting, not a contradiction to repair away.
        EliteConfig config = ReadWritten(
            /*lang=json,strict*/ "{\"engine\": {\"windowScale\": 3, \"tier\": \"8Bit\"}}");

        Assert.Equal(3, config.Engine.WindowScale);
        Assert.Equal("8-bit", config.Engine.Rendition);
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

    // A rendition is written under the name it calls itself. The game has no
    // spelling of its own to apply - it cannot have one for a rendition it
    // has never seen.
    [Theory]
    [InlineData("8-bit")]
    [InlineData("Psychedelic")]
    public void WriteConfigWritesTheRenditionName(string rendition)
    {
        string directory = CreateTempDirectory();
        ConfigFile<EliteConfig> configFile = new(directory, ConfigFileName);

        configFile.WriteConfig(new() { Engine = new() { Rendition = rendition } });

        string json = File.ReadAllText(Path.Combine(directory, ConfigFileName));
        Assert.Contains($"\"rendition\": \"{rendition}\"", json, StringComparison.Ordinal);
    }

    // Files written before renditions existed say "tier", and spell it with a
    // digit. Both the old key and the old spelling have to survive, or every
    // config file written before this change quietly loses its choice.
    [Theory]
    [InlineData("8Bit", "8-bit")]
    [InlineData("16Bit", "16-bit")]
    [InlineData("EightBit", "8-bit")]
    public void RepairUpgradesTheOldTierSetting(string written, string expected)
    {
        EngineConfigSettings engine = new() { Tier = written };

        Assert.True(engine.Repair());
        Assert.Equal(expected, engine.Rendition);
        Assert.Null(engine.Tier);
    }

    // The old key only wins where the new one was never written, so a file
    // holding both - which only a hand-edit produces - keeps the new one.
    [Fact]
    public void RepairKeepsTheRenditionWhenBothAreSet()
    {
        EngineConfigSettings engine = new() { Rendition = "Psychedelic", Tier = "8Bit" };

        Assert.True(engine.Repair());
        Assert.Equal("Psychedelic", engine.Rendition);
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
