// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Abstractions.Trading;

namespace EliteSharp.Goods.TestPlugin;

/// <summary>
/// A goods set with nothing to it beyond being found: five wares, one of each
/// kind the game cares about - a legal one, a contraband one, a weightless one,
/// a wreck-only one, and one a canister can drop. It exists to prove that a
/// class implementing <see cref="IGoodsSet"/> in an assembly that knows nothing
/// of the game or of MEF is discovered and can be traded.
/// <para>
/// It provides the four goods that ships drop - Alloys, Slaves, Minerals,
/// AlienItems - because a set that does not is turned away at startup, and this
/// one is meant to load.
/// </para>
/// </summary>
public sealed class BarterGoodsSet : IGoodsSet
{
    /// <inheritdoc/>
    public string Name => "Barter";

    /// <inheritdoc/>
    public IReadOnlyList<Good> Goods { get; } =
    [
        new("Grain",      "Grain",        2.0f, -3,  10,  7, "t", true,  0x12, true,  0, true),
        new("Slaves",     "Slaves",       4.0f, -5, 200, 31, "t", true,  0x00, true,  2, true),
        new("Minerals",   "Minerals",     3.0f, -1,  50,  3, "t", true,  0x20, true,  0, false),
        new("Alloys",     "Alloys",       8.0f,  1,  20, 31, "t", true,  0x08, true,  0, false),
        new("AlienItems", "Alien Items",  5.0f, 15, 100,  7, "t", true,  0x00, false, 0, false),
    ];
}
