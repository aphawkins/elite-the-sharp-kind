namespace Elite.Engine.Views
{
    using Elite.Common.Enums;
    using Elite.Engine.Enums;

    internal class Hyperspace : IView
    {
        private readonly IGfx _gfx;
        private readonly Audio _audio;
        private readonly space _space;
        private BreakPattern _breakPattern;

        internal Hyperspace(IGfx gfx, Audio audio, space space)
        {
            _gfx = gfx;
            _audio = audio;
            _space = space;
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
            _breakPattern.Reset();
            _audio.PlayEffect(SoundEffect.Hyperspace);
        }

        public void UpdateUniverse()
        {
            _breakPattern.Update();

            if (_breakPattern.IsComplete)
            {
                elite.SetView(SCR.SCR_FRONT_VIEW);
            }
        }
    }
}