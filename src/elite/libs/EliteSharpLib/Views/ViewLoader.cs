// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Composition.Convention;
using System.Composition.Hosting;
using System.Reflection;
using System.Runtime.Loader;
using EliteSharp.Abstractions.Views;
using Microsoft.Extensions.Logging;
using Useful.Assets;

namespace EliteSharpLib.Views;

/// <summary>
/// Finds the view packs in the plugin folder, and picks the one for the tier
/// the commander configured. Everything MEF touches happens in here and is
/// finished with by the time the loader returns, the same as
/// <see cref="Missions.MissionLoader"/>: it hands back a pack, which is then
/// registered like anything else.
/// <para>
/// Unlike a mission, a pack is not optional. A missing Missions folder costs
/// the commander some missions; a missing Views folder leaves the game with
/// nothing to draw with at all, so this fails at startup and names the tier
/// rather than starting a game that cannot show itself.
/// </para>
/// </summary>
internal static class ViewLoader
{
    /// <summary>
    /// The folder plugin assemblies are dropped into, beside the executable.
    /// </summary>
    internal const string FolderName = "Views";

    /// <summary>
    /// Packs are exported by convention rather than by attribute, so that a
    /// plugin references the contracts assembly and nothing else - a pack is a
    /// public class implementing <see cref="IViewPack"/> with a constructor
    /// taking no arguments, and says nothing about MEF.
    /// </summary>
    private static readonly ConventionBuilder s_conventions = BuildConventions();

    /// <summary>
    /// Loads the view pack for one tier.
    /// </summary>
    /// <param name="baseDirectory">
    /// The folder the plugin folder sits in - the executable's, in the game,
    /// and a temporary one in tests.
    /// </param>
    /// <param name="tier">The tier the commander configured.</param>
    /// <param name="logger">Where skipped files and the count found are reported.</param>
    /// <returns>The pack that draws <paramref name="tier"/>.</returns>
    /// <exception cref="InvalidOperationException">
    /// Nothing in the folder draws this tier, which the game cannot start
    /// without.
    /// </exception>
    public static IViewPack LoadFrom(string baseDirectory, SystemTier tier, ILogger logger)
    {
        string folder = Path.Combine(baseDirectory, FolderName);
        List<Assembly> assemblies = [];

        if (Directory.Exists(folder))
        {
            foreach (string file in Directory.EnumerateFiles(folder, "*.dll"))
            {
                // One unreadable file is one tier the commander cannot play,
                // which is only fatal if it was the tier they asked for - so
                // the decision is left to the search below.
                try
                {
                    assemblies.Add(AssemblyLoadContext.Default.LoadFromAssemblyPath(Path.GetFullPath(file)));
                }
                catch (Exception ex) when (ex is BadImageFormatException or FileLoadException or IOException)
                {
                    LogMessages.ViewAssemblyUnreadable(logger, file, ex);
                }
            }
        }

        IViewPack[] packs = [];

        if (assemblies.Count > 0)
        {
            using CompositionHost host = new ContainerConfiguration()
                .WithAssemblies(assemblies, s_conventions)
                .CreateContainer();

            packs = [.. host.GetExports<IViewPack>()];
        }

        LogMessages.ViewPacksLoaded(logger, packs.Length, assemblies.Count, tier);

        return Array.Find(packs, pack => pack.Tier == tier)
            ?? throw new InvalidOperationException(
                $"No view pack in '{folder}' draws the {tier} tier, so there is nothing to draw the game with.");
    }

    private static ConventionBuilder BuildConventions()
    {
        ConventionBuilder conventions = new();
        conventions.ForTypesDerivedFrom<IViewPack>().Export<IViewPack>();

        return conventions;
    }
}
