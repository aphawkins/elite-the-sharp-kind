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

namespace Elite
{
    using System.Diagnostics;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using Elite.Config;

    internal class ConfigFile
	{
		private const string configFileName = "sharpkind.cfg";
        private static readonly JsonSerializerOptions options = new()
        {
            WriteIndented = true,
            Converters = { new JsonStringEnumConverter() },
        };

        /// <summary>
        /// Write the config file.
        /// </summary>
        /// <param name="config">The config to save.</param>
        internal static async Task WriteConfigAsync(ConfigSettings config)
		{
            try
            {
                using FileStream stream = File.OpenWrite(configFileName);
                
                await JsonSerializer.SerializeAsync(stream, config, options);
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
		internal static async Task<ConfigSettings> ReadConfigAsync()
		{
            try
            {
                using FileStream stream = File.OpenRead(configFileName);
                ConfigSettings? config = await JsonSerializer.DeserializeAsync<ConfigSettings>(stream, options);
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