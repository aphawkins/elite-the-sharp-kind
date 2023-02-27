﻿namespace Elite.Engine.Views
{
    using Elite.Engine.Enums;

    internal class PilotRightView : PilotView
    {
        private readonly Stars _stars;

        internal PilotRightView(IGfx gfx, IKeyboard keyboard, Stars stars, pilot pilot) : base(gfx, keyboard, pilot)
        {
            _stars = stars;
        }

        public override void Draw()
        {
            base.Draw();
            DrawViewName("Right View");
            DrawLaserSights(elite.cmdr.front_laser);
        }

        public override void HandleInput()
        {
            base.HandleInput();
        }

        public override void Reset()
        {
            base.Reset();
        }

        public override void UpdateUniverse()
        {
            base.UpdateUniverse();
            _stars.RightStarfield();
        }
    }
}
