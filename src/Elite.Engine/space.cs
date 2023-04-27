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

	internal class space
	{
		private readonly GameState _gameState;
        private readonly IGfx _gfx;
        private readonly threed _threed;
        private readonly Audio _audio;
        private readonly pilot _pilot;
        private readonly swat _swat;
        private readonly Trade _trade;
        private readonly Planet _planet;
		private readonly PlayerShip _ship;

        private static galaxy_seed destination_planet;
		internal static bool hyper_ready;
        internal static int hyper_countdown;
        internal static string hyper_name;
        private static float hyper_distance;
        internal static bool hyper_galactic;
		internal static univ_object[] universe = new univ_object[elite.MAX_UNIV_OBJECTS];
		internal static Dictionary<SHIP, int> ship_count = new(shipdata.NO_OF_SHIPS + 1);  /* many */

		internal space(GameState gameState, IGfx gfx, threed threed, Audio audio, pilot pilot, swat swat, Trade trade, Planet planet, PlayerShip ship)
		{
            _gameState = gameState;
            _gfx = gfx;
			_threed = threed;
			_audio = audio;
			_pilot = pilot;
			_swat = swat;
			_trade = trade;
			_planet = planet;
			_ship = ship;
        }

        private static void rotate_x_first(ref float a, ref float b, float direction)
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
        private void move_univ_object(ref univ_object obj)
		{
			float x, y, z;
			float k2;
			float alpha;
			float beta;
			float rotx, rotz;
			float speed;

			alpha = _ship.roll / 256;
			beta = _ship.climb / 256;

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

			z -= _ship.speed;

			obj.location = new(x, y, z);

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

		internal void update_altitude()
		{
            _ship.altitude = 255;

			if (_gameState.witchspace)
			{
				return;
			}

			float x = MathF.Abs(universe[0].location.X);
			float y = MathF.Abs(universe[0].location.Y);
			float z = MathF.Abs(universe[0].location.Z);

			if ((x == 0 && y == 0 && z == 0) ||
				x > 65535 || y > 65535 || z > 65535)
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
                _ship.altitude = 0;
                _gameState.GameOver();
				return;
			}

			dist = MathF.Sqrt(dist);
			if (dist < 1)
			{
                _ship.altitude = 0;
                _gameState.GameOver();
				return;
			}

            _ship.altitude = dist;
		}

		internal void update_cabin_temp()
		{
            _ship.cabinTemperature = 30;

			if (_gameState.witchspace)
			{
				return;
			}

			if (ship_count[SHIP.SHIP_CORIOLIS] != 0 || ship_count[SHIP.SHIP_DODEC] != 0)
			{
				return;
			}

			float x = MathF.Abs(universe[1].location.X);
            float y = MathF.Abs(universe[1].location.Y);
            float z = MathF.Abs(universe[1].location.Z);

            if ((x == 0 && y == 0 && z == 0) ||
                x > 65535 || y > 65535 || z > 65535)
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

            dist = (int)dist ^ 255;

            _ship.cabinTemperature = dist + 30;

			if (_ship.cabinTemperature > 255)
			{
                _ship.cabinTemperature = 255;
                _gameState.GameOver();
				return;
			}

			if ((_ship.cabinTemperature < 224) || (!_ship.hasFuelScoop))
			{
				return;
			}

            _ship.fuel += _ship.speed / 2;
			if (_ship.fuel > _ship.maxFuel)
			{
                _ship.fuel = _ship.maxFuel;
			}

            elite.info_message("Fuel Scoop On");
		}

        private void make_station_appear()
		{
            Vector3 location = universe[0].location;
            Vector3 vec;
            vec.X = RNG.Random(-16384, 16383) ;
			vec.Y = RNG.Random(-16384, 16383) ;
			vec.Z = RNG.Random(32767);

			vec = VectorMaths.unit_vector(vec);

			Vector3 position = new()
			{
				X = location.X - (vec.X * 65792),
				Y = location.Y - (vec.Y * 65792),
				Z = location.Z - (vec.Z * 65792),
			};

            //	VectorMaths.set_init_matrix (rotmat);
            Vector3[] rotmat = new Vector3[3];

            rotmat[0].X = 1;
			rotmat[0].Y = 0;
			rotmat[0].Z = 0;

			rotmat[1].X = vec.X;
			rotmat[1].Y = vec.Z;
			rotmat[1].Z = -vec.Y;

			rotmat[2].X = vec.X;
			rotmat[2].Y = vec.Y;
			rotmat[2].Z = vec.Z;

			VectorMaths.tidy_matrix(rotmat);

			_swat.add_new_station(position, rotmat);
		}

        private void check_docking(int i)
		{
			if (elite.docked)
			{
				return;
			}

			if (is_docking(i))
			{
                _gameState.SetView(SCR.SCR_DOCKING);
				return;
			}

			if (_ship.speed >= 5)
			{
                _gameState.GameOver();
				return;
			}

            _ship.speed = 1;
            _ship.DamageShip(5, universe[i].location.Z > 0);
			_audio.PlayEffect(SoundEffect.Crash);
		}

        private void switch_to_view(ref univ_object flip)
		{
			float tmp;

			if (_gameState.currentScreen is SCR.SCR_REAR_VIEW or SCR.SCR_GAME_OVER)
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

			if (_gameState.currentScreen == SCR.SCR_LEFT_VIEW)
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

			if (_gameState.currentScreen == SCR.SCR_RIGHT_VIEW)
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
		internal void update_universe()
		{
            threed.RenderStart();

			for (int i = 0; i < elite.MAX_UNIV_OBJECTS; i++)
            {
                SHIP type = universe[i].type;

                if (type == SHIP.SHIP_NONE)
                {
                    continue;
                }

                if (universe[i].flags.HasFlag(FLG.FLG_REMOVE))
                {
                    if (type == SHIP.SHIP_VIPER)
                    {
                        _gameState.cmdr.legal_status |= 64;
                    }

                    float bounty = elite.ship_list[(int)type].bounty;

                    if ((bounty != 0) && (!_gameState.witchspace))
                    {
                        _trade.credits += bounty;
                        elite.info_message($"{_trade.credits:N1} Credits");
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
                    _audio.PlayEffect(SoundEffect.Explode);
                    universe[i].flags |= FLG.FLG_DEAD;
                }

                if (_gameState.currentScreen is
                    not SCR.SCR_INTRO_ONE and
                    not SCR.SCR_INTRO_TWO and
                    not SCR.SCR_GAME_OVER and
                    not SCR.SCR_ESCAPE_POD)
                {
                    _swat.tactics(i);
                }

                move_univ_object(ref universe[i]);

                univ_object flip = (univ_object)universe[i].Clone();
                switch_to_view(ref flip);

                if (type == SHIP.SHIP_PLANET)
                {
                    if ((ship_count[SHIP.SHIP_CORIOLIS] == 0) &&
                        (ship_count[SHIP.SHIP_DODEC] == 0) &&
                        (universe[i].location.Length() < 65792)) // was 49152
                    {
                        make_station_appear();
                    }

					_threed.DrawObject(flip);
                    continue;
                }

                if (type == SHIP.SHIP_SUN)
                {
                    _threed.DrawObject(flip);
                    continue;
                }


                if (universe[i].location.Length() < 170)
                {
                    if (type is SHIP.SHIP_CORIOLIS or SHIP.SHIP_DODEC)
                    {
                        check_docking(i);
                    }
                    else
                    {
                        _swat.scoop_item(i);
                    }

                    continue;
                }

                if (universe[i].location.Length() > 57344)
                {
                    swat.remove_ship(i);
                    continue;
                }

                _threed.DrawObject(flip);

                universe[i].flags = flip.flags;
                universe[i].exp_delta = flip.exp_delta;

                universe[i].flags &= ~FLG.FLG_FIRING;

                if (universe[i].flags.HasFlag(FLG.FLG_DEAD))
                {
                    continue;
                }

                _swat.check_target(i, ref flip);
            }

            _threed.RenderEnd();
			elite.detonate_bomb = false;
		}

		internal void start_hyperspace()
		{
			if (hyper_ready)
			{
				return;
			}

			hyper_distance = Planet.calc_distance_to_planet(_gameState.docked_planet, _gameState.hyperspace_planet);

			if ((hyper_distance == 0) || (hyper_distance > _ship.fuel))
			{
				return;
			}

			destination_planet = (galaxy_seed)_gameState.hyperspace_planet.Clone();
			hyper_name = Planet.name_planet(destination_planet, true);
			hyper_ready = true;
			hyper_countdown = 15;
			hyper_galactic = false;

			_pilot.disengage_auto_pilot();
		}

		internal void start_galactic_hyperspace()
		{
			if (hyper_ready)
			{
				return;
			}

			if (!_ship.hasGalacticHyperdrive)
			{
				return;
			}

			hyper_ready = true;
			hyper_countdown = 2;
			hyper_galactic = true;
			_pilot.disengage_auto_pilot();
		}

		internal void display_hyper_status()
		{             
			_gfx.DrawTextRight(22, 5, $"{hyper_countdown}", GFX_COL.GFX_COL_WHITE);
		}

        private static int rotate_byte_left(int x)
		{
			return ((x << 1) | (x >> 7)) & 255;
		}

        private void enter_next_galaxy()
		{
            _gameState.cmdr.galaxy_number++;
            _gameState.cmdr.galaxy_number &= 7;

            galaxy_seed glx = new()
            {
                a = rotate_byte_left(_gameState.cmdr.galaxy.a),
                b = rotate_byte_left(_gameState.cmdr.galaxy.b),
                c = rotate_byte_left(_gameState.cmdr.galaxy.c),
                d = rotate_byte_left(_gameState.cmdr.galaxy.d),
                e = rotate_byte_left(_gameState.cmdr.galaxy.e),
                f = rotate_byte_left(_gameState.cmdr.galaxy.f)
            };
            _gameState.cmdr.galaxy = glx;

            _gameState.docked_planet = Planet.find_planet(_gameState.cmdr.galaxy, new(0x60, 0x60));
            _gameState.hyperspace_planet = (galaxy_seed)_gameState.docked_planet.Clone();
		}

        private void enter_witchspace()
		{
			int i;
			int nthg;

            _gameState.witchspace = true;
            _gameState.docked_planet.b ^= 31;
			swat.in_battle = true;

            _ship.speed = 12;
            _ship.roll = 0;
            _ship.climb = 0;
			Stars.create_new_stars();
			swat.clear_universe();

			nthg = RNG.Random(1, 4);

			for (i = 0; i < nthg; i++)
			{
				swat.create_thargoid();
			}

			_gameState.SetView(SCR.SCR_HYPERSPACE);
		}

        private void complete_hyperspace()
		{
			hyper_ready = false;
            _gameState.witchspace = false;

			if (hyper_galactic)
			{
                _ship.hasGalacticHyperdrive = false;
				hyper_galactic = false;
				enter_next_galaxy();
                _gameState.cmdr.legal_status = 0;
			}
			else
			{
                _ship.fuel -= hyper_distance;
                _gameState.cmdr.legal_status /= 2;

				if ((RNG.Random(255) > 253) || (_ship.climb >= _ship.maxClimb))
				{
                    enter_witchspace();
					return;
				}

                _gameState.docked_planet = (galaxy_seed)destination_planet.Clone();
			}

            _trade.marketRandomiser = RNG.Random(255);
            _gameState.current_planet_data = Planet.generate_planet_data(_gameState.docked_planet);
            _trade.GenerateStockMarket(_gameState.current_planet_data);

            _ship.speed = 12;
            _ship.roll = 0;
            _ship.climb = 0;
			Stars.create_new_stars();
			swat.clear_universe();

			threed.generate_landscape((_gameState.docked_planet.a * 251) + _gameState.docked_planet.b);
			
            Vector3 position = new()
            {
                Z = (((_gameState.docked_planet.b) & 7) + 7) / 2
            };
            position.X = position.Z / 2;
            position.Y = position.X;

            position.X *= 65536;
            position.Y *= 65536;
            position.Z *= 65536;

            if ((_gameState.docked_planet.b & 1) == 0)
			{
                position.X = -position.X;
                position.Y = -position.Y;
			}

            swat.add_new_ship(SHIP.SHIP_PLANET, position, VectorMaths.GetInitialMatrix(), 0, 0);

            position.Z = -(((_gameState.docked_planet.d & 7) | 1) << 16);
            position.X = ((_gameState.docked_planet.f & 3) << 16) | ((_gameState.docked_planet.f & 3) << 8);

            swat.add_new_ship(SHIP.SHIP_SUN, position, VectorMaths.GetInitialMatrix(), 0, 0);

            _gameState.SetView(SCR.SCR_HYPERSPACE);
		}

		internal void countdown_hyperspace()
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

			if ((universe[0].location.Length() < 75001) || (universe[1].location.Length() < 75001))
			{
                elite.info_message("Mass Locked");
				return;
			}

			jump = universe[0].location.Length() < universe[1].location.Length() ? universe[0].location.Length() - 75000f : universe[1].location.Length() - 75000f;

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

		internal void launch_player()
		{
            _ship.speed = 12;
            // Rotate in the same direction that the station is spinning
            _ship.roll = 15;
            _ship.climb = 0;
            _gameState.cmdr.legal_status |= _trade.IsCarryingContraband();
			Stars.create_new_stars();
			threed.generate_landscape((_gameState.docked_planet.a * 251) + _gameState.docked_planet.b);
			swat.add_new_ship(SHIP.SHIP_PLANET, new(0, 0, 65536), VectorMaths.GetInitialMatrix(), 0, 0);

			Vector3[] rotmat = VectorMaths.GetInitialMatrix();
            rotmat[2].X = -rotmat[2].X;
			rotmat[2].Y = -rotmat[2].Y;
			rotmat[2].Z = -rotmat[2].Z;
			_swat.add_new_station(new(0, 0, -256), rotmat);

            elite.docked = false;
        }

        /// <summary>
        /// Dock the player into the space station.
        /// </summary>
        internal void dock_player()
        {
            _pilot.disengage_auto_pilot();
			elite.docked = true;
			_gameState.Reset();
			_ship.Reset();
            swat.reset_weapons();
        }

        /*
		 * Engage the docking computer.
		 * For the moment we just do an instant dock if we are in the safe zone.
		 */
        internal void engage_docking_computer()
		{
			if (ship_count[SHIP.SHIP_CORIOLIS] != 0 || ship_count[SHIP.SHIP_DODEC] != 0)
			{
                _gameState.SetView(SCR.SCR_DOCKING);
			}
		}
    }
}