// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

namespace Elite.Engine.Ships
{
    internal sealed class ShipLine
    {
        internal ShipLine(int distance, int face1, int face2, int startPoint, int endPoint)
        {
            Distance = distance;
            Face1 = face1;
            Face2 = face2;
            StartPoint = startPoint;
            EndPoint = endPoint;
        }

        internal int Distance { get; set; }

        internal int EndPoint { get; set; }

        internal int Face1 { get; set; }

        internal int Face2 { get; set; }

        internal int StartPoint { get; set; }
    }
}
