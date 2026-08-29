// 'SharpKind Libraries' - Andy Hawkins 2023-2026.

namespace SharpKind.UI;

/// <summary>
/// Where one cell of a <see cref="TableRow"/> sits and which way it reads. The
/// offset is from the row's own left edge, so a whole table moves by moving its
/// rows and the columns keep their shape.
/// <para>
/// A cell is anchored at a point rather than laid out in a box, which is what
/// separates this from a <see cref="Label"/>: a right-aligned number ends at
/// its column and a left-aligned name starts at it, and neither needs to know
/// how wide the column is. That is what lets a units suffix share a column with
/// the quantity it follows.
/// </para>
/// </summary>
/// <param name="OffsetX">The column's distance from the row's left edge.</param>
/// <param name="Alignment">
/// Which end of the text sits on the column. <see cref="TextAlignment.Centre"/>
/// centres it on the column.
/// </param>
public readonly record struct TableColumn(float OffsetX, TextAlignment Alignment);
