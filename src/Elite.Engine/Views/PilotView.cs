﻿// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using Elite.Engine.Enums;
using Elite.Engine.Lasers;
using Elite.Engine.Ships;

namespace Elite.Engine.Views
{
    internal class PilotView : IView
    {
        private readonly GameState _gameState;
        private readonly IGfx _gfx;
        private readonly IKeyboard _keyboard;
        private readonly LaserDraw _laser;
        private readonly Pilot _pilot;
        private readonly PlayerShip _ship;
        private int _drawLaserFrames;

        internal PilotView(GameState gameState, IGfx gfx, IKeyboard keyboard, Pilot pilot, PlayerShip ship)
        {
            _gameState = gameState;
            _gfx = gfx;
            _keyboard = keyboard;
            _laser = new LaserDraw(_gameState, _gfx);
            _pilot = pilot;
            _ship = ship;
        }

        public void Draw()
        {
            if (_drawLaserFrames > 0)
            {
                _laser.DrawLaserLines();
            }

            if (Space.hyper_galactic)
            {
                _gfx.DrawTextCentre(358, "Galactic Hyperspace", 120, GFX_COL.GFX_COL_WHITE);
            }
            else if (Space.hyper_countdown > 0)
            {
                _gfx.DrawTextCentre(358, $"Hyperspace - {Space.hyper_name}", 120, GFX_COL.GFX_COL_WHITE);
            }
        }

        public void HandleInput()
        {
            if (_keyboard.IsKeyPressed(CommandKey.Up, CommandKey.UpArrow))
            {
                if (_ship.climb > 0)
                {
                    _ship.climb = 0;
                }
                else
                {
                    _ship.DecreaseClimb();
                    _ship.DecreaseClimb();
                }

                _ship.isClimbing = true;
            }
            if (_keyboard.IsKeyPressed(CommandKey.Down, CommandKey.DownArrow))
            {
                if (_ship.climb < 0)
                {
                    _ship.climb = 0;
                }
                else
                {
                    _ship.IncreaseClimb();
                    _ship.IncreaseClimb();
                }

                _ship.isClimbing = true;
            }
            if (_keyboard.IsKeyPressed(CommandKey.Left, CommandKey.LeftArrow))
            {
                if (_ship.roll < 0)
                {
                    _ship.roll = 0;
                }
                else
                {
                    _ship.IncreaseRoll();
                    _ship.IncreaseRoll();
                    _ship.isRolling = true;
                }
            }
            if (_keyboard.IsKeyPressed(CommandKey.Right, CommandKey.RightArrow))
            {
                if (_ship.roll > 0)
                {
                    _ship.roll = 0;
                }
                else
                {
                    _ship.DecreaseRoll();
                    _ship.DecreaseRoll();
                    _ship.isRolling = true;
                }
            }
            if (_keyboard.IsKeyPressed(CommandKey.DockingComputerOff))
            {
                if (_gameState.IsAutoPilotOn)
                {
                    _pilot.DisengageAutoPilot();
                }
            }
        }

        public void Reset() => Stars.FlipStars();

        public void UpdateUniverse() => _drawLaserFrames = _gameState.DrawLasers ? 2 : Math.Clamp(_drawLaserFrames - 1, 0, _drawLaserFrames);

        internal void DrawLaserSights(LaserType laserType) => _laser.DrawLaserSights(laserType);

        internal void DrawViewName(string name) => _gfx.DrawTextCentre(32, name, 120, GFX_COL.GFX_COL_WHITE);
    }
}
