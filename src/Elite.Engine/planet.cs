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
 */

using System.Numerics;
using System.Text;
using Elite.Engine.Types;

/*
 *
 * Handle the generation of planet info...
 */

namespace Elite.Engine
{
    internal class Planet
    {
        private readonly GameState _gameState;
        internal static string digrams = "ABOUSEITILETSTONLONUTHNOALLEXEGEZACEBISOUSESARMAINDIREA?ERATENBERALAVETIEDORQUANTEISRION";
        private static readonly string[] inhabitant_desc1 = new string[] { "Large ", "Fierce ", "Small " };
        private static readonly string[] inhabitant_desc2 = new string[] { "Green ", "Red ", "Yellow ", "Blue ", "Black ", "Harmless " };
        private static readonly string[] inhabitant_desc3 = new string[] { "Slimy ", "Bug-Eyed ", "Horned ", "Bony ", "Fat ", "Furry " };
        private static readonly string[] inhabitant_desc4 = new string[] { "Rodent", "Frog", "Lizard", "Lobster", "Bird", "Humanoid", "Feline", "Insect" };

        internal Planet(GameState gameState) => _gameState = gameState;

        internal void WaggleGalaxy(ref GalaxySeed glx_ptr)
        {
            int x = glx_ptr.A + glx_ptr.C;
            int y = glx_ptr.B + glx_ptr.D;

            if (x > 0xFF)
            {
                y++;
            }

            x &= 0xFF;
            y &= 0xFF;

            glx_ptr.A = glx_ptr.C;
            glx_ptr.B = glx_ptr.D;
            glx_ptr.C = glx_ptr.E;
            glx_ptr.D = glx_ptr.F;

            x += glx_ptr.C;
            y += glx_ptr.D;

            if (x > 0xFF)
            {
                y++;
            }

            _gameState.CarryFlag = y > 0xFF ? 1 : 0;

            x &= 0xFF;
            y &= 0xFF;

            glx_ptr.E = x;
            glx_ptr.F = y;
        }

        internal GalaxySeed FindPlanet(GalaxySeed galaxy, Vector2 centre)
        {
            GalaxySeed glx = (GalaxySeed)galaxy.Clone();
            float min_dist = 10000;
            GalaxySeed planet = new();

            for (int i = 0; i < 256; i++)
            {
                float dx = MathF.Abs(centre.X - glx.D);
                float dy = MathF.Abs(centre.Y - glx.B);

                float distance = dx > dy ? (dx + dx + dy) / 2 : (dx + dy + dy) / 2;

                if (distance < min_dist)
                {
                    min_dist = distance;
                    planet = (GalaxySeed)glx.Clone();
                }

                WaggleGalaxy(ref glx);
                WaggleGalaxy(ref glx);
                WaggleGalaxy(ref glx);
                WaggleGalaxy(ref glx);
            }

            return planet;
        }

        internal int FindPlanetNumber(GalaxySeed galaxy, GalaxySeed planet)
        {
            GalaxySeed glx = (GalaxySeed)galaxy.Clone();

            for (int i = 0; i < 256; i++)
            {

                if ((planet.A == glx.A) &&
                    (planet.B == glx.B) &&
                    (planet.C == glx.C) &&
                    (planet.D == glx.D) &&
                    (planet.E == glx.E) &&
                    (planet.F == glx.F))
                {
                    return i;
                }

                WaggleGalaxy(ref glx);
                WaggleGalaxy(ref glx);
                WaggleGalaxy(ref glx);
                WaggleGalaxy(ref glx);
            }

            return -1;
        }

        internal string NamePlanet(GalaxySeed galaxy, bool capitalise)
        {
            GalaxySeed glx = (GalaxySeed)galaxy.Clone();

            string name = string.Empty;
            int size = (glx.A & 0x40) == 0 ? 3 : 4;

            for (int i = 0; i < size; i++)
            {
                int x = glx.F & 0x1F;
                if (x != 0)
                {
                    x += 12;
                    x *= 2;
                    name += digrams[x];
                    if (digrams[x + 1] != '?')
                    {
                        name += digrams[x + 1];
                    }
                }

                WaggleGalaxy(ref glx);
            }

            return capitalise ? char.ToUpper(name[0]) + name[1..].ToLower() : name;
        }

        internal static string DescribeInhabitants(GalaxySeed planet)
        {
            StringBuilder sb = new("(");

            if (planet.E < 128)
            {
                sb.Append("Human Colonial");
            }
            else
            {
                int inhab = (planet.F / 4) & 7;
                if (inhab < 3)
                {
                    sb.Append(inhabitant_desc1[inhab]);
                }

                inhab = planet.F / 32;
                if (inhab < 6)
                {
                    sb.Append(inhabitant_desc2[inhab]);
                }

                inhab = (planet.D ^ planet.B) & 7;
                if (inhab < 6)
                {
                    sb.Append(inhabitant_desc3[inhab]);
                }

                inhab = (inhab + (planet.F & 3)) & 7;
                sb.Append(inhabitant_desc4[inhab]);
            }

            sb.Append("s)");
            return sb.ToString();
        }


        internal static PlanetData GeneratePlanetData(GalaxySeed planet_seed)
        {
            PlanetData pl = new()
            {
                government = (planet_seed.C / 8) & 7,
                economy = planet_seed.B & 7
            };

            if (pl.government < 2)
            {
                pl.economy |= 2;
            }

            pl.techlevel = pl.economy ^ 7;
            pl.techlevel += planet_seed.D & 3;
            pl.techlevel += (pl.government / 2) + (pl.government & 1);


            pl.population = pl.techlevel * 4;
            pl.population += pl.government;
            pl.population += pl.economy;
            pl.population++;


            pl.productivity = (pl.economy ^ 7) + 3;
            pl.productivity *= pl.government + 4;
            pl.productivity *= (int)pl.population;
            pl.productivity *= 8;

            pl.population /= 10;
            pl.radius = (((planet_seed.F & 15) + 11) * 256) + planet_seed.D;

            return pl;
        }

        internal bool FindPlanetByName(string find_name)
        {
            bool found = false;
            GalaxySeed glx = (GalaxySeed)_gameState.Cmdr.Galaxy.Clone();

            for (int i = 0; i < 256; i++)
            {
                string planet_name = NamePlanet(glx, false);

                if (planet_name == find_name)
                {
                    found = true;
                    _gameState.HyperspacePlanet = glx;
                    _gameState.PlanetName = planet_name;
                    break;
                }

                WaggleGalaxy(ref glx);
                WaggleGalaxy(ref glx);
                WaggleGalaxy(ref glx);
                WaggleGalaxy(ref glx);
            }

            return found;
        }

        internal static float CalculateDistanceToPlanet(GalaxySeed from_planet, GalaxySeed to_planet)
        {
            float dx = MathF.Abs(to_planet.D - from_planet.D);
            float dy = MathF.Abs(to_planet.B - from_planet.B);

            dx *= dx;
            dy /= 2;
            dy *= dy;

            float light_years = MathF.Sqrt(dx + dy);
            light_years *= 4f;

            return light_years / 10;
        }
    }
}
