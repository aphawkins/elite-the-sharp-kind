// 'Useful Libraries' - Andy Hawkins 2023-2026.

using System.Numerics;
using Useful.Graphics;

namespace Useful.Widgets;

/// <summary>
/// A box with text in it. The label owns its bounds and draws everything
/// relative to them - the background fills them, and the text is aligned
/// inside them - so a caller positions the label and never positions its
/// contents. Nothing here consults the screen.
/// </summary>
public sealed class Label : IWidget
{
    private readonly IGraphics _graphics;
    private readonly WidgetStyle _style;

    /// <summary>
    /// Initializes a new instance of the <see cref="Label"/> class.
    /// </summary>
    /// <param name="graphics">The surface to draw on.</param>
    /// <param name="style">The screen's font and colours.</param>
    public Label(IGraphics graphics, WidgetStyle style)
    {
        ArgumentNullException.ThrowIfNull(graphics);
        ArgumentNullException.ThrowIfNull(style);

        _graphics = graphics;
        _style = style;
    }

    /// <summary>
    /// Gets or sets the label's top left corner.
    /// </summary>
    public Vector2 Position { get; set; }

    /// <summary>
    /// Gets or sets the label's width. The background fills it and the text
    /// aligns within it.
    /// </summary>
    public float Width { get; set; }

    /// <summary>
    /// Gets or sets the label's height.
    /// </summary>
    public float Height { get; set; }

    /// <summary>
    /// Gets or sets the text to draw.
    /// </summary>
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets where the text sits within the label.
    /// </summary>
    public TextAlignment Alignment { get; set; }

    /// <summary>
    /// Gets or sets the character cell the aligned text is rounded to, or
    /// zero for none. A fixed-cell tier sets this so that centring an
    /// odd-length string cannot land it on half a cell, which is what
    /// pixel-centring does on a monospaced grid.
    /// </summary>
    public float SnapToCell { get; set; }

    /// <summary>
    /// Gets or sets which of the label's looks to draw.
    /// </summary>
    public WidgetState State { get; set; }

    /// <summary>
    /// Draws the label at its current position, in its current state.
    /// </summary>
    public void Draw()
    {
        WidgetColors colors = _style.Colors(State);

        if (colors.HasBackground)
        {
            _graphics.DrawRectangleFilled(Position, Width, Height, colors.Background);
        }

        _graphics.DrawTextLeft(new(TextLeft(), Position.Y), Text, _style.FontType, colors.Text);
    }

    // Alignment is resolved here rather than leaning on DrawTextRight or
    // DrawTextCentre: those align against a point and the screen respectively,
    // and what is wanted is alignment against this label's own box.
    private float TextLeft()
    {
        if (Alignment == TextAlignment.Left)
        {
            return Position.X;
        }

        float textWidth = _graphics.MeasureText(Text, _style.FontType).X;

        // Odd-width text cannot sit exactly in the middle of an even-width
        // box, so the leftover half pixel goes to the left of it. Rounded up
        // rather than down because that is the side the surfaces' own text
        // centring has always given it, and a text renderer floors to whole
        // pixels afterwards either way.
        float offset = Alignment == TextAlignment.Right
            ? Width - textWidth
            : MathF.Ceiling((Width - textWidth) / 2);

        float left = Position.X + offset;

        // Snapped as an absolute position, not as an offset within the label:
        // a label is not necessarily on a cell boundary itself - a 25-cell bar
        // centred in a 40-cell screen starts on half a cell - and it is the
        // glyphs that have to land on the grid, not the gap in front of them.
        // Truncated rather than rounded to nearest, matching the integer
        // division a fixed-cell tier centres whole character counts with.
        if (SnapToCell > 0)
        {
            left = MathF.Floor(left / SnapToCell) * SnapToCell;
        }

        return left;
    }
}
