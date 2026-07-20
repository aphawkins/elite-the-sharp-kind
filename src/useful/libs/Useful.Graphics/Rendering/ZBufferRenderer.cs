// 'Useful Libraries' - Andy Hawkins 2025.

using System.Numerics;

namespace Useful.Graphics.Rendering;

// Per-pixel depth test via Graphics.DrawPolygonFilledDepth: solid
// polygons only, wireframe is a separate WireframeRenderer selected
// instead of this at DI-registration time. Correct tie-breaking for
// coplanar polygons submitted with the same z (e.g. decals sitting on a
// base surface) depends on the caller's projection code submitting a
// stable z key and on submission order for exact ties.
public sealed class ZBufferRenderer(IGraphics graphics) : IPolygonRenderer
{
    private const int MAXPOLYS = 100;
    private readonly IGraphics _graphics = graphics;
    private readonly PolygonData[] _polyChain = new PolygonData[MAXPOLYS];
    private int _startPoly;
    private int _totalPolys;

    public void Submit(Vector2[] points, FastColor color, float z)
    {
        Guard.ArgumentNull(points);

        int i;

        if (_totalPolys == MAXPOLYS)
        {
            return;
        }

        int x = _totalPolys;
        _totalPolys++;

        _polyChain[x].Color = color;
        _polyChain[x].Z = z;
        _polyChain[x].Next = -1;
        _polyChain[x].PointList = new Vector2[points.Length];
        _polyChain[x].Depths = new float[points.Length];

        for (i = 0; i < points.Length; i++)
        {
            _polyChain[x].PointList[i] = points[i];
            _polyChain[x].Depths[i] = z;
        }

        if (x == 0)
        {
            return;
        }

        if (z > _polyChain[_startPoly].Z)
        {
            _polyChain[x].Next = _startPoly;
            _startPoly = x;
            return;
        }

        for (i = _startPoly; _polyChain[i].Next != -1; i = _polyChain[i].Next)
        {
            int nx = _polyChain[i].Next;

            if (z > _polyChain[nx].Z)
            {
                _polyChain[i].Next = x;
                _polyChain[x].Next = nx;
                return;
            }
        }

        _polyChain[i].Next = x;
    }

    public void StartFrame()
    {
        _startPoly = 0;
        _totalPolys = 0;
        _graphics.ClearDepth();
    }

    public void EndFrame()
    {
        if (_totalPolys == 0)
        {
            return;
        }

        for (int i = _startPoly; i != -1; i = _polyChain[i].Next)
        {
            if (_polyChain[i].PointList.Length == 2)
            {
                _graphics.DrawLine(_polyChain[i].PointList[0], _polyChain[i].PointList[1], _polyChain[i].Color);
                continue;
            }

            _graphics.DrawPolygonFilledDepth(_polyChain[i].PointList, _polyChain[i].Depths, _polyChain[i].Color);
        }
    }
}
