// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;

namespace EliteSharp.Abstractions.Views;

/// <summary>
/// Where the compass points: a unit direction, and whether the target is
/// behind the ship, which is drawn differently.
/// </summary>
/// <param name="Direction">
/// The target's unit vector, x and y only - the view scales it by its own
/// compass radius.
/// </param>
/// <param name="IsBehind">Whether the target is behind the ship.</param>
public readonly record struct CompassReading(Vector2 Direction, bool IsBehind);
