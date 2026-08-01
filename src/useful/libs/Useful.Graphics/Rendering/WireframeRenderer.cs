// 'Useful Libraries' - Andy Hawkins 2023-2026.

using System.Numerics;
using Useful.Assets;
using Useful.Assets.Palettes;

namespace Useful.Graphics.Rendering;

// Outline-only rendering with hidden-line removal: every submitted polygon
// draws as a white outline, but only where nothing solid stands in front of
// it. Backface culling alone cannot do this - it removes surfaces turned
// away from the camera, not surfaces facing it from behind something else,
// and ship models are not convex (a fin is a deliberately double-sided
// plate, so one of its two faces survives any cull whichever side you view
// it from). So the frame is buffered, every surface is written to the depth
// buffer without being drawn, and the outlines are then drawn depth-tested
// against it.
public sealed class WireframeRenderer : IPolygonRenderer
{
    private readonly FastColor _colorWhite;
    private readonly IGraphics _graphics;
    private PolygonData[] _polys = new PolygonData[PolygonBuffer.InitialCapacity];
    private int _totalPolys;

    public WireframeRenderer(IGraphics graphics, IAssetLocator assetLocator)
    {
        ArgumentNullException.ThrowIfNull(assetLocator);

        _graphics = graphics;
        _colorWhite = PaletteReader.Read(assetLocator.PalettePath)["White"];
    }

    public void Submit(Vector2[] points, float[] depths, FastColor color, float z)
    {
        ArgumentNullException.ThrowIfNull(points);
        ArgumentNullException.ThrowIfNull(depths);

        PolygonBuffer.EnsureCapacity(ref _polys, _totalPolys + 1);

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
        // Every surface occludes, even though none of them is drawn, and
        // each is tagged so its own edges can recognise it. A 2-point detail
        // line is not a surface: it occludes nothing, and takes the id of
        // whatever it lies on rather than one of its own - which is the id
        // its own submission would carry, so nothing special is needed.
        for (int i = 0; i < _totalPolys; i++)
        {
            if (_polys[i].PointList.Length > 2)
            {
                _graphics.FillDepth(_polys[i].PointList, _polys[i].Depths, SurfaceId(i));
            }
        }

        for (int i = 0; i < _totalPolys; i++)
        {
            DrawOutline(_polys[i].PointList, _polys[i].Depths, SurfaceId(i));
        }
    }

    // Ids are 1-based: 0 means "no surface", which never matches.
    private static int SurfaceId(int polyIndex) => polyIndex + 1;

    private void DrawOutline(Vector2[] points, float[] depths, int surfaceId)
    {
        if (points.Length == 2)
        {
            DrawEdge(points[0], points[1], depths[0], depths[1], surfaceId);
            return;
        }

        for (int i = 0; i < points.Length - 1; i++)
        {
            DrawEdge(points[i], points[i + 1], depths[i], depths[i + 1], surfaceId);
        }

        DrawEdge(points[^1], points[0], depths[^1], depths[0], surfaceId);
    }

    // No depth bias: an edge lies exactly on the surface it bounds, and the
    // id settles that tie by identity instead. A bias would have to be large
    // enough to cover a near edge-on face's depth gradient, which is more
    // than the gap between a wing and the hull behind it - so any value that
    // kept an edge visible also let hidden ones leak through.
    private void DrawEdge(Vector2 start, Vector2 end, float depthStart, float depthEnd, int surfaceId)
        => _graphics.DrawLineDepth(start, end, depthStart, depthEnd, _colorWhite, surfaceId);
}
