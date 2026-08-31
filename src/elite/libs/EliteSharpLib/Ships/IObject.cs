// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;

namespace EliteSharpLib.Ships;

internal interface IObject
{
    /// <summary>
    /// Gets or sets what this object is, by the id the ship table files it
    /// under - or <see cref="ObjectIds.Planet"/>, <see cref="ObjectIds.Sun"/>
    /// or <see cref="ObjectIds.None"/> for the three that are not ships.
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// Gets or sets what this object is, as against what it is doing. Set once
    /// when it is built - see <see cref="ShipTraits"/>.
    /// </summary>
    public ShipTraits Traits { get; set; }

    public ShipProperties Flags { get; set; }

    public Vector4 Location { get; set; }

    public Matrix4x4 Rotmat { get; set; }

    public float RotX { get; set; }

    public float RotZ { get; set; }

    public IObject Clone();

    public void Draw();
}
