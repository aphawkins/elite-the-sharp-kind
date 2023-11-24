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
    public class FractalPlanetTests
    {
        private readonly Mock<IDraw> _drawMoq;

        public FractalPlanetTests() => _drawMoq = MockSetup.MockDraw();

        [Fact]
        public void DrawFractalPlanet()
        {
            // Arrange
            FractalPlanet planet = new(_drawMoq.Object, 12345);

            // Act
            planet.Draw();

            // Assert
            _drawMoq.Verify(x => x.Graphics.DrawPixel(It.IsAny<Vector2>(), It.IsAny<FastColor>()));
        }

        [Fact]
        public void CloneFractalPlanet()
        {
            // Arrange
            FractalPlanet planet = new(_drawMoq.Object, 12345);

            // Act
            IObject obj = planet.Clone();
            obj.Draw();

            // Assert
            Assert.IsType<FractalPlanet>(obj);
            Assert.Equal(planet.Seed, ((FractalPlanet)obj).Seed);
        }
    }
}
