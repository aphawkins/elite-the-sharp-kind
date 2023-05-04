/*
 * Elite - The New Kind.
 *
 * Reverse engineered from the BBC disk version of Elite.
 * Additional material by C.J.Pinder.
 *
 * The original Elite code is (C) I.Bell & D.Braben 1984.
 * This version re-engineered in C by C.J.Pinder 1999-2001.
 *
 * email: <christian@newkind.co.uk>
 *
 *
 */

namespace Elite.Engine.Views
{
    using System.Numerics;
    using Elite.Engine.Enums;
    using Elite.Engine.Ships;
    using Elite.Engine.Types;

    internal class GalacticChart : IView
    {
        private readonly GameState _gameState;
        private readonly IGfx _gfx;
        private readonly Draw _draw;
        private readonly IKeyboard _keyboard;
        private readonly Planet _planet;
        private readonly PlayerShip _ship;
        private readonly List<Vector2> _planetPixels = new();
        private int _crossTimer;
        private bool _isFind;
        private string _findName = string.Empty;

        internal GalacticChart(GameState gameState, IGfx gfx, Draw draw, IKeyboard keyboard, Planet planet, PlayerShip ship)
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
            GalaxySeed glx = (GalaxySeed)_gameState.cmdr.Galaxy.Clone();
            _planetPixels.Clear();

            for (int i = 0; i < 256; i++)
            {
                Vector2 pixel = new()
                {
                    X = glx.D * Graphics.GFX_SCALE,
                    Y = (glx.B / (2f / Graphics.GFX_SCALE)) + (18f * Graphics.GFX_SCALE) + 1
                };

                _planetPixels.Add(pixel);

                if ((glx.E | 0x50) < 0x90)
                {
                    _planetPixels.Add(new(pixel.X + 1, pixel.Y));
                }

                Planet.WaggleGalaxy(ref glx);
                Planet.WaggleGalaxy(ref glx);
                Planet.WaggleGalaxy(ref glx);
                Planet.WaggleGalaxy(ref glx);
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
            _draw.DrawViewHeader($"GALACTIC CHART {_gameState.cmdr.GalaxyNumber + 1}");

            _gfx.DrawLine(new(0, 36 + 258), new(511, 36 + 258));

            // Fuel radius
            Vector2 centre = new(_gameState.docked_planet.D * Graphics.GFX_SCALE, (_gameState.docked_planet.B / (2 / Graphics.GFX_SCALE)) + (18 * Graphics.GFX_SCALE) + 1);
            float radius = _ship.fuel * 2.5f * Graphics.GFX_SCALE;
            float cross_size = 7 * Graphics.GFX_SCALE;
            _gfx.DrawCircle(centre, radius, GFX_COL.GFX_COL_GREEN_1);
            _gfx.DrawLine(new(centre.X, centre.Y - cross_size), new(centre.X, centre.Y + cross_size));
            _gfx.DrawLine(new(centre.X - cross_size, centre.Y), new(centre.X + cross_size, centre.Y));

            // Planets
            foreach (Vector2 pixel in _planetPixels)
            {
                _gfx.DrawPixel(pixel, GFX_COL.GFX_COL_WHITE);
            }

            // Moving cross
            centre = new(EliteMain.cross.X, EliteMain.cross.Y);
            _gfx.SetClipRegion(1, 37, 510, 293);
            _gfx.DrawLine(new(centre.X - 8, centre.Y), new(centre.X + 8, centre.Y), GFX_COL.GFX_COL_RED);
            _gfx.DrawLine(new(centre.X, centre.Y - 8), new(centre.X, centre.Y + 8), GFX_COL.GFX_COL_RED);
            _gfx.SetClipRegion(1, 1, 510, 383);

            // Text
            if (_isFind)
            {
                _gfx.DrawTextLeft(16, 340, "Planet Name?", GFX_COL.GFX_COL_GREEN_1);
                _gfx.DrawTextLeft(16, 356, _findName, GFX_COL.GFX_COL_WHITE);
            }
            else
            {
                if (string.IsNullOrEmpty(_gameState.planetName))
                {
                    _gfx.DrawTextLeft(16, 340, "Unknown Planet", GFX_COL.GFX_COL_GREEN_1);
                    _gfx.DrawTextLeft(16, 356, _findName, GFX_COL.GFX_COL_WHITE);
                }
                else
                {
                    _gfx.DrawTextLeft(16, 340, _gameState.planetName, GFX_COL.GFX_COL_GREEN_1);
                    if (EliteMain.distanceToPlanet > 0)
                    {
                        _gfx.DrawTextLeft(16, 356, $"Distance: {EliteMain.distanceToPlanet:N1} Light Years ", GFX_COL.GFX_COL_WHITE);
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
                        _gameState.planetName = string.Empty;
                    }
                }

                CommandKey letter = _keyboard.GetKeyPressed();
                if (_isFind && _findName.Length <= 16 && (char)letter >= 'A' && (char)letter <= 'Z')
                {
                    _findName += (char)letter;
                }

                return;
            }

            if (_keyboard.IsKeyPressed(CommandKey.Origin))
            {
                EliteMain.cross.X = _gameState.docked_planet.D * Graphics.GFX_SCALE;
                EliteMain.cross.Y = (_gameState.docked_planet.B / (2 / Graphics.GFX_SCALE)) + (18 * Graphics.GFX_SCALE) + 1;
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

            EliteMain.cross.X = Math.Clamp(EliteMain.cross.X + (dx * 2), 1, 510);
            EliteMain.cross.Y = Math.Clamp(EliteMain.cross.Y + (dy * 2), 37, 293);
        }

        private void CalculateDistanceToPlanet()
        {
            Vector2 location = new()
            {
                X = EliteMain.cross.X / Graphics.GFX_SCALE,
                Y = (EliteMain.cross.Y - ((18 * Graphics.GFX_SCALE) + 1)) * (2 / Graphics.GFX_SCALE),
            };

            _gameState.hyperspace_planet = Planet.FindPlanet(_gameState.cmdr.Galaxy, location);
            _gameState.planetName = Planet.NamePlanet(_gameState.hyperspace_planet, false);
            EliteMain.distanceToPlanet = Planet.CalculateDistanceToPlanet(_gameState.docked_planet, _gameState.hyperspace_planet);
            CrossFromHyperspacePlanet();
        }

        private void CrossFromHyperspacePlanet()
        {
            EliteMain.cross.X = _gameState.hyperspace_planet.D * Graphics.GFX_SCALE;
            EliteMain.cross.Y = (_gameState.hyperspace_planet.B / (2 / Graphics.GFX_SCALE)) + (18 * Graphics.GFX_SCALE) + 1;
        }
    }
}