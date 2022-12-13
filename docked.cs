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

        static string[] government_type = {	"Anarchy",
									"Feudal",
									"Multi-Government",
									"Dictatorship",
									"Communist",
									"Confederacy",
									"Democracy",
									"Corporate State"};

		internal static int cross_x = 0;
        internal static int cross_y = 0;

		static void draw_fuel_limit_circle (int cx, int cy)
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

		static int calc_distance_to_planet(galaxy_seed from_planet, galaxy_seed to_planet)
		{
			int dx,dy;
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

		static void show_distance (int ypos, galaxy_seed from_planet, galaxy_seed to_planet)
		{
			char str[100];
			int light_years;

			light_years = calc_distance_to_planet (from_planet, to_planet);
	
			if (light_years > 0)
				sprintf (str, "Distance: %2d.%d Light Years ", light_years / 10, light_years % 10);
			else
				strcpy (str,"                                                     ");

			gfx_display_text (16, ypos, str);
		}

		static void show_distance_to_planet ()
		{
			int px,py;
			char planet_name[16];
			char str[32];

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

			hyperspace_planet = Planet.find_planet(px, py);

			name_planet (planet_name, hyperspace_planet);

			gfx_clear_text_area();
			sprintf (str, "%-18s", planet_name);
			gfx_display_text (16, 340, str);

			show_distance (356, elite.docked_planet, hyperspace_planet);

			if (elite.current_screen == SCR.SCR_GALACTIC_CHART)
			{
				cross_x = hyperspace_planet.d * gfx.GFX_SCALE;
				cross_y = hyperspace_planet.b / (2 / gfx.GFX_SCALE) + (18 * gfx.GFX_SCALE) + 1;
			}
			else
			{
				cross_x = ((hyperspace_planet.d - elite.docked_planet.d) * (4 * gfx.GFX_SCALE)) + gfx.GFX_X_CENTRE;
				cross_y = ((hyperspace_planet.b - elite.docked_planet.b) * (2 * gfx.GFX_SCALE)) + gfx.GFX_Y_CENTRE;
			}
		}

		static void move_cursor_to_origin ()
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

		internal static void find_planet_by_name (string find_name)
		{
			int i;
			galaxy_seed glx;
			char planet_name[16];
			int found;
			char str[32];
	
			glx = elite.cmdr.galaxy;
			found = 0;
	
			for (i = 0; i < 256; i++)
			{
				name_planet (planet_name, glx);
		
				if (strcmp (planet_name, find_name) == 0)
				{
					found = 1;
					break;
				}

				waggle_galaxy (&glx);
				waggle_galaxy (&glx);
				waggle_galaxy (&glx);
				waggle_galaxy (&glx);
			}

			if (!found)
			{
				gfx_clear_text_area();
				gfx_display_text (16, 340, "Unknown Planet");
				return;
			}

			hyperspace_planet = glx;

			gfx_clear_text_area ();
			sprintf (str, "%-18s", planet_name);
			gfx_display_text (16, 340, str);

			show_distance (356, elite.docked_planet, hyperspace_planet);

			if (elite.current_screen == SCR.SCR_GALACTIC_CHART)
			{
				cross_x = hyperspace_planet.d * gfx.GFX_SCALE;
				cross_y = hyperspace_planet.b / (2 / gfx.GFX_SCALE) + (18 * gfx.GFX_SCALE) + 1;
			}
			else
			{
				cross_x = ((hyperspace_planet.d - elite.docked_planet.d) * (4 * gfx.GFX_SCALE)) + gfx.GFX_X_CENTRE;
				cross_y = ((hyperspace_planet.b - elite.docked_planet.b) * (2 * gfx.GFX_SCALE)) + gfx.GFX_Y_CENTRE;
			}
		}

		static void display_short_range_chart ()
		{
			int i;
			galaxy_seed glx;
			int dx,dy;
			int px,py;
			char planet_name[16];
			int row_used[64];
			int row;
			int blob_size;

            elite.current_screen = SCR.SCR_SHORT_RANGE;

			gfx_clear_display();

			gfx_display_centre_text (10, "SHORT RANGE CHART", 140, gfx.GFX_COL_GOLD);

            alg_gfx.gfx_draw_line(0, 36, 511, 36);

			draw_fuel_limit_circle (gfx.GFX_X_CENTRE, gfx.GFX_Y_CENTRE);

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
					waggle_galaxy (&glx);
					waggle_galaxy (&glx);
					waggle_galaxy (&glx);
					waggle_galaxy (&glx);

					continue;
				}

				px = (glx.d - elite.docked_planet.d);
				px = px * 4 * GFX_SCALE + GFX_X_CENTRE;  /* Convert to screen co-ords */

				py = (glx.b - elite.docked_planet.b);
				py = py * 2 * GFX_SCALE + GFX_Y_CENTRE;	/* Convert to screen co-ords */

				row = py / (8 * GFX_SCALE);

				if (row_used[row] == 1)
					row++;

				if (row_used[row] == 1)
					row -= 2;

				if (row <= 3)
				{
					waggle_galaxy (&glx);
					waggle_galaxy (&glx);
					waggle_galaxy (&glx);
					waggle_galaxy (&glx);

					continue;
				}

				if (row_used[row] == 0)
				{
					row_used[row] = 1;

					name_planet (planet_name, glx);
					capitalise_name (planet_name);

					gfx_display_text (px + (4 * GFX_SCALE), (row * 8 - 5) * GFX_SCALE, planet_name);
				}


				/* The next bit calculates the size of the circle used to represent */
				/* a planet.  The carry_flag is left over from the name generation. */
				/* Yes this was how it was done... don't ask :-( */

				blob_size = (glx.f & 1) + 2 + carry_flag;
				blob_size *= gfx.GFX_SCALE;
				gfx_draw_filled_circle (px, py, blob_size, gfx.GFX_COL_GOLD);

				waggle_galaxy (&glx);
				waggle_galaxy (&glx);
				waggle_galaxy (&glx);
				waggle_galaxy (&glx);
			}

			cross_x = ((hyperspace_planet.d - elite.docked_planet.d) * 4 * gfx.GFX_SCALE) + gfx.GFX_X_CENTRE;
			cross_y = ((hyperspace_planet.b - elite.docked_planet.b) * 2 * gfx.GFX_SCALE) + gfx.GFX_Y_CENTRE;
		}

		static void display_galactic_chart()
		{
			int i;
			galaxy_seed glx;
			char str[64];
			int px,py;

            elite.current_screen = SCR.SCR_GALACTIC_CHART;

			gfx_clear_display();

			sprintf (str, "GALACTIC CHART %d", elite.cmdr.galaxy_number + 1);

			gfx_display_centre_text (10, str, 140, gfx.GFX_COL_GOLD);

            alg_gfx.gfx_draw_line(0, 36, 511, 36);
            alg_gfx.gfx_draw_line(0, 36+258, 511, 36+258);

			draw_fuel_limit_circle(elite.docked_planet.d * gfx.GFX_SCALE, (elite.docked_planet.b / (2 / gfx.GFX_SCALE)) + (18 * gfx.GFX_SCALE) + 1);

			glx = elite.cmdr.galaxy;

			for (i = 0; i < 256; i++)
			{
				px = glx.d * gfx.GFX_SCALE;
				py = (glx.b / (2 / gfx.GFX_SCALE)) + (18 * gfx.GFX_SCALE) + 1;

				gfx_plot_pixel(px, py, gfx.GFX_COL_WHITE);

				if ((glx.e | 0x50) < 0x90)
					gfx_plot_pixel (px + 1, py, gfx.GFX_COL_WHITE);

				waggle_galaxy (&glx);
				waggle_galaxy (&glx);
				waggle_galaxy (&glx);
				waggle_galaxy (&glx);

			}


			cross_x = hyperspace_planet.d * GFX_SCALE;
			cross_y = (hyperspace_planet.b / (2 / GFX_SCALE)) + (18 * GFX_SCALE) + 1;
		}

		/*
		 * Displays data on the currently selected Hyperspace Planet.
		 */
		static void display_data_on_planet()
		{
			string planet_name;
			string str;
			string description;
			planet_data hyper_planet_data = new();

            elite.current_screen = SCR.SCR_PLANET_DATA;

			gfx_clear_display();

			name_planet (planet_name, hyperspace_planet);
			sprintf (str, "DATA ON %s", planet_name);

			gfx_display_centre_text (10, str, 140, gfx.GFX_COL_GOLD);

            alg_gfx.gfx_draw_line(0, 36, 511, 36);

			Planet.generate_planet_data(ref hyper_planet_data, hyperspace_planet);

			show_distance (42, elite.docked_planet, hyperspace_planet);

			sprintf (str, "Economy:%s", economy_type[hyper_planet_data.economy]);
			gfx_display_text (16, 74, str);

			sprintf (str, "Government:%s", government_type[hyper_planet_data.government]);
			gfx_display_text (16, 106, str);

			sprintf (str, "Tech.Level:%3d", hyper_planet_data.techlevel + 1);
			gfx_display_text (16, 138, str);

			sprintf (str, "Population:%d.%d Billion", hyper_planet_data.population / 10, hyper_planet_data.population % 10);
			gfx_display_text (16, 170, str);

			describe_inhabitants (str, hyperspace_planet);
			gfx_display_text (16, 202, str);

			sprintf (str, "Gross Productivity:%5d M CR", hyper_planet_data.productivity);
			gfx_display_text (16, 234, str);

			sprintf (str, "Average Radius:%5d km", hyper_planet_data.radius);
			gfx_display_text (16, 266, str);

			description = describe_planet (hyperspace_planet);
			gfx_display_pretty_text (16, 298, 400, 384, description);
		}



		struct rank
		{
			int score;
			char *title;
		};

		#define NO_OF_RANKS	9

		struct rank rating[NO_OF_RANKS] =
		{
			{0x0000, "Harmless"},
			{0x0008, "Mostly Harmless"},
			{0x0010, "Poor"},
			{0x0020, "Average"},
			{0x0040, "Above Average"},
			{0x0080, "Competent"},
			{0x0200, "Dangerous"},
			{0x0A00, "Deadly"},
			{0x1900, "---- E L I T E ---"}
		};

		char *laser_name[5] = {"Pulse", "Beam", "Military", "Mining", "Custom"};



		char *laser_type (int strength)
		{
			switch (strength)
			{
				case PULSE_LASER:
					return laser_name[0];

				case BEAM_LASER:
					return laser_name[1];
		
				case MILITARY_LASER:
					return laser_name[2];
		
				case MINING_LASER:
					return laser_name[3];
			}	

			return laser_name[4];
		}


		#define EQUIP_START_Y	202
		#define EQUIP_START_X	50
		#define EQUIP_MAX_Y		290
		#define EQUIP_WIDTH		200
		#define Y_INC			16


		static string[] condition_txt = new string[]
		{
			"Docked",
			"Green",
			"Yellow",
			"Red"
		};

		void display_commander_status ()
		{
			char planet_name[16];
			char str[100];
			int i;
			int x,y;
			int condition;
			int type;

            elite.current_screen = SCR.SCR_CMDR_STATUS;

			gfx_clear_display();

			sprintf (str, "COMMANDER %s", elite.cmdr.name);

			gfx_display_centre_text (10, str, 140, gfx.GFX_COL_GOLD);

            alg_gfx.gfx_draw_line(0, 36, 511, 36);


			gfx_display_colour_text (16, 58, "Present System:", gfx.GFX_COL_GREEN_1);
	
			if (!elite.witchspace)
			{
				name_planet (planet_name, elite.docked_planet);
				capitalise_name (planet_name);
				sprintf (str, "%s", planet_name);
				gfx_display_text (190, 58, str);
			}

			gfx_display_colour_text (16, 74, "Hyperspace System:", gfx.GFX_COL_GREEN_1);
			name_planet (planet_name, hyperspace_planet);
			capitalise_name (planet_name);
			sprintf (str, "%s", planet_name);
			gfx_display_text (190, 74, str);

			if (docked)
				condition = 0;
			else
			{
				condition = 1;

				for (i = 0; i < MAX_UNIV_OBJECTS; i++)
				{
					type = space.universe[i].type;
		
					if ((type == SHIP_MISSILE) ||
						((type > SHIP_ROCK) && (type < SHIP_DODEC)))
					{
						condition = 2;
						break;
					}
				}
 
				if ((condition == 2) && (energy < 128))
					condition = 3;
			}
	
			gfx_display_colour_text (16, 90, "Condition:", GFX_COL_GREEN_1);
			gfx_display_text (190, 90, condition_txt[condition]);

			sprintf (str, "%d.%d Light Years", elite.cmdr.fuel / 10, elite.cmdr.fuel % 10);
			gfx_display_colour_text (16, 106, "Fuel:", GFX_COL_GREEN_1);
			gfx_display_text (70, 106, str);

			sprintf (str, "%d.%d Cr", elite.cmdr.credits / 10, elite.cmdr.credits % 10);
			gfx_display_colour_text (16, 122, "Cash:", GFX_COL_GREEN_1);
			gfx_display_text (70, 122, str);

			if (elite.cmdr.legal_status == 0)
				strcpy (str, "Clean");
			else
				strcpy (str, elite.cmdr.legal_status > 50 ? "Fugitive" : "Offender");

			gfx_display_colour_text (16, 138, "Legal Status:", GFX_COL_GREEN_1);
			gfx_display_text (128, 138, str);

			for (i = 0; i < NO_OF_RANKS; i++)
				if (elite.cmdr.score >= rating[i].score)
					strcpy (str, rating[i].title);
	
			gfx_display_colour_text (16, 154, "Rating:", GFX_COL_GREEN_1);
			gfx_display_text (80, 154, str);

			gfx_display_colour_text (16, 186, "EQUIPMENT:", GFX_COL_GREEN_1);

			x = EQUIP_START_X;
			y = EQUIP_START_Y;

			if (elite.cmdr.cargo_capacity > 20)
			{
				gfx_display_text (x, y, "Large Cargo Bay");
				y += Y_INC;
			}
	
			if (elite.cmdr.escape_pod)
			{
				gfx_display_text (x, y, "Escape Pod");
				y += Y_INC;
			}
	
			if (elite.cmdr.fuel_scoop)
			{
				gfx_display_text (x, y, "Fuel Scoops");
				y += Y_INC;
			}

			if (elite.cmdr.ecm)
			{
				gfx_display_text (x, y, "E.C.M. System");
				y += Y_INC;
			}

			if (elite.cmdr.energy_bomb)
			{
				gfx_display_text (x, y, "Energy Bomb");
				y += Y_INC;
			}

			if (elite.cmdr.energy_unit)
			{
				gfx_display_text (x, y,
						  elite.cmdr.energy_unit == 1 ? "Extra Energy Unit" :"Naval Energy Unit");
				y += Y_INC;
				if (y > EQUIP_MAX_Y)
				{
					y = EQUIP_START_Y;
					x += EQUIP_WIDTH;
				}
			}

			if (elite.cmdr.docking_computer)
			{
				gfx_display_text (x, y, "Docking Computers");
				y += Y_INC;
				if (y > EQUIP_MAX_Y)
				{
					y = EQUIP_START_Y;
					x += EQUIP_WIDTH;
				}
			}

	
			if (elite.cmdr.galactic_hyperdrive)
			{
				gfx_display_text (x, y, "Galactic Hyperspace");
				y += Y_INC;
				if (y > EQUIP_MAX_Y)
				{
					y = EQUIP_START_Y;
					x += EQUIP_WIDTH;
				}
			}

			if (elite.cmdr.front_laser)
			{
				sprintf (str, "Front %s Laser", laser_type(elite.cmdr.front_laser));
				gfx_display_text (x, y, str);
				y += Y_INC;
				if (y > EQUIP_MAX_Y)
				{
					y = EQUIP_START_Y;
					x += EQUIP_WIDTH;
				}
			}
	
			if (elite.cmdr.rear_laser)
			{
				sprintf (str, "Rear %s Laser", laser_type(elite.cmdr.rear_laser));
				gfx_display_text (x, y, str);
				y += Y_INC;
				if (y > EQUIP_MAX_Y)
				{
					y = EQUIP_START_Y;
					x += EQUIP_WIDTH;
				}
			}

			if (elite.cmdr.left_laser)
			{
				sprintf (str, "Left %s Laser", laser_type(elite.cmdr.left_laser));
				gfx_display_text (x, y, str);
				y += Y_INC;
				if (y > EQUIP_MAX_Y)
				{
					y = EQUIP_START_Y;
					x += EQUIP_WIDTH;
				}
			}

			if (elite.cmdr.right_laser)
			{
				sprintf (str, "Right %s Laser", laser_type(elite.cmdr.right_laser));
				gfx_display_text (x, y, str);
			}
		}



		/***********************************************************************************/

		static int hilite_item;
		static char *unit_name[] = {"t", "kg", "g"};


		void display_stock_price (int i)
		{
			int y;
			char str[100];

			y = i * 15 + 55;

			gfx_display_text (16, y, stock_market[i].name);

			gfx_display_text (180, y, unit_name[stock_market[i].units]);
			sprintf (str, "%d.%d", stock_market[i].current_price / 10,
								   stock_market[i].current_price % 10);
			gfx_display_text (256, y, str);

			if (stock_market[i].current_quantity > 0)
				sprintf (str, "%d%s", stock_market[i].current_quantity,
									  unit_name[stock_market[i].units]);
			else
				strcpy (str, "-");

			gfx_display_text (338, y, str);

			if (elite.cmdr.current_cargo[i] > 0)
				sprintf (str, "%d%s", elite.cmdr.current_cargo[i],
									  unit_name[stock_market[i].units]);
			else
				strcpy (str, "-");

			gfx_display_text (444, y, str);
		}


		void highlight_stock (int i)
		{
			int y;
			char str[30];
	
			if ((hilite_item != -1) && (hilite_item != i))
			{
				y = hilite_item * 15 + 55;
				gfx_clear_area (2, y, 510, y + 15);
				display_stock_price (hilite_item);		
			}

			y = i * 15 + 55;
	
			gfx_draw_rectangle (2, y, 510, y + 15, GFX_COL_DARK_RED);
			display_stock_price (i);		

			hilite_item = i;

			gfx_clear_text_area();
			sprintf (str, "Cash: %d.%d", elite.cmdr.credits / 10, elite.cmdr.credits % 10);
			gfx_display_text (16, 340, str);
		}

		void select_previous_stock ()
		{
			if ((!docked) || (hilite_item == 0))
				return;

			highlight_stock (hilite_item - 1);
		}

		void select_next_stock ()
		{
			if ((!docked) || (hilite_item == 16))
				return;

			highlight_stock (hilite_item + 1);
		}


		void buy_stock ()
		{
			stock_item *item;
			int cargo_held;
	
			if (!docked)
				return;

			item = &stock_market[hilite_item];
		
			if ((item.current_quantity == 0) ||
				(elite.cmdr.credits < item.current_price))
				return;

			cargo_held = trade.total_cargo();
	
			if ((item.units == trade.TONNES) &&
				(cargo_held == elite.cmdr.cargo_capacity))
				return;
	
			elite.cmdr.current_cargo[hilite_item]++;
			item.current_quantity--;
			elite.cmdr.credits -= item.current_price;	

			highlight_stock (hilite_item);
		}


		void sell_stock ()
		{
			stock_item *item;
	
			if ((!docked) || (elite.cmdr.current_cargo[hilite_item] == 0))
				return;

			item = &stock_market[hilite_item];

			elite.cmdr.current_cargo[hilite_item]--;
			item.current_quantity++;
			elite.cmdr.credits += item.current_price;	

			highlight_stock (hilite_item);
		}



		static void display_market_prices ()
		{
			char str[100];
			char planet_name[16];
			int i;

            elite.current_screen = SCR.SCR_MARKET_PRICES;

			gfx_clear_display();

			name_planet (planet_name, elite.docked_planet);
			sprintf (str, "%s MARKET PRICES", planet_name);
			gfx_display_centre_text (10, str, 140, gfx.GFX_COL_GOLD);

            alg_gfx.gfx_draw_line(0, 36, 511, 36);

			gfx_display_colour_text (16,  40, "PRODUCT", gfx.GFX_COL_GREEN_1);
			gfx_display_colour_text (166, 40, "UNIT", gfx.GFX_COL_GREEN_1);
			gfx_display_colour_text (246, 40, "PRICE", gfx.GFX_COL_GREEN_1);
			gfx_display_colour_text (314, 40, "FOR SALE", gfx.GFX_COL_GREEN_1);
			gfx_display_colour_text (420, 40, "IN HOLD", gfx.GFX_COL_GREEN_1);

			for (i = 0; i < 17; i++)
			{
				display_stock_price (i);
			}

			if (docked)
			{
				hilite_item = -1;
				highlight_stock (0);
			}
		}


		void display_inventory ()
		{
			int i;
			int y;
			char str[80];

            elite.current_screen = SCR.SCR_INVENTORY;

			gfx_clear_display();
			gfx_display_centre_text (10, "INVENTORY", 140, gfx.GFX_COL_GOLD);
            alg_gfx.gfx_draw_line(0, 36, 511, 36);
	
			sprintf (str, "%d.%d Light Years", elite.cmdr.fuel / 10, elite.cmdr.fuel % 10);
			gfx_display_colour_text (16, 50, "Fuel:", gfx.GFX_COL_GREEN_1);
			gfx_display_text (70, 50, str);

			sprintf (str, "%d.%d Cr", elite.cmdr.credits / 10, elite.cmdr.credits % 10);
			gfx_display_colour_text (16, 66, "Cash:", gfx.GFX_COL_GREEN_1);
			gfx_display_text (70, 66, str);
	
			y = 98;
			for (i = 0; i < 17; i++)
			{
				if (elite.cmdr.current_cargo[i] > 0)
				{
					gfx_display_text (16, y, stock_market[i].name);

					sprintf (str, "%d%s", elite.cmdr.current_cargo[i],
									  unit_name[stock_market[i].units]);

					gfx_display_text (180, y, str);
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
	
		

		#define NO_OF_EQUIP_ITEMS	34

		struct equip_item
		{
			int canbuy;
			int y;
			int show;
			int level;
			int price;
			char *name;
			int type;
		};

		struct equip_item equip_stock[NO_OF_EQUIP_ITEMS] =
		{
			{0, 0, 1, 1,     2, " Fuel",					EQ_FUEL},
			{0, 0, 1, 1,   300, " Missile",					EQ_MISSILE},
			{0, 0, 1, 1,  4000, " Large Cargo Bay",			EQ_CARGO_BAY},
			{0, 0, 1, 2,  6000, " E.C.M. System",			EQ_ECM},
			{0, 0, 1, 5,  5250, " Fuel Scoops",				EQ_FUEL_SCOOPS},
			{0, 0, 1, 6, 10000, " Escape Pod",				EQ_ESCAPE_POD},
			{0, 0, 1, 7,  9000, " Energy Bomb",				EQ_ENERGY_BOMB},
			{0, 0, 1, 8, 15000, " Extra Energy Unit",		EQ_ENERGY_UNIT},
			{0, 0, 1, 9, 15000, " Docking Computers",		EQ_DOCK_COMP},
			{0, 0, 1,10, 50000, " Galactic Hyperdrive",		EQ_GAL_DRIVE},
			{0, 0, 0, 3,  4000, "+Pulse Laser",				EQ_PULSE_LASER},
			{0, 0, 1, 3,     0, "-Pulse Laser",				EQ_PULSE_LASER},
			{0, 0, 1, 3,  4000, ">Front",					EQ_FRONT_PULSE},
			{0, 0, 1, 3,  4000, ">Rear",					EQ_REAR_PULSE},
			{0, 0, 1, 3,  4000, ">Left",					EQ_LEFT_PULSE},
			{0, 0, 1, 3,  4000, ">Right",					EQ_RIGHT_PULSE},
			{0, 0, 1, 4, 10000, "+Beam Laser",				EQ_BEAM_LASER},
			{0, 0, 0, 4,     0, "-Beam Laser",				EQ_BEAM_LASER},
			{0, 0, 0, 4, 10000, ">Front",					EQ_FRONT_BEAM},
			{0, 0, 0, 4, 10000, ">Rear",					EQ_REAR_BEAM},
			{0, 0, 0, 4, 10000, ">Left",					EQ_LEFT_BEAM},
			{0, 0, 0, 4, 10000, ">Right",					EQ_RIGHT_BEAM},
			{0, 0, 1,10,  8000, "+Mining Laser",			EQ_MINING_LASER},
			{0, 0, 0,10,     0, "-Mining Laser",			EQ_MINING_LASER},
			{0, 0, 0,10,  8000, ">Front",					EQ_FRONT_MINING},
			{0, 0, 0,10,  8000, ">Rear",					EQ_REAR_MINING},
			{0, 0, 0,10,  8000, ">Left",					EQ_LEFT_MINING},
			{0, 0, 0,10,  8000, ">Right",					EQ_RIGHT_MINING},
			{0, 0, 1,10, 60000, "+Military Laser",			EQ_MILITARY_LASER},
			{0, 0, 0,10,     0, "-Military Laser",			EQ_MILITARY_LASER},
			{0, 0, 0,10, 60000, ">Front",					EQ_FRONT_MILITARY},
			{0, 0, 0,10, 60000, ">Rear",					EQ_REAR_MILITARY},
			{0, 0, 0,10, 60000, ">Left",					EQ_LEFT_MILITARY},
			{0, 0, 0,10, 60000, ">Right",					EQ_RIGHT_MILITARY}
		};


		int equip_present (int type)
		{
			switch (type)
			{
				case EQ_FUEL:
					return (elite.cmdr.fuel >= 70);
		
				case EQ_MISSILE:
					return (elite.cmdr.missiles >= 4);
		
				case EQ_CARGO_BAY:
					return (elite.cmdr.cargo_capacity > 20);
		
				case EQ_ECM:
					return elite.cmdr.ecm;
		
				case EQ_FUEL_SCOOPS:
					return elite.cmdr.fuel_scoop;
		
				case EQ_ESCAPE_POD:
					return elite.cmdr.escape_pod;
		
				case EQ_ENERGY_BOMB:
					return elite.cmdr.energy_bomb;

				case EQ_ENERGY_UNIT:
					return elite.cmdr.energy_unit;
			
				case EQ_DOCK_COMP:
					return elite.cmdr.docking_computer;
			
				case EQ_GAL_DRIVE:
					return elite.cmdr.galactic_hyperdrive;
			
				case EQ_FRONT_PULSE:
					return (elite.cmdr.front_laser == PULSE_LASER);
		
				case EQ_REAR_PULSE:
					return (elite.cmdr.rear_laser == PULSE_LASER);

				case EQ_LEFT_PULSE:
					return (elite.cmdr.left_laser == PULSE_LASER);

				case EQ_RIGHT_PULSE:
					return (elite.cmdr.right_laser == PULSE_LASER);

				case EQ_FRONT_BEAM:
					return (elite.cmdr.front_laser == BEAM_LASER);

				case EQ_REAR_BEAM:
					return (elite.cmdr.rear_laser == BEAM_LASER);

				case EQ_LEFT_BEAM:
					return (elite.cmdr.left_laser == BEAM_LASER);

				case EQ_RIGHT_BEAM:
					return (elite.cmdr.right_laser == BEAM_LASER);

				case EQ_FRONT_MINING:
					return (elite.cmdr.front_laser == MINING_LASER);

				case EQ_REAR_MINING:
					return (elite.cmdr.rear_laser == MINING_LASER);

				case EQ_LEFT_MINING:
					return (elite.cmdr.left_laser == MINING_LASER);

				case EQ_RIGHT_MINING:
					return (elite.cmdr.right_laser == MINING_LASER);

				case EQ_FRONT_MILITARY:
					return (elite.cmdr.front_laser == MILITARY_LASER);

				case EQ_REAR_MILITARY:
					return (elite.cmdr.rear_laser == MILITARY_LASER);

				case EQ_LEFT_MILITARY:
					return (elite.cmdr.left_laser == MILITARY_LASER);

				case EQ_RIGHT_MILITARY:
					return (elite.cmdr.right_laser == MILITARY_LASER);
			}

			return 0;
		}


		void display_equip_price (int i)
		{
			int x, y;
			int col;
			char str[100];
	
			y = equip_stock[i].y;
			if (y == 0)
				return;

			col = equip_stock[i].canbuy ? GFX_COL_WHITE : GFX_COL_GREY_1;

			x = *(equip_stock[i].name) == '>' ? 50 : 16; 

			gfx_display_colour_text (x, y, &equip_stock[i].name[1], col);

			if (equip_stock[i].price != 0)
			{
				sprintf (str, "%d.%d", equip_stock[i].price / 10, equip_stock[i].price % 10);
				gfx_display_colour_text (338, y, str, col);
			}
		}


		void highlight_equip (int i)
		{
			int y;
			char str[30];
	
			if ((hilite_item != -1) && (hilite_item != i))
			{
				y = equip_stock[hilite_item].y;
				gfx_clear_area (2, y+1, 510, y + 15);
				display_equip_price (hilite_item);		
			}

			y = equip_stock[i].y;
	
			gfx_draw_rectangle (2, y+1, 510, y + 15, GFX_COL_DARK_RED);
			display_equip_price (i);		

			hilite_item = i;

			gfx_clear_text_area();
			sprintf (str, "Cash: %d.%d", elite.cmdr.credits / 10, elite.cmdr.credits % 10);
			gfx_display_text (16, 340, str);
		}


		void select_next_equip ()
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
				highlight_equip (next);
		}

		void select_previous_equip ()
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
				highlight_equip (prev);
		}


		void list_equip_prices ()
		{
			int i;
			int y;
			int tech_level;

			gfx_clear_area (2, 55, 510, 380);
	
			tech_level = elite.current_planet_data.techlevel + 1;

			equip_stock[0].price = (70 - elite.cmdr.fuel) * 2;
	
			y = 55;
			for (i = 0; i < NO_OF_EQUIP_ITEMS; i++)
			{
				equip_stock[i].canbuy = ((equip_present (equip_stock[i].type) == 0) &&
										 (equip_stock[i].price <= elite.cmdr.credits));
	
				if (equip_stock[i].show && (tech_level >= equip_stock[i].level))
				{
					equip_stock[i].y = y;
					y += 15;
				}
				else
					equip_stock[i].y = 0;

				display_equip_price (i);
			}
	
			i = hilite_item;
			hilite_item = -1;
			highlight_equip (i);
		}


		void collapse_equip_list ()
		{
			int i;
			int ch;
	
			for (i = 0; i < NO_OF_EQUIP_ITEMS; i++)
			{
				ch = *(equip_stock[i].name);
				equip_stock[i].show = ((ch == ' ') || (ch == '+'));
			}
		}


		int laser_refund (int laser_type)
		{
			switch (laser_type)
			{
				case PULSE_LASER:
					return 4000;
		
				case BEAM_LASER:
					return 10000;
		
				case MILITARY_LASER:
					return 60000;
		
				case MINING_LASER:
					return 8000;
			}

			return 0;
		}


		void buy_equip ()
		{
			int i;

			if (equip_stock[hilite_item].name[0] == '+')
			{
				collapse_equip_list();
				equip_stock[hilite_item].show = 0;
				hilite_item++;
				for (i = 0; i < 5; i++)
					equip_stock[hilite_item + i].show = 1;
		
				list_equip_prices();
				return;		
			}

			if (equip_stock[hilite_item].canbuy == 0)
				return;
	
			switch (equip_stock[hilite_item].type)
			{
				case EQ_FUEL:
					elite.cmdr.fuel = myship.max_fuel;
					update_console();
					break;

				case EQ_MISSILE:
					elite.cmdr.missiles++;
					update_console();
					break;
		
				case EQ_CARGO_BAY:
					elite.cmdr.cargo_capacity = 35;
					break;
		
				case EQ_ECM:
					elite.cmdr.ecm = 1;
					break;
		
				case EQ_FUEL_SCOOPS:
					elite.cmdr.fuel_scoop = 1;
					break;
		
				case EQ_ESCAPE_POD:
					elite.cmdr.escape_pod = 1;
					break;
		
				case EQ_ENERGY_BOMB:
					elite.cmdr.energy_bomb = 1;
					break;

				case EQ_ENERGY_UNIT:
					elite.cmdr.energy_unit = 1;
					break;
			
				case EQ_DOCK_COMP:
					elite.cmdr.docking_computer = 1;
					break;
			
				case EQ_GAL_DRIVE:
					elite.cmdr.galactic_hyperdrive = 1;
					break;
			
				case EQ_FRONT_PULSE:
					elite.cmdr.credits += laser_refund (elite.cmdr.front_laser);
					elite.cmdr.front_laser = PULSE_LASER;
					break;
		
				case EQ_REAR_PULSE:
					elite.cmdr.credits += laser_refund (elite.cmdr.rear_laser);
					elite.cmdr.rear_laser = PULSE_LASER;
					break;

				case EQ_LEFT_PULSE:
					elite.cmdr.credits += laser_refund (elite.cmdr.left_laser);
					elite.cmdr.left_laser = PULSE_LASER;
					break;

				case EQ_RIGHT_PULSE:
					elite.cmdr.credits += laser_refund (elite.cmdr.right_laser);
					elite.cmdr.right_laser = PULSE_LASER;
					break;

				case EQ_FRONT_BEAM:
					elite.cmdr.credits += laser_refund (elite.cmdr.front_laser);
					elite.cmdr.front_laser = BEAM_LASER;
					break;

				case EQ_REAR_BEAM:
					elite.cmdr.credits += laser_refund (elite.cmdr.rear_laser);
					elite.cmdr.rear_laser = BEAM_LASER;
					break;

				case EQ_LEFT_BEAM:
					elite.cmdr.credits += laser_refund (elite.cmdr.left_laser);
					elite.cmdr.left_laser = BEAM_LASER;
					break;

				case EQ_RIGHT_BEAM:
					elite.cmdr.credits += laser_refund (elite.cmdr.right_laser);
					elite.cmdr.right_laser = BEAM_LASER;
					break;

				case EQ_FRONT_MINING:
					elite.cmdr.credits += laser_refund (elite.cmdr.front_laser);
					elite.cmdr.front_laser = MINING_LASER;
					break;

				case EQ_REAR_MINING:
					elite.cmdr.credits += laser_refund (elite.cmdr.rear_laser);
					elite.cmdr.rear_laser = MINING_LASER;
					break;

				case EQ_LEFT_MINING:
					elite.cmdr.credits += laser_refund (elite.cmdr.left_laser);
					elite.cmdr.left_laser = MINING_LASER;
					break;

				case EQ_RIGHT_MINING:
					elite.cmdr.credits += laser_refund (elite.cmdr.right_laser);
					elite.cmdr.right_laser = MINING_LASER;
					break;

				case EQ_FRONT_MILITARY:
					elite.cmdr.credits += laser_refund (elite.cmdr.front_laser);
					elite.cmdr.front_laser = MILITARY_LASER;
					break;

				case EQ_REAR_MILITARY:
					elite.cmdr.credits += laser_refund (elite.cmdr.rear_laser);
					elite.cmdr.rear_laser = MILITARY_LASER;
					break;

				case EQ_LEFT_MILITARY:
					elite.cmdr.credits += laser_refund (elite.cmdr.left_laser);
					elite.cmdr.left_laser = MILITARY_LASER;
					break;

				case EQ_RIGHT_MILITARY:
					elite.cmdr.credits += laser_refund (elite.cmdr.right_laser);
					elite.cmdr.right_laser = MILITARY_LASER;
					break;
			}

			elite.cmdr.credits -= equip_stock[hilite_item].price;
			list_equip_prices();
		}


		static void equip_ship ()
		{
			elite.current_screen = SCR.SCR_EQUIP_SHIP;

			gfx_clear_display();
			gfx_display_centre_text (10, "EQUIP SHIP", 140, gfx.GFX_COL_GOLD);
			alg_gfx.gfx_draw_line(0, 36, 511, 36);

			collapse_equip_list();
	
			hilite_item = 0;
	
			list_equip_prices();
		}
	}
}