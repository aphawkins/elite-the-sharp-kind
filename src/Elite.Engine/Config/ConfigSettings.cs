// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using Elite.Engine.Enums;

namespace Elite.Engine.Config
{
    public sealed class ConfigSettings
    {
        public bool AntiAliasWireframe { get; set; }

        public float Fps { get; set; } = 13.5f; // Approx speed of TNK

        public bool InstantDock { get; set; }

        public PlanetDescriptions PlanetDescriptions { get; set; } = PlanetDescriptions.TreeGrubs;

        public PlanetRenderStyle PlanetRenderStyle { get; set; } = PlanetRenderStyle.Fractal;

        public bool UseWireframe { get; set; }
    }
}
