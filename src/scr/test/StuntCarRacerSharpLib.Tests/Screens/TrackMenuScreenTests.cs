// 'Stunt Car Racer - The Sharp Kind' - Andy Hawkins 2026.
// 'Stunt Car Racer Remake' - sourceforge.net/projects/stuntcarremake.
// Stunt Car Racer (C) Geoff Crammond / MicroStyle / MicroProse 1989.

using StuntCarRacerSharpLib.Fakes;
using Xunit;

namespace StuntCarRacerSharpLib.Tests.Screens;

public class TrackMenuScreenTests
{
    [Fact]
    public void DrawsTheMenuBackgroundOverTheWorld()
    {
        RecordingGraphics graphics = new(640, 400);
        FakeAbstraction abstraction = new(graphics);
        StuntCarRacerMain game = new(abstraction);

        game.Draw();

        // the world (track) draws first, then the menu.png overlay on top
        Assert.NotEmpty(graphics.FilledPolygons);
        Assert.Contains(graphics.ImageParts, p => p.ImageType == "Menu");
    }

    [Fact]
    public void MenuOverlayFillsTheWholeScreen()
    {
        RecordingGraphics graphics = new(640, 400);
        FakeAbstraction abstraction = new(graphics);
        StuntCarRacerMain game = new(abstraction);

        game.Draw();

        (_, System.Numerics.Vector2 position, System.Numerics.Vector2 size, _, _) =
            graphics.ImageParts.Single(p => p.ImageType == "Menu");
        Assert.Equal(System.Numerics.Vector2.Zero, position);
        Assert.Equal(new System.Numerics.Vector2(640, 400), size);
    }

    [Fact]
    public void TrackListTextStaysWithinScreenBounds()
    {
        RecordingGraphics graphics = new(640, 400);
        FakeAbstraction abstraction = new(graphics);
        StuntCarRacerMain game = new(abstraction);

        game.Draw();

        Assert.NotEmpty(graphics.LeftTexts);
        Assert.All(
            graphics.LeftTexts,
            t =>
            {
                Assert.InRange(t.Position.X, 0, graphics.ScreenWidth);
                Assert.InRange(t.Position.Y, 0, graphics.ScreenHeight);
            });

        // eight track names plus the title and two footer lines
        Assert.True(graphics.LeftTexts.Count >= 11);
    }
}
