// 'Useful Libraries' - Andy Hawkins 2025.

namespace Useful.Audio;

public sealed class AudioController
{
    private readonly bool _effectsOn;
    private readonly bool _musicOn;
    private readonly IDictionary<string, SfxSample> _sfx;
    private readonly SfxSample[] _samples;
    private readonly ISound _sound;

    public AudioController(ISound sound, IDictionary<string, SfxSample> sfx)
    {
        Guard.ArgumentNull(sfx);

        _sound = sound;
        _sfx = sfx;

        // Two effect names may share one SfxSample to share its cooldown
        // (e.g. sounds that played on one buffer in the original hardware),
        // so the per-update tick runs over the distinct samples.
        _samples = [.. new HashSet<SfxSample>(sfx.Values)];
#if DEBUG
        _musicOn = true;
        _effectsOn = true;
#else
        _musicOn = true;
        _effectsOn = true;
#endif
    }

    public void PlayEffect(string effectType)
    {
        if (!_effectsOn)
        {
            return;
        }

        if (_sfx[effectType].HasTimeRemaining)
        {
            return;
        }

        _sfx[effectType].ResetTime();
        _sound.Play(effectType);
    }

    public void PlayMusic(string musicType, bool loop)
    {
        if (!_musicOn)
        {
            return;
        }

        _sound.Play(musicType, loop);
    }

    public void StopMusic()
    {
        if (!_musicOn)
        {
            return;
        }

        _sound.StopMusic();
    }

    public void UpdateSound()
    {
        foreach (SfxSample sample in _samples)
        {
            sample.ReduceTimeRemaining();
        }
    }
}
