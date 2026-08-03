// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Abstractions.Views;
using EliteSharp.Renditions.SixteenBit;
using EliteSharpLib.Audio;
using EliteSharpLib.Fakes;
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

// When the escape capsule's alert lapses, with no renderer involved - the
// counter is the controller's, so both tiers stop showing the alert on the
// same tick without either view owning a timer.
public class EscapeCapsuleControllerTests
{
    [Fact]
    public void TheAlertIsUpAsSoonAsTheCapsuleLaunches()
    {
        EscapeCapsuleController controller = CreateController();

        controller.Reset();

        Assert.True(controller.BuildModel().IsAlertVisible);
    }

    [Fact]
    public void TheAlertIsStillUpOnTheLastTickOfTheLaunch()
    {
        EscapeCapsuleController controller = CreateController();
        controller.Reset();

        Tick(controller, 89);

        Assert.True(controller.BuildModel().IsAlertVisible);
    }

    [Fact]
    public void TheAlertLapsesAfterNinetyTicks()
    {
        EscapeCapsuleController controller = CreateController();
        controller.Reset();

        Tick(controller, 90);

        Assert.False(controller.BuildModel().IsAlertVisible);
    }

    private static void Tick(EscapeCapsuleController controller, int ticks)
    {
        for (int i = 0; i < ticks; i++)
        {
            controller.Update();
        }
    }

    private static EscapeCapsuleController CreateController()
    {
        ScreenManager<Screen, IScreenController> views = new(new FakeKeyboard());
        GameState gameState = new(views, TestMissions.Registry());
        PlayerShip ship = new();
        Trade trade = new(gameState, ship);
        FakeEliteDraw draw = new();
        RNG rng = new(new FakeRandomSource());
        FakeShipFactory shipFactory = new(draw, rng);
        Universe universe = new(shipFactory, rng);

        // The two effects the sequence plays; AudioController looks a sample up
        // whether or not effects are switched on.
        Dictionary<string, SfxSample> sfx = new()
        {
            { nameof(SoundEffect.Launch), new(32) },
            { nameof(SoundEffect.Explode), new(23) },
        };
        AudioController audio = new(new FakeSound(), sfx, new());
        Stars stars = new(gameState, draw, ship, new SixteenBitRendition().CreateStarfieldRenderer(draw), rng);
        Pilot pilot = new(draw, audio, universe, ship, rng);

        return new EscapeCapsuleController(
            gameState,
            audio,
            stars,
            ship,
            trade,
            universe,
            pilot,
            draw,
            shipFactory,
            rng,
            new FakeEscapeCapsuleView());
    }

    private sealed class FakeEscapeCapsuleView : IView<EscapeCapsuleModel>
    {
        public void Draw(EscapeCapsuleModel model)
        {
            // Drawing is not under test here.
        }
    }
}
