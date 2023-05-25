// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using Elite.Engine.Enums;
using Elite.Engine.Ships;
using Elite.Engine.Trader;

namespace Elite.Engine.Views
{
    internal sealed class CommanderStatusView : IView
    {
        private readonly string[] _conditionText = new string[]
        {
                "Docked",
                "Green",
                "Yellow",
                "Red",
        };

        private readonly Draw _draw;
        private readonly int _equipmentMaxY = 290;
        private readonly int _equipmentStartY = 202;
        private readonly int _equipmentWidth = 200;
        private readonly GameState _gameState;
        private readonly IGraphics _graphics;
        private readonly PlanetController _planet;

        private readonly (int Score, string Title)[] _ratings = new (int Score, string Title)[]
        {
                new(0x0000, "Harmless"),
                new(0x0008, "Mostly Harmless"),
                new(0x0010, "Poor"),
                new(0x0020, "Average"),
                new(0x0040, "Above Average"),
                new(0x0080, "Competent"),
                new(0x0200, "Dangerous"),
                new(0x0A00, "Deadly"),
                new(0x1900, "- - - E L I T E - - -"),
        };

        private readonly PlayerShip _ship;
        private readonly int _spacingY = 16;
        private readonly Trade _trade;
        private readonly Universe _universe;

        internal CommanderStatusView(GameState gameState, IGraphics graphics, Draw draw, PlayerShip ship, Trade trade, PlanetController planet, Universe universe)
        {
            _gameState = gameState;
            _graphics = graphics;
            _draw = draw;
            _ship = ship;
            _trade = trade;
            _planet = planet;
            _universe = universe;
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
            }

            string rating = string.Empty;
            foreach ((int score, string title) in _ratings)
            {
                if (_gameState.Cmdr.Score >= score)
                {
                    rating = title;
                }
            }

            int condition = 0;

            if (!_gameState.IsDocked)
            {
                condition = 1;

                for (int i = 0; i < EliteMain.MaxUniverseObjects; i++)
                {
                    if (_universe.Objects[i].Type is ShipType.Missile or (> ShipType.Rock and < ShipType.Dodec))
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
            _graphics.DrawTextLeft(16, 58, "Present System:", Colour.Green);

            if (!_gameState.InWitchspace)
            {
                _graphics.DrawTextLeft(150, 58, _planet.NamePlanet(_gameState.DockedPlanet).CapitaliseFirstLetter(), Colour.White);
            }

            _graphics.DrawTextLeft(16, 74, "Hyperspace System:", Colour.Green);
            _graphics.DrawTextLeft(150, 74, _planet.NamePlanet(_gameState.HyperspacePlanet).CapitaliseFirstLetter(), Colour.White);

            _graphics.DrawTextLeft(16, 90, "Condition:", Colour.Green);
            _graphics.DrawTextLeft(150, 90, _conditionText[condition], Colour.White);

            _graphics.DrawTextLeft(16, 106, "Fuel:", Colour.Green);
            _graphics.DrawTextLeft(150, 106, $"{_ship.Fuel:N1} Light Years", Colour.White);

            _graphics.DrawTextLeft(16, 122, "Cash:", Colour.Green);
            _graphics.DrawTextLeft(150, 122, $"{_trade.Credits:N1} Credits", Colour.White);

            _graphics.DrawTextLeft(16, 138, "Legal Status:", Colour.Green);
            _graphics.DrawTextLeft(150, 138, _gameState.Cmdr.LegalStatus == 0 ? "Clean" : _gameState.Cmdr.LegalStatus > 50 ? "Fugitive" : "Offender", Colour.White);

            _graphics.DrawTextLeft(16, 154, "Rating:", Colour.Green);
            _graphics.DrawTextLeft(150, 154, rating, Colour.White);

            _graphics.DrawTextLeft(16, 186, "EQUIPMENT:", Colour.Green);

            if (_ship.CargoCapacity > 20)
            {
                _graphics.DrawTextLeft(x, y, "Large Cargo Bay", Colour.White);
                IncrementPosition();
            }

            if (_ship.HasEscapeCapsule)
            {
                _graphics.DrawTextLeft(x, y, "Escape Capsule", Colour.White);
                IncrementPosition();
            }

            if (_ship.HasFuelScoop)
            {
                _graphics.DrawTextLeft(x, y, "Fuel Scoops", Colour.White);
                IncrementPosition();
            }

            if (_ship.HasECM)
            {
                _graphics.DrawTextLeft(x, y, "E.C.M. System", Colour.White);
                IncrementPosition();
            }

            if (_ship.HasEnergyBomb)
            {
                _graphics.DrawTextLeft(x, y, "Energy Bomb", Colour.White);
                IncrementPosition();
            }

            if (_ship.EnergyUnit != EnergyUnit.None)
            {
                _graphics.DrawTextLeft(x, y, _ship.EnergyUnit == EnergyUnit.Extra ? "Extra Energy Unit" : "Naval Energy Unit", Colour.White);
                IncrementPosition();
            }

            if (_ship.HasDockingComputer)
            {
                _graphics.DrawTextLeft(x, y, "Docking Computers", Colour.White);
                IncrementPosition();
            }

            if (_ship.HasGalacticHyperdrive)
            {
                _graphics.DrawTextLeft(x, y, "Galactic Hyperspace", Colour.White);
                IncrementPosition();
            }

            if (_ship.LaserFront.Type != LaserType.None)
            {
                _graphics.DrawTextLeft(x, y, $"Front {_ship.LaserFront.Name} Laser", Colour.White);
                IncrementPosition();
            }

            if (_ship.LaserRear.Type != LaserType.None)
            {
                _graphics.DrawTextLeft(x, y, $"Rear {_ship.LaserRear.Name} Laser", Colour.White);
                IncrementPosition();
            }

            if (_ship.LaserLeft.Type != LaserType.None)
            {
                _graphics.DrawTextLeft(x, y, $"Left {_ship.LaserLeft.Name} Laser", Colour.White);
                IncrementPosition();
            }

            if (_ship.LaserRight.Type != LaserType.None)
            {
                _graphics.DrawTextLeft(x, y, $"Right {_ship.LaserRight.Name} Laser", Colour.White);
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
