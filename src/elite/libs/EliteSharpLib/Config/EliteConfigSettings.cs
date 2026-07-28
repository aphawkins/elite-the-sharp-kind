// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Planets;
using EliteSharpLib.Suns;
using Useful.Abstraction.Config;
using Useful.Assets;
using Useful.Graphics.Rendering;

namespace EliteSharpLib.Config;

internal sealed class EliteConfigSettings : BaseConfigSettings
{
    // Maximum render frame rate. The game speed is independent, fixed by
    // EliteMain.GameTickRate.
    public float Fps { get; set; } = 60f;

    public bool InstantDock { get; set; }

    // Whether the firing laser beams are outlined or filled.
    public bool LaserWireframe { get; set; }

    public PlanetDescriptions PlanetDescriptions { get; set; } = PlanetDescriptions.TreeGrubs;

    public PlanetType PlanetStyle { get; set; } = PlanetType.Fractal;

    // Which depth-sort strategy backs filled ship rendering; only takes
    // effect when ShipWireframe is false.
    public PolygonRenderMode ShipRenderMode { get; set; } = PolygonRenderMode.ZBuffer;

    public bool ShipWireframe { get; set; }

    public SunType SunStyle { get; set; } = SunType.Gradient;

    // Which machine's look the game reproduces: picks the asset set and,
    // with it, the render resolution and scale. See docs/asset-structure.md.
    public SystemTier Tier { get; set; } = SystemTier.SixteenBit;

    public bool IsViewFullFrame { get; }
}
