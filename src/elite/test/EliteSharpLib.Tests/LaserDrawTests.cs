// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Graphics;
using EliteSharpLib.Lasers;
using EliteSharpLib.Views;
using Useful.Abstraction;
using Useful.Assets;
using Useful.Fakes.Controls;
using Useful.Graphics;
using Useful.Graphics.Rendering;

namespace EliteSharpLib.Tests;

public class LaserDrawTests
{
    // The crosshair sprites are two-colour: transparent plus the laser's
    // own colour, so counting that colour proves the right one was drawn.
    [Fact]
    public void DrawLaserSightsDrawsThePulseCrosshair() => AssertCrosshairColor(LaserType.Pulse, 0xFFFFFF5C);

    [Fact]
    public void DrawLaserSightsDrawsTheBeamCrosshair() => AssertCrosshairColor(LaserType.Beam, 0xFFFFFF5C);

    [Fact]
    public void DrawLaserSightsDrawsTheMilitaryCrosshair() => AssertCrosshairColor(LaserType.Military, 0xFFC3FF99);

    [Fact]
    public void DrawLaserSightsDrawsTheMiningCrosshair() => AssertCrosshairColor(LaserType.Mining, 0xFFB855F6);

    [Fact]
    public void DrawLaserSightsDrawsNothingWithoutALaser()
    {
        FastBitmap frame = DrawSights(LaserType.None);

        Assert.Equal(0, CountPixels(frame, 0xFFFFFF5C));
        Assert.Equal(0, CountPixels(frame, 0xFFC3FF99));
        Assert.Equal(0, CountPixels(frame, 0xFFB855F6));
    }

    // Outlined beams are the same two triangles as filled ones, so the
    // wireframe setting shows up as far fewer coloured pixels.
    [Fact]
    public void DrawLaserLinesOutlinesTheBeamsWhenLaserWireframeIsSet()
    {
        const uint miningColor = 0xFFB855F6;

        int filled = CountPixels(DrawLines(LaserType.Mining, laserWireframe: false), miningColor);
        int wireframe = CountPixels(DrawLines(LaserType.Mining, laserWireframe: true), miningColor);

        Assert.True(filled > 0);
        Assert.True(wireframe > 0);
        Assert.True(wireframe < filled);
    }

    private static void AssertCrosshairColor(LaserType laserType, uint expectedColor)
        => Assert.True(CountPixels(DrawSights(laserType), expectedColor) > 0);

    private static FastBitmap DrawSights(LaserType laserType)
        => Render(false, laser => laser.DrawLaserSights(laserType));

    private static FastBitmap DrawLines(LaserType laserType, bool laserWireframe)
        => Render(laserWireframe, laser => laser.DrawLaserLines(laserType));

    private static FastBitmap Render(bool laserWireframe, Action<LaserDraw> draw)
    {
        FastBitmap? lastFrame = null;
        using SoftwareGraphics graphics = SoftwareGraphics.Create(512, 512, b => lastFrame = b, AssetLocator.Create());
        GameState gameState = new(new ScreenManager<Screen, IScreenController>(new FakeKeyboard()));
        gameState.Config.Engine.Graphics.GraphicStyle = laserWireframe ? GraphicStyle.Wireframe : GraphicStyle.Solid;
        RNG rng = new(new Random(0));
        EliteDraw eliteDraw = new(gameState, graphics, AssetLocator.Create(), new ZBufferRenderer(graphics), rng);
        LaserDraw laser = new(gameState, eliteDraw, rng);

        graphics.Clear();
        draw(laser);
        graphics.ScreenUpdate();

        Assert.NotNull(lastFrame);
        return lastFrame;
    }

    private static int CountPixels(FastBitmap bitmap, uint color)
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
