// 'SharpKind Libraries' - Andy Hawkins 2023-2026.

namespace SharpKind.UI;

/// <summary>
/// What a <see cref="TableRow"/> is bound to: the text of each of its cells,
/// read afresh every time the row draws. The same discipline
/// <see cref="ISetting"/> imposes on the other controls, for a control whose
/// content is several strings rather than one named value - a row cannot be
/// left holding a price the market has moved off.
/// </summary>
public interface IRowCells
{
    /// <summary>
    /// Gets this row's cells, one per column, in column order. A row draws
    /// however many of the two it has fewer of, so a binding that answers
    /// short leaves the last columns empty rather than failing.
    /// </summary>
    public IReadOnlyList<string> Cells { get; }
}
