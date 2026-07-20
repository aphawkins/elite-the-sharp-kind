// 'Useful Libraries' - Andy Hawkins 2025.

namespace Useful.Audio;

public interface ISound
{
    public void Play(string musicType, bool repeat);

    /// <summary>
    /// Play a one-shot sound effect. Volume is 0 (silent) - 1 (full), pan is
    /// -1 (left) - 1 (right), and pitch is a multiplier (1.0 = recorded rate).
    /// </summary>
    public void Play(string sfxType, float volume, float pan, double pitch);

    public void StopMusic();

    /// <summary>
    /// Play or retune a continuously looping effect (e.g. an engine).
    /// A pitch of 1.0 plays at the recorded rate; only one loop plays at a time.
    /// </summary>
    public void PlayLoop(string sfxType, double pitch);

    public void StopLoop();
}
