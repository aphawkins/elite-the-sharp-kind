// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

namespace EliteSharpLib.Graphics;

// Which depth-sort strategy backs filled ship rendering (ShipWireframe
// picks wireframe over either of these, independently).
internal enum ShipRenderMode
{
    Painter = 0,
    ZBuffer = 1,
}
