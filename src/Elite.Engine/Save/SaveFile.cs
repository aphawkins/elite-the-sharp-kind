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

    internal class SaveFile
    {
        private const string fileExtension = ".cmdr";
        private readonly GameState _state;
        private SaveState _lastSaved;

        private static readonly JsonSerializerOptions options = new()
        {
            WriteIndented = true,
            Converters = { new JsonStringEnumConverter() },
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };

        internal SaveFile(GameState state)
        {
            _state = state;

#if DEBUG
            _lastSaved = CommanderFactory.Max();
#else
		    _lastSaved = CommanderFactory.Jameson();
#endif
        }

        /// <summary>
        /// Write the save file.
        /// </summary>
        /// <param name="state">The game state to save.</param>
        internal async Task<bool> SaveCommanderAsync(string newName)
        {
            try
            {
                SaveState save = GameStateToSaveState(newName);

                string path = save.CommanderName + fileExtension;
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
                using FileStream stream = File.OpenWrite(path);
                await JsonSerializer.SerializeAsync(stream, save, options);

                _lastSaved = save;
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
        /// Read the save file.
        /// </summary>
        internal async Task<bool> LoadCommanderAsync(string name)
        {
            try
            {
                using FileStream stream = File.OpenRead(name + fileExtension);
                SaveState? save = await JsonSerializer.DeserializeAsync<SaveState>(stream, options);
                if (save != null)
                {
                    _lastSaved = save;
                    SaveStateToGameState();
                    return true;
                }
            }
            catch (Exception ex)
            {
                //TODO: handle error message better
                Debug.WriteLine("Failed to load commander.\n" + ex);
                _lastSaved = CommanderFactory.Jameson();
            }

            return false;
        }

        internal void GetLastSave()
        {
            SaveStateToGameState();
        }

        private SaveState GameStateToSaveState(string newName)
        {
            SaveState save = new()
            {
                CargoCapacity = _state.cmdr.cargo_capacity,
                CommanderName = newName,
                Credits = _state.cmdr.credits,
                CurrentCargo = _state.cmdr.current_cargo,
                EnergyUnit = _state.cmdr.energy_unit.ToString(),
                Fuel = _state.cmdr.fuel,
                GalaxyNumber = _state.cmdr.galaxy_number,
                GalaxySeed = new int[6]
                {
                    _state.cmdr.galaxy.a,
                    _state.cmdr.galaxy.b,
                    _state.cmdr.galaxy.c,
                    _state.cmdr.galaxy.d,
                    _state.cmdr.galaxy.e,
                    _state.cmdr.galaxy.f
                },
                HasDockingComputer = _state.cmdr.docking_computer,
                HasECM = _state.cmdr.ecm,
                HasEnergyBomb = _state.cmdr.energy_bomb,
                HasEscapePod = _state.cmdr.escape_pod,
                HasFuelScoop = _state.cmdr.fuel_scoop,
                HasGalacticHyperdrive = _state.cmdr.galactic_hyperdrive,
                Lasers = new string[4]
                {
                    _state.cmdr.front_laser.Type.ToString(),
                    _state.cmdr.rear_laser.Type.ToString(),
                    _state.cmdr.right_laser.Type.ToString(),
                    _state.cmdr.left_laser.Type.ToString()
                },
                LegalStatus = _state.cmdr.legal_status,
                MarketRandomiser = _state.cmdr.market_rnd,
                Missiles = _state.cmdr.missiles,
                Mission = _state.cmdr.mission,
                Saved = _state.cmdr.saved,
                Score = _state.cmdr.score,
                ShipLocation = new int[2]
                {
                    _state.docked_planet.d,
                    _state.docked_planet.b,
                },
                StationStock = _state.cmdr.station_stock
            };

            return save;
        }

        private void SaveStateToGameState()
        {
            _state.cmdr.cargo_capacity = _lastSaved.CargoCapacity;
            _state.cmdr.name = _lastSaved.CommanderName;
            _state.cmdr.credits = _lastSaved.Credits;
            _state.cmdr.current_cargo = _lastSaved.CurrentCargo;
            _state.cmdr.energy_unit = Enum.Parse<EnergyUnit>(_lastSaved.EnergyUnit);
            _state.cmdr.fuel = _lastSaved.Fuel;
            _state.cmdr.galaxy_number = _lastSaved.GalaxyNumber;
            _state.cmdr.galaxy.a = _lastSaved.GalaxySeed[0];
            _state.cmdr.galaxy.b = _lastSaved.GalaxySeed[1];
            _state.cmdr.galaxy.c = _lastSaved.GalaxySeed[2];
            _state.cmdr.galaxy.d = _lastSaved.GalaxySeed[3];
            _state.cmdr.galaxy.e = _lastSaved.GalaxySeed[4];
            _state.cmdr.galaxy.f = _lastSaved.GalaxySeed[5];
            _state.cmdr.docking_computer = _lastSaved.HasDockingComputer;
            _state.cmdr.ecm = _lastSaved.HasECM;
            _state.cmdr.energy_bomb = _lastSaved.HasEnergyBomb;
            _state.cmdr.escape_pod = _lastSaved.HasEscapePod;
            _state.cmdr.fuel_scoop = _lastSaved.HasFuelScoop;
            _state.cmdr.galactic_hyperdrive = _lastSaved.HasGalacticHyperdrive;
            _state.cmdr.front_laser = LaserFactory.GetLaser(Enum.Parse<LaserType>(_lastSaved.Lasers[0]));
            _state.cmdr.rear_laser = LaserFactory.GetLaser(Enum.Parse<LaserType>(_lastSaved.Lasers[1]));
            _state.cmdr.right_laser = LaserFactory.GetLaser(Enum.Parse<LaserType>(_lastSaved.Lasers[2]));
            _state.cmdr.left_laser = LaserFactory.GetLaser(Enum.Parse<LaserType>(_lastSaved.Lasers[3]));
            _state.cmdr.legal_status = _lastSaved.LegalStatus;
            _state.cmdr.market_rnd = _lastSaved.MarketRandomiser;
            _state.cmdr.missiles = _lastSaved.Missiles;
            _state.cmdr.mission = _lastSaved.Mission;
            _state.cmdr.saved = _lastSaved.Saved;
            _state.cmdr.score = _lastSaved.Score;
            _state.docked_planet.d = _lastSaved.ShipLocation[0];
            _state.docked_planet.b = _lastSaved.ShipLocation[1];
            _state.cmdr.station_stock = _lastSaved.StationStock;
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