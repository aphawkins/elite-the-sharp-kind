// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Graphics;

namespace EliteSharp
{
    internal sealed class BreakPattern
    {
        private const int MaxRings = 20;
        private readonly IDraw _draw;
        private int _breakPatternCount;

        internal BreakPattern(IDraw draw) => _draw = draw;

        internal bool IsComplete { get; private set; }

        internal void Draw()
        {
            // Draw a break pattern (for launching, docking and hyperspacing).
            // Just draw a very simple one for the moment.
            for (int i = 0; i < _breakPatternCount; i++)
            {
                _draw.Graphics.DrawCircle(_draw.Centre, 30 + (i * _draw.Centre.X / MaxRings), EColors.White);
            }
        }

        internal void Reset()
        {
            _draw.SetViewClipRegion();
            _breakPatternCount = 0;
            IsComplete = false;
        }

        internal void Update()
        {
            _breakPatternCount++;

            if (_breakPatternCount >= MaxRings)
            {
                _breakPatternCount = 0;
                IsComplete = true;
            }
        }
    }
}
