// 'Stunt Car Racer - The Sharp Kind' - Andy Hawkins 2026.
// 'Stunt Car Racer Remake' - sourceforge.net/projects/stuntcarremake.
// Stunt Car Racer (C) Geoff Crammond / MicroStyle / MicroProse 1989.

using StuntCarRacerLib.Fakes;
using StuntCarRacerLib.Rendering;
using Xunit;

namespace StuntCarRacerLib.Tests.Rendering;

public class HudRendererTests
{
    [Theory]
    [InlineData(0, 0)] // stationary
    [InlineData(0x1100, 0)] // the original's dead zone
    [InlineData(0x2100, 22)] // (0x1000 * 0xB700 >> 16) >> 7
    [InlineData(0x7000, 7)] // 135 wraps past the end of the bar to 7
    public void SpeedBarLengthMatchesTheOriginalFormula(int playerZSpeed, int expected)
        => Assert.Equal(expected, HudRenderer.SpeedBarLength(playerZSpeed));

    [Fact]
    public void CrackGrowsOnePixelPerDamageUnit()
    {
        RecordingGraphics graphics = new(640, 400);
        HudRenderer hud = new(graphics, new Random(1));

        hud.Draw(0, 0, 50, 0, 0);
        Assert.Equal(50, hud.Crack.Count);

        // no further damage, no further crack
        hud.Draw(0, 0, 50, 0, 0);
        Assert.Equal(50, hud.Crack.Count);

        hud.Draw(0, 0, 60, 0, 0);
        Assert.Equal(60, hud.Crack.Count);
    }

    [Fact]
    public void CrackStaysWithinTheBeam()
    {
        RecordingGraphics graphics = new(640, 400);
        HudRenderer hud = new(graphics, new Random(2));

        hud.Draw(0, 0, 255, 0, 0);

        // the walk is clamped to the beam rows, and the crack stops at the
        // length where the original wrecked the car
        Assert.Equal(HudRenderer.MaxCrackLength, hud.Crack.Count);
        Assert.All(hud.Crack, c => Assert.InRange(c.Row, 2, 5));
    }

    [Fact]
    public void CrackStartsOverWhenDamageResets()
    {
        RecordingGraphics graphics = new(640, 400);
        HudRenderer hud = new(graphics, new Random(3));

        hud.Draw(0, 0, 100, 0, 0);
        Assert.Equal(100, hud.Crack.Count);

        // a new race repairs the car; the crack starts again
        hud.Draw(0, 0, 5, 0, 0);
        Assert.Equal(5, hud.Crack.Count);
    }

    [Fact]
    public void DrawsWithArbitraryValuesWithoutExceptions()
    {
        RecordingGraphics graphics = new(640, 400);
        HudRenderer hud = new(graphics, new Random(4));

        hud.Draw(0, 0, 0, 0, 0);
        hud.Draw(4, 99, 255, 0x7FFF, 9999);
        hud.Draw(-1, -5, 10, -100, -12345);
    }
}
