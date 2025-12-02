// 'Useful Libraries' - Andy Hawkins 2025.

namespace Useful.Audio;

public interface ISound
{
    public void Play(int musicType, bool repeat);

    public void Play(int sfxType);

    public void StopMusic();
}
