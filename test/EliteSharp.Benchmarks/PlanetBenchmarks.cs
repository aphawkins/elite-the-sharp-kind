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
        private readonly IDraw _draw;
        private readonly GameState _gameState;
        private readonly int[,] _buffer = new int[512, 512];
        private readonly IGraphics _graphics;
        private readonly SolidPlanet _solidPlanet;
        private readonly IKeyboard _keyboard;
        private readonly IDictionary<Views.Screen, Views.IView> _views;
        private readonly WireframePlanet _wireframePlanet;
        private readonly FractalPlanet _fractalPlanet;
        private readonly StripedPlanet _stripedPlanet;
        private bool _disposedValue;

        public PlanetBenchmarks()
        {
            _keyboard = new SoftwareKeyboard();
            _views = new Dictionary<Views.Screen, Views.IView>();
            _gameState = new GameState(_keyboard, _views);
            _graphics = new SoftwareGraphics(_buffer);
            _draw = new Draw(_gameState, _graphics);
            _wireframePlanet = new(_draw);
            _solidPlanet = new(_draw, Colour.White);
            _fractalPlanet = new(_draw, 12345);
            _stripedPlanet = new(_draw);
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~PlanetBenchmarks()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }
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
