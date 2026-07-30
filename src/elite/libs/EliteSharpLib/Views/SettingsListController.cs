// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Config;
using Useful.Config;
using Useful.Controls;
using Useful.Maths;

namespace EliteSharpLib.Views;

/// <summary>
/// A two-column list of settings with a Back row underneath: the cursor and
/// navigation the engine and game settings screens share. Each screen
/// supplies its own rows, reads its own current values and applies its own
/// toggles; saving is common, since every change is written as it is made.
/// </summary>
internal abstract class SettingsListController(
    GameState gameState,
    IKeyboard keyboard,
    IConfigWriter<EliteConfig> configWriter,
    string header,
    (string Name, string[] Values)[] settings,
    IView<SettingsListModel> view,
    string footer = "") : IScreenController
{
    // The Back row is the same on every screen, so it's appended here rather
    // than repeated in each list.
    private readonly (string Name, string[] Values)[] _settingList = [.. settings, new("Back", [string.Empty])];

    private int _highlightedItem;

    protected GameState State => gameState;

    public void Draw() => view.Draw(BuildModel());

    public void HandleInput()
    {
        if (keyboard.IsPressed(ConsoleKey.S) || keyboard.IsPressed(ConsoleKey.UpArrow))
        {
            SelectUp();
        }

        if (keyboard.IsPressed(ConsoleKey.X) || keyboard.IsPressed(ConsoleKey.DownArrow))
        {
            SelectDown();
        }

        if (keyboard.IsPressed(ConsoleKey.OemComma) || keyboard.IsPressed(ConsoleKey.LeftArrow))
        {
            SelectLeft();
        }

        if (keyboard.IsPressed(ConsoleKey.OemPeriod) || keyboard.IsPressed(ConsoleKey.RightArrow))
        {
            SelectRight();
        }

        if (keyboard.IsPressed(ConsoleKey.Enter))
        {
            Toggle();
        }
    }

    public void Reset() => _highlightedItem = 0;

    public void Update()
    {
    }

    // Exposed for tests: the resolved rows and the cursor position.
    internal SettingsListModel BuildModel()
    {
        SettingsRow[] rows = new SettingsRow[_settingList.Length];
        for (int i = 0; i < _settingList.Length - 1; i++)
        {
            rows[i] = new(_settingList[i].Name, _settingList[i].Values[SettingValue(i)]);
        }

        rows[^1] = new(_settingList[^1].Name, string.Empty);

        return new(header, rows, _highlightedItem, footer);
    }

    // The enums behind these settings all run contiguously from zero, so
    // cycling one is the next value modulo the count.
    protected static TEnum Next<TEnum>(TEnum value)
        where TEnum : struct, Enum
        => (TEnum)Enum.ToObject(typeof(TEnum), (Convert.ToInt32(value, null) + 1) % Enum.GetValues<TEnum>().Length);

    /// <summary>
    /// Which of setting <paramref name="index"/>'s values is currently selected.
    /// </summary>
    /// <param name="index">The setting's row, in the order the screen listed them.</param>
    /// <returns>The index of the selected value.</returns>
    protected abstract int SettingValue(int index);

    /// <summary>
    /// Advance setting <paramref name="index"/> to its next value and apply it.
    /// </summary>
    /// <param name="index">The setting's row, in the order the screen listed them.</param>
    protected abstract void ToggleSetting(int index);

    private void Toggle()
    {
        if (_highlightedItem == _settingList.Length - 1)
        {
            State.SetView(Screen.Options);
            return;
        }

        ToggleSetting(_highlightedItem);

        // Every change is live and saved as it's made, so there's no save step.
        configWriter.WriteConfig(State.Config);
    }

    private void SelectDown()
    {
        if (_highlightedItem == _settingList.Length - 2)
        {
            _highlightedItem = _settingList.Length - 1;
        }

        if (_highlightedItem < _settingList.Length - 2)
        {
            _highlightedItem += 2;
        }
    }

    private void SelectLeft()
    {
        if (_highlightedItem.IsOdd())
        {
            _highlightedItem--;
        }
    }

    private void SelectRight()
    {
        if (!_highlightedItem.IsOdd() && _highlightedItem < _settingList.Length - 1)
        {
            _highlightedItem++;
        }
    }

    private void SelectUp()
    {
        if (_highlightedItem == _settingList.Length - 1)
        {
            _highlightedItem = _settingList.Length - 2;
        }

        if (_highlightedItem > 1)
        {
            _highlightedItem -= 2;
        }
    }
}
