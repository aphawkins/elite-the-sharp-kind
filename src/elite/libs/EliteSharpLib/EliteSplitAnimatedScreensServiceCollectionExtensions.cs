// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Conflict;
using EliteSharpLib.Graphics;
using EliteSharpLib.Ships;
using EliteSharpLib.Trader;
using EliteSharpLib.Views;
using EliteSharpLib.Views.EightBit;
using EliteSharpLib.Views.SixteenBit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Useful.Assets;
using Useful.Audio;
using Useful.Controls;

namespace EliteSharpLib;

// The half of the split-screen registrations whose screens run under their
// own animation: the sequence screens and the flight screens. Split out of
// EliteSplitScreensServiceCollectionExtensions once each screen's second
// (8-bit) view pushed that class over CA1506's class-coupling limit (96) -
// the metric is per class, so an extra static class is what resolves it.
internal static class EliteSplitAnimatedScreensServiceCollectionExtensions
{
    internal static void AddSplitAnimatedScreens(this IServiceCollection services)
    {
        services.AddSplitSequenceScreens();
        services.AddSplitFlightScreens();
    }

    // Kept separate for the same reason as its twin in
    // EliteSplitScreensServiceCollectionExtensions: a per-tier IView
    // registration only has to reference this bool, not IAssetLocator and
    // SystemTier themselves.
    private static bool IsEightBit(IServiceProvider sp) => sp.GetRequiredService<IAssetLocator>().Tier == SystemTier.EightBit;

    // The screens that play out on their own: mission messages, and the
    // animated sequences that run to a tick count.
    private static void AddSplitSequenceScreens(this IServiceCollection services)
    {
        services.AddSingleton<IView<ThargoidMissionModel>>(sp => IsEightBit(sp)
            ? new ThargoidMissionView8Bit(sp.GetRequiredService<IEliteDraw>())
            : new ThargoidMissionView16Bit(sp.GetRequiredService<IEliteDraw>()));
        services.AddSingleton(sp => new ThargoidMissionController(
            sp.GetRequiredService<GameState>(),
            sp.GetRequiredService<IKeyboard>(),
            sp.GetRequiredService<PlayerShip>(),
            sp.GetRequiredService<IView<ThargoidMissionModel>>()));

        services.AddSingleton<IView<ConstrictorMissionModel>>(sp => IsEightBit(sp)
            ? new ConstrictorMissionView8Bit(sp.GetRequiredService<IEliteDraw>())
            : new ConstrictorMissionView16Bit(sp.GetRequiredService<IEliteDraw>()));
        services.AddSingleton(sp => new ConstrictorMissionController(
            sp.GetRequiredService<GameState>(),
            sp.GetRequiredService<IKeyboard>(),
            sp.GetRequiredService<PlayerShip>(),
            sp.GetRequiredService<Trade>(),
            sp.GetRequiredService<Combat>(),
            sp.GetRequiredService<Universe>(),
            sp.GetRequiredService<IShipFactory>(),
            sp.GetRequiredService<IView<ConstrictorMissionModel>>(),
            sp.GetRequiredService<ILoggerFactory>().CreateLogger<ConstrictorMissionController>()));

        services.AddSingleton<IView<EscapeCapsuleModel>>(sp => IsEightBit(sp)
            ? new EscapeCapsuleView8Bit(sp.GetRequiredService<IEliteDraw>())
            : new EscapeCapsuleView16Bit(sp.GetRequiredService<IEliteDraw>()));
        services.AddSingleton(sp => new EscapeCapsuleController(
            sp.GetRequiredService<GameState>(),
            sp.GetRequiredService<AudioController>(),
            sp.GetRequiredService<Stars>(),
            sp.GetRequiredService<PlayerShip>(),
            sp.GetRequiredService<Trade>(),
            sp.GetRequiredService<Universe>(),
            sp.GetRequiredService<Pilot>(),
            sp.GetRequiredService<IEliteDraw>(),
            sp.GetRequiredService<IShipFactory>(),
            sp.GetRequiredService<RNG>(),
            sp.GetRequiredService<IView<EscapeCapsuleModel>>(),
            sp.GetRequiredService<ILoggerFactory>().CreateLogger<EscapeCapsuleController>()));

        services.AddSingleton<IView<GameOverModel>>(sp => IsEightBit(sp)
            ? new GameOverView8Bit(sp.GetRequiredService<IEliteDraw>())
            : new GameOverView16Bit(sp.GetRequiredService<IEliteDraw>()));
        services.AddSingleton(sp => new GameOverController(
            sp.GetRequiredService<GameState>(),
            sp.GetRequiredService<AudioController>(),
            sp.GetRequiredService<Stars>(),
            sp.GetRequiredService<PlayerShip>(),
            sp.GetRequiredService<Combat>(),
            sp.GetRequiredService<Universe>(),
            sp.GetRequiredService<IShipFactory>(),
            sp.GetRequiredService<RNG>(),
            sp.GetRequiredService<IView<GameOverModel>>(),
            sp.GetRequiredService<ILoggerFactory>().CreateLogger<GameOverController>()));
    }

    // The ship parade and the four cockpit windows: screens whose Update
    // drives the universe directly rather than a screen-local timer. The
    // four cockpit windows share one IView<PilotModel> registration, since
    // none varies the layout - PopulateScreens builds the four
    // PilotControllers directly (see CreatePilotController) rather than
    // resolving them here, since they aren't otherwise-distinct types.
    private static void AddSplitFlightScreens(this IServiceCollection services)
    {
        services.AddSingleton<IView<Intro2Model>>(sp => IsEightBit(sp)
            ? new Intro2View8Bit(sp.GetRequiredService<IEliteDraw>())
            : new Intro2View16Bit(sp.GetRequiredService<IEliteDraw>()));
        services.AddSingleton(sp => new Intro2Controller(
            sp.GetRequiredService<GameState>(),
            sp.GetRequiredService<AudioController>(),
            sp.GetRequiredService<IKeyboard>(),
            sp.GetRequiredService<Stars>(),
            sp.GetRequiredService<PlayerShip>(),
            sp.GetRequiredService<Combat>(),
            sp.GetRequiredService<Universe>(),
            sp.GetRequiredService<IShipFactory>(),
            sp.GetRequiredService<IView<Intro2Model>>(),
            sp.GetRequiredService<ILoggerFactory>().CreateLogger<Intro2Controller>()));

        services.AddSingleton<IView<PilotModel>>(sp => IsEightBit(sp)
            ? new PilotView8Bit(
                sp.GetRequiredService<IEliteDraw>(),
                sp.GetRequiredService<GameState>(),
                sp.GetRequiredService<RNG>())
            : new PilotView16Bit(
                sp.GetRequiredService<IEliteDraw>(),
                sp.GetRequiredService<GameState>(),
                sp.GetRequiredService<RNG>()));
    }
}
