// 'SharpKind Libraries' - Andy Hawkins 2023-2026.

using System.Numerics;

namespace SharpKind.Graphics;

// Perspective projection of a camera-space point onto the screen: the centre
// of the viewport plus focus * x / z, with y negated because camera space has
// y up and the screen has y down. Both games write exactly this form, so the
// focal length and the viewport centre are the whole of what a projection is.
// Anything scaled by the focal length but not projected - Elite's world radii,
// say - stays with the game that needs it.
public readonly record struct PerspectiveProjector(float Focus, Vector2 Centre)
{
    // z must be positive: points at or behind the camera plane project to
    // garbage, so clip against the near plane (see NearPlaneClip) first.
    public Vector2 Project(Vector3 cameraPoint) => Project(cameraPoint.X, cameraPoint.Y, cameraPoint.Z);

    public Vector2 Project(float x, float y, float z) => new(
        Centre.X + (Focus * x / z),
        Centre.Y - (Focus * y / z));
}
