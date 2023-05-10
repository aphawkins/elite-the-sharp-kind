// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using Elite.Common.Enums;
using Elite.Engine.Conflict;
using Elite.Engine.Enums;
using Elite.Engine.Ships;
using Elite.Engine.Types;
using Elite.Engine.Views;

namespace Elite.Engine
{
    /// <summary>
    /// This module handles all the flight system and management of the space universe.
    /// </summary>
    internal sealed class Scanner
    {
        private readonly Combat _combat;
        private readonly Draw _draw;
        private readonly GameState _gameState;
        private readonly IGfx _gfx;
        private readonly PlayerShip _ship;
        private readonly Dictionary<ShipType, int> _shipCount;
        private readonly UniverseObject[] _universe;
        private Vector2 _scannerCentre = new(253, 63 + 385);

        internal Scanner(GameState gameState, IGfx gfx, Draw draw, UniverseObject[] universe, Dictionary<ShipType, int> shipCount, PlayerShip ship, Combat combat)
        {
            _gameState = gameState;
            _gfx = gfx;
            _draw = draw;
            _universe = universe;
            _shipCount = shipCount;
            _ship = ship;
            _combat = combat;
        }

        internal void UpdateConsole()
        {
            _gfx.SetClipRegion(0, 0, 512, 512);
            _draw.DrawScanner();

            DisplaySpeed();
            DisplayFlightClimb();
            DisplayFlightRoll();
            DisplayShields();
            DisplayAltitude();
            DisplayEnergy();
            DisplayCabinTemp();
            DisplayLaserTemp();
            DisplayFuel();
            DisplayMissiles();

            if (_gameState.IsDocked)
            {
                return;
            }

            UpdateScanner();
            UpdateCompass();

            if (_shipCount[ShipType.Coriolis] != 0 || _shipCount[ShipType.Dodec] != 0)
            {
                _gfx.DrawImage(Image.BigS, new(387, 490));
            }

            if (_ship.EcmActive != 0)
            {
                _gfx.DrawImage(Image.BigE, new(115, 490));
            }
        }

        private void DisplayAltitude()
        {
            if (_ship.Altitude > 3)
            {
                DisplayDialBar(_ship.Altitude / 4, new(31, 92));
            }
        }

        private void DisplayCabinTemp()
        {
            if (_ship.CabinTemperature > 3)
            {
                DisplayDialBar(_ship.CabinTemperature / 4, new(31, 60));
            }
        }

        /// <summary>
        /// Draw an indicator bar. Used for shields and energy banks.
        /// </summary>
        /// <param name="len"></param>
        /// <param name="position"></param>
        private void DisplayDialBar(float len, Vector2 position)
        {
            _gfx.DrawLine(new(position.X, position.Y + 384), new(position.X + len, position.Y + 384), Colour.Gold);
            int i = 1;
            _gfx.DrawLine(new(position.X, position.Y + i + 384), new(position.X + len, position.Y + i + 384), Colour.Gold);

            for (i = 2; i < 7; i++)
            {
                _gfx.DrawLine(new(position.X, position.Y + i + 384), new(position.X + len, position.Y + i + 384), Colour.Yellow1);
            }

            _gfx.DrawLine(new(position.X, position.Y + i + 384), new(position.X + len, position.Y + i + 384), Colour.Red2);
        }

        /// <summary>
        /// Display the energy banks.
        /// </summary>
        private void DisplayEnergy()
        {
            float e1 = _ship.Energy > 64 ? 64 : _ship.Energy;
            float e2 = _ship.Energy > 128 ? 64 : _ship.Energy - 64;
            float e3 = _ship.Energy > 192 ? 64 : _ship.Energy - 128;
            float e4 = _ship.Energy - 192;

            if (e4 > 0)
            {
                DisplayDialBar(e4, new(416, 61));
            }

            if (e3 > 0)
            {
                DisplayDialBar(e3, new(416, 79));
            }

            if (e2 > 0)
            {
                DisplayDialBar(e2, new(416, 97));
            }

            if (e1 > 0)
            {
                DisplayDialBar(e1, new(416, 115));
            }
        }

        private void DisplayFlightClimb()
        {
            float sx = 416;
            float sy = 384 + 9 + 14 + 16;

            float pos = sx + (_ship.Climb * 28 / _ship.MaxClimb);
            pos += 32;

            for (int i = 0; i < 4; i++)
            {
                _gfx.DrawLine(new(pos + i, sy), new(pos + i, sy + 7), Colour.Gold);
            }
        }

        private void DisplayFlightRoll()
        {
            float sx = 416;
            float sy = 384 + 9 + 14;

            float pos = sx - (_ship.Roll * 28 / _ship.MaxRoll);
            pos += 32;

            for (int i = 0; i < 4; i++)
            {
                _gfx.DrawLine(new(pos + i, sy), new(pos + i, sy + 7), Colour.Gold);
            }
        }

        private void DisplayFuel()
        {
            if (_ship.Fuel > 0)
            {
                DisplayDialBar(_ship.Fuel * 64 / _ship.MaxFuel, new(31, 44));
            }
        }

        private void DisplayLaserTemp()
        {
            if (_gameState.LaserTemp > 0)
            {
                DisplayDialBar(_gameState.LaserTemp / 4, new(31, 76));
            }
        }

        private void DisplayMissiles()
        {
            if (_ship.MissileCount == 0)
            {
                return;
            }

            int missileCount = _ship.MissileCount > 4 ? 4 : _ship.MissileCount;

            Vector2 location = new(((4 - missileCount) * 16) + 35, 113 + 385);

            if (_combat.MissileTarget != _combat.IsMissileUnarmed)
            {
                _gfx.DrawImage((_combat.MissileTarget < 0) ? Image.MissileYellow : Image.MissileRed, location);
                location.X += 16;
                missileCount--;
            }

            for (; missileCount > 0; missileCount--)
            {
                _gfx.DrawImage(Image.MissileGreen, location);
                location.X += 16;
            }
        }

        /// <summary>
        /// Display the current shield strengths.
        /// </summary>
        private void DisplayShields()
        {
            if (_ship.ShieldFront > 3)
            {
                DisplayDialBar(_ship.ShieldFront / 4, new(31, 7));
            }

            if (_ship.ShieldRear > 3)
            {
                DisplayDialBar(_ship.ShieldRear / 4, new(31, 23));
            }
        }

        /// <summary>
        /// Display the speed bar.
        /// </summary>
        private void DisplaySpeed()
        {
            float sx = 417;
            float sy = 384 + 9;

            float len = (_ship.Speed * 64 / _ship.MaxSpeed) - 1;

            Colour colour = (_ship.Speed > (_ship.MaxSpeed * 2 / 3)) ? Colour.Red2 : Colour.Gold;

            for (int i = 0; i < 6; i++)
            {
                _gfx.DrawLine(new(sx, sy + i), new(sx + len, sy + i), colour);
            }
        }

        /// <summary>
        /// Update the compass which tracks the space station / planet.
        /// </summary>
        private void UpdateCompass()
        {
            if (_gameState.InWitchspace)
            {
                return;
            }

            int un = _shipCount[ShipType.Coriolis] == 0 && _shipCount[ShipType.Dodec] == 0 ? 0 : 1;
            Vector3 dest = VectorMaths.UnitVector(_universe[un].Location);

            if (float.IsNaN(dest.X))
            {
                return;
            }

            Vector2 compass = new(_gameState.CompassCentre.X + (dest.X * 16), _gameState.CompassCentre.Y + (dest.Y * -16));

            if (dest.Z < 0)
            {
                _gfx.DrawImage(Image.DotRed, compass);
            }
            else
            {
                _gfx.DrawImage(Image.GreenDot, compass);
            }
        }

        /// <summary>
        /// Update the scanner and draw all the lollipops.
        /// </summary>
        private void UpdateScanner()
        {
            for (int i = 0; i < EliteMain.MAX_UNIV_OBJECTS; i++)
            {
                if ((_universe[i].Type <= 0) ||
                    _universe[i].Flags.HasFlag(ShipFlags.Dead) ||
                    _universe[i].Flags.HasFlag(ShipFlags.Cloaked))
                {
                    continue;
                }

                float x = _universe[i].Location.X / 256;
                float y1 = -_universe[i].Location.Z / 1024;
                float y2 = y1 - (_universe[i].Location.Y / 512);

                if ((y2 < -28) || (y2 > 28) ||
                    (x < -50) || (x > 50))
                {
                    continue;
                }

                x += _scannerCentre.X;
                y1 += _scannerCentre.Y;
                y2 += _scannerCentre.Y;

                Colour colour = _universe[i].Flags.HasFlag(ShipFlags.Hostile) ? Colour.Yellow5 : Colour.White1;

                switch (_universe[i].Type)
                {
                    case ShipType.Missile:
                        colour = Colour.Pink1;
                        break;

                    case ShipType.Dodec:
                    case ShipType.Coriolis:
                        colour = Colour.Green1;
                        break;

                    case ShipType.Viper:
                        colour = Colour.Blue4;
                        break;

                    case ShipType.Sun:
                        break;

                    case ShipType.Planet:
                        break;

                    case ShipType.None:
                        break;

                    case ShipType.EscapeCapsule:
                        break;

                    case ShipType.Alloy:
                        break;

                    case ShipType.Cargo:
                        break;

                    case ShipType.Boulder:
                        break;

                    case ShipType.Asteroid:
                        break;

                    case ShipType.Rock:
                        break;

                    case ShipType.Shuttle:
                        break;

                    case ShipType.Transporter:
                        break;

                    case ShipType.CobraMk3:
                        break;

                    case ShipType.Python:
                        break;

                    case ShipType.Boa:
                        break;

                    case ShipType.Anaconda:
                        break;

                    case ShipType.Hermit:
                        break;

                    case ShipType.Sidewinder:
                        break;

                    case ShipType.Mamba:
                        break;

                    case ShipType.Krait:
                        break;

                    case ShipType.Adder:
                        break;

                    case ShipType.Gecko:
                        break;

                    case ShipType.CobraMk1:
                        break;

                    case ShipType.Worm:
                        break;

                    case ShipType.CobraMk3Lone:
                        break;

                    case ShipType.Asp2:
                        break;

                    case ShipType.PythonLone:
                        break;

                    case ShipType.FerDeLance:
                        break;

                    case ShipType.Moray:
                        break;

                    case ShipType.Thargoid:
                        break;

                    case ShipType.Tharglet:
                        break;

                    case ShipType.Constrictor:
                        break;

                    case ShipType.Cougar:
                        break;

                    default:
                        break;
                }

                // ship
                _gfx.DrawRectangleFilled(x - 3, y2, 5, 3, colour);
                // stick
                _gfx.DrawRectangleFilled(x, y2 < y1 ? y2 : y1, 2, MathF.Abs(y2 - y1), colour);
            }
        }
    }
}
