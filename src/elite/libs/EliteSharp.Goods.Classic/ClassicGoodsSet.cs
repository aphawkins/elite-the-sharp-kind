// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Abstractions.Trading;

namespace EliteSharp.Goods.Classic;

/// <summary>
/// The seventeen goods the original traded in. Everything the game used to hold
/// about them in four different places - the market table, the starting
/// station's shelf, which of them the police care about, and which a canister
/// can contain - is gathered here as one declaration.
/// <para>
/// The order is the order the market screen lists them, and it is also the
/// order the original numbered them in, which two pieces of behaviour used to
/// depend on. Nothing depends on it now except what the commander reads down.
/// </para>
/// </summary>
public sealed class ClassicGoodsSet : IGoodsSet
{
    private const string Grams = "g";
    private const string Kilograms = "Kg";
    private const string Tonnes = "t";

    /// <inheritdoc/>
    public string Name => "Classic";

    /// <inheritdoc/>
    // Id              Name            Price  Econ  Qty  Mask  Units      Hold   Stock  Sold   Contra  Dropped
    public IReadOnlyList<Good> Goods { get; } =
    [
        new("Food",         "Food",          1.9f,  -2,    6,    1, Tonnes,    true,  0x10,  true,      0, true),
        new("Textiles",     "Textiles",      2.0f,  -1,   10,    3, Tonnes,    true,  0x0F,  true,      0, true),
        new("Radioactives", "Radioactives",  6.5f,  -3,    2,    7, Tonnes,    true,  0x11,  true,      0, true),
        new("Slaves",       "Slaves",        4.0f,  -5,  226,   31, Tonnes,    true,  0x00,  true,      2, true),
        new("LiquorWines",  "Liquor/Wines",  8.3f,  -5,  251,   15, Tonnes,    true,  0x03,  true,      0, true),
        new("Luxuries",     "Luxuries",     19.6f,   8,   54,    3, Tonnes,    true,  0x1C,  true,      0, true),
        new("Narcotics",    "Narcotics",    23.5f,  29,    8,  120, Tonnes,    true,  0x0E,  true,      2, true),
        new("Computers",    "Computers",    15.4f,  14,   56,    3, Tonnes,    true,  0x00,  true,      0, true),
        new("Machinery",    "Machinery",    11.7f,   6,   40,    7, Tonnes,    true,  0x00,  true,      0, false),
        new("Alloys",       "Alloys",        7.8f,   1,   17,   31, Tonnes,    true,  0x0A,  true,      0, false),
        new("Firearms",     "Firearms",     12.4f,  13,   29,    7, Tonnes,    true,  0x00,  true,      1, false),
        new("Furs",         "Furs",         17.6f,  -9,  220,   63, Tonnes,    true,  0x11,  true,      0, false),
        new("Minerals",     "Minerals",      3.2f,  -1,   53,    3, Tonnes,    true,  0x3A,  true,      0, false),
        new("Gold",         "Gold",          9.7f,  -1,   66,    7, Kilograms, false, 0x07,  true,      0, false),
        new("Platinum",     "Platinum",     17.1f,  -2,   55,   31, Kilograms, false, 0x09,  true,      0, false),
        new("GemStones",    "Gem-Stones",    4.5f,  -1,  250,   15, Grams,     false, 0x08,  true,      0, false),
        new("AlienItems",   "Alien Items",   5.3f,  15,  192,    7, Tonnes,    true,  0x00,  false,     0, false),
    ];
}
