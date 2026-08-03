// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Abstractions.Views;
using Useful.Controls;

namespace EliteSharpLib.Views;

/// <summary>
/// The quit confirmation: Y leaves the game, N returns to wherever the
/// commander was.
/// </summary>
internal sealed class QuitController : IScreenController
{
    private readonly GameState _gameState;
    private readonly IKeyboard _keyboard;
    private readonly IView<QuitModel> _view;

    internal QuitController(GameState gameState, IKeyboard keyboard, IView<QuitModel> view)
    {
        _gameState = gameState;
        _keyboard = keyboard;
        _view = view;
    }

    public void Draw() => _view.Draw(BuildModel());

    public void HandleInput()
    {
        if (_keyboard.IsPressed(ConsoleKey.Y))
        {
            _gameState.DoExitGame();
        }

        if (_keyboard.IsPressed(ConsoleKey.N))
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

    public void Update()
    {
    }

    // Exposed for tests.
    internal static QuitModel BuildModel() => new("GAME OPTIONS", "QUIT GAME (Y/N)?");
}
