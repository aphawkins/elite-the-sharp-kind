// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Audio;
using EliteSharp.Enums;

namespace EliteSharp.Assets
{
    public interface IAssets
    {
        public Stream? Load(Image image);

        public Stream? Load(SoundEffect effect);

        public Stream? Load(Music music);
    }
}
