// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using EliteSharpLib.Graphics;
using EliteSharpLib.Ships;
using Useful.Assets.Models;
using Useful.Maths;

namespace EliteSharpLib.Fakes;

internal sealed class FakeShip : ShipBase
{
    public FakeShip(IEliteDraw draw)
        : base(draw)
    {
        Model = ModelReader.None;
        Name = "FakeShip";
        Rotmat = Matrix4x4.Identity.ToVector4Array();
    }
}
