// 'Useful Libraries' - Andy Hawkins 2023-2026.

using Useful.Graphics;

namespace Useful.UI;

/// <summary>
/// A box with text in it. The label owns its bounds and draws everything
/// relative to them - the background fills them, and the text is aligned
/// inside them - so a caller positions the label and never positions its
/// contents. Nothing here consults the screen.
/// <para>
/// The text is read from the binding every time the label draws, so a label
/// showing something that changes needs nothing done to it: the binding is
/// what changes. A caption that never moves is the same thing bound to a
/// setting that always answers the same.
/// </para>
/// </summary>
/// <param name="graphics">The surface to draw on.</param>
/// <param name="style">The screen's font and colours.</param>
/// <param name="setting">The binding whose name this label shows.</param>
public sealed class Label(IGraphics graphics, ControlStyle style, ISetting setting)
    : UIControl(graphics, style, setting)
{
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
    /// Gets the text this label draws.
    /// </summary>
    public string Text => Setting.Name;

    /// <summary>
    /// Draws the label at its current position, in its current state.
    /// </summary>
    public override void Draw()
    {
        ControlColors colors = Colors;

        if (colors.HasBackground)
        {
            Graphics.DrawRectangleFilled(Position, Width, Height, colors.Background);
        }

        Graphics.DrawTextLeft(new(TextLeft(), Position.Y), Text, Style.FontType, colors.Text);
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

        float textWidth = Graphics.MeasureText(Text, Style.FontType).X;

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
