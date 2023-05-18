// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using Elite.Engine.Enums;

namespace Elite.Engine.Views
{
    internal sealed class OptionsView : IView
    {
        private const int OptionBarHeight = 15;
        private const int OptionBarWidth = 400;
        private readonly Draw _draw;
        private readonly GameState _gameState;
        private readonly IGraphics _graphics;
        private readonly IKeyboard _keyboard;

        private readonly (string Label, bool DockedOnly)[] _optionList =
        {
            new("Save Commander",   true),
            new("Load Commander",   true),
            new("Game Settings",    false),
            new("Quit",            false),
        };

        private int _highlightedItem;

        internal OptionsView(GameState gameState, IGraphics graphics, Draw draw, IKeyboard keyboard)
        {
            _gameState = gameState;
            _graphics = graphics;
            _draw = draw;
            _keyboard = keyboard;
        }

        public void Draw()
        {
            _draw.ClearDisplay();
            _draw.DrawViewHeader("GAME OPTIONS");

            for (int i = 0; i < _optionList.Length; i++)
            {
                int y = (384 - (30 * _optionList.Length)) / 2;
                y += i * 30;

                if (i == _highlightedItem)
                {
                    float x = _graphics.Centre.X - (OptionBarWidth / 2);
                    _graphics.DrawRectangleFilled(x, y - 7, OptionBarWidth, OptionBarHeight, Colour.LightRed);
                }

                Colour col = ((!_gameState.IsDocked) && _optionList[i].DockedOnly) ? Colour.LightGrey : Colour.White;

                _graphics.DrawTextCentre(y, _optionList[i].Label, 120, col);
            }

            _graphics.DrawTextCentre(300, $"Version: {typeof(OptionsView).Assembly.GetName().Version}", 120, Colour.White);
            _graphics.DrawTextCentre(320, "The Sharp Kind - Andy Hawkins 2023", 120, Colour.White);
            _graphics.DrawTextCentre(340, "The New Kind - Christian Pinder 1999-2001", 120, Colour.White);
            _graphics.DrawTextCentre(360, "Original Code - Ian Bell & David Braben", 120, Colour.White);
        }

        public void HandleInput()
        {
            if (_keyboard.IsKeyPressed(CommandKey.Up, CommandKey.UpArrow))
            {
                _highlightedItem = Math.Clamp(_highlightedItem - 1, 0, _optionList.Length - 1);
            }

            if (_keyboard.IsKeyPressed(CommandKey.Down, CommandKey.DownArrow))
            {
                _highlightedItem = Math.Clamp(_highlightedItem + 1, 0, _optionList.Length - 1);
            }

            if (_keyboard.IsKeyPressed(CommandKey.Enter))
            {
                ExecuteOption();
            }
        }

        public void Reset() => _highlightedItem = 0;

        public void UpdateUniverse()
        {
        }

        private void ExecuteOption()
        {
            if (_gameState.IsDocked || !_optionList[_highlightedItem].DockedOnly)
            {
                switch (_highlightedItem)
                {
                    case 0:
                        _gameState.SetView(Screen.SaveCommander);
                        break;

                    case 1:
                        _gameState.SetView(Screen.LoadCommander);
                        break;

                    case 2:
                        _gameState.SetView(Screen.Settings);
                        break;

                    case 3:
                        _gameState.SetView(Screen.Quit);
                        break;

                    default:
                        break;
                }
            }
        }
    }
}
