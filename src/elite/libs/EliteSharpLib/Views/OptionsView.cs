// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using EliteSharpLib.Graphics;
using Useful.Controls;

namespace EliteSharpLib.Views;

internal sealed class OptionsView : IView
{
    private const int OptionBarHeight = 15;
    private const int OptionBarWidth = 400;
    private readonly IEliteDraw _draw;
    private readonly GameState _gameState;
    private readonly IKeyboard _keyboard;
    private readonly uint _colorWhite;
    private readonly uint _colorLightRed;
    private readonly uint _colorLightGrey;

    private readonly (string Label, bool DockedOnly)[] _optionList =
    [
        new("Save Commander", true),
        new("Load Commander", true),
        new("Game Settings", false),
        new("Quit", false),
    ];

    private int _highlightedItem;

    internal OptionsView(GameState gameState, IEliteDraw draw, IKeyboard keyboard)
    {
        _gameState = gameState;
        _draw = draw;
        _keyboard = keyboard;

        _colorWhite = draw.Palette["White"];
        _colorLightRed = draw.Palette["LightRed"];
        _colorLightGrey = draw.Palette["LightGrey"];
    }

    public void Draw()
    {
        _draw.DrawViewHeader("GAME OPTIONS");

        for (int i = 0; i < _optionList.Length; i++)
        {
            Vector2 position = new(
                _draw.Centre.X - (OptionBarWidth / 2),
                ((_draw.ScannerTop - (30 * _optionList.Length)) / 2) + (i * 30));

            if (i == _highlightedItem)
            {
                _draw.Graphics.DrawRectangleFilled(position, OptionBarWidth, OptionBarHeight, _colorLightRed);
            }

            uint col = ((!_gameState.IsDocked) && _optionList[i].DockedOnly) ? _colorLightGrey : _colorWhite;

            _draw.Graphics.DrawTextCentre(position.Y, _optionList[i].Label, (int)FontType.Small, col);
        }

        _draw.Graphics.DrawTextCentre(
            _draw.ScannerTop - 80,
            $"Version: {typeof(OptionsView).Assembly.GetName().Version}",
            (int)FontType.Small,
            _colorWhite);
        _draw.Graphics.DrawTextCentre(
            _draw.ScannerTop - 60,
            "The Sharp Kind - Andy Hawkins 2023",
            (int)FontType.Small,
            _colorWhite);
        _draw.Graphics.DrawTextCentre(
            _draw.ScannerTop - 40,
            "The New Kind - Christian Pinder 1999-2001",
            (int)FontType.Small,
            _colorWhite);
        _draw.Graphics.DrawTextCentre(
            _draw.ScannerTop - 20,
            "Original Code - Ian Bell & David Braben",
            (int)FontType.Small,
            _colorWhite);
    }

    public void HandleInput()
    {
        if (_keyboard.IsPressed(ConsoleKey.S) || _keyboard.IsPressed(ConsoleKey.UpArrow))
        {
            _highlightedItem = Math.Clamp(_highlightedItem - 1, 0, _optionList.Length - 1);
        }

        if (_keyboard.IsPressed(ConsoleKey.X) || _keyboard.IsPressed(ConsoleKey.DownArrow))
        {
            _highlightedItem = Math.Clamp(_highlightedItem + 1, 0, _optionList.Length - 1);
        }

        if (_keyboard.IsPressed(ConsoleKey.Enter))
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
            }
        }
    }
}
