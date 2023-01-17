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
			string str;

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
				Vector2 pixel = new();
                pixel.X = glx.d * gfx.GFX_SCALE;
                pixel.Y = (glx.b / (2 / gfx.GFX_SCALE)) + (18 * gfx.GFX_SCALE) + 1;

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

		/***********************************************************************************/

		static int hilite_item;

		internal static void select_previous_stock()
		{
			if ((!elite.docked) || (hilite_item <= 0))
			{
				return;
			}

            hilite_item--;

            elite.draw.DrawMarketPrices(Planet.name_planet(elite.docked_planet), trade.stock_market, hilite_item, elite.cmdr.current_cargo, elite.cmdr.credits);
		}

		internal static void select_next_stock()
		{
			if ((!elite.docked) || (hilite_item >= trade.stock_market.Length - 1))
			{
				return;
			}

			hilite_item++;

            elite.draw.DrawMarketPrices(Planet.name_planet(elite.docked_planet), trade.stock_market, hilite_item, elite.cmdr.current_cargo, elite.cmdr.credits);
		}

		internal static void buy_stock()
		{
			if (!elite.docked)
			{
				return;
			}

			if ((trade.stock_market[hilite_item].current_quantity == 0) || (elite.cmdr.credits < trade.stock_market[hilite_item].current_price))
			{
				return;
			}

			int cargo_held = trade.total_cargo();

			if ((trade.stock_market[hilite_item].units == trade.TONNES) && (cargo_held == elite.cmdr.cargo_capacity))
			{
				return;
			}

			elite.cmdr.current_cargo[hilite_item]++;
            trade.stock_market[hilite_item].current_quantity--;
			elite.cmdr.credits -= trade.stock_market[hilite_item].current_price;

            elite.draw.DrawMarketPrices(Planet.name_planet(elite.docked_planet), trade.stock_market, hilite_item, elite.cmdr.current_cargo, elite.cmdr.credits);
        }

		internal static void sell_stock()
		{
			if ((!elite.docked) || (elite.cmdr.current_cargo[hilite_item] == 0))
			{
				return;
			}

			elite.cmdr.current_cargo[hilite_item]--;
            trade.stock_market[hilite_item].current_quantity++;
			elite.cmdr.credits += trade.stock_market[hilite_item].current_price;

			elite.draw.DrawMarketPrices(Planet.name_planet(elite.docked_planet), trade.stock_market, hilite_item, elite.cmdr.current_cargo, elite.cmdr.credits);
		}

		internal static void display_market_prices()
		{
			elite.current_screen = SCR.SCR_MARKET_PRICES;

			if (!elite.docked)
			{
				hilite_item = -1;
			}

			elite.draw.DrawMarketPrices(Planet.name_planet(elite.docked_planet), trade.stock_market, hilite_item, elite.cmdr.current_cargo, elite.cmdr.credits);
		}

        internal static void display_inventory()
		{
			int i;
			int y;
			string str;

			elite.current_screen = SCR.SCR_INVENTORY;

            elite.alg_gfx.ClearDisplay();
            elite.alg_gfx.DrawTextCentre(20, "INVENTORY", 140, GFX_COL.GFX_COL_GOLD);
            elite.alg_gfx.DrawLine(0, 36, 511, 36);

			str = $"{elite.cmdr.fuel / 10:D}.{elite.cmdr.fuel % 10:D} Light Years";
            elite.alg_gfx.DrawTextLeft(16, 50, "Fuel:", GFX_COL.GFX_COL_GREEN_1);
            elite.alg_gfx.DrawTextLeft(70, 50, str, GFX_COL.GFX_COL_WHITE);

			str = $"{elite.cmdr.credits / 10:D}.{elite.cmdr.credits % 10:D} Cr";
            elite.alg_gfx.DrawTextLeft(16, 66, "Cash:", GFX_COL.GFX_COL_GREEN_1);
            elite.alg_gfx.DrawTextLeft(70, 66, str, GFX_COL.GFX_COL_WHITE);

			y = 98;
			for (i = 0; i < 17; i++)
			{
				if (elite.cmdr.current_cargo[i] > 0)
				{
                    elite.alg_gfx.DrawTextLeft(16, y, trade.stock_market[i].name, GFX_COL.GFX_COL_WHITE);

					str = $"{elite.cmdr.current_cargo[i]}{trade.stock_market[i].units}";

                    elite.alg_gfx.DrawTextLeft(180, y, str, GFX_COL.GFX_COL_WHITE);
					y += 16;
				}
			}
		}

		/***********************************************************************************/
		static EquipmentItem[] EquipmentStock = new EquipmentItem[]
		{
			new(false, true,   1,     2, " Fuel",                EquipmentType.EQ_FUEL),
			new(false, true,   1,   300, " Missile",             EquipmentType.EQ_MISSILE),
			new(false, true,   1,  4000, " Large Cargo Bay",     EquipmentType.EQ_CARGO_BAY),
			new(false, true,   2,  6000, " E.C.M. System",       EquipmentType.EQ_ECM),
			new(false, true,   5,  5250, " Fuel Scoops",         EquipmentType.EQ_FUEL_SCOOPS),
			new(false, true,   6, 10000, " Escape Pod",          EquipmentType.EQ_ESCAPE_POD),
			new(false, true,   7,  9000, " Energy Bomb",         EquipmentType.EQ_ENERGY_BOMB),
			new(false, true,   8, 15000, " Extra Energy Unit",   EquipmentType.EQ_ENERGY_UNIT),
			new(false, true,   9, 15000, " Docking Computers",   EquipmentType.EQ_DOCK_COMP),
			new(false, true,  10, 50000, " Galactic Hyperdrive", EquipmentType.EQ_GAL_DRIVE),
			new(false, false,  3,  4000, "+Pulse Laser",         EquipmentType.EQ_PULSE_LASER),
			new(false, true,   3,     0, "-Pulse Laser",         EquipmentType.EQ_PULSE_LASER),
			new(false, true,   3,  4000, ">Front",               EquipmentType.EQ_FRONT_PULSE),
			new(false, true,   3,  4000, ">Rear",                EquipmentType.EQ_REAR_PULSE),
			new(false, true,   3,  4000, ">Left",                EquipmentType.EQ_LEFT_PULSE),
			new(false, true,   3,  4000, ">Right",               EquipmentType.EQ_RIGHT_PULSE),
			new(false, true,   4, 10000, "+Beam Laser",          EquipmentType.EQ_BEAM_LASER),
			new(false, false,  4,     0, "-Beam Laser",          EquipmentType.EQ_BEAM_LASER),
			new(false, false,  4, 10000, ">Front",               EquipmentType.EQ_FRONT_BEAM),
			new(false, false,  4, 10000, ">Rear",                EquipmentType.EQ_REAR_BEAM),
			new(false, false,  4, 10000, ">Left",                EquipmentType.EQ_LEFT_BEAM),
			new(false, false,  4, 10000, ">Right",               EquipmentType.EQ_RIGHT_BEAM),
			new(false, true,  10,  8000, "+Mining Laser",        EquipmentType.EQ_MINING_LASER),
			new(false, false, 10,     0, "-Mining Laser",        EquipmentType.EQ_MINING_LASER),
			new(false, false, 10,  8000, ">Front",               EquipmentType.EQ_FRONT_MINING),
			new(false, false, 10,  8000, ">Rear",                EquipmentType.EQ_REAR_MINING),
			new(false, false, 10,  8000, ">Left",                EquipmentType.EQ_LEFT_MINING),
			new(false, false, 10,  8000, ">Right",               EquipmentType.EQ_RIGHT_MINING),
			new(false, true,  10, 60000, "+Military Laser",      EquipmentType.EQ_MILITARY_LASER),
			new(false, false, 10,     0, "-Military Laser",      EquipmentType.EQ_MILITARY_LASER),
			new(false, false, 10, 60000, ">Front",               EquipmentType.EQ_FRONT_MILITARY),
			new(false, false, 10, 60000, ">Rear",                EquipmentType.EQ_REAR_MILITARY),
			new(false, false, 10, 60000, ">Left",                EquipmentType.EQ_LEFT_MILITARY),
			new(false, false, 10, 60000, ">Right",               EquipmentType.EQ_RIGHT_MILITARY)
		};

		static bool equip_present(EquipmentType type)
		{
            return type switch
            {
                EquipmentType.EQ_FUEL => elite.cmdr.fuel >= 70,
                EquipmentType.EQ_MISSILE => elite.cmdr.missiles >= 4,
                EquipmentType.EQ_CARGO_BAY => elite.cmdr.cargo_capacity > 20,
                EquipmentType.EQ_ECM => elite.cmdr.ecm,
                EquipmentType.EQ_FUEL_SCOOPS => elite.cmdr.fuel_scoop,
                EquipmentType.EQ_ESCAPE_POD => elite.cmdr.escape_pod,
                EquipmentType.EQ_ENERGY_BOMB => elite.cmdr.energy_bomb,
                EquipmentType.EQ_ENERGY_UNIT => elite.cmdr.energy_unit != 0,
                EquipmentType.EQ_DOCK_COMP => elite.cmdr.docking_computer,
                EquipmentType.EQ_GAL_DRIVE => elite.cmdr.galactic_hyperdrive,
                EquipmentType.EQ_FRONT_PULSE => elite.cmdr.front_laser == elite.PULSE_LASER,
                EquipmentType.EQ_REAR_PULSE => elite.cmdr.rear_laser == elite.PULSE_LASER,
                EquipmentType.EQ_LEFT_PULSE => elite.cmdr.left_laser == elite.PULSE_LASER,
                EquipmentType.EQ_RIGHT_PULSE => elite.cmdr.right_laser == elite.PULSE_LASER,
                EquipmentType.EQ_FRONT_BEAM => elite.cmdr.front_laser == elite.BEAM_LASER,
                EquipmentType.EQ_REAR_BEAM => elite.cmdr.rear_laser == elite.BEAM_LASER,
                EquipmentType.EQ_LEFT_BEAM => elite.cmdr.left_laser == elite.BEAM_LASER,
                EquipmentType.EQ_RIGHT_BEAM => elite.cmdr.right_laser == elite.BEAM_LASER,
                EquipmentType.EQ_FRONT_MINING => elite.cmdr.front_laser == elite.MINING_LASER,
                EquipmentType.EQ_REAR_MINING => elite.cmdr.rear_laser == elite.MINING_LASER,
                EquipmentType.EQ_LEFT_MINING => elite.cmdr.left_laser == elite.MINING_LASER,
                EquipmentType.EQ_RIGHT_MINING => elite.cmdr.right_laser == elite.MINING_LASER,
                EquipmentType.EQ_FRONT_MILITARY => elite.cmdr.front_laser == elite.MILITARY_LASER,
                EquipmentType.EQ_REAR_MILITARY => elite.cmdr.rear_laser == elite.MILITARY_LASER,
                EquipmentType.EQ_LEFT_MILITARY => elite.cmdr.left_laser == elite.MILITARY_LASER,
                EquipmentType.EQ_RIGHT_MILITARY => elite.cmdr.right_laser == elite.MILITARY_LASER,
                _ => false,
            };
        }

		internal static void select_next_equip()
		{
			if (hilite_item == (EquipmentStock.Length - 1))
			{
				return;
			}

			for (int i = hilite_item + 1; i < EquipmentStock.Length; i++)
			{
				if (EquipmentStock[i].Show)
				{
                    hilite_item = i;
					break;
				}
			}

			elite.draw.DrawEquipShip(EquipmentStock, hilite_item, elite.cmdr.credits);
		}

		internal static void select_previous_equip()
		{
			if (hilite_item == 0)
			{
				return;
			}

			for (int i = hilite_item - 1; i >= 0; i--)
			{
				if (EquipmentStock[i].Show)
				{
                    hilite_item = i;
					break;
				}
			}
               
			elite.draw.DrawEquipShip(EquipmentStock, hilite_item, elite.cmdr.credits);
		}

		static void list_equip_prices()
		{
			int tech_level = elite.current_planet_data.techlevel + 1;

			EquipmentStock[0].Price = (70 - elite.cmdr.fuel) * 2;

			for (int i = 0; i < EquipmentStock.Length; i++)
			{
				EquipmentStock[i].CanBuy = (!equip_present(EquipmentStock[i].Type)) && (EquipmentStock[i].Price <= elite.cmdr.credits);
				EquipmentStock[i].Show = EquipmentStock[i].Show && (tech_level >= EquipmentStock[i].TechLevel);
            }

			hilite_item = 0;
			elite.draw.DrawEquipShip(EquipmentStock, hilite_item, elite.cmdr.credits);
        }

		static void collapse_equip_list()
		{
			for (int i = 0; i < EquipmentStock.Length; i++)
			{
				char ch = EquipmentStock[i].Name[0];
				EquipmentStock[i].Show = ch is ' ' or '+';
			}
		}

		static int laser_refund(int laser_type)
		{
            return laser_type switch
            {
                elite.PULSE_LASER => 4000,
                elite.BEAM_LASER => 10000,
                elite.MILITARY_LASER => 60000,
                elite.MINING_LASER => 8000,
                _ => 0,
            };
        }

		internal static void buy_equip()
		{
			if (EquipmentStock[hilite_item].Name[0] == '+')
			{
				collapse_equip_list();
				EquipmentStock[hilite_item].Show = false;
				hilite_item++;
				for (int i = 0; i < 5; i++)
				{
					EquipmentStock[hilite_item + i].Show = true;
				}

				list_equip_prices();
				return;
			}

			if (!EquipmentStock[hilite_item].CanBuy)
			{
				return;
			}

			switch (EquipmentStock[hilite_item].Type)
			{
				case EquipmentType.EQ_FUEL:
					elite.cmdr.fuel = elite.myship.max_fuel;
					space.update_console();
					break;

				case EquipmentType.EQ_MISSILE:
					elite.cmdr.missiles++;
					space.update_console();
					break;

				case EquipmentType.EQ_CARGO_BAY:
					elite.cmdr.cargo_capacity = 35;
					break;

				case EquipmentType.EQ_ECM:
					elite.cmdr.ecm = true;
					break;

				case EquipmentType.EQ_FUEL_SCOOPS:
					elite.cmdr.fuel_scoop = true;
					break;

				case EquipmentType.EQ_ESCAPE_POD:
					elite.cmdr.escape_pod = true;
					break;

				case EquipmentType.EQ_ENERGY_BOMB:
					elite.cmdr.energy_bomb = true;
					break;

				case EquipmentType.EQ_ENERGY_UNIT:
					elite.cmdr.energy_unit = 1;
					break;

				case EquipmentType.EQ_DOCK_COMP:
					elite.cmdr.docking_computer = true;
					break;

				case EquipmentType.EQ_GAL_DRIVE:
					elite.cmdr.galactic_hyperdrive = true;
					break;

				case EquipmentType.EQ_FRONT_PULSE:
					elite.cmdr.credits += laser_refund(elite.cmdr.front_laser);
					elite.cmdr.front_laser = elite.PULSE_LASER;
					break;

				case EquipmentType.EQ_REAR_PULSE:
					elite.cmdr.credits += laser_refund(elite.cmdr.rear_laser);
					elite.cmdr.rear_laser = elite.PULSE_LASER;
					break;

				case EquipmentType.EQ_LEFT_PULSE:
					elite.cmdr.credits += laser_refund(elite.cmdr.left_laser);
					elite.cmdr.left_laser = elite.PULSE_LASER;
					break;

				case EquipmentType.EQ_RIGHT_PULSE:
					elite.cmdr.credits += laser_refund(elite.cmdr.right_laser);
					elite.cmdr.right_laser = elite.PULSE_LASER;
					break;

				case EquipmentType.EQ_FRONT_BEAM:
					elite.cmdr.credits += laser_refund(elite.cmdr.front_laser);
					elite.cmdr.front_laser = elite.BEAM_LASER;
					break;

				case EquipmentType.EQ_REAR_BEAM:
					elite.cmdr.credits += laser_refund(elite.cmdr.rear_laser);
					elite.cmdr.rear_laser = elite.BEAM_LASER;
					break;

				case EquipmentType.EQ_LEFT_BEAM:
					elite.cmdr.credits += laser_refund(elite.cmdr.left_laser);
					elite.cmdr.left_laser = elite.BEAM_LASER;
					break;

				case EquipmentType.EQ_RIGHT_BEAM:
					elite.cmdr.credits += laser_refund(elite.cmdr.right_laser);
					elite.cmdr.right_laser = elite.BEAM_LASER;
					break;

				case EquipmentType.EQ_FRONT_MINING:
					elite.cmdr.credits += laser_refund(elite.cmdr.front_laser);
					elite.cmdr.front_laser = elite.MINING_LASER;
					break;

				case EquipmentType.EQ_REAR_MINING:
					elite.cmdr.credits += laser_refund(elite.cmdr.rear_laser);
					elite.cmdr.rear_laser = elite.MINING_LASER;
					break;

				case EquipmentType.EQ_LEFT_MINING:
					elite.cmdr.credits += laser_refund(elite.cmdr.left_laser);
					elite.cmdr.left_laser = elite.MINING_LASER;
					break;

				case EquipmentType.EQ_RIGHT_MINING:
					elite.cmdr.credits += laser_refund(elite.cmdr.right_laser);
					elite.cmdr.right_laser = elite.MINING_LASER;
					break;

				case EquipmentType.EQ_FRONT_MILITARY:
					elite.cmdr.credits += laser_refund(elite.cmdr.front_laser);
					elite.cmdr.front_laser = elite.MILITARY_LASER;
					break;

				case EquipmentType.EQ_REAR_MILITARY:
					elite.cmdr.credits += laser_refund(elite.cmdr.rear_laser);
					elite.cmdr.rear_laser = elite.MILITARY_LASER;
					break;

				case EquipmentType.EQ_LEFT_MILITARY:
					elite.cmdr.credits += laser_refund(elite.cmdr.left_laser);
					elite.cmdr.left_laser = elite.MILITARY_LASER;
					break;

				case EquipmentType.EQ_RIGHT_MILITARY:
					elite.cmdr.credits += laser_refund(elite.cmdr.right_laser);
					elite.cmdr.right_laser = elite.MILITARY_LASER;
					break;
			}

            elite.cmdr.credits -= EquipmentStock[hilite_item].Price;
			list_equip_prices();
		}

		internal static void equip_ship()
		{
			elite.current_screen = SCR.SCR_EQUIP_SHIP;

			collapse_equip_list();

			hilite_item = 0;

            list_equip_prices();
		}
	}
}