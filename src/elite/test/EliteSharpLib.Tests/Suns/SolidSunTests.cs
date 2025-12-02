// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using EliteSharpLib.Fakes;
using EliteSharpLib.Ships;
using EliteSharpLib.Suns;
using Moq;
using Useful.Fakes.Assets;
using Useful.Graphics;

namespace EliteSharpLib.Tests.Suns;

public class SolidSunTests
{
    [Fact]
    public void DrawSolidSun()
    {
        // Arrange
        Mock<IGraphics> mockGraphics = new();
        FakeEliteDraw fakeEliteDraw = new()
        {
            Graphics = mockGraphics.Object,
        };
        SolidSun sun = new(fakeEliteDraw);

        // Act
        sun.Draw();

        // Assert
        mockGraphics.Verify(x => x.DrawLine(
            It.IsAny<Vector2>(),
            It.IsAny<Vector2>(),
            It.Is<uint>(x => x == FakeColor.TestColor)));
    }

    [Fact]
    public void CloneSolidSun()
    {
        // Arrange
        FakeEliteDraw fakeEliteDraw = new();
        SolidSun sun = new(fakeEliteDraw);

        // Act
        IObject obj = sun.Clone();

        // Assert
        Assert.IsType<SolidSun>(obj);
    }
}
