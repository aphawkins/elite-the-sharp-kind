// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using Useful;

namespace EliteSharp.Abstractions.Views.Planets;

/// <summary>
/// What the game is asking for when it wants a planet drawn. A rendition
/// makes a renderer from this and keeps whatever it needs; the game keeps the
/// planet itself.
/// </summary>
/// <param name="Style">Which of the styles to draw.</param>
/// <param name="HasCrater">
/// For the outlined style only: whether this world shows a crater rather than
/// an equator and meridian. The original picks it from a bit of the system's
/// tech level, so it is the game's to decide.
/// </param>
/// <param name="Random">
/// The stream a generated surface is built from. The game seeds it per system,
/// so the same world looks the same every visit - a rendition rolling its own
/// would lose that.
/// </param>
public sealed record PlanetLook(PlanetStyle Style, bool HasCrater, IRandomSource Random);
