// 'Useful Libraries' - Andy Hawkins 2023-2026.

namespace Useful.Assets;

public interface IAssetLocator
{
    public string PalettePath { get; }

    public IDictionary<string, string> FontBitmapPaths { get; }

    public IDictionary<string, TrueTypeFontAsset> FontTrueTypes { get; }

    public IDictionary<string, string> ImagePaths { get; }

    public IDictionary<string, string> ModelPaths { get; }

    public IDictionary<string, string> MusicPaths { get; }

    public IDictionary<string, string> SoundFontPaths { get; }

    public IDictionary<string, string> SfxPaths { get; }
}
