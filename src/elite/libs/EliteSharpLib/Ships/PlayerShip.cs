// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Diagnostics;
using EliteSharpLib.Equipment;
using EliteSharpLib.Lasers;

namespace EliteSharpLib.Ships;

internal sealed class PlayerShip
{
    /// <summary>
    /// The bounds of <see cref="Altitude"/>: <see cref="AltitudeMin"/> is a
    /// crash, <see cref="AltitudeMax"/> is as high as the dial reads.
    /// </summary>
    internal const float AltitudeMax = 1;

    /// <inheritdoc cref="AltitudeMax"/>
    internal const float AltitudeMin = 0;

    /// <summary>
    /// The cabin temperature the ship sits at away from a sun - the original's
    /// 30 out of 255.
    /// </summary>
    internal const float AmbientTemperature = 30 * TemperatureStep;

    /// <summary>
    /// One unit of the original's 0-255 altitude scale, expressed as a fraction
    /// of <see cref="AltitudeMax"/>. Space measures the planet's distance in
    /// those units, so it scales the result by this.
    /// </summary>
    internal const float AltitudeStep = 1f / 256f;

    /// <summary>
    /// The bounds of <see cref="Energy"/>: <see cref="EnergyMin"/> is empty
    /// banks, <see cref="EnergyMax"/> is full ones.
    /// </summary>
    internal const float EnergyMax = 1;

    /// <inheritdoc cref="EnergyMax"/>
    internal const float EnergyMin = 0;

    /// <summary>
    /// One unit of the original's 0-255 energy counter, expressed as a fraction
    /// of the banks' capacity. Keeps the regeneration and drain rates unchanged
    /// now that <see cref="Energy"/> is a fraction rather than a raw count.
    /// </summary>
    internal const float EnergyStep = 1f / 256f;

    /// <summary>
    /// The cabin temperature at which the fuel scoop starts collecting - the
    /// original's 224 out of 255.
    /// </summary>
    internal const float ScoopTemperature = 224 * TemperatureStep;

    /// <summary>
    /// The bounds of <see cref="ShieldFront"/> and <see cref="ShieldRear"/>:
    /// <see cref="ShieldMin"/> is a collapsed shield, <see cref="ShieldMax"/>
    /// a fully charged one.
    /// </summary>
    internal const float ShieldMax = 1;

    /// <inheritdoc cref="ShieldMax"/>
    internal const float ShieldMin = 0;

    /// <summary>
    /// The bounds of <see cref="CabinTemperature"/>. Reaching
    /// <see cref="TemperatureMax"/> is a burn-up.
    /// </summary>
    internal const float TemperatureMax = 1;

    /// <inheritdoc cref="TemperatureMax"/>
    internal const float TemperatureMin = 0;

    /// <summary>
    /// One unit of the original's 0-255 cabin temperature, expressed as a
    /// fraction of <see cref="TemperatureMax"/>.
    /// </summary>
    internal const float TemperatureStep = 1f / 256f;

    /// <summary>
    /// The level below which <see cref="IsEnergyLow"/> warns the commander -
    /// the original's 50 out of 255.
    /// </summary>
    private const float LowEnergy = 50 * EnergyStep;

    /// <summary>
    /// One unit of the original's 0-255 shield strength, expressed as a
    /// fraction of a shield's capacity. Keeps the regeneration rate and the
    /// damage laser strengths are quoted in unchanged now that
    /// <see cref="ShieldFront"/> and <see cref="ShieldRear"/> are fractions.
    /// </summary>
    private const float ShieldStep = 1f / 256f;

    internal PlayerShip() => Reset();

    /// <summary>
    /// Gets or sets the height above the planet, between
    /// <see cref="AltitudeMin"/> and <see cref="AltitudeMax"/>.
    /// </summary>
    internal float Altitude { get; set; }

    /// <summary>
    /// Gets or sets the cabin temperature, between
    /// <see cref="TemperatureMin"/> and <see cref="TemperatureMax"/>.
    /// </summary>
    internal float CabinTemperature { get; set; }

    internal int CargoCapacity { get; set; }

    internal float Climb { get; set; }

    internal int EcmActive { get; set; }

    /// <summary>
    /// Gets or sets the energy banks, between <see cref="EnergyMin"/> and
    /// <see cref="EnergyMax"/>. It goes briefly below the minimum when a hit
    /// empties the banks, which is what ends the game.
    /// </summary>
    internal float Energy { get; set; }

    internal EnergyUnit EnergyUnit { get; set; }

    internal float Fuel { get; set; }

    internal bool HasDockingComputer { get; set; }

    internal bool HasECM { get; set; }

    internal bool HasEnergyBomb { get; set; }

    internal bool HasEscapeCapsule { get; set; }

    internal bool HasFuelScoop { get; set; }

    internal bool HasGalacticHyperdrive { get; set; }

    internal bool IsClimbing { get; set; }

    internal bool IsRolling { get; set; }

    internal ILaser LaserFront { get; set; } = new LaserNone();

    internal ILaser LaserLeft { get; set; } = new LaserNone();

    internal ILaser LaserRear { get; set; } = new LaserNone();

    internal ILaser LaserRight { get; set; } = new LaserNone();

    internal float MaxClimb { get; } = 8;

    internal float MaxFuel { get; } = 7;

    internal float MaxRoll { get; } = 31;

    // 0.27 Light Mach
    internal float MaxSpeed { get; } = 40;

    internal int MissileCount { get; set; }

    internal float Roll { get; set; }

    /// <summary>
    /// Gets or sets the forward shield, between <see cref="ShieldMin"/> and
    /// <see cref="ShieldMax"/>.
    /// </summary>
    internal float ShieldFront { get; set; }

    /// <summary>
    /// Gets or sets the aft shield, between <see cref="ShieldMin"/> and
    /// <see cref="ShieldMax"/>.
    /// </summary>
    internal float ShieldRear { get; set; }

    internal float Speed { get; set; }

    /// <summary>
    /// Deplete the shields.  Drain the energy banks if the shields fail.
    /// </summary>
    /// <param name="damage">Amount of damage, in the original's 0-255 units.</param>
    /// <param name="front">True if front, false if rear.</param>
    internal void DamageShip(float damage, bool front)
    {
        Debug.Assert(damage > 0, "Damage should be positive.");

        float shield = front ? ShieldFront : ShieldRear;

        shield -= damage * ShieldStep;
        if (shield < ShieldMin)
        {
            // Shields and banks are on the same scale, so whatever the shield
            // couldn't absorb carries straight over as a fraction.
            Energy += shield;
            shield = ShieldMin;
        }

        if (front)
        {
            ShieldFront = shield;
        }
        else
        {
            ShieldRear = shield;
        }
    }

    internal void DecreaseClimb() => Climb = Math.Clamp(Climb - 1, -MaxClimb, MaxClimb);

    /// <summary>
    /// Drain the energy banks. The amount is in the original's 0-255 units, so
    /// it is scaled down to the fraction <see cref="Energy"/> holds.
    /// </summary>
    internal void DecreaseEnergy(float amount) => Energy += amount * EnergyStep;

    internal void DecreaseRoll() => Roll = Math.Clamp(Roll - 1, -MaxRoll, MaxRoll);

    internal void DecreaseSpeed() => Speed = Math.Clamp(Speed - 1, 0, MaxSpeed);

    internal void IncreaseClimb() => Climb = Math.Clamp(Climb + 1, -MaxClimb, MaxClimb);

    internal void IncreaseRoll() => Roll = Math.Clamp(Roll + 1, -MaxRoll, MaxRoll);

    internal void IncreaseSpeed() => Speed = Math.Clamp(Speed + 1, 0, MaxSpeed);

    internal bool IsEnergyLow() => Energy < LowEnergy;

    internal void LevelOut()
    {
        if (!IsRolling)
        {
            if (Roll > 0)
            {
                DecreaseRoll();
            }
            else if (Roll < 0)
            {
                IncreaseRoll();
            }
        }

        if (!IsClimbing)
        {
            if (Climb > 0)
            {
                DecreaseClimb();
            }
            else if (Climb < 0)
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
        // The banks only feed the shields while they are over half full.
        if (Energy > EnergyMax / 2)
        {
            if (ShieldFront < ShieldMax)
            {
                ShieldFront = Math.Min(ShieldFront + ShieldStep, ShieldMax);
                Energy = Math.Clamp(Energy - EnergyStep, EnergyMin, EnergyMax);
            }

            if (ShieldRear < ShieldMax)
            {
                ShieldRear = Math.Min(ShieldRear + ShieldStep, ShieldMax);
                Energy = Math.Clamp(Energy - EnergyStep, EnergyMin, EnergyMax);
            }
        }

        Energy = Math.Clamp(Energy + ((1 + (int)EnergyUnit) * EnergyStep), EnergyMin, EnergyMax);
    }

    internal void Reset()
    {
        Altitude = AltitudeMax;
        CabinTemperature = AmbientTemperature;
        Roll = 0;
        Climb = 0;
        Speed = 0;
        Energy = EnergyMax;
        ShieldFront = ShieldMax;
        ShieldRear = ShieldMax;
    }
}
