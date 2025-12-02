// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Graphics;
using EliteSharpLib.Save;
using Useful.Controls;

namespace EliteSharpLib.Views;

internal sealed class SaveCommanderView : IView
{
    private readonly IEliteDraw _draw;
    private readonly GameState _gameState;
    private readonly IKeyboard _keyboard;
    private readonly SaveFile _save;
    private readonly uint _colorWhite;
    private readonly uint _colorGold;

    private bool? _isSuccess;
    private string _name = string.Empty;

    internal SaveCommanderView(GameState gameState, IEliteDraw draw, IKeyboard keyboard, SaveFile save)
    {
        _gameState = gameState;
        _draw = draw;
        _keyboard = keyboard;
        _save = save;

        _colorGold = draw.Palette["Gold"];
        _colorWhite = draw.Palette["White"];
    }

    public void Draw()
    {
        _draw.DrawViewHeader("SAVE COMMANDER");

        _draw.Graphics.DrawTextCentre(75, "Please enter commander name:", (int)FontType.Small, _colorWhite);
        _draw.Graphics.DrawRectangle(new(100 + _draw.Offset, 100), 312, 50, _colorWhite);
        _draw.Graphics.DrawTextCentre(112, _name, (int)FontType.Large, _colorWhite);

        if (_isSuccess.HasValue)
        {
            if (_isSuccess.Value)
            {
                _draw.Graphics.DrawTextCentre(175, "Commander Saved.", (int)FontType.Large, _colorGold);
                _draw.Graphics.DrawTextCentre(200, "Press SPACE to continue.", (int)FontType.Small, _colorWhite);
            }
            else
            {
                _draw.Graphics.DrawTextCentre(175, "Error Saving Commander!", (int)FontType.Large, _colorGold);
                _draw.Graphics.DrawTextCentre(200, "Press SPACE to continue.", (int)FontType.Small, _colorWhite);
            }
        }
    }

    public void HandleInput()
    {
        if (_keyboard.IsPressed(ConsoleKey.Backspace) &&
            !string.IsNullOrEmpty(_name))
        {
            _name = _name[..^1];
        }

        (ConsoleKey key, ConsoleModifiers _) = _keyboard.LastPressed();
        if (key is >= ConsoleKey.A and <= ConsoleKey.Z)
        {
            _name += (char)key;
        }

        if (_keyboard.IsPressed(ConsoleKey.Enter))
        {
            _isSuccess = _save.SaveCommander(_name);

            if (_isSuccess.Value)
            {
                _save.GetLastSave();
            }
        }

        if (_keyboard.IsPressed(ConsoleKey.Spacebar))
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
