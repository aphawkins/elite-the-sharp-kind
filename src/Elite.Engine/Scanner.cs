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
    using Elite.Engine.Types;

    internal class Scanner
	{
		private readonly IGfx _gfx;
		private readonly univ_object[] _universe;
		private readonly Dictionary<SHIP, int> _shipCount;

		internal Scanner(IGfx gfx, univ_object[] universe, Dictionary<SHIP, int> shipCount)
        { 
			_gfx = gfx;
			_universe = universe;
            _shipCount = shipCount;
        }

        /*
		 * Update the scanner and draw all the lollipops.
		 */
        private void update_scanner()
		{
			for (int i = 0; i < elite.MAX_UNIV_OBJECTS; i++)
			{
				if ((_universe[i].type <= 0) ||
                    _universe[i].flags.HasFlag(FLG.FLG_DEAD) ||
                    _universe[i].flags.HasFlag(FLG.FLG_CLOAKED))
				{
					continue;
				}

				float x = _universe[i].location.X / 256;
                float y1 = -_universe[i].location.Z / 1024;
				float y2 = y1 - (_universe[i].location.Y / 512);

				if ((y2 < -28) || (y2 > 28) ||
					(x < -50) || (x > 50))
				{
					continue;
				}

				x += elite.scanner_centre.X;
				y1 += elite.scanner_centre.Y;
				y2 += elite.scanner_centre.Y;

				GFX_COL colour = _universe[i].flags.HasFlag(FLG.FLG_HOSTILE) ? GFX_COL.GFX_COL_YELLOW_5 : GFX_COL.GFX_COL_WHITE;

				switch (_universe[i].type)
				{
					case SHIP.SHIP_MISSILE:
						colour = GFX_COL.GFX_COL_PINK_1;
						break;

					case SHIP.SHIP_DODEC:
					case SHIP.SHIP_CORIOLIS:
						colour = GFX_COL.GFX_COL_GREEN_1;
						break;

					case SHIP.SHIP_VIPER:
						colour = GFX_COL.GFX_COL_BLUE_4;
						break;
				}

				// ship
				_gfx.DrawRectangleFilled(x - 3, y2, 5, 3, colour);
				// stick
                _gfx.DrawRectangleFilled(x, y2 < y1 ? y2 : y1, 2, MathF.Abs(y2 - y1), colour);
            }
		}

		/// <summary>
		/// Update the compass which tracks the space station / planet.
		/// </summary>
        private void UpdateCompass()
		{
			if (elite.witchspace)
			{
				return;
			}

            int un = _shipCount[SHIP.SHIP_CORIOLIS] == 0 && _shipCount[SHIP.SHIP_DODEC] == 0 ? 0 : 1;
            Vector3 dest = VectorMaths.unit_vector(_universe[un].location);

			if (float.IsNaN(dest.X))
			{
				return;
			}

			Vector2 compass = new(elite.compass_centre.X + (dest.X * 16), elite.compass_centre.Y + (dest.Y * -16));

			if (dest.Z < 0)
			{
                _gfx.DrawImage(Image.DotRed, compass);
			}
			else
			{
                _gfx.DrawImage(Image.GreenDot, compass);
			}

		}

        /*
		 * Display the speed bar.
		 */
        private void display_speed()
		{
			float sx = 417;
            float sy = 384 + 9;

			float len = (elite.flight_speed * 64 / elite.myship.max_speed) - 1;

            GFX_COL colour = (elite.flight_speed > (elite.myship.max_speed * 2 / 3)) ? GFX_COL.GFX_COL_DARK_RED : GFX_COL.GFX_COL_GOLD;

			for (int i = 0; i < 6; i++)
			{
                _gfx.DrawLine(new(sx, sy + i), new(sx + len, sy + i), colour);
			}
		}

        /*
		 * Draw an indicator bar.
		 * Used for shields and energy banks.
		 */
        private void display_dial_bar(float len, Vector2 position)
		{
            _gfx.DrawLine(new(position.X, position.Y + 384f), new(position.X + len, position.Y + 384f), GFX_COL.GFX_COL_GOLD);
			int i = 1;
            _gfx.DrawLine(new(position.X, position.Y + i + 384f), new(position.X + len, position.Y + i + 384f), GFX_COL.GFX_COL_GOLD);

			for (i = 2; i < 7; i++)
			{
				_gfx.DrawLine(new(position.X, position.Y + i + 384f), new(position.X + len, position.Y + i + 384f), GFX_COL.GFX_COL_YELLOW_1);
			}

            _gfx.DrawLine(new(position.X, position.Y + i + 384f), new(position.X + len, position.Y + i + 384f), GFX_COL.GFX_COL_DARK_RED);
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
				display_dial_bar(elite.myship.altitude / 4, new(31, 92));
			}
		}

        private void display_cabin_temp()
		{
			if (elite.myship.cabtemp > 3)
			{
				display_dial_bar(elite.myship.cabtemp / 4, new(31, 60));
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
                _gfx.DrawLine(new(pos + i, sy), new(pos + i, sy + 7), GFX_COL.GFX_COL_GOLD);
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
                _gfx.DrawLine(new(pos + i, sy), new(pos + i, sy + 7), GFX_COL.GFX_COL_GOLD);
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
                _gfx.DrawImage((swat.missile_target < 0) ? Image.MissileYellow : Image.MissileRed, location);
                location.X += 16;
				nomiss--;
			}

			for (; nomiss > 0; nomiss--)
			{
                _gfx.DrawImage(Image.MissileGreen, location);
                location.X += 16;
			}
		}

		internal void update_console()
		{
            _gfx.SetClipRegion(0, 0, 512, 512);
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
			UpdateCompass();

			if (_shipCount[SHIP.SHIP_CORIOLIS] != 0 || _shipCount[SHIP.SHIP_DODEC] != 0)
			{
                _gfx.DrawImage(Image.BigS, new(387, 490));
			}

			if (swat.ecm_active != 0)
			{
                _gfx.DrawImage(Image.BigE, new(115, 490));
			}
		}
	}
}