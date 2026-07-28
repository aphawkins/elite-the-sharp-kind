// 'Useful Libraries' - Andy Hawkins 2023-2026.

using System.Text.Json;

namespace Useful.Assets;

// The one place asset paths are built, so tier resolution lives here and
// nowhere else: tier-varying categories resolve to <Category>/<Tier>/<file>
// and fall back to <Category>/<file>, which is what keeps the tier-neutral
// categories - audio, models, TrueType fonts, tracks - from needing a copy
// per tier. The tier is fixed at construction, so IAssetLocator's consumers
// never have to know one exists.
public sealed class AssetLocator : IAssetLocator
{
    private const string AssetManifestFilename = "AssetManifest.json";
    private const string ImagesCategory = "Images";
    private const string FontsBitmapCategory = "FontsBitmap";
    private const string PaletteCategory = "Palette";
    private readonly AssetManifest _assetManifest = new();
    private readonly string _baseDirectory;

    internal AssetLocator(AssetManifest assetManifest, string baseDirectory, SystemTier tier)
    {
        ArgumentNullException.ThrowIfNull(assetManifest);

        if (assetManifest.Tiers.Count > 0 && !assetManifest.Tiers.Contains(tier))
        {
            throw new UsefulException(
                $"Asset tier {tier} is not one of the tiers this manifest ships: {string.Join(", ", assetManifest.Tiers)}.");
        }

        _assetManifest = assetManifest;
        _baseDirectory = Path.Combine(baseDirectory, "Assets");
        Tier = tier;
    }

    public SystemTier Tier { get; }

    public string PalettePath => TierPath(PaletteCategory, _assetManifest.Palette);

    public IDictionary<string, BitmapFontAsset> FontBitmaps
        => _assetManifest.FontsBitmap.ToDictionary(
            x => x.Key,
            x => new BitmapFontAsset(TierPath(FontsBitmapCategory, x.Value.File), x.Value));

    public IDictionary<string, TrueTypeFontAsset> FontTrueTypes
        => _assetManifest.FontsTrueType.ToDictionary(
            x => x.Key,
            x => new TrueTypeFontAsset(Path.Combine(_baseDirectory, "FontsTrueType", x.Value.File), x.Value.PointSize));

    public IDictionary<string, string> ImagePaths
        => _assetManifest.Images.ToDictionary(x => x.Key, x => TierPath(ImagesCategory, x.Value));

    public IDictionary<string, string> MusicPaths
        => _assetManifest.Music.ToDictionary(x => x.Key, x => Path.Combine(_baseDirectory, "Music", x.Value));

    public IDictionary<string, string> SfxPaths
        => _assetManifest.Sfx.ToDictionary(x => x.Key, x => Path.Combine(_baseDirectory, "SFX", x.Value));

    public IDictionary<string, string> SoundFontPaths
        => _assetManifest.SoundFonts.ToDictionary(x => x.Key, x => Path.Combine(_baseDirectory, "SoundFonts", x.Value));

    public IDictionary<string, string> ModelPaths
        => _assetManifest.Models.ToDictionary(x => x.Key, x => Path.Combine(_baseDirectory, "Models", x.Value));

    public static AssetLocator Create() => Create(SystemTier.SixteenBit);

    public static AssetLocator Create(SystemTier tier)
    {
        string baseDir = Path.GetDirectoryName(AppContext.BaseDirectory) ?? string.Empty;
        string assetsDir = Path.Combine(baseDir, "Assets");
        string path = Path.Combine(assetsDir, AssetManifestFilename);
        AssetManifest manifest = ReadManifest(path);

        // A tier only needs a manifest of its own where it differs from the
        // base one - a different filename, or a font sheet with different
        // geometry. Tiers whose assets merely live in a different folder need
        // no file at all.
        string overlayPath = Path.Combine(assetsDir, $"AssetManifest.{tier}.json");
        if (File.Exists(overlayPath))
        {
            Overlay(manifest, ReadManifest(overlayPath));
        }

        return new(manifest, baseDir, tier);
    }

    public static AssetLocator Create(Stream manifestStream, string baseDirectory)
        => Create(manifestStream, baseDirectory, SystemTier.SixteenBit);

    public static AssetLocator Create(Stream manifestStream, string baseDirectory, SystemTier tier)
        => Create(manifestStream, null, baseDirectory, tier);

    public static AssetLocator Create(
        Stream manifestStream,
        Stream? tierManifestStream,
        string baseDirectory,
        SystemTier tier)
    {
        ArgumentNullException.ThrowIfNull(manifestStream);

        AssetManifest manifest = Deserialize(manifestStream);

        if (tierManifestStream is not null)
        {
            Overlay(manifest, Deserialize(tierManifestStream));
        }

        return new(manifest, baseDirectory, tier);
    }

    private static AssetManifest Deserialize(Stream manifestStream)
    {
        try
        {
            return JsonSerializer.Deserialize<AssetManifest>(manifestStream)
                ?? throw new UsefulException("Failed to read asset manifest from provided stream.");
        }
        catch (JsonException ex)
        {
            throw new UsefulException("Failed to read asset manifest from provided stream.", ex);
        }
    }

    private static AssetManifest ReadManifest(string path)
    {
        try
        {
            using FileStream stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            return JsonSerializer.Deserialize<AssetManifest>(stream)
                ?? throw new UsefulException($"Asset manifest file is empty: {path}");
        }
        catch (Exception ex) when (ex is not UsefulException)
        {
            throw new UsefulException($"Failed to read asset manifest file: {path}", ex);
        }
    }

    // Entry-by-entry: a tier's manifest states only what it changes, so
    // anything it leaves out keeps the base manifest's value.
    private static void Overlay(AssetManifest manifest, AssetManifest tierManifest)
    {
        if (!string.IsNullOrEmpty(tierManifest.Palette))
        {
            manifest.Palette = tierManifest.Palette;
        }

        Overlay(manifest.Images, tierManifest.Images);
        Overlay(manifest.Sfx, tierManifest.Sfx);
        Overlay(manifest.Music, tierManifest.Music);
        Overlay(manifest.SoundFonts, tierManifest.SoundFonts);
        Overlay(manifest.Models, tierManifest.Models);
        Overlay(manifest.FontsBitmap, tierManifest.FontsBitmap);
        Overlay(manifest.FontsTrueType, tierManifest.FontsTrueType);
    }

    private static void Overlay<T>(Dictionary<string, T> entries, Dictionary<string, T> tierEntries)
    {
        foreach (KeyValuePair<string, T> entry in tierEntries)
        {
            entries[entry.Key] = entry.Value;
        }
    }

    private string TierPath(string category, string file)
    {
        string tiered = Path.Combine(_baseDirectory, category, Tier.ToString(), file);
        return File.Exists(tiered) ? tiered : Path.Combine(_baseDirectory, category, file);
    }
}
