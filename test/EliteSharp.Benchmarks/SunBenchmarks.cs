// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using BenchmarkDotNet.Attributes;
using EliteSharp.Controls;
using EliteSharp.Graphics;
using EliteSharp.Suns;

namespace EliteSharp.Benchmarks
{
    public class SunBenchmarks : IDisposable
    {
        private readonly EBitmap _buffer = new(512, 512);
        private readonly SoftwareGraphics _graphics;
        private readonly SolidSun _solidSun;
        private readonly GradientSun _gradientSun;
        private readonly Action _doNothing = () => { };
        private bool _disposedValue;

        public SunBenchmarks()
        {
            SoftwareKeyboard keyboard = new();
            Dictionary<Views.Screen, Views.IView> views = new();
            GameState gameState = new(keyboard, views);
            _graphics = new SoftwareGraphics(_buffer, _doNothing);
            Draw draw = new(gameState, _graphics);
            _gradientSun = new(draw);
            _solidSun = new(draw, EColors.White);
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        [Benchmark]
        public void SolidSun() => _solidSun.Draw();

        [Benchmark]
        public void GradientSun() => _gradientSun.Draw();

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
