// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Controls;
using EliteSharp.Graphics;
using EliteSharp.Save;

namespace EliteSharp.Views
{
    internal sealed class SaveCommanderView : IView
    {
        private readonly IDraw _draw;
        private readonly GameState _gameState;
        private readonly IKeyboard _keyboard;
        private readonly SaveFile _save;
        private bool? _isSuccess;
        private string _name = string.Empty;

        internal SaveCommanderView(GameState gameState, IDraw draw, IKeyboard keyboard, SaveFile save)
        {
            _gameState = gameState;
            _draw = draw;
            _keyboard = keyboard;
            _save = save;
        }

        public void Draw()
        {
            _draw.DrawViewHeader("SAVE COMMANDER");

            _draw.Graphics.DrawTextCentre(75, "Please enter commander name:", FontSize.Small, EColors.White);
            _draw.Graphics.DrawRectangle(new(100 + _draw.Offset, 100), 312, 50, EColors.White);
            _draw.Graphics.DrawTextCentre(112, _name, FontSize.Large, EColors.White);

            if (_isSuccess.HasValue)
            {
                if (_isSuccess.Value)
                {
                    _draw.Graphics.DrawTextCentre(175, "Commander Saved.", FontSize.Large, EColors.Gold);
                    _draw.Graphics.DrawTextCentre(200, "Press SPACE to continue.", FontSize.Small, EColors.White);
                }
                else
                {
                    _draw.Graphics.DrawTextCentre(175, "Error Saving Commander!", FontSize.Large, EColors.Gold);
                    _draw.Graphics.DrawTextCentre(200, "Press SPACE to continue.", FontSize.Small, EColors.White);
                }
            }
        }

        public void HandleInput()
        {
            if (_keyboard.IsKeyPressed(CommandKey.Backspace) &&
                !string.IsNullOrEmpty(_name))
            {
                _name = _name[..^1];
            }

            char key = (char)_keyboard.GetKeyPressed();

            if (key is >= 'A' and <= 'Z')
            {
                _name += key;
            }

            if (_keyboard.IsKeyPressed(CommandKey.Enter))
            {
                _isSuccess = _save.SaveCommander(_name);

                if (_isSuccess.Value)
                {
                    _save.GetLastSave();
                }
            }

            if (_keyboard.IsKeyPressed(CommandKey.SpaceBar))
            {
                _gameState.SetView(Screen.Options);
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
