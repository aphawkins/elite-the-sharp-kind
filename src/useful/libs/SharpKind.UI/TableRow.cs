// 'SharpKind Libraries' - Andy Hawkins 2023-2026.

using SharpKind.Graphics;

namespace SharpKind.UI;

/// <summary>
/// One row of a table: several cells at fixed columns across the row's width,
/// sharing one background block. The selected block fills the whole row and the
/// cells draw over it, so a row is one thing to highlight rather than several
/// that have to agree.
/// <para>
/// The columns are fixed when the row is built - they are the table's shape,
/// which does not change while it is on screen - and the text comes from
/// <see cref="IRowCells"/> at every draw. A row therefore holds no copy of what
/// it shows.
/// </para>
/// </summary>
public sealed class TableRow : UIControl
{
    private readonly IReadOnlyList<TableColumn> _columns;
    private readonly IRowCells _cells;

    /// <summary>
    /// Initializes a new instance of the <see cref="TableRow"/> class.
    /// </summary>
    /// <param name="graphics">The surface to draw on.</param>
    /// <param name="style">The row's font and colours.</param>
    /// <param name="columns">Where each cell sits, in column order.</param>
    /// <param name="cells">The binding the cells are read from.</param>
    public TableRow(IGraphics graphics, ControlStyle style, IReadOnlyList<TableColumn> columns, IRowCells cells)
        : base(graphics, style, ISetting.None)
    {
        ArgumentNullException.ThrowIfNull(columns);
        ArgumentNullException.ThrowIfNull(cells);

        _columns = columns;
        _cells = cells;
    }

    /// <summary>
    /// Draws the row's block and then its cells, in its current state.
    /// </summary>
    public override void Draw()
    {
        ControlColors colors = Colors;

        if (colors.HasBackground)
        {
            Graphics.DrawRectangleFilled(Position, Width, Height, colors.Background);
        }

        IReadOnlyList<string> cells = _cells.Cells;
        int count = Math.Min(_columns.Count, cells.Count);

        for (int i = 0; i < count; i++)
        {
            DrawCell(_columns[i], cells[i], colors.Text);
        }
    }

    private void DrawCell(in TableColumn column, string text, in FastColor color)
    {
        if (text.Length == 0)
        {
            return;
        }

        float x = Position.X + column.OffsetX;

        switch (column.Alignment)
        {
            case TextAlignment.Right:
                Graphics.DrawTextRight(new(x, Position.Y), text, Style.FontType, color);
                break;

            case TextAlignment.Centre:
                float half = Graphics.MeasureText(text, Style.FontType).X / 2;
                Graphics.DrawTextLeft(new(x - half, Position.Y), text, Style.FontType, color);
                break;

            default:
                Graphics.DrawTextLeft(new(x, Position.Y), text, Style.FontType, color);
                break;
        }
    }
}
