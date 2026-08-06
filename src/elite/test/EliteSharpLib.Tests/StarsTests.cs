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
using Useful.Fakes.Input;

namespace EliteSharpLib.Tests;

public class StarsTests
{
    [Fact]
    public void NormalSpaceSimulatesTheRenditionsOwnStarCount()
    {
        // Each rendition tunes its own star count for its own resolution,
        // rather than one being derived from another - the fake renderer's
        // 27 stands in for whatever a rendition author picked.
        Stars stars = CreateStars(out FakeStarfieldRenderer renderer, normalSpaceStarCount: 27);

        stars.CreateNewStars();
        stars.FrontStarfield();

        Assert.Equal(27, renderer.LastDrawCount);
    }

    [Fact]
    public void WitchspaceSimulatesTheRenditionsOwnStarCount()
    {
        // Original NOSTM: 18 particles (NOST) in normal space, dropped to 3
        // in witchspace for a visibly emptier void - each rendition tunes
        // its own pair of counts the same way.
        Stars stars = CreateStars(out FakeStarfieldRenderer renderer, witchspaceStarCount: 5);

        stars.CreateNewWitchspaceStars();
        stars.FrontStarfield();

        Assert.Equal(5, renderer.LastDrawCount);
    }

    private static Stars CreateStars(
        out FakeStarfieldRenderer renderer,
        int normalSpaceStarCount = 18,
        int witchspaceStarCount = 3)
    {
        FakeEliteDraw draw = new();
        PlayerShip ship = new();
        renderer = new(normalSpaceStarCount, witchspaceStarCount);
        RNG rng = new(new FakeRandomSource());

        ScreenManager<Screen, IScreenController> views = new(new FakeKeyboard());
        GameState gameState = new(views, TestMissions.Registry());
        return new(gameState, draw, ship, renderer, rng);
    }

    private sealed class FakeStarfieldRenderer(int normalSpaceStarCount, int witchspaceStarCount) : IStarfieldRenderer
    {
        public int NormalSpaceStarCount { get; } = normalSpaceStarCount;

        public int WitchspaceStarCount { get; } = witchspaceStarCount;

        internal int LastDrawCount { get; private set; }

        public void Draw(IReadOnlyList<StarMark> stars) => LastDrawCount = stars.Count;
    }
}
