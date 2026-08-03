// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using EliteSharp.Abstractions.Views;
using EliteSharpLib.Conflict;
using EliteSharpLib.Ships;
using Useful.Maths;

namespace EliteSharpLib.Views;

/// <summary>
/// What the HUD says: the dial readings, the missile indicators, where the
/// compass points and what is on the scanner. Every screen works this way -
/// the game reads its own state here and the tier's pack draws what comes out,
/// which is what lets the HUD belong to a tier rather than to the game.
/// </summary>
internal sealed class ScannerController
{
    // The four energy banks fill from the bottom one up.
    private const int EnergyBankCount = 4;

    // Four indicators is all there is room for, however many are aboard.
    private const int MaxMissileIndicators = 4;

    private readonly GameState _gameState;
    private readonly PlayerShip _ship;
    private readonly Universe _universe;
    private readonly Combat _combat;
    private readonly IView<ScannerModel> _view;

    internal ScannerController(
        GameState gameState,
        PlayerShip ship,
        Universe universe,
        Combat combat,
        IView<ScannerModel> view)
    {
        _gameState = gameState;
        _ship = ship;
        _universe = universe;
        _combat = combat;
        _view = view;
    }

    internal void UpdateConsole() => _view.Draw(BuildModel());

    // Exposed for tests: everything the HUD shows, with no layout applied.
    internal ScannerModel BuildModel() => new(
        _gameState.IsDocked,
        Dial(_ship.ShieldFront, PlayerShip.ShieldMin),
        Dial(_ship.ShieldRear, PlayerShip.ShieldMin),
        _ship.Fuel > 0 ? _ship.Fuel / _ship.MaxFuel : 0,
        Dial(_ship.CabinTemperature, PlayerShip.TemperatureMin),
        Dial(_gameState.LaserTemp, GameState.LaserTempMin),
        Dial(_ship.Altitude, PlayerShip.AltitudeMin),
        EnergyBanks(),
        _ship.Speed / _ship.MaxSpeed,
        _ship.Speed > (_ship.MaxSpeed * 2 / 3),

        // Roll reads the other way round: a roll to starboard slides left.
        -_ship.Roll / _ship.MaxRoll,
        _ship.Climb / _ship.MaxClimb,
        Missiles(),
        !_gameState.IsDocked && _universe.IsStationPresent,
        _ship.EcmActive != 0,
        Compass(),
        Blips());

    // A dial below the least it is worth showing reads empty rather than
    // drawing a sliver.
    private static float Dial(float reading, float minimum) => reading > minimum ? reading : 0;

    private static ShipClass ClassOf(IObject obj)
        => obj.Flags.HasFlag(ShipProperties.Station)
            ? ShipClass.Station
            : obj.Type == ShipType.Missile
                ? ShipClass.Missile
                : obj.Flags.HasFlag(ShipProperties.Police)
                    ? ShipClass.Police
                    : obj.Flags.HasFlag(ShipProperties.Hostile) ? ShipClass.Hostile : ShipClass.Default;

    private float[] EnergyBanks()
    {
        float[] banks = new float[EnergyBankCount];

        for (int i = 0; i < EnergyBankCount; i++)
        {
            // Bank 0 is the topmost, so it holds the last quarter to fill.
            banks[i] = Math.Clamp((_ship.Energy * EnergyBankCount) - (EnergyBankCount - 1 - i), 0, 1);
        }

        return banks;
    }

    // The armed one leads, and is the only one that shows whether anything is
    // locked; the rest are stowed.
    private MissileIndicator[] Missiles()
    {
        int count = Math.Min(_ship.MissileCount, MaxMissileIndicators);

        if (count == 0)
        {
            return [];
        }

        MissileIndicator[] missiles = new MissileIndicator[count];
        Array.Fill(missiles, MissileIndicator.Stowed);

        if (_combat.IsMissileArmed)
        {
            missiles[0] = _combat.MissileTarget == null ? MissileIndicator.Armed : MissileIndicator.Locked;
        }

        return missiles;
    }

    private CompassReading? Compass()
    {
        if (_gameState.IsDocked || _gameState.InWitchspace)
        {
            return null;
        }

        IObject? obj = _universe.IsStationPresent ? _universe.StationOrSun : _universe.Planet;

        if (obj == null)
        {
            return null;
        }

        Vector4 dest = VectorMaths.UnitVector(obj.Location);

        return float.IsNaN(dest.X) ? null : new CompassReading(new(dest.X, dest.Y), dest.Z < 0);
    }

    // Everything the scanner could plot, in its own units and unclipped: how
    // far off centre is too far depends on the tier's scanner size, so that
    // is left to the view.
    private ScannerBlip[] Blips()
    {
        if (_gameState.IsDocked)
        {
            return [];
        }

        List<ScannerBlip> blips = [];

        foreach (IObject obj in _universe.GetAllObjects())
        {
            if ((obj.Type <= 0) ||
                obj.Flags.HasFlag(ShipProperties.Dead) ||
                obj.Flags.HasFlag(ShipProperties.Cloaked))
            {
                continue;
            }

            float stickY = -obj.Location.Z / 1024;

            blips.Add(new(
                obj.Location.X / 256,
                stickY,
                stickY - (obj.Location.Y / 512),
                ClassOf(obj)));
        }

        return [.. blips];
    }
}
