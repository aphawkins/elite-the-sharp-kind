// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Enums;
using EliteSharp.Lasers;
using EliteSharp.Ships;

namespace EliteSharp.Views
{
    internal sealed class PilotView : IView
    {
        private readonly GameState _gameState;
        private readonly IGraphics _graphics;
        private readonly IKeyboard _keyboard;
        private readonly LaserDraw _laser;
        private readonly Pilot _pilot;
        private readonly PlayerShip _ship;
        private readonly Stars _stars;
        private readonly Space _space;
        private int _drawLaserFrames;

        internal PilotView(GameState gameState, IGraphics graphics, IKeyboard keyboard, Pilot pilot, PlayerShip ship, Stars stars, Space space)
        {
            _gameState = gameState;
            _graphics = graphics;
            _keyboard = keyboard;
            _laser = new LaserDraw(_gameState, _graphics);
            _pilot = pilot;
            _ship = ship;
            _stars = stars;
            _space = space;
        }

        public void Draw()
        {
            if (_drawLaserFrames > 0)
            {
                _laser.DrawLaserLines();
            }

            if (_space.HyperGalactic)
            {
                _graphics.DrawTextCentre(358, "Galactic Hyperspace", 120, Colour.White);
            }
            else if (_space.HyperCountdown > 0)
            {
                _graphics.DrawTextCentre(358, $"Hyperspace - {_space.HyperName}", 120, Colour.White);
            }
        }

        public void HandleInput()
        {
            if (_keyboard.IsKeyPressed(CommandKey.Up, CommandKey.UpArrow))
            {
                if (_ship.Climb > 0)
                {
                    _ship.Climb = 0;
                }
                else
                {
                    _ship.DecreaseClimb();
                    _ship.DecreaseClimb();
                }

                _ship.IsClimbing = true;
            }

            if (_keyboard.IsKeyPressed(CommandKey.Down, CommandKey.DownArrow))
            {
                if (_ship.Climb < 0)
                {
                    _ship.Climb = 0;
                }
                else
                {
                    _ship.IncreaseClimb();
                    _ship.IncreaseClimb();
                }

                _ship.IsClimbing = true;
            }

            if (_keyboard.IsKeyPressed(CommandKey.Left, CommandKey.LeftArrow))
            {
                if (_ship.Roll < 0)
                {
                    _ship.Roll = 0;
                }
                else
                {
                    _ship.IncreaseRoll();
                    _ship.IncreaseRoll();
                    _ship.IsRolling = true;
                }
            }

            if (_keyboard.IsKeyPressed(CommandKey.Right, CommandKey.RightArrow))
            {
                if (_ship.Roll > 0)
                {
                    _ship.Roll = 0;
                }
                else
                {
                    _ship.DecreaseRoll();
                    _ship.DecreaseRoll();
                    _ship.IsRolling = true;
                }
            }

            if (_keyboard.IsKeyPressed(CommandKey.DockingComputerOff))
            {
                _pilot.DisengageAutoPilot();
            }
        }

        public void Reset() => _stars.FlipStars();

        public void UpdateUniverse() => _drawLaserFrames = _gameState.DrawLasers ? 2 : Math.Clamp(_drawLaserFrames - 1, 0, _drawLaserFrames);

        internal void DrawLaserSights(LaserType laserType) => _laser.DrawLaserSights(laserType);

        internal void DrawViewName(string name) => _graphics.DrawTextCentre(32, name, 120, Colour.White);
    }
}
