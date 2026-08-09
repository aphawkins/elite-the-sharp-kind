// 'SharpKind Libraries' - Andy Hawkins 2023-2026.

namespace SharpKind.Graphics.Rendering;

// How a primitive becomes pixels: its outline only, or its whole face. The
// pipeline's rasterisation stage, and independent of what colour those pixels
// take (IShadingModel) and how that colour is reduced to what the display can
// show (IColourQuantiser) - though both of those are moot for an outline,
// which has no face to light or shade.
//
// It applies to everything - ships, lasers, planets and the sun - so the
// picture cannot end up half one and half the other.
public enum FillMode
{
    Wireframe = 0,
    Solid = 1,
}
