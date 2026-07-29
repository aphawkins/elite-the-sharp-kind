// 'Useful Libraries' - Andy Hawkins 2023-2026.

using Microsoft.Extensions.Logging;
using Useful.Config;
using Xunit;

namespace Useful.Tests.Config;

public class ConfigFileTests
{
    private const string ConfigFileName = "test.cfg";

    [Fact]
    public void ReadConfigWithoutAFileReturnsDefaults()
    {
        // Arrange
        ConfigFile<TestSettings> configFile = new(CreateTempDirectory(), ConfigFileName);

        // Act
        TestSettings config = configFile.ReadConfig();

        // Assert
        Assert.True(config.Flag);
        Assert.Equal(5, config.Number);
    }

    [Fact]
    public void WriteConfigThenReadConfigRoundTrips()
    {
        // Arrange
        ConfigFile<TestSettings> configFile = new(CreateTempDirectory(), ConfigFileName);
        TestSettings written = new() { Flag = false, Number = 42 };

        // Act
        configFile.WriteConfig(written);
        TestSettings read = configFile.ReadConfig();

        // Assert
        Assert.False(read.Flag);
        Assert.Equal(42, read.Number);
    }

    [Fact]
    public void ReadConfigWithAMistypedValueReturnsDefaultsInsteadOfThrowing()
    {
        // Arrange: a hand-edited/corrupt file where a bool field holds a
        // non-boolean string - Microsoft.Extensions.Configuration.Binder
        // wraps this as InvalidOperationException, not FormatException.
        string directory = CreateTempDirectory();
        Directory.CreateDirectory(directory);
        File.WriteAllText(Path.Combine(directory, ConfigFileName), /*lang=json,strict*/ "{\"Flag\": \"hello!\"}");
        ConfigFile<TestSettings> configFile = new(directory, ConfigFileName);

        // Act
        TestSettings config = configFile.ReadConfig();

        // Assert
        Assert.True(config.Flag);
    }

    [Fact]
    public void ReadConfigRepairsOnlyTheValueThatCannotBeHonoured()
    {
        // Arrange: the bad Number must not cost the user their Flag as well.
        string directory = CreateTempDirectory();
        Directory.CreateDirectory(directory);
        File.WriteAllText(Path.Combine(directory, ConfigFileName), /*lang=json,strict*/ "{\"Number\": -1, \"Flag\": false}");
        ConfigFile<TestSettings> configFile = new(directory, ConfigFileName, Repair);

        // Act
        TestSettings config = configFile.ReadConfig();

        // Assert
        Assert.Equal(5, config.Number);
        Assert.False(config.Flag);
    }

    [Fact]
    public void ReadConfigWritesTheRepairedConfigBackOut()
    {
        // Arrange
        string directory = CreateTempDirectory();
        Directory.CreateDirectory(directory);
        File.WriteAllText(Path.Combine(directory, ConfigFileName), /*lang=json,strict*/ "{\"Number\": -1}");
        ConfigFile<TestSettings> configFile = new(directory, ConfigFileName, Repair);

        // Act
        configFile.ReadConfig();

        // Assert: reading again, with nothing to repair, sees the fixed value.
        Assert.Equal(5, new ConfigFile<TestSettings>(directory, ConfigFileName).ReadConfig().Number);
    }

    [Fact]
    public void ReadConfigKeepsACopyOfAConfigItHadToRepair()
    {
        // Arrange
        string directory = CreateTempDirectory();
        Directory.CreateDirectory(directory);
        File.WriteAllText(Path.Combine(directory, ConfigFileName), /*lang=json,strict*/ "{\"Number\": -1}");
        ConfigFile<TestSettings> configFile = new(directory, ConfigFileName, Repair);

        // Act
        configFile.ReadConfig();

        // Assert: the original is still recoverable by hand.
        Assert.Contains("-1", File.ReadAllText(Path.Combine(directory, ConfigFileName + ".bad")), StringComparison.Ordinal);
    }

    [Fact]
    public void ReadConfigKeepsACopyOfAConfigItCouldNotRead()
    {
        // Arrange
        string directory = CreateTempDirectory();
        Directory.CreateDirectory(directory);
        File.WriteAllText(Path.Combine(directory, ConfigFileName), /*lang=json,strict*/ "{\"Flag\": \"hello!\"}");
        ConfigFile<TestSettings> configFile = new(directory, ConfigFileName);

        // Act
        configFile.ReadConfig();

        // Assert
        Assert.Contains("hello!", File.ReadAllText(Path.Combine(directory, ConfigFileName + ".bad")), StringComparison.Ordinal);
    }

    [Fact]
    public void ReadConfigLeavesNoBackupWhenThereWasNothingToRepair()
    {
        // Arrange
        string directory = CreateTempDirectory();
        Directory.CreateDirectory(directory);
        File.WriteAllText(Path.Combine(directory, ConfigFileName), /*lang=json,strict*/ "{\"Number\": 7}");
        ConfigFile<TestSettings> configFile = new(directory, ConfigFileName, Repair);

        // Act
        configFile.ReadConfig();

        // Assert
        Assert.False(File.Exists(Path.Combine(directory, ConfigFileName + ".bad")));
    }

    [Fact]
    public void ReadConfigWithAMistypedValueLogsAWarningAndADebugDetailWithTheException()
    {
        // Arrange: the Warning is always visible (no exception attached, so
        // no stack trace at the default level); the Debug-level message
        // carries the exception, so the stack trace only appears once that
        // level is turned on.
        string directory = CreateTempDirectory();
        Directory.CreateDirectory(directory);
        File.WriteAllText(Path.Combine(directory, ConfigFileName), /*lang=json,strict*/ "{\"Flag\": \"hello!\"}");
        RecordingLogger<ConfigFile<TestSettings>> logger = new();
        ConfigFile<TestSettings> configFile = new(directory, ConfigFileName, null, logger);

        // Act
        configFile.ReadConfig();

        // Assert
        Assert.Contains(LogLevel.Warning, logger.Levels);
        (LogLevel Level, Exception? Exception) debugEntry = Assert.Single(logger.Entries, e => e.Level == LogLevel.Debug);
        Assert.NotNull(debugEntry.Exception);
    }

    [Fact]
    public void ReadConfigRepairingAValueLogsAWarningWithNoException()
    {
        // Arrange
        string directory = CreateTempDirectory();
        Directory.CreateDirectory(directory);
        File.WriteAllText(Path.Combine(directory, ConfigFileName), /*lang=json,strict*/ "{\"Number\": -1}");
        RecordingLogger<ConfigFile<TestSettings>> logger = new();
        ConfigFile<TestSettings> configFile = new(directory, ConfigFileName, Repair, logger);

        // Act
        configFile.ReadConfig();

        // Assert: a warning for the repair itself, and information for the
        // copy taken of the file it replaced - neither carries an exception.
        (LogLevel Level, Exception? Exception) entry = Assert.Single(logger.Entries, e => e.Level == LogLevel.Warning);
        Assert.Null(entry.Exception);
        Assert.Contains(LogLevel.Information, logger.Levels);
    }

    // Stands in for a game's own repair: negative numbers cannot be honoured.
    private static bool Repair(TestSettings config)
    {
        if (config.Number >= 0)
        {
            return false;
        }

        config.Number = 5;
        return true;
    }

    private static string CreateTempDirectory()
        => Path.Combine(Path.GetTempPath(), "ConfigFileTests_" + Guid.NewGuid().ToString("N"));

    private sealed class TestSettings
    {
        public bool Flag { get; set; } = true;

        public int Number { get; set; } = 5;
    }

    // Minimal in-test fake: the [LoggerMessage] source generator calls
    // ILogger.Log directly, which Moq's generic-method verification
    // handles awkwardly, so a recording fake is simpler than a mock here.
    private sealed class RecordingLogger<TCategory> : ILogger<TCategory>
    {
        public List<(LogLevel Level, Exception? Exception)> Entries { get; } = [];

        public IEnumerable<LogLevel> Levels => Entries.Select(e => e.Level);

        public IDisposable? BeginScope<TState>(TState state)
            where TState : notnull => null;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter)
            => Entries.Add((logLevel, exception));
    }
}
