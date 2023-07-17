// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using EliteSharp.Graphics;
using EliteSharp.Planets;
using Moq;

namespace EliteSharp.Tests.Planets
{
    public class SolidPlanetTests
    {
        private readonly Mock<IDraw> _drawMoq;

        public SolidPlanetTests() => _drawMoq = MockSetup.MockDraw();

        [Fact]
        public void DrawWireframePlanet()
        {
            // Arrange
            SolidPlanet planet = new(_drawMoq.Object, Colour.Cyan);

            // Act
            planet.Draw();

            // Assert
            _drawMoq.Verify(x => x.Graphics.DrawCircleFilled(
                It.IsAny<Vector2>(),
                It.IsAny<float>(),
                It.Is<Colour>(x => x == Colour.Cyan)));
        }
    }
}
