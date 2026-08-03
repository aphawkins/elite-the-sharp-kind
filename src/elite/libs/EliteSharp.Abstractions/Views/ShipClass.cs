// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

namespace EliteSharp.Abstractions.Views;

/// <summary>
/// What sort of thing a ship is, as far as colouring it goes. The game decides
/// which one something is; the tier decides what colour that is.
/// </summary>
public enum ShipClass
{
    /// <summary>
    /// Traders, pirates and anything with no other claim.
    /// </summary>
    Default = 0,

    /// <summary>
    /// The space station.
    /// </summary>
    Station = 1,

    /// <summary>
    /// A missile in flight.
    /// </summary>
    Missile = 2,

    /// <summary>
    /// Vipers.
    /// </summary>
    Police = 3,

    /// <summary>
    /// Anything currently attacking.
    /// </summary>
    Hostile = 4,
}
