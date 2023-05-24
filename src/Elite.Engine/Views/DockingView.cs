// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using Elite.Engine.Conflict;
using Elite.Engine.Enums;

namespace Elite.Engine.Views
{
    internal sealed class DockingView : IView
    {
        private readonly Audio _audio;
        private readonly BreakPattern _breakPattern;
        private readonly Combat _combat;
        private readonly GameState _gameState;
        private readonly IGraphics _graphics;
        private readonly Space _space;

        internal DockingView(GameState gameState, IGraphics graphics, Audio audio, Space space, Combat combat)
        {
            _gameState = gameState;
            _graphics = graphics;
            _audio = audio;
            _space = space;
            _combat = combat;
            _breakPattern = new(_graphics);
        }

        public void Draw() => _breakPattern.Draw();

        public void HandleInput()
        {
        }

        public void Reset()
        {
            _combat.ClearUniverse();
            _breakPattern.Reset();
            _audio.PlayEffect(SoundEffect.Dock);
        }

        public void UpdateUniverse()
        {
            _breakPattern.Update();

            if (_breakPattern.IsComplete)
            {
                _space.DockPlayer();
                _gameState.SetView(Screen.MissionOne);
            }
        }
    }
}
