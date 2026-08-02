// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Ships;
using EliteSharpLib.Tests.Missions;
using EliteSharpLib.Trader;
using EliteSharpLib.Types;
using EliteSharpLib.Views;
using Useful.Abstraction;
using Useful.Fakes;
using Useful.Fakes.Controls;

namespace EliteSharpLib.Tests.Views;

// The planet data screen's formatting, with no renderer in sight - matches
// CommanderStatusControllerTests' shape for a display-only screen.
public class PlanetDataControllerTests
{
    [Fact]
    public void DistanceIsBlankUntilUpdateHasRunOnce()
    {
        PlanetDataController controller = CreateController(out _);

        Assert.Equal(string.Empty, controller.BuildModel().Distance);
    }

    [Fact]
    public void UpdateComputesTheDistanceInLightYears()
    {
        PlanetDataController controller = CreateController(out GameState gameState);
        gameState.DockedPlanet = new() { B = 1, D = 1 };
        gameState.HyperspacePlanet = new() { B = 200, D = 200 };

        controller.Update();

        Assert.EndsWith("Light Years", controller.BuildModel().Distance, StringComparison.Ordinal);
    }

    [Fact]
    public void HeaderNamesTheHyperspaceTarget()
    {
        PlanetDataController controller = CreateController(out _);

        Assert.StartsWith("DATA ON", controller.BuildModel().Header, StringComparison.Ordinal);
    }

    [Fact]
    public void TechLevelIsOneHigherThanTheGeneratedValue()
    {
        GalaxySeed seed = new() { B = 1, C = 1, D = 1 };
        PlanetDataController controller = CreateController(out GameState gameState);
        gameState.HyperspacePlanet = seed;

        controller.Update();

        int expected = PlanetController.GeneratePlanetData(seed).TechLevel + 1;
        Assert.Equal(expected.ToString(System.Globalization.CultureInfo.InvariantCulture), controller.BuildModel().TechLevel);
    }

    [Fact]
    public void DescriptionIsNeverEmpty()
    {
        PlanetDataController controller = CreateController(out GameState gameState);
        gameState.HyperspacePlanet = new() { A = 1, B = 1, C = 1, D = 1, E = 1, F = 1 };

        Assert.NotEqual(string.Empty, controller.BuildModel().Description);
    }

    private static PlanetDataController CreateController(out GameState gameState)
    {
        ScreenManager<Screen, IScreenController> views = new(new FakeKeyboard());
        gameState = new(views, TestMissions.Registry());
        RNG rng = new(new FakeRandomSource());

        PlayerShip ship = new();
        PlanetController planet = new(gameState);

        return new PlanetDataController(
            gameState,
            planet,
            rng,
            TestMissions.Runner(gameState, ship, new Trade(gameState, ship)),
            new FakePlanetDataView());
    }

    private sealed class FakePlanetDataView : IView<PlanetDataModel>
    {
        public void Draw(PlanetDataModel model)
        {
            // Drawing is not under test here.
        }
    }
}
