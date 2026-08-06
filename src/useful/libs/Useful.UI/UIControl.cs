// 'Useful Libraries' - Andy Hawkins 2023-2026.

using System.Numerics;
using Useful.Graphics;

namespace Useful.UI;

/// <summary>
/// Something with bounds that can draw itself. A control keeps its own
/// position, size and state between frames rather than being told them at
/// every draw, which is what lets a <see cref="Container{TControl}"/> lay one
/// out without knowing what it is.
/// <para>
/// A base class rather than an interface because every control is built the
/// same way - a surface to draw on, a look to draw in, and a binding to read
/// from - and that is worth stating once. Requiring the binding here is what
/// makes a control with a copy of its own content impossible to construct:
/// what is on the screen is read back from the binding at every draw.
/// </para>
/// </summary>
public abstract class UIControl
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UIControl"/> class.
    /// </summary>
    /// <param name="graphics">The surface to draw on.</param>
    /// <param name="style">The control's font and colours.</param>
    /// <param name="setting">The binding the control reads and writes.</param>
    protected UIControl(IGraphics graphics, ControlStyle style, ISetting setting)
    {
        ArgumentNullException.ThrowIfNull(graphics);
        ArgumentNullException.ThrowIfNull(style);
        ArgumentNullException.ThrowIfNull(setting);

        Graphics = graphics;
        Style = style;
        Setting = setting;
    }

    /// <summary>
    /// Gets or sets the control's top left corner. A container sets this when
    /// it lays its children out.
    /// </summary>
    public Vector2 Position { get; set; }

    /// <summary>
    /// Gets or sets the control's width.
    /// </summary>
    public float Width { get; set; }

    /// <summary>
    /// Gets or sets the control's height.
    /// </summary>
    public float Height { get; set; }

    /// <summary>
    /// Gets or sets which of the control's looks to draw.
    /// </summary>
    public ControlState State { get; set; }

    /// <summary>
    /// Gets the binding this control shows, and writes any change back
    /// through.
    /// </summary>
    public ISetting Setting { get; }

    /// <summary>
    /// Gets the surface this control draws on.
    /// </summary>
    protected IGraphics Graphics { get; }

    /// <summary>
    /// Gets the font and colours this control draws in.
    /// </summary>
    protected ControlStyle Style { get; }

    /// <summary>
    /// Gets this control's colours for the state it is currently in.
    /// </summary>
    protected ControlColors Colors => Style.Colors(State);

    /// <summary>
    /// Draws the control at its current position, in its current state.
    /// </summary>
    public abstract void Draw();
}
