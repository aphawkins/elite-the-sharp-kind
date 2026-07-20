// 'Useful Libraries' - Andy Hawkins 2025.

using System.Numerics;

namespace Useful.Graphics.Rendering;

// The depth-sort/fill strategy behind a 3D-to-2D polygon pipeline,
// isolated so algorithms (painter's, z-buffer, wireframe) can be swapped
// by DI registration instead of editing the caller directly.
public interface IPolygonRenderer
{
    // Buffer one polygon for the current frame; z is its flat depth (see
    // the caller's projection code for why flat rather than per-vertex).
    public void Submit(Vector2[] points, FastColor color, float z);

    public void StartFrame();

    public void EndFrame();
}
