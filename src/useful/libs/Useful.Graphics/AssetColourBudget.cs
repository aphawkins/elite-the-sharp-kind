// 'Useful Libraries' - Andy Hawkins 2023-2026.

using Useful.Assets;

namespace Useful.Graphics;

// The outcome of checking one game's asset set against its tier's colour
// budget. The cap applies to the union across the whole set, not per image:
// a tier stands in for one machine's palette.
public sealed class AssetColourBudget
{
    internal AssetColourBudget(
        SystemTier tier,
        int colourCount,
        int partialAlphaCount,
        Dictionary<string, int> perAsset,
        Dictionary<string, uint[]> outsidePalette)
    {
        Tier = tier;
        ColourCount = colourCount;
        PartialAlphaCount = partialAlphaCount;
        PerAsset = perAsset;
        OutsidePalette = outsidePalette;
    }

    public SystemTier Tier { get; }

    // Distinct opaque colours across the whole set. Fully transparent pixels
    // are excluded - they carry no colour.
    public int ColourCount { get; }

    // Pixels whose alpha is neither 0 nor 255. The renderer treats
    // transparency as binary, so anything in between is an authoring mistake.
    public int PartialAlphaCount { get; }

    public IReadOnlyDictionary<string, int> PerAsset { get; }

    // Per asset, the opaque colours it uses that the palette does not name.
    // Populated for every tier so it can be logged, but only enforced where
    // PaletteNamesEveryColour says the palette is the whole colour set.
    public IReadOnlyDictionary<string, uint[]> OutsidePalette { get; }

    public int Cap => MaxColours(Tier);

    public bool IsWithinBudget => ColourCount <= Cap;

    public bool IsWithinPalette => !PaletteNamesEveryColour(Tier) || OutsidePalette.Count == 0;

    public static int MaxColours(SystemTier tier) => tier switch
    {
        SystemTier.EightBit => 16,
        SystemTier.SixteenBit => 4096,
        _ => throw new UsefulException($"No colour budget is defined for tier {tier}."),
    };

    // Whether the palette is the tier's complete colour set, so a bitmap may
    // only use colours it names. True for 8-bit because the hardware it
    // stands in for was indexed-colour - every pixel *was* a palette entry,
    // and one palette served the whole display. 16-bit hardware is
    // direct-colour, where the palette is only a set of names the geometry
    // draws with and bitmaps are free of it. See docs/asset-structure.md.
    public static bool PaletteNamesEveryColour(SystemTier tier) => tier switch
    {
        SystemTier.EightBit => true,
        SystemTier.SixteenBit => false,
        _ => throw new UsefulException($"No palette rule is defined for tier {tier}."),
    };
}
