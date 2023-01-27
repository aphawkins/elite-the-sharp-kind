/*
 * Elite - The New Kind.
 *
 * Reverse engineered from the BBC disk version of Elite.
 * Additional material by C.J.Pinder.
 *
 * The original Elite code is (C) I.Bell & D.Braben 1984.
 * This version re-engineered in C by C.J.Pinder 1999-2001.
 *
 * email: <christian@newkind.co.uk>
 *
 *
 */

namespace Elite.Engine.Views
{
    using Elite.Engine.Enums;
    using Elite.Engine.Types;

    internal static class PlanetData
	{
        private static readonly string[] economy_type = {"Rich Industrial",
                                "Average Industrial",
                                "Poor Industrial",
                                "Mainly Industrial",
                                "Mainly Agricultural",
                                "Rich Agricultural",
                                "Average Agricultural",
                                "Poor Agricultural"};
        private static readonly string[] government_type = { "Anarchy",
                                    "Feudal",
                                    "Multi-Government",
                                    "Dictatorship",
                                    "Communist",
                                    "Confederacy",
                                    "Democracy",
                                    "Corporate State"};

        /// <summary>
        /// Displays data on the currently selected Hyperspace Planet.
        /// </summary>
        internal static void display_data_on_planet()
		{
			planet_data hyper_planet_data = new();

			elite.current_screen = SCR.SCR_PLANET_DATA;

			string planetName = Planet.name_planet(elite.hyperspace_planet, false);
            float lightYears = GalacticChart.calc_distance_to_planet(elite.docked_planet, elite.hyperspace_planet);
            Planet.generate_planet_data(ref hyper_planet_data, elite.hyperspace_planet);

            elite.draw.DrawDataOnPlanet(planetName, lightYears,
                economy_type[hyper_planet_data.economy],
                government_type[hyper_planet_data.government],
                hyper_planet_data.techlevel + 1,
				hyper_planet_data.population,
				Planet.describe_inhabitants(elite.hyperspace_planet),
                hyper_planet_data.productivity,
                hyper_planet_data.radius,
                Planet.describe_planet(elite.hyperspace_planet)
                );
		}
	}
}