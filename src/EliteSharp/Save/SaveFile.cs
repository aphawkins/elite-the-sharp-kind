// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using EliteSharp.Equipment;
using EliteSharp.Lasers;
using EliteSharp.Ships;
using EliteSharp.Trader;

namespace EliteSharp.Save
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

        private readonly PlanetController _planet;
        private readonly PlayerShip _ship;
        private readonly GameState _state;
        private readonly Trade _trade;
        private SaveState _lastSaved;

        internal SaveFile(GameState state, PlayerShip ship, Trade trade, PlanetController planet)
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

        internal bool LoadCommander(string name)
        {
            try
            {
                if (!File.Exists(name + FileExtension))
                {
                    return false;
                }

                using FileStream stream = File.OpenRead(name + FileExtension);
                SaveState? save = JsonSerializer.Deserialize<SaveState>(stream, _options);
                if (save != null)
                {
                    _lastSaved = save;
                    SaveStateToGameState();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to load commander.\n" + ex);
                _lastSaved = CommanderFactory.Jameson();
                throw;
            }

            return false;
        }

        internal bool SaveCommander(string newName)
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
                JsonSerializer.Serialize(stream, save, _options);

                _lastSaved = save;

                return true;
            }
            catch (IOException ex)
            {
                Debug.WriteLine("Failed to save commander.\n" + ex);
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to save commander.\n" + ex);
                Debug.Fail(ex.Message);
                throw;
            }
        }

        private SaveState GameStateToSaveState(string newName) => new()
        {
            CargoCapacity = _ship.CargoCapacity,
            CommanderName = newName,
            Credits = _trade.Credits,
            CurrentCargo = _trade.StockMarket.Values.Select(x => x.CurrentCargo).ToList(),
            EnergyUnit = _ship.EnergyUnit.ToString(),
            Fuel = _ship.Fuel,
            GalaxyNumber = _state.Cmdr.GalaxyNumber,
            GalaxySeed =
                [
                    _state.Cmdr.Galaxy.A,
                    _state.Cmdr.Galaxy.B,
                    _state.Cmdr.Galaxy.C,
                    _state.Cmdr.Galaxy.D,
                    _state.Cmdr.Galaxy.E,
                    _state.Cmdr.Galaxy.F,
                ],
            HasDockingComputer = _ship.HasDockingComputer,
            HasECM = _ship.HasECM,
            HasEnergyBomb = _ship.HasEnergyBomb,
            HasEscapeCapsule = _ship.HasEscapeCapsule,
            HasFuelScoop = _ship.HasFuelScoop,
            HasGalacticHyperdrive = _ship.HasGalacticHyperdrive,
            Lasers =
                [
                    _ship.LaserFront.Type.ToString(),
                    _ship.LaserRear.Type.ToString(),
                    _ship.LaserRight.Type.ToString(),
                    _ship.LaserLeft.Type.ToString(),
                ],
            LegalStatus = _state.Cmdr.LegalStatus,
            MarketRandomiser = _trade.MarketRandomiser,
            Missiles = _ship.MissileCount,
            Mission = _state.Cmdr.Mission,
            Saved = _state.Cmdr.Saved,
            Score = _state.Cmdr.Score,
            ShipLocation =
                [
                    _state.DockedPlanet.D,
                    _state.DockedPlanet.B,
                ],
            StationStock = _trade.StockMarket.Values.Select(x => x.StationStock).ToList(),
        };

        private void RestoreSavedCommander()
        {
            _state.DockedPlanet = _planet.FindPlanet(_state.Cmdr.Galaxy, new(_state.DockedPlanet.D, _state.DockedPlanet.B));
            _state.PlanetName = _planet.NamePlanet(_state.DockedPlanet);
            _state.HyperspacePlanet = new(_state.DockedPlanet);
            _state.CurrentPlanetData = PlanetController.GeneratePlanetData(_state.DockedPlanet);
            _trade.GenerateStockMarket();
            _trade.SetStockQuantities();
        }

        private void SaveStateToGameState()
        {
            _ship.CargoCapacity = _lastSaved.CargoCapacity;
            _state.Cmdr.Name = _lastSaved.CommanderName;
            _trade.Credits = _lastSaved.Credits;
            for (int i = 0; i < _trade.StockMarket.Count; i++)
            {
                _trade.StockMarket[(StockType)i + 1].CurrentCargo = _lastSaved.CurrentCargo[i];
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
            _trade.MarketRandomiser = _lastSaved.MarketRandomiser;
            _ship.MissileCount = _lastSaved.Missiles;
            _state.Cmdr.Mission = _lastSaved.Mission;
            _state.Cmdr.Saved = _lastSaved.Saved;
            _state.Cmdr.Score = _lastSaved.Score;
            _state.DockedPlanet.D = _lastSaved.ShipLocation[0];
            _state.DockedPlanet.B = _lastSaved.ShipLocation[1];
            for (int i = 0; i < _trade.StockMarket.Count; i++)
            {
                _trade.StockMarket[(StockType)i + 1].StationStock = _lastSaved.StationStock[i];
            }
        }
    }
}
