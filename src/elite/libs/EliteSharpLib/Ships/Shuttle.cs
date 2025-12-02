// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Graphics;

namespace EliteSharpLib.Ships;

internal sealed class Shuttle : ShipBase
{
    internal Shuttle(IEliteDraw draw)
        : base(draw)
    {
        Type = ShipType.Shuttle;
        Flags = ShipProperties.SpaceJunk | ShipProperties.FlyToPlanet | ShipProperties.Slow;
        EnergyMax = 32;
        LootMax = 15;
        MinDistance = 200;
        Name = "Orbit Shuttle";
        Size = 2500;
        VanishPoint = 22;
        VelocityMax = 8;
    }
}
