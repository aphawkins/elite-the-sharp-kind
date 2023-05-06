// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using Elite.Engine.Enums;
using Elite.Engine.Save;

namespace Elite.Engine.Views
{
    internal class LoadCommanderView : IView
    {
        private readonly Draw _draw;
        private readonly GameState _gameState;
        private readonly IGfx _gfx;
        private readonly IKeyboard _keyboard;
        private readonly SaveFile _save;
        private bool _isLoaded = true;
        private string _name = string.Empty;
        internal LoadCommanderView(GameState gameState, IGfx gfx, Draw draw, IKeyboard keyboard, SaveFile save)
        {
            _gameState = gameState;
            _gfx = gfx;
            _draw = draw;
            _keyboard = keyboard;
            _save = save;
        }

        public void Draw()
        {
            _draw.ClearDisplay();
            _draw.DrawViewHeader("LOAD COMMANDER");

            _gfx.DrawTextCentre(75, "Please enter commander name:", 120, GFX_COL.GFX_COL_WHITE);
            _gfx.DrawRectangle(100, 100, 312, 50, GFX_COL.GFX_COL_WHITE);
            _gfx.DrawTextCentre(125, _name, 140, GFX_COL.GFX_COL_WHITE);

            if (!_isLoaded)
            {
                _gfx.DrawTextCentre(175, "Error Loading Commander!", 140, GFX_COL.GFX_COL_GOLD);
                _gfx.DrawTextCentre(200, "Press SPACE to continue.", 120, GFX_COL.GFX_COL_WHITE);
            }
        }

        public void HandleInput()
        {
            if (_keyboard.IsKeyPressed(CommandKey.Backspace))
            {
                if (!string.IsNullOrEmpty(_name))
                {
                    _name = _name[..^1]; ;
                }
            }

            char key = (char)_keyboard.GetKeyPressed();

            if (key is >= 'A' and <= 'Z')
            {
                _name += key;
            }

            if (_keyboard.IsKeyPressed(CommandKey.Enter))
            {
                _isLoaded = _save.LoadCommanderAsync(_name).Result;
                if (_isLoaded)
                {
                    _save.GetLastSave();
                    _gameState.SetView(SCR.SCR_CMDR_STATUS);
                }
            }

            if (_keyboard.IsKeyPressed(CommandKey.SpaceBar))
            {
                _gameState.SetView(SCR.SCR_CMDR_STATUS);
            }
        }

        public void Reset()
        {
            _keyboard.ClearKeyPressed();
            _name = _gameState.Cmdr.Name;
            _isLoaded = true;
        }

        public void UpdateUniverse()
        {
        }
    }
}
