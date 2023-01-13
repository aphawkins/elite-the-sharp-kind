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
	using Elite.Enums;
	using Elite.Structs;
	using Elite.Ships;
	using System.Numerics;

	public static class elite
	{
		internal static IGfx alg_gfx;
		internal static ISound sound;
        internal static IKeyboard keyboard;

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
		//TODO: used by gfx a rate limiter meaning this class has to be public. Find a better way.
		public static int speed_cap = 75;
		internal static bool instant_dock = false;

		internal static string scanner_filename;
		internal static Vector2 scanner_centre = new(253, 63 + 385);
		internal static Vector2 compass_centre = new(382, 22 + 385);

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

#if DEBUG
        internal static commander saved_cmdr = CommanderFactory.Max();
#else
		internal static commander saved_cmdr = CommanderFactory.Jameson();
#endif

        internal static commander cmdr;

		internal static player_ship myship;

		internal static Draw draw;

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