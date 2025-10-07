// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using EliteSharp.Graphics;
using EliteSharp.Planets;
using EliteSharp.Ships;
using Moq;
using Useful.Graphics;

namespace EliteSharp.Tests.Planets;

public sealed class StripedPlanetTests
{
    private readonly Mock<IEliteDraw> _drawMoq;

    public StripedPlanetTests() => _drawMoq = MockSetup.MockDraw();

    [Fact]
    public void DrawStripedPlanet()
    {
        // Arrange
        FractalPlanet planet = new(_drawMoq.Object, 12345);

        // Act
        planet.Draw();

        // Assert
        _drawMoq.Verify(x => x.Graphics.DrawPixel(It.IsAny<Vector2>(), It.IsAny<FastColor>()));
    }

    [Fact]
    public void CloneStripedPlanet()
    {
        // Arrange
        StripedPlanet planet = new(_drawMoq.Object);

        // Act
        IObject obj = planet.Clone();

        // Assert
        Assert.IsType<StripedPlanet>(obj);
    }
}
