// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using Elite.Engine.Enums;
using Elite.Engine.Ships;

namespace Elite.Engine.Views
{
    internal class CommanderStatusView : IView
    {
        public readonly GameState _gameState;
        private readonly string[] _conditionText = new string[]
        {
                "Docked",
                "Green",
                "Yellow",
                "Red"
        };

        private readonly Draw _draw;
        private readonly int _equipmentMaxY = 290;
        private readonly int _equipmentStartY = 202;
        private readonly int _equipmentWidth = 200;
        private readonly IGfx _gfx;
        private readonly Planet _planet;
        private readonly (int score, string title)[] _ratings = new (int score, string title)[]
        {
                new(0x0000, "Harmless"),
                new(0x0008, "Mostly Harmless"),
                new(0x0010, "Poor"),
                new(0x0020, "Average"),
                new(0x0040, "Above Average"),
                new(0x0080, "Competent"),
                new(0x0200, "Dangerous"),
                new(0x0A00, "Deadly"),
                new(0x1900, "- - - E L I T E - - -")
        };

        private readonly PlayerShip _ship;
        private readonly int _spacingY = 16;
        private readonly Trade _trade;

        internal CommanderStatusView(GameState gameState, IGfx gfx, Draw draw, PlayerShip ship, Trade trade, Planet planet)
        {
            _gameState = gameState;
            _gfx = gfx;
            _draw = draw;
            _ship = ship;
            _trade = trade;
            _planet = planet;
        }

        public void Draw()
        {
            _draw.ClearDisplay();
            int x = 50;
            int y = _equipmentStartY;

            void IncrementPosition()
            {
                y += _spacingY;
                if (y > _equipmentMaxY)
                {
                    y = _equipmentStartY;
                    x += _equipmentWidth;
                }
            };

            string rating = string.Empty;
            foreach ((int score, string title) in _ratings)
            {
                if (_gameState.Cmdr.Score >= score)
                {
                    rating = title;
                }
            }

            string dockedPlanetName = _planet.NamePlanet(_gameState.DockedPlanet, true);
            string hyperspacePlanetName = _planet.NamePlanet(_gameState.HyperspacePlanet, true);

            int condition = 0;

            if (!_gameState.IsDocked)
            {
                condition = 1;

                for (int i = 0; i < EliteMain.MAX_UNIV_OBJECTS; i++)
                {
                    if (Space.universe[i].type is ShipType.Missile or (> ShipType.Rock and < ShipType.Dodec))
                    {
                        condition = 2;
                        break;
                    }
                }

                if (condition == 2 && _ship.Energy < 128)
                {
                    condition = 3;
                }
            }

            _draw.DrawViewHeader($"COMMANDER {_gameState.Cmdr.Name}");
            _gfx.DrawTextLeft(16, 58, "Present System:", GFX_COL.GFX_COL_GREEN_1);

            if (!_gameState.InWitchspace)
            {
                _gfx.DrawTextLeft(150, 58, dockedPlanetName, GFX_COL.GFX_COL_WHITE);
            }

            _gfx.DrawTextLeft(16, 74, "Hyperspace System:", GFX_COL.GFX_COL_GREEN_1);
            _gfx.DrawTextLeft(150, 74, hyperspacePlanetName, GFX_COL.GFX_COL_WHITE);

            _gfx.DrawTextLeft(16, 90, "Condition:", GFX_COL.GFX_COL_GREEN_1);
            _gfx.DrawTextLeft(150, 90, _conditionText[condition], GFX_COL.GFX_COL_WHITE);

            _gfx.DrawTextLeft(16, 106, "Fuel:", GFX_COL.GFX_COL_GREEN_1);
            _gfx.DrawTextLeft(150, 106, $"{_ship.Fuel:N1} Light Years", GFX_COL.GFX_COL_WHITE);

            _gfx.DrawTextLeft(16, 122, "Cash:", GFX_COL.GFX_COL_GREEN_1);
            _gfx.DrawTextLeft(150, 122, $"{_trade.credits:N1} Credits", GFX_COL.GFX_COL_WHITE);

            _gfx.DrawTextLeft(16, 138, "Legal Status:", GFX_COL.GFX_COL_GREEN_1);
            _gfx.DrawTextLeft(150, 138, _gameState.Cmdr.LegalStatus == 0 ? "Clean" : _gameState.Cmdr.LegalStatus > 50 ? "Fugitive" : "Offender", GFX_COL.GFX_COL_WHITE);

            _gfx.DrawTextLeft(16, 154, "Rating:", GFX_COL.GFX_COL_GREEN_1);
            _gfx.DrawTextLeft(150, 154, rating, GFX_COL.GFX_COL_WHITE);

            _gfx.DrawTextLeft(16, 186, "EQUIPMENT:", GFX_COL.GFX_COL_GREEN_1);

            if (_ship.CargoCapacity > 20)
            {
                _gfx.DrawTextLeft(x, y, "Large Cargo Bay", GFX_COL.GFX_COL_WHITE);
                IncrementPosition();
            }

            if (_ship.HasEscapePod)
            {
                _gfx.DrawTextLeft(x, y, "Escape Pod", GFX_COL.GFX_COL_WHITE);
                IncrementPosition();
            }

            if (_ship.HasFuelScoop)
            {
                _gfx.DrawTextLeft(x, y, "Fuel Scoops", GFX_COL.GFX_COL_WHITE);
                IncrementPosition();
            }

            if (_ship.HasECM)
            {
                _gfx.DrawTextLeft(x, y, "E.C.M. System", GFX_COL.GFX_COL_WHITE);
                IncrementPosition();
            }

            if (_ship.HasEnergyBomb)
            {
                _gfx.DrawTextLeft(x, y, "Energy Bomb", GFX_COL.GFX_COL_WHITE);
                IncrementPosition();
            }

            if (_ship.EnergyUnit != EnergyUnit.None)
            {
                _gfx.DrawTextLeft(x, y, _ship.EnergyUnit == EnergyUnit.Extra ? "Extra Energy Unit" : "Naval Energy Unit", GFX_COL.GFX_COL_WHITE);
                IncrementPosition();
            }

            if (_ship.HasDockingComputer)
            {
                _gfx.DrawTextLeft(x, y, "Docking Computers", GFX_COL.GFX_COL_WHITE);
                IncrementPosition();
            }

            if (_ship.hasGalacticHyperdrive)
            {
                _gfx.DrawTextLeft(x, y, "Galactic Hyperspace", GFX_COL.GFX_COL_WHITE);
                IncrementPosition();
            }

            if (_ship.LaserFront.Type != LaserType.None)
            {
                _gfx.DrawTextLeft(x, y, $"Front {_ship.LaserFront.Name} Laser", GFX_COL.GFX_COL_WHITE);
                IncrementPosition();
            }

            if (_ship.LaserRear.Type != LaserType.None)
            {
                _gfx.DrawTextLeft(x, y, $"Rear {_ship.LaserRear.Name} Laser", GFX_COL.GFX_COL_WHITE);
                IncrementPosition();
            }

            if (_ship.LaserLeft.Type != LaserType.None)
            {
                _gfx.DrawTextLeft(x, y, $"Left {_ship.LaserLeft.Name} Laser", GFX_COL.GFX_COL_WHITE);
                IncrementPosition();
            }

            if (_ship.LaserRight.Type != LaserType.None)
            {
                _gfx.DrawTextLeft(x, y, $"Right {_ship.LaserRight.Name} Laser", GFX_COL.GFX_COL_WHITE);
            }
        }

        public void HandleInput()
        {
        }

        public void Reset()
        {
        }

        public void UpdateUniverse()
        {
        }
    }
}
