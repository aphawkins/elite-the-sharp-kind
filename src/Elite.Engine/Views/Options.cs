/*
 * Elite - The New Kind.
 *
 * Reverse engineered from the BBC disk version of Elite.
 * Additional material by C.J.Pinder.
 *
 * The original Elite code is (C) I.Bell & D.Braben 1984.
 * This version re-engineered in C by C.J.Pinder 1999-2001.
 *
 * email: <christian@newkind.co.uk>
 *
 *
 */

using Elite.Engine.Enums;

namespace Elite.Engine.Views
{
    internal class Options : IView
    {
        private readonly GameState _gameState;
        private readonly IGfx _gfx;
        private readonly Draw _draw;
        private readonly IKeyboard _keyboard;
        private int _highlightedItem;
        private const int OptionBarWidth = 400;
        private const int OptionBarHeight = 15;

        private readonly (string Label, bool DockedOnly)[] optionList =
        {
            new("Save Commander",   true),
            new("Load Commander",   true),
            new("Game Settings",    false),
            new ("Quit",            false)
        };

        internal Options(GameState gameState, IGfx gfx, Draw draw, IKeyboard keyboard)
        {
            _gameState = gameState;
            _gfx = gfx;
            _draw = draw;
            _keyboard = keyboard;
        }

        public void Reset()
        {
            _highlightedItem = 0;
        }

        public void UpdateUniverse()
        {
        }

        public void Draw()
        {
            _draw.ClearDisplay();
            _draw.DrawViewHeader("GAME OPTIONS");

            for (int i = 0; i < optionList.Length; i++)
            {
                int y = (384 - (30 * optionList.Length)) / 2;
                y += i * 30;

                if (i == _highlightedItem)
                {
                    float x = Graphics.GFX_X_CENTRE - (OptionBarWidth / 2);
                    _gfx.DrawRectangleFilled(x, y - 7, OptionBarWidth, OptionBarHeight, GFX_COL.GFX_COL_DARK_RED);
                }

                GFX_COL col = ((!EliteMain.docked) && optionList[i].DockedOnly) ? GFX_COL.GFX_COL_GREY_1 : GFX_COL.GFX_COL_WHITE;

                _gfx.DrawTextCentre(y, optionList[i].Label, 120, col);
            }

            _gfx.DrawTextCentre(300, $"Version: {typeof(Options).Assembly.GetName().Version}", 120, GFX_COL.GFX_COL_WHITE);
            _gfx.DrawTextCentre(320, "The Sharp Kind - Andy Hawkins 2023", 120, GFX_COL.GFX_COL_WHITE);
            _gfx.DrawTextCentre(340, "The New Kind - Christian Pinder 1999-2001", 120, GFX_COL.GFX_COL_WHITE);
            _gfx.DrawTextCentre(360, "Original Code - Ian Bell & David Braben", 120, GFX_COL.GFX_COL_WHITE);
        }

        public void HandleInput()
        {
            if (_keyboard.IsKeyPressed(CommandKey.Up, CommandKey.UpArrow))
            {
                _highlightedItem = Math.Clamp(_highlightedItem - 1, 0, optionList.Length - 1);
            }
            if (_keyboard.IsKeyPressed(CommandKey.Down, CommandKey.DownArrow))
            {
                _highlightedItem = Math.Clamp(_highlightedItem + 1, 0, optionList.Length - 1);
            }
            if (_keyboard.IsKeyPressed(CommandKey.Enter))
            {
                ExecuteOption();
            }
        }

        private void ExecuteOption()
        {
            if (EliteMain.docked || !optionList[_highlightedItem].DockedOnly)
            {
                switch (_highlightedItem)
                {
                    case 0:
                        _gameState.SetView(SCR.SCR_SAVE_CMDR);
                        break;

                    case 1:
                        _gameState.SetView(SCR.SCR_LOAD_CMDR);
                        break;

                    case 2:
                        _gameState.SetView(SCR.SCR_SETTINGS);
                        break;

                    case 3:
                        _gameState.SetView(SCR.SCR_QUIT);
                        break;
                }
            }
        }
    }
}