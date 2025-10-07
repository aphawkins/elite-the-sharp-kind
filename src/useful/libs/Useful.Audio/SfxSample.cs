// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

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
