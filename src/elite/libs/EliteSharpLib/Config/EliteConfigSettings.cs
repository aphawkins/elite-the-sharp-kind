// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Planets;
using EliteSharpLib.Suns;

namespace EliteSharpLib.Config;

// Elite's own settings, stored under the config file's "game" element.
internal sealed class EliteConfigSettings
{
    public bool InstantDock { get; set; }

    public PlanetDescriptions PlanetDescriptions { get; set; } = PlanetDescriptions.TreeGrubs;

    // The filled planet's surface style; ignored when the engine's
    // GraphicStyle is Wireframe, which draws every planet as one.
    public PlanetType PlanetStyle { get; set; } = PlanetType.Fractal;

    // As PlanetStyle, for the sun.
    public SunType SunStyle { get; set; } = SunType.Gradient;
}
