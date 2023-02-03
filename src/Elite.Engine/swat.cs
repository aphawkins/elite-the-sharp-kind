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
 * swat.c
 *
 * Special Weapons And Tactics.
 */

namespace Elite.Engine
{
	using System.Diagnostics;
	using System.Numerics;
    using Elite.Common.Enums;
	using Elite.Engine.Enums;
	using Elite.Engine.Ships;
	using Elite.Engine.Types;

	internal class swat
	{
		private readonly elite _elite;
		private readonly Audio _audio;

        internal static int MISSILE_UNARMED = -2;
		internal static int MISSILE_ARMED = -1;
        private static int laser_counter;
        private static int laser;
        private static int laser2;
		internal static int ecm_active;
		internal static int missile_target;
        private static bool ecm_ours;
		internal static bool in_battle;
        private static readonly FLG[] initial_flags = new FLG[shipdata.NO_OF_SHIPS + 1]
		{
			0,											// NULL,
			0,											// missile 
			0,											// coriolis
			FLG.FLG_SLOW | FLG.FLG_FLY_TO_PLANET,				// escape
			FLG.FLG_INACTIVE,								// alloy
			FLG.FLG_INACTIVE,								// cargo
			FLG.FLG_INACTIVE,								// boulder
			FLG.FLG_INACTIVE,								// asteroid
			FLG.FLG_INACTIVE,								// rock
			FLG.FLG_FLY_TO_PLANET | FLG.FLG_SLOW,				// shuttle
			FLG.FLG_FLY_TO_PLANET | FLG.FLG_SLOW,				// transporter
			0,											// cobra3
			0,											// python
			0,											// boa
			FLG.FLG_SLOW,									// anaconda
			FLG.FLG_SLOW,									// hermit
			FLG.FLG_BOLD | FLG.FLG_POLICE,						// viper
			FLG.FLG_BOLD | FLG.FLG_ANGRY,						// sidewinder
			FLG.FLG_BOLD | FLG.FLG_ANGRY,						// mamba
			FLG.FLG_BOLD | FLG.FLG_ANGRY,						// krait
			FLG.FLG_BOLD | FLG.FLG_ANGRY,						// adder
			FLG.FLG_BOLD | FLG.FLG_ANGRY,						// gecko
			FLG.FLG_BOLD | FLG.FLG_ANGRY,						// cobra1
			FLG.FLG_SLOW | FLG.FLG_ANGRY,						// worm
			FLG.FLG_BOLD | FLG.FLG_ANGRY,						// cobra3
			FLG.FLG_BOLD | FLG.FLG_ANGRY,						// asp2
			FLG.FLG_BOLD | FLG.FLG_ANGRY,						// python
			FLG.FLG_POLICE,									// fer_de_lance
			FLG.FLG_BOLD | FLG.FLG_ANGRY,						// moray
			FLG.FLG_BOLD | FLG.FLG_ANGRY,						// thargoid
			FLG.FLG_ANGRY,									// thargon
			FLG.FLG_ANGRY,									// constrictor
			FLG.FLG_POLICE | FLG.FLG_CLOAKED,					// cougar
			0											// dodec
		};

		internal swat(elite elite, Audio audio)
		{
			_elite = elite;
			_audio = audio;
        }

		internal static void clear_universe()
		{
			for (int i = 0; i < elite.MAX_UNIV_OBJECTS; i++)
			{
                space.universe[i] = new()
                {
                    type = 0
                };
            }

			for (int i = 0; i <= shipdata.NO_OF_SHIPS; i++)
			{
				space.ship_count[(SHIP)i] = 0;
			}

			in_battle = false;
		}

		internal static int add_new_ship(SHIP ship_type, Vector3 location, Vector3[] rotmat, float rotx, float rotz)
		{
			Debug.Assert(rotmat != null);
			for (int i = 0; i < elite.MAX_UNIV_OBJECTS; i++)
			{
				if (space.universe[i].type == SHIP.SHIP_NONE)
				{
					space.universe[i].type = ship_type;
					space.universe[i].location = location;
					space.universe[i].rotmat = rotmat;
					space.universe[i].rotx = rotx;
					space.universe[i].rotz = rotz;
					space.universe[i].velocity = 0;
					space.universe[i].acceleration = 0;
					space.universe[i].bravery = 0;
					space.universe[i].target = 0;
                    space.universe[i].flags = initial_flags[(int)ship_type < 0 ? 0 : (int)ship_type];

					if (ship_type is not SHIP.SHIP_PLANET and not SHIP.SHIP_SUN)
					{
						space.universe[i].energy = elite.ship_list[(int)ship_type].energy;
						space.universe[i].missiles = elite.ship_list[(int)ship_type].missiles;
						space.ship_count[ship_type]++;
					}

					return i;
				}
			}

			return -1;
		}

        private static void check_missiles(int un)
		{
			if (missile_target == un)
			{
				missile_target = MISSILE_UNARMED;
                elite.info_message("Target Lost");
			}

			for (int i = 0; i < elite.MAX_UNIV_OBJECTS; i++)
			{
				if ((space.universe[i].type == SHIP.SHIP_MISSILE) && (space.universe[i].target == un))
				{
					space.universe[i].flags |= FLG.FLG_DEAD;
				}
			}
		}

		internal static void remove_ship(int un)
		{
			SHIP type;
			Vector3[] rotmat = new Vector3[3];
			type = space.universe[un].type;

			if (type == SHIP.SHIP_NONE)
			{
				return;
			}

			if (type > SHIP.SHIP_NONE)
			{
				space.ship_count[type]--;
			}

			space.universe[un].type = SHIP.SHIP_NONE;

			check_missiles(un);

			if (type is SHIP.SHIP_CORIOLIS or SHIP.SHIP_DODEC)
			{
				VectorMaths.set_init_matrix(ref rotmat);
				Vector3 position = space.universe[un].location;

                position.Y = (int)position.Y & 0xFFFF;
                position.Y = (int)position.Y | 0x60000;

				add_new_ship(SHIP.SHIP_SUN, position, rotmat, 0, 0);
			}
		}

		internal static void add_new_station(Vector3 position, Vector3[] rotmat)
		{
			SHIP station = (elite.current_planet_data.techlevel >= 10) ? SHIP.SHIP_DODEC : SHIP.SHIP_CORIOLIS;
			space.universe[1].type = SHIP.SHIP_NONE;
			add_new_ship(station, position, rotmat, 0, -127);
		}

		internal static void reset_weapons()
		{
			elite.laser_temp = 0;
			laser_counter = 0;
			laser = 0;
			ecm_active = 0;
			missile_target = MISSILE_UNARMED;
		}

        private static void launch_enemy(int un, SHIP type, FLG flags, int bravery)
		{
            Debug.Assert(space.universe[un].rotmat != null);
            int newship = add_new_ship(type, space.universe[un].location, space.universe[un].rotmat, space.universe[un].rotx, space.universe[un].rotz);

			if (newship == -1)
			{
				return;
			}

			univ_object ns = space.universe[newship];

			if (space.universe[un].type is SHIP.SHIP_CORIOLIS or SHIP.SHIP_DODEC)
			{
				ns.velocity = 32;
				ns.location.X += ns.rotmat[2].X * 2;
				ns.location.Y += ns.rotmat[2].Y * 2;
				ns.location.Z += ns.rotmat[2].Z * 2;
			}

			ns.flags |= flags;
			ns.rotz /= 2;
			ns.rotz *= 2;
			ns.bravery = bravery;

			if (type is SHIP.SHIP_CARGO or SHIP.SHIP_ALLOY or SHIP.SHIP_ROCK)
			{
				ns.rotz = ((RNG.Random(255) * 2) & 255) - 128;
				ns.rotx = ((RNG.Random(255) * 2) & 255) - 128;
				ns.velocity = RNG.Random(15);
			}
		}

        private static void launch_loot(int un, SHIP loot)
		{
			int i, cnt;

			if (loot == SHIP.SHIP_ROCK)
			{
				cnt = RNG.Random(3);
			}
			else
			{
				cnt = RNG.Random(255);
				if (cnt >= 128)
                {
                    return;
                }

                cnt &= elite.ship_list[(int)space.universe[un].type].max_loot;
				cnt &= 15;
			}

			for (i = 0; i < cnt; i++)
			{
				launch_enemy(un, loot, 0, 0);
			}
		}

        private static bool in_target(SHIP type, float x, float y, float z)
		{
			if (z < 0)
			{
				return false;
			}

			float size = elite.ship_list[(int)type].size;

			return ((x * x) + (y * y)) <= size;
		}

        private static void make_angry(int un)
		{
			SHIP type = space.universe[un].type;
			FLG flags = space.universe[un].flags;

			if (flags.HasFlag(FLG.FLG_INACTIVE))
			{
				return;
			}

			if (type is SHIP.SHIP_CORIOLIS or SHIP.SHIP_DODEC)
			{
				space.universe[un].flags |= FLG.FLG_ANGRY;
				return;
			}

			if (type > SHIP.SHIP_ROCK)
			{
				space.universe[un].rotx = 4;
				space.universe[un].acceleration = 2;
				space.universe[un].flags |= FLG.FLG_ANGRY;
			}
		}

        internal void explode_object(int un)
        {
            elite.cmdr.score++;

            if ((elite.cmdr.score & 255) == 0)
            {
                elite.info_message("Right On Commander!");
            }

            _audio.PlayEffect(SoundEffect.Explode);
            space.universe[un].flags |= FLG.FLG_DEAD;

            if (space.universe[un].type == SHIP.SHIP_CONSTRICTOR)
            {
                elite.cmdr.mission = 2;
            }
        }

        internal void check_target(int un, ref univ_object flip)
		{
			//univ_object univ = space.universe[un];

			if (in_target(space.universe[un].type, flip.location.X, flip.location.Y, flip.location.Z))
			{
				if ((missile_target == MISSILE_ARMED) && (space.universe[un].type >= 0))
				{
					missile_target = un;
                    elite.info_message("Target Locked");
					_audio.PlayEffect(SoundEffect.Beep);
				}

				if (laser > 0)
				{
					_audio.PlayEffect(SoundEffect.HitEnemy);

					if (space.universe[un].type is not SHIP.SHIP_CORIOLIS and not SHIP.SHIP_DODEC)
					{
						if (space.universe[un].type is SHIP.SHIP_CONSTRICTOR or SHIP.SHIP_COUGAR)
						{
							if (laser == (elite.MILITARY_LASER & 127))
							{
                                space.universe[un].energy -= laser / 4;
							}
						}
						else
						{
                            space.universe[un].energy -= laser;
						}
					}

					if (space.universe[un].energy <= 0)
					{
						explode_object(un);

						if (space.universe[un].type == SHIP.SHIP_ASTEROID)
						{
							if (laser == (elite.MINING_LASER & 127))
							{
								launch_loot(un, SHIP.SHIP_ROCK);
							}
						}
						else
						{
							launch_loot(un, SHIP.SHIP_ALLOY);
							launch_loot(un, SHIP.SHIP_CARGO);
						}
					}

					make_angry(un);
				}
			}
		}

		internal void activate_ecm(bool ours)
		{
			if (ecm_active == 0)
			{
				ecm_active = 32;
				ecm_ours = ours;
				_audio.PlayEffect(SoundEffect.Ecm);
			}
		}

		internal void time_ecm()
		{
			if (ecm_active != 0)
			{
				ecm_active--;
				if (ecm_ours)
				{
					_elite.decrease_energy(-1);
				}
			}
		}

		internal static void arm_missile()
		{
			if ((elite.cmdr.missiles != 0) && (missile_target == MISSILE_UNARMED))
			{
				missile_target = MISSILE_ARMED;
			}
		}

		internal void unarm_missile()
		{
			missile_target = MISSILE_UNARMED;
			_audio.PlayEffect(SoundEffect.Boop);
		}

		internal void fire_missile()
		{
			Vector3[] rotmat = new Vector3[3];

			if (missile_target < 0)
			{
				return;
			}

			VectorMaths.set_init_matrix(ref rotmat);
			rotmat[2].Z = 1.0f;
			rotmat[0].X = -1.0f;

			int newship = add_new_ship(SHIP.SHIP_MISSILE, new(0, -28, 14), rotmat, 0, 0);

			if (newship == -1)
			{
                elite.info_message("Missile Jammed");
				return;
			}

            space.universe[newship].velocity = elite.flight_speed * 2;
            space.universe[newship].flags = FLG.FLG_ANGRY;
            space.universe[newship].target = missile_target;

			if (space.universe[missile_target].type > SHIP.SHIP_ROCK)
			{
				space.universe[missile_target].flags |= FLG.FLG_ANGRY;
			}

			elite.cmdr.missiles--;
			missile_target = MISSILE_UNARMED;

			_audio.PlayEffect(SoundEffect.Missile);
		}

        private static void track_object(ref univ_object ship, float direction, Vector3 nvec)
		{
			int rat = 3;
			float rat2 = 0.111f;
			float dir = VectorMaths.vector_dot_product(nvec, ship.rotmat[1]);

			if (direction < -0.861)
			{
				ship.rotx = (dir < 0) ? 7 : -7;
				ship.rotz = 0;
				return;
			}

			ship.rotx = 0;

			if ((MathF.Abs(dir) * 2) >= rat2)
			{
				ship.rotx = (dir < 0) ? rat : -rat;
			}

			if (MathF.Abs(ship.rotz) < 16)
			{
				dir = VectorMaths.vector_dot_product(nvec, ship.rotmat[0]);

				ship.rotz = 0;

				if ((MathF.Abs(dir) * 2) > rat2)
				{
					ship.rotz = (dir < 0) ? rat : -rat;

					if (ship.rotx < 0)
					{
						ship.rotz = -ship.rotz;
					}
				}
			}
		}

        private void missile_tactics(univ_object missile)
		{
			univ_object target;
			Vector3 vec;
			Vector3 nvec;
			float direction;
			float cnt2 = 0.223f;

			if (ecm_active != 0)
			{
				_audio.PlayEffect(SoundEffect.Explode);

				missile.flags |= FLG.FLG_DEAD;
				return;
			}

			if (missile.target == 0)
			{
				if (missile.location.Length() < 512)
				{
					missile.flags |= FLG.FLG_DEAD;
					_audio.PlayEffect(SoundEffect.Explode);
					_elite.damage_ship(250, missile.location.Z >= 0.0);
					return;
				}

				vec = missile.location;
			}
			else
			{
				target = space.universe[missile.target];
				vec = missile.location - target.location;

				if (vec.Length() < 512)
				{
					missile.flags |= FLG.FLG_DEAD;

					if (target.type is not SHIP.SHIP_CORIOLIS and not SHIP.SHIP_DODEC)
					{
						explode_object(missile.target);
					}
					else
					{
						_audio.PlayEffect(SoundEffect.Explode);
					}

					return;
				}

				if ((RNG.Random(255) < 16) && target.flags.HasFlag(FLG.FLG_HAS_ECM))
				{
					activate_ecm(false);
					return;
				}
			}

			nvec = VectorMaths.unit_vector(vec);
			direction = VectorMaths.vector_dot_product(nvec, missile.rotmat[2]);
			nvec.X = -nvec.X;
			nvec.Y = -nvec.Y;
			nvec.Z = -nvec.Z;
			direction = -direction;

			track_object(ref missile, direction, nvec);

			if (direction <= -0.167)
			{
				missile.acceleration = -2;
				return;
			}

			if (direction >= cnt2)
			{
				missile.acceleration = 3;
				return;
			}

			if (missile.velocity < 6)
            {
                missile.acceleration = 3;
            }
            else if (RNG.Random(255) >= 200)
            {
                missile.acceleration = -2;
            }

            return;
		}

        private static void launch_shuttle()
		{
			if ((space.ship_count[SHIP.SHIP_TRANSPORTER] != 0) ||
				(space.ship_count[SHIP.SHIP_SHUTTLE] != 0) ||
				(RNG.Random(255) < 253) || elite.auto_pilot)
			{
				return;
			}

			SHIP type = RNG.TrueOrFalse() ? SHIP.SHIP_SHUTTLE : SHIP.SHIP_TRANSPORTER;
			launch_enemy(1, type, FLG.FLG_HAS_ECM | FLG.FLG_FLY_TO_PLANET, 113);
		}

		internal void tactics(int un)
		{
			int energy;
			int maxeng;
			Vector3 nvec;
			float cnt2 = 0.223f;
			float direction;
			int attacking;

			univ_object ship = space.universe[un];
			SHIP type = ship.type;
			FLG flags = ship.flags;

			if (type is SHIP.SHIP_PLANET or SHIP.SHIP_SUN)
			{
				return;
			}

			if (flags.HasFlag(FLG.FLG_DEAD))
			{
				return;
			}

			if (flags.HasFlag(FLG.FLG_INACTIVE))
			{
				return;
			}

			if (type == SHIP.SHIP_MISSILE)
			{
				if (flags.HasFlag(FLG.FLG_ANGRY))
				{
					missile_tactics(ship);
				}

				return;
			}

			if (((un ^ elite.mcount) & 7) != 0)
            {
                return;
            }

            if (type is SHIP.SHIP_CORIOLIS or SHIP.SHIP_DODEC)
			{
				if (flags.HasFlag(FLG.FLG_ANGRY))
				{
					if (RNG.Random(255) < 240)
					{
						return;
					}

					if (space.ship_count[SHIP.SHIP_VIPER] >= 4)
					{
						return;
					}

					launch_enemy(un, SHIP.SHIP_VIPER, FLG.FLG_ANGRY | FLG.FLG_HAS_ECM, 113);
					return;
				}

				launch_shuttle();
				return;
			}

			if (type == SHIP.SHIP_HERMIT)
			{
				if (RNG.Random(255) > 200)
				{
					launch_enemy(un, SHIP.SHIP_SIDEWINDER + RNG.Random(3), FLG.FLG_ANGRY | FLG.FLG_HAS_ECM, 113);
					ship.flags |= FLG.FLG_INACTIVE;
				}

				return;
			}


			if (ship.energy < elite.ship_list[(int)type].energy)
			{
				ship.energy++;
			}

			if ((type == SHIP.SHIP_THARGLET) && (space.ship_count[SHIP.SHIP_THARGOID] == 0))
			{
				ship.flags = 0;
				ship.velocity /= 2;
				return;
			}

			if (flags.HasFlag(FLG.FLG_SLOW))
			{
				if (RNG.Random(255) > 50)
				{
					return;
				}
			}

			if (flags.HasFlag(FLG.FLG_POLICE))
			{
				if (elite.cmdr.legal_status >= 64)
				{
					flags |= FLG.FLG_ANGRY;
					ship.flags = flags;
				}
			}

			if (!flags.HasFlag(FLG.FLG_ANGRY))
			{
				if (flags.HasFlag(FLG.FLG_FLY_TO_PLANET) || flags.HasFlag(FLG.FLG_FLY_TO_STATION))
				{
					pilot.auto_pilot_ship(ref space.universe[un]);
				}

				return;
			}


			/* If we get to here then the ship is angry so start attacking... */

			if (space.ship_count[SHIP.SHIP_CORIOLIS] != 0 || space.ship_count[SHIP.SHIP_DODEC] != 0)
			{
				if (!flags.HasFlag(FLG.FLG_BOLD))
				{
					ship.bravery = 0;
				}
			}

			if (type == SHIP.SHIP_ANACONDA)
			{
				if (RNG.Random(255) > 200)
				{
					launch_enemy(un, RNG.Random(255) > 100 ? SHIP.SHIP_WORM : SHIP.SHIP_SIDEWINDER, FLG.FLG_ANGRY | FLG.FLG_HAS_ECM, 113);
					return;
				}
			}

			if (RNG.Random(255) >= 250)
			{
				ship.rotz = RNG.Random(255) | 0x68;
				if (ship.rotz > 127)
				{
					ship.rotz = -((int)ship.rotz & 127);
				}
			}

			maxeng = elite.ship_list[(int)type].energy;
			energy = ship.energy;

			if (energy < (maxeng / 2))
			{
				if ((energy < (maxeng / 8)) && (RNG.Random(255) > 230) && (type != SHIP.SHIP_THARGOID))
				{
					ship.flags &= ~FLG.FLG_ANGRY;
					ship.flags |= FLG.FLG_INACTIVE;
					launch_enemy(un, SHIP.SHIP_ESCAPE_CAPSULE, 0, 126);
					return;
				}

				if ((ship.missiles != 0) && (ecm_active == 0) && (ship.missiles >= RNG.Random(31)))
				{
					ship.missiles--;
					if (type == SHIP.SHIP_THARGOID)
					{
						launch_enemy(un, SHIP.SHIP_THARGLET, FLG.FLG_ANGRY, ship.bravery);
					}
					else
					{
						launch_enemy(un, SHIP.SHIP_MISSILE, FLG.FLG_ANGRY, 126);
                        elite.info_message("INCOMING MISSILE");
					}
					return;
				}
			}

			nvec = VectorMaths.unit_vector(space.universe[un].location);
			direction = VectorMaths.vector_dot_product(nvec, ship.rotmat[2]);

			if ((ship.location.Length() < 8192) && (direction <= -0.833) &&
				 (elite.ship_list[(int)type].laser_strength != 0))
			{
				if (direction <= -0.917)
				{
					ship.flags |= FLG.FLG_FIRING | FLG.FLG_HOSTILE;
				}

				if (direction <= -0.972)
				{
					_elite.damage_ship(elite.ship_list[(int)type].laser_strength, ship.location.Z >= 0.0);
					ship.acceleration--;
					if (((ship.location.Z >= 0.0) && (elite.front_shield == 0)) ||
						((ship.location.Z < 0.0) && (elite.aft_shield == 0)))
					{
						_audio.PlayEffect(SoundEffect.IncomingFire2);
					}
					else
					{
						_audio.PlayEffect(SoundEffect.IncomingFire1);
					}
				}
				else
				{
					nvec.X = -nvec.X;
					nvec.Y = -nvec.Y;
					nvec.Z = -nvec.Z;
					direction = -direction;
					track_object(ref space.universe[un], direction, nvec);
				}

				//		if ((fabs(ship.location.z) < 768) && (ship.bravery <= ((random.rand255() & 127) | 64)))
				if (MathF.Abs(ship.location.Z) < 768f)
				{
					ship.rotx = RNG.Random(135);
					if (ship.rotx > 127)
                    {
                        ship.rotx = -((int)ship.rotx & 127);
                    }

                    ship.acceleration = 3;
					return;
				}

				ship.acceleration = ship.location.Length() < 8192 ? -1 : 3;

                return;
			}

			attacking = 0;

			if ((MathF.Abs(ship.location.Z) >= 768f) ||
				(MathF.Abs(ship.location.X) >= 512f) ||
				(MathF.Abs(ship.location.Y) >= 512f))
			{
				if (ship.bravery > RNG.Random(127))
				{
					attacking = 1;
					nvec.X = -nvec.X;
					nvec.Y = -nvec.Y;
					nvec.Z = -nvec.Z;
					direction = -direction;
				}
			}

			track_object(ref space.universe[un], direction, nvec);

			if ((attacking == 1) && (ship.location.Length() < 2048))
			{
				if (direction >= cnt2)
				{
					ship.acceleration = -1;
					return;
				}

				if (ship.velocity < 6)
				{
					ship.acceleration = 3;
				}
				else if (RNG.Random(255) >= 200)
				{
					ship.acceleration = -1;
				}

				return;
			}

			if (direction <= -0.167)
			{
				ship.acceleration = -1;
				return;
			}

			if (direction >= cnt2)
			{
				ship.acceleration = 3;
				return;
			}

			if (ship.velocity < 6)
			{
				ship.acceleration = 3;
			}
			else if (RNG.Random(255) >= 200)
			{
				ship.acceleration = -1;
			}
		}

		internal int fire_laser()
		{
			if ((laser_counter == 0) && (elite.laser_temp < 242))
			{
                laser = elite._state.currentScreen switch
                {
                    SCR.SCR_FRONT_VIEW => elite.cmdr.front_laser,
                    SCR.SCR_REAR_VIEW => elite.cmdr.rear_laser,
                    SCR.SCR_RIGHT_VIEW => elite.cmdr.right_laser,
                    SCR.SCR_LEFT_VIEW => elite.cmdr.left_laser,
                    _ => 0,
                };
                if (laser != 0)
				{
					laser_counter = (laser > 127) ? 0 : (laser & 0xFA);
					laser &= 127;
					laser2 = laser;

					_audio.PlayEffect(SoundEffect.Pulse);
					elite.laser_temp += 8;
					if (elite.energy > 1)
					{
						elite.energy--;
					}

					return 2;
				}
			}

			return 0;
		}

		internal static void cool_laser()
		{
			laser = 0;

			if (elite.laser_temp > 0)
			{
				elite.laser_temp--;
			}

			if (laser_counter > 0)
			{
				laser_counter--;
			}

			if (laser_counter > 0)
			{
				laser_counter--;
			}
		}

		private static int create_other_ship(SHIP type)
		{
			Vector3[] rotmat = new Vector3[3];
			VectorMaths.set_init_matrix(ref rotmat);

			Vector3 position = new()
			{
				X = 1000 + RNG.Random(8191),
				Y = 1000 + RNG.Random(8191),
				Z = 12000,
			};

			if (RNG.Random(255) > 127)
			{
                position.X = -position.X;
			}

			if (RNG.Random(255) > 127)
			{
                position.Y = -position.Y;
			}

			int newship = add_new_ship(type, position, rotmat, 0, 0);

			return newship;
		}

		internal static void create_thargoid()
		{
			int newship = create_other_ship(SHIP.SHIP_THARGOID);
			if (newship != -1)
			{
				space.universe[newship].flags = FLG.FLG_ANGRY | FLG.FLG_HAS_ECM;
				space.universe[newship].bravery = 113;

				if (RNG.Random(255) > 64)
				{
					launch_enemy(newship, SHIP.SHIP_THARGLET, FLG.FLG_ANGRY | FLG.FLG_HAS_ECM, 96);
				}
			}
		}

        private static void create_cougar()
		{
			int newship;

			if (space.ship_count[SHIP.SHIP_COUGAR] != 0)
			{
				return;
			}

			newship = create_other_ship(SHIP.SHIP_COUGAR);
			if (newship != -1)
			{
				space.universe[newship].flags = FLG.FLG_HAS_ECM; // | FLG_CLOAKED;
				space.universe[newship].bravery = 121;
				space.universe[newship].velocity = 18;
			}
		}

        private static void create_trader()
		{
			SHIP type = SHIP.SHIP_COBRA3 + RNG.Random(3);
			int newship = create_other_ship(type);

			if (newship != -1)
			{
				space.universe[newship].rotmat[2].Z = -1.0f;
				space.universe[newship].rotz = RNG.Random(7);
				space.universe[newship].velocity = RNG.Random(31) | 16;
				space.universe[newship].bravery = RNG.Random(127);

				if (RNG.TrueOrFalse())
				{
					space.universe[newship].flags |= FLG.FLG_HAS_ECM;
				}

				//		if (rnd & 2)
				//			space.universe[newship].flags |= FLG.FLG_ANGRY; 
			}
		}

        private static void create_lone_hunter()
		{
			int rnd;
			SHIP type;
			int newship;

			if ((elite.cmdr.mission == 1) && (elite.cmdr.galaxy_number == 1) &&
				(elite.docked_planet.d == 144) && (elite.docked_planet.b == 33) &&
				(space.ship_count[SHIP.SHIP_CONSTRICTOR] == 0))
			{
				type = SHIP.SHIP_CONSTRICTOR;
			}
			else
			{
				rnd = RNG.Random(255);
				type = SHIP.SHIP_COBRA3_LONE + (rnd & 3) + ((rnd > 127) ? 1 : 0);
			}

			newship = create_other_ship(type);

			if (newship != -1)
			{
				space.universe[newship].flags = FLG.FLG_ANGRY;
				if ((RNG.Random(255) > 200) || (type == SHIP.SHIP_CONSTRICTOR))
				{
					space.universe[newship].flags |= FLG.FLG_HAS_ECM;
				}

				space.universe[newship].bravery = ((RNG.Random(255) * 2) | 64) & 127;
				in_battle = true;
			}
		}



        /* Check for a random asteroid encounter... */

        private static void check_for_asteroids()
		{
			SHIP type;

			if ((RNG.Random(255) >= 35) || (space.ship_count[SHIP.SHIP_ASTEROID] >= 3))
			{
				return;
			}

			type = RNG.Random(255) > 253 ? SHIP.SHIP_HERMIT : SHIP.SHIP_ASTEROID;

            int newship = create_other_ship(type);

			if (newship != -1)
			{
				//		space.universe[newship].velocity = (random.rand255() & 31) | 16; 
				space.universe[newship].velocity = 8;
				space.universe[newship].rotz = RNG.TrueOrFalse() ? -127 : 127;
				space.universe[newship].rotx = 16;
			}
		}

        /* If we've been a bad boy then send the cops after us... */
        private static void check_for_cops()
		{
			int offense = trade.carrying_contraband() * 2;
			if (space.ship_count[SHIP.SHIP_VIPER] == 0)
			{
				offense |= elite.cmdr.legal_status;
			}

			if (RNG.Random(255) >= offense)
			{
				return;
			}

			int newship = create_other_ship(SHIP.SHIP_VIPER);

			if (newship != -1)
			{
				space.universe[newship].flags = FLG.FLG_ANGRY;
				if (RNG.Random(255) > 245)
                {
                    space.universe[newship].flags |= FLG.FLG_HAS_ECM;
                }

                space.universe[newship].bravery = ((RNG.Random(255) * 2) | 64) & 127;
			}
		}

		private static void check_for_others()
		{
			int newship;
			Vector3[] rotmat = new Vector3[3];
			SHIP type;
			int i;

			int gov = elite.current_planet_data.government;
			int rnd = RNG.Random(255);

			if ((gov != 0) && ((rnd >= 90) || ((rnd & 7) < gov)))
			{
				return;
			}

			if (RNG.Random(255) < 100)
			{
				create_lone_hunter();
				return;
			}

			/* Pack hunters... */

			VectorMaths.set_init_matrix(ref rotmat);

			Vector3 position = new()
			{
				Z = 12000,
				X = 1000 + RNG.Random(8191),
				Y = 1000 + RNG.Random(8191),
			};

			if (RNG.TrueOrFalse())
			{
                position.X = -position.X;
			}

			if (RNG.TrueOrFalse())
			{
                position.Y = -position.Y;
			}

			rnd = RNG.Random(3);

			for (i = 0; i <= rnd; i++)
			{
				type = SHIP.SHIP_SIDEWINDER + (RNG.Random(255) & RNG.Random(7));
				newship = add_new_ship(type, position, rotmat, 0, 0);
				if (newship != -1)
				{
					space.universe[newship].flags = FLG.FLG_ANGRY;
					if (RNG.Random(255) > 245)
					{
						space.universe[newship].flags |= FLG.FLG_HAS_ECM;
					}

					space.universe[newship].bravery = ((RNG.Random(255) * 2) | 64) & 127;
					in_battle = true;
				}
			}
		}

		internal static void random_encounter()
		{
			if ((space.ship_count[SHIP.SHIP_CORIOLIS] != 0) || (space.ship_count[SHIP.SHIP_DODEC] != 0))
			{
				return;
			}

			if (RNG.Random(255) == 136)
			{
				if (((int)space.universe[0].location.Z & 0x3e) != 0)
				{
					create_thargoid();
				}
				else
				{
					create_cougar();
				}

				return;
			}

			if (RNG.Random(7) == 0)
			{
				create_trader();
				return;
			}

			check_for_asteroids();

			check_for_cops();

			if (space.ship_count[SHIP.SHIP_VIPER] != 0)
			{
				return;
			}

			if (in_battle)
			{
				return;
			}

			if ((elite.cmdr.mission == 5) && (RNG.Random(255) >= 200))
			{
				create_thargoid();
			}

			check_for_others();
		}
	}
}