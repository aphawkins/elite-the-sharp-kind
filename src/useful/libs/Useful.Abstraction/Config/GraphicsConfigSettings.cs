// 'Useful Libraries' - Andy Hawkins 2023-2026.

using Useful.Graphics.Rendering;

namespace Useful.Abstraction.Config;

/// <summary>
/// The engine's graphics settings, stored under the config file's
/// <c>engine.graphics</c> element.
/// </summary>
public sealed class GraphicsConfigSettings
{
    // A frame rate no display runs at and no machine sustains; anything past
    // it is a typo rather than an intention.
    private const float MaxFps = 1000f;

    private const float DefaultFps = 60f;

    // Maximum render frame rate. The game speed is independent of it.
    public float Fps { get; set; } = DefaultFps;

    // Whether the 3D world is drawn as outlines or as filled faces. A game's
    // own per-object style settings (its planet or sun style, say) only apply
    // when this is Solid.
    public GraphicStyle GraphicStyle { get; set; } = GraphicStyle.Solid;

    // Which depth-sort strategy backs filled rendering; only takes effect
    // when GraphicStyle is Solid.
    public DepthSort DepthSort { get; set; } = DepthSort.ZBuffer;

    // Whether to overlay the measured frame rate. Off by default: it is a
    // diagnostic, not part of the game's display. Nothing to repair - a bool
    // the binder cannot read simply stays false.
    public bool ShowFps { get; set; }

    /// <summary>
    /// Replaces any graphics value that cannot be honoured with its default,
    /// in place.
    /// </summary>
    /// <returns><see langword="true"/> if anything had to be replaced.</returns>
    public bool Repair()
    {
        bool repaired = false;

        if (Fps is <= 0 or > MaxFps || float.IsNaN(Fps))
        {
            Fps = DefaultFps;
            repaired = true;
        }

        if (!Enum.IsDefined(GraphicStyle))
        {
            GraphicStyle = GraphicStyle.Solid;
            repaired = true;
        }

        if (!Enum.IsDefined(DepthSort))
        {
            DepthSort = DepthSort.ZBuffer;
            repaired = true;
        }

        return repaired;
    }
}
