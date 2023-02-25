namespace Elite.Engine
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Elite.Common.Enums;
    using Elite.Engine.Enums;

    internal class BreakPattern
    {
        private readonly IGfx _gfx;
        private int _breakPatternCount;

        internal BreakPattern(IGfx gfx)
        {
            _gfx = gfx;
        }

        internal void Reset()
        {
            _gfx.SetClipRegion(1, 1, 510, 383);
            _breakPatternCount = 0;
            IsComplete = false;
        }

        internal void Draw()
        {
            // Draw a break pattern (for launching, docking and hyperspacing).
            // Just draw a very simple one for the moment.
            for (int i = 0; i < _breakPatternCount; i++)
            {
                _gfx.DrawCircle(new(256, 192), 30 + (i * 15), GFX_COL.GFX_COL_WHITE);
            }
        }

        internal void Update()
        {
            _breakPatternCount++;

            if (_breakPatternCount >= 20)
            {
                _breakPatternCount = 0;
                IsComplete = true;
            }
        }

        internal bool IsComplete { get; private set; }
    }
}