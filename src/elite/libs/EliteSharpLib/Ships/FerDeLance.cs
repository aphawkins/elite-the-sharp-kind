// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Graphics;

namespace EliteSharpLib.Ships;

internal sealed class FerDeLance : ShipBase
{
    internal FerDeLance(IEliteDraw draw)
        : base(draw)
    {
        Type = ShipType.FerDeLance;
        Flags = ShipProperties.LoneWolf | ShipProperties.Police;
        EnergyMax = 160;
        LaserStrength = 9;
        MinDistance = 384;
        MissilesMax = 2;
        Name = "Fer-de-Lance";
        Size = 1600;
        VanishPoint = 40;
        VelocityMax = 30;
    }
}
