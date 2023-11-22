// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using EliteSharp.Controls;
using EliteSharp.Graphics;
using EliteSharp.Ships;
using EliteSharp.Types;

namespace EliteSharp.Views
{
    internal sealed class ShortRangeChartView : IView
    {
        private readonly IDraw _draw;
        private readonly GameState _gameState;
        private readonly IKeyboard _keyboard;
        private readonly PlanetController _planet;
        private readonly List<(Vector2 Position, string Name)> _planetNames = [];
        private readonly List<(Vector2 Position, float Size)> _planetSizes = [];
        private readonly PlayerShip _ship;
        private int _crossTimer;
        private string _findName = string.Empty;
        private bool _isFind;

        internal ShortRangeChartView(GameState gameState, IDraw draw, IKeyboard keyboard, PlanetController planet, PlayerShip ship)
        {
            _gameState = gameState;
            _draw = draw;
            _keyboard = keyboard;
            _planet = planet;
            _ship = ship;
        }

        public void Draw()
        {
            // Header
            _draw.DrawViewHeader("SHORT RANGE CHART");

            // Fuel radius
            Vector2 centre = _draw.Centre;
            float radius = _ship.Fuel * 10 * _draw.Graphics.Scale;
            float cross_size = 16 * _draw.Graphics.Scale;
            _draw.Graphics.DrawCircle(centre, radius, FastColors.Green);
            _draw.Graphics.DrawLine(new(centre.X, centre.Y - cross_size), new(centre.X, centre.Y + cross_size), FastColors.White);
            _draw.Graphics.DrawLine(new(centre.X - cross_size, centre.Y), new(centre.X + cross_size, centre.Y), FastColors.White);

            // Planets
            foreach ((Vector2 position, string name) in _planetNames)
            {
                _draw.Graphics.DrawTextLeft(position, name, FastColors.White);
            }

            foreach ((Vector2 position, float size) in _planetSizes)
            {
                _draw.Graphics.DrawCircleFilled(position, size, FastColors.Gold);
            }

            // Cross
            centre = new(_gameState.Cross.X, _gameState.Cross.Y);
            _draw.Graphics.DrawLine(new(centre.X - 16, centre.Y), new(centre.X + 16, centre.Y), FastColors.LighterRed);
            _draw.Graphics.DrawLine(new(centre.X, centre.Y - 16), new(centre.X, centre.Y + 16), FastColors.LighterRed);

            // Text
            if (_isFind)
            {
                _draw.Graphics.DrawTextLeft(new(16 + _draw.Offset, _draw.ScannerTop - 55), "Planet Name?", FastColors.Green);
                _draw.Graphics.DrawTextLeft(new(16 + _draw.Offset, _draw.ScannerTop - 40), _findName, FastColors.White);
            }
            else if (string.IsNullOrEmpty(_gameState.PlanetName))
            {
                _draw.Graphics.DrawTextLeft(new(16 + _draw.Offset, _draw.ScannerTop - 55), "Unknown Planet", FastColors.Green);
                _draw.Graphics.DrawTextLeft(new(16 + _draw.Offset, _draw.ScannerTop - 40), _findName, FastColors.White);
            }
            else
            {
                _draw.Graphics.DrawTextLeft(new(16 + _draw.Offset, _draw.ScannerTop - 55), _gameState.PlanetName, FastColors.Green);
                if (_gameState.DistanceToPlanet > 0)
                {
                    _draw.Graphics.DrawTextLeft(
                        new(16 + _draw.Offset, _draw.ScannerTop - 40),
                        $"Distance: {_gameState.DistanceToPlanet:N1} Light Years ",
                        FastColors.White);
                }
            }
        }

        public void HandleInput()
        {
            if (_isFind)
            {
                if (_keyboard.IsKeyPressed(CommandKey.Backspace) &&
                    !string.IsNullOrEmpty(_findName))
                {
                    _findName = _findName[..^1];
                }

                if (_keyboard.IsKeyPressed(CommandKey.Enter))
                {
                    _isFind = false;
                    if (_planet.FindPlanetByName(_findName))
                    {
                        CrossFromHyperspacePlanet();
                        CalculateDistanceToPlanet();
                    }
                    else
                    {
                        _gameState.PlanetName = string.Empty;
                    }
                }

                char letter = (char)_keyboard.GetKeyPressed();
                if (_isFind && _findName.Length <= 16 && letter >= 'A' && letter <= 'Z')
                {
                    _findName += letter;
                }

                return;
            }

            if (_keyboard.IsKeyPressed(CommandKey.Origin))
            {
                _gameState.Cross = _draw.Centre;
                CalculateDistanceToPlanet();
            }

            if (_keyboard.IsKeyPressed(CommandKey.DistanceToPlanet))
            {
                CalculateDistanceToPlanet();
            }

            if (_keyboard.IsKeyPressed(CommandKey.Up, CommandKey.UpArrow))
            {
                MoveCross(0, -1);
            }

            if (_keyboard.IsKeyPressed(CommandKey.Down, CommandKey.DownArrow))
            {
                MoveCross(0, 1);
            }

            if (_keyboard.IsKeyPressed(CommandKey.Left, CommandKey.LeftArrow))
            {
                MoveCross(-1, 0);
            }

            if (_keyboard.IsKeyPressed(CommandKey.Right, CommandKey.RightArrow))
            {
                MoveCross(1, 0);
            }

            if (_keyboard.IsKeyPressed(CommandKey.Find))
            {
                _isFind = true;
                _findName = string.Empty;
                _keyboard.ClearKeyPressed();  // Clear the F so that it doesn't appear in the find word
            }
        }

        public void Reset()
        {
            _isFind = false;
            _findName = string.Empty;
            int[] row_used = new int[64];
            _planetNames.Clear();
            _planetSizes.Clear();

            for (int i = 0; i < 64; i++)
            {
                row_used[i] = 0;
            }

            GalaxySeed glx = new(_gameState.Cmdr.Galaxy);

            for (int i = 0; i < 256; i++)
            {
                float dx = MathF.Abs(glx.D - _gameState.DockedPlanet.D);
                float dy = MathF.Abs(glx.B - _gameState.DockedPlanet.B);

                if ((dx >= 20) || (dy >= 38))
                {
                    _planet.WaggleGalaxy(glx);
                    _planet.WaggleGalaxy(glx);
                    _planet.WaggleGalaxy(glx);
                    _planet.WaggleGalaxy(glx);

                    continue;
                }

                float px = glx.D - _gameState.DockedPlanet.D;

                // Convert to screen co-ords
                px = (px * 4 * _draw.Graphics.Scale) + _draw.Centre.X;

                float py = glx.B - _gameState.DockedPlanet.B;

                // Convert to screen co-ords
                py = (py * 2 * _draw.Graphics.Scale) + _draw.Centre.Y;

                int row = (int)(py / (8 * _draw.Graphics.Scale));

                if (row_used[row] == 1)
                {
                    row++;
                }

                if (row_used[row] == 1)
                {
                    row -= 2;
                }

                if (row <= 3)
                {
                    _planet.WaggleGalaxy(glx);
                    _planet.WaggleGalaxy(glx);
                    _planet.WaggleGalaxy(glx);
                    _planet.WaggleGalaxy(glx);

                    continue;
                }

                if (row_used[row] == 0)
                {
                    row_used[row] = 1;
                    _planetNames.Add((
                        new(px + (4 * _draw.Graphics.Scale), ((row * 8) - 5) * _draw.Graphics.Scale),
                        _planet.NamePlanet(glx)
                            .CapitaliseFirstLetter()));
                }

                // The next bit calculates the size of the circle used to represent
                // a planet.  The carry_flag is left over from the name generation.
                // Yes this was how it was done... don't ask :-(
                float blob_size = (glx.F & 1) + 2 + _gameState.CarryFlag;
                blob_size *= _draw.Graphics.Scale;
                _planetSizes.Add((new(px, py), blob_size));

                _planet.WaggleGalaxy(glx);
                _planet.WaggleGalaxy(glx);
                _planet.WaggleGalaxy(glx);
                _planet.WaggleGalaxy(glx);
            }

            _crossTimer = 0;
            CrossFromHyperspacePlanet();
            CalculateDistanceToPlanet();
        }

        public void UpdateUniverse()
        {
            if (_crossTimer > 0)
            {
                _crossTimer--;
                if (_crossTimer == 0)
                {
                    CalculateDistanceToPlanet();
                }
            }
        }

        private void CalculateDistanceToPlanet()
        {
            Vector2 location = new()
            {
                X = ((_gameState.Cross.X - _draw.Centre.X) / (4 * _draw.Graphics.Scale)) + _gameState.DockedPlanet.D,
                Y = ((_gameState.Cross.Y - _draw.Centre.Y) / (2 * _draw.Graphics.Scale)) + _gameState.DockedPlanet.B,
            };

            _gameState.HyperspacePlanet = _planet.FindPlanet(_gameState.Cmdr.Galaxy, location);
            _gameState.PlanetName = _planet.NamePlanet(_gameState.HyperspacePlanet);
            _gameState.DistanceToPlanet = PlanetController.CalculateDistanceToPlanet(_gameState.DockedPlanet, _gameState.HyperspacePlanet);
            CrossFromHyperspacePlanet();
        }

        private void CrossFromHyperspacePlanet() => _gameState.Cross = new(
            ((_gameState.HyperspacePlanet.D - _gameState.DockedPlanet.D) * 4 * _draw.Graphics.Scale) + _draw.Centre.X,
            ((_gameState.HyperspacePlanet.B - _gameState.DockedPlanet.B) * 2 * _draw.Graphics.Scale) + _draw.Centre.Y);

        /// <summary>
        /// Move the planet chart cross hairs to specified position.
        /// </summary>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        private void MoveCross(int dx, int dy)
        {
            _crossTimer = 5;
            _gameState.Cross = new(Math.Clamp(_gameState.Cross.X + (dx * 4), 1, 510), Math.Clamp(_gameState.Cross.Y + (dy * 4), 37, 339));
        }
    }
}
