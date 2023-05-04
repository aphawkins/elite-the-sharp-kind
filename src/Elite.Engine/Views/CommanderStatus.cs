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

namespace Elite.Engine.Views
{
    using Elite.Engine.Enums;
    using Elite.Engine.Ships;

    internal class CommanderStatus : IView
    {
        public readonly GameState _gameState;
        private readonly IGfx _gfx;
        private readonly Draw _draw;
        private readonly PlayerShip _ship;
        private readonly Trade _trade;
        readonly int EQUIP_START_Y = 202;
        readonly int Y_INC = 16;
        readonly int EQUIP_MAX_Y = 290;
        readonly int EQUIP_WIDTH = 200;

        readonly string[] condition_txt = new string[]
        {
                "Docked",
                "Green",
                "Yellow",
                "Red"
        };

        readonly (int score, string title)[] ratings = new (int score, string title)[]
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

        internal CommanderStatus(GameState gameState, IGfx gfx, Draw draw, PlayerShip ship, Trade trade)
        {
            _gameState = gameState;
            _gfx = gfx;
            _draw = draw;
            _ship = ship;
            _trade = trade;
        }

        public void Draw()
        {
            _draw.ClearDisplay();
            int x = 50;
            int y = EQUIP_START_Y;

            void IncrementPosition()
            {
                y += Y_INC;
                if (y > EQUIP_MAX_Y)
                {
                    y = EQUIP_START_Y;
                    x += EQUIP_WIDTH;
                }
            };

            string rating = string.Empty;
            foreach ((int score, string title) in ratings)
            {
                if (_gameState.cmdr.Score >= score)
                {
                    rating = title;
                }
            }

            string dockedPlanetName = Planet.NamePlanet(_gameState.docked_planet, true);
            string hyperspacePlanetName = Planet.NamePlanet(_gameState.hyperspace_planet, true);

            int condition = 0;

            if (!EliteMain.docked)
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

                if (condition == 2 && _ship.energy < 128)
                {
                    condition = 3;
                }
            }

            _draw.DrawViewHeader($"COMMANDER {_gameState.cmdr.Name}");
            _gfx.DrawTextLeft(16, 58, "Present System:", GFX_COL.GFX_COL_GREEN_1);

            if (!_gameState.witchspace)
            {
                _gfx.DrawTextLeft(150, 58, dockedPlanetName, GFX_COL.GFX_COL_WHITE);
            }

            _gfx.DrawTextLeft(16, 74, "Hyperspace System:", GFX_COL.GFX_COL_GREEN_1);
            _gfx.DrawTextLeft(150, 74, hyperspacePlanetName, GFX_COL.GFX_COL_WHITE);

            _gfx.DrawTextLeft(16, 90, "Condition:", GFX_COL.GFX_COL_GREEN_1);
            _gfx.DrawTextLeft(150, 90, condition_txt[condition], GFX_COL.GFX_COL_WHITE);

            _gfx.DrawTextLeft(16, 106, "Fuel:", GFX_COL.GFX_COL_GREEN_1);
            _gfx.DrawTextLeft(150, 106, $"{_ship.fuel:N1} Light Years", GFX_COL.GFX_COL_WHITE);

            _gfx.DrawTextLeft(16, 122, "Cash:", GFX_COL.GFX_COL_GREEN_1);
            _gfx.DrawTextLeft(150, 122, $"{_trade.credits:N1} Credits", GFX_COL.GFX_COL_WHITE);

            _gfx.DrawTextLeft(16, 138, "Legal Status:", GFX_COL.GFX_COL_GREEN_1);
            _gfx.DrawTextLeft(150, 138, _gameState.cmdr.LegalStatus == 0 ? "Clean" : _gameState.cmdr.LegalStatus > 50 ? "Fugitive" : "Offender", GFX_COL.GFX_COL_WHITE);

            _gfx.DrawTextLeft(16, 154, "Rating:", GFX_COL.GFX_COL_GREEN_1);
            _gfx.DrawTextLeft(150, 154, rating, GFX_COL.GFX_COL_WHITE);

            _gfx.DrawTextLeft(16, 186, "EQUIPMENT:", GFX_COL.GFX_COL_GREEN_1);

            if (_ship.cargoCapacity > 20)
            {
                _gfx.DrawTextLeft(x, y, "Large Cargo Bay", GFX_COL.GFX_COL_WHITE);
                IncrementPosition();
            }

            if (_ship.hasEscapePod)
            {
                _gfx.DrawTextLeft(x, y, "Escape Pod", GFX_COL.GFX_COL_WHITE);
                IncrementPosition();
            }

            if (_ship.hasFuelScoop)
            {
                _gfx.DrawTextLeft(x, y, "Fuel Scoops", GFX_COL.GFX_COL_WHITE);
                IncrementPosition();
            }

            if (_ship.hasECM)
            {
                _gfx.DrawTextLeft(x, y, "E.C.M. System", GFX_COL.GFX_COL_WHITE);
                IncrementPosition();
            }

            if (_ship.hasEnergyBomb)
            {
                _gfx.DrawTextLeft(x, y, "Energy Bomb", GFX_COL.GFX_COL_WHITE);
                IncrementPosition();
            }

            if (_ship.energyUnit != EnergyUnit.None)
            {
                _gfx.DrawTextLeft(x, y, _ship.energyUnit == EnergyUnit.Extra ? "Extra Energy Unit" : "Naval Energy Unit", GFX_COL.GFX_COL_WHITE);
                IncrementPosition();
            }

            if (_ship.hasDockingComputer)
            {
                _gfx.DrawTextLeft(x, y, "Docking Computers", GFX_COL.GFX_COL_WHITE);
                IncrementPosition();
            }

            if (_ship.hasGalacticHyperdrive)
            {
                _gfx.DrawTextLeft(x, y, "Galactic Hyperspace", GFX_COL.GFX_COL_WHITE);
                IncrementPosition();
            }

            if (_ship.laserFront.Type != LaserType.None)
            {
                _gfx.DrawTextLeft(x, y, $"Front {_ship.laserFront.Name} Laser", GFX_COL.GFX_COL_WHITE);
                IncrementPosition();
            }

            if (_ship.laserRear.Type != LaserType.None)
            {
                _gfx.DrawTextLeft(x, y, $"Rear {_ship.laserRear.Name} Laser", GFX_COL.GFX_COL_WHITE);
                IncrementPosition();
            }

            if (_ship.laserLeft.Type != LaserType.None)
            {
                _gfx.DrawTextLeft(x, y, $"Left {_ship.laserLeft.Name} Laser", GFX_COL.GFX_COL_WHITE);
                IncrementPosition();
            }

            if (_ship.laserRight.Type != LaserType.None)
            {
                _gfx.DrawTextLeft(x, y, $"Right {_ship.laserRight.Name} Laser", GFX_COL.GFX_COL_WHITE);
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