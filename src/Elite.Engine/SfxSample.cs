namespace Elite.Engine
{
    internal class SfxSample
    {
        private readonly int _runtime;
        private int _timeleft = 0;

        internal SfxSample(int runtime)
        {
            _runtime = runtime;
        }

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