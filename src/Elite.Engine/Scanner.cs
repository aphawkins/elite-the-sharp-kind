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

using System.Numerics;
using Elite.Common.Enums;
using Elite.Engine.Enums;
using Elite.Engine.Ships;
using Elite.Engine.Types;
using Elite.Engine.Views;

/*
 * space.c
 *
 * This module handles all the flight system and management of the space universe.
 */

namespace Elite.Engine
{
    internal class Scanner
    {
        private readonly GameState _gameState;
        private readonly IGfx _gfx;
        private readonly UniverseObject[] _universe;
        private readonly Dictionary<ShipType, int> _shipCount;
        private readonly PlayerShip _ship;
        private readonly Combat _combat;
        private readonly Draw _draw;
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

                GFX_COL colour = _universe[i].Flags.HasFlag(ShipFlags.Hostile) ? GFX_COL.GFX_COL_YELLOW_5 : GFX_COL.GFX_COL_WHITE;

                switch (_universe[i].Type)
                {
                    case ShipType.Missile:
                        colour = GFX_COL.GFX_COL_PINK_1;
                        break;

                    case ShipType.Dodec:
                    case ShipType.Coriolis:
                        colour = GFX_COL.GFX_COL_GREEN_1;
                        break;

                    case ShipType.Viper:
                        colour = GFX_COL.GFX_COL_BLUE_4;
                        break;
                }

                // ship
                _gfx.DrawRectangleFilled(x - 3, y2, 5, 3, colour);
                // stick
                _gfx.DrawRectangleFilled(x, y2 < y1 ? y2 : y1, 2, MathF.Abs(y2 - y1), colour);
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
        /// Display the speed bar.
        /// </summary>
        private void DisplaySpeed()
        {
            float sx = 417;
            float sy = 384 + 9;

            float len = (_ship.Speed * 64 / _ship.MaxSpeed) - 1;

            GFX_COL colour = (_ship.Speed > (_ship.MaxSpeed * 2 / 3)) ? GFX_COL.GFX_COL_DARK_RED : GFX_COL.GFX_COL_GOLD;

            for (int i = 0; i < 6; i++)
            {
                _gfx.DrawLine(new(sx, sy + i), new(sx + len, sy + i), colour);
            }
        }

        /// <summary>
        /// Draw an indicator bar. Used for shields and energy banks.
        /// </summary>
        /// <param name="len"></param>
        /// <param name="position"></param>
        private void DisplayDialBar(float len, Vector2 position)
        {
            _gfx.DrawLine(new(position.X, position.Y + 384), new(position.X + len, position.Y + 384), GFX_COL.GFX_COL_GOLD);
            int i = 1;
            _gfx.DrawLine(new(position.X, position.Y + i + 384), new(position.X + len, position.Y + i + 384), GFX_COL.GFX_COL_GOLD);

            for (i = 2; i < 7; i++)
            {
                _gfx.DrawLine(new(position.X, position.Y + i + 384), new(position.X + len, position.Y + i + 384), GFX_COL.GFX_COL_YELLOW_1);
            }

            _gfx.DrawLine(new(position.X, position.Y + i + 384), new(position.X + len, position.Y + i + 384), GFX_COL.GFX_COL_DARK_RED);
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

        private void DisplayLaserTemp()
        {
            if (_gameState.LaserTemp > 0)
            {
                DisplayDialBar(_gameState.LaserTemp / 4, new(31, 76));
            }
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

        private void DisplayFlightRoll()
        {
            float sx = 416;
            float sy = 384 + 9 + 14;

            float pos = sx - (_ship.Roll * 28 / _ship.MaxRoll);
            pos += 32;

            for (int i = 0; i < 4; i++)
            {
                _gfx.DrawLine(new(pos + i, sy), new(pos + i, sy + 7), GFX_COL.GFX_COL_GOLD);
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
                _gfx.DrawLine(new(pos + i, sy), new(pos + i, sy + 7), GFX_COL.GFX_COL_GOLD);
            }
        }

        private void DisplayFuel()
        {
            if (_ship.Fuel > 0)
            {
                DisplayDialBar(_ship.Fuel * 64 / _ship.MaxFuel, new(31, 44));
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
    }
}
