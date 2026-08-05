// 'Useful Libraries' - Andy Hawkins 2023-2026.

using System.Numerics;
using Useful.Controls;
using Useful.Graphics;

namespace Useful.Widgets;

/// <summary>
/// A named setting and the value it is currently on, with arrows either side
/// of the value while the cursor is on the row to show that there are others
/// to cycle to. There is no drop-down: this is a keyboard menu, so the
/// alternatives are stepped through in place rather than listed.
/// <para>
/// The box holds no value of its own. Everything it shows is read from the
/// <see cref="ISetting"/> it is bound to at the moment it draws, and cycling
/// writes straight back through it - so the widget owns the interaction while
/// the binding owns the truth. What it does hold is how it draws: its bounds,
/// its colours and the labels it draws with.
/// </para>
/// </summary>
public sealed class ComboBox : IWidget
{
    private readonly IGraphics _graphics;
    private readonly WidgetStyle _style;
    private readonly Label _name;
    private readonly Label _value;
    private readonly Label _openArrow;
    private readonly Label _closeArrow;

    /// <summary>
    /// Initializes a new instance of the <see cref="ComboBox"/> class.
    /// </summary>
    /// <param name="graphics">The surface to draw on.</param>
    /// <param name="style">The row's font and colours.</param>
    /// <param name="valueStyle">
    /// The value's and arrows' colours. Their backgrounds should be
    /// transparent, since the name's label has already filled the row.
    /// </param>
    /// <param name="setting">The setting this row shows and cycles.</param>
    public ComboBox(IGraphics graphics, WidgetStyle style, WidgetStyle valueStyle, ISetting setting)
    {
        ArgumentNullException.ThrowIfNull(graphics);
        ArgumentNullException.ThrowIfNull(valueStyle);
        ArgumentNullException.ThrowIfNull(setting);

        _graphics = graphics;
        _style = valueStyle;
        Setting = setting;

        _name = new(graphics, style);
        _value = new(graphics, valueStyle);

        // Zero-width boxes: the opening arrow is right-aligned, so it ends at
        // its position, and the closing one left-aligned, so it starts at its
        // position. That lets both be placed against the value itself rather
        // than at fixed columns, which would strand them when the value is
        // short.
        _openArrow = new(graphics, valueStyle) { Text = "<", Alignment = TextAlignment.Right };
        _closeArrow = new(graphics, valueStyle) { Text = ">", Alignment = TextAlignment.Left };
    }

    /// <summary>
    /// Gets the setting this row is bound to.
    /// </summary>
    public ISetting Setting { get; }

    /// <summary>
    /// Gets or sets the row's top left corner.
    /// </summary>
    public Vector2 Position { get; set; }

    /// <summary>
    /// Gets or sets the row's width, which its background fills.
    /// </summary>
    public float Width { get; set; }

    /// <summary>
    /// Gets or sets the row's height.
    /// </summary>
    public float Height { get; set; }

    /// <summary>
    /// Gets or sets which of the row's looks to draw. The arrows show only in
    /// the selected states.
    /// </summary>
    public WidgetState State { get; set; }

    /// <summary>
    /// Gets or sets the value's distance from the row's left edge.
    /// </summary>
    public float ValueOffsetX { get; set; }

    /// <summary>
    /// Gets or sets the space between an arrow and the value.
    /// </summary>
    public float ArrowGap { get; set; }

    /// <summary>
    /// Gets or sets the character cell the value's arrows are rounded to, or
    /// zero for none.
    /// </summary>
    public float SnapToCell { get; set; }

    /// <summary>
    /// Gets the value currently selected, or empty if there are none.
    /// </summary>
    public string Value => Setting.SelectedIndex >= 0 && Setting.SelectedIndex < Setting.Values.Count
        ? Setting.Values[Setting.SelectedIndex]
        : string.Empty;

    /// <summary>
    /// Applies the keys this row answers to while the cursor is on it: Left
    /// and Right - or comma and full stop, which the keyboard delivers where
    /// the arrow keys cannot reach - step between values, and Enter steps
    /// forwards.
    /// </summary>
    /// <param name="keyboard">The keys pressed this tick.</param>
    public void HandleInput(IKeyboard keyboard)
    {
        ArgumentNullException.ThrowIfNull(keyboard);

        if (keyboard.IsPressed(ConsoleKey.RightArrow)
            || keyboard.IsPressed(ConsoleKey.OemPeriod)
            || keyboard.IsPressed(ConsoleKey.Enter))
        {
            Next();
        }

        if (keyboard.IsPressed(ConsoleKey.LeftArrow) || keyboard.IsPressed(ConsoleKey.OemComma))
        {
            Previous();
        }
    }

    /// <summary>
    /// Moves to the next value, wrapping back to the first past the end.
    /// </summary>
    public void Next()
    {
        if (Setting.Values.Count > 0)
        {
            Setting.SelectedIndex = (Setting.SelectedIndex + 1) % Setting.Values.Count;
        }
    }

    /// <summary>
    /// Moves to the previous value, wrapping to the last before the first.
    /// </summary>
    public void Previous()
    {
        if (Setting.Values.Count > 0)
        {
            Setting.SelectedIndex = (Setting.SelectedIndex - 1 + Setting.Values.Count) % Setting.Values.Count;
        }
    }

    /// <summary>
    /// Draws the row at its current position, in its current state.
    /// </summary>
    public void Draw()
    {
        _name.Position = Position;
        _name.Width = Width;
        _name.Height = Height;
        _name.Text = Setting.Name;
        _name.State = State;
        _name.Draw();

        _value.Position = new(Position.X + ValueOffsetX, Position.Y);
        _value.Width = Width - ValueOffsetX;
        _value.Height = Height;
        _value.Text = Value;
        _value.State = State;
        _value.Draw();

        if (State is WidgetState.Selected or WidgetState.SelectedDisabled)
        {
            DrawArrows();
        }
    }

    // Only worth showing when there is somewhere else to go: a setting with a
    // single value is not something the player can cycle.
    private void DrawArrows()
    {
        if (Setting.Values.Count < 2)
        {
            return;
        }

        float valueLeft = Position.X + ValueOffsetX;
        float valueWidth = _graphics.MeasureText(Value, _style.FontType).X;

        _openArrow.Position = new(valueLeft - ArrowGap, Position.Y);
        _openArrow.Height = Height;
        _openArrow.State = State;
        _openArrow.SnapToCell = SnapToCell;
        _openArrow.Draw();

        _closeArrow.Position = new(valueLeft + valueWidth + ArrowGap, Position.Y);
        _closeArrow.Height = Height;
        _closeArrow.State = State;
        _closeArrow.SnapToCell = SnapToCell;
        _closeArrow.Draw();
    }
}
