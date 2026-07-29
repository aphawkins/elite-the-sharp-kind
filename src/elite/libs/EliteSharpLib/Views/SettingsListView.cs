// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using EliteSharpLib.Config;
using EliteSharpLib.Graphics;
using Useful.Config;
using Useful.Controls;
using Useful.Maths;

namespace EliteSharpLib.Views;

/// <summary>
/// A two-column list of settings with a Back row underneath: the layout and
/// navigation the engine and game settings screens share. Each screen supplies
/// its own rows, reads its own current values and applies its own toggles;
/// saving is common, since every change is written as it is made.
/// </summary>
internal abstract class SettingsListView(
    GameState gameState,
    IEliteDraw draw,
    IKeyboard keyboard,
    IConfigWriter<EliteConfig> configWriter,
    string header,
    (string Name, string[] Values)[] settings,
    string footer = "") : IScreenController
{
    // The Back row is the same on every screen, so it's appended here rather
    // than repeated in each list.
    private readonly (string Name, string[] Values)[] _settingList = [.. settings, new("Back", [string.Empty])];

    private readonly uint _colorWhite = draw.Palette["White"];
    private readonly uint _colorLightRed = draw.Palette["LightRed"];

    private int _highlightedItem;

    protected IEliteDraw Draw2D => draw;

    protected GameState State => gameState;

    public void Draw()
    {
        Draw2D.DrawViewHeader(header);

        for (int i = 0; i < _settingList.Length; i++)
        {
            Vector2 position;

            if (i == _settingList.Length - 1)
            {
                position.Y = ((_settingList.Length + 1) / 2 * 30) + (Draw2D.Centre.Y / 2) + 32;
                if (i == _highlightedItem)
                {
                    position.X = Draw2D.Centre.X - 200;
                    Draw2D.Graphics.DrawRectangleFilled(position, 400, 15, _colorLightRed);
                }

                Draw2D.Graphics.DrawTextCentre(position.Y, _settingList[i].Name, nameof(FontType.Small), _colorWhite);

                if (footer.Length > 0)
                {
                    Draw2D.Graphics.DrawTextCentre(position.Y + 40, footer, nameof(FontType.Small), _colorWhite);
                }

                return;
            }

            int v = SettingValue(i);

            position.X = ((i & 1) * 250) + 32 + Draw2D.Offset;
            position.Y = (i / 2 * 30) + (Draw2D.Centre.Y / 2);

            if (i == _highlightedItem)
            {
                Draw2D.Graphics.DrawRectangleFilled(position, 100, 15, _colorLightRed);
            }

            Draw2D.Graphics.DrawTextLeft(position, _settingList[i].Name, nameof(FontType.Small), _colorWhite);
            position.X += 120;
            Draw2D.Graphics.DrawTextLeft(position, _settingList[i].Values[v], nameof(FontType.Small), _colorWhite);
        }
    }

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
