// 'Useful Libraries' - Andy Hawkins 2023-2026.

namespace Useful.Graphics.Rendering;

// Which depth-sort strategy backs filled polygon rendering (a wireframe
// renderer picks outline-only over either of these, independently).
public enum PolygonRenderMode
{
    Painter = 0,
    ZBuffer = 1,
}
