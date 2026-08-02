// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

namespace EliteSharpLib.Save;

/// <summary>
/// The commander's standing with the law. <see cref="Bounty"/> is the value
/// the game works in and the one a load restores; <see cref="Status"/> names
/// the band it falls in so the file reads for itself, and a file whose two
/// disagree is rejected rather than quietly believed.
/// </summary>
public sealed class LegalStatusState
{
    public string Status { get; set; } = string.Empty;

    public int Bounty { get; set; }
}
