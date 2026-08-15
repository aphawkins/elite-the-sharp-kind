// 'SharpKind Libraries' - Andy Hawkins 2023-2026.

using System.Numerics;

namespace SharpKind.Graphics;

/// <summary>
/// The six planes bounding what a <see cref="PerspectiveProjector"/> can put
/// on a given viewport, so a caller can reject a whole object before
/// transforming it face by face.
/// <para>
/// The side planes are the projection read backwards: a point is inside the
/// left edge when <c>Centre.X + (Focus * x / z) &gt;= left</c>, and
/// multiplying through by z turns that into a plane through the camera
/// origin. The near and far planes are the depth range - near is where the
/// perspective divide stops meaning anything, far is however distant the
/// caller stops caring.
/// </para>
/// <para>
/// Inside a plane is <c>Plane.DotCoordinate &gt;= 0</c>, and the normals are
/// unit length, so that value is a signed distance and a sphere's radius
/// compares against it directly.
/// </para>
/// </summary>
public readonly record struct ViewFrustum(Plane Near, Plane Far, Plane Left, Plane Right, Plane Top, Plane Bottom)
{
    /// <summary>
    /// The frustum a projector fills the given viewport rectangle with,
    /// between the two depths.
    /// </summary>
    /// <param name="projector">The projection the frustum bounds.</param>
    /// <param name="left">The viewport's left edge in pixels.</param>
    /// <param name="top">The viewport's top edge in pixels.</param>
    /// <param name="right">The viewport's right edge in pixels.</param>
    /// <param name="bottom">The viewport's bottom edge in pixels.</param>
    /// <param name="near">The nearest camera-space depth drawn, greater than zero.</param>
    /// <param name="far">The furthest camera-space depth drawn.</param>
    /// <returns>The bounding frustum.</returns>
    public static ViewFrustum FromViewport(
        in PerspectiveProjector projector,
        float left,
        float top,
        float right,
        float bottom,
        float near,
        float far)
    {
        float focus = projector.Focus;
        float cx = projector.Centre.X;
        float cy = projector.Centre.Y;

        // Screen y grows downwards while camera y grows up, so the top edge is
        // the one that bounds positive camera y.
        return new(
            Plane.Normalize(new(0, 0, 1, -near)),
            Plane.Normalize(new(0, 0, -1, far)),
            Plane.Normalize(new(focus, 0, cx - left, 0)),
            Plane.Normalize(new(-focus, 0, right - cx, 0)),
            Plane.Normalize(new(0, -focus, cy - top, 0)),
            Plane.Normalize(new(0, focus, bottom - cy, 0)));
    }

    /// <summary>
    /// Whether any part of a camera-space bounding sphere lies inside the
    /// frustum. Conservative: a sphere straddling a plane counts as inside,
    /// so nothing visible is ever rejected.
    /// </summary>
    /// <param name="centre">The sphere's centre in camera space.</param>
    /// <param name="radius">The sphere's radius.</param>
    /// <returns>True when the sphere may be visible.</returns>
    public bool Intersects(Vector3 centre, float radius)
        => Inside(Near, centre, radius)
            && Inside(Far, centre, radius)
            && Inside(Left, centre, radius)
            && Inside(Right, centre, radius)
            && Inside(Top, centre, radius)
            && Inside(Bottom, centre, radius);

    private static bool Inside(Plane plane, Vector3 centre, float radius)
        => Plane.DotCoordinate(plane, centre) >= -radius;
}
