// 'Useful Libraries' - Andy Hawkins 2025.

namespace Useful.Assets;

public interface IAssetLocator
{
    public string PalettePath { get; }

    public IDictionary<int, string> FontBitmapPaths { get; }

    public IDictionary<int, string> FontTrueTypePaths { get; }

    public IDictionary<int, string> ImagePaths { get; }

    public IDictionary<string, string> ModelPaths { get; }

    public IDictionary<int, string> MusicPaths { get; }

    public IDictionary<int, string> SfxPaths { get; }
}
