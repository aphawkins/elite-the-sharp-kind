﻿// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using Elite.Common.Enums;
using Elite.Engine.Conflict;
using Elite.Engine.Enums;

namespace Elite.Engine.Views
{
    internal sealed class LaunchView : IView
    {
        private readonly Audio _audio;
        private readonly BreakPattern _breakPattern;
        private readonly Combat _combat;
        private readonly GameState _gameState;
        private readonly IGfx _gfx;
        private readonly Space _space;

        internal LaunchView(GameState gameState, IGfx gfx, Audio audio, Space space, Combat combat)
        {
            _gameState = gameState;
            _gfx = gfx;
            _audio = audio;
            _space = space;
            _combat = combat;
            _breakPattern = new(_gfx);
        }

        public void Draw() => _breakPattern.Draw();

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
                _space.LaunchPlayer();
                _gameState.SetView(SCR.SCR_FRONT_VIEW);
            }
        }
    }
}
