// 'Useful Libraries' - Andy Hawkins 2023-2026.

using System.Numerics;

namespace Useful.Widgets;

/// <summary>
/// Something with bounds that can draw itself. A widget keeps its own
/// position, size and state between frames rather than being told them at
/// every draw, which is what lets a <see cref="Container{TWidget}"/> lay one
/// out without knowing what it is.
/// </summary>
public interface IWidget
{
    /// <summary>
    /// Gets or sets the widget's top left corner. A container sets this when
    /// it lays its children out.
    /// </summary>
    public Vector2 Position { get; set; }

    /// <summary>
    /// Gets or sets the widget's width.
    /// </summary>
    public float Width { get; set; }

    /// <summary>
    /// Gets or sets the widget's height.
    /// </summary>
    public float Height { get; set; }

    /// <summary>
    /// Gets or sets which of the widget's looks to draw.
    /// </summary>
    public WidgetState State { get; set; }

    /// <summary>
    /// Draws the widget at its current position, in its current state.
    /// </summary>
    public void Draw();
}
