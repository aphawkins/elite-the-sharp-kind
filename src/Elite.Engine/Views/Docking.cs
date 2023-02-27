namespace Elite.Engine.Views
{
    using Elite.Common.Enums;
    using Elite.Engine.Enums;

    internal class Docking : IView
    {
        private readonly IGfx _gfx;
        private readonly Audio _audio;
        private readonly space _space;
        private readonly BreakPattern _breakPattern;

        internal Docking(IGfx gfx, Audio audio, space space)
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
            swat.clear_universe();
            _breakPattern.Reset();
            _audio.PlayEffect(SoundEffect.Dock);
        }

        public void UpdateUniverse()
        {
            _breakPattern.Update();

            if (_breakPattern.IsComplete)
            {
                _space.dock_player();
                elite.SetView(SCR.SCR_MISSION_1);
            }
        }
    }
}