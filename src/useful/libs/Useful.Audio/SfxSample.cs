// 'Useful Libraries' - Andy Hawkins 2025.

namespace Useful.Audio;

public sealed class SfxSample(int runtime, float volume = 1f, float pan = 0f)
{
    private readonly int _runtime = runtime;
    private int _timeleft;

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

    internal void ReduceTimeRemaining()
    {
        if (_timeleft > 0)
        {
            _timeleft--;
        }
    }

    internal void ResetTime() => _timeleft = _runtime;
}
