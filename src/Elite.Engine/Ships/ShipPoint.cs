// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;

namespace Elite.Engine.Ships
{
    internal sealed class ShipPoint
    {
        internal ShipPoint(Vector3 point, int distance, int face1, int face2, int face3, int face4)
        {
            Point = point;
            Distance = distance;
            Face1 = face1;
            Face2 = face2;
            Face3 = face3;
            Face4 = face4;
        }

        internal int Distance { get; set; }
        internal int Face1 { get; set; }
        internal int Face2 { get; set; }
        internal int Face3 { get; set; }
        internal int Face4 { get; set; }
        internal Vector3 Point { get; set; }
    };
}
