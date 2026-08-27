// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Runtime.CompilerServices;

// The goods table is this plugin's own business - the game reads it off
// IGoodsSet rather than naming any of it - so it stays internal, and the tests
// that check the classic economy is intact are let in to use it.
[assembly: InternalsVisibleTo("EliteSharpLib.Tests")]
