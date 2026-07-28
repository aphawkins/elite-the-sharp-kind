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
    private readonly AssetTierOverride _overrides;
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
        _overrides = assetManifest.TierOverrides.TryGetValue(tier, out AssetTierOverride? tierOverride)
            ? tierOverride
            : new AssetTierOverride();
    }

    public SystemTier Tier { get; }

    public string PalettePath => TierPath(
        PaletteCategory,
        string.IsNullOrEmpty(_overrides.Palette) ? _assetManifest.Palette : _overrides.Palette);

    public IDictionary<string, BitmapFontAsset> FontBitmaps
        => _assetManifest.FontsBitmap.ToDictionary(x => x.Key, x => FontFor(x.Key, x.Value));

    public IDictionary<string, TrueTypeFontAsset> FontTrueTypes
        => _assetManifest.FontsTrueType.ToDictionary(
            x => x.Key,
            x => new TrueTypeFontAsset(Path.Combine(_baseDirectory, "FontsTrueType", x.Value.File), x.Value.PointSize));

    public IDictionary<string, string> ImagePaths
        => _assetManifest.Images.ToDictionary(
            x => x.Key,
            x => TierPath(ImagesCategory, Override(_overrides.Images, x.Key, x.Value)));

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
        string path = Path.Combine(baseDir, "Assets", AssetManifestFilename);

        try
        {
            using FileStream stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            return Create(stream, baseDir, tier);
        }
        catch (Exception ex) when (ex is not UsefulException)
        {
            throw new UsefulException($"Failed to read asset manifest file: {path}", ex);
        }
    }

    public static AssetLocator Create(Stream manifestStream, string baseDirectory)
        => Create(manifestStream, baseDirectory, SystemTier.SixteenBit);

    public static AssetLocator Create(Stream manifestStream, string baseDirectory, SystemTier tier)
    {
        ArgumentNullException.ThrowIfNull(manifestStream);

        AssetManifest manifest;

        try
        {
            manifest = JsonSerializer.Deserialize<AssetManifest>(manifestStream)
                ?? throw new UsefulException("Failed to read asset manifest from provided stream.");
        }
        catch (JsonException ex)
        {
            throw new UsefulException("Failed to read asset manifest from provided stream.", ex);
        }

        return new(manifest, baseDirectory, tier);
    }

    private static string Override(Dictionary<string, string> overrides, string logicalName, string manifestFile)
        => overrides.TryGetValue(logicalName, out string? file) ? file : manifestFile;

    // A tier's font sheet can replace the manifest entry outright, since its
    // geometry is part of the art rather than a property of the logical name.
    private BitmapFontAsset FontFor(string logicalName, BitmapFontEntry manifestEntry)
    {
        BitmapFontEntry entry = _overrides.FontsBitmap.TryGetValue(logicalName, out BitmapFontEntry? tierEntry)
            ? tierEntry
            : manifestEntry;

        return new(TierPath(FontsBitmapCategory, entry.File), entry);
    }

    private string TierPath(string category, string file)
    {
        string tiered = Path.Combine(_baseDirectory, category, Tier.ToString(), file);
        return File.Exists(tiered) ? tiered : Path.Combine(_baseDirectory, category, file);
    }
}
