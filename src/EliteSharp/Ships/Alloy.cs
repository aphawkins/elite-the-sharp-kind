// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Enums;
using EliteSharp.Trader;

namespace EliteSharp.Ships
{
    internal sealed class Alloy : NullObject
    {
        internal Alloy()
        {
            Type = ShipType.Alloy;
            Flags = ShipFlags.Inactive;
            EnergyMax = 16;
            FaceNormals = new ShipFaceNormal[]
            {
                new(0, new(0, 0, 0)),
            };
            Faces = new ShipFace[]
            {
                new(Colour.LightGrey, new(0x00, 0x00, 0x00), new[] { 0, 1, 2, 3 }),
                new(Colour.DarkerGrey, new(0x00, 0x00, 0x00), new[] { 3, 2, 1, 0, 0, 0, 0, 0 }),
            };
            Lines = new ShipLine[]
            {
                new(31, 15, 15,  0,  1),
                new(16, 15, 15,  1,  2),
                new(20, 15, 15,  2,  3),
                new(16, 15, 15,  3,  0),
            };
            Name = "Alloy";
            Points = new ShipPoint[]
            {
                new(new(-15,  -22,   -9), 31, 15, 15, 15, 15),
                new(new(-15,   38,   -9), 31, 15, 15, 15, 15),
                new(new(19,   32,   11), 20, 15, 15, 15, 15),
                new(new(10,  -46,    6), 20, 15, 15, 15, 15),
            };
            ScoopedType = StockType.Alloys;
            Size = 100;
            Class = ShipClass.SpaceJunk;
            VanishPoint = 5;
            VelocityMax = 16;
        }
    }
}
