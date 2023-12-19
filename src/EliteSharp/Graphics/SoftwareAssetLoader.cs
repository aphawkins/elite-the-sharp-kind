// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Assets;
using EliteSharp.Audio;

namespace EliteSharp.Graphics
{
    public class SoftwareAssetLoader(IAssetLocator assets)
    {
        private readonly IAssetLocator _assets = assets;

        public Dictionary<ImageType, FastBitmap> LoadImages()
            => _assets.ImageAssetPaths().ToDictionary(x => x.Key, x => BitmapFile.Read(x.Value));

        public Dictionary<MusicType, EWave> LoadMusic()
            => _assets.MusicAssetPaths().ToDictionary(
                x => x.Key,
                x =>
                {
                    using MemoryStream memStream = new();
                    using FileStream stream = new(x.Value, FileMode.Open);
                    stream.CopyToAsync(memStream).ConfigureAwait(false);
                    memStream.Position = 0;
                    return new EWave(memStream.ToArray());
                });

        public Dictionary<SoundEffect, EWave> LoadSfx()
            => _assets.SfxAssetPaths().ToDictionary(
                x => x.Key,
                x =>
                {
                    using MemoryStream memStream = new();
                    using FileStream stream = new(x.Value, FileMode.Open);
                    stream.CopyToAsync(memStream).ConfigureAwait(false);
                    memStream.Position = 0;
                    return new EWave(memStream.ToArray());
                });
    }
}
