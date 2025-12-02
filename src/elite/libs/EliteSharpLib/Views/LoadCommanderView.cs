// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Graphics;
using EliteSharpLib.Save;
using Useful.Controls;

namespace EliteSharpLib.Views;

internal sealed class LoadCommanderView : IView
{
    private readonly IEliteDraw _draw;
    private readonly GameState _gameState;
    private readonly IKeyboard _keyboard;
    private readonly SaveFile _save;
    private readonly uint _colorWhite;
    private readonly uint _colorGold;

    private bool _isLoaded = true;
    private string _name = string.Empty;

    internal LoadCommanderView(GameState gameState, IEliteDraw draw, IKeyboard keyboard, SaveFile save)
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
        _draw.DrawViewHeader("LOAD COMMANDER");

        _draw.Graphics.DrawTextCentre(75, "Please enter commander name:", (int)FontType.Small, _colorWhite);
        _draw.Graphics.DrawRectangleCentre(100, 312, 50, _colorWhite);
        _draw.Graphics.DrawTextCentre(112, _name, (int)FontType.Large, _colorWhite);

        if (!_isLoaded)
        {
            _draw.Graphics.DrawTextCentre(175, "Error Loading Commander!", (int)FontType.Large, _colorGold);
            _draw.Graphics.DrawTextCentre(200, "Press SPACE to continue.", (int)FontType.Small, _colorWhite);
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
            _isLoaded = _save.LoadCommander(_name);
            if (_isLoaded)
            {
                _save.GetLastSave();
                _gameState.SetView(Screen.CommanderStatus);
            }
        }

        if (_keyboard.IsPressed(ConsoleKey.Spacebar))
        {
            _gameState.SetView(Screen.CommanderStatus);
        }
    }

    public void Reset()
    {
        _keyboard.ClearPressed();
        _name = _gameState.Cmdr.Name;
        _isLoaded = true;
    }

    public void UpdateUniverse()
    {
    }
}
