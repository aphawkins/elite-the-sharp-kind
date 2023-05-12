// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using Elite.Engine.Enums;
using Elite.Engine.Lasers;
using Elite.Engine.Ships;
using Elite.Engine.Trader;
using Elite.Engine.Types;

namespace Elite.Engine.Save
{
    internal sealed class SaveFile
    {
        private const string FileExtension = ".cmdr";

        private readonly JsonSerializerOptions _options = new()
        {
            WriteIndented = true,
            Converters = { new JsonStringEnumConverter() },
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };

        private readonly Planet _planet;
        private readonly PlayerShip _ship;
        private readonly GameState _state;
        private readonly Trade _trade;
        private SaveState _lastSaved;

        internal SaveFile(GameState state, PlayerShip ship, Trade trade, Planet planet)
        {
            _state = state;
            _ship = ship;
            _trade = trade;
            _planet = planet;

#if DEBUG
            _lastSaved = CommanderFactory.Max();
#else
		    _lastSaved = CommanderFactory.Jameson();
#endif
        }

        internal void GetLastSave()
        {
            SaveStateToGameState();
            RestoreSavedCommander();
        }

        /// <summary>
        /// Read the save file.
        /// </summary>
        internal async Task<bool> LoadCommanderAsync(string name)
        {
            try
            {
                using FileStream stream = File.OpenRead(name + FileExtension);
                SaveState? save = await JsonSerializer.DeserializeAsync<SaveState>(stream, _options).ConfigureAwait(false);
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

        /// <summary>
        /// Write the save file.
        /// </summary>
        /// <param name="state">The game state to save.</param>
        internal async Task<bool> SaveCommanderAsync(string newName)
        {
            try
            {
                SaveState save = GameStateToSaveState(newName);

                string path = save.CommanderName + FileExtension;
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
                using FileStream stream = File.OpenWrite(path);
                await JsonSerializer.SerializeAsync(stream, save, _options).ConfigureAwait(false);

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

        private SaveState GameStateToSaveState(string newName)
        {
            SaveState save = new()
            {
                CargoCapacity = _ship.CargoCapacity,
                CommanderName = newName,
                Credits = _trade._credits,
                CurrentCargo = _trade._stockMarket.Values.Select(x => x.CurrentCargo).ToArray(),
                EnergyUnit = _ship.EnergyUnit.ToString(),
                Fuel = _ship.Fuel,
                GalaxyNumber = _state.Cmdr.GalaxyNumber,
                GalaxySeed = new int[6]
                {
                    _state.Cmdr.Galaxy.A,
                    _state.Cmdr.Galaxy.B,
                    _state.Cmdr.Galaxy.C,
                    _state.Cmdr.Galaxy.D,
                    _state.Cmdr.Galaxy.E,
                    _state.Cmdr.Galaxy.F,
                },
                HasDockingComputer = _ship.HasDockingComputer,
                HasECM = _ship.HasECM,
                HasEnergyBomb = _ship.HasEnergyBomb,
                HasEscapeCapsule = _ship.HasEscapeCapsule,
                HasFuelScoop = _ship.HasFuelScoop,
                HasGalacticHyperdrive = _ship.HasGalacticHyperdrive,
                Lasers = new string[4]
                {
                    _ship.LaserFront.Type.ToString(),
                    _ship.LaserRear.Type.ToString(),
                    _ship.LaserRight.Type.ToString(),
                    _ship.LaserLeft.Type.ToString(),
                },
                LegalStatus = _state.Cmdr.LegalStatus,
                MarketRandomiser = _trade._marketRandomiser,
                Missiles = _ship.MissileCount,
                Mission = _state.Cmdr.Mission,
                Saved = _state.Cmdr.Saved,
                Score = _state.Cmdr.Score,
                ShipLocation = new int[2]
                {
                    _state.DockedPlanet.D,
                    _state.DockedPlanet.B,
                },
                StationStock = _trade._stockMarket.Values.Select(x => x.StationStock).ToArray(),
            };

            return save;
        }

        private void RestoreSavedCommander()
        {
            _state.DockedPlanet = _planet.FindPlanet(_state.Cmdr.Galaxy, new(_state.DockedPlanet.D, _state.DockedPlanet.B));
            _state.PlanetName = _planet.NamePlanet(_state.DockedPlanet, false);
            _state.HyperspacePlanet = (GalaxySeed)_state.DockedPlanet.Clone();
            _state.CurrentPlanetData = Planet.GeneratePlanetData(_state.DockedPlanet);
            _trade.GenerateStockMarket(_state.CurrentPlanetData);
            _trade.SetStockQuantities();
        }

        private void SaveStateToGameState()
        {
            _ship.CargoCapacity = _lastSaved.CargoCapacity;
            _state.Cmdr.Name = _lastSaved.CommanderName;
            _trade._credits = _lastSaved.Credits;
            for (int i = 0; i < _trade._stockMarket.Count; i++)
            {
                _trade._stockMarket[(StockType)i + 1].CurrentCargo = _lastSaved.CurrentCargo[i];
            }
            _ship.EnergyUnit = Enum.Parse<EnergyUnit>(_lastSaved.EnergyUnit);
            _ship.Fuel = _lastSaved.Fuel;
            _state.Cmdr.GalaxyNumber = _lastSaved.GalaxyNumber;
            _state.Cmdr.Galaxy.A = _lastSaved.GalaxySeed[0];
            _state.Cmdr.Galaxy.B = _lastSaved.GalaxySeed[1];
            _state.Cmdr.Galaxy.C = _lastSaved.GalaxySeed[2];
            _state.Cmdr.Galaxy.D = _lastSaved.GalaxySeed[3];
            _state.Cmdr.Galaxy.E = _lastSaved.GalaxySeed[4];
            _state.Cmdr.Galaxy.F = _lastSaved.GalaxySeed[5];
            _ship.HasDockingComputer = _lastSaved.HasDockingComputer;
            _ship.HasECM = _lastSaved.HasECM;
            _ship.HasEnergyBomb = _lastSaved.HasEnergyBomb;
            _ship.HasEscapeCapsule = _lastSaved.HasEscapeCapsule;
            _ship.HasFuelScoop = _lastSaved.HasFuelScoop;
            _ship.HasGalacticHyperdrive = _lastSaved.HasGalacticHyperdrive;
            _ship.LaserFront = LaserFactory.GetLaser(Enum.Parse<LaserType>(_lastSaved.Lasers[0]));
            _ship.LaserRear = LaserFactory.GetLaser(Enum.Parse<LaserType>(_lastSaved.Lasers[1]));
            _ship.LaserRight = LaserFactory.GetLaser(Enum.Parse<LaserType>(_lastSaved.Lasers[2]));
            _ship.LaserLeft = LaserFactory.GetLaser(Enum.Parse<LaserType>(_lastSaved.Lasers[3]));
            _state.Cmdr.LegalStatus = _lastSaved.LegalStatus;
            _trade._marketRandomiser = _lastSaved.MarketRandomiser;
            _ship.MissileCount = _lastSaved.Missiles;
            _state.Cmdr.Mission = _lastSaved.Mission;
            _state.Cmdr.Saved = _lastSaved.Saved;
            _state.Cmdr.Score = _lastSaved.Score;
            _state.DockedPlanet.D = _lastSaved.ShipLocation[0];
            _state.DockedPlanet.B = _lastSaved.ShipLocation[1];
            for (int i = 0; i < _trade._stockMarket.Count; i++)
            {
                _trade._stockMarket[(StockType)i + 1].StationStock = _lastSaved.StationStock[i];
            }
        }

        //static int checksum(unsigned char* block)
        //{
        //  int acc, carry;
        //  int i;

        //  acc = 0x49;
        //  carry = 0;
        //  for (i = 0x49; i > 0; i--)
        //  {
        //      acc += block[i - 1] + carry;
        //      carry = acc >> 8;
        //      acc &= 255;
        //      acc ^= block[i];
        //  }

        //  return acc;
        //}
    }
}
