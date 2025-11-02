// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Graphics;

namespace EliteSharpLib.Ships;

internal sealed class Mamba : ShipBase
{
    internal Mamba(IEliteDraw draw)
        : base(draw)
    {
        Type = ShipType.Mamba;
        Flags = ShipProperties.PackHunter | ShipProperties.Bold | ShipProperties.Angry;
        Bounty = 15;
        EnergyMax = 90;
        FaceNormals =
        [
            new(30, new(0, -24, 2)),
            new(30, new(0, 24, 2)),
            new(30, new(-32, 64, 16)),
            new(30, new(32, 64, 16)),
            new(30, new(0, 0, -127)),
        ];
        Faces =
        [
            new(EliteColors.Green, new(0x00, -0x18, 0x02), [1, 4, 0]),
            new(EliteColors.LighterGreen, new(0x00, 0x18, 0x02), [2, 0, 3]),
            new(EliteColors.LightGreen, new(-0x20, 0x40, 0x10), [1, 0, 2]),
            new(EliteColors.LightGreen, new(0x20, 0x40, 0x10), [3, 0, 4]),

            new(EliteColors.LightGrey, new(0x00, 0x00, -0x7F), [1, 2, 3, 4]),
            new(EliteColors.LightBlue, new(0x00, -0x18, 0x02), [11, 12, 9]),
            new(EliteColors.LighterRed, new(0x00, 0x00, -0x7F), [17, 18, 15, 16]),
            new(EliteColors.Blue, new(0x00, 0x18, 0x02), [7, 6, 5, 8]),
            new(EliteColors.LightBlue, new(0x00, -0x18, 0x02), [13, 14, 10]),
            new(EliteColors.LighterRed, new(0x00, 0x00, -0x7F), [20, 24, 21]),
            new(EliteColors.LighterRed, new(0x00, 0x00, -0x7F), [22, 23, 19]),
        ];
        LaserStrength = 9;
        Lines =
        [
            new(31, 2, 0, 0, 1),
            new(31, 3, 0, 0, 4),
            new(31, 4, 0, 1, 4),
            new(30, 4, 2, 1, 2),
            new(30, 4, 1, 2, 3),
            new(30, 4, 3, 3, 4),
            new(14, 1, 1, 5, 6),
            new(12, 1, 1, 6, 7),
            new(13, 1, 1, 7, 8),
            new(12, 1, 1, 5, 8),
            new(20, 0, 0, 9, 11),
            new(16, 0, 0, 9, 12),
            new(16, 0, 0, 10, 13),
            new(20, 0, 0, 10, 14),
            new(14, 0, 0, 13, 14),
            new(14, 0, 0, 11, 12),
            new(13, 4, 4, 15, 16),
            new(14, 4, 4, 17, 18),
            new(12, 4, 4, 15, 18),
            new(12, 4, 4, 16, 17),
            new(7, 4, 4, 20, 21),
            new(5, 4, 4, 20, 24),
            new(5, 4, 4, 21, 24),
            new(7, 4, 4, 19, 22),
            new(5, 4, 4, 19, 23),
            new(5, 4, 4, 22, 23),
            new(30, 2, 1, 0, 2),
            new(30, 3, 1, 0, 3),
        ];
        LootMax = 1;
        MinDistance = 384;
        MissilesMax = 2;
        Name = "Mamba";
        Points =
        [
            new(new(0, 0, 64), 31, 1, 0, 3, 2),
            new(new(-64, -8, -32), 31, 2, 0, 4, 4),
            new(new(-32, 8, -32), 30, 2, 1, 4, 4),
            new(new(32, 8, -32), 30, 3, 1, 4, 4),
            new(new(64, -8, -32), 31, 3, 0, 4, 4),
            new(new(-4, 4, 16), 14, 1, 1, 1, 1),
            new(new(4, 4, 16), 14, 1, 1, 1, 1),
            new(new(8, 3, 28), 13, 1, 1, 1, 1),
            new(new(-8, 3, 28), 13, 1, 1, 1, 1),
            new(new(-20, -4, 16), 20, 0, 0, 0, 0),
            new(new(20, -4, 16), 20, 0, 0, 0, 0),
            new(new(-24, -7, -20), 20, 0, 0, 0, 0),
            new(new(-16, -7, -20), 16, 0, 0, 0, 0),
            new(new(16, -7, -20), 16, 0, 0, 0, 0),
            new(new(24, -7, -20), 20, 0, 0, 0, 0),
            new(new(-8, 4, -32), 13, 4, 4, 4, 4),
            new(new(8, 4, -32), 13, 4, 4, 4, 4),
            new(new(8, -4, -32), 14, 4, 4, 4, 4),
            new(new(-8, -4, -32), 14, 4, 4, 4, 4),
            new(new(-32, 4, -32), 7, 4, 4, 4, 4),
            new(new(32, 4, -32), 7, 4, 4, 4, 4),
            new(new(36, -4, -32), 7, 4, 4, 4, 4),
            new(new(-36, -4, -32), 7, 4, 4, 4, 4),
            new(new(-38, 0, -32), 5, 4, 4, 4, 4),
            new(new(38, 0, -32), 5, 4, 4, 4, 4),
        ];
        Size = 4900;
        VanishPoint = 25;
        VelocityMax = 30;
    }
}
