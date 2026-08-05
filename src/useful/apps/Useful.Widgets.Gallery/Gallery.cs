// 'Useful Libraries' - Andy Hawkins 2023-2026.

using Useful.Controls;
using Useful.Graphics;

namespace Useful.Widgets.Gallery;

/// <summary>
/// Every widget, on one screen, with each one's bounds outlined. The outline
/// is the point: alignment and state are claims about where a thing sits
/// inside its own box, and without the box drawn there is nothing to judge
/// them against.
/// <para>
/// The layout is in rows, and a row is however tall the font actually is -
/// measured, not assumed. The two backends do not share a font: the software
/// one draws from an 8x8 bitmap sheet, the hardware one from a 12pt true-type
/// face that is around twice as tall, and a pitch hardcoded to either would
/// pile the rows on top of each other in the other. The same measurement
/// decides whether text snaps to a character cell, since only the fixed-cell
/// font has a grid to snap to.
/// </para>
/// </summary>
internal sealed class Gallery
{
    /// <summary>
    /// How many rows the gallery lays out in, which is what the canvas has to
    /// be tall enough for. Named here rather than where the window is sized
    /// because it is a fact about this layout, not about the window.
    /// </summary>
    internal const int LayoutRows = 32;

    private const string Font = "Small";

    // The horizontal grid stays in 8s whatever the font: they are the
    // gallery's own margins, not the font's.
    private const int Cell = 8;
    private const int BoxLeft = 9 * Cell;
    private const int BoxWidth = 22 * Cell;
    private const int ComboLeft = 4 * Cell;
    private const int ComboWidth = 32 * Cell;
    private const int ScreenColumns = 40;

    // Tall and deep, so the measurement is of the line rather than of one
    // short glyph.
    private const string MeasuringText = "Ag";

    private static readonly FastColor s_white = new(0xFFFFFFFF);
    private static readonly FastColor s_grey = new(0xFF909090);
    private static readonly FastColor s_red = new(0xFFAA0000);
    private static readonly FastColor s_yellow = new(0xFFFFFF00);
    private static readonly FastColor s_outline = new(0xFF303030);

    private readonly IGraphics _graphics;
    private readonly WidgetStyle _style;
    private readonly float _pitch;
    private readonly float _snap;
    private readonly Container<Label> _alignment = new() { ChildAlignment = TextAlignment.Left };
    private readonly Container<Label> _states = new() { ChildAlignment = TextAlignment.Left };
    private readonly Container<ComboBox> _combos = new() { ChildAlignment = TextAlignment.Left };
    private readonly Container<Label> _centred = new() { ChildAlignment = TextAlignment.Centre };

    private int _focus;

    internal Gallery(IGraphics graphics)
    {
        _graphics = graphics;

        // Measured, then held to what the canvas can actually show: the window
        // was sized from an estimate of the font's height, and an estimate a
        // pixel short would otherwise push the last rows off the bottom.
        float lineHeight = MathF.Max(Cell, graphics.MeasureText(MeasuringText, Font).Y);
        _pitch = MathF.Min(lineHeight, graphics.ScreenHeight / LayoutRows);

        // A proportional font has no grid to sit on, so nothing snaps to one:
        // a line taller than the cell is how this tells the two apart, which
        // is the same distinction the 8-bit and 16-bit tiers draw.
        _snap = _pitch > Cell ? 0 : Cell;

        // Every state is given a look, so that every state is visible: the
        // selected ones carry a block, the disabled ones grey text.
        _style = new(
            Font,
            WidgetColors.TextOnly(s_white),
            new(s_red, s_white),
            WidgetColors.TextOnly(s_grey),
            new(s_red, s_grey));

        // The demonstration rows are spaced a blank row apart so each one's
        // outline reads as its own box. The combo rows are not: a settings
        // list is a list, and that is how one looks.
        _alignment.Spacing = 2 * _pitch;
        _states.Spacing = 2 * _pitch;
        _centred.Spacing = 2 * _pitch;
        _combos.Spacing = _pitch;

        BuildAlignment();
        BuildStates();
        BuildCombos();
        BuildCentred();
    }

    /// <summary>
    /// Up and Down move between the combo boxes; Left and Right cycle the one
    /// the cursor is on, which is the widget's own input handling rather than
    /// the gallery's.
    /// <para>
    /// S and X do what Up and Down do, as they do throughout Elite: the arrow
    /// keys are extended-key codes, which the screenshot harness cannot post
    /// to an SDL window, so a gallery reachable only by arrows could not be
    /// driven by the thing that takes pictures of it.
    /// </para>
    /// </summary>
    /// <param name="keyboard">The keys pressed this tick.</param>
    internal void HandleInput(IKeyboard keyboard)
    {
        if ((keyboard.IsPressed(ConsoleKey.UpArrow) || keyboard.IsPressed(ConsoleKey.S)) && _focus > 0)
        {
            _focus--;
        }

        if ((keyboard.IsPressed(ConsoleKey.DownArrow) || keyboard.IsPressed(ConsoleKey.X))
            && _focus < _combos.Children.Count - 1)
        {
            _focus++;
        }

        for (int i = 0; i < _combos.Children.Count; i++)
        {
            _combos.Children[i].State = i == _focus ? WidgetState.Selected : WidgetState.Normal;
        }

        _combos.Children[_focus].HandleInput(keyboard);
    }

    /// <summary>
    /// Draws the whole gallery, top to bottom. The vertical position is a
    /// running total of rows rather than a set of fixed numbers, so a taller
    /// font moves everything below it down instead of overlapping it.
    /// </summary>
    internal void Draw()
    {
        float y = 0;

        y = DrawHeading(y, "WIDGET GALLERY", s_yellow);

        y = DrawHeading(y, "LABEL ALIGNMENT", s_yellow);
        y = DrawSection(y, _alignment, BoxLeft, BoxWidth);

        y = DrawHeading(y, "LABEL STATES", s_yellow);
        y = DrawSection(y, _states, BoxLeft, BoxWidth);

        y = DrawHeading(y, "COMBOBOX", s_yellow);
        y = DrawSection(y, _combos, ComboLeft, ComboWidth);

        y = DrawHeading(y, "CONTAINER - CHILDREN CENTRED", s_yellow);
        y = DrawSection(y, _centred, BoxLeft, BoxWidth);

        y = DrawHeading(y, "Up/Down or S/X select a row", s_white);
        y = DrawHeading(y, "Left/Right or , / . cycle it", s_white);
        DrawHeading(y, "Esc to quit", s_white);
    }

    // A value draws over the row's own block, so it never paints one itself.
    private static WidgetStyle ValueStyle()
    {
        WidgetColors text = WidgetColors.TextOnly(s_white);
        return new(Font, text, text, WidgetColors.TextOnly(s_grey), WidgetColors.TextOnly(s_grey));
    }

    // The section headings are chrome rather than widgets - each wants its own
    // colour, which a shared style does not give it - so they are drawn
    // straight onto the surface. Returns the next free row.
    private float DrawHeading(float y, string text, in FastColor colour)
    {
        float width = _graphics.MeasureText(text, Font).X;
        float left = (ScreenColumns * Cell / 2f) - (width / 2);

        if (_snap > 0)
        {
            left = MathF.Floor(left / _snap) * _snap;
        }

        _graphics.DrawTextLeft(new(left, y), text, Font, colour);

        return y + _pitch;
    }

    private float DrawSection<TWidget>(float y, Container<TWidget> container, float left, float width)
        where TWidget : IWidget
    {
        container.Position = new(left, y);
        container.Width = width;
        container.Draw();
        Outline(container);

        // Every child but the last is followed by its spacing, and the last
        // occupies a row of its own.
        return y + ((container.Children.Count - 1) * container.Spacing) + (2 * _pitch);
    }

    // Every widget's bounds, so that what a label did with its width can be
    // seen rather than taken on trust.
    private void Outline<TWidget>(Container<TWidget> container)
        where TWidget : IWidget
    {
        foreach (TWidget child in container.Children)
        {
            _graphics.DrawRectangle(child.Position, child.Width, child.Height, s_outline);
        }
    }

    private void BuildAlignment()
    {
        foreach (TextAlignment alignment in Enum.GetValues<TextAlignment>())
        {
            _alignment.Add(new Label(_graphics, _style)
            {
                Text = alignment.ToString(),
                Alignment = alignment,
                Width = BoxWidth,
                Height = _pitch,
                SnapToCell = _snap,
            });
        }
    }

    private void BuildStates()
    {
        foreach (WidgetState state in Enum.GetValues<WidgetState>())
        {
            _states.Add(new Label(_graphics, _style)
            {
                Text = state.ToString(),
                Alignment = TextAlignment.Centre,
                State = state,
                Width = BoxWidth,
                Height = _pitch,
                SnapToCell = _snap,
            });
        }
    }

    private void BuildCombos()
    {
        // The last has one value, which is how a row with nothing to cycle to
        // shows: selected, but with no arrows offered.
        MemorySetting[] settings =
        [
            new("Graphic Style:", "Wireframe", "Solid"),
            new("Depth Sort:", "Painter", "ZBuffer"),
            new("Rendition *:", "8-bit"),
        ];

        foreach (MemorySetting setting in settings)
        {
            _combos.Add(new ComboBox(_graphics, _style, ValueStyle(), setting)
            {
                Width = ComboWidth,
                Height = _pitch,
                ValueOffsetX = 17 * Cell,
                ArrowGap = Cell,
                SnapToCell = _snap,
            });
        }

        _combos.Children[0].State = WidgetState.Selected;
    }

    private void BuildCentred()
    {
        foreach (string text in (string[])["Narrow", "A bit wider", "Wider again still"])
        {
            _centred.Add(new Label(_graphics, _style)
            {
                Text = text,
                Alignment = TextAlignment.Left,
                Width = _graphics.MeasureText(text, Font).X,
                Height = _pitch,
            });
        }
    }
}
