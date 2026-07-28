// 'Useful Libraries' - Andy Hawkins 2023-2026.

using Microsoft.Extensions.Logging;
using Useful.Assets;

namespace Useful.Graphics;

// Every image and bitmap font for the active tier, decoded once up front -
// assets are never loaded on demand. Both graphics backends take their
// bitmaps from here rather than decoding their own, so the tier's colour
// budget is checked in one place whichever backend is running. The bitmap
// fonts are decoded even for backends that draw text with TrueType fonts
// instead: they are part of the tier's set, so they count against its budget.
public sealed class AssetSet
{
    private AssetSet(Dictionary<string, FastBitmap> images, Dictionary<string, BitmapFont> bitmapFonts, AssetColourBudget budget)
    {
        Images = images;
        BitmapFonts = bitmapFonts;
        Budget = budget;
    }

    public Dictionary<string, FastBitmap> Images { get; }

    public Dictionary<string, BitmapFont> BitmapFonts { get; }

    public AssetColourBudget Budget { get; }

    public static AssetSet Load(IAssetLocator assetLocator) => Load(assetLocator, null);

    public static AssetSet Load(IAssetLocator assetLocator, ILogger? logger)
    {
        ArgumentNullException.ThrowIfNull(assetLocator);

        Dictionary<string, FastBitmap> images = assetLocator.ImagePaths.ToDictionary(
            x => x.Key,
            x => ImageReader.Read(x.Value));

        Dictionary<string, FastBitmap> fontBitmaps = assetLocator.FontBitmapPaths.ToDictionary(
            x => x.Key,
            x => ImageReader.Read(x.Value));

        AssetColourBudget budget = Measure(assetLocator.Tier, images, fontBitmaps);
        Report(budget, logger);

        return new(
            images,
            fontBitmaps.ToDictionary(x => x.Key, x => new BitmapFont(x.Value)),
            budget);
    }

    private static AssetColourBudget Measure(
        SystemTier tier,
        Dictionary<string, FastBitmap> images,
        Dictionary<string, FastBitmap> fontBitmaps)
    {
        HashSet<uint> distinct = [];
        Dictionary<string, int> perAsset = [];
        int partialAlpha = 0;

        foreach (KeyValuePair<string, FastBitmap> asset in images.Concat(fontBitmaps))
        {
            HashSet<uint> assetColours = [];

            for (int y = 0; y < asset.Value.Height; y++)
            {
                for (int x = 0; x < asset.Value.Width; x++)
                {
                    uint argb = asset.Value.GetPixel(x, y);
                    uint alpha = argb >> 24;

                    if (alpha == 0)
                    {
                        continue;
                    }

                    if (alpha != 0xFF)
                    {
                        partialAlpha++;
                    }

                    assetColours.Add(argb);
                }
            }

            perAsset[asset.Key] = assetColours.Count;
            distinct.UnionWith(assetColours);
        }

        return new(tier, distinct.Count, partialAlpha, perAsset);
    }

    // Warn-only for now: the committed 16-bit sets are over budget until
    // font2.bmp and atlas.bmp are posterised, at which point this becomes a
    // hard failure. See docs/asset-structure.md.
    private static void Report(AssetColourBudget budget, ILogger? logger)
    {
        if (logger is null)
        {
            return;
        }

        if (!budget.IsWithinBudget)
        {
            LogMessages.AssetColourBudgetExceeded(logger, budget.Tier, budget.ColourCount, budget.Cap);

            foreach (KeyValuePair<string, int> asset in budget.PerAsset.OrderByDescending(x => x.Value))
            {
                LogMessages.AssetColourCount(logger, asset.Key, asset.Value);
            }
        }

        if (budget.PartialAlphaCount > 0)
        {
            LogMessages.AssetPartialAlpha(logger, budget.PartialAlphaCount);
        }
    }
}
