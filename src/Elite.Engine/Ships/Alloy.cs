// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using Elite.Engine.Enums;
using Elite.Engine.Trader;

namespace Elite.Engine.Ships
{
    internal sealed class Alloy : Ship
    {
        public override int EnergyMax => 16;

        public override ShipFaceNormal[] FaceNormals { get; protected set; } =
        {
            new(0, new(0, 0, 0)),
        };

        public override ShipFace[] Faces { get; protected set; } =
        {
            new(Colour.LightGrey, new(0x00, 0x00, 0x00), new[] { 0, 1, 2, 3 }),
            new(Colour.DarkerGrey, new(0x00, 0x00, 0x00), new[] { 3, 2, 1, 0, 0, 0, 0, 0 }),
        };

        public override ShipLine[] Lines { get; protected set; } =
        {
            new(31, 15, 15,  0,  1),
            new(16, 15, 15,  1,  2),
            new(20, 15, 15,  2,  3),
            new(16, 15, 15,  3,  0),
        };

        public override string Name => "Alloy";

        public override ShipPoint[] Points { get; protected set; } =
        {
            new(new(-15,  -22,   -9), 31, 15, 15, 15, 15),
            new(new(-15,   38,   -9), 31, 15, 15, 15, 15),
            new(new(19,   32,   11), 20, 15, 15, 15, 15),
            new(new(10,  -46,    6), 20, 15, 15, 15, 15),
        };

        public override StockType ScoopedType => StockType.Alloys;

        public override float Size => 100;

        public override ShipClass Class => ShipClass.SpaceJunk;

        public override int VanishPoint => 5;

        public override float VelocityMax => 16;
    }
}
