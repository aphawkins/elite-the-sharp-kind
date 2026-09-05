// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Abstractions.Renditions;

namespace EliteSharpLib.Renditions;

/// <summary>
/// What the loader found: the rendition the commander configured, where it
/// was loaded from, and the names of every one installed beside it. The
/// settings screen offers the last of those, so a commander can only switch
/// to one that is actually there.
/// </summary>
/// <param name="Chosen">The rendition the game will draw itself with.</param>
/// <param name="Folder">
/// The directory the chosen rendition was loaded from. A rendition brings its
/// own artwork with it, and this is where the game looks for it.
/// </param>
/// <param name="Installed">
/// Every rendition found, in the order their names are offered. Kept in full
/// - not just by name - so a screen that offers switching rendition can read
/// what one other than <paramref name="Chosen"/> would offer, such as the
/// window scales it supports, without loading it.
/// </param>
public sealed record InstalledRenditions(IRendition Chosen, string Folder, IReadOnlyList<IRendition> Installed)
{
    /// <summary>
    /// Gets every installed rendition's name, in order.
    /// </summary>
    public IReadOnlyList<string> Names => [.. Installed.Select(r => r.Name)];

    /// <summary>
    /// Finds the rendition by the name given, falling back to <see cref="Chosen"/>
    /// when the name is not one that is installed - a hand-edited or
    /// not-yet-saved choice cannot be shown scales for a rendition that isn't
    /// there.
    /// </summary>
    /// <param name="name">The rendition name to look up.</param>
    /// <returns>The rendition by that name, or <see cref="Chosen"/>.</returns>
    public IRendition Find(string name)
        => Installed.FirstOrDefault(r => string.Equals(r.Name, name, StringComparison.Ordinal)) ?? Chosen;
}
