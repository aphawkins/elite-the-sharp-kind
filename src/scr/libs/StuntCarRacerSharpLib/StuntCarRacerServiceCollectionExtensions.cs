// 'Stunt Car Racer - The Sharp Kind' - Andy Hawkins 2026.
// 'Stunt Car Racer Remake' - sourceforge.net/projects/stuntcarremake.
// Stunt Car Racer (C) Geoff Crammond / MicroStyle / MicroProse 1989.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StuntCarRacerSharpLib.Config;
using Useful;
using Useful.Abstraction;
using Useful.Abstraction.Config;
using Useful.Assets;
using Useful.Audio;
using Useful.Config;

namespace StuntCarRacerSharpLib;

public static class StuntCarRacerServiceCollectionExtensions
{
    private const string ConfigFileName = "stuntcarracer.sharp";

    // ScrConfig is internal, so Program.Main can't reference or
    // construct a ConfigFile<ScrConfig> directly; this registers it
    // from inside the assembly that can, exposing only the already-public
    // AudioOptions that StuntCarRacerMain's constructor accepts.
    public static IServiceCollection AddScrConfig(this IServiceCollection services, string userDataPath)
    {
        services.AddSingleton(sp => new ConfigFile<ScrConfig>(
            userDataPath,
            ConfigFileName,
            RepairConfig,
            sp.GetRequiredService<ILoggerFactory>().CreateLogger<ConfigFile<ScrConfig>>()));
        services.AddSingleton(sp =>
        {
            ScrConfig config = sp.GetRequiredService<ConfigFile<ScrConfig>>().ReadConfig();
            return new AudioOptions { MusicOn = config.Engine.Sound.Music, EffectsOn = config.Engine.Sound.Effects };
        });
        return services;
    }

    // Exposes the (public) engine settings from the (internal) ScrConfig, so
    // Program.Main - which picks between SoftwareAbstraction and SDLAbstraction
    // and therefore needs to reference Useful.SDL, a dependency
    // StuntCarRacerSharpLib itself deliberately does not have - can read the
    // backend, the tier and the window scale before the DI container exists.
    public static EngineConfigSettings ReadEngineSettings(string userDataPath, ILoggerFactory loggerFactory)
        => EngineConfigReader.Read<ScrConfig>(userDataPath, ConfigFileName, RepairConfig, loggerFactory);

    // The single shared source of entropy for this app instance: an
    // unseeded Random in production, replaceable with a seeded one in
    // tests via RandomSource's constructor seam.
    public static IServiceCollection AddScrRandom(this IServiceCollection services)
    {
        services.AddSingleton(_ => Random.Shared);
        services.AddSingleton<IRandomSource>(sp => new RandomSource(sp.GetRequiredService<Random>()));
        return services;
    }

    // Registers the game itself, as AddEliteMain does for Elite: the
    // composition root asks for the game, not for the pieces it is built from.
    public static IServiceCollection AddScrMain(this IServiceCollection services)
    {
        services.AddSingleton(sp => new StuntCarRacerMain(
            sp.GetRequiredService<IAbstraction>(),
            sp.GetRequiredService<IAssetLocator>(),
            sp.GetRequiredService<AudioOptions>(),
            sp.GetRequiredService<IRandomSource>()));
        services.AddSingleton<IGame>(sp => sp.GetRequiredService<StuntCarRacerMain>());
        services.AddSingleton<IGameApp>(sp => sp.GetRequiredService<StuntCarRacerMain>());
        return services;
    }

    // Stunt Car Racer has no settings of its own yet, so this is the shared
    // engine repair - which it previously skipped altogether, leaving a bad
    // backend or tier in the file to be discovered at startup.
    internal static bool RepairConfig(ScrConfig config) => config.Repair();
}
