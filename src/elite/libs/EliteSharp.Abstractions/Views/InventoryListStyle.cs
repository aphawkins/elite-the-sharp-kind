// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using SharpKind.UI;

namespace EliteSharp.Abstractions.Views;

/// <summary>
/// What the inventory screen looks like in one rendition: the two figures at
/// the top, the columns a cargo row is laid out in, and how many rows there is
/// room for. The same division <see cref="MarketListStyle"/> uses, and for the
/// same reason - the goods are a plugin, so what a commander can be carrying is
/// not something a rendition can be written against.
/// <para>
/// The cargo list carries no cursor: nothing on this screen can be chosen. It
/// still scrolls, because a hold can hold more sorts of thing than the screen
/// has rows for.
/// </para>
/// </summary>
/// <param name="RowStyle">
/// A cargo row's font and colour. Its selected look is never drawn - the list
/// shows no cursor - so only the normal one matters.
/// </param>
/// <param name="CaptionStyle">The "Fuel:" and "Cash:" captions' font and colour.</param>
/// <param name="Columns">
/// Where a cargo row's two cells sit, offset from the row's left edge: what is
/// being carried, and how much of it.
/// </param>
/// <param name="FuelCaption">The "Fuel:" caption, wherever this tier puts it.</param>
/// <param name="FuelValue">
/// Where the range goes. Its text is the game's, so only the position and the
/// alignment are the rendition's.
/// </param>
/// <param name="CashCaption">The "Cash:" caption.</param>
/// <param name="CashValue">Where the amount goes.</param>
/// <param name="RowsLeft">The left edge of the cargo rows.</param>
/// <param name="FirstRowY">The top of the first cargo row.</param>
/// <param name="RowHeight">The height of a row, and the pitch between them.</param>
/// <param name="RowWidth">The width of a row.</param>
/// <param name="VisibleRows">
/// How many cargo rows this tier has room for above the HUD. The list scrolls
/// when the hold holds more sorts of thing than that.
/// </param>
public sealed record InventoryListStyle(
    ControlStyle RowStyle,
    ControlStyle CaptionStyle,
    IReadOnlyList<TableColumn> Columns,
    PlacedText FuelCaption,
    PlacedText FuelValue,
    PlacedText CashCaption,
    PlacedText CashValue,
    float RowsLeft,
    float FirstRowY,
    float RowHeight,
    float RowWidth,
    int VisibleRows);
