// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Graphics;
using EliteSharpLib.Views;
using Useful.Abstraction;
using Useful.Fakes.Assets;
using Useful.Fakes.Controls;
using Useful.Graphics.Fakes;
using Useful.Graphics.Rendering;

namespace EliteSharpLib.Tests;

public class EliteDrawTests
{
    [Fact]
    public void DrawTextPrettyHardBreaksWordLongerThanLineWidth()
    {
        FakeGraphics graphics = new();
        GameState gameState = new(new ScreenManager<Screen, IView>(new FakeKeyboard()));
        ZBufferRenderer shipRenderer = new(graphics);
        RNG rng = new(new Random(0));
        EliteDraw draw = new(gameState, graphics, new FakeAssetLocator(), shipRenderer, rng);

        // No spaces/commas/periods anywhere, so the line-width scan must
        // never find a break point and previously underflowed past index 0.
        string unbreakableText = new('a', 200);

        Exception? exception = Record.Exception(() => draw.DrawTextPretty(new(0, 0), 64, unbreakableText));

        Assert.Null(exception);
    }
}
