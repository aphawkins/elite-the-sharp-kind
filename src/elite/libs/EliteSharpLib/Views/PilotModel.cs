// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Lasers;

namespace EliteSharpLib.Views;

/// <summary>
/// One cockpit window: its name, the hyperspace countdown text (empty when
/// there is none to show), and this direction's laser mount. The starfield
/// and the ship ahead are drawn by the universe, not through this model.
/// </summary>
internal sealed record PilotModel(string ViewName, string HyperspaceStatus, LaserType LaserType, bool IsFiring);
