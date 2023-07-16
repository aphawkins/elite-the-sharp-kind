// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Controls;
using EliteSharp.Graphics;
using EliteSharp.Planets;
using EliteSharp.Views;

namespace EliteSharp.Tests.Planets
{
    public class WireframePlanetTests : IDisposable
    {
        private readonly int[,] _buffer;
        private readonly IDraw _draw;
        private readonly GameState _gameState;
        private readonly IGraphics _graphics;
        private readonly IKeyboard _keyboard;
        private readonly IDictionary<Screen, IView> _views;
        private bool _disposedValue;

        public WireframePlanetTests()
        {
            _buffer = new int[512, 512];
            _keyboard = new SoftwareKeyboard();
            _views = new Dictionary<Screen, IView>();
            _gameState = new GameState(_keyboard, _views);
            _graphics = new SoftwareGraphics(_buffer);
            _draw = new Draw(_gameState, _graphics);
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        [Fact]
        public void DrawWireframePlanet()
        {
            // Arrange
            WireframePlanet planet = new(_draw);

            // Act
            planet.Draw();

            // Assert
        }

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
