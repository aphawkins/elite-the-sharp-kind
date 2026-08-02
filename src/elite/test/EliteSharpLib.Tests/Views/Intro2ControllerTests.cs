// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Conflict;
using EliteSharpLib.Fakes;
using EliteSharpLib.Missions;
using EliteSharpLib.Ships;
using EliteSharpLib.Tests.Missions;
using EliteSharpLib.Trader;
using EliteSharpLib.Views;
using Useful.Abstraction;
using Useful.Audio;
using Useful.Fakes;
using Useful.Fakes.Audio;
using Useful.Fakes.Controls;

namespace EliteSharpLib.Tests.Views;

// The ship parade's behaviour, with no renderer involved: which ship's name
// is on show, and that it changes as the parade advances.
public class Intro2ControllerTests
{
    [Fact]
    public void NoShipNameBeforeTheFirstReset()
    {
        Intro2Controller controller = CreateController();

        Assert.Equal(string.Empty, controller.BuildModel().ShipName);
    }

    [Fact]
    public void ResetPutsTheFirstParadeShipInTheUniverse()
    {
        Intro2Controller controller = CreateController();

        controller.Reset();

        Assert.NotEqual(string.Empty, controller.BuildModel().ShipName);
    }

    [Fact]
    public void RightArrowKeepsAShipOnShow()
    {
        // FakeShipFactory.CreateParade returns a single ship, so this can't
        // assert the *name* changes - only that cycling past the end of the
        // parade still leaves a ship in view rather than clearing it.
        Intro2Controller controller = CreateController(out FakeKeyboard keyboard);
        controller.Reset();

        keyboard.KeyDown(ConsoleKey.RightArrow, default);
        controller.HandleInput();

        Assert.NotEqual(string.Empty, controller.BuildModel().ShipName);
    }

    [Fact]
    public void ThePromptIsFixedWording()
    {
        Intro2Controller controller = CreateController();

        Assert.Equal("Press Fire or Space, Commander.", controller.BuildModel().Prompt);
    }

    private static Intro2Controller CreateController() => CreateController(out _);

    private static Intro2Controller CreateController(out FakeKeyboard keyboard)
    {
        keyboard = new FakeKeyboard();
        ScreenManager<Screen, IScreenController> views = new(keyboard);
        GameState gameState = new(views, TestMissions.Registry());
        PlayerShip ship = new();
        FakeEliteDraw draw = new();
        RNG rng = new(new FakeRandomSource());
        FakeShipFactory shipFactory = new(draw, rng);
        Universe universe = new(shipFactory, rng);
        AudioController audio = new(new FakeSound(), new Dictionary<string, SfxSample>(), new());
        Pilot pilot = new(draw, audio, universe, ship, rng);
        Trade trade = new(gameState, ship);
        MissionRunner missions = TestMissions.Runner(gameState, ship, trade);

        Combat combat = new(gameState, audio, ship, trade, pilot, universe, draw, shipFactory, rng, missions);
        Stars stars = new(gameState, draw, ship, rng);

        return new Intro2Controller(
            gameState, audio, keyboard, stars, ship, combat, universe, shipFactory, new FakeIntro2View());
    }

    private sealed class FakeIntro2View : IView<Intro2Model>
    {
        public void Draw(Intro2Model model)
        {
            // Drawing is not under test here.
        }
    }
}
