// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Renditions.SixteenBit;
using EliteSharpLib.Graphics;
using EliteSharpLib.Tests.Missions;
using EliteSharpLib.Views;
using Useful.Abstraction;
using Useful.Fakes.Controls;
using Useful.Graphics.Fakes;
using Useful.Graphics.Rendering;

namespace EliteSharpLib.Tests;

public class BaseViewTests
{
    [Fact]
    public void DrawTextPrettyHardBreaksWordLongerThanLineWidth()
    {
        BaseView16Bit baseView = new(Draw());

        // No spaces/commas/periods anywhere, so the line-width scan must
        // never find a break point and previously underflowed past index 0.
        string unbreakableText = new('a', 200);

        Exception? exception = Record.Exception(() => baseView.DrawTextPretty(new(0, 0), 64, unbreakableText));

        Assert.Null(exception);
    }

    private static EliteDraw Draw()
    {
        RecordingGraphics graphics = new();
        GameState gameState = new(new ScreenManager<Screen, IScreenController>(new FakeKeyboard()), TestMissions.Registry());
        ZBufferRenderer shipRenderer = new(graphics);
        RNG rng = new(new Random(0));
        return new EliteDraw(gameState, graphics, TestAssets.Locator(), new SixteenBitRendition(), shipRenderer, rng);
    }
}
