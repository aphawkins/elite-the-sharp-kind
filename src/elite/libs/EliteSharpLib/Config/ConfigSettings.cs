// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Graphics;
using EliteSharpLib.Planets;
using EliteSharpLib.Suns;

namespace EliteSharpLib.Config;

internal sealed class ConfigSettings
{
    public bool EffectsOn { get; set; } = true;

    // Maximum render frame rate. The game speed is independent, fixed by
    // EliteMain.GameTickRate.
    public float Fps { get; set; } = 60f;

    public bool InstantDock { get; set; }

    public bool MusicOn { get; set; } = true;

    public PlanetDescriptions PlanetDescriptions { get; set; } = PlanetDescriptions.TreeGrubs;

    public PlanetType PlanetStyle { get; set; } = PlanetType.Fractal;

    // Which depth-sort strategy backs filled ship rendering; only takes
    // effect when ShipWireframe is false.
    public ShipRenderMode ShipRenderMode { get; set; } = ShipRenderMode.ZBuffer;

    public bool ShipWireframe { get; set; }

    public SunType SunStyle { get; set; } = SunType.Gradient;

    public bool IsViewFullFrame { get; }
}
