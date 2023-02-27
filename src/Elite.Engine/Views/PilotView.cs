namespace Elite.Engine.Views
{
    using Elite.Engine.Enums;
    using static Elite.Engine.elite;

    internal abstract class PilotView : IView
    {
        private readonly IGfx _gfx;
        private readonly IKeyboard _keyboard;
        private readonly Laser _laser;
        private readonly pilot _pilot;
        private int drawLaserFrames;

        internal PilotView(IGfx gfx, IKeyboard keyboard, pilot pilot)
        {
            _gfx = gfx;
            _keyboard = keyboard;
            _laser = new Laser(_gfx);
            _pilot = pilot;
        }

        public virtual void Draw()
        {
            if (drawLaserFrames > 0)
            {
                _laser.DrawLaserLines();
            }

            if (space.hyper_galactic)
            {
                _gfx.DrawTextCentre(358, "Galactic Hyperspace", 120, GFX_COL.GFX_COL_WHITE);
            }
            else if (space.hyper_countdown > 0)
            {
                _gfx.DrawTextCentre(358, $"Hyperspace - {space.hyper_name}", 120, GFX_COL.GFX_COL_WHITE);
            }
        }

        public virtual void HandleInput()
        {
            if (_keyboard.IsKeyPressed(CommandKey.Up, CommandKey.UpArrow))
            {
                if (flight_climb > 0)
                {
                    flight_climb = 0;
                }
                else
                {
                    space.decrease_flight_climb();
                    space.decrease_flight_climb();
                }

                elite.climbing = true;
            }
            if (_keyboard.IsKeyPressed(CommandKey.Down, CommandKey.DownArrow))
            {
                if (flight_climb < 0)
                {
                    flight_climb = 0;
                }
                else
                {
                    space.increase_flight_climb();
                    space.increase_flight_climb();
                }
                elite.climbing = true;
            }
            if (_keyboard.IsKeyPressed(CommandKey.Left, CommandKey.LeftArrow))
            {
                if (flight_roll < 0)
                {
                    flight_roll = 0;
                }
                else
                {
                    space.increase_flight_roll();
                    space.increase_flight_roll();
                    elite.rolling = true;
                }
            }
            if (_keyboard.IsKeyPressed(CommandKey.Right, CommandKey.RightArrow))
            {
                if (flight_roll > 0)
                {
                    flight_roll = 0;
                }
                else
                {
                    space.decrease_flight_roll();
                    space.decrease_flight_roll();
                    elite.rolling = true;
                }
            }
            if (_keyboard.IsKeyPressed(CommandKey.DockingComputerOff))
            {
                if (auto_pilot)
                {
                    _pilot.disengage_auto_pilot();
                }
            }
        }

        public virtual void Reset()
        {
            Stars.flip_stars();
        }

        public virtual void UpdateUniverse()
        {
            drawLaserFrames = elite.drawLasers ? 2 : Math.Clamp(drawLaserFrames - 1, 0, drawLaserFrames);
        }

        protected void DrawViewName(string name)
        {
            _gfx.DrawTextCentre(32, name, 120, GFX_COL.GFX_COL_WHITE);
        }

        protected void DrawLaserSights(int laserType)
        {
            _laser.DrawLaserSights(laserType);
        }
    }
}
