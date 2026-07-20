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

    public void PlayEffect(string effectType) => PlayEffect(effectType, volume: null, pitch: 1.0);

    /// <summary>
    /// Play an effect, using the sample's static volume/pan unless overridden.
    /// </summary>
    /// <param name="effectType">The effect's key in the sample dictionary passed to the constructor.</param>
    /// <param name="volume">Per-play volume override (0 silent - 1 full), or null to use the sample's static volume.</param>
    /// <param name="pitch">Per-play pitch multiplier (1.0 = recorded rate).</param>
    public void PlayEffect(string effectType, float? volume, double pitch)
    {
        if (!_effectsOn)
        {
            return;
        }

        SfxSample sample = _sfx[effectType];
        if (sample.HasTimeRemaining)
        {
            return;
        }

        sample.ResetTime();
        _sound.Play(effectType, volume ?? sample.Volume, sample.Pan, pitch);
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
