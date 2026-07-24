// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using EliteSharpLib.Fakes;
using EliteSharpLib.Graphics;
using EliteSharpLib.Planets;
using EliteSharpLib.Ships;
using Moq;
using Useful;
using Useful.Assets.Palettes;
using Useful.Graphics;

namespace EliteSharpLib.Tests.Planets;

public sealed class FractalPlanetTests
{
    [Fact]
    public void DrawFractalPlanet()
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
        mockGraphics.Verify(x => x.DrawPixel(It.IsAny<Vector2>(), It.IsAny<FastColor>()));
    }

    [Fact]
    public void GenerateLandscapeIsDeterministicPerSeed()
    {
        // Arrange
        Mock<IEliteDraw> mockDraw = new();
        mockDraw.Setup(x => x.Palette).Returns(new Palette(new Dictionary<string, FastColor>
        {
            ["Blue"] = new(0xFF0000FFu),
            ["Green"] = new(0xFF00FF00u),
            ["LightBlue"] = new(0xFF8080FFu),
            ["LightGreen"] = new(0xFF80FF80u),
        }));

        // Act
        FractalPlanet planet1 = new(mockDraw.Object, 12345);
        FractalPlanet planet2 = new(mockDraw.Object, 12345);

        // Assert
        for (int x = 0; x <= 128; x++)
        {
            for (int y = 0; y <= 128; y++)
            {
                Assert.Equal(planet1.Landscape[x, y], planet2.Landscape[x, y]);
            }
        }
    }

    [Fact]
    public void CloneFractalPlanet()
    {
        // Arrange
        FakeEliteDraw fakeEliteDraw = new();
        FractalPlanet planet = new(fakeEliteDraw, 12345);

        // Act
        IObject obj = planet.Clone();

        // Assert
        Assert.IsType<FractalPlanet>(obj);
        Assert.Equal(planet.Seed, ((FractalPlanet)obj).Seed);
    }
}
