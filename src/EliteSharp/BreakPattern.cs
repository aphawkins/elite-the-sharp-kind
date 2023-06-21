﻿// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Graphics;
using EliteSharp.Views;

namespace EliteSharp
{
    internal sealed class BreakPattern
    {
        private readonly IGraphics _graphics;
        private readonly Draw _draw;
        private int _breakPatternCount;

        internal BreakPattern(IGraphics graphics, Draw draw)
        {
            _graphics = graphics;
            _draw = draw;
        }

        internal bool IsComplete { get; private set; }

        internal void Draw()
        {
            // Draw a break pattern (for launching, docking and hyperspacing).
            // Just draw a very simple one for the moment.
            for (int i = 0; i < _breakPatternCount; i++)
            {
                _graphics.DrawCircle(new(256, 192), 30 + (i * 15), Colour.White);
            }
        }

        internal void Reset()
        {
            _draw.SetDisplayClipRegion();
            _breakPatternCount = 0;
            IsComplete = false;
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
    }
}
