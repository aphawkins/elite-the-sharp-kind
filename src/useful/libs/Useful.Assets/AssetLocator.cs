// 'Useful Libraries' - Andy Hawkins 2023-2026.

using System.Text.Json;

namespace Useful.Assets;

// The one place asset paths are built, so rendition resolution lives here and
// nowhere else: rendition-varying categories resolve to <Category>/<Rendition>/<file>
// and fall back to <Category>/<file>, which is what keeps the rendition-neutral
// categories - audio, TrueType fonts, tracks - from needing a copy per rendition.
// Models are rendition-varying: their 'usemtl' names are resolved through the
// rendition's palette, and the two palettes name different colours. The rendition is
// fixed at construction, so IAssetLocator's consumers never have to know one
// exists.
public sealed class AssetLocator : IAssetLocator
{
    private const string AssetManifestFilename = "AssetManifest.json";
    private const string DefaultRendition = "SixteenBit";
    private const string ImagesCategory = "Images";
    private const string FontsBitmapCategory = "FontsBitmap";
    private const string ModelsCategory = "Models";
    private const string PaletteCategory = "Palette";
    private readonly AssetManifest _assetManifest = new();
    private readonly string _baseDirectory;

    internal AssetLocator(AssetManifest assetManifest, string baseDirectory, string rendition)
    {
        ArgumentNullException.ThrowIfNull(assetManifest);

        // The name becomes a directory segment, so it may not climb out of
        // the assets folder. A rendition the game was never built against
        // names itself, and a name is not a path.
        if (string.IsNullOrWhiteSpace(rendition) || rendition.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
        {
            throw new UsefulException($"'{rendition}' cannot be used as a folder name, so no assets could be found for it.");
        }

        _assetManifest = assetManifest;
        _baseDirectory = Path.Combine(baseDirectory, "Assets");
        Rendition = rendition;
    }

    public string Rendition { get; }

    public AssetColourLimits Colours => _assetManifest.Colours;

    public string PalettePath => RenditionPath(PaletteCategory, _assetManifest.Palette);

    public IDictionary<string, BitmapFontAsset> FontBitmaps
        => _assetManifest.FontsBitmap.ToDictionary(
            x => x.Key,
            x => new BitmapFontAsset(RenditionPath(FontsBitmapCategory, x.Value.File), x.Value));

    public IDictionary<string, TrueTypeFontAsset> FontTrueTypes
        => _assetManifest.FontsTrueType.ToDictionary(
            x => x.Key,
            x => new TrueTypeFontAsset(Path.Combine(_baseDirectory, "FontsTrueType", x.Value.File), x.Value.PointSize));

    public IDictionary<string, string> ImagePaths
        => _assetManifest.Images.ToDictionary(x => x.Key, x => RenditionPath(ImagesCategory, x.Value));

    public IDictionary<string, string> MusicPaths
        => _assetManifest.Music.ToDictionary(x => x.Key, x => Path.Combine(_baseDirectory, "Music", x.Value));

    public IDictionary<string, string> SfxPaths
        => _assetManifest.Sfx.ToDictionary(x => x.Key, x => Path.Combine(_baseDirectory, "SFX", x.Value));

    public IDictionary<string, string> SoundFontPaths
        => _assetManifest.SoundFonts.ToDictionary(x => x.Key, x => Path.Combine(_baseDirectory, "SoundFonts", x.Value));

    public IDictionary<string, string> ModelPaths
        => _assetManifest.Models.ToDictionary(x => x.Key, x => RenditionPath(ModelsCategory, x.Value));

    public static AssetLocator Create() => Create(DefaultRendition);

    public static AssetLocator Create(string rendition)
    {
        string baseDir = Path.GetDirectoryName(AppContext.BaseDirectory) ?? string.Empty;
        string assetsDir = Path.Combine(baseDir, "Assets");
        string path = Path.Combine(assetsDir, AssetManifestFilename);
        AssetManifest manifest = ReadManifest(path);

        // A rendition only needs a manifest of its own where it differs from the
        // base one - a different filename, or a font sheet with different
        // geometry. Tiers whose assets merely live in a different folder need
        // no file at all.
        string overlayPath = Path.Combine(assetsDir, $"AssetManifest.{rendition}.json");
        if (File.Exists(overlayPath))
        {
            Overlay(manifest, ReadManifest(overlayPath));
        }

        return new(manifest, baseDir, rendition);
    }

    public static AssetLocator Create(Stream manifestStream, string baseDirectory)
        => Create(manifestStream, baseDirectory, DefaultRendition);

    public static AssetLocator Create(Stream manifestStream, string baseDirectory, string rendition)
        => Create(manifestStream, null, baseDirectory, rendition);

    public static AssetLocator Create(
        Stream manifestStream,
        Stream? renditionManifestStream,
        string baseDirectory,
        string rendition)
    {
        ArgumentNullException.ThrowIfNull(manifestStream);

        AssetManifest manifest = Deserialize(manifestStream);

        if (renditionManifestStream is not null)
        {
            Overlay(manifest, Deserialize(renditionManifestStream));
        }

        return new(manifest, baseDirectory, rendition);
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

    // Entry-by-entry: a rendition's manifest states only what it changes, so
    // anything it leaves out keeps the base manifest's value.
    private static void Overlay(AssetManifest manifest, AssetManifest renditionManifest)
    {
        if (!string.IsNullOrEmpty(renditionManifest.Palette))
        {
            manifest.Palette = renditionManifest.Palette;
        }

        // Colour limits belong to the rendition rather than the game, so its
        // own manifest is the only place they can be stated - the base is
        // what is shared, and a shared limit would be a limit on everyone.
        manifest.Colours = renditionManifest.Colours;

        Overlay(manifest.Images, renditionManifest.Images);
        Overlay(manifest.Sfx, renditionManifest.Sfx);
        Overlay(manifest.Music, renditionManifest.Music);
        Overlay(manifest.SoundFonts, renditionManifest.SoundFonts);
        Overlay(manifest.Models, renditionManifest.Models);
        Overlay(manifest.FontsBitmap, renditionManifest.FontsBitmap);
        Overlay(manifest.FontsTrueType, renditionManifest.FontsTrueType);
    }

    private static void Overlay<T>(Dictionary<string, T> entries, Dictionary<string, T> renditionEntries)
    {
        foreach (KeyValuePair<string, T> entry in renditionEntries)
        {
            entries[entry.Key] = entry.Value;
        }
    }

    private string RenditionPath(string category, string file)
    {
        string renditioned = Path.Combine(_baseDirectory, category, Rendition, file);
        return File.Exists(renditioned) ? renditioned : Path.Combine(_baseDirectory, category, file);
    }
}
