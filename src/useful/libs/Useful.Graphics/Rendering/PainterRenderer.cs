// 'Useful Libraries' - Andy Hawkins 2025.

using System.Numerics;

namespace Useful.Graphics.Rendering;

// The classic painter's algorithm: the back-to-front chain alone decides
// occlusion via a plain (non-depth-tested) fill, unlike ZBufferRenderer's
// per-pixel z-buffer test — solid polygons only, wireframe is a separate
// WireframeRenderer selected instead of this at DI-registration time.
public sealed class PainterRenderer(IGraphics graphics) : IPolygonRenderer
{
    private const int MAXPOLYS = 100;
    private readonly IGraphics _graphics = graphics;
    private readonly PolygonData[] _polyChain = new PolygonData[MAXPOLYS];
    private int _startPoly;
    private int _totalPolys;

    public void Submit(Vector2[] points, uint color, float z)
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

        for (i = 0; i < points.Length; i++)
        {
            _polyChain[x].PointList[i] = points[i];
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

            _graphics.DrawPolygonFilled(_polyChain[i].PointList, _polyChain[i].Color);
        }
    }
}
