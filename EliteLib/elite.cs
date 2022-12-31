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

//# include <stdlib.h>

//# include "config.h"
//# include "elite.h"
//# include "vector.h"
//# include "planet.h"
//# include "shipdata.h"

namespace Elite
{
	using Elite.Enums;
	using Elite.Structs;
	using Elite.Ships;
	using System.Drawing;

	internal static class elite
	{
		internal static IGfx alg_gfx;

		internal const int PULSE_LASER = 0x0F;
		internal const int BEAM_LASER = 0x8F;
		internal const int MILITARY_LASER = 0x97;
		internal const int MINING_LASER = 0x32;

		internal const int MAX_UNIV_OBJECTS = 20;

		internal static galaxy_seed docked_planet;
		internal static galaxy_seed hyperspace_planet;
		internal static planet_data current_planet_data;

		//static int curr_galaxy_num = 1;
		//static int curr_fuel = 70;
		internal static int carry_flag = 0;
		internal static SCR current_screen = 0;
		internal static bool witchspace;

		internal static bool wireframe = false;
		internal static bool anti_alias_gfx = false;
		internal static bool hoopy_casinos = false;
		internal static int speed_cap = 75;
		internal static bool instant_dock = false;

		internal static string scanner_filename;
		internal static int scanner_cx;
		internal static int scanner_cy;
		internal static int compass_centre_x;
		internal static int compass_centre_y;

		internal static int planet_render_style = 0;

		internal static bool game_over;
		internal static bool docked;
		internal static bool finish;
		internal static int flight_speed;
		internal static int flight_roll;
		internal static int flight_climb;
		internal static int front_shield;
		internal static int aft_shield;
		internal static int energy;
		internal static int laser_temp;
		internal static bool detonate_bomb;
		internal static bool auto_pilot;

		internal static commander saved_cmdr = new commander(
			"JAMESON",                                  /* Name 			*/
			0,                                          /* Mission Number 	*/
			0x14, 0xAD,                                 /* Ship X,Y			*/
			new(0x4a, 0x5a, 0x48, 0x02, 0x53, 0xb7),        /* Galaxy Seed		*/
			1000,                                       /* Credits * 10		*/
			70,                                         /* Fuel	* 10		*/
			0,
			0,                                          /* Galaxy - 1		*/
			PULSE_LASER,                                /* Front Laser		*/
			0,                                          /* Rear Laser		*/
			0,                                          /* Left Laser		*/
			0,                                          /* Right Laser		*/
			0, 0,
			20,                                         /* Cargo Capacity	*/
			new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },        /* Current Cargo	*/
			false,                                          /* ECM				*/
			false,                                          /* Fuel Scoop		*/
			false,                                          /* Energy Bomb		*/
			0,                                          /* Energy Unit		*/
			false,                                          /* Docking Computer */
			false,                                          /* Galactic H'Drive	*/
			false,                                          /* Escape Pod		*/
			0, 0, 0, 0,
			3,                                          /* No. of Missiles	*/
			0,                                          /* Legal Status		*/
			new int[] {0x10, 0x0F, 0x11, 0x00, 0x03, 0x1C,		/* Station Stock	*/
			 0x0E, 0x00, 0x00, 0x0A, 0x00, 0x11,
			 0x3A, 0x07, 0x09, 0x08, 0x00},
			0,                                          /* Fluctuation		*/
			0,                                          /* Score			*/
			0x80                                        /* Saved			*/
		);

		internal static commander cmdr;

		internal static player_ship myship;

		internal static ship_data[] ship_list = new ship_data[shipdata.NO_OF_SHIPS + 1]
		{
			new(),
			shipdata.missile_data,
			shipdata.coriolis_data,
			shipdata.esccaps_data,
			shipdata.alloy_data,
			shipdata.cargo_data,
			shipdata.boulder_data,
			shipdata.asteroid_data,
			shipdata.rock_data,
			shipdata.orbit_data,
			shipdata.transp_data,
			shipdata.cobra3a_data,
			shipdata.pythona_data,
			shipdata.boa_data,
			shipdata.anacnda_data,
			shipdata.hermit_data,
			shipdata.viper_data,
			shipdata.sidewnd_data,
			shipdata.mamba_data,
			shipdata.krait_data,
			shipdata.adder_data,
			shipdata.gecko_data,
			shipdata.cobra1_data,
			shipdata.worm_data,
			shipdata.cobra3b_data,
			shipdata.asp2_data,
			shipdata.pythonb_data,
			shipdata.ferdlce_data,
			shipdata.moray_data,
			shipdata.thargoid_data,
			shipdata.thargon_data,
			shipdata.constrct_data,
			shipdata.cougar_data,
			shipdata.dodec_data
		};

		internal static void restore_saved_commander()
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