// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;

namespace EliteSharp.Ships
{
    internal interface IObject
    {
        ShipProperties Flags { get; set; }

        Vector3 Location { get; set; }

        Vector3[] Rotmat { get; set; }

        float RotX { get; set; }

        float RotZ { get; set; }

        ShipType Type { get; set; }

        IObject Clone();

        void Draw();
    }
}
