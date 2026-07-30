// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

namespace EliteSharpLib.Views;

/// <summary>
/// Which cockpit window a <see cref="PilotController"/> is for. Flight
/// controls, docking and weapons are identical in all four; only the view
/// name, the laser mount drawn and the starfield direction differ.
/// </summary>
internal enum PilotDirection
{
    Front = 0,
    Rear = 1,
    Left = 2,
    Right = 3,
}
