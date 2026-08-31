// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

namespace EliteSharpLib.Ships;

/// <summary>
/// One ship as the table declares it: everything that used to be a class in
/// <c>Ships</c> whose whole body was a constructor setting these fields. None
/// of it changes while playing - the energy a ship has left, where it is and
/// what it is angry at are the game's to track, not the table's to state.
/// </summary>
/// <param name="Id">
/// What the game calls this ship: the name <see cref="IShipFactory.CreateShip"/>
/// is given, and the name the asset manifest files its model under.
/// </param>
/// <param name="Model">
/// Which model to draw it with. Usually its own <paramref name="Id"/>; a
/// variant that borrows another ship's mesh names that ship instead, so the
/// manifest lists only real model files.
/// </param>
/// <param name="Flags">How it behaves - who it flies for, and how boldly.</param>
/// <param name="Traits">What it is, as against what it is doing.</param>
/// <param name="Name">What the screens call it.</param>
/// <param name="ScoopedType">
/// The good scooping the wreck yields, for the junk that leaves any. Null for
/// everything else. The four that name one are gathered in
/// <see cref="ScoopableGoods"/>, which is what a goods set is checked against.
/// </param>
/// <param name="Bounty">The credits killing it pays.</param>
/// <param name="EnergyMax">Its shields at full.</param>
/// <param name="LaserFront">The strength of the laser it fires forward.</param>
/// <param name="LaserStrength">The damage its laser does.</param>
/// <param name="LootMax">How many canisters it can leave behind.</param>
/// <param name="MinDistance">How close it will fly to what it is chasing.</param>
/// <param name="MissilesMax">How many missiles it carries.</param>
/// <param name="Size">Its collision size, squared, as the original held it.</param>
/// <param name="VanishPoint">The distance at which it stops being drawn.</param>
/// <param name="VelocityMax">The fastest it flies.</param>
internal sealed record ShipDefinition(
    string Id,
    string Model,
    ShipProperties Flags,
    ShipTraits Traits,
    string Name,
    string? ScoopedType,
    float Bounty,
    int EnergyMax,
    int LaserFront,
    int LaserStrength,
    int LootMax,
    float MinDistance,
    int MissilesMax,
    float Size,
    int VanishPoint,
    float VelocityMax);
