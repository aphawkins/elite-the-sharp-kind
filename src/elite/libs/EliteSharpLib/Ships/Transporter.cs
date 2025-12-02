// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Graphics;

namespace EliteSharpLib.Ships;

internal sealed class Transporter : ShipBase
{
    internal Transporter(IEliteDraw draw)
        : base(draw)
    {
        Type = ShipType.Transporter;
        Flags = ShipProperties.SpaceJunk | ShipProperties.FlyToPlanet | ShipProperties.Slow;
        EnergyMax = 32;
        LaserFront = 12;
        MinDistance = 200;
        Name = "Transporter";
        Size = 2500;
        VanishPoint = 16;
        VelocityMax = 10;
    }
}
