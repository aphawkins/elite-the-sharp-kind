// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using EliteSharp.Abstractions.Ships;
using EliteSharp.Renditions.SixteenBit;
using EliteSharpLib.Graphics;
using EliteSharpLib.Tests.Missions;
using EliteSharpLib.Views;
using Useful;
using Useful.Abstraction;
using Useful.Fakes.Input;
using Useful.Graphics;
using Useful.Graphics.Rendering;

namespace EliteSharpLib.Tests;

public class LaserDrawTests
{
    // The crosshair sprites are two-colour: transparent plus the laser's
    // own colour, so counting that colour proves the right one was drawn.
    [Fact]
    public void DrawLaserSightsDrawsThePulseCrosshair() => AssertCrosshairColor(LaserType.Pulse, FastColor.FromUInt32(0xFFFFFF55));

    [Fact]
    public void DrawLaserSightsDrawsTheBeamCrosshair() => AssertCrosshairColor(LaserType.Beam, FastColor.FromUInt32(0xFFFFFF55));

    [Fact]
    public void DrawLaserSightsDrawsTheMilitaryCrosshair() => AssertCrosshairColor(LaserType.Military, FastColor.FromUInt32(0xFFBBFF99));

    [Fact]
    public void DrawLaserSightsDrawsTheMiningCrosshair() => AssertCrosshairColor(LaserType.Mining, FastColor.FromUInt32(0xFFBB55EE));

    [Fact]
    public void DrawLaserSightsDrawsNothingWithoutALaser()
    {
        FastBitmap frame = DrawSights(LaserType.None);

        Assert.Equal(0, CountPixels(frame, FastColor.FromUInt32(0xFFFFFF55)));
        Assert.Equal(0, CountPixels(frame, FastColor.FromUInt32(0xFFBBFF99)));
        Assert.Equal(0, CountPixels(frame, FastColor.FromUInt32(0xFFBB55EE)));
    }

    // Outlined beams are the same two triangles as filled ones, so the
    // wireframe setting shows up as far fewer coloured pixels.
    [Fact]
    public void DrawLaserLinesOutlinesTheBeamsWhenLaserWireframeIsSet()
    {
        // BrightPurple, the 16-bit palette's mining-laser beam colour.
        FastColor miningColor = FastColor.FromUInt32(0xFFBB55EE);

        int filled = CountPixels(DrawLines(LaserType.Mining, laserWireframe: false), miningColor);
        int wireframe = CountPixels(DrawLines(LaserType.Mining, laserWireframe: true), miningColor);

        Assert.True(filled > 0);
        Assert.True(wireframe > 0);
        Assert.True(wireframe < filled);
    }

    private static void AssertCrosshairColor(LaserType laserType, in FastColor expectedColor)
        => Assert.True(CountPixels(DrawSights(laserType), expectedColor) > 0);

    private static FastBitmap DrawSights(LaserType laserType)
        => Render(laser => laser.DrawLaserSights(laserType));

    // Where the beams converge is the game's roll now rather than the view's,
    // so the test picks it: dead centre, one of the four the game can roll.
    private static FastBitmap DrawLines(LaserType laserType, bool laserWireframe)
        => Render(laser => laser.DrawLaserLines(laserType, Vector2.Zero, laserWireframe));

    private static FastBitmap Render(Action<LaserDraw16Bit> draw)
    {
        FastBitmap? lastFrame = null;
        using SoftwareGraphics graphics = SoftwareGraphics.Create(512, 512, b => lastFrame = b, TestAssets.Locator());
        GameState gameState = new(new ScreenManager<Screen, IScreenController>(new FakeKeyboard()), TestMissions.Registry());
        RNG rng = new(new Random(0));
        EliteDraw eliteDraw = new(
            gameState,
            graphics,
            TestAssets.Locator(),
            new SixteenBitRendition(),
            new ZBufferRenderer(graphics),
            rng);
        LaserDraw16Bit laser = new(eliteDraw);

        graphics.Clear();
        draw(laser);
        graphics.ScreenUpdate();

        Assert.NotNull(lastFrame);
        return lastFrame;
    }

    private static int CountPixels(FastBitmap bitmap, in FastColor color)
    {
        int count = 0;
        for (int y = 0; y < bitmap.Height; y++)
        {
            for (int x = 0; x < bitmap.Width; x++)
            {
                if (bitmap.GetPixel(x, y) == color)
                {
                    count++;
                }
            }
        }

        return count;
    }
}
