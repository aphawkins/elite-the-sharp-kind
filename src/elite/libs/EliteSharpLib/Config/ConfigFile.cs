// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;

namespace EliteSharpLib.Config;

internal sealed class ConfigFile : IConfigWriter
{
    private const string ConfigFileName = "elitesharp.cfg";

    private readonly JsonSerializerOptions _writeOptions = new()
    {
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter() },
    };

    internal ConfigFile(string baseDirectory)
    {
        BaseDirectory = baseDirectory;
        Directory.CreateDirectory(BaseDirectory);
    }

    internal string BaseDirectory { get; }

    private string ConfigPath => Path.Combine(BaseDirectory, ConfigFileName);

    /// <summary>
    /// Write the config file.
    /// </summary>
    /// <param name="config">The config to save.</param>
    public void WriteConfig(ConfigSettings config)
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
            Debug.WriteLine("Failed to save config.\n" + ex);
            Debug.Fail(ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Read the config file.
    /// </summary>
    internal ConfigSettings ReadConfig()
    {
        try
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(BaseDirectory)
                .AddJsonFile(ConfigFileName, optional: true, reloadOnChange: false)
                .Build();

            ConfigSettings config = new();
            configuration.Bind(config);

            if (IsValid(config))
            {
                return config;
            }

            Debug.WriteLine("Config file failed validation; using defaults.");
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or FormatException)
        {
            Debug.WriteLine("Failed to read config.\n" + ex);
        }

        return new();
    }

    private static bool IsValid(ConfigSettings config) => config.Fps > 0 &&
        Enum.IsDefined(config.PlanetDescriptions) &&
        Enum.IsDefined(config.PlanetStyle) &&
        Enum.IsDefined(config.ShipRenderMode) &&
        Enum.IsDefined(config.SunStyle);
}
