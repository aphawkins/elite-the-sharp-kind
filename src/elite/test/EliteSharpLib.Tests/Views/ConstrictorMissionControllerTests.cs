// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using EliteSharpLib.Conflict;
using EliteSharpLib.Fakes;
using EliteSharpLib.Missions;
using EliteSharpLib.Ships;
using EliteSharpLib.Trader;
using EliteSharpLib.Views;
using Useful.Abstraction;
using Useful.Audio;
using Useful.Fakes;
using Useful.Fakes.Audio;
using Useful.Fakes.Controls;

namespace EliteSharpLib.Tests.Views;

// Which briefing the Constrictor mission shows, with no renderer involved -
// the selection is content, so both tiers get it from the model rather than
// each re-deriving it from the commander's mission number.
public class ConstrictorMissionControllerTests
{
    [Fact]
    public void NothingIsShownOutsideTheMission()
    {
        ConstrictorMissionController controller = CreateController(out GameState gameState);
        gameState.Cmdr.Missions.MoveTo(ConstrictorMission.Id, ConstrictorMission.None);

        ConstrictorMissionModel model = controller.BuildModel();

        Assert.Equal(ConstrictorMission.None, model.Stage);
        Assert.Empty(model.Paragraphs);
    }

    [Fact]
    public void TheBriefIsTwoParagraphsAndNoHeadline()
    {
        ConstrictorMissionController controller = CreateController(out GameState gameState);
        gameState.Cmdr.Missions.MoveTo(ConstrictorMission.Id, ConstrictorMission.Briefed);

        ConstrictorMissionModel model = controller.BuildModel();

        Assert.Equal(ConstrictorMission.Briefed, model.Stage);
        Assert.Equal(2, model.Paragraphs.Count);
        Assert.Equal(string.Empty, model.Headline);
    }

    [Fact]
    public void TheFirstGalaxyIsToldWhereTheConstrictorWasSeen()
    {
        ConstrictorMissionController controller = CreateController(out GameState gameState);
        gameState.Cmdr.Missions.MoveTo(ConstrictorMission.Id, ConstrictorMission.Briefed);
        gameState.Cmdr.GalaxyNumber = 0;

        Assert.Contains("Reesdice", controller.BuildModel().Paragraphs[1], StringComparison.Ordinal);
    }

    [Fact]
    public void LaterGalaxiesAreToldItJumpedHere()
    {
        ConstrictorMissionController controller = CreateController(out GameState gameState);
        gameState.Cmdr.Missions.MoveTo(ConstrictorMission.Id, ConstrictorMission.Briefed);
        gameState.Cmdr.GalaxyNumber = 1;

        string paragraph = controller.BuildModel().Paragraphs[1];

        Assert.Contains("jumped to this galaxy", paragraph, StringComparison.Ordinal);
        Assert.DoesNotContain("Reesdice", paragraph, StringComparison.Ordinal);
    }

    [Fact]
    public void TheDebriefCarriesAHeadlineAndOneParagraph()
    {
        ConstrictorMissionController controller = CreateController(out GameState gameState);
        gameState.Cmdr.Missions.MoveTo(ConstrictorMission.Id, ConstrictorMission.Rewarded);

        ConstrictorMissionModel model = controller.BuildModel();

        Assert.Equal(ConstrictorMission.Rewarded, model.Stage);
        Assert.Equal("Congratulations Commander!", model.Headline);
        Assert.Single(model.Paragraphs);
    }

    [Fact]
    public void ResetPaysTheBountyOnceTheConstrictorIsDead()
    {
        ConstrictorMissionController controller = CreateController(out GameState gameState, out Trade trade);
        gameState.Cmdr.Missions.MoveTo(ConstrictorMission.Id, ConstrictorMission.Destroyed);
        float creditsBefore = trade.Credits;

        controller.Reset();

        Assert.Equal(ConstrictorMission.Rewarded, gameState.Cmdr.Missions.StageOf(ConstrictorMission.Id));
        Assert.Equal(creditsBefore + 5000, trade.Credits);
    }

    private static ConstrictorMissionController CreateController(out GameState gameState)
        => CreateController(out gameState, out _);

    private static ConstrictorMissionController CreateController(out GameState gameState, out Trade trade)
    {
        ScreenManager<Screen, IScreenController> views = new(new FakeKeyboard());
        gameState = new(views, ClassicMissions.Registry());
        PlayerShip ship = new();
        trade = new(gameState, ship);
        FakeEliteDraw draw = new();
        RNG rng = new(new FakeRandomSource());
        FakeShipFactory shipFactory = new(draw, rng);
        Universe universe = new(shipFactory, rng);
        AudioController audio = new(new FakeSound(), new Dictionary<string, SfxSample>(), new());
        Pilot pilot = new(draw, audio, universe, ship, rng);
        Combat combat = new(gameState, audio, ship, trade, pilot, universe, draw, shipFactory, rng);

        return new ConstrictorMissionController(
            gameState,
            new FakeKeyboard(),
            ship,
            trade,
            combat,
            universe,
            shipFactory,
            new FakeConstrictorMissionView());
    }

    private sealed class FakeConstrictorMissionView : IConstrictorMissionView
    {
        public Vector4 ShipLocation => new(200, 90, 600, 0);

        public void Draw(ConstrictorMissionModel model)
        {
            // Drawing is not under test here.
        }
    }
}
