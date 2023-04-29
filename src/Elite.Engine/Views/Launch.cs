namespace Elite.Engine.Views
{
    using Elite.Common.Enums;
    using Elite.Engine.Enums;

    internal class Launch : IView
    {
        private readonly GameState _gameState;
        private readonly IGfx _gfx;
        private readonly Audio _audio;
        private readonly space _space;
        private readonly Combat _combat;
        private readonly BreakPattern _breakPattern;

        internal Launch(GameState gameState, IGfx gfx, Audio audio, space space, Combat combat)
        {
            _gameState = gameState;
            _gfx = gfx;
            _audio = audio;
            _space = space;
            _combat = combat;
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
            _combat.ClearUniverse();
            _breakPattern.Reset();
            _audio.PlayEffect(SoundEffect.Launch);
        }

        public void UpdateUniverse()
        {
            _breakPattern.Update();

            if (_breakPattern.IsComplete)
            {
                _space.launch_player();
                _gameState.SetView(SCR.SCR_FRONT_VIEW);
            }
        }
    }
}