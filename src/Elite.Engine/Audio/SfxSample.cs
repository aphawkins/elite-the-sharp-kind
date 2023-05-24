// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

namespace Elite.Engine.Audio
{
    internal sealed class SfxSample
    {
        private readonly int _runtime;
        private int _timeleft;

        internal SfxSample(int runtime) => _runtime = runtime;

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
}
