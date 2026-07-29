// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using Useful.Abstraction.Config;

namespace EliteSharpLib.Config;

// The root of elite.sharp: shared engine settings plus Elite's own.
internal sealed class EliteConfig : ConfigSettings<EliteConfigSettings>;
