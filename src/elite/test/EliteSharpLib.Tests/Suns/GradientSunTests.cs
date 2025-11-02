// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using EliteSharpLib.Graphics;
using EliteSharpLib.Ships;
using EliteSharpLib.Suns;
using Moq;
using Useful.Graphics;

namespace EliteSharpLib.Tests.Suns;

public class GradientSunTests
{
    private readonly Mock<IEliteDraw> _drawMoq;

    public GradientSunTests() => _drawMoq = MockSetup.MockDraw();

    [Fact]
    public void DrawGradientSun()
    {
        // Arrange
        GradientSun sun = new(_drawMoq.Object);

        // Act
        sun.Draw();

        // Assert
        _drawMoq.Verify(x => x.Graphics.DrawPixel(
            It.IsAny<Vector2>(),
            It.IsAny<FastColor>()));
    }

    [Fact]
    public void CloneGradientSun()
    {
        // Arrange
        GradientSun sun = new(_drawMoq.Object);

        // Act
        IObject obj = sun.Clone();

        // Assert
        Assert.IsType<GradientSun>(obj);
    }
}
