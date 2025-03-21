// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using EliteSharp.Assets.Fonts;
using EliteSharp.Controls;
using EliteSharp.Graphics;

namespace EliteSharp.Views;

internal sealed class OptionsView : IView
{
    private const int OptionBarHeight = 15;
    private const int OptionBarWidth = 400;
    private readonly IDraw _draw;
    private readonly GameState _gameState;
    private readonly IKeyboard _keyboard;

    private readonly (string Label, bool DockedOnly)[] _optionList =
    [
        new("Save Commander", true),
        new("Load Commander", true),
        new("Game Settings", false),
        new("Quit", false),
    ];

    private int _highlightedItem;

    internal OptionsView(GameState gameState, IDraw draw, IKeyboard keyboard)
    {
        _gameState = gameState;
        _draw = draw;
        _keyboard = keyboard;
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
                _draw.Graphics.DrawRectangleFilled(position, OptionBarWidth, OptionBarHeight, EliteColors.LightRed);
            }

            FastColor col = ((!_gameState.IsDocked) && _optionList[i].DockedOnly) ? EliteColors.LightGrey : EliteColors.White;

            _draw.Graphics.DrawTextCentre(position.Y, _optionList[i].Label, FontType.Small, col);
        }

        _draw.Graphics.DrawTextCentre(
            _draw.ScannerTop - 80,
            $"Version: {typeof(OptionsView).Assembly.GetName().Version}",
            FontType.Small,
            EliteColors.White);
        _draw.Graphics.DrawTextCentre(
            _draw.ScannerTop - 60,
            "The Sharp Kind - Andy Hawkins 2023",
            FontType.Small,
            EliteColors.White);
        _draw.Graphics
            .DrawTextCentre(
                _draw.ScannerTop - 40,
                "The New Kind - Christian Pinder 1999-2001",
                FontType.Small,
                EliteColors.White);
        _draw.Graphics.DrawTextCentre(
            _draw.ScannerTop - 20,
            "Original Code - Ian Bell & David Braben",
            FontType.Small,
            EliteColors.White);
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
            }
        }
    }
}
