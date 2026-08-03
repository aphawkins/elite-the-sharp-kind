// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using Useful;

namespace EliteSharp.Abstractions.Views;

/// <summary>
/// What each sort of ship is painted, in this tier's colours. There is one of
/// these per rendition and everything that colours a ship reads it, so a ship is
/// the same colour wherever it appears - on the scanner and in the beam it
/// fires. Before this existed the two disagreed at 16-bit, where a police ship
/// showed purple on the scanner and fired cyan.
/// <para>
/// The two tiers need not agree with each other, and do not: 8-bit police are
/// cyan because that palette's one purple is already the missile, while 16-bit
/// has both a purple and a lilac and can afford to spend them differently.
/// </para>
/// </summary>
/// <param name="Default">Traders, pirates and anything with no other claim.</param>
/// <param name="Station">The space station.</param>
/// <param name="Missile">A missile in flight.</param>
/// <param name="Police">Vipers.</param>
/// <param name="Hostile">Anything currently attacking.</param>
public sealed record ShipColours(
    FastColor Default,
    FastColor Station,
    FastColor Missile,
    FastColor Police,
    FastColor Hostile)
{
    /// <summary>
    /// Gets the colour for one class of ship.
    /// </summary>
    /// <param name="shipClass">The class to colour.</param>
    /// <returns>That class's colour in this tier's palette.</returns>
    public FastColor For(ShipClass shipClass) => shipClass switch
    {
        ShipClass.Station => Station,
        ShipClass.Missile => Missile,
        ShipClass.Police => Police,
        ShipClass.Hostile => Hostile,
        _ => Default,
    };
}
