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

    public IDictionary<int, string> FontBitmapPaths => _assetManifest.FontsBitmap.Select((kvp, index) => new { kvp, index })
        .ToDictionary(x => x.index, x => Path.Combine(_baseDirectory, "FontsBitmap", x.kvp.Value));

    public IDictionary<int, string> FontTrueTypePaths => _assetManifest.FontsTrueType.Select((kvp, index) => new { kvp, index })
        .ToDictionary(x => x.index, x => Path.Combine(_baseDirectory, "FontsTrueType", x.kvp.Value));

    public IDictionary<int, string> ImagePaths => _assetManifest.Images.Select((kvp, index) => new { kvp, index })
        .ToDictionary(x => x.index, x => Path.Combine(_baseDirectory, "Images", x.kvp.Value));

    public IDictionary<int, string> MusicPaths => _assetManifest.Music.Select((kvp, index) => new { kvp, index })
        .ToDictionary(x => x.index, x => Path.Combine(_baseDirectory, "Music", x.kvp.Value));

    public IDictionary<int, string> SfxPaths => _assetManifest.Sfx.Select((kvp, index) => new { kvp, index })
        .ToDictionary(x => x.index, x => Path.Combine(_baseDirectory, "SFX", x.kvp.Value));

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
