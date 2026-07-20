// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using Useful.Assets;
using Useful.Assets.Palettes;
using Useful.Graphics;

namespace EliteSharpLib.Graphics;

// Outline-only ship rendering: every submitted face (2-point detail line
// or polygon) draws immediately as a white outline. Unlike the filled
// strategies, drawing order doesn't affect the result, so this needs no
// depth-sort chain at all.
internal sealed class WireframeRenderer : IShipRenderer
{
    private readonly uint _colorWhite;
    private readonly IGraphics _graphics;

    internal WireframeRenderer(IGraphics graphics, IAssetLocator assetLocator)
    {
        _graphics = graphics;
        _colorWhite = PaletteReader.Read(assetLocator.PalettePath)["White"];
    }

    public void SubmitFace(Vector2[] points, uint faceColor, float z)
    {
        if (points.Length == 2)
        {
            _graphics.DrawLine(points[0], points[1], _colorWhite);
        }
        else
        {
            _graphics.DrawPolygon(points, _colorWhite);
        }
    }

    public void StartFrame()
    {
    }

    public void EndFrame()
    {
    }
}
