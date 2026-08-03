// 'Useful Libraries' - Andy Hawkins 2023-2026.

namespace Useful.Assets;

public interface IAssetLocator
{
    // Which rendition the paths below resolve to. Consumers that only load
    // assets can ignore it.
    public string Rendition { get; }

    // What that rendition declared its colours are limited to, which the
    // load-time checks are run against.
    public AssetColourLimits Colours { get; }

    public string PalettePath { get; }

    public IDictionary<string, BitmapFontAsset> FontBitmaps { get; }

    public IDictionary<string, TrueTypeFontAsset> FontTrueTypes { get; }

    public IDictionary<string, string> ImagePaths { get; }

    public IDictionary<string, string> ModelPaths { get; }

    public IDictionary<string, string> MusicPaths { get; }

    public IDictionary<string, string> SoundFontPaths { get; }

    public IDictionary<string, string> SfxPaths { get; }
}
