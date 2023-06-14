// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Audio;
using EliteSharp.Graphics;

namespace EliteSharp.Views
{
    internal sealed class HyperspaceView : IView
    {
        private readonly AudioController _audio;
        private readonly BreakPattern _breakPattern;
        private readonly GameState _gameState;
        private readonly IGraphics _graphics;

        internal HyperspaceView(GameState gameState, IGraphics graphics, AudioController audio)
        {
            _gameState = gameState;
            _graphics = graphics;
            _audio = audio;
            _breakPattern = new(_graphics);
        }

        public void Draw() => _breakPattern.Draw();

        public void HandleInput()
        {
        }

        public void Reset()
        {
            _breakPattern.Reset();
            _audio.PlayEffect(SoundEffect.Hyperspace);
        }

        public void UpdateUniverse()
        {
            _breakPattern.Update();

            if (_breakPattern.IsComplete)
            {
                _gameState.SetView(Screen.FrontView);
            }
        }
    }
}
