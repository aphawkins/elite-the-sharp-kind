// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Planets;

namespace EliteSharp.Config
{
    internal sealed class ConfigSettings
    {
        internal bool AntiAliasWireframe { get; set; }

        internal float Fps { get; set; } = 13.5f; // Approx speed of TNK

        internal bool InstantDock { get; set; }

        internal PlanetDescriptions PlanetDescriptions { get; set; } = PlanetDescriptions.TreeGrubs;

        internal PlanetType PlanetRenderStyle { get; set; } = PlanetType.Fractal;

        internal bool UseWireframe { get; set; }
    }
}
