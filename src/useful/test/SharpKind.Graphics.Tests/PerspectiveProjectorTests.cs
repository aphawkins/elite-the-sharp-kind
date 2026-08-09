// 'SharpKind Libraries' - Andy Hawkins 2023-2026.

using System.Numerics;

namespace SharpKind.Graphics.Tests;

public class PerspectiveProjectorTests
{
    [Fact]
    public void PointOnTheAxisProjectsToTheCentre()
    {
        PerspectiveProjector projector = new(256, new(160, 100));

        Assert.Equal(new Vector2(160, 100), projector.Project(new Vector3(0, 0, 500)));
    }

    [Fact]
    public void ScreenYIsInvertedRelativeToCameraY()
    {
        PerspectiveProjector projector = new(256, new(160, 100));

        Vector2 up = projector.Project(new Vector3(0, 128, 256));
        Vector2 down = projector.Project(new Vector3(0, -128, 256));

        Assert.Equal(-28, up.Y);
        Assert.Equal(228, down.Y);
    }

    [Theory]
    [InlineData(100, 100, 416)]
    [InlineData(100, 200, 288)]
    [InlineData(-100, 100, -96)]
    public void XScalesWithFocusOverDepth(float x, float z, float expected)
    {
        PerspectiveProjector projector = new(256, new(160, 100));

        Assert.Equal(expected, projector.Project(new Vector3(x, 0, z)).X);
    }
}
