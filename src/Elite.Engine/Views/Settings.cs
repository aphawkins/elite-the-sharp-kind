// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using Elite.Engine.Config;
using Elite.Engine.Enums;

namespace Elite.Engine.Views
{
    internal sealed class SettingsView : IView
    {
        private readonly ConfigFile _configFile;
        private readonly Draw _draw;
        private readonly GameState _gameState;
        private readonly IGfx _gfx;
        private readonly IKeyboard _keyboard;

        private readonly (string Name, string[] Values)[] _settingList =
        {
            new("Graphics:", new [] {"Solid", "Wireframe", "", "", ""}),
            new("Anti Alias:", new [] {"Off", "On", "", "", ""}),
            new("Planet Style:", new [] {"Wireframe", "Green", "SNES", "Fractal", ""}),
            new("Planet Desc.:", new [] {"BBC", "MSX", "", "", ""}),
            new("Instant Dock:", new [] {"Off", "On", "", "", ""}),
            new("Save Settings", new [] {"", "", "", "", ""})
        };

        private int _highlightedItem;

        internal SettingsView(GameState gameState, IGfx gfx, Draw draw, IKeyboard keyboard, ConfigFile configFile)
        {
            _gameState = gameState;
            _gfx = gfx;
            _draw = draw;
            _keyboard = keyboard;
            _configFile = configFile;
        }

        public void Draw()
        {
            _draw.ClearDisplay();
            _draw.DrawViewHeader("GAME SETTINGS");

            for (int i = 0; i < _settingList.Length; i++)
            {
                float x;
                int y;
                if (i == _settingList.Length - 1)
                {
                    y = ((_settingList.Length + 1) / 2 * 30) + 96 + 32;
                    if (i == _highlightedItem)
                    {
                        x = Graphics.GFX_X_CENTRE - 200;
                        _gfx.DrawRectangleFilled(x, y - 7, 400, 15, GFX_COL.GFX_COL_DARK_RED);
                    }

                    _gfx.DrawTextCentre(y, _settingList[i].Name, 120, GFX_COL.GFX_COL_WHITE);
                    return;
                }

                int v = i switch
                {
                    0 => _gameState.Config.UseWireframe ? 1 : 0,
                    1 => _gameState.Config.AntiAliasWireframe ? 1 : 0,
                    2 => (int)_gameState.Config.PlanetRenderStyle,
                    3 => _gameState.Config.PlanetDescriptions == PlanetDescriptions.HoopyCasinos ? 1 : 0,
                    4 => _gameState.Config.InstantDock ? 1 : 0,
                    _ => 0,
                };
                x = ((i & 1) * 250) + 32;
                y = (i / 2 * 30) + 96;

                if (i == _highlightedItem)
                {
                    _gfx.DrawRectangleFilled(x, y, 100, 15, GFX_COL.GFX_COL_DARK_RED);
                }
                _gfx.DrawTextLeft(x, y, _settingList[i].Name, GFX_COL.GFX_COL_WHITE);
                _gfx.DrawTextLeft(x + 120, y, _settingList[i].Values[v], GFX_COL.GFX_COL_WHITE);
            }
        }

        public void HandleInput()
        {
            if (_keyboard.IsKeyPressed(CommandKey.Up, CommandKey.UpArrow))
            {
                SelectUp();
            }
            if (_keyboard.IsKeyPressed(CommandKey.Down, CommandKey.DownArrow))
            {
                SelectDown();
            }
            if (_keyboard.IsKeyPressed(CommandKey.Left, CommandKey.LeftArrow))
            {
                SelectLeft();
            }
            if (_keyboard.IsKeyPressed(CommandKey.Right, CommandKey.RightArrow))
            {
                SelectRight();
            }
            if (_keyboard.IsKeyPressed(CommandKey.Enter))
            {
                ToggleSetting();
            }
        }

        public void Reset() => _highlightedItem = 0;

        public void UpdateUniverse()
        {
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

        private void ToggleSetting()
        {
            if (_highlightedItem == _settingList.Length - 1)
            {
                _configFile.WriteConfigAsync(_gameState.Config).Wait();
                _gameState.SetView(SCR.SCR_OPTIONS);
                return;
            }

            switch (_highlightedItem)
            {
                case 0:
                    _gameState.Config.UseWireframe = !_gameState.Config.UseWireframe;
                    break;

                case 1:
                    _gameState.Config.AntiAliasWireframe = !_gameState.Config.AntiAliasWireframe;
                    break;

                case 2:
                    _gameState.Config.PlanetRenderStyle = (PlanetRenderStyle)((int)(_gameState.Config.PlanetRenderStyle + 1) % 4);
                    break;

                case 3:
                    _gameState.Config.PlanetDescriptions = (PlanetDescriptions)((int)(_gameState.Config.PlanetDescriptions + 1) % 2);
                    break;

                case 4:
                    _gameState.Config.InstantDock = !_gameState.Config.InstantDock;
                    break;

                default:
                    break;
            }
        }
    }
}
