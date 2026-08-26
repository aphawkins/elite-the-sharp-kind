// 'SharpKind Libraries' - Andy Hawkins 2023-2026.

namespace SharpKind.Audio;

public sealed class SfxSample(int runtime, float volume = 1f, float pan = 0f)
{
    private readonly int _runtime = runtime;
    private float _timeleft;

    /// <summary>
    /// Gets the sample's static playback volume (0 silent - 1 full), used
    /// when a play does not override it.
    /// </summary>
    internal float Volume { get; } = volume;

    /// <summary>
    /// Gets the sample's fixed stereo pan (-1 left - 1 right), matching the
    /// original's per-sample channel side.
    /// </summary>
    internal float Pan { get; } = pan;

    internal bool HasTimeRemaining => _timeleft > 0;

    /// <summary>
    /// Ages the sample by <paramref name="ticks"/> of its caller's clock.
    /// </summary>
    /// <remarks>
    /// A runtime is a whole number of the caller's ticks, and a caller that
    /// updates once per tick passes 1 and gets exactly what it always got.
    /// One updating faster passes a fraction, so the sample lasts the same
    /// length of time rather than the same number of updates.
    /// </remarks>
    internal void ReduceTimeRemaining(float ticks)
    {
        if (_timeleft > 0)
        {
            _timeleft = MathF.Max(_timeleft - ticks, 0);
        }
    }

    internal void ResetTime() => _timeleft = _runtime;
}
