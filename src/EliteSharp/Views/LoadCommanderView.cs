// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Controls;
using EliteSharp.Graphics;
using EliteSharp.Save;

namespace EliteSharp.Views
{
    internal sealed class LoadCommanderView : IView
    {
        private readonly IDraw _draw;
        private readonly GameState _gameState;
        private readonly IGraphics _graphics;
        private readonly IKeyboard _keyboard;
        private readonly SaveFile _save;
        private bool _isLoaded = true;
        private string _name = string.Empty;

        internal LoadCommanderView(GameState gameState, IGraphics graphics, IDraw draw, IKeyboard keyboard, SaveFile save)
        {
            _gameState = gameState;
            _graphics = graphics;
            _draw = draw;
            _keyboard = keyboard;
            _save = save;
        }

        public void Draw()
        {
            _draw.DrawViewHeader("LOAD COMMANDER");

            _graphics.DrawTextCentre(75, "Please enter commander name:", FontSize.Small, Colour.White);
            _graphics.DrawRectangleCentre(100, 312, 50, Colour.White);
            _graphics.DrawTextCentre(112, _name, FontSize.Large, Colour.White);

            if (!_isLoaded)
            {
                _graphics.DrawTextCentre(175, "Error Loading Commander!", FontSize.Large, Colour.Gold);
                _graphics.DrawTextCentre(200, "Press SPACE to continue.", FontSize.Small, Colour.White);
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
                _isLoaded = _save.LoadCommanderAsync(_name).Result;
                if (_isLoaded)
                {
                    _save.GetLastSave();
                    _gameState.SetView(Screen.CommanderStatus);
                }
            }

            if (_keyboard.IsKeyPressed(CommandKey.SpaceBar))
            {
                _gameState.SetView(Screen.CommanderStatus);
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
