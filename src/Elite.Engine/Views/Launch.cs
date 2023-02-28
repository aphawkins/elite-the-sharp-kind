namespace Elite.Engine.Views
{
    using Elite.Common.Enums;
    using Elite.Engine.Enums;

    internal class Launch : IView
    {
        private readonly GameState _gameState;
        private readonly IGfx _gfx;
        private readonly Audio _audio;
        private readonly BreakPattern _breakPattern;

        internal Launch(GameState gameState, IGfx gfx, Audio audio)
        {
            _gameState = gameState;
            _gfx = gfx;
            _audio = audio;
            _breakPattern = new(_gfx);
        }

        public void Draw()
        {
            _breakPattern.Draw();
        }

        public void HandleInput()
        {
        }

        public void Reset()
        {
            swat.clear_universe();
            _breakPattern.Reset();
            _audio.PlayEffect(SoundEffect.Launch);
        }

        public void UpdateUniverse()
        {
            _breakPattern.Update();

            if (_breakPattern.IsComplete)
            {
                space.launch_player();
                _gameState.SetView(SCR.SCR_FRONT_VIEW);
            }
        }
    }
}