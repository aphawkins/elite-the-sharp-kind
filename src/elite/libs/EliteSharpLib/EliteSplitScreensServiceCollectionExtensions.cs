// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Config;
using EliteSharpLib.Conflict;
using EliteSharpLib.Graphics;
using EliteSharpLib.Ships;
using EliteSharpLib.Trader;
using EliteSharpLib.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Useful.Audio;
using Useful.Config;
using Useful.Controls;

namespace EliteSharpLib;

// Screens already split controller/view: the tier selects which IView
// implementation is registered, and the controller is what goes into the
// screen map. Kept in its own class, separate from
// EliteServiceCollectionExtensions, because the combined registrations
// tripped CA1506's class-coupling limit; screens move here as they convert,
// split across the methods below for the same reason.
internal static class EliteSplitScreensServiceCollectionExtensions
{
    internal static void AddSplitScreens(this IServiceCollection services)
    {
        services.AddSplitConsoleScreens();
        services.AddSplitSequenceScreens();
        services.AddSplitMenuScreens();
    }

    // The screens the commander drives: charts, status, and the menus.
    private static void AddSplitConsoleScreens(this IServiceCollection services)
    {
        services.AddSingleton<IView<GalacticChartModel>>(sp => new GalacticChartView(
            sp.GetRequiredService<IEliteDraw>()));
        services.AddSingleton(sp => new GalacticChartController(
            sp.GetRequiredService<GameState>(),
            sp.GetRequiredService<IKeyboard>(),
            sp.GetRequiredService<PlanetController>(),
            sp.GetRequiredService<PlayerShip>(),
            sp.GetRequiredService<IView<GalacticChartModel>>()));

        services.AddSingleton<IView<QuitModel>>(sp => new QuitView(
            sp.GetRequiredService<IEliteDraw>()));
        services.AddSingleton(sp => new QuitController(
            sp.GetRequiredService<GameState>(),
            sp.GetRequiredService<IKeyboard>(),
            sp.GetRequiredService<IView<QuitModel>>()));

        services.AddSingleton<IView<Intro1Model>>(sp => new Intro1View(
            sp.GetRequiredService<IEliteDraw>()));
        services.AddSingleton(sp => new Intro1Controller(
            sp.GetRequiredService<GameState>(),
            sp.GetRequiredService<AudioController>(),
            sp.GetRequiredService<IKeyboard>(),
            sp.GetRequiredService<PlayerShip>(),
            sp.GetRequiredService<Combat>(),
            sp.GetRequiredService<Universe>(),
            sp.GetRequiredService<IShipFactory>(),
            sp.GetRequiredService<IView<Intro1Model>>(),
            sp.GetRequiredService<ILoggerFactory>().CreateLogger<Intro1Controller>()));

        services.AddSingleton<IView<CommanderStatusModel>>(sp => new CommanderStatusView(
            sp.GetRequiredService<IEliteDraw>()));
        services.AddSingleton(sp => new CommanderStatusController(
            sp.GetRequiredService<GameState>(),
            sp.GetRequiredService<PlayerShip>(),
            sp.GetRequiredService<Trade>(),
            sp.GetRequiredService<PlanetController>(),
            sp.GetRequiredService<Universe>(),
            sp.GetRequiredService<IView<CommanderStatusModel>>()));

        services.AddSingleton<IView<InventoryModel>>(sp => new InventoryView(
            sp.GetRequiredService<IEliteDraw>()));
        services.AddSingleton(sp => new InventoryController(
            sp.GetRequiredService<PlayerShip>(),
            sp.GetRequiredService<Trade>(),
            sp.GetRequiredService<IView<InventoryModel>>()));
    }

    // The screens that play out on their own: mission messages, and the
    // animated sequences that run to a tick count.
    private static void AddSplitSequenceScreens(this IServiceCollection services)
    {
        services.AddSingleton<IView<ThargoidMissionModel>>(sp => new ThargoidMissionView(
            sp.GetRequiredService<IEliteDraw>()));
        services.AddSingleton(sp => new ThargoidMissionController(
            sp.GetRequiredService<GameState>(),
            sp.GetRequiredService<IKeyboard>(),
            sp.GetRequiredService<PlayerShip>(),
            sp.GetRequiredService<IView<ThargoidMissionModel>>()));

        services.AddSingleton<IView<ConstrictorMissionModel>>(sp => new ConstrictorMissionView(
            sp.GetRequiredService<IEliteDraw>()));
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

        services.AddSingleton<IView<EscapeCapsuleModel>>(sp => new EscapeCapsuleView(
            sp.GetRequiredService<IEliteDraw>()));
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

        services.AddSingleton<IView<GameOverModel>>(sp => new GameOverView(
            sp.GetRequiredService<IEliteDraw>()));
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

    // The menu screens: options, the market, equip-ship and the two settings
    // lists. All share the selection-cursor shape, and the settings pair
    // additionally share one IView<SettingsListModel> registration.
    private static void AddSplitMenuScreens(this IServiceCollection services)
    {
        services.AddSingleton<IView<OptionsModel>>(sp => new OptionsView(
            sp.GetRequiredService<IEliteDraw>()));
        services.AddSingleton(sp => new OptionsController(
            sp.GetRequiredService<GameState>(),
            sp.GetRequiredService<IKeyboard>(),
            sp.GetRequiredService<IView<OptionsModel>>()));

        services.AddSingleton<IView<MarketModel>>(sp => new MarketView(
            sp.GetRequiredService<IEliteDraw>()));
        services.AddSingleton(sp => new MarketController(
            sp.GetRequiredService<GameState>(),
            sp.GetRequiredService<IKeyboard>(),
            sp.GetRequiredService<Trade>(),
            sp.GetRequiredService<PlanetController>(),
            sp.GetRequiredService<IView<MarketModel>>()));

        services.AddSingleton<IView<EquipmentModel>>(sp => new EquipmentView(
            sp.GetRequiredService<IEliteDraw>()));
        services.AddSingleton(sp => new EquipmentController(
            sp.GetRequiredService<GameState>(),
            sp.GetRequiredService<IKeyboard>(),
            sp.GetRequiredService<PlayerShip>(),
            sp.GetRequiredService<Trade>(),
            sp.GetRequiredService<Scanner>(),
            sp.GetRequiredService<IView<EquipmentModel>>()));

        services.AddSingleton<IView<SettingsListModel>>(sp => new SettingsListView(
            sp.GetRequiredService<IEliteDraw>()));
        services.AddSingleton(sp => new SettingsController(
            sp.GetRequiredService<GameState>(),
            sp.GetRequiredService<IKeyboard>(),
            sp.GetRequiredService<Space>(),
            sp.GetRequiredService<ConfigFile<EliteConfig>>(),
            sp.GetRequiredService<IView<SettingsListModel>>()));
        services.AddSingleton(sp => new EngineSettingsController(
            sp.GetRequiredService<GameState>(),
            sp.GetRequiredService<IKeyboard>(),
            sp.GetRequiredService<Space>(),
            sp.GetRequiredService<AudioController>(),
            sp.GetRequiredService<ConfigFile<EliteConfig>>(),
            sp.GetRequiredService<IView<SettingsListModel>>()));
    }
}
