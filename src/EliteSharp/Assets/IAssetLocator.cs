// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Audio;
using EliteSharp.Graphics;

namespace EliteSharp.Assets
{
    public interface IAssetLocator
    {
        IDictionary<ImageType, string> ImageAssetPaths();

        IDictionary<SoundEffect, string> SfxAssetPaths();

        IDictionary<MusicType, string> MusicAssetPaths();

        IEnumerable<string> FontAssetPaths();
    }
}
