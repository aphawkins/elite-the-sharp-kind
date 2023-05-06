// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

namespace Elite.Engine
{
    public partial class EliteMain
    {
        internal class FrameCounter
        {
            internal int drawn = 0;
            internal List<long> framesDrawn = new();
            internal int missed = 0;
        }
    }
}
