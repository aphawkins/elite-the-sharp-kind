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
    using Elite.Engine.Enums;
    using Elite.Engine.Types;

    internal static class SaveFile
    {
        private const string fileExtension = ".cmdr";
        private static readonly JsonSerializerOptions options = new()
        {
            WriteIndented = true,
            Converters = { new JsonStringEnumConverter() },
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };

        /// <summary>
        /// Write the config file.
        /// </summary>
        /// <param name="state">The config to save.</param>
        internal static async Task<bool> SaveCommanderAsync(GameState state)
        {
            try
            {
                SaveState save = GameStateToSaveState(state);

                string path = save.CommanderName + fileExtension;
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
                using FileStream stream = File.OpenWrite(path);
                await JsonSerializer.SerializeAsync(stream, save, options);
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
        internal static async Task<bool> LoadCommanderAsync(string name, GameState state)
        {
            try
            {
                using FileStream stream = File.OpenRead(name + fileExtension);
                SaveState? save = await JsonSerializer.DeserializeAsync<SaveState>(stream, options);
                if (save != null)
                {
                    SaveStateToGameState(save, state);
                    return true;
                }
            }
            catch (Exception ex)
            {
                //TODO: handle error message better
                Debug.WriteLine("Failed to load commander.\n" + ex);
                state.saved_cmdr = CommanderFactory.Jameson();
            }

            return false;
        }

        private static SaveState GameStateToSaveState(GameState state)
        {
            SaveState save = new SaveState();
            save.CargoCapacity = state.cmdr.cargo_capacity;
            save.CommanderName = state.cmdr.name;
            save.Credits = state.cmdr.credits;
            save.CurrentCargo = state.cmdr.current_cargo;
            save.EnergyUnit = state.cmdr.energy_unit.ToString();
            save.Fuel = state.cmdr.fuel;
            save.GalaxyNumber = state.cmdr.galaxy_number;
            save.GalaxySeed = new int[6] 
            { 
                state.cmdr.galaxy.a, 
                state.cmdr.galaxy.b, 
                state.cmdr.galaxy.c, 
                state.cmdr.galaxy.d, 
                state.cmdr.galaxy.e, 
                state.cmdr.galaxy.f 
            };
            save.HasDockingComputer = state.cmdr.docking_computer;
            save.HasECM = state.cmdr.ecm;
            save.HasEnergyBomb = state.cmdr.energy_bomb;
            save.HasEscapePod = state.cmdr.escape_pod;
            save.HasFuelScoop = state.cmdr.fuel_scoop;
            save.HasGalacticHyperdrive = state.cmdr.galactic_hyperdrive;
            save.Lasers = new string[4] 
            { 
                state.cmdr.front_laser.Type.ToString(),
                state.cmdr.rear_laser.Type.ToString(),
                state.cmdr.right_laser.Type.ToString(),
                state.cmdr.left_laser.Type.ToString()
            };
            save.LegalStatus = state.cmdr.legal_status;
            save.MarketRandomiser = state.cmdr.market_rnd;
            save.Missiles = state.cmdr.missiles;
            save.Mission = state.cmdr.mission;
            save.Saved = state.cmdr.saved;
            save.Score = state.cmdr.score;
            save.ShipLocation = new float[2] 
            { 
                state.cmdr.ShipLocationX, 
                state.cmdr.ShipLocationY 
            };
            save.StationStock = state.cmdr.station_stock;

            return save;
        }

        private static void SaveStateToGameState(SaveState save, GameState state)
        {
            state.saved_cmdr.cargo_capacity = save.CargoCapacity;
            state.saved_cmdr.name = save.CommanderName;
            state.saved_cmdr.credits = save.Credits;
            state.saved_cmdr.current_cargo = save.CurrentCargo;
            state.saved_cmdr.energy_unit = Enum.Parse<EnergyUnit>(save.EnergyUnit);
            state.saved_cmdr.fuel = save.Fuel;
            state.saved_cmdr.galaxy_number = save.GalaxyNumber;
            state.saved_cmdr.galaxy.a = save.GalaxySeed[0];
            state.saved_cmdr.galaxy.b = save.GalaxySeed[1];
            state.saved_cmdr.galaxy.c = save.GalaxySeed[2];
            state.saved_cmdr.galaxy.d = save.GalaxySeed[3];
            state.saved_cmdr.galaxy.e = save.GalaxySeed[4];
            state.saved_cmdr.galaxy.f = save.GalaxySeed[5];
            state.saved_cmdr.docking_computer = save.HasDockingComputer;
            state.saved_cmdr.ecm = save.HasECM;
            state.saved_cmdr.energy_bomb = save.HasEnergyBomb;
            state.saved_cmdr.escape_pod = save.HasEscapePod;
            state.saved_cmdr.fuel_scoop = save.HasFuelScoop;
            state.saved_cmdr.galactic_hyperdrive = save.HasGalacticHyperdrive;
            state.saved_cmdr.front_laser = LaserFactory.GetLaser(Enum.Parse<LaserType>(save.Lasers[0]));
            state.saved_cmdr.rear_laser = LaserFactory.GetLaser(Enum.Parse<LaserType>(save.Lasers[1]));
            state.saved_cmdr.right_laser = LaserFactory.GetLaser(Enum.Parse<LaserType>(save.Lasers[2]));
            state.saved_cmdr.left_laser = LaserFactory.GetLaser(Enum.Parse<LaserType>(save.Lasers[3]));
            state.saved_cmdr.legal_status = save.LegalStatus;
            state.saved_cmdr.market_rnd = save.MarketRandomiser;
            state.saved_cmdr.missiles = save.Missiles;
            state.saved_cmdr.mission = save.Mission;
            state.saved_cmdr.saved = save.Saved;
            state.saved_cmdr.score = save.Score;
            state.saved_cmdr.ShipLocationX = save.ShipLocation[0];
            state.saved_cmdr.ShipLocationY = save.ShipLocation[1];
            state.saved_cmdr.station_stock = save.StationStock;
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