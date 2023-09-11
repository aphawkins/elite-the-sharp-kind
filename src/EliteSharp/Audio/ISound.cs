// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

namespace EliteSharp.Audio
{
    public interface ISound : IDisposable
    {
        Task LoadAsync(MusicType musicType, string filePath, CancellationToken token);

        Task LoadAsync(SoundEffect sfxType, string filePath, CancellationToken token);

        void Play(MusicType musicType, bool repeat);

        void Play(SoundEffect sfxType);

        void StopMusic();
    }
}
