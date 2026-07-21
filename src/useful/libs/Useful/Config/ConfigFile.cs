// 'Useful Libraries' - Andy Hawkins 2025.

using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Useful.Config;

/// <summary>
/// Reads and writes a JSON-backed settings file of type <typeparamref name="T"/> from a
/// user-data directory. Falls back to <c>new T()</c> if the file is missing, malformed, or
/// (when a validation predicate is supplied) fails validation.
/// </summary>
/// <typeparam name="T">The settings type. Must be default-constructible so a missing or invalid file can fall back to defaults.</typeparam>
public sealed class ConfigFile<T> : IConfigWriter<T>
    where T : new()
{
    private readonly string _configFileName;
    private readonly Func<T, bool> _isValid;
    private readonly ILogger<ConfigFile<T>> _logger;

    private readonly JsonSerializerOptions _writeOptions = new()
    {
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter() },
    };

    public ConfigFile(string baseDirectory, string configFileName)
        : this(baseDirectory, configFileName, null, null)
    {
    }

    public ConfigFile(string baseDirectory, string configFileName, Func<T, bool>? isValid)
        : this(baseDirectory, configFileName, isValid, null)
    {
    }

    public ConfigFile(string baseDirectory, string configFileName, Func<T, bool>? isValid, ILogger<ConfigFile<T>>? logger)
    {
        Guard.ArgumentNull(baseDirectory);
        Guard.ArgumentNull(configFileName);

        BaseDirectory = baseDirectory;
        _configFileName = configFileName;
        _isValid = isValid ?? (_ => true);
        _logger = logger ?? NullLogger<ConfigFile<T>>.Instance;

        Directory.CreateDirectory(BaseDirectory);
    }

    public string BaseDirectory { get; }

    private string ConfigPath => Path.Combine(BaseDirectory, _configFileName);

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
    /// Read the config file.
    /// </summary>
    public T ReadConfig()
    {
        try
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(BaseDirectory)
                .AddJsonFile(_configFileName, optional: true, reloadOnChange: false)
                .Build();

            T config = new();
            configuration.Bind(config);

            if (_isValid(config))
            {
                return config;
            }

            LogMessages.ConfigValidationFailed(_logger, ConfigPath);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or FormatException or InvalidOperationException)
        {
            LogMessages.ConfigReadFailed(_logger, ConfigPath);
            LogMessages.ConfigReadFailedDetail(_logger, ConfigPath, ex);
        }

        return new();
    }
}
