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

namespace Elite.Engine.Views
{
    using Elite.Engine;
    using Elite.Engine.Enums;

    internal class Options : IView
    {
        private readonly IGfx _gfx;
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

        internal Options(IGfx gfx, IKeyboard keyboard)
        {
            _gfx = gfx;
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
            elite.draw.ClearDisplay();
            elite.draw.DrawViewHeader("GAME OPTIONS");

            for (int i = 0; i < optionList.Length; i++)
            {
                int y = (384 - (30 * optionList.Length)) / 2;
                y += i * 30;

                if (i == _highlightedItem)
                {
                    float x = gfx.GFX_X_CENTRE - (OptionBarWidth / 2);
                    _gfx.DrawRectangleFilled(x, y - 7, OptionBarWidth, OptionBarHeight, GFX_COL.GFX_COL_DARK_RED);
                }

                GFX_COL col = ((!elite.docked) && optionList[i].DockedOnly) ? GFX_COL.GFX_COL_GREY_1 : GFX_COL.GFX_COL_WHITE;

                _gfx.DrawTextCentre(y, optionList[i].Label, 120, col);
            }

            _gfx.DrawTextCentre(300, $"Version: {typeof(Options).Assembly.GetName().Version}", 120, GFX_COL.GFX_COL_WHITE);
            _gfx.DrawTextCentre(320, "The Sharp Kind - Andy Hawkins 2023", 120, GFX_COL.GFX_COL_WHITE);
            _gfx.DrawTextCentre(340, "The New Kind - Christian Pinder 1999-2001", 120, GFX_COL.GFX_COL_WHITE);
            _gfx.DrawTextCentre(360, "Original Code - Ian Bell & David Braben", 120, GFX_COL.GFX_COL_WHITE);
        }

        public void HandleInput()
        {
            if (_keyboard.IsKeyPressed(CommandKey.Up))
            {
                _highlightedItem = Math.Clamp(_highlightedItem - 1, 0, optionList.Length - 1);
            }
            if (_keyboard.IsKeyPressed(CommandKey.Down))
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
            if (elite.docked || !optionList[_highlightedItem].DockedOnly)
            {
                switch (_highlightedItem)
                {
                    case 0:
                        elite.SetView(SCR.SCR_SAVE_CMDR);
                        break;

                    case 1:
                        elite.SetView(SCR.SCR_LOAD_CMDR);
                        break;

                    case 2:
                        elite.SetView(SCR.SCR_SETTINGS);
                        break;

                    case 3:
                        elite.SetView(SCR.SCR_QUIT);
                        break;
                }
            }
        }
    }
}