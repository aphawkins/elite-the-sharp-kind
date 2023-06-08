// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Audio;
using EliteSharp.Enums;

namespace EliteSharp.Assets
{
    internal interface IAssets
    {
        Task<byte[]> LoadAsync(Image image, CancellationToken token);

        Task<byte[]> LoadAsync(SoundEffect effect, CancellationToken token);

        Task<byte[]> LoadAsync(Music music, CancellationToken token);
    }
}
