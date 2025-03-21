// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EliteSharp.Config;

internal sealed class ConfigFile
{
    private const string ConfigFileName = "sharpkind.cfg";

    private readonly JsonSerializerOptions _options = new()
    {
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter() },
    };

    /// <summary>
    /// Read the config file.
    /// </summary>
    internal ConfigSettings ReadConfig()
    {
        try
        {
            using FileStream stream = File.OpenRead(ConfigFileName);
            ConfigSettings? config = JsonSerializer.Deserialize<ConfigSettings>(stream, _options);
            if (config != null)
            {
                return config;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine("Failed to read config.\n" + ex);
            Debug.Fail(ex.Message);
            throw;
        }

        return new();
    }

    /// <summary>
    /// Write the config file.
    /// </summary>
    /// <param name="config">The config to save.</param>
    internal void WriteConfig(ConfigSettings config)
    {
        try
        {
            if (File.Exists(ConfigFileName))
            {
                File.Delete(ConfigFileName);
            }

            using FileStream stream = File.OpenWrite(ConfigFileName);

            JsonSerializer.Serialize(stream, config, _options);
        }
        catch (Exception ex)
        {
            Debug.WriteLine("Failed to save config.\n" + ex);
            Debug.Fail(ex.Message);
            throw;
        }
    }
}
