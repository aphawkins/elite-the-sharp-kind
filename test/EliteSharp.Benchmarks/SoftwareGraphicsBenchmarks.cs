// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using BenchmarkDotNet.Attributes;
using EliteSharp.Graphics;

namespace EliteSharp.Benchmarks
{
    public class SoftwareGraphicsBenchmarks : IDisposable
    {
        private const int ScreenWidth = 512;
        private const int ScreenHeight = 512;
        private readonly SoftwareGraphics _graphics;
        private bool _disposedValue;

        public SoftwareGraphicsBenchmarks() => _graphics = new(ScreenWidth, ScreenHeight, (_) => { });

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        [Benchmark]
        public void Clear() => _graphics.Clear();

        [Benchmark]
        public void DrawCircle() => _graphics.DrawCircle(new(255, 255), 100, EliteColors.White);

        [Benchmark]
        public void DrawCircleFilled() => _graphics.DrawCircleFilled(new(255, 255), 100, EliteColors.White);

        [Benchmark]
        public void DrawLine() => _graphics.DrawLine(new(0, 0), new(511, 511), EliteColors.White);

        [Benchmark]
        public void DrawPixel() => _graphics.DrawPixel(new(255, 255), EliteColors.White);

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    // dispose managed state (managed objects)
                    _graphics.Dispose();
                }

                // free unmanaged resources (unmanaged objects) and override finalizer
                // set large fields to null
                _disposedValue = true;
            }
        }
    }
}
