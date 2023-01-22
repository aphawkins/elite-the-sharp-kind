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

namespace Elite
{
	using System.Numerics;
	using Elite.Enums;
	using Elite.Ships;
	using Elite.Structs;
	using Elite.Views;

	internal static class space
	{
        private static galaxy_seed destination_planet;
		internal static bool hyper_ready;
        private static int hyper_countdown;
        private static string hyper_name;
        private static float hyper_distance;
        private static bool hyper_galactic;
		internal static univ_object[] universe = new univ_object[elite.MAX_UNIV_OBJECTS];
		internal static int[] ship_count = new int[shipdata.NO_OF_SHIPS + 1];  /* many */

        private static void rotate_x_first(ref float a, ref float b, int direction)
		{
			float fx = a;
			float ux = b;

			if (direction < 0)
			{
				a = fx - (fx / 512) + (ux / 19);
				b = ux - (ux / 512) - (fx / 19);
			}
			else
			{
				a = fx - (fx / 512) - (ux / 19);
				b = ux - (ux / 512) + (fx / 19);
			}
		}

        /*
		 * Update an objects location in the universe.
		 */
        private static void move_univ_object(ref univ_object obj)
		{
			float x, y, z;
			float k2;
			float alpha;
			float beta;
			int rotx, rotz;
			float speed;

			alpha = elite.flight_roll / 256.0f;
			beta = elite.flight_climb / 256.0f;

			x = obj.location.X;
			y = obj.location.Y;
			z = obj.location.Z;

			if (!obj.flags.HasFlag(FLG.FLG_DEAD))
			{
				if (obj.velocity != 0)
				{
					speed = obj.velocity;
					speed *= 1.5f;
					x += obj.rotmat[2].X * speed;
					y += obj.rotmat[2].Y * speed;
					z += obj.rotmat[2].Z * speed;
				}

				if (obj.acceleration != 0)
				{
					obj.velocity += obj.acceleration;
					obj.acceleration = 0;
					if (obj.velocity > elite.ship_list[(int)obj.type].velocity)
					{
						obj.velocity = elite.ship_list[(int)obj.type].velocity;
					}

					if (obj.velocity <= 0)
					{
						obj.velocity = 1;
					}
				}
			}

			k2 = y - (alpha * x);
			z += beta * k2;
			y = k2 - (z * beta);
			x += alpha * y;

			z -= elite.flight_speed;

			obj.location.X = x;
			obj.location.Y = y;
			obj.location.Z = z;

			obj.distance = obj.location.Length();

			if (obj.type == SHIP.SHIP_PLANET)
			{
				beta = 0.0f;
			}

            VectorMaths.rotate_vec(ref obj.rotmat, alpha, beta);

			if (obj.flags.HasFlag(FLG.FLG_DEAD))
			{
				return;
			}

			rotx = obj.rotx;
			rotz = obj.rotz;

			/* If necessary rotate the object around the X axis... */

			if (rotx != 0)
			{
				rotate_x_first(ref obj.rotmat[2].X, ref obj.rotmat[1].X, rotx);
				rotate_x_first(ref obj.rotmat[2].Y, ref obj.rotmat[1].Y, rotx);
				rotate_x_first(ref obj.rotmat[2].Z, ref obj.rotmat[1].Z, rotx);

				if (rotx is not 127 and not (-127))
                {
                    obj.rotx -= (rotx < 0) ? -1 : 1;
                }
            }


			/* If necessary rotate the object around the Z axis... */

			if (rotz != 0)
			{
				rotate_x_first(ref obj.rotmat[0].X, ref obj.rotmat[1].X, rotz);
				rotate_x_first(ref obj.rotmat[0].Y, ref obj.rotmat[1].Y, rotz);
				rotate_x_first(ref obj.rotmat[0].Z, ref obj.rotmat[1].Z, rotz);

				if (rotz is not 127 and not (-127))
				{
					obj.rotz -= (rotz < 0) ? -1 : 1;
				}
			}


			/* Orthonormalize the rotation matrix... */

			VectorMaths.tidy_matrix(obj.rotmat);
		}

		/*
		 * Dock the player into the space station.
		 */
		internal static void dock_player()
		{
			pilot.disengage_auto_pilot();
			elite.docked = true;
			elite.flight_speed = 0;
			elite.flight_roll = 0;
			elite.flight_climb = 0;
			elite.front_shield = 255;
			elite.aft_shield = 255;
			elite.energy = 255;
			elite.myship.altitude = 255;
			elite.myship.cabtemp = 30;
			swat.reset_weapons();
		}

        /*
		 * Check if we are correctly aligned to dock.
		 */
        private static bool is_docking(int sn)
		{
			Vector3 vec;
			float fz;
			float ux;

			if (elite.auto_pilot)     // Don't want it to kill anyone!
			{
				return true;
			}

			fz = universe[sn].rotmat[2].Z;

			if (fz > -0.90)
			{
				return false;
			}

			vec = VectorMaths.unit_vector(universe[sn].location);

			if (vec.Z < 0.927)
			{
				return false;
			}

			ux = universe[sn].rotmat[1].X;
			if (ux < 0)
			{
				ux = -ux;
			}

			if (ux < 0.84)
			{
				return false;
			}

			return true;
		}

        /*
		 * Game Over...
		 */
        private static void do_game_over()
		{
			elite.sound.PlaySample(Sfx.Gameover);
			elite.game_over = true;
		}

		internal static void update_altitude()
		{
			elite.myship.altitude = 255;

			if (elite.witchspace)
			{
				return;
			}

			float x = MathF.Abs(universe[0].location.X);
			float y = MathF.Abs(universe[0].location.Y);
			float z = MathF.Abs(universe[0].location.Z);

			if ((x > 65535) || (y > 65535) || (z > 65535))
			{
				return;
			}

			x /= 256;
			y /= 256;
			z /= 256;

			float dist = (x * x) + (y * y) + (z * z);

			if (dist > 65535)
			{
				return;
			}

			dist -= 9472;
			if (dist < 1)
			{
				elite.myship.altitude = 0;
				do_game_over();
				return;
			}

			dist = MathF.Sqrt(dist);
			if (dist < 1)
			{
				elite.myship.altitude = 0;
				do_game_over();
				return;
			}

			elite.myship.altitude = dist;
		}

		internal static void update_cabin_temp()
		{
			elite.myship.cabtemp = 30;

			if (elite.witchspace)
			{
				return;
			}

			if (ship_count[(int)SHIP.SHIP_CORIOLIS] != 0 || ship_count[(int)SHIP.SHIP_DODEC] != 0)
			{
				return;
			}

			float x = MathF.Abs(universe[1].location.X);
            float y = MathF.Abs(universe[1].location.Y);
            float z = MathF.Abs(universe[1].location.Z);

			if ((x > 65535f) || (y > 65535f) || (z > 65535f))
			{
				return;
			}

			x /= 256;
			y /= 256;
			z /= 256;

			float dist = ((x * x) + (y * y) + (z * z)) / 256;

			if (dist > 255)
            {
                return;
            }

            dist = MathF.Pow(dist, 255f);

			elite.myship.cabtemp = dist + 30f;

			if (elite.myship.cabtemp > 255f)
			{
				elite.myship.cabtemp = 255f;
				do_game_over();
				return;
			}

			if ((elite.myship.cabtemp < 224) || (!elite.cmdr.fuel_scoop))
			{
				return;
			}

			elite.cmdr.fuel += elite.flight_speed / 2;
			if (elite.cmdr.fuel > elite.myship.max_fuel)
			{
				elite.cmdr.fuel = elite.myship.max_fuel;
			}

            elite.info_message("Fuel Scoop On");
		}

		/*
		 * Regenerate the shields and the energy banks.
		 */
		internal static void regenerate_shields()
		{
			if (elite.energy > 127)
			{
				if (elite.front_shield < 255)
				{
					elite.front_shield++;
					elite.energy--;
				}

				if (elite.aft_shield < 255)
				{
					elite.aft_shield++;
					elite.energy--;
				}
			}

			elite.energy++;
			elite.energy += elite.cmdr.energy_unit;
			if (elite.energy > 255)
			{
				elite.energy = 255;
			}
		}

		internal static void decrease_energy(int amount)
		{
			elite.energy += amount;

			if (elite.energy <= 0)
			{
				do_game_over();
			}
		}

		/*
		 * Deplete the shields.  Drain the energy banks if the shields fail.
		 */
		internal static void damage_ship(int damage, bool front)
		{
			int shield;

			if (damage <= 0)    /* sanity check */
			{
				return;
			}

			shield = front ? elite.front_shield : elite.aft_shield;

			shield -= damage;
			if (shield < 0)
			{
				decrease_energy(shield);
				shield = 0;
			}

			if (front)
			{
				elite.front_shield = shield;
			}
			else
			{
				elite.aft_shield = shield;
			}
		}

        private static void make_station_appear()
		{
			float px, py, pz;
			float sx, sy, sz;
			Vector3 vec;
			Vector3[] rotmat = new Vector3[3];

			px = universe[0].location.X;
			py = universe[0].location.Y;
			pz = universe[0].location.Z;

			vec.X = (random.rand() & 32767) - 16384;
			vec.Y = (random.rand() & 32767) - 16384;
			vec.Z = random.rand() & 32767;

			vec = VectorMaths.unit_vector(vec);

			sx = px - (vec.X * 65792);
			sy = py - (vec.Y * 65792);
			sz = pz - (vec.Z * 65792);

			//	VectorMaths.set_init_matrix (rotmat);

			rotmat[0].X = 1.0f;
			rotmat[0].Y = 0.0f;
			rotmat[0].Z = 0.0f;

			rotmat[1].X = vec.X;
			rotmat[1].Y = vec.Z;
			rotmat[1].Z = -vec.Y;

			rotmat[2].X = vec.X;
			rotmat[2].Y = vec.Y;
			rotmat[2].Z = vec.Z;

			VectorMaths.tidy_matrix(rotmat);

			swat.add_new_station(sx, sy, sz, rotmat);
		}

        private static void check_docking(int i)
		{
			if (is_docking(i))
			{
				elite.sound.PlaySample(Sfx.Dock);
				dock_player();
				elite.current_screen = SCR.SCR_BREAK_PATTERN;
				return;
			}

			if (elite.flight_speed >= 5)
			{
				do_game_over();
				return;
			}

			elite.flight_speed = 1;
			damage_ship(5, universe[i].location.Z > 0);
			elite.sound.PlaySample(Sfx.Crash);
		}

        private static void switch_to_view(ref univ_object flip)
		{
			float tmp;

			if (elite.current_screen is SCR.SCR_REAR_VIEW or SCR.SCR_GAME_OVER)
			{
				flip.location.X = -flip.location.X;
				flip.location.Z = -flip.location.Z;

				flip.rotmat[0].X = -flip.rotmat[0].X;
				flip.rotmat[0].Z = -flip.rotmat[0].Z;

				flip.rotmat[1].X = -flip.rotmat[1].X;
				flip.rotmat[1].Z = -flip.rotmat[1].Z;

				flip.rotmat[2].X = -flip.rotmat[2].X;
				flip.rotmat[2].Z = -flip.rotmat[2].Z;
				return;
			}

			if (elite.current_screen == SCR.SCR_LEFT_VIEW)
			{
				tmp = flip.location.X;
				flip.location.X = flip.location.Z;
				flip.location.Z = -tmp;

				if (flip.type < 0)
                {
                    return;
                }

                tmp = flip.rotmat[0].X;
				flip.rotmat[0].X = flip.rotmat[0].Z;
				flip.rotmat[0].Z = -tmp;

				tmp = flip.rotmat[1].X;
				flip.rotmat[1].X = flip.rotmat[1].Z;
				flip.rotmat[1].Z = -tmp;

				tmp = flip.rotmat[2].X;
				flip.rotmat[2].X = flip.rotmat[2].Z;
				flip.rotmat[2].Z = -tmp;
				return;
			}

			if (elite.current_screen == SCR.SCR_RIGHT_VIEW)
			{
				tmp = flip.location.X;
				flip.location.X = -flip.location.Z;
				flip.location.Z = tmp;

				if (flip.type < 0)
                {
                    return;
                }

                tmp = flip.rotmat[0].X;
				flip.rotmat[0].X = -flip.rotmat[0].Z;
				flip.rotmat[0].Z = tmp;

				tmp = flip.rotmat[1].X;
				flip.rotmat[1].X = -flip.rotmat[1].Z;
				flip.rotmat[1].Z = tmp;

				tmp = flip.rotmat[2].X;
				flip.rotmat[2].X = -flip.rotmat[2].Z;
				flip.rotmat[2].Z = tmp;

			}
		}

		/*
		 * Update all the objects in the universe and render them.
		 */
		internal static void update_universe()
		{
			SHIP type;
            threed.RenderStart();

			for (int i = 0; i < elite.MAX_UNIV_OBJECTS; i++)
			{
				type = universe[i].type;

				if (type != 0)
				{
					if (universe[i].flags.HasFlag(FLG.FLG_REMOVE))
					{
						if (type == SHIP.SHIP_VIPER)
						{
							elite.cmdr.legal_status |= 64;
						}

						float bounty = elite.ship_list[(int)type].bounty;

						if ((bounty != 0) && (!elite.witchspace))
						{
							elite.cmdr.credits += bounty;
                            elite.info_message($"{elite.cmdr.credits:N1} Credits");
						}

						swat.remove_ship(i);
						continue;
					}

					if (elite.detonate_bomb &&
						(!universe[i].flags.HasFlag(FLG.FLG_DEAD)) &&
						(type != SHIP.SHIP_PLANET) &&
						(type != SHIP.SHIP_SUN) &&
						(type != SHIP.SHIP_CONSTRICTOR) &&
						(type != SHIP.SHIP_COUGAR) &&
						(type != SHIP.SHIP_CORIOLIS) &&
						(type != SHIP.SHIP_DODEC))
					{
						elite.sound.PlaySample(Sfx.Explode);
						universe[i].flags |= FLG.FLG_DEAD;
					}

					if (elite.current_screen is 
						not SCR.SCR_INTRO_ONE and
                        not SCR.SCR_INTRO_TWO and
                        not SCR.SCR_GAME_OVER and
                        not SCR.SCR_ESCAPE_POD)
					{
						swat.tactics(i);
					}

					move_univ_object(ref universe[i]);

                    univ_object flip = (univ_object)universe[i].Clone();
					switch_to_view(ref flip);

					if (type == SHIP.SHIP_PLANET)
					{
						if ((ship_count[(int)SHIP.SHIP_CORIOLIS] == 0) &&
							(ship_count[(int)SHIP.SHIP_DODEC] == 0) &&
							(universe[i].distance < 65792)) // was 49152
						{
							make_station_appear();
						}

						threed.draw_ship(ref flip);
						continue;
					}

					if (type == SHIP.SHIP_SUN)
					{
						threed.draw_ship(ref flip);
						continue;
					}


					if (universe[i].distance < 170)
					{
						if (type is SHIP.SHIP_CORIOLIS or SHIP.SHIP_DODEC)
						{
							check_docking(i);
						}
						else
                        {
                            trade.scoop_item(i);
                        }

                        continue;
					}

					if (universe[i].distance > 57344)
					{
						swat.remove_ship(i);
						continue;
					}

					threed.draw_ship(ref flip);

					universe[i].flags = flip.flags;
					universe[i].exp_seed = flip.exp_seed;
					universe[i].exp_delta = flip.exp_delta;

					universe[i].flags &= ~FLG.FLG_FIRING;

					if (universe[i].flags.HasFlag(FLG.FLG_DEAD))
					{
						continue;
					}

					swat.check_target(i, ref flip);
				}
			}

            threed.RenderFinish();
			elite.detonate_bomb = false;
		}

        /*
		 * Update the scanner and draw all the lollipops.
		 */
        private static void update_scanner()
		{
            GFX_COL colour;

			for (int i = 0; i < elite.MAX_UNIV_OBJECTS; i++)
			{
				if ((universe[i].type <= 0) ||
					universe[i].flags.HasFlag(FLG.FLG_DEAD) ||
					universe[i].flags.HasFlag(FLG.FLG_CLOAKED))
				{
					continue;
				}

                float x = universe[i].location.X / 256;
                float y = universe[i].location.Y / 256;
                float z = universe[i].location.Z / 256;

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

				colour = universe[i].flags.HasFlag(FLG.FLG_HOSTILE) ? GFX_COL.GFX_COL_YELLOW_5 : GFX_COL.GFX_COL_WHITE;

				switch (universe[i].type)
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
        private static void update_compass()
		{
			int un = 0;

			if (elite.witchspace)
			{
				return;
			}

			if (ship_count[(int)SHIP.SHIP_CORIOLIS] != 0 || ship_count[(int)SHIP.SHIP_DODEC] != 0)
			{
				un = 1;
			}

			Vector3 dest = VectorMaths.unit_vector(universe[un].location);

			Vector2 compass = new(elite.compass_centre.X + (dest.X * 16), elite.compass_centre.Y + (dest.Y * -16));

			if (dest.Z < 0)
			{
                elite.alg_gfx.DrawImage(IMG.IMG_RED_DOT, compass);
			}
			else
			{
                elite.alg_gfx.DrawImage(IMG.IMG_GREEN_DOT, compass);
			}

		}

        /*
		 * Display the speed bar.
		 */
        private static void display_speed()
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
        private static void display_dial_bar(float len, Vector2 position)
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
        private static void display_shields()
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

        private static void display_altitude()
		{
			if (elite.myship.altitude > 3)
			{
				display_dial_bar(elite.myship.altitude / 4, new(31f, 92f));
			}
		}

        private static void display_cabin_temp()
		{
			if (elite.myship.cabtemp > 3)
			{
				display_dial_bar(elite.myship.cabtemp / 4f, new(31f, 60f));
			}
		}

        private static void display_laser_temp()
		{
			if (elite.laser_temp > 0)
			{
				display_dial_bar(elite.laser_temp / 4, new(31f, 76f));
			}
		}

        /*
		 * Display the energy banks.
		 */
        private static void display_energy()
		{
			int e1, e2, e3, e4;

			e1 = elite.energy > 64 ? 64 : elite.energy;
			e2 = elite.energy > 128 ? 64 : elite.energy - 64;
			e3 = elite.energy > 192 ? 64 : elite.energy - 128;
			e4 = elite.energy - 192;

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

        private static void display_flight_roll()
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

        private static void display_flight_climb()
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

        private static void display_fuel()
		{
			if (elite.cmdr.fuel > 0)
			{
				display_dial_bar(elite.cmdr.fuel * 64 / elite.myship.max_fuel, new(31f, 44f));
			}
		}

        private static void display_missiles()
		{
			if (elite.cmdr.missiles == 0)
			{
				return;
			}

			int nomiss = elite.cmdr.missiles > 4 ? 4 : elite.cmdr.missiles;

			Vector2 location = new(((4 - nomiss) * 16) + 35, 113 + 385);

			if (swat.missile_target != swat.MISSILE_UNARMED)
			{
                elite.alg_gfx.DrawImage((swat.missile_target < 0) ? IMG.IMG_MISSILE_YELLOW : IMG.IMG_MISSILE_RED, location);
                location.X += 16;
				nomiss--;
			}

			for (; nomiss > 0; nomiss--)
			{
                elite.alg_gfx.DrawImage(IMG.IMG_MISSILE_GREEN, location);
                location.X += 16;
			}
		}

		internal static void update_console()
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

			if (ship_count[(int)SHIP.SHIP_CORIOLIS] != 0 || ship_count[(int)SHIP.SHIP_DODEC] != 0)
			{
                elite.alg_gfx.DrawImage(IMG.IMG_BIG_S, new(387, 490));
			}

			if (swat.ecm_active != 0)
			{
                elite.alg_gfx.DrawImage(IMG.IMG_BIG_E, new(115, 490));
			}
		}

		internal static void increase_flight_roll()
		{
			if (elite.flight_roll < elite.myship.max_roll)
			{
				elite.flight_roll++;
			}
		}

		internal static void decrease_flight_roll()
		{
			if (elite.flight_roll > -elite.myship.max_roll)
			{
				elite.flight_roll--;
			}
		}

		internal static void increase_flight_climb()
		{
			if (elite.flight_climb < elite.myship.max_climb)
			{
				elite.flight_climb++;
			}
		}

		internal static void decrease_flight_climb()
		{
			if (elite.flight_climb > -elite.myship.max_climb)
			{
				elite.flight_climb--;
			}
		}

		internal static void start_hyperspace()
		{
			if (hyper_ready)
			{
				return;
			}

			hyper_distance = GalacticChart.calc_distance_to_planet(elite.docked_planet, elite.hyperspace_planet);

			if ((hyper_distance == 0) || (hyper_distance > elite.cmdr.fuel))
			{
				return;
			}

			destination_planet = (galaxy_seed)elite.hyperspace_planet.Clone();
			hyper_name = Planet.name_planet(destination_planet);
			hyper_name = Planet.capitalise_name(hyper_name);

			hyper_ready = true;
			hyper_countdown = 15;
			hyper_galactic = false;

			pilot.disengage_auto_pilot();
		}

		internal static void start_galactic_hyperspace()
		{
			if (hyper_ready)
			{
				return;
			}

			if (!elite.cmdr.galactic_hyperdrive)
			{
				return;
			}

			hyper_ready = true;
			hyper_countdown = 2;
			hyper_galactic = true;
			pilot.disengage_auto_pilot();
		}

		internal static void display_hyper_status()
		{
			string str = $"{hyper_countdown}";

			if (elite.current_screen is 
				SCR.SCR_FRONT_VIEW or SCR.SCR_REAR_VIEW or
                SCR.SCR_LEFT_VIEW or SCR.SCR_RIGHT_VIEW)
			{
                elite.alg_gfx.DrawTextLeft(5, 5, str, GFX_COL.GFX_COL_WHITE);
				if (hyper_galactic)
				{
                    elite.alg_gfx.DrawTextCentre(358, "Galactic Hyperspace", 120, GFX_COL.GFX_COL_WHITE);
				}
				else
				{
					str = "Hyperspace - " + hyper_name;
                    elite.alg_gfx.DrawTextCentre(358, str, 120, GFX_COL.GFX_COL_WHITE);
				}
			}
			else
			{
                elite.alg_gfx.ClearArea(5, 5, 25, 34);
                elite.alg_gfx.DrawTextLeft(5, 5, str, GFX_COL.GFX_COL_WHITE);
			}
		}

        private static int rotate_byte_left(int x)
		{
			return ((x << 1) | (x >> 7)) & 255;
		}

        private static void enter_next_galaxy()
		{
			elite.cmdr.galaxy_number++;
			elite.cmdr.galaxy_number &= 7;

            galaxy_seed glx = new()
            {
                a = rotate_byte_left(elite.cmdr.galaxy.a),
                b = rotate_byte_left(elite.cmdr.galaxy.b),
                c = rotate_byte_left(elite.cmdr.galaxy.c),
                d = rotate_byte_left(elite.cmdr.galaxy.d),
                e = rotate_byte_left(elite.cmdr.galaxy.e),
                f = rotate_byte_left(elite.cmdr.galaxy.f)
            };
            elite.cmdr.galaxy = glx;

            elite.docked_planet = Planet.find_planet(new(0x60, 0x60));
			elite.hyperspace_planet = (galaxy_seed)elite.docked_planet.Clone();
		}

        private static void enter_witchspace()
		{
			int i;
			int nthg;

			elite.witchspace = true;
			elite.docked_planet.b ^= 31;
			swat.in_battle = true;

			elite.flight_speed = 12;
			elite.flight_roll = 0;
			elite.flight_climb = 0;
			Stars.create_new_stars();
			swat.clear_universe();

			nthg = (random.randint() & 3) + 1;

			for (i = 0; i < nthg; i++)
			{
				swat.create_thargoid();
			}

			elite.current_screen = SCR.SCR_BREAK_PATTERN;
			elite.sound.PlaySample(Sfx.Hyperspace);
		}

        private static void complete_hyperspace()
		{
			Vector3[] rotmat = new Vector3[3];
			int px, py, pz;

			hyper_ready = false;
			elite.witchspace = false;

			if (hyper_galactic)
			{
				elite.cmdr.galactic_hyperdrive = false;
				enter_next_galaxy();
				elite.cmdr.legal_status = 0;
			}
			else
			{
				elite.cmdr.fuel -= hyper_distance;
				elite.cmdr.legal_status /= 2;

				if ((random.rand255() > 253) || (elite.flight_climb == elite.myship.max_climb))
				{
					enter_witchspace();
					return;
				}

				elite.docked_planet = (galaxy_seed)destination_planet.Clone();
			}

			elite.cmdr.market_rnd = random.rand255();
			Planet.generate_planet_data(ref elite.current_planet_data, elite.docked_planet);
			trade.generate_stock_market();

			elite.flight_speed = 12;
			elite.flight_roll = 0;
			elite.flight_climb = 0;
			Stars.create_new_stars();
			swat.clear_universe();

			threed.generate_landscape((elite.docked_planet.a * 251) + elite.docked_planet.b);
			VectorMaths.set_init_matrix(ref rotmat);

			pz = (((elite.docked_planet.b) & 7) + 7) / 2;
			px = pz / 2;
			py = px;

			px <<= 16;
			py <<= 16;
			pz <<= 16;

			if ((elite.docked_planet.b & 1) == 0)
			{
				px = -px;
				py = -py;
			}

			swat.add_new_ship(SHIP.SHIP_PLANET, px, py, pz, rotmat, 0, 0);


			pz = -(((elite.docked_planet.d & 7) | 1) << 16);
			px = ((elite.docked_planet.f & 3) << 16) | ((elite.docked_planet.f & 3) << 8);

			swat.add_new_ship(SHIP.SHIP_SUN, px, py, pz, rotmat, 0, 0);

			elite.current_screen = SCR.SCR_BREAK_PATTERN;
			elite.sound.PlaySample(Sfx.Hyperspace);
		}

		internal static void countdown_hyperspace()
		{
			if (hyper_countdown == 0)
			{
				complete_hyperspace();
				return;
			}

			hyper_countdown--;
		}

		internal static void jump_warp()
		{
			int i;
			SHIP type;
			float jump;

			for (i = 0; i < elite.MAX_UNIV_OBJECTS; i++)
			{
				type = universe[i].type;

				if (type is > 0 and not SHIP.SHIP_ASTEROID and not SHIP.SHIP_CARGO and
                    not SHIP.SHIP_ALLOY and not SHIP.SHIP_ROCK and
                    not SHIP.SHIP_BOULDER and not SHIP.SHIP_ESCAPE_CAPSULE)
				{
                    elite.info_message("Mass Locked");
					return;
				}
			}

			if ((universe[0].distance < 75001) || (universe[1].distance < 75001))
			{
                elite.info_message("Mass Locked");
				return;
			}

			jump = universe[0].distance < universe[1].distance ? universe[0].distance - 75000f : universe[1].distance - 75000f;

            if (jump > 1024)
			{
				jump = 1024;
			}

			for (i = 0; i < elite.MAX_UNIV_OBJECTS; i++)
			{
				if (universe[i].type != 0)
				{
					universe[i].location.Z -= jump;
				}
			}

			Stars.warp_stars = true;
            elite.mcount &= 63;
			swat.in_battle = false;
		}

		internal static void launch_player()
		{
			Vector3[] rotmat = new Vector3[3];

			elite.docked = false;
			elite.flight_speed = 12;
			elite.flight_roll = -15;
			elite.flight_climb = 0;
			elite.cmdr.legal_status |= trade.carrying_contraband();
			Stars.create_new_stars();
			swat.clear_universe();
			threed.generate_landscape((elite.docked_planet.a * 251) + elite.docked_planet.b);
			VectorMaths.set_init_matrix(ref rotmat);
			swat.add_new_ship(SHIP.SHIP_PLANET, 0, 0, 65536, rotmat, 0, 0);

			rotmat[2].X = -rotmat[2].X;
			rotmat[2].Y = -rotmat[2].Y;
			rotmat[2].Z = -rotmat[2].Z;
			swat.add_new_station(0, 0, -256, rotmat);

			elite.current_screen = SCR.SCR_BREAK_PATTERN;
			elite.sound.PlaySample(Sfx.Launch);
		}

		/*
		 * Engage the docking computer.
		 * For the moment we just do an instant dock if we are in the safe zone.
		 */
		internal static void engage_docking_computer()
		{
			if (ship_count[(int)SHIP.SHIP_CORIOLIS] != 0 || ship_count[(int)SHIP.SHIP_DODEC] != 0)
			{
				elite.sound.PlaySample(Sfx.Dock);
				dock_player();
				elite.current_screen = SCR.SCR_BREAK_PATTERN;
			}
		}
	}
}