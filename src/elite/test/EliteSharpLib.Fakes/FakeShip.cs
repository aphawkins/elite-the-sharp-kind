// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using EliteSharpLib.Ships;
using EliteSharpLib.Trader;
using Useful.Assets.Models;
using Useful.Maths;

namespace EliteSharpLib.Fakes;

internal sealed class FakeShip : IShip
{
    public FakeShip()
    {
        Model = ModelReader.None;
        Name = "FakeShip";
        Rotmat = Matrix4x4.Identity.ToVector4Array();
    }

    public ThreeDModel Model { get; set; }

    public int Acceleration { get; set; }

    public float Bounty { get; set; }

    public int Bravery { get; set; }

    public int Energy { get; set; }

    public int EnergyMax { get; set; }

    public int ExpDelta { get; set; }

    public int LaserFront { get; set; }

    public int LaserStrength { get; set; }

    public int LootMax { get; set; }

    public float MinDistance { get; set; }

    public int Missiles { get; set; }

    public int MissilesMax { get; set; }

    public string Name { get; set; }

    public StockType ScoopedType { get; set; }

    public float Size { get; set; }

    public IObject? Target { get; set; }

    public int VanishPoint { get; set; }

    public float Velocity { get; set; }

    public float VelocityMax { get; set; }

    public ShipProperties Flags { get; set; }

    public Vector4 Location { get; set; }

    public Vector4[] Rotmat { get; set; }

    public float RotX { get; set; }

    public float RotZ { get; set; }

    public ShipType Type { get; set; }

    public IObject Clone() => this;

    public void Draw()
    {
    }
}
