// 'SharpKind Libraries' - Andy Hawkins 2023-2026.

using System.Numerics;

namespace SharpKind.Graphics.Rendering;

// The depth-sort/fill strategy behind a 3D-to-2D polygon pipeline,
// isolated so algorithms (painter's, z-buffer, wireframe) can be swapped
// by DI registration instead of editing the caller directly.
public interface IPolygonRenderer
{
    // Buffer one polygon for the current frame. depths gives the
    // camera-space depth at each point, parallel to points, for a per-pixel
    // depth test. z is a single whole-polygon key, for the strategies that
    // order polygons rather than pixels.
    public void Submit(Vector2[] points, float[] depths, FastColor color, float z);

    // As Submit, carrying a quantiser the fill has to ask per pixel - which
    // only a dither does. Null when the caller already resolved the colour.
    public void Submit(Vector2[] points, float[] depths, FastColor color, float z, IColourQuantiser? dither);

    // As Submit, with a colour per point rather than one for the whole
    // polygon (Gouraud shading): colours pairs with points, and the quantiser
    // is asked per pixel whether or not it dithers, since an interpolated
    // colour cannot be resolved once by the caller. A strategy that cannot
    // blend across a face - the painter's chain, the wireframe outline -
    // stands the colours down to one, so a caller submits per-vertex colours
    // without first asking whether the strategy in force can use them.
    public void Submit(Vector2[] points, float[] depths, FastColor[] colours, float z, IColourQuantiser? quantiser);

    public void StartFrame();

    public void EndFrame();
}
