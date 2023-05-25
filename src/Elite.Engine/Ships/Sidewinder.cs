// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using Elite.Engine.Enums;

namespace Elite.Engine.Ships
{
    internal sealed class Sidewinder : Ship
    {
        public override float Bounty => 5;

        public override int EnergyMax => 70;

        public override ShipFaceNormal[] FaceNormals { get; protected set; } =
        {
            new(31, new(0,   32,    8)),
            new(31, new(-12,   47,    6)),
            new(31, new(12,   47,    6)),
            new(31, new(0,    0, -112)),
            new(31, new(-12,  -47,    6)),
            new(31, new(0,  -32,    8)),
            new(31, new(12,  -47,    6)),
        };

        public override ShipFace[] Faces { get; protected set; } =
        {
            new(Colour.DarkYellow, new(0x00, 0x20, 0x08), new[] { 4, 0, 1 }),
            new(Colour.Gold, new(-0x0C, 0x2F, 0x06), new[] { 4, 3, 0 }),
            new(Colour.Gold, new(0x0C, 0x2F, 0x06), new[] { 2, 4, 1 }),

            new(Colour.LightGrey, new(0x00, 0x00, -0x70), new[] { 2, 5, 3, 4 }),

            new(Colour.DarkYellow, new(-0x0C, -0x2F, 0x06), new[] { 5, 0, 3 }),
            new(Colour.Gold, new(0x00, -0x20, 0x08), new[] { 1, 0, 5 }),
            new(Colour.DarkYellow, new(0x0C, -0x2F, 0x06), new[] { 2, 1, 5 }),
            new(Colour.LighterRed, new(0x00, 0x00, -0x70), new[] { 8, 9, 6, 7 }),
        };

        public override int LaserStrength => 8;

        public override ShipLine[] Lines { get; protected set; } =
        {
            new(31,  5,  0,  0,  1),
            new(31,  6,  2,  1,  2),
            new(31,  2,  0,  1,  4),
            new(31,  1,  0,  0,  4),
            new(31,  4,  1,  0,  3),
            new(31,  3,  1,  3,  4),
            new(31,  3,  2,  2,  4),
            new(31,  4,  3,  3,  5),
            new(31,  6,  3,  2,  5),
            new(31,  6,  5,  1,  5),
            new(31,  5,  4,  0,  5),
            new(15,  3,  3,  6,  7),
            new(12,  3,  3,  7,  8),
            new(12,  3,  3,  6,  9),
            new(12,  3,  3,  8,  9),
        };

        public override string Name => "Sidewinder";

        public override ShipPoint[] Points { get; protected set; } =
        {
            new(new(-32,    0,   36), 31,  1,  0,  5,  4),
            new(new(32,    0,   36), 31,  2,  0,  6,  5),
            new(new(64,    0,  -28), 31,  3,  2,  6,  6),
            new(new(-64,    0,  -28), 31,  3,  1,  4,  4),
            new(new(0,   16,  -28), 31,  1,  0,  3,  2),
            new(new(0,  -16,  -28), 31,  4,  3,  6,  5),
            new(new(-12,    6,  -28), 15,  3,  3,  3,  3),
            new(new(12,    6,  -28), 15,  3,  3,  3,  3),
            new(new(12,   -6,  -28), 12,  3,  3,  3,  3),
            new(new(-12,   -6,  -28), 12,  3,  3,  3,  3),
        };

        public override float Size => 4225;

        public override ShipClass Class => ShipClass.PackHunter;

        public override int VanishPoint => 20;

        public override float VelocityMax => 37;
    }
}
