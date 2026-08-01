// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;

namespace EliteSharpLib.Views;

/// <summary>
/// The Constrictor mission's view. The Constrictor posing behind the brief is
/// drawn by the universe rather than the view, but where it sits is a layout
/// decision, so each asset tier chooses its own.
/// </summary>
internal interface IConstrictorMissionView : IView<ConstrictorMissionModel>
{
    /// <summary>
    /// Gets where the controller places the Constrictor for the brief.
    /// </summary>
    public Vector4 ShipLocation { get; }
}
