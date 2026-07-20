// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Audio;
using EliteSharpLib.Config;
using EliteSharpLib.Conflict;
using EliteSharpLib.Graphics;
using EliteSharpLib.Save;
using EliteSharpLib.Ships;
using EliteSharpLib.Trader;
using EliteSharpLib.Views;
using Microsoft.Extensions.DependencyInjection;
using Useful.Abstraction;
using Useful.Assets;
using Useful.Audio;
using Useful.Controls;
using Useful.Graphics;

namespace EliteSharpLib;

public static class EliteServiceCollectionExtensions
{
    // ConfigFile is internal, so Program.Main can't reference or construct it
    // directly; this registers it from inside the assembly that can.
    public static IServiceCollection AddEliteConfig(this IServiceCollection services, string userDataPath)
        => services.AddSingleton(_ => new ConfigFile(userDataPath));

    // The whole domain graph below is internal to EliteSharpLib (same
    // reason as ConfigFile above), so it can only be registered from in
    // here; EliteMain's constructor now just receives it instead of
    // building it.
    public static IServiceCollection AddEliteMain(this IServiceCollection services)
    {
        services.AddSingleton<IAssetLocator>(sp => sp.GetRequiredService<AssetLocator>());

        services.AddSingleton(sp => new ScreenManager<Screen, IView>(sp.GetRequiredService<IKeyboard>()));
        services.AddSingleton(sp => new GameState(sp.GetRequiredService<ScreenManager<Screen, IView>>())
        {
            Config = sp.GetRequiredService<ConfigFile>().ReadConfig(),
        });
        services.AddSingleton(_ => new PlayerShip());
        services.AddSingleton(sp => new Trade(sp.GetRequiredService<GameState>(), sp.GetRequiredService<PlayerShip>()));
        services.AddSingleton(sp => new PlanetController(sp.GetRequiredService<GameState>()));
        services.AddSingleton<IShipRenderer>(sp => new ShipRenderer(
            sp.GetRequiredService<GameState>(),
            sp.GetRequiredService<IGraphics>(),
            sp.GetRequiredService<IAssetLocator>()));
        services.AddSingleton<IEliteDraw>(sp => new EliteDraw(
            sp.GetRequiredService<GameState>(),
            sp.GetRequiredService<IGraphics>(),
            sp.GetRequiredService<IAssetLocator>(),
            sp.GetRequiredService<IShipRenderer>()));
        services.AddSingleton<IShipFactory>(sp => ShipFactory.Create(
            sp.GetRequiredService<IAssetLocator>(),
            sp.GetRequiredService<IEliteDraw>()));
        services.AddSingleton(sp => new Universe(sp.GetRequiredService<IShipFactory>()));
        services.AddSingleton(sp => new Stars(
            sp.GetRequiredService<GameState>(),
            sp.GetRequiredService<IEliteDraw>(),
            sp.GetRequiredService<PlayerShip>()));
        services.AddSingleton(sp =>
        {
            ConfigSettings config = sp.GetRequiredService<GameState>().Config;
            return new AudioController(
                sp.GetRequiredService<ISound>(),
                BuildEliteSfx(),
                new() { MusicOn = config.MusicOn, EffectsOn = config.EffectsOn });
        });
        services.AddSingleton(sp => new Pilot(
            sp.GetRequiredService<IEliteDraw>(),
            sp.GetRequiredService<AudioController>(),
            sp.GetRequiredService<Universe>(),
            sp.GetRequiredService<PlayerShip>()));
        services.AddSingleton(sp => new Combat(
            sp.GetRequiredService<GameState>(),
            sp.GetRequiredService<AudioController>(),
            sp.GetRequiredService<PlayerShip>(),
            sp.GetRequiredService<Trade>(),
            sp.GetRequiredService<Pilot>(),
            sp.GetRequiredService<Universe>(),
            sp.GetRequiredService<IEliteDraw>(),
            sp.GetRequiredService<IShipFactory>()));
        services.AddSingleton(sp => new SaveFile(
            sp.GetRequiredService<GameState>(),
            sp.GetRequiredService<PlayerShip>(),
            sp.GetRequiredService<Trade>(),
            sp.GetRequiredService<PlanetController>(),
            sp.GetRequiredService<ConfigFile>().BaseDirectory));
        services.AddSingleton(sp => new Space(
            sp.GetRequiredService<GameState>(),
            sp.GetRequiredService<AudioController>(),
            sp.GetRequiredService<Pilot>(),
            sp.GetRequiredService<Combat>(),
            sp.GetRequiredService<Trade>(),
            sp.GetRequiredService<PlayerShip>(),
            sp.GetRequiredService<PlanetController>(),
            sp.GetRequiredService<Stars>(),
            sp.GetRequiredService<Universe>(),
            sp.GetRequiredService<IEliteDraw>()));
        services.AddSingleton(sp => new Scanner(
            sp.GetRequiredService<GameState>(),
            sp.GetRequiredService<IEliteDraw>(),
            sp.GetRequiredService<Universe>(),
            sp.GetRequiredService<PlayerShip>(),
            sp.GetRequiredService<Combat>()));

        services.AddEliteViews();

        // Populating the screen map needs every view built above, so it
        // happens here rather than inside EliteMain's own constructor —
        // EliteMain no longer news up (or even sees) any view.
        services.AddSingleton(sp =>
        {
            ScreenManager<Screen, IView> views = sp.GetRequiredService<ScreenManager<Screen, IView>>();
            views.Add(Screen.IntroOne, sp.GetRequiredService<Intro1View>());
            views.Add(Screen.IntroTwo, sp.GetRequiredService<Intro2View>());
            views.Add(Screen.GalacticChart, sp.GetRequiredService<GalacticChartView>());
            views.Add(Screen.ShortRangeChart, sp.GetRequiredService<ShortRangeChartView>());
            views.Add(Screen.PlanetData, sp.GetRequiredService<PlanetDataView>());
            views.Add(Screen.MarketPrices, sp.GetRequiredService<MarketView>());
            views.Add(Screen.CommanderStatus, sp.GetRequiredService<CommanderStatusView>());
            views.Add(Screen.FrontView, sp.GetRequiredService<PilotFrontView>());
            views.Add(Screen.RearView, sp.GetRequiredService<PilotRearView>());
            views.Add(Screen.LeftView, sp.GetRequiredService<PilotLeftView>());
            views.Add(Screen.RightView, sp.GetRequiredService<PilotRightView>());
            views.Add(Screen.Docking, sp.GetRequiredService<DockingView>());
            views.Add(Screen.Undocking, sp.GetRequiredService<LaunchView>());
            views.Add(Screen.Hyperspace, sp.GetRequiredService<HyperspaceView>());
            views.Add(Screen.Inventory, sp.GetRequiredService<InventoryView>());
            views.Add(Screen.EquipShip, sp.GetRequiredService<EquipmentView>());
            views.Add(Screen.Options, sp.GetRequiredService<OptionsView>());
            views.Add(Screen.LoadCommander, sp.GetRequiredService<LoadCommanderView>());
            views.Add(Screen.SaveCommander, sp.GetRequiredService<SaveCommanderView>());
            views.Add(Screen.Quit, sp.GetRequiredService<QuitView>());
            views.Add(Screen.Settings, sp.GetRequiredService<SettingsView>());
            views.Add(Screen.MissionOne, sp.GetRequiredService<ConstrictorMissionView>());
            views.Add(Screen.MissionTwo, sp.GetRequiredService<ThargoidMissionView>());
            views.Add(Screen.EscapeCapsule, sp.GetRequiredService<EscapeCapsuleView>());
            views.Add(Screen.GameOver, sp.GetRequiredService<GameOverView>());

            return new EliteMain(
                sp.GetRequiredService<IAbstraction>(),
                sp.GetRequiredService<GameState>(),
                sp.GetRequiredService<PlayerShip>(),
                sp.GetRequiredService<IEliteDraw>(),
                sp.GetRequiredService<Universe>(),
                sp.GetRequiredService<Stars>(),
                sp.GetRequiredService<Pilot>(),
                sp.GetRequiredService<Combat>(),
                sp.GetRequiredService<SaveFile>(),
                sp.GetRequiredService<Space>(),
                sp.GetRequiredService<Scanner>(),
                sp.GetRequiredService<AudioController>());
        });
        services.AddSingleton<IGame>(sp => sp.GetRequiredService<EliteMain>());
        return services;
    }

    // The ~25 views EliteMain used to construct itself, now registered so
    // AddEliteMain's screen-map factory above can resolve them.
    private static void AddEliteViews(this IServiceCollection services)
    {
        services.AddSingleton(sp => new Intro1View(
            sp.GetRequiredService<GameState>(),
            sp.GetRequiredService<AudioController>(),
            sp.GetRequiredService<IKeyboard>(),
            sp.GetRequiredService<PlayerShip>(),
            sp.GetRequiredService<Combat>(),
            sp.GetRequiredService<Universe>(),
            sp.GetRequiredService<IEliteDraw>(),
            sp.GetRequiredService<IShipFactory>()));
        services.AddSingleton(sp => new Intro2View(
            sp.GetRequiredService<GameState>(),
            sp.GetRequiredService<AudioController>(),
            sp.GetRequiredService<IKeyboard>(),
            sp.GetRequiredService<Stars>(),
            sp.GetRequiredService<PlayerShip>(),
            sp.GetRequiredService<Combat>(),
            sp.GetRequiredService<Universe>(),
            sp.GetRequiredService<IEliteDraw>(),
            sp.GetRequiredService<IShipFactory>()));
        services.AddSingleton(sp => new GalacticChartView(
            sp.GetRequiredService<GameState>(),
            sp.GetRequiredService<IEliteDraw>(),
            sp.GetRequiredService<IKeyboard>(),
            sp.GetRequiredService<PlanetController>(),
            sp.GetRequiredService<PlayerShip>()));
        services.AddSingleton(sp => new ShortRangeChartView(
            sp.GetRequiredService<GameState>(),
            sp.GetRequiredService<IEliteDraw>(),
            sp.GetRequiredService<IKeyboard>(),
            sp.GetRequiredService<PlanetController>(),
            sp.GetRequiredService<PlayerShip>()));
        services.AddSingleton(sp => new PlanetDataView(
            sp.GetRequiredService<GameState>(),
            sp.GetRequiredService<IEliteDraw>(),
            sp.GetRequiredService<PlanetController>()));
        services.AddSingleton(sp => new MarketView(
            sp.GetRequiredService<GameState>(),
            sp.GetRequiredService<IEliteDraw>(),
            sp.GetRequiredService<IKeyboard>(),
            sp.GetRequiredService<Trade>(),
            sp.GetRequiredService<PlanetController>()));
        services.AddSingleton(sp => new CommanderStatusView(
            sp.GetRequiredService<GameState>(),
            sp.GetRequiredService<IEliteDraw>(),
            sp.GetRequiredService<PlayerShip>(),
            sp.GetRequiredService<Trade>(),
            sp.GetRequiredService<PlanetController>(),
            sp.GetRequiredService<Universe>()));
        services.AddSingleton(sp => new PilotFrontView(
            sp.GetRequiredService<GameState>(),
            sp.GetRequiredService<IKeyboard>(),
            sp.GetRequiredService<Stars>(),
            sp.GetRequiredService<Pilot>(),
            sp.GetRequiredService<PlayerShip>(),
            sp.GetRequiredService<Space>(),
            sp.GetRequiredService<IEliteDraw>(),
            sp.GetRequiredService<Combat>()));
        services.AddSingleton(sp => new PilotRearView(
            sp.GetRequiredService<GameState>(),
            sp.GetRequiredService<IKeyboard>(),
            sp.GetRequiredService<Stars>(),
            sp.GetRequiredService<Pilot>(),
            sp.GetRequiredService<PlayerShip>(),
            sp.GetRequiredService<Space>(),
            sp.GetRequiredService<IEliteDraw>(),
            sp.GetRequiredService<Combat>()));
        services.AddSingleton(sp => new PilotLeftView(
            sp.GetRequiredService<GameState>(),
            sp.GetRequiredService<IKeyboard>(),
            sp.GetRequiredService<Stars>(),
            sp.GetRequiredService<Pilot>(),
            sp.GetRequiredService<PlayerShip>(),
            sp.GetRequiredService<Space>(),
            sp.GetRequiredService<IEliteDraw>(),
            sp.GetRequiredService<Combat>()));
        services.AddSingleton(sp => new PilotRightView(
            sp.GetRequiredService<GameState>(),
            sp.GetRequiredService<IKeyboard>(),
            sp.GetRequiredService<Stars>(),
            sp.GetRequiredService<Pilot>(),
            sp.GetRequiredService<PlayerShip>(),
            sp.GetRequiredService<Space>(),
            sp.GetRequiredService<IEliteDraw>(),
            sp.GetRequiredService<Combat>()));
        services.AddSingleton(sp => new DockingView(
            sp.GetRequiredService<GameState>(),
            sp.GetRequiredService<AudioController>(),
            sp.GetRequiredService<Space>(),
            sp.GetRequiredService<Combat>(),
            sp.GetRequiredService<Universe>(),
            sp.GetRequiredService<IEliteDraw>()));
        services.AddSingleton(sp => new LaunchView(
            sp.GetRequiredService<GameState>(),
            sp.GetRequiredService<AudioController>(),
            sp.GetRequiredService<Space>(),
            sp.GetRequiredService<Combat>(),
            sp.GetRequiredService<Universe>(),
            sp.GetRequiredService<IEliteDraw>()));
        services.AddSingleton(sp => new HyperspaceView(
            sp.GetRequiredService<GameState>(),
            sp.GetRequiredService<AudioController>(),
            sp.GetRequiredService<IEliteDraw>()));
        services.AddSingleton(sp => new InventoryView(
            sp.GetRequiredService<IEliteDraw>(),
            sp.GetRequiredService<PlayerShip>(),
            sp.GetRequiredService<Trade>()));
        services.AddSingleton(sp => new EquipmentView(
            sp.GetRequiredService<GameState>(),
            sp.GetRequiredService<IEliteDraw>(),
            sp.GetRequiredService<IKeyboard>(),
            sp.GetRequiredService<PlayerShip>(),
            sp.GetRequiredService<Trade>(),
            sp.GetRequiredService<Scanner>()));
        services.AddSingleton(sp => new OptionsView(
            sp.GetRequiredService<GameState>(),
            sp.GetRequiredService<IEliteDraw>(),
            sp.GetRequiredService<IKeyboard>()));
        services.AddSingleton(sp => new LoadCommanderView(
            sp.GetRequiredService<GameState>(),
            sp.GetRequiredService<IEliteDraw>(),
            sp.GetRequiredService<IKeyboard>(),
            sp.GetRequiredService<SaveFile>()));
        services.AddSingleton(sp => new SaveCommanderView(
            sp.GetRequiredService<GameState>(),
            sp.GetRequiredService<IEliteDraw>(),
            sp.GetRequiredService<IKeyboard>(),
            sp.GetRequiredService<SaveFile>()));
        services.AddSingleton(sp => new QuitView(
            sp.GetRequiredService<GameState>(),
            sp.GetRequiredService<IEliteDraw>(),
            sp.GetRequiredService<IKeyboard>()));
        services.AddSingleton(sp => new SettingsView(
            sp.GetRequiredService<GameState>(),
            sp.GetRequiredService<IEliteDraw>(),
            sp.GetRequiredService<IKeyboard>(),
            sp.GetRequiredService<ConfigFile>()));
        services.AddSingleton(sp => new ConstrictorMissionView(
            sp.GetRequiredService<GameState>(),
            sp.GetRequiredService<IEliteDraw>(),
            sp.GetRequiredService<IKeyboard>(),
            sp.GetRequiredService<PlayerShip>(),
            sp.GetRequiredService<Trade>(),
            sp.GetRequiredService<Combat>(),
            sp.GetRequiredService<Universe>(),
            sp.GetRequiredService<IShipFactory>()));
        services.AddSingleton(sp => new ThargoidMissionView(
            sp.GetRequiredService<GameState>(),
            sp.GetRequiredService<IEliteDraw>(),
            sp.GetRequiredService<IKeyboard>(),
            sp.GetRequiredService<PlayerShip>()));
        services.AddSingleton(sp => new EscapeCapsuleView(
            sp.GetRequiredService<GameState>(),
            sp.GetRequiredService<AudioController>(),
            sp.GetRequiredService<Stars>(),
            sp.GetRequiredService<PlayerShip>(),
            sp.GetRequiredService<Trade>(),
            sp.GetRequiredService<Universe>(),
            sp.GetRequiredService<Pilot>(),
            sp.GetRequiredService<IEliteDraw>(),
            sp.GetRequiredService<IShipFactory>()));
        services.AddSingleton(sp => new GameOverView(
            sp.GetRequiredService<GameState>(),
            sp.GetRequiredService<AudioController>(),
            sp.GetRequiredService<Stars>(),
            sp.GetRequiredService<PlayerShip>(),
            sp.GetRequiredService<Combat>(),
            sp.GetRequiredService<Universe>(),
            sp.GetRequiredService<IEliteDraw>(),
            sp.GetRequiredService<IShipFactory>()));
    }

    // TODO: improve this (moved from EliteMain, see backlog)
    private static Dictionary<string, SfxSample> BuildEliteSfx() => new()
    {
        { nameof(SoundEffect.Launch), new(32) },
        { nameof(SoundEffect.Crash), new(7) },
        { nameof(SoundEffect.Dock), new(36) },
        { nameof(SoundEffect.Gameover), new(24) },
        { nameof(SoundEffect.Pulse), new(4) },
        { nameof(SoundEffect.HitEnemy), new(4) },
        { nameof(SoundEffect.Explode), new(23) },
        { nameof(SoundEffect.Ecm), new(23) },
        { nameof(SoundEffect.Missile), new(25) },
        { nameof(SoundEffect.Hyperspace), new(37) },
        { nameof(SoundEffect.IncomingFire1), new(4) },
        { nameof(SoundEffect.IncomingFire2), new(5) },
        { nameof(SoundEffect.Beep), new(2) },
        { nameof(SoundEffect.Boop), new(7) },
    };
}
