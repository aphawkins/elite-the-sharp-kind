// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

namespace EliteSharpLib.Trader;

/// <summary>
/// One good as it stands right now: today's price, what is left on the shelf,
/// and what the commander is carrying. What the good <i>is</i> stays on its
/// <see cref="GoodsDefinition"/>, which nothing here can write to - the base
/// price and the mask used to be settable alongside the quantity, and nothing
/// ever set them.
/// </summary>
internal sealed class StockItem(GoodsDefinition definition)
{
    /// <summary>
    /// Gets what this good is: its name, its price before the market, and the
    /// rules the game trades it under.
    /// </summary>
    internal GoodsDefinition Definition { get; } = definition;

    /// <summary>
    /// Gets or sets how many units are in the hold.
    /// </summary>
    internal int CurrentCargo { get; set; }

    /// <summary>
    /// Gets or sets what one unit costs at this station today.
    /// </summary>
    internal float CurrentPrice { get; set; }

    /// <summary>
    /// Gets or sets how many units this station still has to sell.
    /// </summary>
    internal int CurrentQuantity { get; set; }

    /// <summary>
    /// Gets or sets the quantity the station is restored to on returning to it,
    /// which a save file carries so a market does not restock itself by being
    /// left.
    /// </summary>
    internal int StationStock { get; set; }
}
