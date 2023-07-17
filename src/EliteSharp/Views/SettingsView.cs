// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using EliteSharp.Config;
using EliteSharp.Controls;
using EliteSharp.Graphics;
using EliteSharp.Planets;

namespace EliteSharp.Views
{
    internal sealed class SettingsView : IView
    {
        private readonly ConfigFile _configFile;
        private readonly IDraw _draw;
        private readonly GameState _gameState;
        private readonly IKeyboard _keyboard;

        private readonly (string Name, string[] Values)[] _settingList =
        {
            new("Graphics:", new[] { "Solid", "Wireframe", string.Empty, string.Empty, string.Empty }),
            new("Planet Style:", new[] { "Wireframe", "Green", "SNES", "Fractal", string.Empty }),
            new("Planet Desc.:", new[] { "BBC", "MSX", string.Empty, string.Empty, string.Empty }),
            new("Instant Dock:", new[] { "Off", "On", string.Empty, string.Empty, string.Empty }),
            new("Save Settings", new[] { string.Empty, string.Empty, string.Empty, string.Empty, string.Empty }),
        };

        private int _highlightedItem;

        internal SettingsView(GameState gameState, IDraw draw, IKeyboard keyboard, ConfigFile configFile)
        {
            _gameState = gameState;
            _draw = draw;
            _keyboard = keyboard;
            _configFile = configFile;
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
                        _draw.Graphics.DrawRectangleFilled(position, 400, 15, Colour.LightRed);
                    }

                    _draw.Graphics.DrawTextCentre(position.Y, _settingList[i].Name, FontSize.Small, Colour.White);
                    return;
                }

                int v = i switch
                {
                    0 => _gameState.Config.UseWireframe ? 1 : 0,
                    1 => (int)_gameState.Config.PlanetRenderStyle,
                    2 => _gameState.Config.PlanetDescriptions == PlanetDescriptions.HoopyCasinos ? 1 : 0,
                    3 => _gameState.Config.InstantDock ? 1 : 0,
                    _ => 0,
                };

                position.X = ((i & 1) * 250) + 32 + _draw.Offset;
                position.Y = (i / 2 * 30) + (_draw.Centre.Y / 2);

                if (i == _highlightedItem)
                {
                    _draw.Graphics.DrawRectangleFilled(position, 100, 15, Colour.LightRed);
                }

                _draw.Graphics.DrawTextLeft(position, _settingList[i].Name, Colour.White);
                position.X += 120;
                _draw.Graphics.DrawTextLeft(position, _settingList[i].Values[v], Colour.White);
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
                _gameState.SetView(Screen.Options);
                return;
            }

            switch (_highlightedItem)
            {
                case 0:
                    _gameState.Config.UseWireframe = !_gameState.Config.UseWireframe;
                    break;

                case 1:
                    _gameState.Config.PlanetRenderStyle = (PlanetType)((int)(_gameState.Config.PlanetRenderStyle + 1) % 4);
                    break;

                case 2:
                    _gameState.Config.PlanetDescriptions = (PlanetDescriptions)((int)(_gameState.Config.PlanetDescriptions + 1) % 2);
                    break;

                case 3:
                    _gameState.Config.InstantDock = !_gameState.Config.InstantDock;
                    break;

                default:
                    break;
            }
        }
    }
}
