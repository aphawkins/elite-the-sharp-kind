// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Controls;
using EliteSharp.Graphics;

namespace EliteSharp.Views
{
    internal sealed class QuitView : IView
    {
        private readonly Draw _draw;
        private readonly GameState _gameState;
        private readonly IGraphics _graphics;
        private readonly IKeyboard _keyboard;

        internal QuitView(GameState gameState, IGraphics graphics, Draw draw, IKeyboard keyboard)
        {
            _gameState = gameState;
            _graphics = graphics;
            _draw = draw;
            _keyboard = keyboard;
        }

        public void Draw()
        {
            _draw.DrawViewHeader("GAME OPTIONS");

            _graphics.DrawTextCentre(175, "QUIT GAME (Y/N)?", FontSize.Large, Colour.Gold);
        }

        public void HandleInput()
        {
            if (_keyboard.IsKeyPressed(CommandKey.Yes))
            {
                _gameState.DoExitGame();
            }

            if (_keyboard.IsKeyPressed(CommandKey.No))
            {
                if (_gameState.IsDocked)
                {
                    _gameState.SetView(Screen.CommanderStatus);
                }
                else
                {
                    _gameState.SetView(Screen.FrontView);
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
