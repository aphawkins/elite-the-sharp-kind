// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

namespace EliteSharpLib.Ships;

/// <summary>
/// The ids the game itself names. A ship is a row in <c>ships.json</c> now, so
/// the game recognises one by its id rather than by a member of an enum it had
/// to be compiled against - which is what lets a table describe a ship nobody
/// built the game for.
/// <para>
/// Most of what the game asks about a ship it asks through
/// <see cref="ShipProperties"/>, which the table states: whether it is crewed,
/// whether it is a station, whether it mass-locks. What is left here is the
/// handful the game has behaviour for by name - the Thargoid mothership its
/// drones die with, the two mission ships, the wreckage a kill leaves. A table
/// that does not describe one of these loses that behaviour; it does not stop
/// the game, in the way a goods set missing <see cref="ScoopableGoods"/> would
/// stop the market.
/// </para>
/// <para>
/// <see cref="Planet"/> and <see cref="Sun"/> are not ships and are not in the
/// table. They are named here because the two of them are the only objects in
/// space that come from somewhere else, and the game does have to tell them
/// apart.
/// </para>
/// </summary>
internal static class ObjectIds
{
    /// <summary>
    /// An object that is nothing in particular: the player's own proxy, which
    /// the autopilot flies and which is in no ship table.
    /// </summary>
    internal const string None = "None";

    /// <summary>The planet the system is named for.</summary>
    internal const string Planet = "Planet";

    /// <summary>The system's star.</summary>
    internal const string Sun = "Sun";

    /// <summary>A missile in flight, which is nobody's traffic and nobody's prize.</summary>
    internal const string Missile = "Missile";

    /// <summary>The mothership a Tharglet dies without.</summary>
    internal const string Thargoid = "Thargoid";

    /// <summary>Its drone, which gives up when the mothership is gone.</summary>
    internal const string Tharglet = "Tharglet";

    /// <summary>The stolen prototype of the first mission.</summary>
    internal const string Constrictor = "Constrictor";

    /// <summary>The cloaked ship of the second.</summary>
    internal const string Cougar = "Cougar";

    /// <summary>The trader that panics and jumps out.</summary>
    internal const string Anaconda = "Anaconda";

    /// <summary>The police, who take exception to being shot at.</summary>
    internal const string Viper = "Viper";

    /// <summary>What a mining laser breaks into splinters.</summary>
    internal const string Asteroid = "Asteroid";

    /// <summary>What the mining laser leaves.</summary>
    internal const string RockSplinter = "RockSplinter";

    /// <summary>What a hull leaves.</summary>
    internal const string Alloy = "Alloy";

    /// <summary>What a hold leaves.</summary>
    internal const string CargoCannister = "CargoCannister";

    /// <summary>The asteroid that turns out to be somebody's home.</summary>
    internal const string RockHermit = "RockHermit";

    /// <summary>The two the station launches, which keep each other company.</summary>
    internal const string Shuttle = "Shuttle";

    /// <inheritdoc cref="Shuttle"/>
    internal const string Transporter = "Transporter";
}
