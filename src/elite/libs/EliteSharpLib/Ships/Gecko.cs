// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Graphics;

namespace EliteSharpLib.Ships;

internal sealed class Gecko : ShipBase
{
    internal Gecko(IEliteDraw draw)
        : base(draw)
    {
        Type = ShipType.Gecko;
        Flags = ShipProperties.PackHunter | ShipProperties.Bold | ShipProperties.Angry;
        Bounty = 5.5f;
        EnergyMax = 70;
        LaserStrength = 8;
        MinDistance = 384;
        Name = "Gecko";
        Size = 9801;
        VanishPoint = 18;
        VelocityMax = 30;
    }
}
