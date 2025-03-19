// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Assets.Fonts;
using EliteSharp.Audio;
using EliteSharp.Graphics;

namespace EliteSharp.Assets;

public interface ISoftwareAssetLoader
{
    public Dictionary<FontType, BitmapFont> LoadFonts();

    public Dictionary<ImageType, FastBitmap> LoadImages();

    public Dictionary<MusicType, SoundSampleProvider> LoadMusic();

    public Dictionary<SoundEffect, SoundSampleProvider> LoadSfx();
}
