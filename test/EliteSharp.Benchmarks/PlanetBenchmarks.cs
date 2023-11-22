// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using BenchmarkDotNet.Attributes;
using EliteSharp.Controls;
using EliteSharp.Graphics;
using EliteSharp.Planets;

namespace EliteSharp.Benchmarks
{
    public class PlanetBenchmarks : IDisposable
    {
        private readonly EBitmap _buffer = new(512, 512);
        private readonly SoftwareGraphics _graphics;
        private readonly SolidPlanet _solidPlanet;
        private readonly WireframePlanet _wireframePlanet;
        private readonly FractalPlanet _fractalPlanet;
        private readonly StripedPlanet _stripedPlanet;
        private bool _disposedValue;

        public PlanetBenchmarks()
        {
            SoftwareKeyboard keyboard = new();
            Dictionary<Views.Screen, Views.IView> views = [];
            GameState gameState = new(keyboard, views);
            _graphics = new SoftwareGraphics(_buffer, ScreenUpdate);
            Draw draw = new(gameState, _graphics);
            _wireframePlanet = new(draw);
            _solidPlanet = new(draw, EColors.White);
            _fractalPlanet = new(draw, 12345);
            _stripedPlanet = new(draw);
        }

        public static void ScreenUpdate()
        {
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        [Benchmark]
        public void SolidPlanet() => _solidPlanet.Draw();

        [Benchmark]
        public void WireframePlanet() => _wireframePlanet.Draw();

        [Benchmark]
        public void FractalPlanet() => _fractalPlanet.Draw();

        [Benchmark]
        public void StripedPlanet() => _stripedPlanet.Draw();

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
