// 'Useful Libraries' - Andy Hawkins 2023-2026.

using System.Globalization;
using Microsoft.Extensions.Logging;
using Useful.Assets;
using Useful.Assets.Palettes;

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

        RequireEveryFile(assetLocator);

        Dictionary<string, FastBitmap> images = assetLocator.ImagePaths.ToDictionary(
            x => x.Key,
            x => ImageReader.Read(x.Value));

        Dictionary<string, FastBitmap> fontBitmaps = assetLocator.FontBitmaps.ToDictionary(
            x => x.Key,
            x => ImageReader.Read(x.Value.Path));

        AssetColourBudget budget = Measure(assetLocator.Tier, images, fontBitmaps, PaletteColours(assetLocator));
        Validate(budget, logger);

        return new(
            images,
            fontBitmaps.ToDictionary(x => x.Key, x => new BitmapFont(x.Value, assetLocator.FontBitmaps[x.Key])),
            budget);
    }

    // Reports every missing file at once. Decoding them one at a time
    // surfaces only the first, which turns filling in a new tier's asset
    // set into a game of whack-a-mole.
    private static void RequireEveryFile(IAssetLocator assetLocator)
    {
        string[] missing =
        [
            .. assetLocator.ImagePaths
                .Concat(assetLocator.FontBitmaps.ToDictionary(x => x.Key, x => x.Value.Path))
                .Where(x => !File.Exists(x.Value))
                .Select(x => $"{x.Key} ({Path.GetFileName(x.Value)})")
                .Order(),
        ];

        if (missing.Length > 0)
        {
            throw new UsefulException(
                $"The {assetLocator.Tier} asset set is missing {missing.Length} file(s): {string.Join(", ", missing)}.");
        }
    }

    // The named palette counts against the tier's budget like any other
    // asset: colours the game draws with are colours the tier has to be able
    // to show, whether they arrive as pixels or as a name.
    private static HashSet<uint> PaletteColours(IAssetLocator assetLocator)
        => string.IsNullOrEmpty(assetLocator.PalettePath) ? []
            : !File.Exists(assetLocator.PalettePath)
            ? throw new UsefulException(
                $"The {assetLocator.Tier} asset set is missing its palette: {assetLocator.PalettePath}")
            : [.. PaletteReader.Read(assetLocator.PalettePath).Values.Select(x => x.Argb)];

    private static AssetColourBudget Measure(
        SystemTier tier,
        Dictionary<string, FastBitmap> images,
        Dictionary<string, FastBitmap> fontBitmaps,
        HashSet<uint> paletteColours)
    {
        HashSet<uint> distinct = [.. paletteColours];
        Dictionary<string, int> perAsset = new() { ["Palette"] = paletteColours.Count };
        Dictionary<string, uint[]> outsidePalette = [];
        Dictionary<string, uint[]> offGrid = [];
        int channelBits = AssetColourBudget.ChannelBits(tier);
        int partialAlpha = 0;

        AddOffGrid(offGrid, "Palette", paletteColours, channelBits);

        foreach (KeyValuePair<string, FastBitmap> asset in images.Concat(fontBitmaps))
        {
            (HashSet<uint> assetColours, int assetPartialAlpha) = Scan(asset.Value);

            perAsset[asset.Key] = assetColours.Count;
            distinct.UnionWith(assetColours);
            partialAlpha += assetPartialAlpha;

            AddOffGrid(offGrid, asset.Key, assetColours, channelBits);

            // A set with no palette at all has nothing to be a subset of, so
            // there is nothing to report rather than everything.
            if (paletteColours.Count == 0)
            {
                continue;
            }

            uint[] unnamed = [.. assetColours.Where(x => !paletteColours.Contains(x)).Order()];

            if (unnamed.Length > 0)
            {
                outsidePalette[asset.Key] = unnamed;
            }
        }

        return new(tier, distinct.Count, partialAlpha, perAsset, outsidePalette, offGrid);
    }

    // Colours from one asset that the tier's DAC could not have produced.
    // Recorded rather than counted, because fixing one means knowing which
    // colour to snap.
    private static void AddOffGrid(
        Dictionary<string, uint[]> offGrid,
        string asset,
        HashSet<uint> colours,
        int channelBits)
    {
        uint[] strays = [.. colours.Where(x => !AssetColourBudget.IsOnGrid(x, channelBits)).Order()];

        if (strays.Length > 0)
        {
            offGrid[asset] = strays;
        }
    }

    // One asset's distinct opaque colours, and how many of its pixels have an
    // alpha the renderer cannot express.
    private static (HashSet<uint> Colours, int PartialAlpha) Scan(FastBitmap asset)
    {
        HashSet<uint> colours = [];
        int partialAlpha = 0;

        for (int y = 0; y < asset.Height; y++)
        {
            for (int x = 0; x < asset.Width; x++)
            {
                uint argb = asset.GetPixel(x, y).Argb;
                uint alpha = argb >> 24;

                if (alpha == 0)
                {
                    continue;
                }

                if (alpha != 0xFF)
                {
                    partialAlpha++;
                }

                colours.Add(argb);
            }
        }

        return (colours, partialAlpha);
    }

    // A set that breaks its tier's budget fails the game at startup rather
    // than rendering something the tier could never have displayed. The
    // per-asset breakdown is logged first, so the message names which files
    // to look at. See docs/asset-structure.md.
    private static void Validate(AssetColourBudget budget, ILogger? logger)
    {
        if (budget.IsWithinBudget && budget.IsWithinPalette && budget.IsOnColourGrid && budget.PartialAlphaCount == 0)
        {
            return;
        }

        if (logger is not null)
        {
            foreach (KeyValuePair<string, int> asset in budget.PerAsset.OrderByDescending(x => x.Value))
            {
                LogMessages.AssetColourCount(logger, asset.Key, asset.Value);
            }
        }

        if (!budget.IsWithinBudget)
        {
            string counted = $"{budget.ColourCount} distinct opaque colours against a cap of {budget.Cap}";
            throw new UsefulException($"Asset colour cap exceeded for the {budget.Tier} tier: {counted}.");
        }

        if (!budget.IsWithinPalette)
        {
            string rule = $"The {budget.Tier} palette is the tier's whole colour set, so every asset colour has to be one it names";

            throw new UsefulException(
                $"{rule}, but {budget.OutsidePalette.Count} asset(s) use colours it does not: {Offenders(budget.OutsidePalette)}.");
        }

        if (!budget.IsOnColourGrid)
        {
            int bits = AssetColourBudget.ChannelBits(budget.Tier);
            string drives = $"The {budget.Tier} tier drives {bits} bits per channel";
            string rule = $"{drives}, so every colour has to sit on one of its {1 << bits} levels per channel";

            throw new UsefulException(
                $"{rule}, but {budget.OffGrid.Count} asset(s) use colours between them: {Offenders(budget.OffGrid)}.");
        }

        string partial = $"{budget.PartialAlphaCount} asset pixels have partial alpha";
        throw new UsefulException($"{partial}; the renderer needs alpha to be either 0 or 255.");
    }

    // Names each asset with the colours at fault, so the message says which
    // file to open and what to look for in it.
    private static string Offenders(IReadOnlyDictionary<string, uint[]> assets)
        => string.Join(
            "; ",
            assets
                .OrderBy(x => x.Key, StringComparer.Ordinal)
                .Select(x => $"{x.Key} ({Hexes(x.Value)})"));

    private static string Hexes(uint[] colours)
        => string.Join(", ", colours.Select(c => c.ToString("X8", CultureInfo.InvariantCulture)));
}
