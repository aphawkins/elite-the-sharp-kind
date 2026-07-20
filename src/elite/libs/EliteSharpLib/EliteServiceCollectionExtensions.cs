// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Config;
using Microsoft.Extensions.DependencyInjection;
using Useful.Abstraction;
using Useful.Assets;

namespace EliteSharpLib;

public static class EliteServiceCollectionExtensions
{
    // ConfigFile is internal, so Program.Main can't reference or construct it
    // directly; this registers it from inside the assembly that can.
    public static IServiceCollection AddEliteConfig(this IServiceCollection services, string userDataPath)
        => services.AddSingleton(_ => new ConfigFile(userDataPath));

    // EliteMain's constructor takes the internal ConfigFile too, for the same reason.
    public static IServiceCollection AddEliteMain(this IServiceCollection services)
    {
        services.AddSingleton(sp => new EliteMain(
            sp.GetRequiredService<IAbstraction>(),
            sp.GetRequiredService<AssetLocator>(),
            sp.GetRequiredService<ConfigFile>()));
        services.AddSingleton<IGame>(sp => sp.GetRequiredService<EliteMain>());
        return services;
    }
}
