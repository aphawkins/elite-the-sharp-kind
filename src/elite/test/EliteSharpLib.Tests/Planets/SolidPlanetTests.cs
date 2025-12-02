// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using EliteSharpLib.Fakes;
using EliteSharpLib.Planets;
using EliteSharpLib.Ships;
using Moq;
using Useful.Fakes.Assets;
using Useful.Graphics;

namespace EliteSharpLib.Tests.Planets;

public class SolidPlanetTests
{
    [Fact]
    public void DrawSolidPlanet()
    {
        // Arrange
        Mock<IGraphics> mockGraphics = new();
        FakeEliteDraw fakeEliteDraw = new()
        {
            Graphics = mockGraphics.Object,
        };
        SolidPlanet planet = new(fakeEliteDraw);

        // Act
        planet.Draw();

        // Assert
        mockGraphics.Verify(x => x.DrawCircleFilled(
            It.IsAny<Vector2>(),
            It.IsAny<float>(),
            It.Is<uint>(x => x == FakeColor.TestColor)));
    }

    [Fact]
    public void CloneSolidPlanet()
    {
        // Arrange
        FakeEliteDraw fakeEliteDraw = new();
        SolidPlanet planet = new(fakeEliteDraw);

        // Act
        IObject obj = planet.Clone();

        // Assert
        Assert.IsType<SolidPlanet>(obj);
    }
}
