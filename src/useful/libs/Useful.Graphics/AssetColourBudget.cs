// 'Useful Libraries' - Andy Hawkins 2023-2026.

using Useful.Assets;

namespace Useful.Graphics;

// The outcome of checking one game's asset set against its tier's colour
// budget. The cap applies to the union across the whole set, not per image:
// a tier stands in for one machine's palette.
public sealed class AssetColourBudget
{
    internal AssetColourBudget(SystemTier tier, int colourCount, int partialAlphaCount, Dictionary<string, int> perAsset)
    {
        Tier = tier;
        ColourCount = colourCount;
        PartialAlphaCount = partialAlphaCount;
        PerAsset = perAsset;
    }

    public SystemTier Tier { get; }

    // Distinct opaque colours across the whole set. Fully transparent pixels
    // are excluded - they carry no colour.
    public int ColourCount { get; }

    // Pixels whose alpha is neither 0 nor 255. The renderer treats
    // transparency as binary, so anything in between is an authoring mistake.
    public int PartialAlphaCount { get; }

    public IReadOnlyDictionary<string, int> PerAsset { get; }

    public int Cap => MaxColours(Tier);

    public bool IsWithinBudget => ColourCount <= Cap;

    public static int MaxColours(SystemTier tier) => tier switch
    {
        SystemTier.EightBit => 16,
        SystemTier.SixteenBit => 4096,
        _ => throw new UsefulException($"No colour budget is defined for tier {tier}."),
    };
}
