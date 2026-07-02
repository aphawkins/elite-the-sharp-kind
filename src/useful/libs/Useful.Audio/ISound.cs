// 'Useful Libraries' - Andy Hawkins 2025.

namespace Useful.Audio;

public interface ISound
{
    public void Play(string musicType, bool repeat);

    public void Play(string sfxType);

    public void StopMusic();
}
