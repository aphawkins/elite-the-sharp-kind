// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using EliteSharp.Conflict;
using EliteSharp.Graphics;
using EliteSharp.Ships;

namespace EliteSharp
{
    /// <summary>
    /// This module handles all the flight system and management of the space universe.
    /// </summary>
    internal sealed class Scanner
    {
        private readonly Combat _combat;
        private readonly IDraw _draw;
        private readonly GameState _gameState;
        private readonly PlayerShip _ship;
        private readonly Universe _universe;
        private Vector2 _scannerCentre;

        internal Scanner(GameState gameState, IDraw draw, Universe universe, PlayerShip ship, Combat combat)
        {
            _gameState = gameState;
            _draw = draw;
            _universe = universe;
            _ship = ship;
            _combat = combat;
            _scannerCentre = new(_draw.Centre.X - 3, _draw.ScannerTop + 63);
        }

        internal void DrawScanner() => _draw.Graphics.DrawImage(Image.Scanner, new(_draw.ScannerLeft, _draw.ScannerTop));

        internal void UpdateConsole()
        {
            DrawScanner();
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

            if (_universe.IsStationPresent)
            {
                _draw.Graphics.DrawImage(Image.BigS, new(_draw.ScannerLeft + 387, _draw.ScannerTop + 105));
            }

            if (_ship.EcmActive != 0)
            {
                _draw.Graphics.DrawImage(Image.BigE, new(_draw.ScannerLeft + 115, _draw.ScannerTop + 105));
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
        private void DisplayDialBar(float len, Vector2 position)
        {
            float x = _draw.ScannerLeft + position.X;
            float y = _draw.ScannerTop + position.Y;

            _draw.Graphics.DrawLine(new(x, y), new(x + len, y), EColor.Gold);
            int i = 1;
            _draw.Graphics.DrawLine(new(x, y + i), new(x + len, y + i), EColor.Gold);

            for (i = 2; i < 7; i++)
            {
                _draw.Graphics.DrawLine(new(x, y + i), new(x + len, y + i), EColor.DarkYellow);
            }

            _draw.Graphics.DrawLine(new(x, y + i), new(x + len, y + i), EColor.LightRed);
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
            float x = _draw.ScannerLeft + 416;
            float y = _draw.ScannerTop + 9 + 14 + 16;
            float position = x + (_ship.Climb * 28 / _ship.MaxClimb);
            position += 32;

            for (int i = 0; i < 4; i++)
            {
                _draw.Graphics.DrawLine(new(position + i, y), new(position + i, y + 7), EColor.Gold);
            }
        }

        private void DisplayFlightRoll()
        {
            float x = _draw.ScannerLeft + 416;
            float y = _draw.ScannerTop + 9 + 14;
            float position = x - (_ship.Roll * 28 / _ship.MaxRoll);
            position += 32;

            for (int i = 0; i < 4; i++)
            {
                _draw.Graphics.DrawLine(new(position + i, y), new(position + i, y + 7), EColor.Gold);
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

            Vector2 location = new(_draw.ScannerLeft + ((4 - missileCount) * 16) + 35, _draw.ScannerTop + 113);

            if (_combat.IsMissileArmed)
            {
                _draw.Graphics.DrawImage((_combat.MissileTarget == null) ? Image.MissileYellow : Image.MissileRed, location);
                location.X += 16;
                missileCount--;
            }

            for (; missileCount > 0; missileCount--)
            {
                _draw.Graphics.DrawImage(Image.MissileGreen, location);
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
            float x = _draw.ScannerLeft + 417;
            float y = _draw.ScannerTop + 9;
            float length = (_ship.Speed * 64 / _ship.MaxSpeed) - 1;
            EColor colour = (_ship.Speed > (_ship.MaxSpeed * 2 / 3)) ? EColor.LightRed : EColor.Gold;

            for (int i = 0; i < 6; i++)
            {
                _draw.Graphics.DrawLine(new(x, y + i), new(x + length, y + i), colour);
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

            IObject? obj = _universe.IsStationPresent ? _universe.StationOrSun : _universe.Planet;
            if (obj == null)
            {
                return;
            }

            Vector3 dest = VectorMaths.UnitVector(obj!.Location);

            if (float.IsNaN(dest.X))
            {
                return;
            }

            Vector2 position = new(_draw.ScannerLeft + 382 + (dest.X * 16), _draw.ScannerTop + 22 + (dest.Y * -16));

            if (dest.Z < 0)
            {
                _draw.Graphics.DrawImage(Image.DotRed, position);
            }
            else
            {
                _draw.Graphics.DrawImage(Image.GreenDot, position);
            }
        }

        /// <summary>
        /// Update the scanner and draw all the lollipops.
        /// </summary>
        private void UpdateScanner()
        {
            foreach (IObject obj in _universe.GetAllObjects())
            {
                if ((obj.Type <= 0) ||
                    obj.Flags.HasFlag(ShipFlags.Dead) ||
                    obj.Flags.HasFlag(ShipFlags.Cloaked))
                {
                    continue;
                }

                float x = obj.Location.X / 256;
                float y1 = -obj.Location.Z / 1024;
                float y2 = y1 - (obj.Location.Y / 512);

                if ((y2 < -28) || (y2 > 28) ||
                    (x < -50) || (x > 50))
                {
                    continue;
                }

                x += _scannerCentre.X;
                y1 += _scannerCentre.Y;
                y2 += _scannerCentre.Y;

                EColor colour = obj.Flags.HasFlag(ShipFlags.Hostile) ? EColor.Yellow : EColor.White;

                if (obj.Flags.HasFlag(ShipFlags.Station))
                {
                    colour = EColor.Green;
                }
                else if (obj.Type == ShipType.Missile)
                {
                    colour = EColor.Lilac;
                }
                else if (obj.Flags.HasFlag(ShipFlags.Police))
                {
                    colour = EColor.Purple;
                }

                // ship
                _draw.Graphics.DrawRectangleFilled(new(x - 3, y2), 5, 3, colour);

                // stick
                _draw.Graphics.DrawRectangleFilled(new(x, y2 < y1 ? y2 : y1), 2, MathF.Abs(y2 - y1), colour);
            }
        }
    }
}
