// 'Useful Libraries' - Andy Hawkins 2025.

using System.Numerics;
using Useful.Assets;
using Useful.Assets.Palettes;

namespace Useful.Graphics.Rendering;

// Outline-only rendering: every submitted polygon (2-point detail line or
// filled shape) draws immediately as a white outline. Unlike the filled
// strategies, drawing order doesn't affect the result, so this needs no
// depth-sort chain at all.
public sealed class WireframeRenderer : IPolygonRenderer
{
    private readonly uint _colorWhite;
    private readonly IGraphics _graphics;

    public WireframeRenderer(IGraphics graphics, IAssetLocator assetLocator)
    {
        Guard.ArgumentNull(assetLocator);

        _graphics = graphics;
        _colorWhite = PaletteReader.Read(assetLocator.PalettePath)["White"];
    }

    public void Submit(Vector2[] points, uint color, float z)
    {
        Guard.ArgumentNull(points);

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
