using Elite.Engine.Enums;
using Elite.Engine.Save;

namespace Elite.Engine.Views
{
    internal class SaveCommander : IView
    {
        private readonly GameState _gameState;
        private readonly IGfx _gfx;
        private readonly Draw _draw;
        private readonly IKeyboard _keyboard;
        private readonly SaveFile _save;
        private string _name = string.Empty;
        private bool? _isSuccess = null;

        internal SaveCommander(GameState gameState, IGfx gfx, Draw draw, IKeyboard keyboard, SaveFile save)
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
            _draw.DrawViewHeader("SAVE COMMANDER");

            _gfx.DrawTextCentre(75, "Please enter commander name:", 120, GFX_COL.GFX_COL_WHITE);
            _gfx.DrawRectangle(100, 100, 312, 50, GFX_COL.GFX_COL_WHITE);
            _gfx.DrawTextCentre(125, _name, 140, GFX_COL.GFX_COL_WHITE);

            if (_isSuccess.HasValue)
            {
                if (_isSuccess.Value)
                {
                    _gfx.DrawTextCentre(175, "Commander Saved.", 140, GFX_COL.GFX_COL_GOLD);
                    _gfx.DrawTextCentre(200, "Press SPACE to continue.", 120, GFX_COL.GFX_COL_WHITE);
                }
                else
                {
                    _gfx.DrawTextCentre(175, "Error Saving Commander!", 140, GFX_COL.GFX_COL_GOLD);
                    _gfx.DrawTextCentre(200, "Press SPACE to continue.", 120, GFX_COL.GFX_COL_WHITE);
                }
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
                _isSuccess = _save.SaveCommanderAsync(_name).Result;

                if (_isSuccess.HasValue && _isSuccess.Value)
                {
                    _save.GetLastSave();
                }
            }

            if (_keyboard.IsKeyPressed(CommandKey.SpaceBar))
            {
                _gameState.SetView(SCR.SCR_OPTIONS);
            }
        }

        public void Reset()
        {
            _isSuccess = null;
            _name = _gameState.Cmdr.Name;
        }

        public void UpdateUniverse()
        {
        }
    }
}