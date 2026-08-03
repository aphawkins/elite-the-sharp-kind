// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using Useful.Assets;

namespace EliteSharpLib.Renditions;

/// <summary>
/// The game's assets and the rendition's, as one set. What a rendition looks
/// like is its own - the artwork, the palette, the fonts it draws with and
/// the ship models, whose material names are resolved through that palette -
/// and it ships them beside its assembly.
/// <para>
/// What is left is the game's, and stays with the executable: the music, the
/// sound effects and the soundfont. None of those is a rendition concern, and
/// copying them per rendition is what made the tier-first asset layout a bad
/// idea when it was first considered.
/// </para>
/// </summary>
internal sealed class RenditionAssets : IAssetLocator
{
    private readonly IAssetLocator _rendition;
    private readonly IAssetLocator _game;

    internal RenditionAssets(IAssetLocator rendition, IAssetLocator game)
    {
        _rendition = rendition;
        _game = game;
    }

    public string Rendition => _rendition.Rendition;

    public AssetColourLimits Colours => _rendition.Colours;

    public string PalettePath => _rendition.PalettePath;

    public IDictionary<string, BitmapFontAsset> FontBitmaps => _rendition.FontBitmaps;

    public IDictionary<string, string> ImagePaths => _rendition.ImagePaths;

    public IDictionary<string, string> ModelPaths => _rendition.ModelPaths;

    public IDictionary<string, TrueTypeFontAsset> FontTrueTypes => _game.FontTrueTypes;

    public IDictionary<string, string> MusicPaths => _game.MusicPaths;

    public IDictionary<string, string> SfxPaths => _game.SfxPaths;

    public IDictionary<string, string> SoundFontPaths => _game.SoundFontPaths;
}
