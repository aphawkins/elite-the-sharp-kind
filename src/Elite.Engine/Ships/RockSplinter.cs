// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using Elite.Engine.Enums;
using Elite.Engine.Trader;

namespace Elite.Engine.Ships
{
    internal sealed class RockSplinter : Ship
    {
        public override int EnergyMax => 20;

        public override ShipFaceNormal[] FaceNormals { get; protected set; } =
        {
            new(18, new(30,    0,    0)),
            new(20, new(22,   32,   -8)),
            new(0, new(0,    2,    0)),
            new(0, new(17,   23,   95)),
        };

        public override ShipFace[] Faces { get; protected set; } =
        {
            new(Colour.LightGrey, new(0x00, 0x00, 0x00), new[] { 3, 2, 1 }),
            new(Colour.DarkGrey, new(0x00, 0x00, 0x00), new[] { 0, 2, 3 }),
            new(Colour.DarkerGrey, new(0x00, 0x00, 0x00), new[] { 3, 1, 0 }),
            new(Colour.Grey, new(0x00, 0x00, 0x00), new[] { 0, 1, 2 }),
        };

        public override ShipLine[] Lines { get; protected set; } =
        {
            new(31,  2,  3,  0,  1),
            new(31,  0,  3,  1,  2),
            new(31,  0,  1,  2,  3),
            new(31,  1,  2,  3,  0),
            new(31,  1,  3,  0,  2),
            new(31,  0,  2,  3,  1),
        };

        public override string Name => "Rock Splinter";

        public override ShipPoint[] Points { get; protected set; } =
        {
            new(new(-24,  -25,   16), 31,  1,  2,  3,  3),
            new(new(0,   12,  -10), 31,  0,  2,  3,  3),
            new(new(11,   -6,    2), 31,  0,  1,  3,  3),
            new(new(12,   42,    7), 31,  0,  1,  2,  2),
        };

        public override StockType ScoopedType => StockType.Minerals;

        public override float Size => 256;

        public override ShipClass Class => ShipClass.SpaceJunk;

        public override int VanishPoint => 8;

        public override float VelocityMax => 10;
    }
}
