// 'Useful Libraries' - Andy Hawkins 2023-2026.

namespace Useful.Graphics.Rendering;

// How the game draws its 3D world: outlines only, or filled faces. It applies
// to everything - ships, lasers, planets and the sun - so the picture cannot
// end up half one and half the other.
public enum GraphicStyle
{
    Wireframe = 0,
    Solid = 1,
}
