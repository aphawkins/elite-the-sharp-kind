// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

namespace EliteSharpLib.Ships;

/// <summary>
/// The goods that a destroyed ship or a piece of space junk leaves behind. Each
/// is named by the ship it comes from - the Alloy, the Escape Capsule, the
/// Rock Splinter and the Tharglet - as that row's <c>scoopedType</c> in
/// <c>ships.json</c>. Gathered here so
/// a goods set can be checked at startup for providing all four: a set that
/// does not is one where scooping a wreck would ask for cargo that does not
/// exist.
/// </summary>
internal static class ScoopableGoods
{
    /// <summary>
    /// Gets the good ids the ships expect a goods set to provide.
    /// </summary>
    internal static IReadOnlyList<string> Required { get; } =
    [
        "Alloys",
        "Slaves",
        "Minerals",
        "AlienItems",
    ];
}
