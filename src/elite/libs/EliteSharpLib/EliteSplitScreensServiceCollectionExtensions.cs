// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Config;
using EliteSharpLib.Conflict;
using EliteSharpLib.Graphics;
using EliteSharpLib.Missions;
using EliteSharpLib.Save;
using EliteSharpLib.Ships;
using EliteSharpLib.Trader;
using EliteSharpLib.Views;
using EliteSharpLib.Views.EightBit;
using EliteSharpLib.Views.SixteenBit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Useful.Audio;
using Useful.Config;
using Useful.Controls;

namespace EliteSharpLib;

// The screen registrations: the tier selects which IView implementation is
// registered, and the controller is what goes into the screen map. Kept in its
// own class, separate from EliteServiceCollectionExtensions, because the
// combined registrations tripped CA1506's class-coupling limit - the same
// reason they are split across the methods below, and the reason the sequence
// and flight screens live in
// EliteSplitAnimatedScreensServiceCollectionExtensions rather than here.
internal static class EliteSplitScreensServiceCollectionExtensions
{
    internal static void AddSplitScreens(this IServiceCollection services)
    {
        services.AddSplitConsoleScreens();
        services.AddSplitStatusScreens();
        services.AddSplitMenuScreens();
        services.AddSplitTextEntryScreens();
        services.AddSplitAnimatedScreens();
    }

    // Kept separate so a per-tier IView registration only has to reference
    // this bool, not IAssetLocator/SystemTier themselves - each one otherwise
    // adds two types to its method's CA1506 class-coupling count for a check
    // that's the same everywhere.
    // The screens the commander drives: charts, status, and the menus.
    private static void AddSplitConsoleScreens(this IServiceCollection services)
    {
        services.AddSingleton<IView<GalacticChartModel>>(sp => EliteServiceCollectionExtensions.IsEightBit(sp)
            ? new GalacticChartView8Bit(sp.GetRequiredService<IEliteDraw>())
            : new GalacticChartView16Bit(sp.GetRequiredService<IEliteDraw>()));
        services.AddSingleton(sp => new GalacticChartController(
            sp.GetRequiredService<GameState>(),
            sp.GetRequiredService<IKeyboard>(),
            sp.GetRequiredService<PlanetController>(),
            sp.GetRequiredService<PlayerShip>(),
            sp.GetRequiredService<IView<GalacticChartModel>>()));

        services.AddSingleton<IView<ShortRangeChartModel>>(sp => EliteServiceCollectionExtensions.IsEightBit(sp)
            ? new ShortRangeChartView8Bit(sp.GetRequiredService<IEliteDraw>())
            : new ShortRangeChartView16Bit(sp.GetRequiredService<IEliteDraw>()));
        services.AddSingleton(sp => new ShortRangeChartController(
            sp.GetRequiredService<GameState>(),
            sp.GetRequiredService<IEliteDraw>(),
            sp.GetRequiredService<IKeyboard>(),
            sp.GetRequiredService<PlanetController>(),
            sp.GetRequiredService<PlayerShip>(),
            sp.GetRequiredService<IView<ShortRangeChartModel>>()));

        services.AddSingleton<IView<QuitModel>>(sp => EliteServiceCollectionExtensions.IsEightBit(sp)
            ? new QuitView8Bit(sp.GetRequiredService<IEliteDraw>())
            : new QuitView16Bit(sp.GetRequiredService<IEliteDraw>()));
        services.AddSingleton(sp => new QuitController(
            sp.GetRequiredService<GameState>(),
            sp.GetRequiredService<IKeyboard>(),
            sp.GetRequiredService<IView<QuitModel>>()));

        services.AddSingleton<IView<Intro1Model>>(sp => EliteServiceCollectionExtensions.IsEightBit(sp)
            ? new Intro1View8Bit(sp.GetRequiredService<IEliteDraw>())
            : new Intro1View16Bit(sp.GetRequiredService<IEliteDraw>()));
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
    }

    // The screens that just report status: commander status, inventory and
    // planet data. Split from AddSplitConsoleScreens once their 8-bit views
    // pushed it over CA1506's per-method limit.
    private static void AddSplitStatusScreens(this IServiceCollection services)
    {
        services.AddSingleton<IView<CommanderStatusModel>>(sp => EliteServiceCollectionExtensions.IsEightBit(sp)
            ? new CommanderStatusView8Bit(sp.GetRequiredService<IEliteDraw>())
            : new CommanderStatusView16Bit(sp.GetRequiredService<IEliteDraw>()));
        services.AddSingleton(sp => new CommanderStatusController(
            sp.GetRequiredService<GameState>(),
            sp.GetRequiredService<PlayerShip>(),
            sp.GetRequiredService<Trade>(),
            sp.GetRequiredService<PlanetController>(),
            sp.GetRequiredService<Universe>(),
            sp.GetRequiredService<IView<CommanderStatusModel>>()));

        services.AddSingleton<IView<InventoryModel>>(sp => EliteServiceCollectionExtensions.IsEightBit(sp)
            ? new InventoryView8Bit(sp.GetRequiredService<IEliteDraw>())
            : new InventoryView16Bit(sp.GetRequiredService<IEliteDraw>()));
        services.AddSingleton(sp => new InventoryController(
            sp.GetRequiredService<PlayerShip>(),
            sp.GetRequiredService<Trade>(),
            sp.GetRequiredService<IView<InventoryModel>>()));

        services.AddSingleton<IView<PlanetDataModel>>(sp => EliteServiceCollectionExtensions.IsEightBit(sp)
            ? new PlanetDataView8Bit(sp.GetRequiredService<IEliteDraw>())
            : new PlanetDataView16Bit(sp.GetRequiredService<IEliteDraw>()));
        services.AddSingleton(sp => new PlanetDataController(
            sp.GetRequiredService<GameState>(),
            sp.GetRequiredService<PlanetController>(),
            sp.GetRequiredService<RNG>(),
            sp.GetRequiredService<MissionRunner>(),
            sp.GetRequiredService<IView<PlanetDataModel>>()));
    }

    // The menu screens: options, the market, equip-ship and the two settings
    // lists. All share the selection-cursor shape, and the settings pair
    // additionally share one IView<SettingsListModel> registration.
    private static void AddSplitMenuScreens(this IServiceCollection services)
    {
        services.AddSingleton<IView<OptionsModel>>(sp => EliteServiceCollectionExtensions.IsEightBit(sp)
            ? new OptionsView8Bit(sp.GetRequiredService<IEliteDraw>())
            : new OptionsView16Bit(sp.GetRequiredService<IEliteDraw>()));
        services.AddSingleton(sp => new OptionsController(
            sp.GetRequiredService<GameState>(),
            sp.GetRequiredService<IKeyboard>(),
            sp.GetRequiredService<IView<OptionsModel>>()));

        services.AddSingleton<IView<MarketModel>>(sp => EliteServiceCollectionExtensions.IsEightBit(sp)
            ? new MarketView8Bit(sp.GetRequiredService<IEliteDraw>())
            : new MarketView16Bit(sp.GetRequiredService<IEliteDraw>()));
        services.AddSingleton(sp => new MarketController(
            sp.GetRequiredService<GameState>(),
            sp.GetRequiredService<IKeyboard>(),
            sp.GetRequiredService<Trade>(),
            sp.GetRequiredService<PlanetController>(),
            sp.GetRequiredService<IView<MarketModel>>()));

        services.AddSingleton<IView<EquipmentModel>>(sp => EliteServiceCollectionExtensions.IsEightBit(sp)
            ? new EquipmentView8Bit(sp.GetRequiredService<IEliteDraw>())
            : new EquipmentView16Bit(sp.GetRequiredService<IEliteDraw>()));
        services.AddSingleton(sp => new EquipmentController(
            sp.GetRequiredService<GameState>(),
            sp.GetRequiredService<IKeyboard>(),
            sp.GetRequiredService<PlayerShip>(),
            sp.GetRequiredService<Trade>(),
            sp.GetRequiredService<ScannerBase>(),
            sp.GetRequiredService<IView<EquipmentModel>>()));

        services.AddSingleton<IView<SettingsListModel>>(sp => EliteServiceCollectionExtensions.IsEightBit(sp)
            ? new SettingsListView8Bit(sp.GetRequiredService<IEliteDraw>())
            : new SettingsListView16Bit(sp.GetRequiredService<IEliteDraw>()));
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

    // The name-typing screens: load and save commander.
    private static void AddSplitTextEntryScreens(this IServiceCollection services)
    {
        services.AddSingleton<IView<LoadCommanderModel>>(sp => EliteServiceCollectionExtensions.IsEightBit(sp)
            ? new LoadCommanderView8Bit(sp.GetRequiredService<IEliteDraw>())
            : new LoadCommanderView16Bit(sp.GetRequiredService<IEliteDraw>()));
        services.AddSingleton(sp => new LoadCommanderController(
            sp.GetRequiredService<GameState>(),
            sp.GetRequiredService<IKeyboard>(),
            sp.GetRequiredService<SaveFile>(),
            sp.GetRequiredService<IView<LoadCommanderModel>>()));

        services.AddSingleton<IView<SaveCommanderModel>>(sp => EliteServiceCollectionExtensions.IsEightBit(sp)
            ? new SaveCommanderView8Bit(sp.GetRequiredService<IEliteDraw>())
            : new SaveCommanderView16Bit(sp.GetRequiredService<IEliteDraw>()));
        services.AddSingleton(sp => new SaveCommanderController(
            sp.GetRequiredService<GameState>(),
            sp.GetRequiredService<IKeyboard>(),
            sp.GetRequiredService<SaveFile>(),
            sp.GetRequiredService<IView<SaveCommanderModel>>()));
    }
}
