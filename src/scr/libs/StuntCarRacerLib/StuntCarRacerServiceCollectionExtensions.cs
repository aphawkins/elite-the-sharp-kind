// 'Stunt Car Racer - The Sharp Kind' - Andy Hawkins 2026.
// 'Stunt Car Racer Remake' - sourceforge.net/projects/stuntcarremake.
// Stunt Car Racer (C) Geoff Crammond / MicroStyle / MicroProse 1989.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StuntCarRacerLib.Config;
using Useful.Audio;
using Useful.Config;

namespace StuntCarRacerLib;

public static class StuntCarRacerServiceCollectionExtensions
{
    private const string ConfigFileName = "stuntcarracersharp.cfg";

    // ScrConfigSettings is internal, so Program.Main can't reference or
    // construct a ConfigFile<ScrConfigSettings> directly; this registers it
    // from inside the assembly that can, exposing only the already-public
    // AudioOptions that StuntCarRacerMain's constructor accepts.
    public static IServiceCollection AddScrConfig(this IServiceCollection services, string userDataPath)
    {
        services.AddSingleton(sp => new ConfigFile<ScrConfigSettings>(
            userDataPath,
            ConfigFileName,
            null,
            sp.GetRequiredService<ILoggerFactory>().CreateLogger<ConfigFile<ScrConfigSettings>>()));
        services.AddSingleton(sp =>
        {
            ScrConfigSettings config = sp.GetRequiredService<ConfigFile<ScrConfigSettings>>().ReadConfig();
            return new AudioOptions { MusicOn = config.MusicOn, EffectsOn = config.EffectsOn };
        });
        return services;
    }
}
