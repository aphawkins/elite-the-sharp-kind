// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Graphics;

namespace EliteSharpLib.Ships;

internal sealed class CobraMk3Lone : CobraMk3
{
    internal CobraMk3Lone(IEliteDraw draw)
        : base(draw)
    {
        Type = ShipType.CobraMk3Lone;
        Flags = ShipProperties.Bold | ShipProperties.Angry;
        Bounty = 17.5f;
        LootMax = 1;
        MissilesMax = 2;
    }
}
