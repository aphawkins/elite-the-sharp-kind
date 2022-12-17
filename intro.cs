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

/*
 * intro.c
 *
 * Run the two intro screens.
 * First is a rolling Cobra MkIII.
 * Second is a parade of the various ships.
 *
 */


//# include <stdlib.h>

//# include "config.h"
//# include "elite.h"
//# include "gfx.h"
//# include "vector.h"
//# include "shipdata.h"
//# include "shipface.h"
//# include "threed.h"
//# include "space.h"
//# include "stars.h"

namespace Elite
{
    using Elite.Enums;
	using Elite.Ships;
	using Elite.Structs;

    internal static class intro
	{
		static int ship_no;
		static int show_time;
		static int direction;

		static int[] min_dist = new int[shipdata.NO_OF_SHIPS + 1]
		{
			0, 
			200, 800, 200, 200, 200, 300, 384, 200,
			200, 200, 420, 900, 500, 800, 384, 384,
			384, 384, 384, 200, 384, 384, 384,   0,
			384,   0, 384, 384, 700, 384,   0,   0,
			900
		};

		static Vector[] intro_ship_matrix;

		static void initialise_intro1()
		{
			swat.clear_universe();
            VectorMaths.set_init_matrix(intro_ship_matrix);
			swat.add_new_ship(SHIP.SHIP_COBRA3, 0, 0, 4500, intro_ship_matrix, -127, -127);
		}

		static void initialise_intro2()
		{
			ship_no = 0;
			show_time = 0;
			direction = 100;

			swat.clear_universe();
			create_new_stars();
            VectorMaths.set_init_matrix(intro_ship_matrix);
			swat.add_new_ship(SHIP.SHIP_MISSILE, 0, 0, 5000, intro_ship_matrix, -127, -127);
		}

		static void update_intro1()
		{
            space.universe[0].location.z -= 100;

			if (space.universe[0].location.z < 384)
			{
				space.universe[0].location.z = 384;
			}

            alg_gfx.gfx_clear_display();

            elite.flight_roll = 1;
			update_universe();

			gfx_draw_sprite(IMG_ELITE_TXT, -1, 10);

            alg_gfx.gfx_display_centre_text(310, "Original Game (C) I.Bell & D.Braben.", 120, GFX_COL_WHITE);
            alg_gfx.gfx_display_centre_text(330, "Re-engineered by C.J.Pinder.", 120, GFX_COL_WHITE);
            alg_gfx.gfx_display_centre_text(360, "Load New Commander (Y/N)?", 140, GFX_COL_GOLD);
		}

		static void update_intro2()
		{
			show_time++;

			if ((show_time >= 140) && (direction < 0))
			{
				direction = -direction;
			}

			space.universe[0].location.z += direction;

			if (space.universe[0].location.z < min_dist[ship_no])
			{
				space.universe[0].location.z = min_dist[ship_no];
			}

			if (space.universe[0].location.z > 4500)
			{
				do
				{
					ship_no++;
					if (ship_no > shipdata.NO_OF_SHIPS)
					{
						ship_no = 1;
					}
				} while (min_dist[ship_no] == 0);

				show_time = 0;
				direction = -100;

                space.ship_count[(int)space.universe[0].type] = 0;
                space.universe[0].type = 0;

				add_new_ship(ship_no, 0, 0, 4500, intro_ship_matrix, -127, -127);
			}

            alg_gfx.gfx_clear_display();
			update_starfield();
			update_universe();

			gfx_draw_sprite(IMG_ELITE_TXT, -1, 10);

            alg_gfx.gfx_display_centre_text(360, "Press Fire or Space, Commander.", 140, gfx.GFX_COL_GOLD);
            alg_gfx.gfx_display_centre_text(330, elite.ship_list[ship_no].name, 120, gfx.GFX_COL_WHITE);
		}
	}
}