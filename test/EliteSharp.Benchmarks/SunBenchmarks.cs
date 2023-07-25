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
        private readonly IDraw _draw;
        private readonly GameState _gameState;
        private readonly EBitmap _buffer = new(512, 512);
        private readonly IGraphics _graphics;
        private readonly SolidSun _solidSun;
        private readonly IKeyboard _keyboard;
        private readonly IDictionary<Views.Screen, Views.IView> _views;
        private readonly GradientSun _gradientSun;
        private bool _disposedValue;

        public SunBenchmarks()
        {
            _keyboard = new SoftwareKeyboard();
            _views = new Dictionary<Views.Screen, Views.IView>();
            _gameState = new GameState(_keyboard, _views);
            _graphics = new SoftwareGraphics(_buffer);
            _draw = new Draw(_gameState, _graphics);
            _gradientSun = new(_draw);
            _solidSun = new(_draw, EColor.White);
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
        public void SolidSun() => _solidSun.Draw();

        [Benchmark]
        public void GradientPlanet() => _gradientSun.Draw();

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
