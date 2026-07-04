// 'Useful Libraries' - Andy Hawkins 2025.

namespace Useful.Audio;

public interface ISound
{
    public void Play(string musicType, bool repeat);

    public void Play(string sfxType);

    public void StopMusic();

    /// <summary>
    /// Play or retune a continuously looping effect (e.g. an engine).
    /// A pitch of 1.0 plays at the recorded rate; only one loop plays at a time.
    /// </summary>
    public void PlayLoop(string sfxType, double pitch);

    public void StopLoop();
}
