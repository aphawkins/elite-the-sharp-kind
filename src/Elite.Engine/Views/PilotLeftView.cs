namespace Elite.Engine.Views
{
    using Elite.Engine.Enums;

    internal class PilotLeftView : PilotView
    {
        private readonly Stars _stars;

        internal PilotLeftView(IGfx gfx, Stars stars) : base(gfx)
        {
            _stars = stars;
        }

        public override void Draw()
        {
            base.Draw();
            DrawViewName("Left View");
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
            _stars.LeftStarfield();
        }
    }
}
