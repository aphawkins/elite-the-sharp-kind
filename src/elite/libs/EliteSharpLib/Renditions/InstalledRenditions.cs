// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Abstractions.Renditions;

namespace EliteSharpLib.Renditions;

/// <summary>
/// What the loader found: the rendition the commander configured, and the
/// names of every one installed beside it. The settings screen offers the
/// second, so a commander can only switch to one that is actually there.
/// </summary>
/// <param name="Chosen">The rendition the game will draw itself with.</param>
/// <param name="Names">Every installed rendition's name, in order.</param>
public sealed record InstalledRenditions(IRendition Chosen, IReadOnlyList<string> Names);
