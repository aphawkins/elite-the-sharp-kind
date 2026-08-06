// 'Useful Libraries' - Andy Hawkins 2023-2026.

using Useful.Graphics;

namespace Useful.UI;

/// <summary>
/// A box that stacks its children down its own height and aligns each one
/// across its width. The children keep their own sizes and states; all the
/// container decides is where they go, which is the arithmetic every menu
/// screen was otherwise writing out per row. It has no look of its own -
/// setting its state changes nothing, since which row is selected is the
/// caller's business.
/// <para>
/// Typed by what it holds, so a screen that fills one with rows of a known
/// kind gets them back as that kind. Laying out needs nothing but
/// <see cref="UIControl"/>, but the caller putting a value in a row does need
/// the row, and casting back out of the container to reach it would be a hole
/// in the abstraction rather than a convenience. A container of mixed controls
/// is a <c>Container&lt;UIControl&gt;</c>.
/// </para>
/// <para>
/// A container is itself a <see cref="UIControl"/>, so one can hold another.
/// </para>
/// </summary>
/// <typeparam name="TControl">What this container holds.</typeparam>
/// <param name="graphics">The surface its children draw on.</param>
/// <param name="style">The look its children are built with.</param>
public sealed class Container<TControl>(IGraphics graphics, ControlStyle style)
    : UIControl(graphics, style, ISetting.None)
    where TControl : UIControl
{
    private readonly List<TControl> _children = [];

    /// <summary>
    /// Gets or sets the gap between one child's top edge and the next. A row
    /// pitch rather than a margin, so a tier whose rows touch sets this to the
    /// row height.
    /// </summary>
    public float Spacing { get; set; }

    /// <summary>
    /// Gets or sets where each child sits across the container's width.
    /// </summary>
    public TextAlignment ChildAlignment { get; set; }

    /// <summary>
    /// Gets the children, in the order they are stacked.
    /// </summary>
    public IReadOnlyList<TControl> Children => _children;

    /// <summary>
    /// Adds a child to the bottom of the stack.
    /// </summary>
    /// <param name="child">The control to add.</param>
    public void Add(TControl child) => _children.Add(child);

    /// <summary>
    /// Removes every child.
    /// </summary>
    public void Clear() => _children.Clear();

    /// <summary>
    /// Positions the children and draws them.
    /// </summary>
    public override void Draw()
    {
        Layout();

        foreach (TControl child in _children)
        {
            child.Draw();
        }
    }

    /// <summary>
    /// Positions the children without drawing them, for a caller that needs
    /// to know where a child landed.
    /// </summary>
    public void Layout()
    {
        float y = Position.Y;

        foreach (TControl child in _children)
        {
            child.Position = new(AlignedX(child.Width), y);
            y += Spacing;
        }
    }

    private float AlignedX(float childWidth) => ChildAlignment switch
    {
        TextAlignment.Right => Position.X + Width - childWidth,
        TextAlignment.Centre => Position.X + ((Width - childWidth) / 2),
        _ => Position.X,
    };
}
