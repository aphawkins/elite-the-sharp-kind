// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;

namespace EliteSharp.Ships;

internal interface IObject
{
    public ShipProperties Flags { get; set; }

    public Vector3 Location { get; set; }

    public Vector3[] Rotmat { get; set; }

    public float RotX { get; set; }

    public float RotZ { get; set; }

    public ShipType Type { get; set; }

    public IObject Clone();

    public void Draw();
}
