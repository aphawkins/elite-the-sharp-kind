// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

namespace EliteSharpLib.Ships;

/// <summary>
/// What a thing in space is, as against what it is currently doing. These are
/// the facts the old <c>ShipType</c> enum stated by where a ship sat in its
/// numbering - "above a Rock Splinter", "above zero" - and which a row in the
/// ship table states outright now.
/// <para>
/// Deliberately not part of <see cref="ShipProperties"/>: those are the ship's
/// mood and are assigned wholesale in a dozen places (an angered pack hunter
/// becomes <c>Angry</c> and nothing else), so a fact about what a ship *is*
/// would not survive being provoked. A trait is set when the object is built
/// and never again.
/// </para>
/// </summary>
[Flags]
internal enum ShipTraits
{
    /// <summary>Nothing in particular: the player's own autopilot proxy.</summary>
    None = 0,

    /// <summary>
    /// Somebody is flying it, so it can be provoked. The original's "above a
    /// Rock Splinter" band. Note the two stations disagree - the Dodec is in
    /// that band and the Coriolis is not - which is an accident of the
    /// original's numbering, kept because a missile fired at one angers it and
    /// at the other does not.
    /// </summary>
    Crewed = 1 << 0,

    /// <summary>
    /// Enough mass to stop a jump. Everything but the inert junk: rubble,
    /// cargo, alloys and an escape capsule are too small to hold the player up.
    /// </summary>
    MassLocks = 1 << 1,

    /// <summary>
    /// A planet or a sun rather than anything flying. The original said this
    /// with a negative ship number; neither comes from the ship table.
    /// </summary>
    Stellar = 1 << 2,
}
