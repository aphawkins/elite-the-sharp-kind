// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;

namespace EliteSharp.Views
{
    internal interface IDraw
    {
        float Bottom { get; }

        Vector2 Centre { get; }

        float Left { get; }

        float Top { get; }

        void ClearDisplay();

        void SetFullScreenClipRegion();

        void SetViewClipRegion();
    }
}
