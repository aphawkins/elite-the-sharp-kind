// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using EliteSharpLib.Fakes;
using EliteSharpLib.Planets;
using EliteSharpLib.Ships;
using Moq;
using Useful;
using Useful.Graphics;

namespace EliteSharpLib.Tests.Planets;

public class WireframePlanetTests
{
    [Fact]
    public void DrawWireframePlanet()
    {
        // Arrange
        Mock<IGraphics> mockGraphics = new();
        FakeEliteDraw fakeEliteDraw = new()
        {
            Graphics = mockGraphics.Object,
        };
        WireframePlanet planet = new(fakeEliteDraw, false);

        // Act
        planet.Draw();

        // Assert
        mockGraphics.Verify(x => x.DrawCircle(
            It.IsAny<Vector2>(),
            It.IsAny<float>(),
            It.IsAny<FastColor>()));
    }

    [Fact]
    public void DrawWireframePlanetEquatorAndMeridian()
    {
        // Arrange
        Mock<IGraphics> mockGraphics = new();
        FakeEliteDraw fakeEliteDraw = new()
        {
            Graphics = mockGraphics.Object,
        };
        WireframePlanet planet = new(fakeEliteDraw, false)
        {
            Rotmat = Matrix4x4.Identity,
        };

        // Act
        planet.Draw();

        // Assert - two half ellipses of eight segments each
        mockGraphics.Verify(
            x => x.DrawLine(It.IsAny<Vector2>(), It.IsAny<Vector2>(), It.IsAny<FastColor>()),
            Times.Exactly(16));
    }

    [Fact]
    public void DrawWireframePlanetCrater()
    {
        // Arrange
        Mock<IGraphics> mockGraphics = new();
        FakeEliteDraw fakeEliteDraw = new()
        {
            Graphics = mockGraphics.Object,
        };

        // roofv_z is positive, so the crater faces us
        Matrix4x4 rotmat = Matrix4x4.Identity;
        rotmat.M22 = 0;
        rotmat.M23 = 1;
        rotmat.M32 = -1;
        rotmat.M33 = 0;
        WireframePlanet planet = new(fakeEliteDraw, true)
        {
            Rotmat = rotmat,
        };

        // Act
        planet.Draw();

        // Assert - one full ellipse of sixteen segments
        mockGraphics.Verify(
            x => x.DrawLine(It.IsAny<Vector2>(), It.IsAny<Vector2>(), It.IsAny<FastColor>()),
            Times.Exactly(16));
    }

    [Fact]
    public void DrawWireframePlanetCraterOnFarSide()
    {
        // Arrange
        Mock<IGraphics> mockGraphics = new();
        FakeEliteDraw fakeEliteDraw = new()
        {
            Graphics = mockGraphics.Object,
        };

        // roofv_z is negative, so the crater is hidden
        Matrix4x4 rotmat = Matrix4x4.Identity;
        rotmat.M22 = 0;
        rotmat.M23 = -1;
        rotmat.M32 = 1;
        rotmat.M33 = 0;
        WireframePlanet planet = new(fakeEliteDraw, true)
        {
            Rotmat = rotmat,
        };

        // Act
        planet.Draw();

        // Assert
        mockGraphics.Verify(
            x => x.DrawLine(It.IsAny<Vector2>(), It.IsAny<Vector2>(), It.IsAny<FastColor>()),
            Times.Never);
    }

    [Fact]
    public void CloneWireframePlanet()
    {
        // Arrange
        FakeEliteDraw fakeEliteDraw = new();
        WireframePlanet planet = new(fakeEliteDraw, false);

        // Act
        IObject obj = planet.Clone();

        // Assert
        Assert.IsType<WireframePlanet>(obj);
    }
}
