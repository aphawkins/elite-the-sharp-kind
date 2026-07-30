// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

namespace EliteSharpLib.Views;

/// <summary>
/// The escape capsule launch: its alert, and whether the alert is still up.
/// The controller decides when it lapses, so the view has no counter to
/// consult and both tiers agree on the timing for free.
/// </summary>
internal sealed record EscapeCapsuleModel(string Alert, bool IsAlertVisible);
