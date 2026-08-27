// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

namespace EliteSharpLib.Trader;

/// <summary>
/// One tradeable good, as the game is configured with it: everything true of
/// the good itself, none of which changes while playing. What does change - the
/// price today, what the station has left, what is in the hold - belongs to
/// <see cref="StockItem"/>.
/// <para>
/// The split is what lets a whole set of goods arrive from outside the game
/// rather than being written into it. Until that lands the classic seventeen
/// are declared in <see cref="ClassicGoods"/>, which is the only thing that
/// then has to move.
/// </para>
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
/// <paramref name="Units"/>: the classic set happens to weigh everything held
/// in tonnes and nothing else, but that is a fact about those goods, not a rule
/// about units, and a set naming its unit differently must not silently start
/// carrying free cargo.
/// </param>
/// <param name="OpeningStationStock">What a new commander's station has on the shelf.</param>
/// <param name="IsSoldByStations">
/// Whether a station ever offers it. Alien Items are the classic set's only no:
/// they are scooped from a dead Thargoid and sold, never bought.
/// </param>
/// <param name="ContrabandWeight">
/// How heavily one unit counts when the police weigh the hold. Zero is legal
/// cargo.
/// </param>
/// <param name="IsDroppedByShips">
/// Whether a cargo canister can hold it. The classic set drops the first eight,
/// contraband included - a canister is nobody's paperwork.
/// </param>
internal sealed record GoodsDefinition(
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
