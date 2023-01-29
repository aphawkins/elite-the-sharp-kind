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
 * space.c
 *
 * This module handles all the flight system and management of the space universe.
 */

namespace Elite.Engine
{
    using System.Numerics;
    using Elite.Common.Enums;
	using Elite.Engine.Enums;
	using Elite.Engine.Ships;
	using Elite.Engine.Types;
	using Elite.Engine.Views;

	internal class Scanner
	{
		//      private static galaxy_seed destination_planet;
		//internal static bool hyper_ready;
		//      private static int hyper_countdown;
		//      private static string hyper_name;
		//      private static float hyper_distance;
		//      private static bool hyper_galactic;
		private univ_object[] _universe;
		private Dictionary<SHIP, int> _shipCount;

		internal Scanner(univ_object[] universe, Dictionary<SHIP, int> shipCount)
        { 
			_universe = universe;
            _shipCount = shipCount;

        }

        /*
		 * Update the scanner and draw all the lollipops.
		 */
        private void update_scanner()
		{
            GFX_COL colour;

			for (int i = 0; i < elite.MAX_UNIV_OBJECTS; i++)
			{
				if ((_universe[i].type <= 0) ||
                    _universe[i].flags.HasFlag(FLG.FLG_DEAD) ||
                    _universe[i].flags.HasFlag(FLG.FLG_CLOAKED))
				{
					continue;
				}

                float x = _universe[i].location.X / 256;
                float y = _universe[i].location.Y / 256;
                float z = _universe[i].location.Z / 256;

				float x1 = x;
				float y1 = -z / 4;
				float y2 = y1 - (y / 2);

				if ((y2 < -28) || (y2 > 28) ||
					(x1 < -50) || (x1 > 50))
				{
					continue;
				}

				x1 += elite.scanner_centre.X;
				y1 += elite.scanner_centre.Y;
				y2 += elite.scanner_centre.Y;

				colour = _universe[i].flags.HasFlag(FLG.FLG_HOSTILE) ? GFX_COL.GFX_COL_YELLOW_5 : GFX_COL.GFX_COL_WHITE;

				switch (_universe[i].type)
				{
					case SHIP.SHIP_MISSILE:
						colour = GFX_COL.UNKNOWN_1;
						break;

					case SHIP.SHIP_DODEC:
					case SHIP.SHIP_CORIOLIS:
						colour = GFX_COL.GFX_COL_GREEN_1;
						break;

					case SHIP.SHIP_VIPER:
						colour = GFX_COL.UNKNOWN_2;
						break;
				}

                elite.alg_gfx.DrawLine(new(x1 + 2, y2), new(x1 - 3, y2), colour);
                elite.alg_gfx.DrawLine(new(x1 + 2, y2 + 1), new(x1 - 3, y2 + 1), colour);
                elite.alg_gfx.DrawLine(new(x1 + 2, y2 + 2), new(x1 - 3, y2 + 2), colour);
                elite.alg_gfx.DrawLine(new(x1 + 2, y2 + 3), new(x1 - 3, y2 + 3), colour);

                elite.alg_gfx.DrawLine(new(x1, y1), new(x1, y2), colour);
                elite.alg_gfx.DrawLine(new(x1 + 1, y1), new(x1 + 1, y2), colour);
                elite.alg_gfx.DrawLine(new(x1 + 2, y1), new(x1 + 2, y2), colour);
			}
		}

        /*
		 * Update the compass which tracks the space station / planet.
		 */
        private void update_compass()
		{
			int un = 0;

			if (elite.witchspace)
			{
				return;
			}

			if (_shipCount[SHIP.SHIP_CORIOLIS] != 0 || _shipCount[SHIP.SHIP_DODEC] != 0)
			{
				un = 1;
			}

			Vector3 dest = VectorMaths.unit_vector(_universe[un].location);

			Vector2 compass = new(elite.compass_centre.X + (dest.X * 16), elite.compass_centre.Y + (dest.Y * -16));

			if (dest.Z < 0)
			{
                elite.alg_gfx.DrawImage(Image.DotRed, compass);
			}
			else
			{
                elite.alg_gfx.DrawImage(Image.GreenDot, compass);
			}

		}

        /*
		 * Display the speed bar.
		 */
        private void display_speed()
		{
			float sx = 417f;
            float sy = 384f + 9f;

			float len = (elite.flight_speed * 64 / elite.myship.max_speed) - 1;

            GFX_COL colour = (elite.flight_speed > (elite.myship.max_speed * 2 / 3)) ? GFX_COL.GFX_COL_DARK_RED : GFX_COL.GFX_COL_GOLD;

			for (int i = 0; i < 6; i++)
			{
                elite.alg_gfx.DrawLine(new(sx, sy + i), new(sx + len, sy + i), colour);
			}
		}

        /*
		 * Draw an indicator bar.
		 * Used for shields and energy banks.
		 */
        private void display_dial_bar(float len, Vector2 position)
		{
            elite.alg_gfx.DrawLine(new(position.X, position.Y + 384f), new(position.X + len, position.Y + 384f), GFX_COL.GFX_COL_GOLD);
			int i = 1;
            elite.alg_gfx.DrawLine(new(position.X, position.Y + i + 384f), new(position.X + len, position.Y + i + 384f), GFX_COL.GFX_COL_GOLD);

			for (i = 2; i < 7; i++)
			{
				elite.alg_gfx.DrawLine(new(position.X, position.Y + i + 384f), new(position.X + len, position.Y + i + 384f), GFX_COL.GFX_COL_YELLOW_1);
			}

            elite.alg_gfx.DrawLine(new(position.X, position.Y + i + 384f), new(position.X + len, position.Y + i + 384f), GFX_COL.GFX_COL_DARK_RED);
		}

        /*
		 * Display the current shield strengths.
		 */
        private void display_shields()
		{
			if (elite.front_shield > 3)
			{
				display_dial_bar(elite.front_shield / 4, new(31f, 7f));
			}

			if (elite.aft_shield > 3)
			{
				display_dial_bar(elite.aft_shield / 4, new(31f, 23));
			}
		}

        private void display_altitude()
		{
			if (elite.myship.altitude > 3)
			{
				display_dial_bar(elite.myship.altitude / 4, new(31f, 92f));
			}
		}

        private void display_cabin_temp()
		{
			if (elite.myship.cabtemp > 3)
			{
				display_dial_bar(elite.myship.cabtemp / 4f, new(31f, 60f));
			}
		}

        private void display_laser_temp()
		{
			if (elite.laser_temp > 0)
			{
				display_dial_bar(elite.laser_temp / 4, new(31f, 76f));
			}
		}

        /*
		 * Display the energy banks.
		 */
        private void display_energy()
		{
            float e1 = elite.energy > 64 ? 64 : elite.energy;
            float e2 = elite.energy > 128 ? 64 : elite.energy - 64;
            float e3 = elite.energy > 192 ? 64 : elite.energy - 128;
            float e4 = elite.energy - 192;

			if (e4 > 0)
			{
				display_dial_bar(e4, new(416f, 61f));
			}

			if (e3 > 0)
			{
				display_dial_bar(e3, new(416f, 79f));
			}

			if (e2 > 0)
			{
				display_dial_bar(e2, new(416f, 97f));
			}

			if (e1 > 0)
			{
				display_dial_bar(e1, new(416f, 115f));
			}
		}

        private void display_flight_roll()
		{
			float sx = 416;
			float sy = 384 + 9 + 14;

			float pos = sx - (elite.flight_roll * 28 / elite.myship.max_roll);
			pos += 32;

			for (int i = 0; i < 4; i++)
			{
                elite.alg_gfx.DrawLine(new(pos + i, sy), new(pos + i, sy + 7), GFX_COL.GFX_COL_GOLD);
			}
		}

        private void display_flight_climb()
		{
			float sx = 416;
			float sy = 384 + 9 + 14 + 16;

			float pos = sx + (elite.flight_climb * 28 / elite.myship.max_climb);
			pos += 32;

			for (int i = 0; i < 4; i++)
			{
                elite.alg_gfx.DrawLine(new(pos + i, sy), new(pos + i, sy + 7), GFX_COL.GFX_COL_GOLD);
			}
		}

        private void display_fuel()
		{
			if (elite.cmdr.fuel > 0)
			{
				display_dial_bar(elite.cmdr.fuel * 64 / elite.myship.max_fuel, new(31f, 44f));
			}
		}

        private void display_missiles()
		{
			if (elite.cmdr.missiles == 0)
			{
				return;
			}

			int nomiss = elite.cmdr.missiles > 4 ? 4 : elite.cmdr.missiles;

			Vector2 location = new(((4 - nomiss) * 16) + 35, 113 + 385);

			if (swat.missile_target != swat.MISSILE_UNARMED)
			{
                elite.alg_gfx.DrawImage((swat.missile_target < 0) ? Image.MissileYellow : Image.MissileRed, location);
                location.X += 16;
				nomiss--;
			}

			for (; nomiss > 0; nomiss--)
			{
                elite.alg_gfx.DrawImage(Image.MissileGreen, location);
                location.X += 16;
			}
		}

		internal void update_console()
		{
            elite.alg_gfx.SetClipRegion(0, 0, 512, 512);
            elite.draw.DrawScanner();

			display_speed();
			display_flight_climb();
			display_flight_roll();
			display_shields();
			display_altitude();
			display_energy();
			display_cabin_temp();
			display_laser_temp();
			display_fuel();
			display_missiles();

			if (elite.docked)
			{
				return;
			}

			update_scanner();
			update_compass();

			if (_shipCount[SHIP.SHIP_CORIOLIS] != 0 || _shipCount[SHIP.SHIP_DODEC] != 0)
			{
                elite.alg_gfx.DrawImage(Image.BigS, new(387, 490));
			}

			if (swat.ecm_active != 0)
			{
                elite.alg_gfx.DrawImage(Image.BigE, new(115, 490));
			}
		}
	}
}