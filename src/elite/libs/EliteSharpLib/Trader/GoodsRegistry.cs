// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Abstractions.Trading;
using EliteSharpLib.Ships;
using Microsoft.Extensions.Logging;

namespace EliteSharpLib.Trader;

/// <summary>
/// The goods set the game is trading, checked once at startup so nothing after
/// has to. A set arrives from a plugin and could say anything; the two things
/// the rest of the game takes on trust - that a good's id is a usable key, and
/// that the goods a wrecked ship drops all exist - are settled here.
/// </summary>
internal sealed class GoodsRegistry
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GoodsRegistry"/> class.
    /// </summary>
    /// <param name="set">The goods set the loader chose.</param>
    /// <param name="logger">Where a failed check is reported before it is thrown.</param>
    /// <exception cref="InvalidOperationException">
    /// The set names a good with no id or the same id twice, or leaves out one
    /// of the goods a ship drops. Either way the game cannot trade it.
    /// </exception>
    public GoodsRegistry(IGoodsSet set, ILogger<GoodsRegistry> logger)
    {
        ArgumentNullException.ThrowIfNull(set);

        Name = set.Name;
        Goods = [.. set.Goods];

        HashSet<string> ids = new(StringComparer.Ordinal);
        foreach (Good good in Goods)
        {
            if (string.IsNullOrWhiteSpace(good.Id) || !ids.Add(good.Id))
            {
                string id = string.IsNullOrWhiteSpace(good.Id) ? "(none)" : good.Id;
                LogMessages.GoodsSetHasBadIds(logger, Name, id);
                throw new InvalidOperationException(
                    $"The '{Name}' goods set names a good '{id}' more than once, or with no name at all.");
            }
        }

        string[] missing = [.. ScoopableGoods.Required.Where(required => !ids.Contains(required))];
        if (missing.Length > 0)
        {
            string names = string.Join(", ", missing);
            LogMessages.GoodsSetMissingShipDrops(logger, Name, names);
            throw new InvalidOperationException(
                $"The '{Name}' goods set is missing goods that ships drop: {names}.");
        }
    }

    /// <summary>
    /// Gets the name of the set being traded, for the save file to record and
    /// for a mismatch to name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the goods, in the order the set listed them.
    /// </summary>
    public IReadOnlyList<Good> Goods { get; }
}
