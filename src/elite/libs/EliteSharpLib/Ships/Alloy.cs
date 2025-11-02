// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Graphics;
using EliteSharpLib.Trader;

namespace EliteSharpLib.Ships;

internal sealed class Alloy : ShipBase
{
    internal Alloy(IEliteDraw draw)
        : base(draw)
    {
        Type = ShipType.Alloy;
        Flags = ShipProperties.SpaceJunk | ShipProperties.Inactive;
        EnergyMax = 16;
        FaceNormals =
        [
            new(0, new(0, 0, 0)),
        ];
        Faces =
        [
            new(EliteColors.LightGrey, new(0x00, 0x00, 0x00), [0, 1, 2, 3]),
            new(EliteColors.DarkerGrey, new(0x00, 0x00, 0x00), [3, 2, 1, 0, 0, 0, 0, 0]),
        ];
        Lines =
        [
            new(31, 15, 15, 0, 1),
            new(16, 15, 15, 1, 2),
            new(20, 15, 15, 2, 3),
            new(16, 15, 15, 3, 0),
        ];
        MinDistance = 200;
        Name = "Alloy";
        Points =
        [
            new(new(-15, -22, -9), 31, 15, 15, 15, 15),
            new(new(-15, 38, -9), 31, 15, 15, 15, 15),
            new(new(19, 32, 11), 20, 15, 15, 15, 15),
            new(new(10, -46, 6), 20, 15, 15, 15, 15),
        ];
        ScoopedType = StockType.Alloys;
        Size = 100;
        VanishPoint = 5;
        VelocityMax = 16;
    }
}
