// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using EliteSharp.Equipment;
using EliteSharp.Graphics;
using EliteSharp.Lasers;
using EliteSharp.Ships;
using EliteSharp.Trader;

namespace EliteSharp.Views
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

        private readonly IDraw _draw;
        private readonly int _equipmentMaxY = 290;
        private readonly int _equipmentStartY = 202;
        private readonly int _equipmentWidth = 200;
        private readonly GameState _gameState;
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

        internal CommanderStatusView(GameState gameState, IDraw draw, PlayerShip ship, Trade trade, PlanetController planet, Universe universe)
        {
            _gameState = gameState;
            _draw = draw;
            _ship = ship;
            _trade = trade;
            _planet = planet;
            _universe = universe;
        }

        public void Draw()
        {
            Vector2 position = new(50 + _draw.Offset, _equipmentStartY);

            void IncrementPosition()
            {
                position.Y += _spacingY;
                if (position.Y > _equipmentMaxY)
                {
                    position.Y = _equipmentStartY;
                    position.X += _equipmentWidth;
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

                foreach (IShip universeObj in _universe.GetAllObjects())
                {
                    if (universeObj.Type is ShipType.Missile or (> ShipType.Rock and < ShipType.Dodec))
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

            _draw.Graphics.DrawTextLeft(new(16 + _draw.Offset, 58), "Present System:", Colour.Green);

            if (!_gameState.InWitchspace)
            {
                _draw.Graphics.DrawTextLeft(new(150 + _draw.Offset, 58), _planet.NamePlanet(_gameState.DockedPlanet).CapitaliseFirstLetter(), Colour.White);
            }

            _draw.Graphics.DrawTextLeft(new(16 + _draw.Offset, 74), "Hyperspace System:", Colour.Green);
            _draw.Graphics.DrawTextLeft(new(150 + _draw.Offset, 74), _planet.NamePlanet(_gameState.HyperspacePlanet).CapitaliseFirstLetter(), Colour.White);

            _draw.Graphics.DrawTextLeft(new(16 + _draw.Offset, 90), "Condition:", Colour.Green);
            _draw.Graphics.DrawTextLeft(new(150 + _draw.Offset, 90), _conditionText[condition], Colour.White);

            _draw.Graphics.DrawTextLeft(new(16 + _draw.Offset, 106), "Fuel:", Colour.Green);
            _draw.Graphics.DrawTextLeft(new(150 + _draw.Offset, 106), $"{_ship.Fuel:N1} Light Years", Colour.White);

            _draw.Graphics.DrawTextLeft(new(16 + _draw.Offset, 122), "Cash:", Colour.Green);
            _draw.Graphics.DrawTextLeft(new(150 + _draw.Offset, 122), $"{_trade.Credits:N1} Credits", Colour.White);

            _draw.Graphics.DrawTextLeft(new(16 + _draw.Offset, 138), "Legal Status:", Colour.Green);
            _draw.Graphics.DrawTextLeft(new(150 + _draw.Offset, 138), _gameState.Cmdr.LegalStatus == 0 ? "Clean" : _gameState.Cmdr.LegalStatus > 50 ? "Fugitive" : "Offender", Colour.White);

            _draw.Graphics.DrawTextLeft(new(16 + _draw.Offset, 154), "Rating:", Colour.Green);
            _draw.Graphics.DrawTextLeft(new(150 + _draw.Offset, 154), rating, Colour.White);

            _draw.Graphics.DrawTextLeft(new(16 + _draw.Offset, 186), "EQUIPMENT:", Colour.Green);

            if (_ship.CargoCapacity > 20)
            {
                _draw.Graphics.DrawTextLeft(position, "Large Cargo Bay", Colour.White);
                IncrementPosition();
            }

            if (_ship.HasEscapeCapsule)
            {
                _draw.Graphics.DrawTextLeft(position, "Escape Capsule", Colour.White);
                IncrementPosition();
            }

            if (_ship.HasFuelScoop)
            {
                _draw.Graphics.DrawTextLeft(position, "Fuel Scoops", Colour.White);
                IncrementPosition();
            }

            if (_ship.HasECM)
            {
                _draw.Graphics.DrawTextLeft(position, "E.C.M. System", Colour.White);
                IncrementPosition();
            }

            if (_ship.HasEnergyBomb)
            {
                _draw.Graphics.DrawTextLeft(position, "Energy Bomb", Colour.White);
                IncrementPosition();
            }

            if (_ship.EnergyUnit != EnergyUnit.None)
            {
                _draw.Graphics.DrawTextLeft(position, _ship.EnergyUnit == EnergyUnit.Extra ? "Extra Energy Unit" : "Naval Energy Unit", Colour.White);
                IncrementPosition();
            }

            if (_ship.HasDockingComputer)
            {
                _draw.Graphics.DrawTextLeft(position, "Docking Computers", Colour.White);
                IncrementPosition();
            }

            if (_ship.HasGalacticHyperdrive)
            {
                _draw.Graphics.DrawTextLeft(position, "Galactic Hyperspace", Colour.White);
                IncrementPosition();
            }

            if (_ship.LaserFront.Type != LaserType.None)
            {
                _draw.Graphics.DrawTextLeft(position, $"Front {_ship.LaserFront.Name} Laser", Colour.White);
                IncrementPosition();
            }

            if (_ship.LaserRear.Type != LaserType.None)
            {
                _draw.Graphics.DrawTextLeft(position, $"Rear {_ship.LaserRear.Name} Laser", Colour.White);
                IncrementPosition();
            }

            if (_ship.LaserLeft.Type != LaserType.None)
            {
                _draw.Graphics.DrawTextLeft(position, $"Left {_ship.LaserLeft.Name} Laser", Colour.White);
                IncrementPosition();
            }

            if (_ship.LaserRight.Type != LaserType.None)
            {
                _draw.Graphics.DrawTextLeft(position, $"Right {_ship.LaserRight.Name} Laser", Colour.White);
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
