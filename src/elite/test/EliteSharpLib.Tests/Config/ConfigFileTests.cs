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
            $"{{\"engine\": {{\"windowScale\": {scale}, \"tier\": \"8Bit\"}}}}");

        Assert.Equal(1, config.Engine.WindowScale);
        Assert.Equal(SystemTier.EightBit, config.Engine.Tier);
    }

    [Fact]
    public void ReadConfigKeepsAWindowScaleItCanHonour()
    {
        // The scale is independent of the tier: a magnified 8-bit window is
        // the point of the setting, not a contradiction to repair away.
        EliteConfig config = ReadWritten(
            /*lang=json,strict*/ "{\"engine\": {\"windowScale\": 3, \"tier\": \"8Bit\"}}");

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

    // The tier is spelled with a digit in the file - "8Bit", "16Bit" - not
    // with the C# member name, which cannot start with one. A round trip
    // alone would pass either way, so the written text is checked directly.
    [Theory]
    [InlineData(SystemTier.EightBit, "8Bit")]
    [InlineData(SystemTier.SixteenBit, "16Bit")]
    public void WriteConfigSpellsTheTierWithADigit(SystemTier tier, string expected)
    {
        string directory = CreateTempDirectory();
        ConfigFile<EliteConfig> configFile = new(directory, ConfigFileName);

        configFile.WriteConfig(new() { Engine = new() { Tier = tier } });

        string json = File.ReadAllText(Path.Combine(directory, ConfigFileName));
        Assert.Contains($"\"tier\": \"{expected}\"", json, StringComparison.Ordinal);
    }

    // Reading goes through the configuration binder rather than
    // System.Text.Json, so the digit spelling has to be understood on that
    // side too - and the old member-name spelling still has to read, or
    // every config file written before this change loses its tier.
    [Theory]
    [InlineData("8Bit", SystemTier.EightBit)]
    [InlineData("16Bit", SystemTier.SixteenBit)]
    [InlineData("EightBit", SystemTier.EightBit)]
    [InlineData("SixteenBit", SystemTier.SixteenBit)]
    public void ReadConfigAcceptsBothTierSpellings(string written, SystemTier expected)
        => Assert.Equal(expected, ReadWritten($"{{\"engine\": {{\"tier\": \"{written}\"}}}}").Engine.Tier);

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
