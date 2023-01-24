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
    using System.Numerics;
	using Elite.Engine.Enums;
	using Elite.Engine.Types;

	internal static class GalacticChart
	{
		internal static Vector2 cross = new Vector2(0, 0);
		internal static List<Vector2> planetPixels = new();
        internal static List<(Vector2 position, string name)> planetNames = new();
		internal static List<(Vector2 position, int size)> planetSizes = new();
		internal static string planetName;
        internal static float distanceToPlanet;

        internal static float calc_distance_to_planet(galaxy_seed from_planet, galaxy_seed to_planet)
		{
            float dx = MathF.Abs(to_planet.d - from_planet.d);
            float dy = MathF.Abs(to_planet.b - from_planet.b);

			dx *= dx;
			dy /= 2;
			dy *= dy;

            float light_years = MathF.Sqrt(dx + dy);
			light_years *= 4f;

			return light_years / 10;
		}

		internal static void show_distance_to_planet()
		{
			Vector2 location;

			if (elite.current_screen == SCR.SCR_GALACTIC_CHART)
			{
				location.X = cross.X / gfx.GFX_SCALE;
				location.Y = (cross.Y - ((18f * gfx.GFX_SCALE) + 1)) * (2f / gfx.GFX_SCALE);
			}
			else
			{
				location.X = ((cross.X - gfx.GFX_X_CENTRE) / (4f * gfx.GFX_SCALE)) + elite.docked_planet.d;
				location.Y = ((cross.Y - gfx.GFX_Y_CENTRE) / (2f * gfx.GFX_SCALE)) + elite.docked_planet.b;
			}

			elite.hyperspace_planet = Planet.find_planet(location);
			planetName = Planet.name_planet(elite.hyperspace_planet);
			distanceToPlanet = calc_distance_to_planet(elite.docked_planet, elite.hyperspace_planet);

			if (elite.current_screen == SCR.SCR_GALACTIC_CHART)
			{
				cross.X = elite.hyperspace_planet.d * gfx.GFX_SCALE;
				cross.Y = (elite.hyperspace_planet.b / (2f / gfx.GFX_SCALE)) + (18 * gfx.GFX_SCALE) + 1;
			}
			else
			{
				cross.X = ((elite.hyperspace_planet.d - elite.docked_planet.d) * 4 * gfx.GFX_SCALE) + gfx.GFX_X_CENTRE;
				cross.Y = ((elite.hyperspace_planet.b - elite.docked_planet.b) * 2 * gfx.GFX_SCALE) + gfx.GFX_Y_CENTRE;
			}
		}

		internal static void move_cursor_to_origin()
		{
			if (elite.current_screen == SCR.SCR_GALACTIC_CHART)
			{
				cross.X = elite.docked_planet.d * gfx.GFX_SCALE;
				cross.Y = (elite.docked_planet.b / (2 / gfx.GFX_SCALE)) + (18 * gfx.GFX_SCALE) + 1;
			}
			else
			{
				cross = new(gfx.GFX_X_CENTRE, gfx.GFX_Y_CENTRE);
			}

			show_distance_to_planet();
		}

		internal static void find_planet_by_name(string find_name)
		{
			string planet_name = string.Empty;
			bool found = false;
			galaxy_seed glx = (galaxy_seed)elite.cmdr.galaxy.Clone();

			for (int i = 0; i < 256; i++)
			{
				planet_name = Planet.name_planet(glx);

				if (planet_name == find_name)
				{
					found = true;
					break;
				}

				Planet.waggle_galaxy(ref glx);
				Planet.waggle_galaxy(ref glx);
				Planet.waggle_galaxy(ref glx);
				Planet.waggle_galaxy(ref glx);
			}

			if (!found)
			{
                elite.draw.ClearTextArea();
                elite.alg_gfx.DrawTextLeft(16, 340, "Unknown Planet", GFX_COL.GFX_COL_WHITE);
				return;
			}

			elite.hyperspace_planet = glx;
			planetName = planet_name;
			distanceToPlanet = calc_distance_to_planet(elite.docked_planet, elite.hyperspace_planet);

			if (elite.current_screen == SCR.SCR_GALACTIC_CHART)
			{
				cross.X = elite.hyperspace_planet.d * gfx.GFX_SCALE;
				cross.Y = (elite.hyperspace_planet.b / (2f / gfx.GFX_SCALE)) + (18f * gfx.GFX_SCALE) + 1;
			}
			else
			{
				cross.X = ((elite.hyperspace_planet.d - elite.docked_planet.d) * 4f * gfx.GFX_SCALE) + gfx.GFX_X_CENTRE;
				cross.Y = ((elite.hyperspace_planet.b - elite.docked_planet.b) * 2f * gfx.GFX_SCALE) + gfx.GFX_Y_CENTRE;
			}
		}

		internal static void display_short_range_chart()
		{
			int[] row_used = new int[64];
			elite.current_screen = SCR.SCR_SHORT_RANGE;
			planetNames.Clear();
			planetSizes.Clear();

            for (int i = 0; i < 64; i++)
			{
				row_used[i] = 0;
			}

			galaxy_seed glx = (galaxy_seed)elite.cmdr.galaxy.Clone();

			for (int i = 0; i < 256; i++)
			{
				float dx = MathF.Abs(glx.d - elite.docked_planet.d);
				float dy = MathF.Abs(glx.b - elite.docked_planet.b);

				if ((dx >= 20) || (dy >= 38))
				{
					Planet.waggle_galaxy(ref glx);
					Planet.waggle_galaxy(ref glx);
					Planet.waggle_galaxy(ref glx);
					Planet.waggle_galaxy(ref glx);

					continue;
				}

				int px = glx.d - elite.docked_planet.d;
				px = (px * 4 * gfx.GFX_SCALE) + gfx.GFX_X_CENTRE;  /* Convert to screen co-ords */

				int py = glx.b - elite.docked_planet.b;
				py = (py * 2 * gfx.GFX_SCALE) + gfx.GFX_Y_CENTRE; /* Convert to screen co-ords */

				int row = py / (8 * gfx.GFX_SCALE);

				if (row_used[row] == 1)
				{
					row++;
				}

				if (row_used[row] == 1)
				{
					row -= 2;
				}

				if (row <= 3)
				{
					Planet.waggle_galaxy(ref glx);
					Planet.waggle_galaxy(ref glx);
					Planet.waggle_galaxy(ref glx);
					Planet.waggle_galaxy(ref glx);

					continue;
				}

				if (row_used[row] == 0)
				{
					row_used[row] = 1;
					string planet_name = Planet.name_planet(glx);
					planet_name = Planet.capitalise_name(planet_name);

					planetNames.Add((new(px + (4 * gfx.GFX_SCALE), ((row * 8) - 5) * gfx.GFX_SCALE), planet_name));
				}

				/* The next bit calculates the size of the circle used to represent */
				/* a planet.  The carry_flag is left over from the name generation. */
				/* Yes this was how it was done... don't ask :-( */
				int blob_size = (glx.f & 1) + 2 + elite.carry_flag;
				blob_size *= gfx.GFX_SCALE;
				planetSizes.Add((new(px, py), blob_size));
                

				Planet.waggle_galaxy(ref glx);
				Planet.waggle_galaxy(ref glx);
				Planet.waggle_galaxy(ref glx);
				Planet.waggle_galaxy(ref glx);
			}

			elite.draw.DrawShortRangeChart(planetNames, planetSizes, planetName, distanceToPlanet);

            cross.X = ((elite.hyperspace_planet.d - elite.docked_planet.d) * 4f * gfx.GFX_SCALE) + gfx.GFX_X_CENTRE;
			cross.Y = ((elite.hyperspace_planet.b - elite.docked_planet.b) * 2f * gfx.GFX_SCALE) + gfx.GFX_Y_CENTRE;
		}

		internal static void display_galactic_chart()
		{
			elite.current_screen = SCR.SCR_GALACTIC_CHART;
			galaxy_seed glx = (galaxy_seed)elite.cmdr.galaxy.Clone();
			planetPixels.Clear();

            for (int i = 0; i < 256; i++)
			{
                Vector2 pixel = new()
                {
                    X = glx.d * gfx.GFX_SCALE,
                    Y = (glx.b / (2f / gfx.GFX_SCALE)) + (18f * gfx.GFX_SCALE) + 1
                };

                planetPixels.Add(pixel);

				if ((glx.e | 0x50) < 0x90)
				{
					planetPixels.Add(new(pixel.X + 1, pixel.Y));
				}

				Planet.waggle_galaxy(ref glx);
				Planet.waggle_galaxy(ref glx);
				Planet.waggle_galaxy(ref glx);
				Planet.waggle_galaxy(ref glx);
			}

            elite.draw.DrawGalacticChart(elite.cmdr.galaxy_number + 1, planetPixels, planetName, distanceToPlanet);

            cross.X = elite.hyperspace_planet.d * gfx.GFX_SCALE;
			cross.Y = (elite.hyperspace_planet.b / (2 / gfx.GFX_SCALE)) + (18 * gfx.GFX_SCALE) + 1;
		}
	}
}