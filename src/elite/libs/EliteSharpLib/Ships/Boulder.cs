// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Graphics;

namespace EliteSharpLib.Ships;

internal sealed class Boulder : ShipBase
{
    internal Boulder(IEliteDraw draw)
        : base(draw)
    {
        Type = ShipType.Boulder;
        Flags = ShipProperties.SpaceJunk | ShipProperties.Inactive;
        Bounty = 0.1f;
        EnergyMax = 20;
        MinDistance = 300;
        Name = "Boulder";
        Size = 900;
        VanishPoint = 20;
        VelocityMax = 30;
    }
}
