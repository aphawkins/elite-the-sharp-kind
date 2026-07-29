// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using EliteSharpLib.Config;
using EliteSharpLib.Graphics;
using EliteSharpLib.Planets;
using EliteSharpLib.Suns;
using Useful.Config;
using Useful.Controls;
using Useful.Maths;

namespace EliteSharpLib.Views;

internal sealed class SettingsView : IView
{
    private readonly IConfigWriter<EliteConfig> _configWriter;
    private readonly IEliteDraw _draw;
    private readonly GameState _gameState;
    private readonly IKeyboard _keyboard;
    private readonly Space _space;
    private readonly uint _colorWhite;
    private readonly uint _colorLightRed;

    private readonly (string Name, string[] Values)[] _settingList =
    [
        new("Ship Style:", ["Solid", "Wireframe", string.Empty, string.Empty, string.Empty]),
        new("Laser Style:", ["Solid", "Wireframe", string.Empty, string.Empty, string.Empty]),
        new("Planet Style:", ["Wireframe", "Solid", "Striped", "Fractal", string.Empty]),
        new("Sun Style:", ["Solid", "Gradient", string.Empty]),
        new("Planet Desc.:", ["BBC", "MSX", string.Empty, string.Empty, string.Empty]),
        new("Instant Dock:", ["Off", "On", string.Empty, string.Empty, string.Empty]),
        new("Back", [string.Empty, string.Empty, string.Empty, string.Empty, string.Empty]),
    ];

    private int _highlightedItem;

    internal SettingsView(
        GameState gameState,
        IEliteDraw draw,
        IKeyboard keyboard,
        Space space,
        IConfigWriter<EliteConfig> configWriter)
    {
        _gameState = gameState;
        _draw = draw;
        _keyboard = keyboard;
        _space = space;
        _configWriter = configWriter;

        _colorWhite = draw.Palette["White"];
        _colorLightRed = draw.Palette["LightRed"];
    }

    public void Draw()
    {
        _draw.DrawViewHeader("GAME SETTINGS");

        for (int i = 0; i < _settingList.Length; i++)
        {
            Vector2 position;

            if (i == _settingList.Length - 1)
            {
                position.Y = ((_settingList.Length + 1) / 2 * 30) + (_draw.Centre.Y / 2) + 32;
                if (i == _highlightedItem)
                {
                    position.X = _draw.Centre.X - 200;
                    _draw.Graphics.DrawRectangleFilled(position, 400, 15, _colorLightRed);
                }

                _draw.Graphics.DrawTextCentre(position.Y, _settingList[i].Name, nameof(FontType.Small), _colorWhite);
                return;
            }

            int v = SettingValue(i);

            position.X = ((i & 1) * 250) + 32 + _draw.Offset;
            position.Y = (i / 2 * 30) + (_draw.Centre.Y / 2);

            if (i == _highlightedItem)
            {
                _draw.Graphics.DrawRectangleFilled(position, 100, 15, _colorLightRed);
            }

            _draw.Graphics.DrawTextLeft(position, _settingList[i].Name, nameof(FontType.Small), _colorWhite);
            position.X += 120;
            _draw.Graphics.DrawTextLeft(position, _settingList[i].Values[v], nameof(FontType.Small), _colorWhite);
        }
    }

    public void HandleInput()
    {
        if (_keyboard.IsPressed(ConsoleKey.S) || _keyboard.IsPressed(ConsoleKey.UpArrow))
        {
            SelectUp();
        }

        if (_keyboard.IsPressed(ConsoleKey.X) || _keyboard.IsPressed(ConsoleKey.DownArrow))
        {
            SelectDown();
        }

        if (_keyboard.IsPressed(ConsoleKey.OemComma) || _keyboard.IsPressed(ConsoleKey.LeftArrow))
        {
            SelectLeft();
        }

        if (_keyboard.IsPressed(ConsoleKey.OemPeriod) || _keyboard.IsPressed(ConsoleKey.RightArrow))
        {
            SelectRight();
        }

        if (_keyboard.IsPressed(ConsoleKey.Enter))
        {
            ToggleSetting();
        }
    }

    public void Reset() => _highlightedItem = 0;

    public void Update()
    {
    }

    // Which of setting i's values is currently selected.
    private int SettingValue(int i) => i switch
    {
        0 => _gameState.Config.Game.ShipWireframe ? 1 : 0,
        1 => _gameState.Config.Game.LaserWireframe ? 1 : 0,
        2 => (int)_gameState.Config.Game.PlanetStyle,
        3 => (int)_gameState.Config.Game.SunStyle,
        4 => _gameState.Config.Game.PlanetDescriptions == PlanetDescriptions.HoopyCasinos ? 1 : 0,
        5 => _gameState.Config.Game.InstantDock ? 1 : 0,
        _ => 0,
    };

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

    private void ToggleSetting()
    {
        if (_highlightedItem == _settingList.Length - 1)
        {
            _gameState.SetView(Screen.Options);
            return;
        }

        switch (_highlightedItem)
        {
            case 0:
                _gameState.Config.Game.ShipWireframe = !_gameState.Config.Game.ShipWireframe;
                break;

            case 1:
                _gameState.Config.Game.LaserWireframe = !_gameState.Config.Game.LaserWireframe;
                break;

            case 2:
                _gameState.Config.Game.PlanetStyle = (PlanetType)((int)(_gameState.Config.Game.PlanetStyle + 1)
                    % Enum.GetValues<PlanetType>().Length);
                _space.RefreshPlanetStyle();
                break;

            case 3:
                _gameState.Config.Game.SunStyle = (SunType)((int)(_gameState.Config.Game.SunStyle + 1) % Enum.GetValues<SunType>().Length);
                _space.RefreshSunStyle();
                break;

            case 4:
                _gameState.Config.Game.PlanetDescriptions = (PlanetDescriptions)((int)(_gameState.Config.Game.PlanetDescriptions + 1) % 2);
                break;

            case 5:
                _gameState.Config.Game.InstantDock = !_gameState.Config.Game.InstantDock;
                break;
        }

        // Every change is live and saved as it's made, so there's no save step.
        _configWriter.WriteConfig(_gameState.Config);
    }
}
