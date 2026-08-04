// 'Useful Libraries' - Andy Hawkins 2023-2026.

using System.Text.Json;

namespace Useful.Assets;

// The one place asset paths are built: <Category>/<file>, under whatever
// directory this locator was pointed at. There is no rendition segment in a
// path any more - a rendition keeps its assets in its own folder, so the
// folder is the answer and the name is only a label for messages. A game
// wanting both its own assets and a rendition's composes two of these.
public sealed class AssetLocator : IAssetLocator
{
    private const string AssetManifestFilename = "AssetManifest.json";
    private const string DefaultRendition = "16-bit";
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

    public string PalettePath => CategoryPath(PaletteCategory, _assetManifest.Palette);

    public IDictionary<string, BitmapFontAsset> FontBitmaps
        => _assetManifest.FontsBitmap.ToDictionary(
            x => x.Key,
            x => new BitmapFontAsset(CategoryPath(FontsBitmapCategory, x.Value.File), x.Value));

    public IDictionary<string, TrueTypeFontAsset> FontTrueTypes
        => _assetManifest.FontsTrueType.ToDictionary(
            x => x.Key,
            x => new TrueTypeFontAsset(Path.Combine(_baseDirectory, "FontsTrueType", x.Value.File), x.Value.PointSize));

    public IDictionary<string, string> ImagePaths
        => _assetManifest.Images.ToDictionary(x => x.Key, x => CategoryPath(ImagesCategory, x.Value));

    public IDictionary<string, string> MusicPaths
        => _assetManifest.Music.ToDictionary(x => x.Key, x => Path.Combine(_baseDirectory, "Music", x.Value));

    public IDictionary<string, string> SfxPaths
        => _assetManifest.Sfx.ToDictionary(x => x.Key, x => Path.Combine(_baseDirectory, "SFX", x.Value));

    public IDictionary<string, string> SoundFontPaths
        => _assetManifest.SoundFonts.ToDictionary(x => x.Key, x => Path.Combine(_baseDirectory, "SoundFonts", x.Value));

    public IDictionary<string, string> ModelPaths
        => _assetManifest.Models.ToDictionary(x => x.Key, x => CategoryPath(ModelsCategory, x.Value));

    public static AssetLocator Create() => Create(DefaultRendition);

    public static AssetLocator Create(string rendition)
        => CreateFrom(Path.GetDirectoryName(AppContext.BaseDirectory) ?? string.Empty, rendition);

    public static AssetLocator Create(Stream manifestStream, string baseDirectory)
        => Create(manifestStream, baseDirectory, DefaultRendition);

    public static AssetLocator Create(Stream manifestStream, string baseDirectory, string rendition)
    {
        ArgumentNullException.ThrowIfNull(manifestStream);

        return new(Deserialize(manifestStream), baseDirectory, rendition);
    }

    /// <summary>
    /// Reads the manifest in an Assets folder under <paramref name="baseDirectory"/>
    /// and resolves everything it names against it. A rendition's assets live
    /// beside its assembly rather than beside the executable, so the game
    /// builds one of these per place it keeps assets.
    /// </summary>
    /// <param name="baseDirectory">The directory the Assets folder sits in.</param>
    /// <param name="rendition">The name to label this set with, for messages.</param>
    /// <returns>A locator over that folder.</returns>
    public static AssetLocator CreateFrom(string baseDirectory, string rendition)
        => new(ReadManifest(Path.Combine(baseDirectory, "Assets", AssetManifestFilename)), baseDirectory, rendition);

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

    private string CategoryPath(string category, string file) => Path.Combine(_baseDirectory, category, file);
}
