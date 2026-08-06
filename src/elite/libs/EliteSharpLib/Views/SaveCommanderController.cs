// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Abstractions.Views;
using EliteSharpLib.Save;
using Useful.Input;

namespace EliteSharpLib.Views;

/// <summary>
/// The save-commander screen's behaviour: typing a name, then saving it and
/// reporting whether it worked.
/// </summary>
internal sealed class SaveCommanderController : IScreenController
{
    private const string SuccessMessage = "Commander Saved.";
    private const string ErrorMessage = "Error Saving Commander!";

    private readonly GameState _gameState;
    private readonly IKeyboard _keyboard;
    private readonly SaveFile _save;
    private readonly IView<SaveCommanderModel> _view;

    private bool? _isSuccess;
    private string _name = string.Empty;

    internal SaveCommanderController(GameState gameState, IKeyboard keyboard, SaveFile save, IView<SaveCommanderModel> view)
    {
        _gameState = gameState;
        _keyboard = keyboard;
        _save = save;
        _view = view;
    }

    public void Draw() => _view.Draw(BuildModel());

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

    public void Update()
    {
    }

    // Exposed for tests: the typed name and the outcome of the last attempt.
    internal SaveCommanderModel BuildModel()
    {
        string statusMessage = _isSuccess switch
        {
            true => SuccessMessage,
            false => ErrorMessage,
            null => string.Empty,
        };

        return new(_name, statusMessage);
    }
}
