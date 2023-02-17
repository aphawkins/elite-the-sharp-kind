namespace Elite.Engine.Views
{
    using Elite.Common.Enums;
    using Elite.Engine.Enums;

    internal class BreakPattern : IView
    {
        private readonly IGfx _gfx;
        private readonly Audio _audio;
        private readonly space _space;
        private int breakPatternCount;

        internal BreakPattern(IGfx gfx, Audio audio, space space)
        {
            _gfx = gfx;
            _audio = audio;
            _space = space;
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
            _audio.PlayEffect(SoundEffect.Launch);
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
                    _space.launch_player();
                }
            }
        }
    }
}