// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Planets;

namespace EliteSharp.Config
{
    internal sealed class ConfigSettings
    {
        public bool AntiAliasWireframe { get; set; }

        public float Fps { get; set; } = 13.5f; // Approx speed of TNK

        public bool InstantDock { get; set; }

        public PlanetDescriptions PlanetDescriptions { get; set; } = PlanetDescriptions.TreeGrubs;

        public PlanetType PlanetRenderStyle { get; set; } = PlanetType.Fractal;

        public bool UseWireframe { get; set; }

        /// <summary>
        /// Gets a value indicating whether to use all of the area to render the view.
        /// </summary>
        public bool IsViewFullFrame { get; }
    }
}
