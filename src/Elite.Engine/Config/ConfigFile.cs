/*
 * Elite - The New Kind.
 *
 * Reverse engineered from the BBC disk version of Elite.
 * Additional material by C.J.Pinder.
 *
 * The original Elite code is (C) I.Bell & D.Braben 1984.
 * This version re-engineered in C by C.J.Pinder 1999-2001.
 *
 * email: <christian@newkind.co.uk>
 *
 *
 */

using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using Elite.Engine.Config;

namespace Elite.Engine
{
    internal class ConfigFile
	{
		private const string ConfigFileName = "sharpkind.cfg";
        private readonly JsonSerializerOptions _options = new()
        {
            WriteIndented = true,
            Converters = { new JsonStringEnumConverter() },
        };

        /// <summary>
        /// Write the config file.
        /// </summary>
        /// <param name="config">The config to save.</param>
        internal async Task WriteConfigAsync(ConfigSettings config)
		{
            try
            {
                if (File.Exists(ConfigFileName))
                {
                    File.Delete(ConfigFileName);
                }
                using FileStream stream = File.OpenWrite(ConfigFileName);
                
                await JsonSerializer.SerializeAsync(stream, config, _options);
            }
            catch (Exception ex)
            {
                //TODO: handle error message better
                Debug.WriteLine("Failed to save config.\n" + ex);
            }
        }

		/// <summary>
		/// Read the config file.
		/// </summary>
		internal async Task<ConfigSettings> ReadConfigAsync()
		{
            try
            {
                using FileStream stream = File.OpenRead(ConfigFileName);
                ConfigSettings? config = await JsonSerializer.DeserializeAsync<ConfigSettings>(stream, _options);
                if (config != null)
                {
                    return config;
                }
            }
            catch (Exception ex)
            {
                //TODO: handle error message better
                Debug.WriteLine("Failed to read config.\n" + ex);
            }

            return new();
        }
    }
}
