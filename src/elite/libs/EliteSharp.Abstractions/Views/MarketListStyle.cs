// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using SharpKind.UI;

namespace EliteSharp.Abstractions.Views;

/// <summary>
/// What the market screen looks like in one rendition: every colour, every
/// column and every caption it needs, and nothing else. The game builds the
/// list and binds it to the goods it is trading; the rendition says only how it
/// is to look, which is the division <see cref="SettingsListStyle"/> already
/// uses.
/// <para>
/// This is why there is no market view per rendition any more. The goods are a
/// plugin now, so how many rows there are is not something a rendition can be
/// written against - it has to declare how many it has room for and let the
/// list scroll, and a screen whose whole tier-specific content is a set of
/// columns does not need an assembly's worth of drawing code in each rendition
/// to express them.
/// </para>
/// </summary>
/// <param name="RowStyle">
/// A goods row's font and colours. Its selected background is the cursor block,
/// which fills the whole row.
/// </param>
/// <param name="HeadingStyle">
/// The column headings' font and colour, which the "Cash:" caption also takes -
/// both tiers draw it in the same green as the headings.
/// </param>
/// <param name="Columns">
/// Where each of a row's cells sits, offset from the row's left edge. The game
/// fills them in a fixed order - name, units, price, quantity for sale, the
/// unit that quantity is in, quantity in hold, the unit that is in - so a
/// rendition with no separate unit column simply repeats a column or places one
/// off the row.
/// </param>
/// <param name="Headings">The column captions, wherever this tier puts them.</param>
/// <param name="RowsLeft">The left edge of the goods rows.</param>
/// <param name="FirstRowY">The top of the first goods row.</param>
/// <param name="RowHeight">The height of a row, and the pitch between them.</param>
/// <param name="RowWidth">The width of a row, which its cursor block fills.</param>
/// <param name="VisibleRows">
/// How many rows this tier has room for. The list scrolls when the goods
/// outnumber them, so this is the one number that keeps a large goods set from
/// drawing over the cash line.
/// </param>
/// <param name="CashCaption">The "Cash:" caption, wherever this tier puts it.</param>
/// <param name="CashAmount">
/// Where the amount goes, drawn in <paramref name="RowStyle"/>'s normal colour
/// as the goods' own figures are. Its text is the game's, so only the position
/// and the alignment are the rendition's.
/// </param>
public sealed record MarketListStyle(
    ControlStyle RowStyle,
    ControlStyle HeadingStyle,
    IReadOnlyList<TableColumn> Columns,
    IReadOnlyList<PlacedText> Headings,
    float RowsLeft,
    float FirstRowY,
    float RowHeight,
    float RowWidth,
    int VisibleRows,
    PlacedText CashCaption,
    PlacedText CashAmount);
