namespace Elite.Engine.Views
{
    using Elite.Engine.Enums;

    internal class BreakPattern : IView
    {
        private readonly IGfx _gfx;
        private int breakPatternCount;

        internal BreakPattern(IGfx gfx)
        {
            _gfx = gfx;
        }

        public void Draw()
        {
            // Draw a break pattern (for launching, docking and hyperspacing).
            // Just draw a very simple one for the moment.
            for (int i = 0; i < breakPatternCount; i++)
            {
                _gfx.DrawCircle(new(256, 192), 30 + (i * 15), GFX_COL.GFX_COL_WHITE);
            }
        }

        public void HandleInput()
        {
        }

        public void Reset()
        {
            _gfx.SetClipRegion(1, 1, 510, 383);
            breakPatternCount = 0;
        }

        public void UpdateUniverse()
        {
            breakPatternCount++;

            if (breakPatternCount >= 20)
            {
                breakPatternCount = 0;

                if (elite.docked)
                {
                    elite.SetView(SCR.SCR_MISSION);
                }
                else
                {
                    elite.SetView(SCR.SCR_FRONT_VIEW);
                }
            }
        }
    }
}