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

using Elite;
using Elite.Enums;
using Elite.Structs;

namespace Elite
{
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

		static void draw_fuel_limit_circle(int cx, int cy)
		{
			int radius;
			int cross_size;

			if (elite.current_screen == SCR.SCR_GALACTIC_CHART)
			{
				radius = elite.cmdr.fuel / 4 * gfx.GFX_SCALE;
				cross_size = 7 * gfx.GFX_SCALE;
			}
			else
			{
				radius = elite.cmdr.fuel * gfx.GFX_SCALE;
				cross_size = 16 * gfx.GFX_SCALE;
			}

			alg_gfx.gfx_draw_circle(cx, cy, radius, gfx.GFX_COL_GREEN_1);

			alg_gfx.gfx_draw_line(cx, cy - cross_size, cx, cy + cross_size);
			alg_gfx.gfx_draw_line(cx - cross_size, cy, cx + cross_size, cy);
		}

		internal static int calc_distance_to_planet(galaxy_seed from_planet, galaxy_seed to_planet)
		{
			int dx, dy;
			int light_years;

			dx = Math.Abs(to_planet.d - from_planet.d);
			dy = Math.Abs(to_planet.b - from_planet.b);

			dx = dx * dx;
			dy = dy / 2;
			dy = dy * dy;

			light_years = (int)Math.Sqrt(dx + dy);
			light_years *= 4;

			return light_years;
		}

		static void show_distance(int ypos, galaxy_seed from_planet, galaxy_seed to_planet)
		{
			string str;
			int light_years;

			light_years = calc_distance_to_planet(from_planet, to_planet);

			if (light_years > 0)
			{
				str = $"Distance: {light_years / 10:2d}.{light_years % 10:d} Light Years ";
			}
			else
			{
				str = "                                                     ";
			}

			alg_gfx.gfx_display_text(16, ypos, str);
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

			string planet_name = Planet.name_planet(ref elite.hyperspace_planet);

			alg_gfx.gfx_clear_text_area();
			str = $"{planet_name:-18s}";
			alg_gfx.gfx_display_text(16, 340, str);

			show_distance(356, elite.docked_planet, elite.hyperspace_planet);

			if (elite.current_screen == SCR.SCR_GALACTIC_CHART)
			{
				cross_x = elite.hyperspace_planet.d * gfx.GFX_SCALE;
				cross_y = elite.hyperspace_planet.b / (2 / gfx.GFX_SCALE) + (18 * gfx.GFX_SCALE) + 1;
			}
			else
			{
				cross_x = ((elite.hyperspace_planet.d - elite.docked_planet.d) * (4 * gfx.GFX_SCALE)) + gfx.GFX_X_CENTRE;
				cross_y = ((elite.hyperspace_planet.b - elite.docked_planet.b) * (2 * gfx.GFX_SCALE)) + gfx.GFX_Y_CENTRE;
			}
		}

		internal static void move_cursor_to_origin()
		{
			if (elite.current_screen == SCR.SCR_GALACTIC_CHART)
			{
				cross_x = elite.docked_planet.d * gfx.GFX_SCALE;
				cross_y = elite.docked_planet.b / (2 / gfx.GFX_SCALE) + (18 * gfx.GFX_SCALE) + 1;
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
				planet_name = Planet.name_planet(ref glx);

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
				alg_gfx.gfx_clear_text_area();
				alg_gfx.gfx_display_text(16, 340, "Unknown Planet");
				return;
			}

			elite.hyperspace_planet = glx;

			alg_gfx.gfx_clear_text_area();
			string str = $"{planet_name:-18s}";
			alg_gfx.gfx_display_text(16, 340, str);

			show_distance(356, elite.docked_planet, elite.hyperspace_planet);

			if (elite.current_screen == SCR.SCR_GALACTIC_CHART)
			{
				cross_x = elite.hyperspace_planet.d * gfx.GFX_SCALE;
				cross_y = elite.hyperspace_planet.b / (2 / gfx.GFX_SCALE) + (18 * gfx.GFX_SCALE) + 1;
			}
			else
			{
				cross_x = ((elite.hyperspace_planet.d - elite.docked_planet.d) * (4 * gfx.GFX_SCALE)) + gfx.GFX_X_CENTRE;
				cross_y = ((elite.hyperspace_planet.b - elite.docked_planet.b) * (2 * gfx.GFX_SCALE)) + gfx.GFX_Y_CENTRE;
			}
		}

		static void display_short_range_chart()
		{
			int i;
			galaxy_seed glx;
			int dx, dy;
			int px, py;
			int[] row_used = new int[64];
			int row;
			int blob_size;

			elite.current_screen = SCR.SCR_SHORT_RANGE;

			alg_gfx.gfx_clear_display();

			alg_gfx.gfx_display_centre_text(10, "SHORT RANGE CHART", 140, gfx.GFX_COL_GOLD);

			alg_gfx.gfx_draw_line(0, 36, 511, 36);

			draw_fuel_limit_circle(gfx.GFX_X_CENTRE, gfx.GFX_Y_CENTRE);

			for (i = 0; i < 64; i++)
			{
				row_used[i] = 0;
			}

			glx = elite.cmdr.galaxy;

			for (i = 0; i < 256; i++)
			{

				dx = Math.Abs(glx.d - elite.docked_planet.d);
				dy = Math.Abs(glx.b - elite.docked_planet.b);

				if ((dx >= 20) || (dy >= 38))
				{
					Planet.waggle_galaxy(ref glx);
					Planet.waggle_galaxy(ref glx);
					Planet.waggle_galaxy(ref glx);
					Planet.waggle_galaxy(ref glx);

					continue;
				}

				px = (glx.d - elite.docked_planet.d);
				px = px * 4 * gfx.GFX_SCALE + gfx.GFX_X_CENTRE;  /* Convert to screen co-ords */

				py = (glx.b - elite.docked_planet.b);
				py = py * 2 * gfx.GFX_SCALE + gfx.GFX_Y_CENTRE; /* Convert to screen co-ords */

				row = py / (8 * gfx.GFX_SCALE);

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
					string planet_name = Planet.name_planet(ref glx);
					planet_name = Planet.capitalise_name(planet_name);
					alg_gfx.gfx_display_text(px + (4 * gfx.GFX_SCALE), (row * 8 - 5) * gfx.GFX_SCALE, planet_name);
				}

				/* The next bit calculates the size of the circle used to represent */
				/* a planet.  The carry_flag is left over from the name generation. */
				/* Yes this was how it was done... don't ask :-( */

				blob_size = (glx.f & 1) + 2 + elite.carry_flag;
				blob_size *= gfx.GFX_SCALE;
				alg_gfx.gfx_draw_filled_circle(px, py, blob_size, gfx.GFX_COL_GOLD);

				Planet.waggle_galaxy(ref glx);
				Planet.waggle_galaxy(ref glx);
				Planet.waggle_galaxy(ref glx);
				Planet.waggle_galaxy(ref glx);
			}

			cross_x = ((elite.hyperspace_planet.d - elite.docked_planet.d) * 4 * gfx.GFX_SCALE) + gfx.GFX_X_CENTRE;
			cross_y = ((elite.hyperspace_planet.b - elite.docked_planet.b) * 2 * gfx.GFX_SCALE) + gfx.GFX_Y_CENTRE;
		}

		static void display_galactic_chart()
		{
			int px, py;

			elite.current_screen = SCR.SCR_GALACTIC_CHART;

			alg_gfx.gfx_clear_display();

			string str = "GALACTIC CHART {elite.cmdr.galaxy_number + 1:d}";

			alg_gfx.gfx_display_centre_text(10, str, 140, gfx.GFX_COL_GOLD);

			alg_gfx.gfx_draw_line(0, 36, 511, 36);
			alg_gfx.gfx_draw_line(0, 36 + 258, 511, 36 + 258);

			draw_fuel_limit_circle(elite.docked_planet.d * gfx.GFX_SCALE, (elite.docked_planet.b / (2 / gfx.GFX_SCALE)) + (18 * gfx.GFX_SCALE) + 1);

			galaxy_seed glx = elite.cmdr.galaxy;

			for (int i = 0; i < 256; i++)
			{
				px = glx.d * gfx.GFX_SCALE;
				py = (glx.b / (2 / gfx.GFX_SCALE)) + (18 * gfx.GFX_SCALE) + 1;

				alg_gfx.gfx_plot_pixel(px, py, gfx.GFX_COL_WHITE);

				if ((glx.e | 0x50) < 0x90)
				{
					alg_gfx.gfx_plot_pixel(px + 1, py, gfx.GFX_COL_WHITE);
				}

				Planet.waggle_galaxy(ref glx);
				Planet.waggle_galaxy(ref glx);
				Planet.waggle_galaxy(ref glx);
				Planet.waggle_galaxy(ref glx);
			}

			cross_x = elite.hyperspace_planet.d * gfx.GFX_SCALE;
			cross_y = (elite.hyperspace_planet.b / (2 / gfx.GFX_SCALE)) + (18 * gfx.GFX_SCALE) + 1;
		}

		/*
		 * Displays data on the currently selected Hyperspace Planet.
		 */
		static void display_data_on_planet()
		{
			string description;
			planet_data hyper_planet_data = new();

			elite.current_screen = SCR.SCR_PLANET_DATA;

			alg_gfx.gfx_clear_display();

			string planet_name = Planet.name_planet(ref elite.hyperspace_planet);
			string str = "DATA ON " + planet_name;

			alg_gfx.gfx_display_centre_text(10, str, 140, gfx.GFX_COL_GOLD);

			alg_gfx.gfx_draw_line(0, 36, 511, 36);

			Planet.generate_planet_data(ref hyper_planet_data, elite.hyperspace_planet);

			show_distance(42, elite.docked_planet, elite.hyperspace_planet);

			str = "Economy:" + economy_type[hyper_planet_data.economy];
			alg_gfx.gfx_display_text(16, 74, str);

			str = "Government:" + government_type[hyper_planet_data.government];
			alg_gfx.gfx_display_text(16, 106, str);

			str = $"Tech.Level:{hyper_planet_data.techlevel + 1:3d}";
			alg_gfx.gfx_display_text(16, 138, str);

			str = $"Population:{hyper_planet_data.population / 10:d}.{hyper_planet_data.population % 10:d} Billion";
			alg_gfx.gfx_display_text(16, 170, str);

			str = Planet.describe_inhabitants(str, elite.hyperspace_planet);
			alg_gfx.gfx_display_text(16, 202, str);

			str = $"Gross Productivity:{hyper_planet_data.productivity:5d} M CR";
			alg_gfx.gfx_display_text(16, 234, str);

			str = "Average Radius:{hyper_planet_data.radius:5d} km";
			alg_gfx.gfx_display_text(16, 266, str);

			description = Planet.describe_planet(elite.hyperspace_planet);
			alg_gfx.gfx_display_pretty_text(16, 298, 400, 384, description);
		}

		struct rank
		{
			internal int score;
			internal string title;

			internal rank(int score, string title)
			{
				this.score = score;
				this.title = title;
			}
		};

		const int NO_OF_RANKS = 9;

		static rank[] rating = new rank[NO_OF_RANKS]
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

			alg_gfx.gfx_clear_display();

			str = "COMMANDER " + elite.cmdr.name;

			alg_gfx.gfx_display_centre_text(10, str, 140, gfx.GFX_COL_GOLD);

			alg_gfx.gfx_draw_line(0, 36, 511, 36);

			alg_gfx.gfx_display_colour_text(16, 58, "Present System:", gfx.GFX_COL_GREEN_1);

			if (!elite.witchspace)
			{
				planet_name = Planet.name_planet(ref elite.docked_planet);
				planet_name = Planet.capitalise_name(planet_name);
				str = planet_name;
				alg_gfx.gfx_display_text(190, 58, str);
			}

			alg_gfx.gfx_display_colour_text(16, 74, "Hyperspace System:", gfx.GFX_COL_GREEN_1);
			planet_name = Planet.name_planet(ref elite.hyperspace_planet);
			planet_name = Planet.capitalise_name(planet_name);
			str = planet_name;
			alg_gfx.gfx_display_text(190, 74, str);

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

					if ((type == SHIP.SHIP_MISSILE) ||
						((type > SHIP.SHIP_ROCK) && (type < SHIP.SHIP_DODEC)))
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

			alg_gfx.gfx_display_colour_text(16, 90, "Condition:", gfx.GFX_COL_GREEN_1);
			alg_gfx.gfx_display_text(190, 90, condition_txt[condition]);

			str = $"{elite.cmdr.fuel / 10:d}.{elite.cmdr.fuel % 10:d} Light Years";
			alg_gfx.gfx_display_colour_text(16, 106, "Fuel:", gfx.GFX_COL_GREEN_1);
			alg_gfx.gfx_display_text(70, 106, str);

			str = $"{elite.cmdr.credits / 10:d}.{elite.cmdr.credits % 10:d} Cr";
			alg_gfx.gfx_display_colour_text(16, 122, "Cash:", gfx.GFX_COL_GREEN_1);
			alg_gfx.gfx_display_text(70, 122, str);

			if (elite.cmdr.legal_status == 0)
			{
				str = "Clean";
			}
			else
			{
				str = elite.cmdr.legal_status > 50 ? "Fugitive" : "Offender";
			}

			alg_gfx.gfx_display_colour_text(16, 138, "Legal Status:", gfx.GFX_COL_GREEN_1);
			alg_gfx.gfx_display_text(128, 138, str);

			for (i = 0; i < NO_OF_RANKS; i++)
			{
				if (elite.cmdr.score >= rating[i].score)
				{
					str = rating[i].title;
				}
			}

			alg_gfx.gfx_display_colour_text(16, 154, "Rating:", gfx.GFX_COL_GREEN_1);
			alg_gfx.gfx_display_text(80, 154, str);

			alg_gfx.gfx_display_colour_text(16, 186, "EQUIPMENT:", gfx.GFX_COL_GREEN_1);

			x = EQUIP_START_X;
			y = EQUIP_START_Y;

			if (elite.cmdr.cargo_capacity > 20)
			{
				alg_gfx.gfx_display_text(x, y, "Large Cargo Bay");
				y += Y_INC;
			}

			if (elite.cmdr.escape_pod)
			{
				alg_gfx.gfx_display_text(x, y, "Escape Pod");
				y += Y_INC;
			}

			if (elite.cmdr.fuel_scoop)
			{
				alg_gfx.gfx_display_text(x, y, "Fuel Scoops");
				y += Y_INC;
			}

			if (elite.cmdr.ecm)
			{
				alg_gfx.gfx_display_text(x, y, "E.C.M. System");
				y += Y_INC;
			}

			if (elite.cmdr.energy_bomb)
			{
				alg_gfx.gfx_display_text(x, y, "Energy Bomb");
				y += Y_INC;
			}

			if (elite.cmdr.energy_unit != 0)
			{
				alg_gfx.gfx_display_text(x, y, elite.cmdr.energy_unit == 1 ? "Extra Energy Unit" : "Naval Energy Unit");
				y += Y_INC;
				if (y > EQUIP_MAX_Y)
				{
					y = EQUIP_START_Y;
					x += EQUIP_WIDTH;
				}
			}

			if (elite.cmdr.docking_computer)
			{
				alg_gfx.gfx_display_text(x, y, "Docking Computers");
				y += Y_INC;
				if (y > EQUIP_MAX_Y)
				{
					y = EQUIP_START_Y;
					x += EQUIP_WIDTH;
				}
			}


			if (elite.cmdr.galactic_hyperdrive)
			{
				alg_gfx.gfx_display_text(x, y, "Galactic Hyperspace");
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
				alg_gfx.gfx_display_text(x, y, str);
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
				alg_gfx.gfx_display_text(x, y, str);
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
				alg_gfx.gfx_display_text(x, y, str);
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
				alg_gfx.gfx_display_text(x, y, str);
			}
		}

		/***********************************************************************************/

		static int hilite_item;
		static string[] unit_name = { "t", "kg", "g" };

		static void display_stock_price(int i)
		{
			int y;
			string str;

			y = i * 15 + 55;

			alg_gfx.gfx_display_text(16, y, trade.stock_market[i].name);

			alg_gfx.gfx_display_text(180, y, unit_name[trade.stock_market[i].units]);
			str = $"{trade.stock_market[i].current_price / 10:d}.{trade.stock_market[i].current_price % 10:d}";
			alg_gfx.gfx_display_text(256, y, str);

			if (trade.stock_market[i].current_quantity > 0)
			{
				str = $"{trade.stock_market[i].current_quantity:d}{unit_name[trade.stock_market[i].units]}";
			}
			else
			{
				str = "-";
			}

			alg_gfx.gfx_display_text(338, y, str);

			if (elite.cmdr.current_cargo[i] > 0)
			{
				str = $"{elite.cmdr.current_cargo[i]:d}{unit_name[trade.stock_market[i].units]}";
			}
			else
			{
				str = "-";
			}

			alg_gfx.gfx_display_text(444, y, str);
		}

		static void highlight_stock(int i)
		{
			int y;
			string str;

			if ((hilite_item != -1) && (hilite_item != i))
			{
				y = hilite_item * 15 + 55;
				alg_gfx.gfx_clear_area(2, y, 510, y + 15);
				display_stock_price(hilite_item);
			}

			y = i * 15 + 55;

			alg_gfx.gfx_draw_rectangle(2, y, 510, y + 15, gfx.GFX_COL_DARK_RED);
			display_stock_price(i);

			hilite_item = i;

			alg_gfx.gfx_clear_text_area();
			str = $"Cash: {elite.cmdr.credits / 10:d}.{elite.cmdr.credits % 10:d}";
			alg_gfx.gfx_display_text(16, 340, str);
		}

		internal static void select_previous_stock()
		{
			if ((!elite.docked) || (hilite_item == 0))
			{
				return;
			}

			highlight_stock(hilite_item - 1);
		}

		internal static void select_next_stock()
		{
			if ((!elite.docked) || (hilite_item == 16))
			{
				return;
			}

			highlight_stock(hilite_item + 1);
		}

		internal static void buy_stock()
		{
			int cargo_held;

			if (!elite.docked)
			{
				return;
			}

			stock_item item = trade.stock_market[hilite_item];

			if ((item.current_quantity == 0) ||
				(elite.cmdr.credits < item.current_price))
			{
				return;
			}

			cargo_held = trade.total_cargo();

			if ((item.units == trade.TONNES) &&
				(cargo_held == elite.cmdr.cargo_capacity))
			{
				return;
			}

			elite.cmdr.current_cargo[hilite_item]++;
			item.current_quantity--;
			elite.cmdr.credits -= item.current_price;

			highlight_stock(hilite_item);
		}

		internal static void sell_stock()
		{
			if ((!elite.docked) || (elite.cmdr.current_cargo[hilite_item] == 0))
			{
				return;
			}

			stock_item item = trade.stock_market[hilite_item];

			elite.cmdr.current_cargo[hilite_item]--;
			item.current_quantity++;
			elite.cmdr.credits += item.current_price;

			highlight_stock(hilite_item);
		}



		static void display_market_prices()
		{
			elite.current_screen = SCR.SCR_MARKET_PRICES;

			alg_gfx.gfx_clear_display();

			string planet_name = Planet.name_planet(ref elite.docked_planet);
			string str = planet_name + " MARKET PRICES";
			alg_gfx.gfx_display_centre_text(10, str, 140, gfx.GFX_COL_GOLD);

			alg_gfx.gfx_draw_line(0, 36, 511, 36);

			alg_gfx.gfx_display_colour_text(16, 40, "PRODUCT", gfx.GFX_COL_GREEN_1);
			alg_gfx.gfx_display_colour_text(166, 40, "UNIT", gfx.GFX_COL_GREEN_1);
			alg_gfx.gfx_display_colour_text(246, 40, "PRICE", gfx.GFX_COL_GREEN_1);
			alg_gfx.gfx_display_colour_text(314, 40, "FOR SALE", gfx.GFX_COL_GREEN_1);
			alg_gfx.gfx_display_colour_text(420, 40, "IN HOLD", gfx.GFX_COL_GREEN_1);

			for (int i = 0; i < 17; i++)
			{
				display_stock_price(i);
			}

			if (elite.docked)
			{
				hilite_item = -1;
				highlight_stock(0);
			}
		}

		static void display_inventory()
		{
			int i;
			int y;
			string str;

			elite.current_screen = SCR.SCR_INVENTORY;

			alg_gfx.gfx_clear_display();
			alg_gfx.gfx_display_centre_text(10, "INVENTORY", 140, gfx.GFX_COL_GOLD);
			alg_gfx.gfx_draw_line(0, 36, 511, 36);

			str = $"{elite.cmdr.fuel / 10:d}.{elite.cmdr.fuel % 10:d} Light Years";
			alg_gfx.gfx_display_colour_text(16, 50, "Fuel:", gfx.GFX_COL_GREEN_1);
			alg_gfx.gfx_display_text(70, 50, str);

			str = $"{elite.cmdr.credits / 10:d}.{elite.cmdr.credits % 10:d} Cr";
			alg_gfx.gfx_display_colour_text(16, 66, "Cash:", gfx.GFX_COL_GREEN_1);
			alg_gfx.gfx_display_text(70, 66, str);

			y = 98;
			for (i = 0; i < 17; i++)
			{
				if (elite.cmdr.current_cargo[i] > 0)
				{
					alg_gfx.gfx_display_text(16, y, trade.stock_market[i].name);

					str = $"{elite.cmdr.current_cargo[i]:d}{unit_name[trade.stock_market[i].units]}";

					alg_gfx.gfx_display_text(180, y, str);
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
					return (elite.cmdr.fuel >= 70);

				case equip_types.EQ_MISSILE:
					return (elite.cmdr.missiles >= 4);

				case equip_types.EQ_CARGO_BAY:
					return (elite.cmdr.cargo_capacity > 20);

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
					return (elite.cmdr.front_laser == elite.PULSE_LASER);

				case equip_types.EQ_REAR_PULSE:
					return (elite.cmdr.rear_laser == elite.PULSE_LASER);

				case equip_types.EQ_LEFT_PULSE:
					return (elite.cmdr.left_laser == elite.PULSE_LASER);

				case equip_types.EQ_RIGHT_PULSE:
					return (elite.cmdr.right_laser == elite.PULSE_LASER);

				case equip_types.EQ_FRONT_BEAM:
					return (elite.cmdr.front_laser == elite.BEAM_LASER);

				case equip_types.EQ_REAR_BEAM:
					return (elite.cmdr.rear_laser == elite.BEAM_LASER);

				case equip_types.EQ_LEFT_BEAM:
					return (elite.cmdr.left_laser == elite.BEAM_LASER);

				case equip_types.EQ_RIGHT_BEAM:
					return (elite.cmdr.right_laser == elite.BEAM_LASER);

				case equip_types.EQ_FRONT_MINING:
					return (elite.cmdr.front_laser == elite.MINING_LASER);

				case equip_types.EQ_REAR_MINING:
					return (elite.cmdr.rear_laser == elite.MINING_LASER);

				case equip_types.EQ_LEFT_MINING:
					return (elite.cmdr.left_laser == elite.MINING_LASER);

				case equip_types.EQ_RIGHT_MINING:
					return (elite.cmdr.right_laser == elite.MINING_LASER);

				case equip_types.EQ_FRONT_MILITARY:
					return (elite.cmdr.front_laser == elite.MILITARY_LASER);

				case equip_types.EQ_REAR_MILITARY:
					return (elite.cmdr.rear_laser == elite.MILITARY_LASER);

				case equip_types.EQ_LEFT_MILITARY:
					return (elite.cmdr.left_laser == elite.MILITARY_LASER);

				case equip_types.EQ_RIGHT_MILITARY:
					return (elite.cmdr.right_laser == elite.MILITARY_LASER);
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

			int col = equip_stock[i].canbuy ? gfx.GFX_COL_WHITE : gfx.GFX_COL_GREY_1;

			int x = equip_stock[i].name[0] == '>' ? 50 : 16;

			alg_gfx.gfx_display_colour_text(x, y, equip_stock[i].name[1..], col);

			if (equip_stock[i].price != 0)
			{
				str = $"{equip_stock[i].price / 10:d}.{equip_stock[i].price % 10:d}";
				alg_gfx.gfx_display_colour_text(338, y, str, col);
			}
		}

		static void highlight_equip(int i)
		{
			int y;
			string str;

			if ((hilite_item != -1) && (hilite_item != i))
			{
				y = equip_stock[hilite_item].y;
				alg_gfx.gfx_clear_area(2, y + 1, 510, y + 15);
				display_equip_price(hilite_item);
			}

			y = equip_stock[i].y;

			alg_gfx.gfx_draw_rectangle(2, y + 1, 510, y + 15, gfx.GFX_COL_DARK_RED);
			display_equip_price(i);

			hilite_item = i;

			alg_gfx.gfx_clear_text_area();
			str = $"Cash: {elite.cmdr.credits / 10:d}.{elite.cmdr.credits % 10:d}";
			alg_gfx.gfx_display_text(16, 340, str);
		}

		internal static void select_next_equip()
		{
			int next;
			int i;

			if (hilite_item == (NO_OF_EQUIP_ITEMS - 1))
				return;

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
				highlight_equip(next);
		}

		internal static void select_previous_equip()
		{
			int i;
			int prev;

			if (hilite_item == 0)
				return;

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

			alg_gfx.gfx_clear_area(2, 55, 510, 380);

			int tech_level = elite.current_planet_data.techlevel + 1;

			equip_stock[0].price = (70 - elite.cmdr.fuel) * 2;

			int y = 55;
			for (i = 0; i < NO_OF_EQUIP_ITEMS; i++)
			{
				equip_stock[i].canbuy = ((!equip_present(equip_stock[i].type)) && (equip_stock[i].price <= elite.cmdr.credits));

				if ((equip_stock[i].show) && (tech_level >= equip_stock[i].level))
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
				equip_stock[i].show = ((ch == ' ') || (ch == '+'));
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

		static void equip_ship()
		{
			elite.current_screen = SCR.SCR_EQUIP_SHIP;

			alg_gfx.gfx_clear_display();
			alg_gfx.gfx_display_centre_text(10, "EQUIP SHIP", 140, gfx.GFX_COL_GOLD);
			alg_gfx.gfx_draw_line(0, 36, 511, 36);

			collapse_equip_list();

			hilite_item = 0;

			list_equip_prices();
		}
	}
}