// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Graphics;

namespace EliteSharp.Ships;

internal sealed class Gecko : ShipBase
{
    internal Gecko(IDraw draw)
        : base(draw)
    {
        Type = ShipType.Gecko;
        Flags = ShipProperties.PackHunter | ShipProperties.Bold | ShipProperties.Angry;
        Bounty = 5.5f;
        EnergyMax = 70;
        FaceNormals =
        [
            new(31, new(0, 31, 5)),
            new(31, new(4, 45, 8)),
            new(31, new(25, -108, 19)),
            new(31, new(0, -84, 12)),
            new(31, new(-25, -108, 19)),
            new(31, new(-4, 45, 8)),
            new(31, new(-88, 16, -214)),
            new(31, new(0, 0, -187)),
            new(31, new(88, 16, -214)),
        ];
        Faces =
        [
            new(EliteColors.DarkGrey, new(0x00, 0x1F, 0x05), [3, 2, 0, 1]),
            new(EliteColors.LightGrey, new(0x04, 0x2D, 0x08), [3, 1, 5]),
            new(EliteColors.DarkerGrey, new(0x19, -0x6C, 0x13), [5, 1, 7]),

            new(EliteColors.LightGrey, new(0x00, -0x54, 0x0C), [1, 0, 6, 7]),
            new(EliteColors.DarkerGrey, new(-0x19, -0x6C, 0x13), [4, 6, 0]),
            new(EliteColors.LightGrey, new(-0x04, 0x2D, 0x08), [0, 2, 4]),

            new(EliteColors.LightRed, new(-0x58, 0x10, -0xD6), [4, 2, 6]),
            new(EliteColors.LighterRed, new(0x00, 0x00, -0xBB), [2, 3, 7, 6]),
            new(EliteColors.LightRed, new(0x58, 0x10, -0xD6), [5, 7, 3]),

            new(EliteColors.White, new(0x00, -0x54, 0x0C), [8, 10 /*, 9 */]),
            new(EliteColors.White, new(0x00, -0x54, 0x0C), [11, 9 /*, 8 */]),
        ];
        LaserStrength = 8;
        Lines =
        [
            new(31, 0, 3, 0, 1),
            new(31, 1, 2, 1, 5),
            new(31, 1, 8, 5, 3),
            new(31, 0, 7, 3, 2),
            new(31, 5, 6, 2, 4),
            new(31, 4, 5, 4, 0),
            new(31, 2, 8, 5, 7),
            new(31, 3, 7, 7, 6),
            new(31, 4, 6, 6, 4),
            new(29, 0, 5, 0, 2),
            new(30, 0, 1, 1, 3),
            new(29, 3, 4, 0, 6),
            new(30, 2, 3, 1, 7),
            new(20, 6, 7, 2, 6),
            new(20, 7, 8, 3, 7),
            new(16, 3, 3, 8, 10),
            new(17, 3, 3, 9, 11),
        ];
        MinDistance = 384;
        Name = "Gecko";
        Points =
        [
            new(new(-10, -4, 47), 31, 0, 3, 4, 5),
            new(new(10, -4, 47), 31, 0, 1, 2, 3),
            new(new(-16, 8, -23), 31, 0, 5, 6, 7),
            new(new(16, 8, -23), 31, 0, 1, 7, 8),
            new(new(-66, 0, -3), 31, 4, 5, 6, 6),
            new(new(66, 0, -3), 31, 1, 2, 8, 8),
            new(new(-20, -14, -23), 31, 3, 4, 6, 7),
            new(new(20, -14, -23), 31, 2, 3, 7, 8),
            new(new(-8, -6, 33), 16, 3, 3, 3, 3),
            new(new(8, -6, 33), 17, 3, 3, 3, 3),
            new(new(-8, -13, -16), 16, 3, 3, 3, 3),
            new(new(8, -13, -16), 17, 3, 3, 3, 3),
        ];
        Size = 9801;
        VanishPoint = 18;
        VelocityMax = 30;
    }
}
