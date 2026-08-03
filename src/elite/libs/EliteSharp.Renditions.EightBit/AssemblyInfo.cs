// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Runtime.CompilerServices;

// The views are this rendition's own business - the game only ever sees them as
// IView, off the rendition - so they stay internal, and the tests that check what
// this tier draws are let in to use them.
[assembly: InternalsVisibleTo("EliteSharpLib.Tests")]
