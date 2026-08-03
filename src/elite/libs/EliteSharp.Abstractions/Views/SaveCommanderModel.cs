// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

namespace EliteSharp.Abstractions.Views;

/// <summary>
/// The save-commander screen: the name typed so far, and the outcome of the
/// last save attempt. <paramref name="StatusMessage"/> is empty before any
/// attempt has been made, which the view takes as "don't draw it" rather
/// than carrying a separate visibility flag; success and failure differ only
/// in which fixed text this carries; both draw the same way.
/// </summary>
public sealed record SaveCommanderModel(string Name, string StatusMessage);
