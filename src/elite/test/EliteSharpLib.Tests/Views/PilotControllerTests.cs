// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Conflict;
using EliteSharpLib.Fakes;
using EliteSharpLib.Lasers;
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

// One controller serves all four cockpit windows; these tests exercise that
// each PilotDirection selects its own view name and laser mount, with no
// renderer involved.
public class PilotControllerTests
{
    [Fact]
    public void EachDirectionNamesItsOwnView()
    {
        Assert.Equal("Front View", CreateController(PilotDirection.Front, out _).BuildModel().ViewName);
        Assert.Equal("Rear View", CreateController(PilotDirection.Rear, out _).BuildModel().ViewName);
        Assert.Equal("Left View", CreateController(PilotDirection.Left, out _).BuildModel().ViewName);
        Assert.Equal("Right View", CreateController(PilotDirection.Right, out _).BuildModel().ViewName);
    }

    [Fact]
    public void EachDirectionDrawsItsOwnLaserMount()
    {
        PilotController front = CreateController(PilotDirection.Front, out PlayerShip frontShip);
        frontShip.LaserFront = new PulseLaser();
        PilotController rear = CreateController(PilotDirection.Rear, out PlayerShip rearShip);
        rearShip.LaserRear = new BeamLaser();

        Assert.Equal(LaserType.Pulse, front.BuildModel().LaserType);
        Assert.Equal(LaserType.Beam, rear.BuildModel().LaserType);
    }

    [Fact]
    public void NoHyperspaceStatusByDefault()
    {
        PilotController controller = CreateController(PilotDirection.Front, out _);

        Assert.Equal(string.Empty, controller.BuildModel().HyperspaceStatus);
    }

    [Fact]
    public void GalacticHyperspaceIsReportedOnceEngaged()
    {
        PilotController controller = CreateController(PilotDirection.Front, out PlayerShip ship, out Space space);
        ship.HasGalacticHyperdrive = true;

        space.StartGalacticHyperspace();

        Assert.Equal("Galactic Hyperspace", controller.BuildModel().HyperspaceStatus);
    }

    [Fact]
    public void FiringSetsTheLaserFramesWhichDecayOverThreeUpdates()
    {
        PilotController controller = CreateController(PilotDirection.Front, out _, out _, out GameState gameState);
        gameState.DrawLasers = true;

        controller.Update();
        Assert.True(controller.BuildModel().IsFiring);

        gameState.DrawLasers = false;
        controller.Update(); // frames: 2 -> 1
        Assert.True(controller.BuildModel().IsFiring);

        controller.Update(); // frames: 1 -> 0
        Assert.False(controller.BuildModel().IsFiring);
    }

    private static PilotController CreateController(PilotDirection direction, out PlayerShip ship)
        => CreateController(direction, out ship, out _);

    private static PilotController CreateController(PilotDirection direction, out PlayerShip ship, out Space space)
        => CreateController(direction, out ship, out space, out _);

    private static PilotController CreateController(
        PilotDirection direction, out PlayerShip ship, out Space space, out GameState gameState)
    {
        ScreenManager<Screen, IScreenController> views = new(new FakeKeyboard());
        gameState = new(views, ClassicMissions.Registry());
        ship = new PlayerShip();
        Trade trade = new(gameState, ship);
        FakeEliteDraw draw = new();
        RNG rng = new(new FakeRandomSource());
        FakeShipFactory shipFactory = new(draw, rng);
        Universe universe = new(shipFactory, rng);
        AudioController audio = new(new FakeSound(), new Dictionary<string, SfxSample>(), new());
        Stars stars = new(gameState, draw, ship, rng);
        Pilot pilot = new(draw, audio, universe, ship, rng);
        Combat combat = new(gameState, audio, ship, trade, pilot, universe, draw, shipFactory, rng);
        space = new(gameState, audio, pilot, combat, trade, ship, new PlanetController(gameState), stars, universe, draw, rng);

        return new PilotController(
            gameState, new FakeKeyboard(), pilot, ship, stars, space, combat, direction, new FakePilotView());
    }

    private sealed class FakePilotView : IView<PilotModel>
    {
        public void Draw(PilotModel model)
        {
            // Drawing is not under test here.
        }
    }
}
