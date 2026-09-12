// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Abstractions.Views.Stars;
using EliteSharpLib.Fakes;
using EliteSharpLib.Ships;
using EliteSharpLib.Tests.Missions;
using EliteSharpLib.Views;
using SharpKind.Abstraction;
using SharpKind.Fakes.Input;

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

        // The starfield pass moves the stars and collects the marks; Draw is
        // what hands them to the rendition, now that the game ticks and the
        // frame is composed separately.
        stars.Draw();

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
        stars.Draw();

        Assert.Equal(5, renderer.LastDrawCount);
    }

    // The rear and side starfields recycle a star once it passes the view's
    // edge, and that edge is in star space, which the focal length sizes. A
    // wide field of view shortens the focal length and so stretches the view
    // further across star space; the bounds used to be the constants 110 and
    // 116, which are that edge only at the classic focus and fall well inside
    // the view once it widens - leaving an empty band along the top and
    // bottom of the rear and side views.
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void RearAndSideStarfieldsFillAWidenedView(bool rear)
    {
        // Half the classic focal length, so star space is twice as tall: the
        // view reaches 191 units either side of centre where the old
        // constants stop at 110 and 116.
        const int frames = 200;
        const int starCount = 64;

        Stars stars = CreateStars(
            out FakeStarfieldRenderer renderer,
            out PlayerShip ship,
            normalSpaceStarCount: starCount,
            focus: 256);

        ship.Speed = 20;
        stars.CreateNewStars();

        for (int i = 0; i < frames; i++)
        {
            if (rear)
            {
                stars.RearStarfield();
            }
            else
            {
                stars.RightStarfield();
            }

            stars.Draw();
        }

        // StarScale is 1 at this focus, so 116 star-space units is 116 pixels
        // from the centre. A starfield held inside the old bounds never puts
        // a star past that.
        // Nearly every star should be on screen on nearly every frame. Held
        // to the old bounds the field is thinned instead, because a star is
        // recycled to an edge that is now inside the view and spends its life
        // out past the drawable area: 93 % of the rear field survived and
        // only 69 % of the side one, against 98 % of both once the bounds
        // follow the focal length.
        Assert.True(
            renderer.AllMarks.Count > (int)(frames * starCount * 0.95),
            $"only {renderer.AllMarks.Count} of {frames * starCount} star marks were drawn");

        // And nothing is drawn outside the view itself.
        Assert.All(renderer.AllMarks, mark => Assert.InRange(mark.Position.Y, 0, 383));
    }

    private static Stars CreateStars(
        out FakeStarfieldRenderer renderer,
        int normalSpaceStarCount = 18,
        int witchspaceStarCount = 3)
        => CreateStars(out renderer, out _, normalSpaceStarCount, witchspaceStarCount);

    private static Stars CreateStars(
        out FakeStarfieldRenderer renderer,
        out PlayerShip ship,
        int normalSpaceStarCount = 18,
        int witchspaceStarCount = 3,
        float focus = 512)
    {
        // A real stream rather than the fixed fake: where a recycled star
        // reappears has to vary, or every star stacks on one spot and a test
        // about how the field fills the view cannot see anything.
        FakeEliteDraw draw = new() { Focus = focus, Jitter = new RenderRandom(new Random(1)) };
        renderer = new(normalSpaceStarCount, witchspaceStarCount);

        ScreenManager<Screen, IScreenController> views = new(new FakeKeyboard());
        GameState gameState = new(views, TestMissions.Registry());
        ship = new(gameState);
        return new(gameState, draw, ship, renderer);
    }

    private sealed class FakeStarfieldRenderer(int normalSpaceStarCount, int witchspaceStarCount) : IStarfieldRenderer
    {
        public int NormalSpaceStarCount { get; } = normalSpaceStarCount;

        public int WitchspaceStarCount { get; } = witchspaceStarCount;

        internal int LastDrawCount { get; private set; }

        internal List<StarMark> AllMarks { get; } = [];

        public void Draw(IReadOnlyList<StarMark> stars)
        {
            LastDrawCount = stars.Count;
            AllMarks.AddRange(stars);
        }
    }
}
