// 'Useful Libraries' - Andy Hawkins 2025.

namespace Useful.Audio;

public sealed class SfxSample
{
    private readonly int _runtime;
    private int _timeleft;

    public SfxSample(int runtime) => _runtime = runtime;

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
