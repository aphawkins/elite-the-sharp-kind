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
    using Elite.Engine.Lasers;
    using Elite.Engine.Ships;
    using Elite.Engine.Types;

    internal class Equipment : IView
    {
        private readonly GameState _gameState;
        private readonly IGfx _gfx;
        private readonly Draw _draw;
        private readonly IKeyboard _keyboard;
        private readonly PlayerShip _ship;
        private readonly Trade _trade;
        private int _highlightedItem;

        internal Equipment(GameState gameState, IGfx gfx, Draw draw, IKeyboard keyboard, PlayerShip ship, Trade trade)
        {
            _gameState = gameState;
            _gfx = gfx;
            _draw = draw;
            _keyboard = keyboard;
            _ship = ship;
            _trade = trade;
        }

        private readonly EquipmentItem[] EquipmentStock = new EquipmentItem[]
        {
            new(false, true,   1, 0.2f, " Fuel",                EquipmentType.EQ_FUEL),
            new(false, true,   1,   30, " Missile",             EquipmentType.EQ_MISSILE),
            new(false, true,   1,  400, " Large Cargo Bay",     EquipmentType.EQ_CARGO_BAY),
            new(false, true,   2,  600, " E.C.M. System",       EquipmentType.EQ_ECM),
            new(false, true,   5,  525, " Fuel Scoops",         EquipmentType.EQ_FUEL_SCOOPS),
            new(false, true,   6, 1000, " Escape Pod",          EquipmentType.EQ_ESCAPE_POD),
            new(false, true,   7,  900, " Energy Bomb",         EquipmentType.EQ_ENERGY_BOMB),
            new(false, true,   8, 1500, " Extra Energy Unit",   EquipmentType.EQ_ENERGY_UNIT),
            new(false, true,   9, 1500, " Docking Computers",   EquipmentType.EQ_DOCK_COMP),
            new(false, true,  10, 5000, " Galactic Hyperdrive", EquipmentType.EQ_GAL_DRIVE),
            new(false, false,  3,  400, "+Pulse Laser",         EquipmentType.EQ_PULSE_LASER),
            new(false, true,   3,    0, "-Pulse Laser",         EquipmentType.EQ_PULSE_LASER),
            new(false, true,   3,  400, ">Front",               EquipmentType.EQ_FRONT_PULSE),
            new(false, true,   3,  400, ">Rear",                EquipmentType.EQ_REAR_PULSE),
            new(false, true,   3,  400, ">Left",                EquipmentType.EQ_LEFT_PULSE),
            new(false, true,   3,  400, ">Right",               EquipmentType.EQ_RIGHT_PULSE),
            new(false, true,   4, 1000, "+Beam Laser",          EquipmentType.EQ_BEAM_LASER),
            new(false, false,  4,    0, "-Beam Laser",          EquipmentType.EQ_BEAM_LASER),
            new(false, false,  4, 1000, ">Front",               EquipmentType.EQ_FRONT_BEAM),
            new(false, false,  4, 1000, ">Rear",                EquipmentType.EQ_REAR_BEAM),
            new(false, false,  4, 1000, ">Left",                EquipmentType.EQ_LEFT_BEAM),
            new(false, false,  4, 1000, ">Right",               EquipmentType.EQ_RIGHT_BEAM),
            new(false, true,  10,  800, "+Mining Laser",        EquipmentType.EQ_MINING_LASER),
            new(false, false, 10,    0, "-Mining Laser",        EquipmentType.EQ_MINING_LASER),
            new(false, false, 10,  800, ">Front",               EquipmentType.EQ_FRONT_MINING),
            new(false, false, 10,  800, ">Rear",                EquipmentType.EQ_REAR_MINING),
            new(false, false, 10,  800, ">Left",                EquipmentType.EQ_LEFT_MINING),
            new(false, false, 10,  800, ">Right",               EquipmentType.EQ_RIGHT_MINING),
            new(false, true,  10, 6000, "+Military Laser",      EquipmentType.EQ_MILITARY_LASER),
            new(false, false, 10,    0, "-Military Laser",      EquipmentType.EQ_MILITARY_LASER),
            new(false, false, 10, 6000, ">Front",               EquipmentType.EQ_FRONT_MILITARY),
            new(false, false, 10, 6000, ">Rear",                EquipmentType.EQ_REAR_MILITARY),
            new(false, false, 10, 6000, ">Left",                EquipmentType.EQ_LEFT_MILITARY),
            new(false, false, 10, 6000, ">Right",               EquipmentType.EQ_RIGHT_MILITARY)
        };

        private bool PresentEquipment(EquipmentType type)
        {
            return type switch
            {
                EquipmentType.EQ_FUEL => _ship.fuel >= 7,
                EquipmentType.EQ_MISSILE => _ship.missileCount >= 4,
                EquipmentType.EQ_CARGO_BAY => _ship.cargoCapacity > 20,
                EquipmentType.EQ_ECM => _ship.hasECM,
                EquipmentType.EQ_FUEL_SCOOPS => _ship.hasFuelScoop,
                EquipmentType.EQ_ESCAPE_POD => _ship.hasEscapePod,
                EquipmentType.EQ_ENERGY_BOMB => _ship.hasEnergyBomb,
                EquipmentType.EQ_ENERGY_UNIT => _ship.energyUnit != EnergyUnit.None,
                EquipmentType.EQ_DOCK_COMP => _ship.hasDockingComputer,
                EquipmentType.EQ_GAL_DRIVE => _ship.hasGalacticHyperdrive,
                EquipmentType.EQ_FRONT_PULSE => _ship.laserFront.Type == LaserType.Pulse,
                EquipmentType.EQ_REAR_PULSE => _ship.laserRear.Type == LaserType.Pulse,
                EquipmentType.EQ_LEFT_PULSE => _ship.laserLeft.Type == LaserType.Pulse,
                EquipmentType.EQ_RIGHT_PULSE => _ship.laserRight.Type == LaserType.Pulse,
                EquipmentType.EQ_FRONT_BEAM => _ship.laserFront.Type == LaserType.Beam,
                EquipmentType.EQ_REAR_BEAM => _ship.laserRear.Type == LaserType.Beam,
                EquipmentType.EQ_LEFT_BEAM => _ship.laserLeft.Type == LaserType.Beam,
                EquipmentType.EQ_RIGHT_BEAM => _ship.laserRight.Type == LaserType.Beam,
                EquipmentType.EQ_FRONT_MINING => _ship.laserFront.Type == LaserType.Mining,
                EquipmentType.EQ_REAR_MINING => _ship.laserRear.Type == LaserType.Mining,
                EquipmentType.EQ_LEFT_MINING => _ship.laserLeft.Type == LaserType.Mining,
                EquipmentType.EQ_RIGHT_MINING => _ship.laserRight.Type == LaserType.Mining,
                EquipmentType.EQ_FRONT_MILITARY => _ship.laserFront.Type == LaserType.Military,
                EquipmentType.EQ_REAR_MILITARY => _ship.laserRear.Type == LaserType.Military,
                EquipmentType.EQ_LEFT_MILITARY => _ship.laserLeft.Type == LaserType.Military,
                EquipmentType.EQ_RIGHT_MILITARY => _ship.laserRight.Type == LaserType.Military,
                _ => false,
            };
        }

        public void Reset()
        {
            CollapseList();

            _highlightedItem = 0;

            ListPrices();
        }

        public void UpdateUniverse()
        {
        }

        public void Draw()
        {
            _draw.ClearDisplay();
            _draw.DrawViewHeader("EQUIP SHIP");

            int y = 55;

            for (int i = 0; i < EquipmentStock.Length; i++)
            {
                if (!EquipmentStock[i].Show)
                {
                    continue;
                }

                if (i == _highlightedItem)
                {
                    _gfx.DrawRectangleFilled(2, y + 1, 508, 15, GFX_COL.GFX_COL_DARK_RED);
                }

                GFX_COL col = EquipmentStock[i].CanBuy ? GFX_COL.GFX_COL_WHITE : GFX_COL.GFX_COL_GREY_1;
                int x = EquipmentStock[i].Name[0] == '>' ? 50 : 16;
                _gfx.DrawTextLeft(x, y, EquipmentStock[i].Name[1..], col);

                if (EquipmentStock[i].Price != 0)
                {
                    _gfx.DrawTextRight(450, y, $"{EquipmentStock[i].Price:N1}", col);
                }

                y += 15;
            }

            _gfx.DrawTextLeft(16, 340, $"Cash: {_trade.credits:N1} Credits", GFX_COL.GFX_COL_WHITE);
        }

        public void HandleInput()
        {
            if (_keyboard.IsKeyPressed(CommandKey.Up, CommandKey.UpArrow))
            {
                SelectPrevious();
            }
            if (_keyboard.IsKeyPressed(CommandKey.Down, CommandKey.DownArrow))
            {
                SelectNext();
            }
            if (_keyboard.IsKeyPressed(CommandKey.Enter))
            {
                Buy();
            }
        }

        internal void SelectNext()
        {
            if (_highlightedItem == EquipmentStock.Length - 1)
            {
                return;
            }

            for (int i = _highlightedItem + 1; i < EquipmentStock.Length; i++)
            {
                if (EquipmentStock[i].Show)
                {
                    _highlightedItem = i;
                    break;
                }
            }
        }

        internal void SelectPrevious()
        {
            if (_highlightedItem == 0)
            {
                return;
            }

            for (int i = _highlightedItem - 1; i >= 0; i--)
            {
                if (EquipmentStock[i].Show)
                {
                    _highlightedItem = i;
                    break;
                }
            }
        }

        private void ListPrices()
        {
            int techLevel = _gameState.current_planet_data.techlevel + 1;

            EquipmentStock[0].Price = (7 - _ship.fuel) * 2;

            for (int i = 0; i < EquipmentStock.Length; i++)
            {
                EquipmentStock[i].CanBuy = !PresentEquipment(EquipmentStock[i].Type) && EquipmentStock[i].Price <= _trade.credits;
                EquipmentStock[i].Show = EquipmentStock[i].Show && techLevel >= EquipmentStock[i].TechLevel;
            }

            _highlightedItem = 0;
        }

        private void CollapseList()
        {
            for (int i = 0; i < EquipmentStock.Length; i++)
            {
                EquipmentStock[i].Show = EquipmentStock[i].Name[0] is ' ' or '+';
            }
        }

        private static float LaserRefund(LaserType laserType)
        {
            return laserType switch
            {
                LaserType.Pulse => 400,
                LaserType.Beam => 1000,
                LaserType.Military => 6000,
                LaserType.Mining => 800,
                _ => 0,
            };
        }

        internal void Buy()
        {
            if (EquipmentStock[_highlightedItem].Name[0] == '+')
            {
                CollapseList();
                EquipmentStock[_highlightedItem].Show = false;
                _highlightedItem++;
                for (int i = 0; i < 5; i++)
                {
                    EquipmentStock[_highlightedItem + i].Show = true;
                }

                ListPrices();
                return;
            }

            if (!EquipmentStock[_highlightedItem].CanBuy)
            {
                return;
            }

            switch (EquipmentStock[_highlightedItem].Type)
            {
                case EquipmentType.EQ_FUEL:
                    _ship.fuel = _ship.maxFuel;
                    elite.scanner.update_console();
                    break;

                case EquipmentType.EQ_MISSILE:
                    _ship.missileCount++;
                    elite.scanner.update_console();
                    break;

                case EquipmentType.EQ_CARGO_BAY:
                    _ship.cargoCapacity = 35;
                    break;

                case EquipmentType.EQ_ECM:
                    _ship.hasECM = true;
                    break;

                case EquipmentType.EQ_FUEL_SCOOPS:
                    _ship.hasFuelScoop = true;
                    break;

                case EquipmentType.EQ_ESCAPE_POD:
                    _ship.hasEscapePod = true;
                    break;

                case EquipmentType.EQ_ENERGY_BOMB:
                    _ship.hasEnergyBomb = true;
                    break;

                case EquipmentType.EQ_ENERGY_UNIT:
                    _ship.energyUnit = EnergyUnit.Extra;
                    break;

                case EquipmentType.EQ_DOCK_COMP:
                    _ship.hasDockingComputer = true;
                    break;

                case EquipmentType.EQ_GAL_DRIVE:
                    _ship.hasGalacticHyperdrive = true;
                    break;

                case EquipmentType.EQ_FRONT_PULSE:
                    _trade.credits += LaserRefund(_ship.laserFront.Type);
                    _ship.laserFront = new PulseLaser();
                    break;

                case EquipmentType.EQ_REAR_PULSE:
                    _trade.credits += LaserRefund(_ship.laserRear.Type);
                    _ship.laserRear = new PulseLaser();
                    break;

                case EquipmentType.EQ_LEFT_PULSE:
                    _trade.credits += LaserRefund(_ship.laserLeft.Type);
                    _ship.laserLeft = new PulseLaser();
                    break;

                case EquipmentType.EQ_RIGHT_PULSE:
                    _trade.credits += LaserRefund(_ship.laserRight.Type);
                    _ship.laserRight = new PulseLaser();
                    break;

                case EquipmentType.EQ_FRONT_BEAM:
                    _trade.credits += LaserRefund(_ship.laserFront.Type);
                    _ship.laserFront = new BeamLaser();
                    break;

                case EquipmentType.EQ_REAR_BEAM:
                    _trade.credits += LaserRefund(_ship.laserRear.Type);
                    _ship.laserRear = new BeamLaser();
                    break;

                case EquipmentType.EQ_LEFT_BEAM:
                    _trade.credits += LaserRefund(_ship.laserLeft.Type);
                    _ship.laserLeft = new BeamLaser();
                    break;

                case EquipmentType.EQ_RIGHT_BEAM:
                    _trade.credits += LaserRefund(_ship.laserRight.Type);
                    _ship.laserRight = new BeamLaser();
                    break;

                case EquipmentType.EQ_FRONT_MINING:
                    _trade.credits += LaserRefund(_ship.laserFront.Type);
                    _ship.laserFront = new MiningLaser();
                    break;

                case EquipmentType.EQ_REAR_MINING:
                    _trade.credits += LaserRefund(_ship.laserRear.Type);
                    _ship.laserRear = new MiningLaser();
                    break;

                case EquipmentType.EQ_LEFT_MINING:
                    _trade.credits += LaserRefund(_ship.laserLeft.Type);
                    _ship.laserLeft = new MiningLaser();
                    break;

                case EquipmentType.EQ_RIGHT_MINING:
                    _trade.credits += LaserRefund(_ship.laserRight.Type);
                    _ship.laserRight = new MiningLaser();
                    break;

                case EquipmentType.EQ_FRONT_MILITARY:
                    _trade.credits += LaserRefund(_ship.laserFront.Type);
                    _ship.laserFront = new MilitaryLaser();
                    break;

                case EquipmentType.EQ_REAR_MILITARY:
                    _trade.credits += LaserRefund(_ship.laserRear.Type);
                    _ship.laserRear = new MilitaryLaser();
                    break;

                case EquipmentType.EQ_LEFT_MILITARY:
                    _trade.credits += LaserRefund(_ship.laserLeft.Type);
                    _ship.laserLeft = new MilitaryLaser();
                    break;

                case EquipmentType.EQ_RIGHT_MILITARY:
                    _trade.credits += LaserRefund(_ship.laserRight.Type);
                    _ship.laserRight = new MilitaryLaser();
                    break;
            }

            _trade.credits -= EquipmentStock[_highlightedItem].Price;
            ListPrices();
        }
    }
}