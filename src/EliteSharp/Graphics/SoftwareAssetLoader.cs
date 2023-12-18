// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Assets;

namespace EliteSharp.Graphics
{
    public class SoftwareAssetLoader(IAssetLocator assets)
    {
        private readonly IAssetLocator _assets = assets;

        public Dictionary<ImageType, FastBitmap> LoadImages()
            => _assets.ImageAssetPaths().ToDictionary(x => x.Key, x => BitmapFile.Read(x.Value));
    }
}
