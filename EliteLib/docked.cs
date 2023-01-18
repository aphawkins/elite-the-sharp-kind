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

namespace Elite
{
	using System.Numerics;
	using Elite.Enums;
	using Elite.Structs;

	internal static class Docked
	{
		internal static int cross_x = 0;
		internal static int cross_y = 0;

		internal static List<Vector2> planetPixels = new();
        internal static List<(Vector2 position, string name)> planetNames = new();
		internal static List<(Vector2 position, int size)> planetSizes = new();
		internal static string planetName;
        internal static int distanceToPlanet;

        internal static int calc_distance_to_planet(galaxy_seed from_planet, galaxy_seed to_planet)
		{
            int dx = Math.Abs(to_planet.d - from_planet.d);
            int dy = Math.Abs(to_planet.b - from_planet.b);

			dx *= dx;
			dy /= 2;
			dy *= dy;

            int light_years = (int)Math.Sqrt(dx + dy);
			light_years *= 4;

			return light_years;
		}

		internal static void show_distance_to_planet()
		{
			int px, py;

			if (elite.current_screen == SCR.SCR_GALACTIC_CHART)
			{
				px = cross_x / gfx.GFX_SCALE;
				py = (cross_y - ((18 * gfx.GFX_SCALE) + 1)) * (2 / gfx.GFX_SCALE);
			}
			else
			{
				px = ((cross_x - gfx.GFX_X_CENTRE) / (4 * gfx.GFX_SCALE)) + elite.docked_planet.d;
				py = ((cross_y - gfx.GFX_Y_CENTRE) / (2 * gfx.GFX_SCALE)) + elite.docked_planet.b;
			}

			elite.hyperspace_planet = Planet.find_planet(px, py);
			planetName = Planet.name_planet(elite.hyperspace_planet);
			distanceToPlanet = calc_distance_to_planet(elite.docked_planet, elite.hyperspace_planet);

			if (elite.current_screen == SCR.SCR_GALACTIC_CHART)
			{
				cross_x = elite.hyperspace_planet.d * gfx.GFX_SCALE;
				cross_y = (elite.hyperspace_planet.b / (2 / gfx.GFX_SCALE)) + (18 * gfx.GFX_SCALE) + 1;
			}
			else
			{
				cross_x = ((elite.hyperspace_planet.d - elite.docked_planet.d) * 4 * gfx.GFX_SCALE) + gfx.GFX_X_CENTRE;
				cross_y = ((elite.hyperspace_planet.b - elite.docked_planet.b) * 2 * gfx.GFX_SCALE) + gfx.GFX_Y_CENTRE;
			}
		}

		internal static void move_cursor_to_origin()
		{
			if (elite.current_screen == SCR.SCR_GALACTIC_CHART)
			{
				cross_x = elite.docked_planet.d * gfx.GFX_SCALE;
				cross_y = (elite.docked_planet.b / (2 / gfx.GFX_SCALE)) + (18 * gfx.GFX_SCALE) + 1;
			}
			else
			{
				cross_x = gfx.GFX_X_CENTRE;
				cross_y = gfx.GFX_Y_CENTRE;
			}

			show_distance_to_planet();
		}

		internal static void find_planet_by_name(string find_name)
		{
			string planet_name = string.Empty;
			bool found = false;
			galaxy_seed glx = elite.cmdr.galaxy;

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
                elite.alg_gfx.ClearTextArea();
                elite.alg_gfx.DrawTextLeft(16, 340, "Unknown Planet", GFX_COL.GFX_COL_WHITE);
				return;
			}

			elite.hyperspace_planet = glx;
			planetName = planet_name;
			distanceToPlanet = calc_distance_to_planet(elite.docked_planet, elite.hyperspace_planet);

			if (elite.current_screen == SCR.SCR_GALACTIC_CHART)
			{
				cross_x = elite.hyperspace_planet.d * gfx.GFX_SCALE;
				cross_y = (elite.hyperspace_planet.b / (2 / gfx.GFX_SCALE)) + (18 * gfx.GFX_SCALE) + 1;
			}
			else
			{
				cross_x = ((elite.hyperspace_planet.d - elite.docked_planet.d) * 4 * gfx.GFX_SCALE) + gfx.GFX_X_CENTRE;
				cross_y = ((elite.hyperspace_planet.b - elite.docked_planet.b) * 2 * gfx.GFX_SCALE) + gfx.GFX_Y_CENTRE;
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

			galaxy_seed glx = elite.cmdr.galaxy;

			for (int i = 0; i < 256; i++)
			{
				int dx = Math.Abs(glx.d - elite.docked_planet.d);
				int dy = Math.Abs(glx.b - elite.docked_planet.b);

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

            cross_x = ((elite.hyperspace_planet.d - elite.docked_planet.d) * 4 * gfx.GFX_SCALE) + gfx.GFX_X_CENTRE;
			cross_y = ((elite.hyperspace_planet.b - elite.docked_planet.b) * 2 * gfx.GFX_SCALE) + gfx.GFX_Y_CENTRE;
		}

		internal static void display_galactic_chart()
		{
			elite.current_screen = SCR.SCR_GALACTIC_CHART;
			galaxy_seed glx = elite.cmdr.galaxy;
			planetPixels.Clear();

            for (int i = 0; i < 256; i++)
			{
                Vector2 pixel = new()
                {
                    X = glx.d * gfx.GFX_SCALE,
                    Y = (glx.b / (2 / gfx.GFX_SCALE)) + (18 * gfx.GFX_SCALE) + 1
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

            cross_x = elite.hyperspace_planet.d * gfx.GFX_SCALE;
			cross_y = (elite.hyperspace_planet.b / (2 / gfx.GFX_SCALE)) + (18 * gfx.GFX_SCALE) + 1;
		}
	}
}