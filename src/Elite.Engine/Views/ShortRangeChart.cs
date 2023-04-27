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

    internal class ShortRangeChart : IView
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
        private string _findName;

        internal ShortRangeChart(GameState gameState, IGfx gfx, Draw draw, IKeyboard keyboard, Planet planet, PlayerShip ship)
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

            galaxy_seed glx = (galaxy_seed)_gameState.cmdr.galaxy.Clone();

            for (int i = 0; i < 256; i++)
            {
                float dx = MathF.Abs(glx.d - _gameState.docked_planet.d);
                float dy = MathF.Abs(glx.b - _gameState.docked_planet.b);

                if ((dx >= 20) || (dy >= 38))
                {
                    Planet.waggle_galaxy(ref glx);
                    Planet.waggle_galaxy(ref glx);
                    Planet.waggle_galaxy(ref glx);
                    Planet.waggle_galaxy(ref glx);

                    continue;
                }

                float px = glx.d - _gameState.docked_planet.d;
                px = (px * 4 * gfx.GFX_SCALE) + gfx.GFX_X_CENTRE;  /* Convert to screen co-ords */

                float py = glx.b - _gameState.docked_planet.b;
                py = (py * 2 * gfx.GFX_SCALE) + gfx.GFX_Y_CENTRE; /* Convert to screen co-ords */

                int row = (int)(py / (8 * gfx.GFX_SCALE));

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
                    Planet.waggle_galaxy(ref glx);
                    Planet.waggle_galaxy(ref glx);
                    Planet.waggle_galaxy(ref glx);
                    Planet.waggle_galaxy(ref glx);

                    continue;
                }

                if (row_used[row] == 0)
                {
                    row_used[row] = 1;
                    _planetNames.Add((new(px + (4 * gfx.GFX_SCALE), ((row * 8) - 5) * gfx.GFX_SCALE), Planet.name_planet(glx, true)));
                }

                /* The next bit calculates the size of the circle used to represent */
                /* a planet.  The carry_flag is left over from the name generation. */
                /* Yes this was how it was done... don't ask :-( */
                float blob_size = (glx.f & 1) + 2 + elite.carry_flag;
                blob_size *= gfx.GFX_SCALE;
                _planetSizes.Add((new(px, py), blob_size));


                Planet.waggle_galaxy(ref glx);
                Planet.waggle_galaxy(ref glx);
                Planet.waggle_galaxy(ref glx);
                Planet.waggle_galaxy(ref glx);
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
            Vector2 centre = new(gfx.GFX_X_CENTRE, gfx.GFX_Y_CENTRE);
            float radius = _ship.fuel * 10 * gfx.GFX_SCALE;
            float cross_size = 16 * gfx.GFX_SCALE;
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
            centre = new(elite.cross.X, elite.cross.Y);
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
                if (string.IsNullOrEmpty(_gameState.planetName))
                {
                    _gfx.DrawTextLeft(16, 340, "Unknown Planet", GFX_COL.GFX_COL_GREEN_1);
                    _gfx.DrawTextLeft(16, 356, _findName, GFX_COL.GFX_COL_WHITE);
                }
                else
                {
                    _gfx.DrawTextLeft(16, 340, _gameState.planetName, GFX_COL.GFX_COL_GREEN_1);
                    if (elite.distanceToPlanet > 0)
                    {
                        _gfx.DrawTextLeft(16, 356, $"Distance: {elite.distanceToPlanet:N1} Light Years ", GFX_COL.GFX_COL_WHITE);
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
                    if (_planet.find_planet_by_name(_findName))
                    {
                        CrossFromHyperspacePlanet();
                        CalculateDistanceToPlanet();
                    }
                    else
                    {
                        _gameState.planetName = string.Empty;
                    }
                }

                int letter = _keyboard.GetKeyPressed();
                if (_isFind && _findName.Length <= 16 && letter >= 'A' && letter <= 'Z')
                {
                    _findName += (char)letter;
                }

                return;
            }

            if (_keyboard.IsKeyPressed(CommandKey.Origin))
            {
                elite.cross = new(gfx.GFX_X_CENTRE, gfx.GFX_Y_CENTRE);
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

            elite.cross.X = Math.Clamp(elite.cross.X + (dx * 4), 1, 510);
            elite.cross.Y = Math.Clamp(elite.cross.Y + (dy * 4), 37, 339);
        }

        private void CalculateDistanceToPlanet()
        {
            Vector2 location = new()
            {
                X = ((elite.cross.X - gfx.GFX_X_CENTRE) / (4 * gfx.GFX_SCALE)) + _gameState.docked_planet.d,
                Y = ((elite.cross.Y - gfx.GFX_Y_CENTRE) / (2 * gfx.GFX_SCALE)) + _gameState.docked_planet.b,
            };

            _gameState.hyperspace_planet = Planet.find_planet(_gameState.cmdr.galaxy, location);
            _gameState.planetName = Planet.name_planet(_gameState.hyperspace_planet, false);
            elite.distanceToPlanet = Planet.calc_distance_to_planet(_gameState.docked_planet, _gameState.hyperspace_planet);
            CrossFromHyperspacePlanet();
        }

        private void CrossFromHyperspacePlanet()
        {
            elite.cross.X = ((_gameState.hyperspace_planet.d - _gameState.docked_planet.d) * 4 * gfx.GFX_SCALE) + gfx.GFX_X_CENTRE;
            elite.cross.Y = ((_gameState.hyperspace_planet.b - _gameState.docked_planet.b) * 2 * gfx.GFX_SCALE) + gfx.GFX_Y_CENTRE;
        }
    }
}