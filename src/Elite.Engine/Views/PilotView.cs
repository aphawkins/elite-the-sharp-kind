namespace Elite.Engine.Views
{
    using Elite.Engine.Enums;

    internal abstract class PilotView : IView
    {
        private readonly IGfx _gfx;
        private Laser _laser;

        internal PilotView(IGfx gfx)
        {
            _gfx = gfx;
            _laser = new Laser(_gfx);
        }

        public virtual void Draw()
        {
            if (elite.draw_lasers != 0)
            {
                _laser.DrawLaserLines();
                elite.draw_lasers--;
            }
        }

        public virtual void HandleInput()
        {
        }

        public virtual void Reset()
        {
            Stars.flip_stars();
        }

        public virtual void UpdateUniverse()
        {
        }

        protected void DrawViewName(string name)
        {
            _gfx.DrawTextCentre(32, name, 120, GFX_COL.GFX_COL_WHITE);
        }

        protected void DrawLaserSights(int laserType)
        {
            _laser.draw_laser_sights(laserType);
        }
    }
}
