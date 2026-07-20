// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;

namespace EliteSharpLib.Graphics;

// The depth-sort/fill strategy behind EliteDraw's ship rendering, isolated
// so algorithms (painter's, z-buffer, wireframe) can be swapped by DI
// registration instead of editing EliteDraw directly.
internal interface IShipRenderer
{
    // Buffer one face for the current frame; z is its flat depth (see
    // EliteDraw.DrawPolygonFilled for why flat rather than per-vertex).
    public void SubmitFace(Vector2[] points, uint faceColor, float z);

    public void StartFrame();

    public void EndFrame();
}
