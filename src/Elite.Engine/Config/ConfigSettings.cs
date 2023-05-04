using Elite.Engine.Enums;

namespace Elite.Engine.Config
{
    public class ConfigSettings
    {
        public float Fps { get; set; } = 13.5f; // Approx speed of TNK

        public bool UseWireframe { get; set; } = false;

        public bool AntiAliasWireframe { get; set; } = false;

        public PlanetRenderStyle PlanetRenderStyle { get; set; } = PlanetRenderStyle.Fractal;

        public PlanetDescriptions PlanetDescriptions { get; set; } = PlanetDescriptions.TreeGrubs;

        public bool InstantDock { get; set; } = false;
    }
}