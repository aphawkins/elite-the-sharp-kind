﻿// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using Elite.Engine.Enums;
using Elite.Engine.Save;

namespace Elite.Engine.Views
{
    internal sealed class SaveCommanderView : IView
    {
        private readonly Draw _draw;
        private readonly GameState _gameState;
        private readonly IGraphics _graphics;
        private readonly IKeyboard _keyboard;
        private readonly SaveFile _save;
        private bool? _isSuccess;
        private string _name = string.Empty;

        internal SaveCommanderView(GameState gameState, IGraphics graphics, Draw draw, IKeyboard keyboard, SaveFile save)
        {
            _gameState = gameState;
            _graphics = graphics;
            _draw = draw;
            _keyboard = keyboard;
            _save = save;
        }

        public void Draw()
        {
            _draw.ClearDisplay();
            _draw.DrawViewHeader("SAVE COMMANDER");

            _graphics.DrawTextCentre(75, "Please enter commander name:", 120, Colour.White1);
            _graphics.DrawRectangle(100, 100, 312, 50, Colour.White1);
            _graphics.DrawTextCentre(125, _name, 140, Colour.White1);

            if (_isSuccess.HasValue)
            {
                if (_isSuccess.Value)
                {
                    _graphics.DrawTextCentre(175, "Commander Saved.", 140, Colour.Gold);
                    _graphics.DrawTextCentre(200, "Press SPACE to continue.", 120, Colour.White1);
                }
                else
                {
                    _graphics.DrawTextCentre(175, "Error Saving Commander!", 140, Colour.Gold);
                    _graphics.DrawTextCentre(200, "Press SPACE to continue.", 120, Colour.White1);
                }
            }
        }

        public void HandleInput()
        {
            if (_keyboard.IsKeyPressed(CommandKey.Backspace))
            {
                if (!string.IsNullOrEmpty(_name))
                {
                    _name = _name[..^1];
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