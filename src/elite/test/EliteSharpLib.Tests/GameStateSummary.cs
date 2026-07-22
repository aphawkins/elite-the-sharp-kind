// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Views;

namespace EliteSharpLib.Tests;

// A minimal, image-free snapshot of the running game, so most harness
// assertions never need to inspect a rendered frame.
internal readonly record struct GameStateSummary(
    Screen Screen,
    bool IsDocked,
    bool IsGameOver);
