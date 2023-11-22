// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using EliteSharp.Graphics;
using EliteSharp.Ships;
using EliteSharp.Suns;
using Moq;

namespace EliteSharp.Tests.Suns
{
    public class SolidSunTests
    {
        private readonly Mock<IDraw> _drawMoq;

        public SolidSunTests() => _drawMoq = MockSetup.MockDraw();

        [Fact]
        public void DrawSolidSun()
        {
            // Arrange
            SolidSun sun = new(_drawMoq.Object, FastColors.Cyan);

            // Act
            sun.Draw();

            // Assert
            _drawMoq.Verify(x => x.Graphics.DrawLine(
                It.IsAny<Vector2>(),
                It.IsAny<Vector2>(),
                It.Is<FastColor>(x => x == FastColors.Cyan)));
        }

        [Fact]
        public void CloneSolidSun()
        {
            // Arrange
            SolidSun sun = new(_drawMoq.Object, FastColors.Cyan);

            // Act
            IObject obj = sun.Clone();

            // Assert
            Assert.IsType<SolidSun>(obj);
            Assert.Equal(sun.Colour, ((SolidSun)obj).Colour);
        }
    }
}
