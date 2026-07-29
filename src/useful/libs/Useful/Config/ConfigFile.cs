// 'Useful Libraries' - Andy Hawkins 2023-2026.

using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Useful.Config;

/// <summary>
/// Reads and writes a JSON-backed settings file of type <typeparamref name="T"/> from a
/// user-data directory. A missing file starts from <c>new T()</c>; a file that holds
/// values which cannot be honoured keeps everything that can be, with only the offending
/// entries reset. Whenever the file on disk is about to be replaced by something other
/// than what it held, the original is kept alongside it as <c>&lt;name&gt;.bad</c>.
/// </summary>
/// <typeparam name="T">The settings type. Must be default-constructible so a missing or unreadable file can fall back to defaults.</typeparam>
public sealed class ConfigFile<T> : IConfigWriter<T>
    where T : new()
{
    private readonly string _configFileName;
    private readonly Func<T, bool> _repair;
    private readonly ILogger<ConfigFile<T>> _logger;

    private readonly JsonSerializerOptions _writeOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = { new JsonStringEnumConverter() },
    };

    public ConfigFile(string baseDirectory, string configFileName)
        : this(baseDirectory, configFileName, null, null)
    {
    }

    public ConfigFile(string baseDirectory, string configFileName, Func<T, bool>? repair)
        : this(baseDirectory, configFileName, repair, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ConfigFile{T}"/> class,
    /// creating its directory if it does not exist yet.
    /// </summary>
    /// <param name="baseDirectory">The user-data directory the file lives in.</param>
    /// <param name="configFileName">The file's name within that directory.</param>
    /// <param name="repair">
    /// Replaces any value that cannot be honoured with its default, in place, returning
    /// whether it had to change anything. Null accepts whatever binds.
    /// </param>
    /// <param name="logger">Where read/write/repair problems are reported.</param>
    public ConfigFile(string baseDirectory, string configFileName, Func<T, bool>? repair, ILogger<ConfigFile<T>>? logger)
    {
        ArgumentNullException.ThrowIfNull(baseDirectory);
        ArgumentNullException.ThrowIfNull(configFileName);

        BaseDirectory = baseDirectory;
        _configFileName = configFileName;
        _repair = repair ?? (_ => false);
        _logger = logger ?? NullLogger<ConfigFile<T>>.Instance;

        Directory.CreateDirectory(BaseDirectory);
    }

    public string BaseDirectory { get; }

    private string ConfigPath => Path.Combine(BaseDirectory, _configFileName);

    private string BackupPath => ConfigPath + ".bad";

    /// <summary>
    /// Write the config file.
    /// </summary>
    /// <param name="config">The config to save.</param>
    public void WriteConfig(T config)
    {
        try
        {
            if (File.Exists(ConfigPath))
            {
                File.Delete(ConfigPath);
            }

            using FileStream stream = File.OpenWrite(ConfigPath);

            JsonSerializer.Serialize(stream, config, _writeOptions);
        }
        catch (Exception ex)
        {
            LogMessages.ConfigWriteFailed(_logger, ConfigPath, ex);
            Debug.Fail(ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Read the config file. If no config file exists yet (e.g. first run), the resulting
    /// defaults are written out immediately so a config file always appears under the
    /// user-data directory rather than only after the user first changes a setting.
    /// </summary>
    /// <returns>The settings held in the file, repaired where they could not be honoured.</returns>
    public T ReadConfig()
    {
        bool fileExisted = File.Exists(ConfigPath);

        try
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(BaseDirectory)
                .AddJsonFile(_configFileName, optional: true, reloadOnChange: false)
                .Build();

            T config = new();
            configuration.Bind(config);

            if (!fileExisted)
            {
                WriteConfig(config);
                return config;
            }

            if (_repair(config))
            {
                // The file is about to be rewritten without the values that
                // could not be honoured, so keep what the user actually had.
                LogMessages.ConfigRepaired(_logger, ConfigPath);
                BackUp();
                WriteConfig(config);
            }

            return config;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or FormatException or InvalidOperationException)
        {
            // Nothing bound, so there is nothing to keep from the file beyond
            // the copy taken here.
            LogMessages.ConfigReadFailed(_logger, ConfigPath);
            LogMessages.ConfigReadFailedDetail(_logger, ConfigPath, ex);
            BackUp();
        }

        return new();
    }

    private void BackUp()
    {
        try
        {
            if (File.Exists(ConfigPath))
            {
                File.Copy(ConfigPath, BackupPath, overwrite: true);
                LogMessages.ConfigBackedUp(_logger, BackupPath);
            }
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            // Best effort: failing to keep a copy must not stop the game from
            // starting with usable settings.
            LogMessages.ConfigBackupFailed(_logger, BackupPath, ex);
        }
    }
}
