// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;

namespace EliteSharp.Abstractions.Views.Stars;

/// <summary>
/// One star, as the game has decided it should appear this frame. Whether a
/// star is drawn at all is the game's - it is the one moving them, recycling
/// them off the edges and deciding when a streak is worth having - and how it
/// then looks is the rendition's.
/// </summary>
/// <param name="Position">Where it is on screen.</param>
/// <param name="StreakTo">
/// Where it has just come from, when it is streaking. Meaningless otherwise.
/// </param>
/// <param name="IsStreaking">
/// Whether the ship is moving fast enough to smear this star into a line.
/// </param>
/// <param name="Distance">
/// How far away it is, in the original's terms. Nearer stars were drawn
/// fatter on the machines this stands in for, but where a rendition puts that
/// threshold - or whether it does it at all - is its own business.
/// </param>
public readonly record struct StarMark(Vector2 Position, Vector2 StreakTo, bool IsStreaking, float Distance);
