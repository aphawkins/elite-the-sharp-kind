// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Abstractions.Views.Stars;
using EliteSharpLib.Fakes;
using EliteSharpLib.Ships;
using EliteSharpLib.Tests.Missions;
using EliteSharpLib.Views;
using Useful.Abstraction;
using Useful.Fakes;
using Useful.Fakes.Controls;

namespace EliteSharpLib.Tests;

public class StarsTests
{
    [Fact]
    public void NormalSpaceSimulatesEighteenStars()
    {
        Stars stars = CreateStars(out FakeStarfieldRenderer renderer);

        stars.CreateNewStars();
        stars.FrontStarfield();

        Assert.Equal(18, renderer.LastDrawCount);
    }

    [Fact]
    public void WitchspaceSimulatesThreeStars()
    {
        // Original NOSTM: 18 particles (NOST) in normal space, dropped to 3
        // in witchspace for a visibly emptier void.
        Stars stars = CreateStars(out FakeStarfieldRenderer renderer);

        stars.CreateNewStars(3);
        stars.FrontStarfield();

        Assert.Equal(3, renderer.LastDrawCount);
    }

    private static Stars CreateStars(out FakeStarfieldRenderer renderer)
    {
        FakeEliteDraw draw = new();
        PlayerShip ship = new();
        renderer = new();
        RNG rng = new(new FakeRandomSource());

        ScreenManager<Screen, IScreenController> views = new(new FakeKeyboard());
        GameState gameState = new(views, TestMissions.Registry());
        return new(gameState, draw, ship, renderer, rng);
    }

    private sealed class FakeStarfieldRenderer : IStarfieldRenderer
    {
        internal int LastDrawCount { get; private set; }

        public void Draw(IReadOnlyList<StarMark> stars) => LastDrawCount = stars.Count;
    }
}
