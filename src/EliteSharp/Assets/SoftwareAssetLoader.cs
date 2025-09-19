// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Assets.Fonts;
using EliteSharp.Audio;
using EliteSharp.Graphics;

namespace EliteSharp.Assets;

public class SoftwareAssetLoader(IAssetLocator assets) : ISoftwareAssetLoader
{
    private readonly IAssetLocator _assets = assets;

    public Dictionary<ImageType, FastBitmap> LoadImages()
        => _assets.ImageAssets().ToDictionary(
            x => x.Key,
            x => BitmapFile.Read(x.Value));

    public Dictionary<MusicType, SoundSampleProvider> LoadMusic()
        => _assets.MusicAssets().ToDictionary(
            x => x.Key,
            x => SoundSampleProvider.Create(x.Value));

    public Dictionary<SoundEffect, SoundSampleProvider> LoadSfx()
        => _assets.SfxAssets().ToDictionary(
            x => x.Key,
            x => SoundSampleProvider.Create(x.Value));

    public Dictionary<FontType, BitmapFont> LoadFonts()
        => _assets.FontAssets().ToDictionary(
            x => x.Key,
            x => new BitmapFont(BitmapFile.Read(x.Value)));
}
