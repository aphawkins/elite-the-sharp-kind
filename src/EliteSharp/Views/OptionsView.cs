﻿// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using EliteSharp.Controls;
using EliteSharp.Graphics;

namespace EliteSharp.Views
{
    internal sealed class OptionsView : IView
    {
        private const int OptionBarHeight = 15;
        private const int OptionBarWidth = 400;
        private readonly IDraw _draw;
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

        internal OptionsView(GameState gameState, IGraphics graphics, IDraw draw, IKeyboard keyboard)
        {
            _gameState = gameState;
            _graphics = graphics;
            _draw = draw;
            _keyboard = keyboard;
        }

        public void Draw()
        {
            _draw.DrawViewHeader("GAME OPTIONS");

            for (int i = 0; i < _optionList.Length; i++)
            {
                Vector2 position = new(_draw.Centre.X - (OptionBarWidth / 2), ((_draw.ScannerTop - (30 * _optionList.Length)) / 2) + (i * 30));

                if (i == _highlightedItem)
                {
                    _graphics.DrawRectangleFilled(position, OptionBarWidth, OptionBarHeight, Colour.LightRed);
                }

                Colour col = ((!_gameState.IsDocked) && _optionList[i].DockedOnly) ? Colour.LightGrey : Colour.White;

                _graphics.DrawTextCentre(position.Y, _optionList[i].Label, FontSize.Small, col);
            }

            _graphics.DrawTextCentre(_draw.ScannerTop - 80, $"Version: {typeof(OptionsView).Assembly.GetName().Version}", FontSize.Small, Colour.White);
            _graphics.DrawTextCentre(_draw.ScannerTop - 60, "The Sharp Kind - Andy Hawkins 2023", FontSize.Small, Colour.White);
            _graphics.DrawTextCentre(_draw.ScannerTop - 40, "The New Kind - Christian Pinder 1999-2001", FontSize.Small, Colour.White);
            _graphics.DrawTextCentre(_draw.ScannerTop - 20, "Original Code - Ian Bell & David Braben", FontSize.Small, Colour.White);
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
