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
    using Elite.Engine.Types;

    internal class GalacticChart : IView
    {
        private readonly IGfx _gfx;
        private readonly IKeyboard _keyboard;
        private readonly List<Vector2> _planetPixels = new();
        private int _crossTimer;
        private bool _isFind;
        private string _findName;

        internal GalacticChart(IGfx gfx, IKeyboard keyboard)
        {
            _gfx = gfx;
            _keyboard = keyboard;
        }

        public void Reset()
        {
            _isFind = false;
            _findName = string.Empty;
            galaxy_seed glx = (galaxy_seed)elite.cmdr.galaxy.Clone();
            _planetPixels.Clear();

            for (int i = 0; i < 256; i++)
            {
                Vector2 pixel = new()
                {
                    X = glx.d * gfx.GFX_SCALE,
                    Y = (glx.b / (2f / gfx.GFX_SCALE)) + (18f * gfx.GFX_SCALE) + 1
                };

                _planetPixels.Add(pixel);

                if ((glx.e | 0x50) < 0x90)
                {
                    _planetPixels.Add(new(pixel.X + 1, pixel.Y));
                }

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
            elite.draw.ClearDisplay();
            elite.draw.DrawViewHeader($"GALACTIC CHART {elite.cmdr.galaxy_number + 1}");

            _gfx.DrawLine(new(0, 36 + 258), new(511, 36 + 258));

            // Fuel radius
            Vector2 centre = new(elite.docked_planet.d * gfx.GFX_SCALE, (elite.docked_planet.b / (2 / gfx.GFX_SCALE)) + (18 * gfx.GFX_SCALE) + 1);
            float radius = elite.cmdr.fuel * 2.5f * gfx.GFX_SCALE;
            float cross_size = 7 * gfx.GFX_SCALE;
            _gfx.DrawCircle(centre, radius, GFX_COL.GFX_COL_GREEN_1);
            _gfx.DrawLine(new(centre.X, centre.Y - cross_size), new(centre.X, centre.Y + cross_size));
            _gfx.DrawLine(new(centre.X - cross_size, centre.Y), new(centre.X + cross_size, centre.Y));

            // Planets
            foreach (Vector2 pixel in _planetPixels)
            {
                _gfx.DrawPixel(pixel, GFX_COL.GFX_COL_WHITE);
            }

            // Moving cross
            centre = new(elite.cross.X, elite.cross.Y);
            _gfx.SetClipRegion(1, 37, 510, 293);
            _gfx.DrawLine(new(centre.X - 8, centre.Y), new(centre.X + 8, centre.Y), GFX_COL.GFX_COL_RED);
            _gfx.DrawLine(new(centre.X, centre.Y - 8), new(centre.X, centre.Y + 8), GFX_COL.GFX_COL_RED);
            _gfx.SetClipRegion(1, 1, 510, 383);

            // Text
            elite.draw.ClearTextArea();
            if (_isFind)
            {
                _gfx.DrawTextLeft(16, 340, "Planet Name?", GFX_COL.GFX_COL_GREEN_1);
                _gfx.DrawTextLeft(16, 356, _findName, GFX_COL.GFX_COL_WHITE);
            }
            else
            {
                if (string.IsNullOrEmpty(elite.planetName))
                {
                    _gfx.DrawTextLeft(16, 340, "Unknown Planet", GFX_COL.GFX_COL_GREEN_1);
                    _gfx.DrawTextLeft(16, 356, _findName, GFX_COL.GFX_COL_WHITE);
                }
                else
                {
                    _gfx.DrawTextLeft(16, 340, elite.planetName, GFX_COL.GFX_COL_GREEN_1);
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
                    if (Planet.find_planet_by_name(_findName))
                    {
                        CrossFromHyperspacePlanet();
                        CalculateDistanceToPlanet();
                    }
                    else
                    {
                        elite.planetName = string.Empty;
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
                elite.cross.X = elite.docked_planet.d * gfx.GFX_SCALE;
                elite.cross.Y = (elite.docked_planet.b / (2 / gfx.GFX_SCALE)) + (18 * gfx.GFX_SCALE) + 1;
                CalculateDistanceToPlanet();
            }
            if (_keyboard.IsKeyPressed(CommandKey.D))
            {
                CalculateDistanceToPlanet();
            }
            if (_keyboard.IsKeyPressed(CommandKey.Up))
            {
                MoveCross(0, -1);
            }
            if (_keyboard.IsKeyPressed(CommandKey.Down))
            {
                MoveCross(0, 1);
            }
            if (_keyboard.IsKeyPressed(CommandKey.Left))
            {
                MoveCross(-1, 0);
            }
            if (_keyboard.IsKeyPressed(CommandKey.Right))
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

            elite.cross.X = Math.Clamp(elite.cross.X + (dx * 2), 1, 510);
            elite.cross.Y = Math.Clamp(elite.cross.Y + (dy * 2), 37, 293);
        }

        private static void CalculateDistanceToPlanet()
        {
            Vector2 location = new()
            {
                X = elite.cross.X / gfx.GFX_SCALE,
                Y = (elite.cross.Y - ((18 * gfx.GFX_SCALE) + 1)) * (2 / gfx.GFX_SCALE),
            };

            elite.hyperspace_planet = Planet.find_planet(location);
            elite.planetName = Planet.name_planet(elite.hyperspace_planet, false);
            elite.distanceToPlanet = Planet.calc_distance_to_planet(elite.docked_planet, elite.hyperspace_planet);
            CrossFromHyperspacePlanet();
        }

        private static void CrossFromHyperspacePlanet()
        {
            elite.cross.X = elite.hyperspace_planet.d * gfx.GFX_SCALE;
            elite.cross.Y = (elite.hyperspace_planet.b / (2 / gfx.GFX_SCALE)) + (18 * gfx.GFX_SCALE) + 1;
        }
    }
}