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

        internal GalacticChart(IGfx gfx, IKeyboard keyboard)
        {
            _gfx = gfx;
            _keyboard = keyboard;
        }

        public void Reset()
        {
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
            _gfx.DrawTextCentre(20, $"GALACTIC CHART {elite.cmdr.galaxy_number + 1}", 140, GFX_COL.GFX_COL_GOLD);
            _gfx.DrawLine(new(0, 36), new(511, 36));
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
            _gfx.DrawTextLeft(16, 340, $"{elite.planetName:-18s}", GFX_COL.GFX_COL_WHITE);
            string str = elite.distanceToPlanet > 0
                ? $"Distance: {elite.distanceToPlanet:N1} Light Years "
                : "                                                     ";
            _gfx.DrawTextLeft(16, 356, str, GFX_COL.GFX_COL_WHITE);
        }

        public void HandleInput()
        {
            if (_keyboard.IsKeyPressed(CommandKey.Origin))
            {
                elite.cross.X = elite.docked_planet.d * gfx.GFX_SCALE;
                elite.cross.Y = (elite.docked_planet.b / (2 / gfx.GFX_SCALE)) + (18 * gfx.GFX_SCALE) + 1;
                CalculateDistanceToPlanet();
            }
            else if (_keyboard.IsKeyPressed(CommandKey.D))
            {
                CalculateDistanceToPlanet();
            }
            else if (_keyboard.IsKeyPressed(CommandKey.Up))
            {
                move_cross(0, -1);
            }
            else if (_keyboard.IsKeyPressed(CommandKey.Down))
            {
                move_cross(0, 1);
            }
            else if (_keyboard.IsKeyPressed(CommandKey.Left))
            {
                move_cross(-1, 0);
            }
            else if (_keyboard.IsKeyPressed(CommandKey.Right))
            {
                move_cross(1, 0);
            }
        }

        /// <summary>
        /// Move the planet chart cross hairs to specified position.
        /// </summary>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        private void move_cross(int dx, int dy)
        {
            _crossTimer = 5;

            elite.cross.X += dx * 2;
            elite.cross.Y += dy * 2;

            if (elite.cross.X < 1)
            {
                elite.cross.X = 1;
            }

            if (elite.cross.X > 510)
            {
                elite.cross.X = 510;
            }

            if (elite.cross.Y < 37)
            {
                elite.cross.Y = 37;
            }

            if (elite.cross.Y > 293)
            {
                elite.cross.Y = 293;
            }
        }

        private void CalculateDistanceToPlanet()
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

        private void CrossFromHyperspacePlanet()
        {
            elite.cross.X = elite.hyperspace_planet.d * gfx.GFX_SCALE;
            elite.cross.Y = (elite.hyperspace_planet.b / (2 / gfx.GFX_SCALE)) + (18 * gfx.GFX_SCALE) + 1;
        }
    }
}