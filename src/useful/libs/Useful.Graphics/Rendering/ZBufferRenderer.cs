// 'Useful Libraries' - Andy Hawkins 2023-2026.

using System.Numerics;

namespace Useful.Graphics.Rendering;

// Per-pixel depth test via Graphics.DrawPolygonFilledDepth: solid
// polygons only, wireframe is a separate WireframeRenderer selected
// instead of this by the caller. The per-pixel test decides occlusion on
// its own, so polygons are drawn in submission order and the flat z sort
// key is ignored - unlike PainterRenderer, which is nothing but that
// order. Exactly coplanar surfaces (decals on a hull face) still resolve
// by submission order, because the depth test lets the later draw win a
// tie; the caller is responsible for biasing them if interpolation across
// differently shaped triangles makes the tie inexact. 2-point detail lines
// are depth-tested too, so a line lying on a face turned away from the
// camera is hidden by the hull rather than drawn straight through it.
public sealed class ZBufferRenderer(IGraphics graphics) : IPolygonRenderer
{
    private const int MAXPOLYS = 100;
    private readonly IGraphics _graphics = graphics;
    private readonly PolygonData[] _polys = new PolygonData[MAXPOLYS];
    private int _totalPolys;

    public void Submit(Vector2[] points, float[] depths, FastColor color, float z)
    {
        ArgumentNullException.ThrowIfNull(points);
        ArgumentNullException.ThrowIfNull(depths);

        if (_totalPolys == MAXPOLYS)
        {
            return;
        }

        int x = _totalPolys;
        _totalPolys++;

        _polys[x].Color = color;
        _polys[x].PointList = new Vector2[points.Length];
        _polys[x].Depths = new float[points.Length];

        for (int i = 0; i < points.Length; i++)
        {
            _polys[x].PointList[i] = points[i];
            _polys[x].Depths[i] = depths[i];
        }
    }

    public void StartFrame()
    {
        _totalPolys = 0;
        _graphics.ClearDepth();
    }

    public void EndFrame()
    {
        for (int i = 0; i < _totalPolys; i++)
        {
            if (_polys[i].PointList.Length == 2)
            {
                _graphics.DrawLineDepth(
                    _polys[i].PointList[0],
                    _polys[i].PointList[1],
                    _polys[i].Depths[0],
                    _polys[i].Depths[1],
                    _polys[i].Color,
                    surfaceId: 0);
                continue;
            }

            _graphics.DrawPolygonFilledDepth(_polys[i].PointList, _polys[i].Depths, _polys[i].Color);
        }
    }
}
