// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using Useful.Controls;

namespace EliteSharpLib.Views;

/// <summary>
/// The options menu's behaviour: the cursor over its five rows, and choosing
/// one greys out the docked-only rows while undocked rather than hiding them.
/// </summary>
internal sealed class OptionsController : IScreenController
{
    private static readonly IReadOnlyList<string> s_credits =
    [
        "The Sharp Kind - A Hawkins",
        "The New Kind - C Pinder",
        "Original Game - I Bell & D Braben",
    ];

    private readonly (string Label, bool DockedOnly)[] _optionList =
    [
        new("Save Commander", true),
        new("Load Commander", true),
        new("Game Settings", false),
        new("Engine Settings", false),
        new("Quit", false),
    ];

    private readonly GameState _gameState;
    private readonly IKeyboard _keyboard;
    private readonly IView<OptionsModel> _view;

    private int _highlightedItem;

    internal OptionsController(GameState gameState, IKeyboard keyboard, IView<OptionsModel> view)
    {
        _gameState = gameState;
        _keyboard = keyboard;
        _view = view;
    }

    public void Draw() => _view.Draw(BuildModel());

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

    public void Update()
    {
    }

    // Exposed for tests: the rows' enabled state and the cursor position.
    internal OptionsModel BuildModel()
    {
        OptionRow[] rows = new OptionRow[_optionList.Length];
        for (int i = 0; i < _optionList.Length; i++)
        {
            rows[i] = new(_optionList[i].Label, _gameState.IsDocked || !_optionList[i].DockedOnly);
        }

        return new(
            rows,
            _highlightedItem,
            $"Version: {typeof(OptionsController).Assembly.GetName().Version}",
            s_credits);
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
                    _gameState.SetView(Screen.EngineSettings);
                    break;

                case 4:
                    _gameState.SetView(Screen.Quit);
                    break;
            }
        }
    }
}
