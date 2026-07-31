// 'Useful Libraries' - Andy Hawkins 2023-2026.

using System.Numerics;

namespace Useful.Graphics.Rendering;

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

    public void StartFrame();

    public void EndFrame();
}
