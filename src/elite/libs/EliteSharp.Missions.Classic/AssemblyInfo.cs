// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Runtime.CompilerServices;

// The stage and mission names are this plugin's own business - the game reads
// them off IMission rather than naming them - so they stay internal, and the
// tests that check the missions behave are let in to use them.
[assembly: InternalsVisibleTo("EliteSharpLib.Tests")]
