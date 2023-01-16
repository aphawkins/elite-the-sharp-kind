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
			switch (strength)
			{
				case elite.PULSE_LASER:
					return laser_name[0];

				case elite.BEAM_LASER:
					return laser_name[1];

				case elite.MILITARY_LASER:
					return laser_name[2];

				case elite.MINING_LASER:
					return laser_name[3];
			}

			return laser_name[4];
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

					if (type is SHIP.SHIP_MISSILE or
                        > SHIP.SHIP_ROCK and < SHIP.SHIP_DODEC)
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

        private static string[] unit_name = { "t", "kg", "g" };

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

					str = $"{elite.cmdr.current_cargo[i]:D}{unit_name[trade.stock_market[i].units]}";

                    elite.alg_gfx.DrawTextLeft(180, y, str, GFX_COL.GFX_COL_WHITE);
					y += 16;
				}
			}
		}

		/***********************************************************************************/

		enum equip_types
		{
			EQ_FUEL, EQ_MISSILE, EQ_CARGO_BAY, EQ_ECM, EQ_FUEL_SCOOPS,
			EQ_ESCAPE_POD, EQ_ENERGY_BOMB, EQ_ENERGY_UNIT, EQ_DOCK_COMP,
			EQ_GAL_DRIVE, EQ_PULSE_LASER, EQ_FRONT_PULSE, EQ_REAR_PULSE,
			EQ_LEFT_PULSE, EQ_RIGHT_PULSE, EQ_BEAM_LASER, EQ_FRONT_BEAM,
			EQ_REAR_BEAM, EQ_LEFT_BEAM, EQ_RIGHT_BEAM, EQ_MINING_LASER,
			EQ_FRONT_MINING, EQ_REAR_MINING, EQ_LEFT_MINING, EQ_RIGHT_MINING,
			EQ_MILITARY_LASER, EQ_FRONT_MILITARY, EQ_REAR_MILITARY,
			EQ_LEFT_MILITARY, EQ_RIGHT_MILITARY
		};

		const int NO_OF_EQUIP_ITEMS = 34;

		struct equip_item
		{
			internal bool canbuy;
			internal int y;
			internal bool show;
			internal int level;
			internal int price;
			internal string name;
			internal equip_types type;

			internal equip_item(bool canbuy, int y, bool show, int level, int price, string name, equip_types type)
			{
				this.canbuy = canbuy;
				this.y = y;
				this.show = show;
				this.level = level;
				this.price = price;
				this.name = name;
				this.type = type;
			}
		};

		static equip_item[] equip_stock = new equip_item[NO_OF_EQUIP_ITEMS]
		{
			new(false, 0, true, 1,     2, " Fuel",                     equip_types.EQ_FUEL),
			new(false, 0, true, 1,   300, " Missile",                  equip_types.EQ_MISSILE),
			new(false, 0, true, 1,  4000, " Large Cargo Bay",          equip_types.EQ_CARGO_BAY),
			new(false, 0, true, 2,  6000, " E.C.M. System",            equip_types.EQ_ECM),
			new(false, 0, true, 5,  5250, " Fuel Scoops",              equip_types.EQ_FUEL_SCOOPS),
			new(false, 0, true, 6, 10000, " Escape Pod",               equip_types.EQ_ESCAPE_POD),
			new(false, 0, true, 7,  9000, " Energy Bomb",              equip_types.EQ_ENERGY_BOMB),
			new(false, 0, true, 8, 15000, " Extra Energy Unit",        equip_types.EQ_ENERGY_UNIT),
			new(false, 0, true, 9, 15000, " Docking Computers",        equip_types.EQ_DOCK_COMP),
			new(false, 0, true,10, 50000, " Galactic Hyperdrive",      equip_types.EQ_GAL_DRIVE),
			new(false, 0, false, 3,  4000, "+Pulse Laser",              equip_types.EQ_PULSE_LASER),
			new(false, 0, true, 3,     0, "-Pulse Laser",              equip_types.EQ_PULSE_LASER),
			new(false, 0, true, 3,  4000, ">Front",                    equip_types.EQ_FRONT_PULSE),
			new(false, 0, true, 3,  4000, ">Rear",                     equip_types.EQ_REAR_PULSE),
			new(false, 0, true, 3,  4000, ">Left",                     equip_types.EQ_LEFT_PULSE),
			new(false, 0, true, 3,  4000, ">Right",                    equip_types.EQ_RIGHT_PULSE),
			new(false, 0, true, 4, 10000, "+Beam Laser",               equip_types.EQ_BEAM_LASER),
			new(false, 0, false, 4,     0, "-Beam Laser",               equip_types.EQ_BEAM_LASER),
			new(false, 0, false, 4, 10000, ">Front",                    equip_types.EQ_FRONT_BEAM),
			new(false, 0, false, 4, 10000, ">Rear",                     equip_types.EQ_REAR_BEAM),
			new(false, 0, false, 4, 10000, ">Left",                     equip_types.EQ_LEFT_BEAM),
			new(false, 0, false, 4, 10000, ">Right",                    equip_types.EQ_RIGHT_BEAM),
			new(false, 0, true,10,  8000, "+Mining Laser",             equip_types.EQ_MINING_LASER),
			new(false, 0, false,10,     0, "-Mining Laser",             equip_types.EQ_MINING_LASER),
			new(false, 0, false,10,  8000, ">Front",                    equip_types.EQ_FRONT_MINING),
			new(false, 0, false,10,  8000, ">Rear",                     equip_types.EQ_REAR_MINING),
			new(false, 0, false,10,  8000, ">Left",                     equip_types.EQ_LEFT_MINING),
			new(false, 0, false,10,  8000, ">Right",                    equip_types.EQ_RIGHT_MINING),
			new(false, 0, true,10, 60000, "+Military Laser",           equip_types.EQ_MILITARY_LASER),
			new(false, 0, false,10,     0, "-Military Laser",           equip_types.EQ_MILITARY_LASER),
			new(false, 0, false,10, 60000, ">Front",                    equip_types.EQ_FRONT_MILITARY),
			new(false, 0, false,10, 60000, ">Rear",                     equip_types.EQ_REAR_MILITARY),
			new(false, 0, false,10, 60000, ">Left",                     equip_types.EQ_LEFT_MILITARY),
			new(false, 0, false,10, 60000, ">Right",                    equip_types.EQ_RIGHT_MILITARY)
		};

		static bool equip_present(equip_types type)
		{
			switch (type)
			{
				case equip_types.EQ_FUEL:
					return elite.cmdr.fuel >= 70;

				case equip_types.EQ_MISSILE:
					return elite.cmdr.missiles >= 4;

				case equip_types.EQ_CARGO_BAY:
					return elite.cmdr.cargo_capacity > 20;

				case equip_types.EQ_ECM:
					return elite.cmdr.ecm;

				case equip_types.EQ_FUEL_SCOOPS:
					return elite.cmdr.fuel_scoop;

				case equip_types.EQ_ESCAPE_POD:
					return elite.cmdr.escape_pod;

				case equip_types.EQ_ENERGY_BOMB:
					return elite.cmdr.energy_bomb;

				case equip_types.EQ_ENERGY_UNIT:
					return elite.cmdr.energy_unit != 0;

				case equip_types.EQ_DOCK_COMP:
					return elite.cmdr.docking_computer;

				case equip_types.EQ_GAL_DRIVE:
					return elite.cmdr.galactic_hyperdrive;

				case equip_types.EQ_FRONT_PULSE:
					return elite.cmdr.front_laser == elite.PULSE_LASER;

				case equip_types.EQ_REAR_PULSE:
					return elite.cmdr.rear_laser == elite.PULSE_LASER;

				case equip_types.EQ_LEFT_PULSE:
					return elite.cmdr.left_laser == elite.PULSE_LASER;

				case equip_types.EQ_RIGHT_PULSE:
					return elite.cmdr.right_laser == elite.PULSE_LASER;

				case equip_types.EQ_FRONT_BEAM:
					return elite.cmdr.front_laser == elite.BEAM_LASER;

				case equip_types.EQ_REAR_BEAM:
					return elite.cmdr.rear_laser == elite.BEAM_LASER;

				case equip_types.EQ_LEFT_BEAM:
					return elite.cmdr.left_laser == elite.BEAM_LASER;

				case equip_types.EQ_RIGHT_BEAM:
					return elite.cmdr.right_laser == elite.BEAM_LASER;

				case equip_types.EQ_FRONT_MINING:
					return elite.cmdr.front_laser == elite.MINING_LASER;

				case equip_types.EQ_REAR_MINING:
					return elite.cmdr.rear_laser == elite.MINING_LASER;

				case equip_types.EQ_LEFT_MINING:
					return elite.cmdr.left_laser == elite.MINING_LASER;

				case equip_types.EQ_RIGHT_MINING:
					return elite.cmdr.right_laser == elite.MINING_LASER;

				case equip_types.EQ_FRONT_MILITARY:
					return elite.cmdr.front_laser == elite.MILITARY_LASER;

				case equip_types.EQ_REAR_MILITARY:
					return elite.cmdr.rear_laser == elite.MILITARY_LASER;

				case equip_types.EQ_LEFT_MILITARY:
					return elite.cmdr.left_laser == elite.MILITARY_LASER;

				case equip_types.EQ_RIGHT_MILITARY:
					return elite.cmdr.right_laser == elite.MILITARY_LASER;
			}

			return false;
		}

		static void display_equip_price(int i)
		{
			string str;

			int y = equip_stock[i].y;
			if (y == 0)
			{
				return;
			}

            GFX_COL col = equip_stock[i].canbuy ? GFX_COL.GFX_COL_WHITE : GFX_COL.GFX_COL_GREY_1;

			int x = equip_stock[i].name[0] == '>' ? 50 : 16;

            elite.alg_gfx.DrawTextLeft(x, y, equip_stock[i].name[1..], col);

			if (equip_stock[i].price != 0)
			{
				str = $"{equip_stock[i].price / 10:D}.{equip_stock[i].price % 10:D}";
                elite.alg_gfx.DrawTextLeft(338, y, str, col);
			}
		}

		static void highlight_equip(int i)
		{
			int y;
			string str;

			if ((hilite_item != -1) && (hilite_item != i))
			{
				y = equip_stock[hilite_item].y;
                elite.alg_gfx.ClearArea(2, y + 1, 508, 15);
				display_equip_price(hilite_item);
			}

			y = equip_stock[i].y;

            elite.alg_gfx.DrawRectangleFilled(2, y + 1, 508, 15, GFX_COL.GFX_COL_DARK_RED);
			display_equip_price(i);

			hilite_item = i;

            elite.alg_gfx.ClearTextArea();
			str = $"Cash: {elite.cmdr.credits / 10:D}.{elite.cmdr.credits % 10:D}";
            elite.alg_gfx.DrawTextLeft(16, 340, str, GFX_COL.GFX_COL_WHITE);
		}

		internal static void select_next_equip()
		{
			int next;
			int i;

			if (hilite_item == (NO_OF_EQUIP_ITEMS - 1))
			{
				return;
			}

			next = hilite_item;
			for (i = hilite_item + 1; i < NO_OF_EQUIP_ITEMS; i++)
			{
				if (equip_stock[i].y != 0)
				{
					next = i;
					break;
				}
			}

			if (next != hilite_item)
			{
				highlight_equip(next);
			}
		}

		internal static void select_previous_equip()
		{
			int i;
			int prev;

			if (hilite_item == 0)
			{
				return;
			}

			prev = hilite_item;
			for (i = hilite_item - 1; i >= 0; i--)
			{
				if (equip_stock[i].y != 0)
				{
					prev = i;
					break;
				}
			}

			if (prev != hilite_item)
			{
				highlight_equip(prev);
			}
		}

		static void list_equip_prices()
		{
			int i;

            elite.alg_gfx.ClearArea(2, 55, 508, 325);

			int tech_level = elite.current_planet_data.techlevel + 1;

			equip_stock[0].price = (70 - elite.cmdr.fuel) * 2;

			int y = 55;
			for (i = 0; i < NO_OF_EQUIP_ITEMS; i++)
			{
				equip_stock[i].canbuy = (!equip_present(equip_stock[i].type)) && (equip_stock[i].price <= elite.cmdr.credits);

				if (equip_stock[i].show && (tech_level >= equip_stock[i].level))
				{
					equip_stock[i].y = y;
					y += 15;
				}
				else
				{
					equip_stock[i].y = 0;
				}

				display_equip_price(i);
			}

			i = hilite_item;
			hilite_item = -1;
			highlight_equip(i);
		}

		static void collapse_equip_list()
		{
			char ch;

			for (int i = 0; i < NO_OF_EQUIP_ITEMS; i++)
			{
				ch = equip_stock[i].name[0];
				equip_stock[i].show = (ch == ' ') || (ch == '+');
			}
		}

		static int laser_refund(int laser_type)
		{
			switch (laser_type)
			{
				case elite.PULSE_LASER:
					return 4000;

				case elite.BEAM_LASER:
					return 10000;

				case elite.MILITARY_LASER:
					return 60000;

				case elite.MINING_LASER:
					return 8000;
			}

			return 0;
		}

		internal static void buy_equip()
		{
			int i;

			if (equip_stock[hilite_item].name[0] == '+')
			{
				collapse_equip_list();
				equip_stock[hilite_item].show = false;
				hilite_item++;
				for (i = 0; i < 5; i++)
				{
					equip_stock[hilite_item + i].show = true;
				}

				list_equip_prices();
				return;
			}

			if (!equip_stock[hilite_item].canbuy)
			{
				return;
			}

			switch (equip_stock[hilite_item].type)
			{
				case equip_types.EQ_FUEL:
					elite.cmdr.fuel = elite.myship.max_fuel;
					space.update_console();
					break;

				case equip_types.EQ_MISSILE:
					elite.cmdr.missiles++;
					space.update_console();
					break;

				case equip_types.EQ_CARGO_BAY:
					elite.cmdr.cargo_capacity = 35;
					break;

				case equip_types.EQ_ECM:
					elite.cmdr.ecm = true;
					break;

				case equip_types.EQ_FUEL_SCOOPS:
					elite.cmdr.fuel_scoop = true;
					break;

				case equip_types.EQ_ESCAPE_POD:
					elite.cmdr.escape_pod = true;
					break;

				case equip_types.EQ_ENERGY_BOMB:
					elite.cmdr.energy_bomb = true;
					break;

				case equip_types.EQ_ENERGY_UNIT:
					elite.cmdr.energy_unit = 1;
					break;

				case equip_types.EQ_DOCK_COMP:
					elite.cmdr.docking_computer = true;
					break;

				case equip_types.EQ_GAL_DRIVE:
					elite.cmdr.galactic_hyperdrive = true;
					break;

				case equip_types.EQ_FRONT_PULSE:
					elite.cmdr.credits += laser_refund(elite.cmdr.front_laser);
					elite.cmdr.front_laser = elite.PULSE_LASER;
					break;

				case equip_types.EQ_REAR_PULSE:
					elite.cmdr.credits += laser_refund(elite.cmdr.rear_laser);
					elite.cmdr.rear_laser = elite.PULSE_LASER;
					break;

				case equip_types.EQ_LEFT_PULSE:
					elite.cmdr.credits += laser_refund(elite.cmdr.left_laser);
					elite.cmdr.left_laser = elite.PULSE_LASER;
					break;

				case equip_types.EQ_RIGHT_PULSE:
					elite.cmdr.credits += laser_refund(elite.cmdr.right_laser);
					elite.cmdr.right_laser = elite.PULSE_LASER;
					break;

				case equip_types.EQ_FRONT_BEAM:
					elite.cmdr.credits += laser_refund(elite.cmdr.front_laser);
					elite.cmdr.front_laser = elite.BEAM_LASER;
					break;

				case equip_types.EQ_REAR_BEAM:
					elite.cmdr.credits += laser_refund(elite.cmdr.rear_laser);
					elite.cmdr.rear_laser = elite.BEAM_LASER;
					break;

				case equip_types.EQ_LEFT_BEAM:
					elite.cmdr.credits += laser_refund(elite.cmdr.left_laser);
					elite.cmdr.left_laser = elite.BEAM_LASER;
					break;

				case equip_types.EQ_RIGHT_BEAM:
					elite.cmdr.credits += laser_refund(elite.cmdr.right_laser);
					elite.cmdr.right_laser = elite.BEAM_LASER;
					break;

				case equip_types.EQ_FRONT_MINING:
					elite.cmdr.credits += laser_refund(elite.cmdr.front_laser);
					elite.cmdr.front_laser = elite.MINING_LASER;
					break;

				case equip_types.EQ_REAR_MINING:
					elite.cmdr.credits += laser_refund(elite.cmdr.rear_laser);
					elite.cmdr.rear_laser = elite.MINING_LASER;
					break;

				case equip_types.EQ_LEFT_MINING:
					elite.cmdr.credits += laser_refund(elite.cmdr.left_laser);
					elite.cmdr.left_laser = elite.MINING_LASER;
					break;

				case equip_types.EQ_RIGHT_MINING:
					elite.cmdr.credits += laser_refund(elite.cmdr.right_laser);
					elite.cmdr.right_laser = elite.MINING_LASER;
					break;

				case equip_types.EQ_FRONT_MILITARY:
					elite.cmdr.credits += laser_refund(elite.cmdr.front_laser);
					elite.cmdr.front_laser = elite.MILITARY_LASER;
					break;

				case equip_types.EQ_REAR_MILITARY:
					elite.cmdr.credits += laser_refund(elite.cmdr.rear_laser);
					elite.cmdr.rear_laser = elite.MILITARY_LASER;
					break;

				case equip_types.EQ_LEFT_MILITARY:
					elite.cmdr.credits += laser_refund(elite.cmdr.left_laser);
					elite.cmdr.left_laser = elite.MILITARY_LASER;
					break;

				case equip_types.EQ_RIGHT_MILITARY:
					elite.cmdr.credits += laser_refund(elite.cmdr.right_laser);
					elite.cmdr.right_laser = elite.MILITARY_LASER;
					break;
			}

            elite.cmdr.credits -= equip_stock[hilite_item].price;
			list_equip_prices();
		}

		internal static void equip_ship()
		{
			elite.current_screen = SCR.SCR_EQUIP_SHIP;

            elite.alg_gfx.ClearDisplay();
            elite.alg_gfx.DrawTextCentre(20, "EQUIP SHIP", 140, GFX_COL.GFX_COL_GOLD);
            elite.alg_gfx.DrawLine(0, 36, 511, 36);

			collapse_equip_list();

			hilite_item = 0;

			list_equip_prices();
		}
	}
}