// 'Useful Libraries' - Andy Hawkins 2023-2026.

using System.Numerics;

namespace Useful.Widgets;

/// <summary>
/// A box that stacks its children down its own height and aligns each one
/// across its width. The children keep their own sizes and states; all the
/// container decides is where they go, which is the arithmetic every menu
/// screen was otherwise writing out per row.
/// <para>
/// Typed by what it holds, so a screen that fills one with rows of a known
/// kind gets them back as that kind. Laying out needs nothing but
/// <see cref="IWidget"/>, but the caller putting a value in a row does need
/// the row, and casting back out of the container to reach it would be a hole
/// in the abstraction rather than a convenience. A container of mixed widgets
/// is a <c>Container&lt;IWidget&gt;</c>.
/// </para>
/// <para>
/// A container is itself an <see cref="IWidget"/>, so one can hold another.
/// </para>
/// </summary>
/// <typeparam name="TWidget">What this container holds.</typeparam>
public sealed class Container<TWidget> : IWidget
    where TWidget : IWidget
{
    private readonly List<TWidget> _children = [];

    /// <summary>
    /// Gets or sets the container's top left corner.
    /// </summary>
    public Vector2 Position { get; set; }

    /// <summary>
    /// Gets or sets the container's width. Children are aligned across it.
    /// </summary>
    public float Width { get; set; }

    /// <summary>
    /// Gets or sets the container's height. Nothing is clipped to it; it is
    /// here so a container can itself be laid out by another.
    /// </summary>
    public float Height { get; set; }

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
    /// Gets or sets the container's own state. Setting it does not change the
    /// children's - which row is selected is the caller's business, and a
    /// container has no look of its own.
    /// </summary>
    public WidgetState State { get; set; }

    /// <summary>
    /// Gets the children, in the order they are stacked.
    /// </summary>
    public IReadOnlyList<TWidget> Children => _children;

    /// <summary>
    /// Adds a child to the bottom of the stack.
    /// </summary>
    /// <param name="child">The widget to add.</param>
    public void Add(TWidget child) => _children.Add(child);

    /// <summary>
    /// Removes every child.
    /// </summary>
    public void Clear() => _children.Clear();

    /// <summary>
    /// Positions the children and draws them.
    /// </summary>
    public void Draw()
    {
        Layout();

        foreach (TWidget child in _children)
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

        foreach (TWidget child in _children)
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
