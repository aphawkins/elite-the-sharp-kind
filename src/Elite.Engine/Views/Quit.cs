﻿using Elite.Engine.Enums;

namespace Elite.Engine.Views
{
    internal class Quit : IView
    {
        private readonly GameState _gameState;
        private readonly IGfx _gfx;
        private readonly Draw _draw;
        private readonly IKeyboard _keyboard;

        internal Quit(GameState gameState, IGfx gfx, Draw draw, IKeyboard keyboard) 
        {
            _gameState = gameState;
            _gfx = gfx;
            _draw = draw;
            _keyboard = keyboard;
        }

        public void Draw()
        {
            _draw.ClearDisplay();
            _draw.DrawViewHeader("GAME OPTIONS");

            _gfx.DrawTextCentre(175, "QUIT GAME (Y/N)?", 140, GFX_COL.GFX_COL_GOLD);
        }

        public void HandleInput()
        {
            if (_keyboard.IsKeyPressed(CommandKey.Yes))
            {
                EliteMain.ExitGame();
            }

            if (_keyboard.IsKeyPressed(CommandKey.No))
            {
                if (EliteMain.docked)
                {
                    _gameState.SetView(SCR.SCR_CMDR_STATUS);
                }
                else
                {
                    _gameState.SetView(SCR.SCR_FRONT_VIEW);
                }
            }
        }

        public void Reset()
        {
        }

        public void UpdateUniverse()
        {
        }
    }
}
