// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;

namespace EliteSharpLib.Views;

/// <summary>
/// The one screen every mission's messages are drawn on. A ship posing behind a
/// briefing is drawn by the universe rather than by the view, but where it sits
/// is a layout decision, so each asset tier chooses its own.
/// </summary>
internal interface IMissionBriefingView : IView<MissionBriefingModel>
{
    /// <summary>
    /// Gets where the controller places a briefing's ship.
    /// </summary>
    public Vector4 ShipLocation { get; }
}
