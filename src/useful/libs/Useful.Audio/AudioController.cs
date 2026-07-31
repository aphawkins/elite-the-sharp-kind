// 'Useful Libraries' - Andy Hawkins 2023-2026.

using Microsoft.Extensions.Logging;

namespace Useful.Audio;

public sealed class AudioController
{
    private readonly IDictionary<string, SfxSample> _sfx;
    private readonly SfxSample[] _samples;
    private readonly ISound _sound;
    private readonly ILogger? _logger;

    public AudioController(ISound sound, IDictionary<string, SfxSample> sfx, AudioOptions options)
        : this(sound, sfx, options, logger: null)
    {
    }

    public AudioController(ISound sound, IDictionary<string, SfxSample> sfx, AudioOptions options, ILogger? logger)
    {
        ArgumentNullException.ThrowIfNull(sfx);
        ArgumentNullException.ThrowIfNull(options);

        _sound = sound;
        _sfx = sfx;
        _logger = logger;
        MusicOn = options.MusicOn;
        EffectsOn = options.EffectsOn;

        // Two effect names may share one SfxSample to share its cooldown
        // (e.g. sounds that played on one buffer in the original hardware),
        // so the per-update tick runs over the distinct samples.
        _samples = [.. new HashSet<SfxSample>(sfx.Values)];
    }

    // Settable so a settings screen can turn sound on and off mid-game; the
    // starting values come from the config file via AudioOptions.
    public bool EffectsOn { get; set; }

    public bool MusicOn { get; set; }

    public void PlayEffect(string effectType) => PlayEffect(effectType, volume: null, pitch: 1.0);

    /// <summary>
    /// Play an effect, using the sample's static volume/pan unless overridden.
    /// </summary>
    /// <param name="effectType">The effect's key in the sample dictionary passed to the constructor.</param>
    /// <param name="volume">Per-play volume override (0 silent - 1 full), or null to use the sample's static volume.</param>
    /// <param name="pitch">Per-play pitch multiplier (1.0 = recorded rate).</param>
    public void PlayEffect(string effectType, float? volume, double pitch)
    {
        if (!EffectsOn)
        {
            return;
        }

        // An effect with no registered sample is a content gap, not a
        // fatal error: log it and play nothing rather than throwing from
        // whatever gameplay code happened to ask for the sound.
        if (!_sfx.TryGetValue(effectType, out SfxSample? sample))
        {
            if (_logger is not null)
            {
                LogMessages.MissingSfxSample(_logger, effectType);
            }

            return;
        }

        if (sample.HasTimeRemaining)
        {
            return;
        }

        sample.ResetTime();
        _sound.Play(effectType, volume ?? sample.Volume, sample.Pan, pitch);
    }

    public void PlayMusic(string musicType, bool loop)
    {
        if (!MusicOn)
        {
            return;
        }

        _sound.Play(musicType, loop);
    }

    // Unconditional: this is also how the settings screen silences music that
    // is already playing at the moment music is switched off.
    public void StopMusic() => _sound.StopMusic();

    public void UpdateSound()
    {
        foreach (SfxSample sample in _samples)
        {
            sample.ReduceTimeRemaining();
        }
    }
}
