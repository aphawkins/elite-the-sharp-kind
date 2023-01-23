namespace Elite
{
    using System.Media;

    internal class SfxSample : IDisposable
    {
        private readonly int _runtime;
        private readonly SoundPlayer _sample;
        private bool _disposedValue;
        private int _timeleft = 0;

        internal SfxSample(string filename, int runtime)
        {
            _sample = new(Path.Combine("sfx", filename));
            _runtime = runtime;
        }

        internal bool HasTimeRemaining => _timeleft > 0;

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        internal void Play()
        {
            _sample.Play();
        }

        internal void ReduceTimeRemaining()
        {
            _timeleft--;
        }

        internal void ResetTime()
        {
            _timeleft = _runtime;
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    // dispose managed state (managed objects)
                    _sample.Dispose();
                }

                // free unmanaged resources (unmanaged objects) and override finalizer
                // set large fields to null
                _disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~SfxSample()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }
    }
}