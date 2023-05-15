﻿// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using Elite.Engine.Enums;
using Elite.Engine.Lasers;
using Elite.Engine.Ships;
using Elite.Engine.Trader;
using Elite.Engine.Types;

namespace Elite.Engine.Views
{
    internal sealed class EquipmentView : IView
    {
        private readonly Draw _draw;

        private readonly EquipmentItem[] _equipmentStock = new EquipmentItem[]
        {
            new(false, true,   1, 0.2f, " Fuel",                EquipmentType.EQ_FUEL),
            new(false, true,   1,   30, " Missile",             EquipmentType.EQ_MISSILE),
            new(false, true,   1,  400, " Large Cargo Bay",     EquipmentType.EQ_CARGO_BAY),
            new(false, true,   2,  600, " E.C.M. System",       EquipmentType.EQ_ECM),
            new(false, true,   5,  525, " Fuel Scoops",         EquipmentType.EQ_FUEL_SCOOPS),
            new(false, true,   6, 1000, " Escape Capsule",      EquipmentType.EQ_ESCAPE_CAPSULE),
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
            new(false, false, 10, 6000, ">Right",               EquipmentType.EQ_RIGHT_MILITARY),
        };

        private readonly GameState _gameState;
        private readonly IGraphics _graphics;
        private readonly IKeyboard _keyboard;
        private readonly Scanner _scanner;
        private readonly PlayerShip _ship;
        private readonly Trade _trade;
        private int _highlightedItem;

        internal EquipmentView(GameState gameState, IGraphics graphics, Draw draw, IKeyboard keyboard, PlayerShip ship, Trade trade, Scanner scanner)
        {
            _gameState = gameState;
            _graphics = graphics;
            _draw = draw;
            _keyboard = keyboard;
            _ship = ship;
            _trade = trade;
            _scanner = scanner;
        }

        public void Draw()
        {
            _draw.ClearDisplay();
            _draw.DrawViewHeader("EQUIP SHIP");

            int y = 55;

            for (int i = 0; i < _equipmentStock.Length; i++)
            {
                if (!_equipmentStock[i].Show)
                {
                    continue;
                }

                if (i == _highlightedItem)
                {
                    _graphics.DrawRectangleFilled(2, y + 1, 508, 15, Colour.Red2);
                }

                Colour col = _equipmentStock[i].CanBuy ? Colour.White1 : Colour.Grey1;
                int x = _equipmentStock[i].Name[0] == '>' ? 50 : 16;
                _graphics.DrawTextLeft(x, y, _equipmentStock[i].Name[1..], col);

                if (_equipmentStock[i].Price != 0)
                {
                    _graphics.DrawTextRight(450, y, $"{_equipmentStock[i].Price:N1}", col);
                }

                y += 15;
            }

            _graphics.DrawTextLeft(16, 340, $"Cash: {_trade._credits:N1} Credits", Colour.White1);
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

        public void Reset()
        {
            CollapseList();

            _highlightedItem = 0;

            ListPrices();
        }

        public void UpdateUniverse()
        {
        }

        internal void Buy()
        {
            if (_equipmentStock[_highlightedItem].Name[0] == '+')
            {
                CollapseList();
                _equipmentStock[_highlightedItem].Show = false;
                _highlightedItem++;
                for (int i = 0; i < 5; i++)
                {
                    _equipmentStock[_highlightedItem + i].Show = true;
                }

                ListPrices();
                return;
            }

            if (!_equipmentStock[_highlightedItem].CanBuy)
            {
                return;
            }

            switch (_equipmentStock[_highlightedItem].Type)
            {
                case EquipmentType.EQ_FUEL:
                    _ship.Fuel = _ship.MaxFuel;
                    _scanner.UpdateConsole();
                    break;

                case EquipmentType.EQ_MISSILE:
                    _ship.MissileCount++;
                    _scanner.UpdateConsole();
                    break;

                case EquipmentType.EQ_CARGO_BAY:
                    _ship.CargoCapacity = 35;
                    break;

                case EquipmentType.EQ_ECM:
                    _ship.HasECM = true;
                    break;

                case EquipmentType.EQ_FUEL_SCOOPS:
                    _ship.HasFuelScoop = true;
                    break;

                case EquipmentType.EQ_ESCAPE_CAPSULE:
                    _ship.HasEscapeCapsule = true;
                    break;

                case EquipmentType.EQ_ENERGY_BOMB:
                    _ship.HasEnergyBomb = true;
                    break;

                case EquipmentType.EQ_ENERGY_UNIT:
                    _ship.EnergyUnit = EnergyUnit.Extra;
                    break;

                case EquipmentType.EQ_DOCK_COMP:
                    _ship.HasDockingComputer = true;
                    break;

                case EquipmentType.EQ_GAL_DRIVE:
                    _ship.HasGalacticHyperdrive = true;
                    break;

                case EquipmentType.EQ_FRONT_PULSE:
                    _trade._credits += LaserRefund(_ship.LaserFront.Type);
                    _ship.LaserFront = new PulseLaser();
                    break;

                case EquipmentType.EQ_REAR_PULSE:
                    _trade._credits += LaserRefund(_ship.LaserRear.Type);
                    _ship.LaserRear = new PulseLaser();
                    break;

                case EquipmentType.EQ_LEFT_PULSE:
                    _trade._credits += LaserRefund(_ship.LaserLeft.Type);
                    _ship.LaserLeft = new PulseLaser();
                    break;

                case EquipmentType.EQ_RIGHT_PULSE:
                    _trade._credits += LaserRefund(_ship.LaserRight.Type);
                    _ship.LaserRight = new PulseLaser();
                    break;

                case EquipmentType.EQ_FRONT_BEAM:
                    _trade._credits += LaserRefund(_ship.LaserFront.Type);
                    _ship.LaserFront = new BeamLaser();
                    break;

                case EquipmentType.EQ_REAR_BEAM:
                    _trade._credits += LaserRefund(_ship.LaserRear.Type);
                    _ship.LaserRear = new BeamLaser();
                    break;

                case EquipmentType.EQ_LEFT_BEAM:
                    _trade._credits += LaserRefund(_ship.LaserLeft.Type);
                    _ship.LaserLeft = new BeamLaser();
                    break;

                case EquipmentType.EQ_RIGHT_BEAM:
                    _trade._credits += LaserRefund(_ship.LaserRight.Type);
                    _ship.LaserRight = new BeamLaser();
                    break;

                case EquipmentType.EQ_FRONT_MINING:
                    _trade._credits += LaserRefund(_ship.LaserFront.Type);
                    _ship.LaserFront = new MiningLaser();
                    break;

                case EquipmentType.EQ_REAR_MINING:
                    _trade._credits += LaserRefund(_ship.LaserRear.Type);
                    _ship.LaserRear = new MiningLaser();
                    break;

                case EquipmentType.EQ_LEFT_MINING:
                    _trade._credits += LaserRefund(_ship.LaserLeft.Type);
                    _ship.LaserLeft = new MiningLaser();
                    break;

                case EquipmentType.EQ_RIGHT_MINING:
                    _trade._credits += LaserRefund(_ship.LaserRight.Type);
                    _ship.LaserRight = new MiningLaser();
                    break;

                case EquipmentType.EQ_FRONT_MILITARY:
                    _trade._credits += LaserRefund(_ship.LaserFront.Type);
                    _ship.LaserFront = new MilitaryLaser();
                    break;

                case EquipmentType.EQ_REAR_MILITARY:
                    _trade._credits += LaserRefund(_ship.LaserRear.Type);
                    _ship.LaserRear = new MilitaryLaser();
                    break;

                case EquipmentType.EQ_LEFT_MILITARY:
                    _trade._credits += LaserRefund(_ship.LaserLeft.Type);
                    _ship.LaserLeft = new MilitaryLaser();
                    break;

                case EquipmentType.EQ_RIGHT_MILITARY:
                    _trade._credits += LaserRefund(_ship.LaserRight.Type);
                    _ship.LaserRight = new MilitaryLaser();
                    break;

                case EquipmentType.EQ_PULSE_LASER:
                    break;

                case EquipmentType.EQ_BEAM_LASER:
                    break;

                case EquipmentType.EQ_MINING_LASER:
                    break;

                case EquipmentType.EQ_MILITARY_LASER:
                    break;

                default:
                    break;
            }

            _trade._credits -= _equipmentStock[_highlightedItem].Price;
            ListPrices();
        }

        internal void SelectNext()
        {
            if (_highlightedItem == _equipmentStock.Length - 1)
            {
                return;
            }

            for (int i = _highlightedItem + 1; i < _equipmentStock.Length; i++)
            {
                if (_equipmentStock[i].Show)
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
                if (_equipmentStock[i].Show)
                {
                    _highlightedItem = i;
                    break;
                }
            }
        }

        private static float LaserRefund(LaserType laserType) => laserType switch
        {
            LaserType.Pulse => 400,
            LaserType.Beam => 1000,
            LaserType.Military => 6000,
            LaserType.Mining => 800,
            LaserType.None => throw new NotImplementedException(),
            _ => 0,
        };

        private void CollapseList()
        {
            for (int i = 0; i < _equipmentStock.Length; i++)
            {
                _equipmentStock[i].Show = _equipmentStock[i].Name[0] is ' ' or '+';
            }
        }

        private void ListPrices()
        {
            int techLevel = _gameState.CurrentPlanetData.TechLevel + 1;

            _equipmentStock[0].Price = (7 - _ship.Fuel) * 2;

            for (int i = 0; i < _equipmentStock.Length; i++)
            {
                _equipmentStock[i].CanBuy = !PresentEquipment(_equipmentStock[i].Type) && _equipmentStock[i].Price <= _trade._credits;
                _equipmentStock[i].Show = _equipmentStock[i].Show && techLevel >= _equipmentStock[i].TechLevel;
            }

            _highlightedItem = 0;
        }

        private bool PresentEquipment(EquipmentType type) => type switch
        {
            EquipmentType.EQ_FUEL => _ship.Fuel >= 7,
            EquipmentType.EQ_MISSILE => _ship.MissileCount >= 4,
            EquipmentType.EQ_CARGO_BAY => _ship.CargoCapacity > 20,
            EquipmentType.EQ_ECM => _ship.HasECM,
            EquipmentType.EQ_FUEL_SCOOPS => _ship.HasFuelScoop,
            EquipmentType.EQ_ESCAPE_CAPSULE => _ship.HasEscapeCapsule,
            EquipmentType.EQ_ENERGY_BOMB => _ship.HasEnergyBomb,
            EquipmentType.EQ_ENERGY_UNIT => _ship.EnergyUnit != EnergyUnit.None,
            EquipmentType.EQ_DOCK_COMP => _ship.HasDockingComputer,
            EquipmentType.EQ_GAL_DRIVE => _ship.HasGalacticHyperdrive,
            EquipmentType.EQ_FRONT_PULSE => _ship.LaserFront.Type == LaserType.Pulse,
            EquipmentType.EQ_REAR_PULSE => _ship.LaserRear.Type == LaserType.Pulse,
            EquipmentType.EQ_LEFT_PULSE => _ship.LaserLeft.Type == LaserType.Pulse,
            EquipmentType.EQ_RIGHT_PULSE => _ship.LaserRight.Type == LaserType.Pulse,
            EquipmentType.EQ_FRONT_BEAM => _ship.LaserFront.Type == LaserType.Beam,
            EquipmentType.EQ_REAR_BEAM => _ship.LaserRear.Type == LaserType.Beam,
            EquipmentType.EQ_LEFT_BEAM => _ship.LaserLeft.Type == LaserType.Beam,
            EquipmentType.EQ_RIGHT_BEAM => _ship.LaserRight.Type == LaserType.Beam,
            EquipmentType.EQ_FRONT_MINING => _ship.LaserFront.Type == LaserType.Mining,
            EquipmentType.EQ_REAR_MINING => _ship.LaserRear.Type == LaserType.Mining,
            EquipmentType.EQ_LEFT_MINING => _ship.LaserLeft.Type == LaserType.Mining,
            EquipmentType.EQ_RIGHT_MINING => _ship.LaserRight.Type == LaserType.Mining,
            EquipmentType.EQ_FRONT_MILITARY => _ship.LaserFront.Type == LaserType.Military,
            EquipmentType.EQ_REAR_MILITARY => _ship.LaserRear.Type == LaserType.Military,
            EquipmentType.EQ_LEFT_MILITARY => _ship.LaserLeft.Type == LaserType.Military,
            EquipmentType.EQ_RIGHT_MILITARY => _ship.LaserRight.Type == LaserType.Military,
            EquipmentType.EQ_PULSE_LASER => throw new NotImplementedException(),
            EquipmentType.EQ_BEAM_LASER => throw new NotImplementedException(),
            EquipmentType.EQ_MINING_LASER => throw new NotImplementedException(),
            EquipmentType.EQ_MILITARY_LASER => throw new NotImplementedException(),
            _ => false,
        };
    }
}