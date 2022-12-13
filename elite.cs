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

# include <stdlib.h>

# include "config.h"
# include "elite.h"
# include "vector.h"
# include "planet.h"
# include "shipdata.h"

namespace Elite
{
	using Elite.Enums;
	using Elite.Structs;
	using EliteLib.Structs;

	internal static class elite
	{
		internal const int PULSE_LASER = 0x0F;
		internal const int BEAM_LASER = 0x8F;
		internal const int MILITARY_LASER = 0x97;
		internal const int MINING_LASER = 0x32;

		internal const int MAX_UNIV_OBJECTS = 20;

        internal static galaxy_seed docked_planet;
		internal static galaxy_seed hyperspace_planet;
		internal static planet_data current_planet_data;

		static int curr_galaxy_num = 1;
		static int curr_fuel = 70;
		internal static int carry_flag = 0;
		internal static SCR current_screen = 0;
        internal static bool witchspace;

		internal static int wireframe = 0;
        internal static int anti_alias_gfx = 0;
        internal static bool hoopy_casinos = false;
        internal static int speed_cap = 75;
        internal static int instant_dock = 0;


        internal static string scanner_filename;
        internal static int scanner_cx;
        internal static int scanner_cy;
        internal static int compass_centre_x;
        internal static int compass_centre_y;

        internal static int planet_render_style = 0;

        internal static int game_over;
        internal static int docked;
        internal static int finish;
        internal static int flight_speed;
        internal static int flight_roll;
        internal static int flight_climb;
        internal static int front_shield;
        internal static int aft_shield;
        internal static int energy;
        internal static int laser_temp;
        internal static int detonate_bomb;
        internal static int auto_pilot;


		internal static commander saved_cmdr =
		{
			"JAMESON",									/* Name 			*/
			0,											/* Mission Number 	*/
			0x14,0xAD,									/* Ship X,Y			*/
			{0x4a, 0x5a, 0x48, 0x02, 0x53, 0xb7},		/* Galaxy Seed		*/
			1000,										/* Credits * 10		*/
			70,											/* Fuel	* 10		*/
			0,
			0,											/* Galaxy - 1		*/
			PULSE_LASER,								/* Front Laser		*/
			0,											/* Rear Laser		*/
			0,											/* Left Laser		*/
			0,											/* Right Laser		*/
			0, 0,
			20,											/* Cargo Capacity	*/
			{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},		/* Current Cargo	*/
			0,											/* ECM				*/
			0,											/* Fuel Scoop		*/
			0,											/* Energy Bomb		*/
			0,											/* Energy Unit		*/
			0,											/* Docking Computer */
			0,											/* Galactic H'Drive	*/
			0,											/* Escape Pod		*/
			0,0,0,0,
			3,											/* No. of Missiles	*/
			0,											/* Legal Status		*/
			{0x10, 0x0F, 0x11, 0x00, 0x03, 0x1C,		/* Station Stock	*/
			 0x0E, 0x00, 0x00, 0x0A, 0x00, 0x11,
			 0x3A, 0x07, 0x09, 0x08, 0x00},
			0,											/* Fluctuation		*/
			0,											/* Score			*/
			0x80										/* Saved			*/
		};

		internal static commander cmdr;

		internal static player_ship myship;

		internal static ship_data[] ship_list = new ship_data[shipdata.NO_OF_SHIPS + 1]
		{
			NULL,
			&missile_data,
			&coriolis_data,
			&esccaps_data,
			&alloy_data,
			&cargo_data,
			&boulder_data,
			&asteroid_data,
			&rock_data,
			&orbit_data,
			&transp_data,
			&cobra3a_data,
			&pythona_data,
			&boa_data,
			&anacnda_data,
			&hermit_data,
			&viper_data,
			&sidewnd_data,
			&mamba_data,
			&krait_data,
			&adder_data,
			&gecko_data,
			&cobra1_data,
			&worm_data,
			&cobra3b_data,
			&asp2_data,
			&pythonb_data,
			&ferdlce_data,
			&moray_data,
			&thargoid_data,
			&thargon_data,
			&constrct_data,
			&cougar_data,
			&dodec_data
		};

		static void restore_saved_commander()
		{
			cmdr = saved_cmdr;

			docked_planet = Planet.find_planet(cmdr.ship_x, cmdr.ship_y);
			hyperspace_planet = docked_planet;

			Planet.generate_planet_data(ref elite.current_planet_data, docked_planet);
			trade.generate_stock_market();
			trade.set_stock_quantities(cmdr.station_stock);
		}
	}
}