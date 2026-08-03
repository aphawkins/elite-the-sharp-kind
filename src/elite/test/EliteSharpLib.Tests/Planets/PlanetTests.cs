// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Abstractions.Views.Planets;
using EliteSharpLib.Fakes;
using EliteSharpLib.Planets;
using EliteSharpLib.Ships;

namespace EliteSharpLib.Tests.Planets;

// The game's half of a planet: where it is, which way up, and cloning. What
// it looks like belongs to the rendition and is tested in
// PlanetRendererTests.
public sealed class PlanetTests
{
    [Fact]
    public void DrawsThroughItsRenderer()
    {
        // Arrange
        RecordingRenderer renderer = new();
        Planet planet = new(new FakeEliteDraw(), renderer, spins: false);

        // Act
        planet.Draw();

        // Assert
        Assert.NotNull(renderer.LastDrawn);
    }

    [Fact]
    public void ProjectsItselfBeforeTheRendererSeesIt()
    {
        // Arrange: a renderer is handed screen terms only - it never sees
        // where the planet is in space.
        RecordingRenderer renderer = new();
        FakeEliteDraw draw = new();
        Planet planet = new(draw, renderer, spins: false) { Location = new(0, 0, 123456, 0) };

        // Act
        planet.Draw();

        // Assert: dead ahead, so it lands on the viewport centre.
        PlanetView view = renderer.LastDrawn!.Value;
        Assert.Equal(draw.Layout.ViewportCentre, view.Centre);
        Assert.True(view.Radius > 0);

        // Focus over the original's 256-wide space, which is what a renderer
        // scales its own thresholds by.
        Assert.Equal(draw.Focus / 256, view.UnitScale);
    }

    [Fact]
    public void DrawsNothingWhenItIsOffScreen()
    {
        // Arrange: far enough to one side that no part of it is in view.
        RecordingRenderer renderer = new();
        Planet planet = new(new FakeEliteDraw(), renderer, spins: false)
        {
            Location = new(100000000, 0, 123456, 0),
        };

        // Act
        planet.Draw();

        // Assert
        Assert.Null(renderer.LastDrawn);
    }

    // Only the outlined style turns: the surfaced ones map their detail from
    // the orientation and expect it to stay put.
    [Theory]
    [InlineData(true, 127)]
    [InlineData(false, 0)]
    public void OnlySpinsWhenItsStyleAsksTo(bool spins, float expected)
    {
        Planet planet = new(new FakeEliteDraw(), new RecordingRenderer(), spins);

        Assert.Equal(expected, planet.RotX);
        Assert.Equal(expected, planet.RotZ);
    }

    [Fact]
    public void ClonesAsAPlanet()
    {
        // Arrange
        Planet planet = new(new FakeEliteDraw(), new RecordingRenderer(), spins: false)
        {
            Location = new(1, 2, 3, 0),
        };

        // Act
        IObject clone = planet.Clone();

        // Assert
        Assert.IsType<Planet>(clone);
        Assert.Equal(planet.Location, clone.Location);
    }

    [Fact]
    public void ACloneDrawsTheSameWayTheOriginalDoes()
    {
        // Arrange: the renderer goes with the clone, so a planet copied into
        // witchspace and back still looks like itself.
        RecordingRenderer renderer = new();
        Planet planet = new(new FakeEliteDraw(), renderer, spins: false);

        // Act
        planet.Clone().Draw();

        // Assert
        Assert.NotNull(renderer.LastDrawn);
    }

    private sealed class RecordingRenderer : IPlanetRenderer
    {
        internal PlanetView? LastDrawn { get; private set; }

        public void Draw(PlanetView planet) => LastDrawn = planet;
    }
}
