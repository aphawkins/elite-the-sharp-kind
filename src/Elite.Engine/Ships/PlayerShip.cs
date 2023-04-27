namespace Elite.Engine.Ships
{
    using System;
    using System.Diagnostics;
    using System.Numerics;
    using Elite.Engine.Enums;
    using Elite.Engine.Lasers;
    using Elite.Engine.Types;

    internal class PlayerShip
    {
        internal readonly float maxClimb = 8;
        internal readonly float maxFuel = 7;
        internal readonly float maxRoll = 31;
        // 0.27 Light Mach
        internal readonly float maxSpeed = 40;

        internal float altitude;
        internal float cabinTemperature;
        internal int cargoCapacity;
        internal float climb;
        internal float energy;
        internal EnergyUnit energyUnit;
        internal float fuel;
        internal bool hasDockingComputer;
        internal bool hasECM;
        internal bool hasEnergyBomb;
        internal bool hasEscapePod;
        internal bool hasFuelScoop;
        internal bool hasGalacticHyperdrive;
        internal bool isClimbing;
        internal bool isRolling;
        internal ILaser laserFront = new LaserNone();
        internal ILaser laserLeft = new LaserNone();
        internal ILaser laserRear = new LaserNone();
        internal ILaser laserRight = new LaserNone();
        internal int missileCount;
        internal float roll;
        internal float shieldFront;
        internal float shieldRear;
        internal float speed;

        internal PlayerShip()
        {
            Reset();
        }

        internal void AutoDock()
        {
            univ_object ship = new()
            {
                rotmat = VectorMaths.GetInitialMatrix(),
                location = Vector3.Zero
            };

            ship.rotmat[2].Z = 1;
            ship.rotmat[0].X = -1;
            ship.type = (SHIP)(-96);
            ship.velocity = speed;
            ship.acceleration = 0;
            ship.bravery = 0;
            ship.rotz = 0;
            ship.rotx = 0;

            pilot.auto_pilot_ship(ref ship);

            speed = ship.velocity > 22 ? 22 : ship.velocity;

            if (ship.acceleration > 0)
            {
                speed++;
                if (speed > 22)
                {
                    speed = 22;
                }
            }

            if (ship.acceleration < 0)
            {
                speed--;
                if (speed < 1)
                {
                    speed = 1;
                }
            }

            if (ship.rotx == 0)
            {
                climb = 0;
            }

            if (ship.rotx < 0)
            {
                IncreaseClimb();

                if (ship.rotx < -1)
                {
                    IncreaseClimb();
                }
            }

            if (ship.rotx > 0)
            {
                DecreaseClimb();

                if (ship.rotx > 1)
                {
                    DecreaseClimb();
                }
            }

            if (ship.rotz == 127)
            {
                roll = -14;
            }
            else
            {
                if (ship.rotz == 0)
                {
                    roll = 0;
                }
                else if (ship.rotz > 0)
                {
                    IncreaseRoll();

                    if (ship.rotz > 1)
                    {
                        IncreaseRoll();
                    }
                }
                else if (ship.rotz < 0)
                {
                    DecreaseRoll();

                    if (ship.rotz < -1)
                    {
                        DecreaseRoll();
                    }
                }
            }
        }

        /// <summary>
        /// Deplete the shields.  Drain the energy banks if the shields fail.
        /// </summary>
        /// <param name="damage">Amount of damage.</param>
        /// <param name="front">True if front, false if rear.</param>
        internal void DamageShip(float damage, bool front)
        {
            Debug.Assert(damage > 0);

            float shield = front ? shieldFront : shieldRear;

            shield -= damage;
            if (shield < 0)
            {
                DecreaseEnergy(shield);
                shield = 0;
            }

            if (front)
            {
                shieldFront = shield;
            }
            else
            {
                shieldRear = shield;
            }
        }

        internal void DecreaseClimb() => climb = Math.Clamp(climb - 1, -maxClimb, maxClimb);

        internal void DecreaseEnergy(float amount) => energy += amount;

        internal void DecreaseRoll() => roll = Math.Clamp(roll - 1, -maxRoll, maxRoll);

        internal void DecreaseSpeed() => speed = Math.Clamp(speed - 1, 0, maxSpeed);

        internal void IncreaseClimb() => climb = Math.Clamp(climb + 1, -maxClimb, maxClimb);

        internal void IncreaseRoll() => roll = Math.Clamp(roll + 1, -maxRoll, maxRoll);

        internal void IncreaseSpeed() => speed = Math.Clamp(speed + 1, 0, maxSpeed);

        internal bool IsEnergyLow() => energy < 50;

        internal void LevelOut()
        {
            if (!isRolling)
            {
                if (roll > 0)
                {
                    DecreaseRoll();
                }
                else if (roll < 0)
                {
                    IncreaseRoll();
                }
            }

            if (!isClimbing)
            {
                if (climb > 0)
                {
                    DecreaseClimb();
                }
                else if (climb < 0)
                {
                    IncreaseClimb();
                }
            }
        }

        /// <summary>
        /// Regenerate the shields and the energy banks.
        /// </summary>
        internal void RegenerateShields()
        {
            if (energy > 127)
            {
                if (shieldFront < 255)
                {
                    shieldFront++;
                    energy = Math.Clamp(energy - 1, 0, 255);
                }

                if (shieldRear < 255)
                {
                    shieldRear++;
                    energy = Math.Clamp(energy - 1, 0, 255);
                }
            }

            energy = Math.Clamp(energy + 1 + (int)energyUnit, 0, 255);
        }

        internal void Reset()
        {
            altitude = 255;
            cabinTemperature = 30;
            roll = 0;
            climb = 0;
            speed = 0;
            energy = 255;
            shieldFront = 255;
            shieldRear = 255;
        }
    }
}