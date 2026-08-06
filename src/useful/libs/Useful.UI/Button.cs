// 'Useful Libraries' - Andy Hawkins 2023-2026.

using Useful.Graphics;
using Useful.Input;

namespace Useful.UI;

/// <summary>
/// A caption that does something when it is pressed. What it does is not here:
/// pressing applies the binding's choice, and the setting's setter is where
/// the consequence lives - the same setter a <see cref="ComboBox"/> writes
/// through when it cycles.
/// <para>
/// So a button is a setting with one value, and pressing it is choosing that
/// value. The control knows nothing about what that then does.
/// </para>
/// </summary>
public sealed class Button : UIControl
{
    private readonly Label _caption;

    /// <summary>
    /// Initializes a new instance of the <see cref="Button"/> class.
    /// </summary>
    /// <param name="graphics">The surface to draw on.</param>
    /// <param name="style">The button's font and colours.</param>
    /// <param name="setting">The binding pressing this button applies.</param>
    public Button(IGraphics graphics, ControlStyle style, ISetting setting)
        : base(graphics, style, setting)
        => _caption = new(graphics, style, setting) { Alignment = TextAlignment.Centre };

    /// <summary>
    /// Gets or sets where the caption sits within the button. Centred by
    /// default, which is what a button looks like.
    /// </summary>
    public TextAlignment Alignment
    {
        get => _caption.Alignment;
        set => _caption.Alignment = value;
    }

    /// <summary>
    /// Gets or sets the character cell the caption is rounded to, or zero for
    /// none.
    /// </summary>
    public float SnapToCell
    {
        get => _caption.SnapToCell;
        set => _caption.SnapToCell = value;
    }

    /// <summary>
    /// Applies the keys this button answers to while the cursor is on it:
    /// Enter, or Space. A disabled button answers to neither, which is what
    /// being shown but not available means.
    /// </summary>
    /// <param name="keyboard">The keys pressed this tick.</param>
    public void HandleInput(IKeyboard keyboard)
    {
        ArgumentNullException.ThrowIfNull(keyboard);

        if (State is ControlState.Disabled or ControlState.SelectedDisabled)
        {
            return;
        }

        if (keyboard.IsPressed(ConsoleKey.Enter) || keyboard.IsPressed(ConsoleKey.Spacebar))
        {
            Press();
        }
    }

    /// <summary>
    /// Chooses the binding's first value, which is what pressing the button
    /// means: a button's setting has one value, and applying it is where
    /// whatever the button does happens.
    /// </summary>
    public void Press()
    {
        if (Setting.Values.Count > 0)
        {
            Setting.SelectedIndex = 0;
        }
    }

    /// <summary>
    /// Draws the button at its current position, in its current state.
    /// </summary>
    public override void Draw()
    {
        _caption.Position = Position;
        _caption.Width = Width;
        _caption.Height = Height;
        _caption.State = State;
        _caption.Draw();
    }
}
