// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using Elite.Engine.Enums;
using Elite.Engine.Ships;
using Elite.Engine.Types;

namespace Elite.Engine.Views
{
    internal sealed class ShortRangeChartView : IView
    {
        private readonly GameState _gameState;
        private readonly IGfx _gfx;
        private readonly Draw _draw;
        private readonly IKeyboard _keyboard;
        private readonly Planet _planet;
        private readonly PlayerShip _ship;
        private readonly List<(Vector2 position, string name)> _planetNames = new();
        private readonly List<(Vector2 position, float size)> _planetSizes = new();
        private int _crossTimer;
        private bool _isFind;
        private string _findName = string.Empty;

        internal ShortRangeChartView(GameState gameState, IGfx gfx, Draw draw, IKeyboard keyboard, Planet planet, PlayerShip ship)
        {
            _gameState = gameState;
            _gfx = gfx;
            _draw = draw;
            _keyboard = keyboard;
            _planet = planet;
            _ship = ship;
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

            GalaxySeed glx = (GalaxySeed)_gameState.Cmdr.Galaxy.Clone();

            for (int i = 0; i < 256; i++)
            {
                float dx = MathF.Abs(glx.D - _gameState.DockedPlanet.D);
                float dy = MathF.Abs(glx.B - _gameState.DockedPlanet.B);

                if ((dx >= 20) || (dy >= 38))
                {
                    _planet.WaggleGalaxy(ref glx);
                    _planet.WaggleGalaxy(ref glx);
                    _planet.WaggleGalaxy(ref glx);
                    _planet.WaggleGalaxy(ref glx);

                    continue;
                }

                float px = glx.D - _gameState.DockedPlanet.D;
                // Convert to screen co-ords
                px = (px * 4 * Graphics.GFX_SCALE) + Graphics.GFX_X_CENTRE;

                float py = glx.B - _gameState.DockedPlanet.B;
                // Convert to screen co-ords
                py = (py * 2 * Graphics.GFX_SCALE) + Graphics.GFX_Y_CENTRE; 

                int row = (int)(py / (8 * Graphics.GFX_SCALE));

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
                    _planet.WaggleGalaxy(ref glx);
                    _planet.WaggleGalaxy(ref glx);
                    _planet.WaggleGalaxy(ref glx);
                    _planet.WaggleGalaxy(ref glx);

                    continue;
                }

                if (row_used[row] == 0)
                {
                    row_used[row] = 1;
                    _planetNames.Add((new(px + (4 * Graphics.GFX_SCALE), ((row * 8) - 5) * Graphics.GFX_SCALE), _planet.NamePlanet(glx, true)));
                }

                // The next bit calculates the size of the circle used to represent
                // a planet.  The carry_flag is left over from the name generation.
                // Yes this was how it was done... don't ask :-(
                float blob_size = (glx.F & 1) + 2 + _gameState.CarryFlag;
                blob_size *= Graphics.GFX_SCALE;
                _planetSizes.Add((new(px, py), blob_size));

                _planet.WaggleGalaxy(ref glx);
                _planet.WaggleGalaxy(ref glx);
                _planet.WaggleGalaxy(ref glx);
                _planet.WaggleGalaxy(ref glx);
            }

            _crossTimer = 0;
            CrossFromHyperspacePlanet();
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

        public void Draw()
        {
            // Header
            _draw.ClearDisplay();
            _draw.DrawViewHeader("SHORT RANGE CHART");

            // Fuel radius
            Vector2 centre = new(Graphics.GFX_X_CENTRE, Graphics.GFX_Y_CENTRE);
            float radius = _ship.Fuel * 10 * Graphics.GFX_SCALE;
            float cross_size = 16 * Graphics.GFX_SCALE;
            _gfx.DrawCircle(centre, radius, GFX_COL.GFX_COL_GREEN_1);
            _gfx.DrawLine(new(centre.X, centre.Y - cross_size), new(centre.X, centre.Y + cross_size));
            _gfx.DrawLine(new(centre.X - cross_size, centre.Y), new(centre.X + cross_size, centre.Y));

            // Planets
            foreach ((Vector2 position, string name) in _planetNames)
            {
                _gfx.DrawTextLeft(position.X, position.Y, name, GFX_COL.GFX_COL_WHITE);
            }
            foreach ((Vector2 position, float size) in _planetSizes)
            {
                _gfx.DrawCircleFilled(position, size, GFX_COL.GFX_COL_GOLD);
            }

            // Moving cross
            centre = new(_gameState.Cross.X, _gameState.Cross.Y);
            _gfx.SetClipRegion(1, 37, 510, 339);
            _gfx.DrawLine(new(centre.X - 16f, centre.Y), new(centre.X + 16f, centre.Y), GFX_COL.GFX_COL_RED);
            _gfx.DrawLine(new(centre.X, centre.Y - 16), new(centre.X, centre.Y + 16), GFX_COL.GFX_COL_RED);
            _gfx.SetClipRegion(1, 1, 510, 383);

            // Text
            if (_isFind)
            {
                _gfx.DrawTextLeft(16, 340, "Planet Name?", GFX_COL.GFX_COL_GREEN_1);
                _gfx.DrawTextLeft(16, 356, _findName, GFX_COL.GFX_COL_WHITE);
            }
            else
            {
                if (string.IsNullOrEmpty(_gameState.PlanetName))
                {
                    _gfx.DrawTextLeft(16, 340, "Unknown Planet", GFX_COL.GFX_COL_GREEN_1);
                    _gfx.DrawTextLeft(16, 356, _findName, GFX_COL.GFX_COL_WHITE);
                }
                else
                {
                    _gfx.DrawTextLeft(16, 340, _gameState.PlanetName, GFX_COL.GFX_COL_GREEN_1);
                    if (_gameState.DistanceToPlanet > 0)
                    {
                        _gfx.DrawTextLeft(16, 356, $"Distance: {_gameState.DistanceToPlanet:N1} Light Years ", GFX_COL.GFX_COL_WHITE);
                    }
                }
            }
        }

        public void HandleInput()
        {
            if (_isFind)
            {
                if (_keyboard.IsKeyPressed(CommandKey.Backspace))
                {
                    if (_isFind && !string.IsNullOrEmpty(_findName))
                    {
                        _findName = _findName[..^1];
                    }
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
                _gameState.Cross = new(Graphics.GFX_X_CENTRE, Graphics.GFX_Y_CENTRE);
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

        private void CalculateDistanceToPlanet()
        {
            Vector2 location = new()
            {
                X = ((_gameState.Cross.X - Graphics.GFX_X_CENTRE) / (4 * Graphics.GFX_SCALE)) + _gameState.DockedPlanet.D,
                Y = ((_gameState.Cross.Y - Graphics.GFX_Y_CENTRE) / (2 * Graphics.GFX_SCALE)) + _gameState.DockedPlanet.B,
            };

            _gameState.HyperspacePlanet = _planet.FindPlanet(_gameState.Cmdr.Galaxy, location);
            _gameState.PlanetName = _planet.NamePlanet(_gameState.HyperspacePlanet, false);
            _gameState.DistanceToPlanet = Planet.CalculateDistanceToPlanet(_gameState.DockedPlanet, _gameState.HyperspacePlanet);
            CrossFromHyperspacePlanet();
        }

        private void CrossFromHyperspacePlanet() => _gameState.Cross = new(((_gameState.HyperspacePlanet.D - _gameState.DockedPlanet.D) * 4 * Graphics.GFX_SCALE) + Graphics.GFX_X_CENTRE,
                ((_gameState.HyperspacePlanet.B - _gameState.DockedPlanet.B) * 2 * Graphics.GFX_SCALE) + Graphics.GFX_Y_CENTRE);
    }
}
