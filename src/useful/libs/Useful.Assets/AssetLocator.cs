// 'Useful Libraries' - Andy Hawkins 2025.

using System.Text.Json;

namespace Useful.Assets;

public sealed class AssetLocator : IAssetLocator
{
    private const string AssetManifestFilename = "AssetManifest.json";
    private readonly AssetManifest _assetManifest = new();
    private readonly string _baseDirectory;

    internal AssetLocator(AssetManifest assetManifest, string baseDirectory)
    {
        Guard.ArgumentNull(assetManifest);

        _assetManifest = assetManifest;
        _baseDirectory = Path.Combine(baseDirectory, "Assets");
    }

    public string PalettePath => Path.Combine(_baseDirectory, "Palette", _assetManifest.Palette);

    public IDictionary<string, string> FontBitmapPaths
        => _assetManifest.FontsBitmap.ToDictionary(x => x.Key, x => Path.Combine(_baseDirectory, "FontsBitmap", x.Value));

    public IDictionary<string, string> FontTrueTypePaths
        => _assetManifest.FontsTrueType.ToDictionary(x => x.Key, x => Path.Combine(_baseDirectory, "FontsTrueType", x.Value));

    public IDictionary<string, string> ImagePaths
        => _assetManifest.Images.ToDictionary(x => x.Key, x => Path.Combine(_baseDirectory, "Images", x.Value));

    public IDictionary<string, string> MusicPaths
        => _assetManifest.Music.ToDictionary(x => x.Key, x => Path.Combine(_baseDirectory, "Music", x.Value));

    public IDictionary<string, string> SfxPaths
        => _assetManifest.Sfx.ToDictionary(x => x.Key, x => Path.Combine(_baseDirectory, "SFX", x.Value));

    public IDictionary<string, string> SoundFontPaths
        => _assetManifest.SoundFonts.ToDictionary(x => x.Key, x => Path.Combine(_baseDirectory, "SoundFonts", x.Value));

    public IDictionary<string, string> ModelPaths
        => _assetManifest.Models.ToDictionary(x => x.Key, x => Path.Combine(_baseDirectory, "Models", x.Value));

    public static AssetLocator Create()
    {
        string baseDir = Path.GetDirectoryName(AppContext.BaseDirectory) ?? string.Empty;
        string path = Path.Combine(baseDir, "Assets", AssetManifestFilename);

        try
        {
            using FileStream stream = File.Open(path, FileMode.Open);
            return Create(stream, baseDir);
        }
        catch (Exception ex)
        {
            throw new UsefulException($"Failed to read asset manifest file: {path}", ex);
        }
    }

    public static AssetLocator Create(Stream manifestStream, string baseDirectory)
    {
        Guard.ArgumentNull(manifestStream);

        try
        {
            AssetManifest manifest = JsonSerializer.Deserialize<AssetManifest>(manifestStream)
                ?? throw new UsefulException("Failed to read asset manifest from provided stream.");
            return new AssetLocator(manifest, baseDirectory);
        }
        catch (JsonException ex)
        {
            throw new UsefulException("Failed to read asset manifest from provided stream.", ex);
        }
    }
}
