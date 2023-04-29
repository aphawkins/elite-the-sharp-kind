namespace Elite.Engine
{
	using System.Diagnostics;
	using System.Numerics;
	using Elite.Common.Enums;
	using Elite.Engine.Enums;
	using Elite.Engine.Ships;
	using Elite.Engine.Types;

	internal class Combat
	{
        internal bool inBattle;
        internal int MISSILE_ARMED = -1;
        internal int MISSILE_UNARMED = -2;
        internal int missileTarget;
        private readonly Audio _audio;
        private readonly GameState _gameState;
        private readonly PlayerShip _ship;
		private readonly Trade _trade;
        private readonly FLG[] initialFlags = new FLG[shipdata.NO_OF_SHIPS + 1]
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

        private bool isEcmOurs;
        private int laserCounter;
        private int laserStrength;
        private LaserType laserType;

		internal Combat(GameState gameState, Audio audio, PlayerShip ship, Trade trade)
		{
			_gameState = gameState;
            _audio = audio;
			_ship = ship;
			_trade = trade;
        }

        internal void ActivateECM(bool ours)
        {
            if (_ship.ecmActive == 0)
            {
                _ship.ecmActive = 32;
                isEcmOurs = ours;
                _audio.PlayEffect(SoundEffect.Ecm);
            }
        }

        internal int AddNewShip(SHIP ship_type, Vector3 location, Vector3[] rotmat, float rotx, float rotz)
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
                    space.universe[i].flags = initialFlags[(int)ship_type < 0 ? 0 : (int)ship_type];

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

        internal void AddNewStation(Vector3 position, Vector3[] rotmat)
        {
            SHIP station = (_gameState.current_planet_data.techlevel >= 10) ? SHIP.SHIP_DODEC : SHIP.SHIP_CORIOLIS;
            space.universe[1].type = SHIP.SHIP_NONE;
            AddNewShip(station, position, rotmat, 0, -127);
        }

        internal void ArmMissile()
        {
            if ((_ship.missileCount != 0) && (missileTarget == MISSILE_UNARMED))
            {
                missileTarget = MISSILE_ARMED;
            }
        }

        internal void CheckTarget(int un, ref univ_object flip)
        {
            //univ_object univ = space.universe[un];

            if (IsInTarget(space.universe[un].type, flip.location.X, flip.location.Y, flip.location.Z))
            {
                if ((missileTarget == MISSILE_ARMED) && (space.universe[un].type >= 0))
                {
                    missileTarget = un;
                    elite.info_message("Target Locked");
                    _audio.PlayEffect(SoundEffect.Beep);
                }

                if (laserStrength > 0)
                {
                    _audio.PlayEffect(SoundEffect.HitEnemy);

                    if (space.universe[un].type is not SHIP.SHIP_CORIOLIS and not SHIP.SHIP_DODEC)
                    {
                        if (space.universe[un].type is SHIP.SHIP_CONSTRICTOR or SHIP.SHIP_COUGAR)
                        {
                            if (laserType == LaserType.Military)
                            {
                                space.universe[un].energy -= laserStrength / 4;
                            }
                        }
                        else
                        {
                            space.universe[un].energy -= laserStrength;
                        }
                    }

                    if (space.universe[un].energy <= 0)
                    {
                        ExplodeObject(un);

                        if (space.universe[un].type == SHIP.SHIP_ASTEROID)
                        {
                            if (laserType is LaserType.Mining or LaserType.Pulse)
                            {
                                LaunchLoot(un, SHIP.SHIP_ROCK);
                            }
                        }
                        else
                        {
                            LaunchLoot(un, SHIP.SHIP_ALLOY);
                            LaunchLoot(un, SHIP.SHIP_CARGO);
                        }
                    }

                    MakeAngry(un);
                }
            }
        }

        internal void ClearUniverse()
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

			inBattle = false;
		}
        internal void CoolLaser()
        {
            laserStrength = 0;
            laserType = LaserType.None;
            elite.drawLasers = false;

            if (elite.laser_temp > 0)
            {
                elite.laser_temp--;
            }

            laserCounter = Math.Clamp(laserCounter - 2, 0, laserCounter);
        }

        internal void CreateThargoid()
        {
            int newship = CreateOtherShip(SHIP.SHIP_THARGOID);
            if (newship != -1)
            {
                space.universe[newship].flags = FLG.FLG_ANGRY | FLG.FLG_HAS_ECM;
                space.universe[newship].bravery = 113;

                if (RNG.Random(255) > 64)
                {
                    LaunchEnemy(newship, SHIP.SHIP_THARGLET, FLG.FLG_ANGRY | FLG.FLG_HAS_ECM, 96);
                }
            }
        }

        internal void ExplodeObject(int un)
        {
            _gameState.cmdr.score++;

            if ((_gameState.cmdr.score & 255) == 0)
            {
                elite.info_message("Right On Commander!");
            }

            _audio.PlayEffect(SoundEffect.Explode);
            space.universe[un].flags |= FLG.FLG_DEAD;

            if (space.universe[un].type == SHIP.SHIP_CONSTRICTOR)
            {
                _gameState.cmdr.mission = 2;
            }
        }

        internal bool FireLaser()
        {
            if (elite.docked ||
                elite.drawLasers ||
                laserCounter != 0 ||
                elite.laser_temp >= 242)
            {
                return false;
            }

            laserStrength = _gameState.currentScreen switch
            {
                SCR.SCR_FRONT_VIEW => _ship.laserFront.Strength,
                SCR.SCR_REAR_VIEW => _ship.laserRear.Strength,
                SCR.SCR_RIGHT_VIEW => _ship.laserRight.Strength,
                SCR.SCR_LEFT_VIEW => _ship.laserLeft.Strength,
                _ => 0,
            };

            if (laserStrength == 0)
            {
                return false;
            }

            laserType = _gameState.currentScreen switch
            {
                SCR.SCR_FRONT_VIEW => _ship.laserFront.Type,
                SCR.SCR_REAR_VIEW => _ship.laserRear.Type,
                SCR.SCR_RIGHT_VIEW => _ship.laserRight.Type,
                SCR.SCR_LEFT_VIEW => _ship.laserLeft.Type,
                _ => LaserType.None,
            };

            laserCounter = (laserStrength > 127) ? 0 : (laserStrength & 250);
            laserStrength &= 127;

            _audio.PlayEffect(SoundEffect.Pulse);
            elite.laser_temp += 8;
            if (_ship.energy > 1)
            {
                _ship.energy--;
            }

            return true;
        }

        internal void FireMissile()
        {
            if (missileTarget < 0)
            {
                return;
            }

            Vector3[] rotmat = VectorMaths.GetInitialMatrix();
            rotmat[2].Z = 1;
            rotmat[0].X = -1;

            int newship = AddNewShip(SHIP.SHIP_MISSILE, new(0, -28, 14), rotmat, 0, 0);

            if (newship == -1)
            {
                elite.info_message("Missile Jammed");
                return;
            }

            space.universe[newship].velocity = _ship.speed * 2;
            space.universe[newship].flags = FLG.FLG_ANGRY;
            space.universe[newship].target = missileTarget;

            if (space.universe[missileTarget].type > SHIP.SHIP_ROCK)
            {
                space.universe[missileTarget].flags |= FLG.FLG_ANGRY;
            }

            _ship.missileCount--;
            missileTarget = MISSILE_UNARMED;

            _audio.PlayEffect(SoundEffect.Missile);
        }

        internal void RandomEncounter()
        {
            if ((space.ship_count[SHIP.SHIP_CORIOLIS] != 0) || (space.ship_count[SHIP.SHIP_DODEC] != 0))
            {
                return;
            }

            if (RNG.Random(255) == 136)
            {
                if (((int)space.universe[0].location.Z & 0x3e) != 0)
                {
                    CreateThargoid();
                }
                else
                {
                    CreateCougar();
                }

                return;
            }

            if (RNG.Random(7) == 0)
            {
                CreateTrader();
                return;
            }

            CheckForAsteroids();

            CheckForPolice();

            if (space.ship_count[SHIP.SHIP_VIPER] != 0)
            {
                return;
            }

            if (inBattle)
            {
                return;
            }

            if ((_gameState.cmdr.mission == 5) && (RNG.Random(255) >= 200))
            {
                CreateThargoid();
            }

            CheckForOthers();
        }

        internal void RemoveShip(int un)
        {
            SHIP type = space.universe[un].type;

            if (type == SHIP.SHIP_NONE)
            {
                return;
            }

            if (type > SHIP.SHIP_NONE)
            {
                space.ship_count[type]--;
            }

            space.universe[un].type = SHIP.SHIP_NONE;

            CheckMissiles(un);

            if (type is SHIP.SHIP_CORIOLIS or SHIP.SHIP_DODEC)
            {
                Vector3 position = space.universe[un].location;
                position.Y = (int)position.Y & 0xFFFF;
                position.Y = (int)position.Y | 0x60000;
                AddNewShip(SHIP.SHIP_SUN, position, VectorMaths.GetInitialMatrix(), 0, 0);
            }
        }

        internal void ResetWeapons()
        {
            elite.laser_temp = 0;
            laserCounter = 0;
            laserStrength = 0;
            laserType = LaserType.None;
            _ship.ecmActive = 0;
            missileTarget = MISSILE_UNARMED;
        }

        internal void ScoopItem(int un)
        {
            if (space.universe[un].flags.HasFlag(FLG.FLG_DEAD))
            {
                return;
            }

            SHIP type = space.universe[un].type;

            if (type == SHIP.SHIP_MISSILE)
            {
                return;
            }

            if ((!_ship.hasFuelScoop) || (space.universe[un].location.Y >= 0) ||
                (_trade.TotalCargoTonnage() == _ship.cargoCapacity))
            {
                ExplodeObject(un);
                _ship.DamageShip(128 + (space.universe[un].energy / 2), space.universe[un].location.Z > 0);
                return;
            }

            if (type == SHIP.SHIP_CARGO)
            {
                StockType trade = (StockType)RNG.Random(1, 8);
                _trade.AddCargo(trade);
                elite.info_message(_trade.stockMarket[trade].name);
                RemoveShip(un);
                return;
            }

            if (elite.ship_list[(int)type].scoopedType != StockType.None)
            {
                StockType trade = elite.ship_list[(int)type].scoopedType;
                _trade.AddCargo(trade);
                elite.info_message(_trade.stockMarket[(StockType)trade].name);
                RemoveShip(un);
                return;
            }

            ExplodeObject(un);
            _ship.DamageShip(space.universe[un].energy / 2, space.universe[un].location.Z > 0);
        }

        internal void Tactics(int un)
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
                    MissileTactics(ship);
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

                    LaunchEnemy(un, SHIP.SHIP_VIPER, FLG.FLG_ANGRY | FLG.FLG_HAS_ECM, 113);
                    return;
                }

                LaunchShuttle();
                return;
            }

            if (type == SHIP.SHIP_HERMIT)
            {
                if (RNG.Random(255) > 200)
                {
                    LaunchEnemy(un, SHIP.SHIP_SIDEWINDER + RNG.Random(3), FLG.FLG_ANGRY | FLG.FLG_HAS_ECM, 113);
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
                if (_gameState.cmdr.legal_status >= 64)
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
                    LaunchEnemy(un, RNG.Random(255) > 100 ? SHIP.SHIP_WORM : SHIP.SHIP_SIDEWINDER, FLG.FLG_ANGRY | FLG.FLG_HAS_ECM, 113);
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
                    LaunchEnemy(un, SHIP.SHIP_ESCAPE_CAPSULE, 0, 126);
                    return;
                }

                if ((ship.missiles != 0) && (_ship.ecmActive == 0) && (ship.missiles >= RNG.Random(31)))
                {
                    ship.missiles--;
                    if (type == SHIP.SHIP_THARGOID)
                    {
                        LaunchEnemy(un, SHIP.SHIP_THARGLET, FLG.FLG_ANGRY, ship.bravery);
                    }
                    else
                    {
                        LaunchEnemy(un, SHIP.SHIP_MISSILE, FLG.FLG_ANGRY, 126);
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
                    _ship.DamageShip(elite.ship_list[(int)type].laser_strength, ship.location.Z >= 0.0);
                    ship.acceleration--;
                    if (((ship.location.Z >= 0.0) && (_ship.shieldFront == 0)) ||
                        ((ship.location.Z < 0.0) && (_ship.shieldRear == 0)))
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
                    TrackObject(ref space.universe[un], direction, nvec);
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

            TrackObject(ref space.universe[un], direction, nvec);

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

        internal void TimeECM()
        {
            if (_ship.ecmActive != 0)
            {
                _ship.ecmActive--;
                if (isEcmOurs)
                {
                    _ship.DecreaseEnergy(-1);
                }
            }
        }

        internal void UnarmMissile()
        {
            missileTarget = MISSILE_UNARMED;
            _audio.PlayEffect(SoundEffect.Boop);
        }

        private static bool IsInTarget(SHIP type, float x, float y, float z)
        {
            if (z < 0)
            {
                return false;
            }

            float size = elite.ship_list[(int)type].size;

            return ((x * x) + (y * y)) <= size;
        }

        private static void MakeAngry(int un)
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

        private static void TrackObject(ref univ_object ship, float direction, Vector3 nvec)
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

        /// <summary>
        /// Check for a random asteroid encounter.
        /// </summary>
        private void CheckForAsteroids()
        {
            SHIP type;

            if ((RNG.Random(255) >= 35) || (space.ship_count[SHIP.SHIP_ASTEROID] >= 3))
            {
                return;
            }

            type = RNG.Random(255) > 253 ? SHIP.SHIP_HERMIT : SHIP.SHIP_ASTEROID;

            int newship = CreateOtherShip(type);

            if (newship != -1)
            {
                //		space.universe[newship].velocity = (random.rand255() & 31) | 16; 
                space.universe[newship].velocity = 8;
                space.universe[newship].rotz = RNG.TrueOrFalse() ? -127 : 127;
                space.universe[newship].rotx = 16;
            }
        }

        private void CheckForOthers()
        {
            int gov = _gameState.current_planet_data.government;
            int rnd = RNG.Random(255);

            if ((gov != 0) && ((rnd >= 90) || ((rnd & 7) < gov)))
            {
                return;
            }

            if (RNG.Random(255) < 100)
            {
                CreateLoneHunter();
                return;
            }

            /* Pack hunters... */
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

            for (int i = 0; i <= rnd; i++)
            {
                SHIP type = SHIP.SHIP_SIDEWINDER + (RNG.Random(255) & RNG.Random(7));
                int newship = AddNewShip(type, position, VectorMaths.GetInitialMatrix(), 0, 0);
                if (newship != -1)
                {
                    space.universe[newship].flags = FLG.FLG_ANGRY;
                    if (RNG.Random(255) > 245)
                    {
                        space.universe[newship].flags |= FLG.FLG_HAS_ECM;
                    }

                    space.universe[newship].bravery = ((RNG.Random(255) * 2) | 64) & 127;
                    inBattle = true;
                }
            }
        }

        private void CheckForPolice()
        {
            int offense = _trade.IsCarryingContraband() * 2;
            if (space.ship_count[SHIP.SHIP_VIPER] == 0)
            {
                offense |= _gameState.cmdr.legal_status;
            }

            if (RNG.Random(255) >= offense)
            {
                return;
            }

            int newship = CreateOtherShip(SHIP.SHIP_VIPER);

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

        private void CheckMissiles(int un)
        {
			if (missileTarget == un)
			{
				missileTarget = MISSILE_UNARMED;
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
        private void CreateCougar()
        {
            int newship;

            if (space.ship_count[SHIP.SHIP_COUGAR] != 0)
            {
                return;
            }

            newship = CreateOtherShip(SHIP.SHIP_COUGAR);
            if (newship != -1)
            {
                space.universe[newship].flags = FLG.FLG_HAS_ECM; // | FLG_CLOAKED;
                space.universe[newship].bravery = 121;
                space.universe[newship].velocity = 18;
            }
        }

        private void CreateLoneHunter()
        {
            int rnd;
            SHIP type;
            int newship;

            if ((_gameState.cmdr.mission == 1) && (_gameState.cmdr.galaxy_number == 1) &&
                (_gameState.docked_planet.d == 144) && (_gameState.docked_planet.b == 33) &&
                (space.ship_count[SHIP.SHIP_CONSTRICTOR] == 0))
            {
                type = SHIP.SHIP_CONSTRICTOR;
            }
            else
            {
                rnd = RNG.Random(255);
                type = SHIP.SHIP_COBRA3_LONE + (rnd & 3) + ((rnd > 127) ? 1 : 0);
            }

            newship = CreateOtherShip(type);

            if (newship != -1)
            {
                space.universe[newship].flags = FLG.FLG_ANGRY;
                if ((RNG.Random(255) > 200) || (type == SHIP.SHIP_CONSTRICTOR))
                {
                    space.universe[newship].flags |= FLG.FLG_HAS_ECM;
                }

                space.universe[newship].bravery = ((RNG.Random(255) * 2) | 64) & 127;
                inBattle = true;
            }
        }

        private int CreateOtherShip(SHIP type)
        {
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

            int newship = AddNewShip(type, position, VectorMaths.GetInitialMatrix(), 0, 0);
            return newship;
        }

        private void CreateTrader()
        {
            SHIP type = SHIP.SHIP_COBRA3 + RNG.Random(3);
            int newship = CreateOtherShip(type);

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

        private void LaunchEnemy(int un, SHIP type, FLG flags, int bravery)
        {
            Debug.Assert(space.universe[un].rotmat != null);
			Vector3[] rotmat = space.universe[un].rotmat.Cloner();
            int newship = AddNewShip(type, space.universe[un].location, rotmat, space.universe[un].rotx, space.universe[un].rotz);

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

        private void LaunchLoot(int un, SHIP loot)
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
				LaunchEnemy(un, loot, 0, 0);
			}
		}
        private void LaunchShuttle()
        {
            if ((space.ship_count[SHIP.SHIP_TRANSPORTER] != 0) ||
                (space.ship_count[SHIP.SHIP_SHUTTLE] != 0) ||
                (RNG.Random(255) < 253) || elite.auto_pilot)
            {
                return;
            }

            SHIP type = RNG.TrueOrFalse() ? SHIP.SHIP_SHUTTLE : SHIP.SHIP_TRANSPORTER;
            LaunchEnemy(1, type, FLG.FLG_HAS_ECM | FLG.FLG_FLY_TO_PLANET, 113);
        }

        private void MissileTactics(univ_object missile)
        {
			univ_object target;
			Vector3 vec;
			Vector3 nvec;
			float direction;
			float cnt2 = 0.223f;

			if (_ship.ecmActive != 0)
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
                    _ship.DamageShip(250, missile.location.Z >= 0.0);
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
						ExplodeObject(missile.target);
					}
					else
					{
						_audio.PlayEffect(SoundEffect.Explode);
					}

					return;
				}

				if ((RNG.Random(255) < 16) && target.flags.HasFlag(FLG.FLG_HAS_ECM))
				{
					ActivateECM(false);
					return;
				}
			}

			nvec = VectorMaths.unit_vector(vec);
			direction = VectorMaths.vector_dot_product(nvec, missile.rotmat[2]);
			nvec.X = -nvec.X;
			nvec.Y = -nvec.Y;
			nvec.Z = -nvec.Z;
			direction = -direction;

			TrackObject(ref missile, direction, nvec);

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
    }
}