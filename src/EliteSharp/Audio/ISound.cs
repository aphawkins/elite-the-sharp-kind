// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

namespace EliteSharp.Audio;

public interface ISound : IDisposable
{
    public void Load();

    public void Play(MusicType musicType, bool repeat);

    public void Play(SoundEffect sfxType);

    public void StopMusic();
}
