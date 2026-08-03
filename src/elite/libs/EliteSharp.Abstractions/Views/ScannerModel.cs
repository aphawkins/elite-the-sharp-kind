// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

namespace EliteSharp.Abstractions.Views;

/// <summary>
/// Everything the HUD draws, with no layout applied. The dials arrive as
/// fractions of full rather than as pixel lengths, and the blips in the
/// scanner's own units around its centre, so each tier applies its own bitmap
/// offsets, extents and colours.
/// <para>
/// A dial reading of zero is a dial with nothing to draw - the controller has
/// already applied the minimum each one is worth showing - so a view draws
/// what it is given without knowing what any of it means.
/// </para>
/// </summary>
/// <param name="IsDocked">
/// Docked, the dials are still drawn but the scanner, compass and indicators
/// are not: there is nothing out there to plot.
/// </param>
/// <param name="ShieldFront">Front shield, 0 to 1.</param>
/// <param name="ShieldRear">Rear shield, 0 to 1.</param>
/// <param name="Fuel">Fuel as a fraction of a full tank, 0 to 1.</param>
/// <param name="CabinTemperature">Cabin temperature, 0 to 1.</param>
/// <param name="LaserTemperature">Laser temperature, 0 to 1.</param>
/// <param name="Altitude">Altitude, 0 to 1.</param>
/// <param name="EnergyBanks">
/// The four banks' fills, 0 to 1 each, topmost first. They fill from the
/// bottom bank up, which the controller has already worked out.
/// </param>
/// <param name="Speed">Speed as a fraction of maximum, 0 to 1.</param>
/// <param name="IsSpeedWarning">Whether speed is high enough to draw in the warning colour.</param>
/// <param name="Roll">Roll indicator offset from centre, -1 to 1, already reversed for display.</param>
/// <param name="Climb">Climb indicator offset from centre, -1 to 1.</param>
/// <param name="Missiles">The missile indicators, left to right.</param>
/// <param name="IsStationPresent">Whether the station indicator is lit.</param>
/// <param name="IsEcmActive">Whether the E.C.M. indicator is lit.</param>
/// <param name="Compass">
/// Where the compass points, or null when there is nothing to point at.
/// </param>
/// <param name="Blips">
/// What is out there, in the scanner's units relative to its centre and
/// unclipped - each tier's scanner is a different size, so how far off centre
/// is too far is the view's own business.
/// </param>
public sealed record ScannerModel(
    bool IsDocked,
    float ShieldFront,
    float ShieldRear,
    float Fuel,
    float CabinTemperature,
    float LaserTemperature,
    float Altitude,
    IReadOnlyList<float> EnergyBanks,
    float Speed,
    bool IsSpeedWarning,
    float Roll,
    float Climb,
    IReadOnlyList<MissileIndicator> Missiles,
    bool IsStationPresent,
    bool IsEcmActive,
    CompassReading? Compass,
    IReadOnlyList<ScannerBlip> Blips);
