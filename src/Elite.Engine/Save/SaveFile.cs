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

namespace Elite.Engine.Save
{
    using System.Diagnostics;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using Elite.Engine.Types;

    internal static class SaveFile
    {
        private const string fileExtension = ".cmdr";
        private static readonly JsonSerializerOptions options = new()
        {
            WriteIndented = true,
            Converters = { new JsonStringEnumConverter() },
        };

        /// <summary>
        /// Write the config file.
        /// </summary>
        /// <param name="config">The config to save.</param>
        internal static async Task<bool> SaveCommanderAsync(Commander config)
        {
            try
            {
                string path = config.name + fileExtension;
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
                using FileStream stream = File.OpenWrite(path);
                await JsonSerializer.SerializeAsync(stream, config, options);
                return true;
            }
            catch (Exception ex)
            {
                //TODO: handle error message better
                Debug.WriteLine("Failed to save commander.\n" + ex);
                return false;
            }
        }

        /// <summary>
        /// Read the config file.
        /// </summary>
        internal static async Task<Commander?> LoadCommanderAsync(string name)
        {
            try
            {
                using FileStream stream = File.OpenRead(name + fileExtension);
                Commander? cmdr = await JsonSerializer.DeserializeAsync<Commander>(stream, options);
                if (cmdr != null)
                {
                    return (Commander)cmdr;
                }
            }
            catch (Exception ex)
            {
                //TODO: handle error message better
                Debug.WriteLine("Failed to load commander.\n" + ex);
            }

            return null;
        }


        //static int checksum(unsigned char* block)
        //{
        //	int acc, carry;
        //	int i;

        //	acc = 0x49;
        //	carry = 0;
        //	for (i = 0x49; i > 0; i--)
        //	{
        //		acc += block[i - 1] + carry;
        //		carry = acc >> 8;
        //		acc &= 255;
        //		acc ^= block[i];
        //	}

        //	return acc;
        //}

    }
}