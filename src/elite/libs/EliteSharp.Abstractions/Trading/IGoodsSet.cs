// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

namespace EliteSharp.Abstractions.Trading;

/// <summary>
/// The wares a game of Elite trades in, written in its own assembly and found
/// at startup - the same door the missions and the renditions come through. A
/// different economy is then an assembly rather than a branch in the game.
/// <para>
/// A set holds no state: it declares its goods once and the game keeps the
/// standings. Unlike a mission and like a rendition, a set is not optional -
/// there is no market without one, so the configured set has to be installed or
/// the game will not start.
/// </para>
/// </summary>
public interface IGoodsSet
{
    /// <summary>
    /// Gets the name this set is known by - in the config file, and in the
    /// folder its assembly sits in. It has to stay put across releases: a
    /// renamed set is one the commander's config no longer selects.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the goods, in the order the market screen lists them. The game
    /// shows this order and the save file records positions against it, so a
    /// set settles its own order rather than being sorted into one.
    /// </summary>
    public IReadOnlyList<Good> Goods { get; }
}
