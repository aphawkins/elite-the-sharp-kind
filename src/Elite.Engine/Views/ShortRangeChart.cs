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
    using static Elite.Engine.elite;

    internal class ShortRangeChart : IView
    {
        private readonly IGfx _gfx;
        private readonly IKeyboard _keyboard;
        private readonly List<(Vector2 position, string name)> _planetNames = new();
        private readonly List<(Vector2 position, float size)> _planetSizes = new();
        private int _crossTimer;

        internal ShortRangeChart(IGfx gfx, IKeyboard keyboard)
        {
            _gfx = gfx;
            _keyboard = keyboard;
        }

        public void Reset()
        {
            int[] row_used = new int[64];
            _planetNames.Clear();
            _planetSizes.Clear();

            for (int i = 0; i < 64; i++)
            {
                row_used[i] = 0;
            }

            galaxy_seed glx = (galaxy_seed)elite.cmdr.galaxy.Clone();

            for (int i = 0; i < 256; i++)
            {
                float dx = MathF.Abs(glx.d - elite.docked_planet.d);
                float dy = MathF.Abs(glx.b - elite.docked_planet.b);

                if ((dx >= 20) || (dy >= 38))
                {
                    Planet.waggle_galaxy(ref glx);
                    Planet.waggle_galaxy(ref glx);
                    Planet.waggle_galaxy(ref glx);
                    Planet.waggle_galaxy(ref glx);

                    continue;
                }

                float px = glx.d - elite.docked_planet.d;
                px = (px * 4 * gfx.GFX_SCALE) + gfx.GFX_X_CENTRE;  /* Convert to screen co-ords */

                float py = glx.b - elite.docked_planet.b;
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
            elite.draw.ClearDisplay();
            _gfx.DrawTextCentre(20, "SHORT RANGE CHART", 140, GFX_COL.GFX_COL_GOLD);
            _gfx.DrawLine(new(0, 36), new(511, 36));

            // Fuel radius
            Vector2 centre = new(gfx.GFX_X_CENTRE, gfx.GFX_Y_CENTRE);
            float radius = elite.cmdr.fuel * 10 * gfx.GFX_SCALE;
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
                elite.cross = new(gfx.GFX_X_CENTRE, gfx.GFX_Y_CENTRE);
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

            elite.cross.X += dx * 4;
            elite.cross.Y += dy * 4;
        }

        private void CalculateDistanceToPlanet()
        {
            Vector2 location = new()
            {
                X = ((elite.cross.X - gfx.GFX_X_CENTRE) / (4f * gfx.GFX_SCALE)) + elite.docked_planet.d,
                Y = ((elite.cross.Y - gfx.GFX_Y_CENTRE) / (2f * gfx.GFX_SCALE)) + elite.docked_planet.b,
            };

            elite.hyperspace_planet = Planet.find_planet(location);
            elite.planetName = Planet.name_planet(elite.hyperspace_planet, false);
            elite.distanceToPlanet = Planet.calc_distance_to_planet(elite.docked_planet, elite.hyperspace_planet);
            CrossFromHyperspacePlanet();
        }

        private void CrossFromHyperspacePlanet()
        {
            elite.cross.X = ((elite.hyperspace_planet.d - elite.docked_planet.d) * 4 * gfx.GFX_SCALE) + gfx.GFX_X_CENTRE;
            elite.cross.Y = ((elite.hyperspace_planet.b - elite.docked_planet.b) * 2 * gfx.GFX_SCALE) + gfx.GFX_Y_CENTRE;
        }
    }
}