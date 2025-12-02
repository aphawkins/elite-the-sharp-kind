// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using EliteSharpLib.Fakes;
using EliteSharpLib.Planets;
using EliteSharpLib.Ships;
using Moq;
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
        WireframePlanet planet = new(fakeEliteDraw);

        // Act
        planet.Draw();

        // Assert
        mockGraphics.Verify(x => x.DrawCircle(
            It.IsAny<Vector2>(),
            It.IsAny<float>(),
            It.IsAny<uint>()));
    }

    [Fact]
    public void CloneWireframePlanet()
    {
        // Arrange
        FakeEliteDraw fakeEliteDraw = new();
        WireframePlanet planet = new(fakeEliteDraw);

        // Act
        IObject obj = planet.Clone();

        // Assert
        Assert.IsType<WireframePlanet>(obj);
    }
}
