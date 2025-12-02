// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Graphics;

namespace EliteSharpLib.Ships;

internal sealed class CargoCannister : ShipBase
{
    internal CargoCannister(IEliteDraw draw)
        : base(draw)
    {
        Type = ShipType.Cargo;
        Flags = ShipProperties.SpaceJunk | ShipProperties.Inactive;
        EnergyMax = 17;
        MinDistance = 200;
        Name = "Cargo Cannister";
        Size = 400;
        VanishPoint = 12;
        VelocityMax = 15;
    }
}
