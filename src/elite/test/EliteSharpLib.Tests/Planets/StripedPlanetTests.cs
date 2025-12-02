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

public sealed class StripedPlanetTests
{
    [Fact]
    public void DrawStripedPlanet()
    {
        // Arrange
        Mock<IGraphics> mockGraphics = new();
        FakeEliteDraw fakeEliteDraw = new()
        {
            Graphics = mockGraphics.Object,
        };
        FractalPlanet planet = new(fakeEliteDraw, 12345);

        // Act
        planet.Draw();

        // Assert
        mockGraphics.Verify(x => x.DrawPixel(It.IsAny<Vector2>(), It.IsAny<uint>()));
    }

    [Fact]
    public void CloneStripedPlanet()
    {
        // Arrange
        FakeEliteDraw fakeEliteDraw = new();
        StripedPlanet planet = new(fakeEliteDraw);

        // Act
        IObject obj = planet.Clone();

        // Assert
        Assert.IsType<StripedPlanet>(obj);
    }
}
