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
		static string[] economy_type = {"Rich Industrial",
								"Average Industrial",
								"Poor Industrial",
								"Mainly Industrial",
								"Mainly Agricultural",
								"Rich Agricultural",
								"Average Agricultural",
								"Poor Agricultural"};

		static string[] government_type = { "Anarchy",
									"Feudal",
									"Multi-Government",
									"Dictatorship",
									"Communist",
									"Confederacy",
									"Democracy",
									"Corporate State"};

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

        /*
		 * Displays data on the currently selected Hyperspace Planet.
		 */
        internal static void display_data_on_planet()
		{
			planet_data hyper_planet_data = new();

			elite.current_screen = SCR.SCR_PLANET_DATA;

			string planetName = Planet.name_planet(elite.hyperspace_planet);
            int lightYears = calc_distance_to_planet(elite.docked_planet, elite.hyperspace_planet);
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

		static (int score, string title)[] ratings = new(int score, string title)[]
		{
			new(0x0000, "Harmless"),
			new(0x0008, "Mostly Harmless"),
			new(0x0010, "Poor"),
			new(0x0020, "Average"),
			new(0x0040, "Above Average"),
			new(0x0080, "Competent"),
			new(0x0200, "Dangerous"),
			new(0x0A00, "Deadly"),
			new(0x1900, "---- E L I T E ---")
		};

		static string[] laser_name = new string[5] { "Pulse", "Beam", "Military", "Mining", "Custom" };

		static string laser_type(int strength)
		{
            return strength switch
            {
                elite.PULSE_LASER => laser_name[0],
                elite.BEAM_LASER => laser_name[1],
                elite.MILITARY_LASER => laser_name[2],
                elite.MINING_LASER => laser_name[3],
                _ => laser_name[4],
            };
        }

		const int EQUIP_START_Y = 202;
		const int EQUIP_START_X = 50;
		const int EQUIP_MAX_Y = 290;
		const int EQUIP_WIDTH = 200;
		const int Y_INC = 16;

		static string[] condition_txt = new string[]
		{
			"Docked",
			"Green",
			"Yellow",
			"Red"
		};

		internal static void display_commander_status()
		{
			string planet_name;
			string str;
			int i;
			int x, y;
			int condition;
			SHIP type;

			elite.current_screen = SCR.SCR_CMDR_STATUS;

            elite.alg_gfx.ClearDisplay();

			str = "COMMANDER " + elite.cmdr.name;

            elite.alg_gfx.DrawTextCentre(20, str, 140, GFX_COL.GFX_COL_GOLD);

            elite.alg_gfx.DrawLine(0, 36, 511, 36);

            elite.alg_gfx.DrawTextLeft(16, 58, "Present System:", GFX_COL.GFX_COL_GREEN_1);

			if (!elite.witchspace)
			{
				planet_name = Planet.name_planet(elite.docked_planet);
				planet_name = Planet.capitalise_name(planet_name);
				str = planet_name;
                elite.alg_gfx.DrawTextLeft(190, 58, str, GFX_COL.GFX_COL_WHITE);
			}

            elite.alg_gfx.DrawTextLeft(16, 74, "Hyperspace System:", GFX_COL.GFX_COL_GREEN_1);
			planet_name = Planet.name_planet(elite.hyperspace_planet);
			planet_name = Planet.capitalise_name(planet_name);
			str = planet_name;
            elite.alg_gfx.DrawTextLeft(190, 74, str, GFX_COL.GFX_COL_WHITE);

			if (elite.docked)
			{
				condition = 0;
			}
			else
			{
				condition = 1;

				for (i = 0; i < elite.MAX_UNIV_OBJECTS; i++)
				{
					type = space.universe[i].type;

					if (type is SHIP.SHIP_MISSILE or (> SHIP.SHIP_ROCK and < SHIP.SHIP_DODEC))
					{
						condition = 2;
						break;
					}
				}

				if ((condition == 2) && (elite.energy < 128))
				{
					condition = 3;
				}
			}

            elite.alg_gfx.DrawTextLeft(16, 90, "Condition:", GFX_COL.GFX_COL_GREEN_1);
            elite.alg_gfx.DrawTextLeft(190, 90, condition_txt[condition], GFX_COL.GFX_COL_WHITE);

			str = $"{elite.cmdr.fuel / 10:D}.{elite.cmdr.fuel % 10:D} Light Years";
            elite.alg_gfx.DrawTextLeft(16, 106, "Fuel:", GFX_COL.GFX_COL_GREEN_1);
            elite.alg_gfx.DrawTextLeft(70, 106, str, GFX_COL.GFX_COL_WHITE);

			str = $"{elite.cmdr.credits / 10:D}.{elite.cmdr.credits % 10:D} Cr";
            elite.alg_gfx.DrawTextLeft(16, 122, "Cash:", GFX_COL.GFX_COL_GREEN_1);
            elite.alg_gfx.DrawTextLeft(70, 122, str, GFX_COL.GFX_COL_WHITE);

			str = elite.cmdr.legal_status == 0 ? "Clean" : elite.cmdr.legal_status > 50 ? "Fugitive" : "Offender";

            elite.alg_gfx.DrawTextLeft(16, 138, "Legal Status:", GFX_COL.GFX_COL_GREEN_1);
            elite.alg_gfx.DrawTextLeft(128, 138, str, GFX_COL.GFX_COL_WHITE);

			foreach ((int score, string title) in ratings)
			{
				if (elite.cmdr.score >= score)
				{
					str = title;
				}
			}

            elite.alg_gfx.DrawTextLeft(16, 154, "Rating:", GFX_COL.GFX_COL_GREEN_1);
            elite.alg_gfx.DrawTextLeft(80, 154, str, GFX_COL.GFX_COL_WHITE);

            elite.alg_gfx.DrawTextLeft(16, 186, "EQUIPMENT:", GFX_COL.GFX_COL_GREEN_1);

			x = EQUIP_START_X;
			y = EQUIP_START_Y;

			if (elite.cmdr.cargo_capacity > 20)
			{
                elite.alg_gfx.DrawTextLeft(x, y, "Large Cargo Bay", GFX_COL.GFX_COL_WHITE);
				y += Y_INC;
			}

			if (elite.cmdr.escape_pod)
			{
                elite.alg_gfx.DrawTextLeft(x, y, "Escape Pod", GFX_COL.GFX_COL_WHITE);
				y += Y_INC;
			}

			if (elite.cmdr.fuel_scoop)
			{
                elite.alg_gfx.DrawTextLeft(x, y, "Fuel Scoops", GFX_COL.GFX_COL_WHITE);
				y += Y_INC;
			}

			if (elite.cmdr.ecm)
			{
                elite.alg_gfx.DrawTextLeft(x, y, "E.C.M. System", GFX_COL.GFX_COL_WHITE);
				y += Y_INC;
			}

			if (elite.cmdr.energy_bomb)
			{
                elite.alg_gfx.DrawTextLeft(x, y, "Energy Bomb", GFX_COL.GFX_COL_WHITE);
				y += Y_INC;
			}

			if (elite.cmdr.energy_unit != 0)
			{
                elite.alg_gfx.DrawTextLeft(x, y, elite.cmdr.energy_unit == 1 ? "Extra Energy Unit" : "Naval Energy Unit", GFX_COL.GFX_COL_WHITE);
				y += Y_INC;
				if (y > EQUIP_MAX_Y)
				{
					y = EQUIP_START_Y;
					x += EQUIP_WIDTH;
				}
			}

			if (elite.cmdr.docking_computer)
			{
                elite.alg_gfx.DrawTextLeft(x, y, "Docking Computers", GFX_COL.GFX_COL_WHITE);
				y += Y_INC;
				if (y > EQUIP_MAX_Y)
				{
					y = EQUIP_START_Y;
					x += EQUIP_WIDTH;
				}
			}


			if (elite.cmdr.galactic_hyperdrive)
			{
                elite.alg_gfx.DrawTextLeft(x, y, "Galactic Hyperspace", GFX_COL.GFX_COL_WHITE);
				y += Y_INC;
				if (y > EQUIP_MAX_Y)
				{
					y = EQUIP_START_Y;
					x += EQUIP_WIDTH;
				}
			}

			if (elite.cmdr.front_laser != 0)
			{
				str = $"Front {laser_type(elite.cmdr.front_laser)} Laser";
                elite.alg_gfx.DrawTextLeft(x, y, str, GFX_COL.GFX_COL_WHITE);
				y += Y_INC;
				if (y > EQUIP_MAX_Y)
				{
					y = EQUIP_START_Y;
					x += EQUIP_WIDTH;
				}
			}

			if (elite.cmdr.rear_laser != 0)
			{
				str = $"Rear {laser_type(elite.cmdr.rear_laser)} Laser";
                elite.alg_gfx.DrawTextLeft(x, y, str, GFX_COL.GFX_COL_WHITE);
				y += Y_INC;
				if (y > EQUIP_MAX_Y)
				{
					y = EQUIP_START_Y;
					x += EQUIP_WIDTH;
				}
			}

			if (elite.cmdr.left_laser != 0)
			{
				str = $"Left {laser_type(elite.cmdr.left_laser)} Laser";
                elite.alg_gfx.DrawTextLeft(x, y, str, GFX_COL.GFX_COL_WHITE);
				y += Y_INC;
				if (y > EQUIP_MAX_Y)
				{
					y = EQUIP_START_Y;
					x += EQUIP_WIDTH;
				}
			}

			if (elite.cmdr.right_laser != 0)
			{
				str = $"Right {laser_type(elite.cmdr.right_laser)} Laser";
                elite.alg_gfx.DrawTextLeft(x, y, str, GFX_COL.GFX_COL_WHITE);
			}
		}
	}
}