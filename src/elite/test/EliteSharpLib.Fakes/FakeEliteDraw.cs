// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using EliteSharpLib.Graphics;
using EliteSharpLib.Ships;
using EliteSharpLib.Views;
using Useful;
using Useful.Assets;
using Useful.Assets.Palettes;
using Useful.Fakes.Assets;
using Useful.Graphics;
using Useful.Graphics.Fakes;

namespace EliteSharpLib.Fakes;

internal class FakeEliteDraw : IEliteDraw
{
    public ViewLayout Layout { get; set; } = new(512, 512, new(512, 129), 2);

    public SystemTier Tier { get; set; } = SystemTier.SixteenBit;

    public float Focus => 512;

    public IGraphics Graphics { get; set; } = new FakeGraphics();

    public IPaletteCollection Palette => new FakePalette();

    public List<(Vector2[] Points, float[] Depths, FastColor FaceColor, float Z)> DrawnPolygons { get; } = [];

    public void DrawObject(IObject obj)
    {
    }

    public void DrawPolygonFilled(Vector2[] points, float[] depths, FastColor faceColor, float z)
        => DrawnPolygons.Add((points, depths, faceColor, z));

    public void RenderEnd()
    {
    }

    public void RenderStart()
    {
    }

    public void SetFullScreenClipRegion()
    {
    }

    public void SetViewClipRegion()
    {
    }
}
