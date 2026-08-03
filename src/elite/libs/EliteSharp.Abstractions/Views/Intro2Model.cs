// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

namespace EliteSharp.Abstractions.Views;

/// <summary>
/// The ship parade screen: the fixed prompt, and the name of whichever ship
/// is currently on show. <paramref name="ShipName"/> is empty before the
/// first ship is added, which the view takes as "don't draw it".
/// </summary>
public sealed record Intro2Model(string Prompt, string ShipName);
