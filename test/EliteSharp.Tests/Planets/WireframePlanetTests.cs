// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using EliteSharp.Graphics;
using EliteSharp.Planets;
using EliteSharp.Ships;
using Moq;

namespace EliteSharp.Tests.Planets
{
    public class WireframePlanetTests
    {
        private readonly Mock<IDraw> _drawMoq;

        public WireframePlanetTests() => _drawMoq = MockSetup.MockDraw();

        [Fact]
        public void DrawWireframePlanet()
        {
            // Arrange
            WireframePlanet planet = new(_drawMoq.Object);

            // Act
            planet.Draw();

            // Assert
            _drawMoq.Verify(x => x.Graphics.DrawCircle(
                It.IsAny<Vector2>(),
                It.IsAny<float>(),
                It.IsAny<Colour>()));
        }

        [Fact]
        public void CloneWireframePlanet()
        {
            // Arrange
            WireframePlanet planet = new(_drawMoq.Object);

            // Act
            IObject obj = planet.Clone();

            // Assert
            Assert.IsType<WireframePlanet>(obj);
        }
    }
}
