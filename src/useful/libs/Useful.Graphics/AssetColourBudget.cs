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
        Dictionary<string, uint[]> outsidePalette,
        Dictionary<string, uint[]> offGrid)
    {
        Tier = tier;
        ColourCount = colourCount;
        PartialAlphaCount = partialAlphaCount;
        PerAsset = perAsset;
        OutsidePalette = outsidePalette;
        OffGrid = offGrid;
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

    // Per asset - plus "Palette" for the named palette itself - the opaque
    // colours the tier's DAC could not have produced. Empty for a tier whose
    // channels are already a full eight bits.
    public IReadOnlyDictionary<string, uint[]> OffGrid { get; }

    public int Cap => MaxColours(Tier);

    public bool IsWithinBudget => ColourCount <= Cap;

    public bool IsWithinPalette => !PaletteNamesEveryColour(Tier) || OutsidePalette.Count == 0;

    public bool IsOnColourGrid => OffGrid.Count == 0;

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

    // Bits per colour channel the tier's hardware could actually drive. The
    // 16-bit machines this tier stands in for ran a 12-bit DAC - four bits
    // each of red, green and blue, giving 4096 colours - so a channel there
    // may only hold one of sixteen levels. The 8-bit tier is capped by its
    // 16-entry palette instead, not by channel depth, so it keeps all eight.
    public static int ChannelBits(SystemTier tier) => tier switch
    {
        SystemTier.EightBit => 8,
        SystemTier.SixteenBit => 4,
        _ => throw new UsefulException($"No channel depth is defined for tier {tier}."),
    };

    // Whether every channel of a colour sits on one of the tier's levels. An
    // n-bit channel widens to eight by replication, so 4 bits gives 0x00,
    // 0x11 ... 0xFF - the expansion that reaches a true white, unlike a plain
    // left shift, which tops out at 0xF0. Alpha is not a channel of the DAC
    // and is checked separately as PartialAlphaCount.
    public static bool IsOnGrid(uint argb, int channelBits)
    {
        if (channelBits >= 8)
        {
            return true;
        }

        int top = (1 << channelBits) - 1;

        for (int shift = 0; shift <= 16; shift += 8)
        {
            int channel = (int)((argb >> shift) & 0xFF);

            // The nearest level's own expansion has to come back to the
            // channel itself, otherwise it sits between two of them.
            if (NearestLevel(channel, top) != channel)
            {
                return false;
            }
        }

        return true;
    }

    // The tier's level closest to an eight-bit channel value, widened back to
    // eight bits. Snapping an asset to the grid means running every channel
    // through this.
    public static int NearestLevel(int channel, int topLevel)
        => (int)Math.Round((double)channel * topLevel / 255, MidpointRounding.AwayFromZero) * 255 / topLevel;
}
