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
    // building it. The ~25 view registrations still happen inside
    // EliteMain itself — that's a separate backlog item.
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
        services.AddSingleton<IEliteDraw>(sp => new EliteDraw(
            sp.GetRequiredService<GameState>(),
            sp.GetRequiredService<IGraphics>(),
            sp.GetRequiredService<IAssetLocator>()));
        services.AddSingleton<IShipFactory>(sp => ShipFactory.Create(
            sp.GetRequiredService<IAssetLocator>(),
            sp.GetRequiredService<IEliteDraw>()));
        services.AddSingleton(sp => new Universe(sp.GetRequiredService<IShipFactory>()));
        services.AddSingleton(sp => new Stars(
            sp.GetRequiredService<GameState>(),
            sp.GetRequiredService<IEliteDraw>(),
            sp.GetRequiredService<PlayerShip>()));
        services.AddSingleton(sp => new AudioController(sp.GetRequiredService<ISound>(), BuildEliteSfx()));
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

        services.AddSingleton(sp => new EliteMain(
            sp.GetRequiredService<IAbstraction>(),
            sp.GetRequiredService<ConfigFile>(),
            sp.GetRequiredService<GameState>(),
            sp.GetRequiredService<PlayerShip>(),
            sp.GetRequiredService<Trade>(),
            sp.GetRequiredService<PlanetController>(),
            sp.GetRequiredService<IEliteDraw>(),
            sp.GetRequiredService<IShipFactory>(),
            sp.GetRequiredService<Universe>(),
            sp.GetRequiredService<Stars>(),
            sp.GetRequiredService<Pilot>(),
            sp.GetRequiredService<Combat>(),
            sp.GetRequiredService<SaveFile>(),
            sp.GetRequiredService<Space>(),
            sp.GetRequiredService<Scanner>(),
            sp.GetRequiredService<AudioController>(),
            sp.GetRequiredService<ScreenManager<Screen, IView>>()));
        services.AddSingleton<IGame>(sp => sp.GetRequiredService<EliteMain>());
        return services;
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
