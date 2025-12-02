// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using EliteSharpLib.Fakes;
using EliteSharpLib.Ships;
using EliteSharpLib.Suns;
using Moq;
using Useful.Graphics;

namespace EliteSharpLib.Tests.Suns;

public class GradientSunTests
{
    [Fact]
    public void DrawGradientSun()
    {
        // Arrange
        Mock<IGraphics> mockGraphics = new();
        FakeEliteDraw fakeEliteDraw = new()
        {
            Graphics = mockGraphics.Object,
        };
        GradientSun sun = new(fakeEliteDraw);

        // Act
        sun.Draw();

        // Assert
        mockGraphics.Verify(x => x.DrawPixel(
            It.IsAny<Vector2>(),
            It.IsAny<uint>()));
    }

    [Fact]
    public void CloneGradientSun()
    {
        // Arrange
        FakeEliteDraw fakeEliteDraw = new();
        GradientSun sun = new(fakeEliteDraw);

        // Act
        IObject obj = sun.Clone();

        // Assert
        Assert.IsType<GradientSun>(obj);
    }
}
