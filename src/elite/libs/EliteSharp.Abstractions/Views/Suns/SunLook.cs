// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using Useful;

namespace EliteSharp.Abstractions.Views.Suns;

/// <summary>
/// What the game is asking for when it wants a sun drawn.
/// </summary>
/// <param name="Style">Which of the styles to draw.</param>
/// <param name="Random">
/// The stream the flaring rim shimmers from. It is the game's one source of
/// entropy, handed over rather than replaced: a renderer rolling its own would
/// put drawing outside the seeded stream and stop a frame being reproducible.
/// </param>
public sealed record SunLook(SunStyle Style, IRandomSource Random);
