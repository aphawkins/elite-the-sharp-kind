// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Graphics;
using EliteSharp.Save;
using Useful.Controls;

namespace EliteSharp.Views;

internal sealed class SaveCommanderView : IView
{
    private readonly IEliteDraw _draw;
    private readonly GameState _gameState;
    private readonly IKeyboard _keyboard;
    private readonly SaveFile _save;
    private bool? _isSuccess;
    private string _name = string.Empty;

    internal SaveCommanderView(GameState gameState, IEliteDraw draw, IKeyboard keyboard, SaveFile save)
    {
        _gameState = gameState;
        _draw = draw;
        _keyboard = keyboard;
        _save = save;
    }

    public void Draw()
    {
        _draw.DrawViewHeader("SAVE COMMANDER");

        _draw.Graphics.DrawTextCentre(75, "Please enter commander name:", (int)FontType.Small, EliteColors.White);
        _draw.Graphics.DrawRectangle(new(100 + _draw.Offset, 100), 312, 50, EliteColors.White);
        _draw.Graphics.DrawTextCentre(112, _name, (int)FontType.Large, EliteColors.White);

        if (_isSuccess.HasValue)
        {
            if (_isSuccess.Value)
            {
                _draw.Graphics.DrawTextCentre(175, "Commander Saved.", (int)FontType.Large, EliteColors.Gold);
                _draw.Graphics.DrawTextCentre(200, "Press SPACE to continue.", (int)FontType.Small, EliteColors.White);
            }
            else
            {
                _draw.Graphics.DrawTextCentre(175, "Error Saving Commander!", (int)FontType.Large, EliteColors.Gold);
                _draw.Graphics.DrawTextCentre(200, "Press SPACE to continue.", (int)FontType.Small, EliteColors.White);
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
