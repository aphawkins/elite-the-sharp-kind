// 'Useful Libraries' - Andy Hawkins 2025.

using System.Text.Json;

namespace Useful.Assets;

public class AssetLocator : IAssetLocator
{
    private const string AssetManifestFilename = "AssetManifest.json";
    private AssetManifest _assetNames = new();

    public IDictionary<int, string> FontBitmapPaths
        => _assetNames.FontsBitmap.Select((value, index) => new { value, index })
            .ToDictionary(x => x.index, x => Path.Combine(GetAssetPath(), "FontsBitmap", _assetNames.FontsBitmap[x.value.Key]));

    public IDictionary<int, string> FontTrueTypePaths
    => _assetNames.FontsTrueType.Select((value, index) => new { value, index })
        .ToDictionary(x => x.index, x => Path.Combine(GetAssetPath(), "FontsTrueType", _assetNames.FontsTrueType[x.value.Key]));

    public IDictionary<int, string> ImagePaths
        => _assetNames.Images.Select((value, index) => new { value, index })
            .ToDictionary(x => x.index, x => Path.Combine(GetAssetPath(), "Images", _assetNames.Images[x.value.Key]));

    public bool IsInitialized { get; private set; }

    public IDictionary<int, string> MusicPaths
        => _assetNames.Music.Select((value, index) => new { value, index })
            .ToDictionary(x => x.index, x => Path.Combine(GetAssetPath(), "Music", _assetNames.Music[x.value.Key]));

    public IDictionary<int, string> SfxPaths
        => _assetNames.Sfx.Select((value, index) => new { value, index })
            .ToDictionary(x => x.index, x => Path.Combine(GetAssetPath(), "SFX", _assetNames.Sfx[x.value.Key]));

    public void Initialize()
    {
        string path = Path.Combine(Path.GetDirectoryName(AppContext.BaseDirectory) ?? string.Empty, "Assets", AssetManifestFilename);
        FileStream stream;

        try
        {
            stream = File.Open(path, FileMode.Open);
        }
        catch (Exception ex)
        {
            throw new UsefulException($"Failed to read asset manifest file: {path}", ex);
        }

        _assetNames = JsonSerializer.Deserialize<AssetManifest>(stream)
            ?? throw new UsefulException($"Failed to read asset manifest file: {path}");

        stream.Dispose();

        IsInitialized = true;
    }

    protected virtual string GetAssetPath()
        => Path.Combine(Path.GetDirectoryName(AppContext.BaseDirectory) ?? string.Empty, "Assets");
}
