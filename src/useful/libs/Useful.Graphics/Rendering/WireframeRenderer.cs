// 'Useful Libraries' - Andy Hawkins 2023-2026.

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
    private readonly FastColor _colorWhite;
    private readonly IGraphics _graphics;

    public WireframeRenderer(IGraphics graphics, IAssetLocator assetLocator)
    {
        ArgumentNullException.ThrowIfNull(assetLocator);

        _graphics = graphics;
        _colorWhite = PaletteReader.Read(assetLocator.PalettePath)["White"];
    }

    public void Submit(Vector2[] points, FastColor color, float z)
    {
        ArgumentNullException.ThrowIfNull(points);

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
