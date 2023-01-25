﻿namespace Elite.Engine.Config
{
    using Elite.Engine.Enums;

    public class ConfigSettings
    {
        public int SpeedCap { get; set; } = 75;

        public bool UseWireframe { get; set; } = false;

        public bool AntiAliasWireframe { get; set; } = false;

        public PlanetRenderStyle PlanetRenderStyle { get; set; } = PlanetRenderStyle.Fractal;

        public PlanetDescriptions PlanetDescriptions { get; set; } = PlanetDescriptions.TreeGrubs;

        public bool InstantDock { get; set; } = false;
    }
}