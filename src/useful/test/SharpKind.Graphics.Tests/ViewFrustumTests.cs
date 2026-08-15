// 'SharpKind Libraries' - Andy Hawkins 2023-2026.

using System.Numerics;

namespace SharpKind.Graphics.Tests;

public class ViewFrustumTests
{
    // A 320x200 viewport with the projector centred on it.
    private static readonly ViewFrustum s_frustum = ViewFrustum.FromViewport(
        new PerspectiveProjector(256, new(160, 100)),
        0,
        0,
        320,
        200,
        1,
        10000);

    [Fact]
    public void PointOnTheAxisIsInside() => Assert.True(s_frustum.Intersects(new(0, 0, 500), 0));

    [Theory]
    [InlineData(0, 0, 0.5f)]
    [InlineData(0, 0, -500)]
    [InlineData(0, 0, 20000)]
    public void PointOutsideTheDepthRangeIsCulled(float x, float y, float z)
        => Assert.False(s_frustum.Intersects(new(x, y, z), 0));

    [Theory]

    // Half the viewport width at depth 256 is 160 camera units, so 200 is well past each edge.
    [InlineData(-200, 0)]
    [InlineData(200, 0)]

    // Half the viewport height at depth 256 is 100 camera units.
    [InlineData(0, -150)]
    [InlineData(0, 150)]
    public void PointOutsideASideIsCulled(float x, float y)
        => Assert.False(s_frustum.Intersects(new(x, y, 256), 0));

    [Theory]
    [InlineData(-200, 0)]
    [InlineData(200, 0)]
    [InlineData(0, -150)]
    [InlineData(0, 150)]
    public void SphereReachingBackOverASideIsKept(float x, float y)
        => Assert.True(s_frustum.Intersects(new(x, y, 256), 100));

    [Fact]
    public void SphereStraddlingTheCameraPlaneIsKept()
        => Assert.True(s_frustum.Intersects(new(0, 0, -50), 100));

    [Fact]
    public void ProjectionAndCullAgreeOnWhereTheEdgeIs()
    {
        PerspectiveProjector projector = new(256, new(160, 100));
        ViewFrustum frustum = ViewFrustum.FromViewport(projector, 0, 0, 320, 200, 1, 10000);

        // Just inside and just outside the right edge at the same depth.
        Vector3 inside = new(158, 0, 256);
        Vector3 outside = new(162, 0, 256);

        Assert.True(projector.Project(inside).X < 320);
        Assert.True(projector.Project(outside).X > 320);
        Assert.True(frustum.Intersects(inside, 0));
        Assert.False(frustum.Intersects(outside, 0));
    }
}
