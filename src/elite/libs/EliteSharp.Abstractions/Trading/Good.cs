// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

namespace EliteSharp.Abstractions.Trading;

/// <summary>
/// One tradeable good, as a goods set declares it: everything true of the good
/// itself, none of which changes while playing. The price today, what the
/// station has left, and what is in the hold are the game's to track, not the
/// set's to state.
/// </summary>
/// <param name="Id">
/// What a save file calls this good. It has to stay put across releases, as a
/// mission's name does: a renamed good is cargo a commander loses.
/// </param>
/// <param name="Name">What the market and inventory screens show.</param>
/// <param name="BasePrice">The price before the economy and the market roll.</param>
/// <param name="EconomyAdjust">
/// How far the planet's economy moves the price and the quantity, and in which
/// direction. Negative means an agricultural world is the dear one.
/// </param>
/// <param name="BaseQuantity">The quantity before the economy and the market roll.</param>
/// <param name="Mask">Which bits of the market randomiser this good responds to.</param>
/// <param name="Units">The suffix the screens print after a quantity.</param>
/// <param name="FillsHold">
/// Whether a unit takes up cargo space. Stated rather than inferred from
/// <paramref name="Units"/>: a set is free to weigh its goods in whatever it
/// likes, and naming a unit must not be what decides whether cargo is free.
/// </param>
/// <param name="OpeningStationStock">What a new commander's station has on the shelf.</param>
/// <param name="IsSoldByStations">
/// Whether a station ever offers it. A good scooped from a wreck and sold but
/// never bought - the classic set's Alien Items - says no.
/// </param>
/// <param name="ContrabandWeight">
/// How heavily one unit counts when the police weigh the hold. Zero is legal
/// cargo.
/// </param>
/// <param name="IsDroppedByShips">
/// Whether a cargo canister can hold it. A canister is nobody's paperwork, so a
/// set may drop contraband this way.
/// </param>
public sealed record Good(
    string Id,
    string Name,
    float BasePrice,
    int EconomyAdjust,
    int BaseQuantity,
    int Mask,
    string Units,
    bool FillsHold,
    int OpeningStationStock,
    bool IsSoldByStations,
    int ContrabandWeight,
    bool IsDroppedByShips);
