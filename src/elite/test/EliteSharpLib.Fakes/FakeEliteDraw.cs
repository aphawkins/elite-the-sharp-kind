// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using EliteSharpLib.Graphics;
using EliteSharpLib.Ships;
using Useful.Assets.Palettes;
using Useful.Fakes.Assets;
using Useful.Graphics;
using Useful.Graphics.Fakes;

namespace EliteSharpLib.Fakes;

internal class FakeEliteDraw : IEliteDraw
{
    public float Bottom => 511;

    public Vector2 Centre => new(255, 255);

    public IGraphics Graphics { get; set; } = new FakeGraphics();

    public float Left => 0;

    public IPaletteCollection Palette => new FakePalette();

    public float Offset { get; }

    public float Right => 511;

    public float ScannerLeft { get; }

    public float ScannerRight { get; }

    public float ScannerTop { get; }

    public float Top => 0;

    public void DrawBorder()
    {
    }

    public void DrawHyperspaceCountdown(int countdown)
    {
    }

    public void DrawObject(IObject obj)
    {
    }

    public void DrawPolygonFilled(Vector2[] points, uint faceColor, float averageZ)
    {
    }

    public void DrawTextPretty(Vector2 position, float width, string text)
    {
    }

    public void DrawViewHeader(string title)
    {
    }

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
