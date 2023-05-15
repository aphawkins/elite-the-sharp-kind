// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

namespace Elite.Engine
{
    internal sealed class FrameCounter
    {
        internal int Drawn { get; set; }

        internal List<long> FramesDrawn { get; set; } = new();

        internal int Missed { get; set; }
    }
}
