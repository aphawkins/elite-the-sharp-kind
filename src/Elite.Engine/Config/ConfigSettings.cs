// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using Elite.Engine.Enums;

namespace Elite.Engine.Config
{
    public class ConfigSettings
    {
        public float Fps { get; set; } = 13.5f; // Approx speed of TNK

        public bool UseWireframe { get; set; }

        public bool AntiAliasWireframe { get; set; }

        public PlanetRenderStyle PlanetRenderStyle { get; set; } = PlanetRenderStyle.Fractal;

        public PlanetDescriptions PlanetDescriptions { get; set; } = PlanetDescriptions.TreeGrubs;

        public bool InstantDock { get; set; }
    }
}