// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Renditions.SixteenBit;
using EliteSharpLib.Graphics;
using EliteSharpLib.Tests.Missions;
using EliteSharpLib.Views;
using SharpKind.Abstraction;
using SharpKind.Fakes.Input;
using SharpKind.Graphics.Fakes;
using SharpKind.Graphics.Rendering;

namespace EliteSharpLib.Tests;

public class BaseViewTests
{
    [Fact]
    public void DrawTextPrettyHardBreaksWordLongerThanLineWidth()
    {
        BaseView16Bit baseView = new(Draw(out _));

        // No spaces/commas/periods anywhere, so the line-width scan must
        // never find a break point and previously underflowed past index 0.
        string unbreakableText = new('a', 200);

        Exception? exception = Record.Exception(() => baseView.DrawTextPretty(new(0, 0), 64, unbreakableText));

        Assert.Null(exception);
    }

    // Unchosen means the original's projection, which is a focal length of
    // exactly one screen height - not 53 degrees put back through the
    // arithmetic, which lands 0.3 % away from it.
    [Fact]
    public void FocusWithNoChosenFieldOfViewIsTheScreenHeight()
    {
        EliteDraw draw = Draw(out GameState gameState);

        Assert.Null(gameState.Config.Engine.FieldOfView);
        Assert.Equal(512, draw.Layout.ScreenHeight);
        Assert.Equal(draw.Layout.ScreenHeight, draw.Focus);
    }

    // Widening the view shortens the focal length, so more of the universe
    // fits across the same viewport.
    [Theory]
    [InlineData(65)]
    [InlineData(90)]
    [InlineData(105)]
    public void AWiderFieldOfViewShortensTheFocalLength(int fieldOfView)
    {
        EliteDraw draw = Draw(out GameState gameState);
        float classic = draw.Focus;

        gameState.Config.Engine.FieldOfView = fieldOfView;

        Assert.True(draw.Focus < classic);
    }

    // And narrowing it lengthens the focal length, magnifying what is left.
    [Fact]
    public void ANarrowerFieldOfViewLengthensTheFocalLength()
    {
        EliteDraw draw = Draw(out GameState gameState);
        float classic = draw.Focus;

        gameState.Config.Engine.FieldOfView = 40;

        Assert.True(draw.Focus > classic);
    }

    private static EliteDraw Draw(out GameState gameState)
    {
        // The 16-bit tier's own canvas: Focus is derived from the screen
        // height, so a zero-sized fake would make every assertion about it
        // vacuously true.
        RecordingGraphics graphics = new(640, 512);
        gameState = new(new ScreenManager<Screen, IScreenController>(new FakeKeyboard()), TestMissions.Registry());
        ZBufferRenderer shipRenderer = new(graphics);
        RenderRandom rng = new(new Random(0));
        return new EliteDraw(gameState, graphics, graphics.Layout, TestAssets.Locator(), new SixteenBitRendition(), shipRenderer, rng);
    }
}
