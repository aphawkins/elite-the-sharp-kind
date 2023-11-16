// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using System.Text;
using EliteSharp.Types;

namespace EliteSharp
{
    internal sealed class PlanetController
    {
        private readonly GameState _gameState;
        private readonly string[] _inhabitant_desc1 = ["Large ", "Fierce ", "Small "];
        private readonly string[] _inhabitant_desc2 = ["Green ", "Red ", "Yellow ", "Blue ", "Black ", "Harmless "];
        private readonly string[] _inhabitant_desc3 = ["Slimy ", "Bug-Eyed ", "Horned ", "Bony ", "Fat ", "Furry "];
        private readonly string[] _inhabitant_desc4 =
        [
            "Rodent",
            "Frog",
            "Lizard",
            "Lobster",
            "Bird",
            "Humanoid",
            "Feline",
            "Insect",
        ];

        internal PlanetController(GameState gameState) => _gameState = gameState;

        internal string Digrams { get; } = "ABOUSEITILETSTONLONUTHNOALLEXEGEZACEBISOUSESARMAINDIREA?ERATENBERALAVETIEDORQUANTEISRION";

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

        internal static PlanetData GeneratePlanetData(GalaxySeed planet_seed)
        {
            PlanetData pl = new()
            {
                Government = (planet_seed.C / 8) & 7,
                Economy = planet_seed.B & 7,
            };

            if (pl.Government < 2)
            {
                pl.Economy |= 2;
            }

            pl.TechLevel = pl.Economy ^ 7;
            pl.TechLevel += planet_seed.D & 3;
            pl.TechLevel += (pl.Government / 2) + (pl.Government & 1);

            pl.Population = pl.TechLevel * 4;
            pl.Population += pl.Government;
            pl.Population += pl.Economy;
            pl.Population++;

            pl.Productivity = (pl.Economy ^ 7) + 3;
            pl.Productivity *= pl.Government + 4;
            pl.Productivity *= (int)pl.Population;
            pl.Productivity *= 8;

            pl.Population /= 10;
            pl.Radius = (((planet_seed.F & 15) + 11) * 256) + planet_seed.D;

            return pl;
        }

        internal string DescribeInhabitants(GalaxySeed planet)
        {
            StringBuilder sb = new("(");

            if (planet.E < 128)
            {
                _ = sb.Append("Human Colonial");
            }
            else
            {
                int inhab = (planet.F / 4) & 7;
                if (inhab < 3)
                {
                    _ = sb.Append(_inhabitant_desc1[inhab]);
                }

                inhab = planet.F / 32;
                if (inhab < 6)
                {
                    _ = sb.Append(_inhabitant_desc2[inhab]);
                }

                inhab = (planet.D ^ planet.B) & 7;
                if (inhab < 6)
                {
                    _ = sb.Append(_inhabitant_desc3[inhab]);
                }

                inhab = (inhab + (planet.F & 3)) & 7;
                _ = sb.Append(_inhabitant_desc4[inhab]);
            }

            _ = sb.Append("s)");
            return sb.ToString();
        }

        internal GalaxySeed FindPlanet(GalaxySeed galaxy, Vector2 centre)
        {
            GalaxySeed glx = new(galaxy);
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
                    planet = new(glx);
                }

                WaggleGalaxy(glx);
                WaggleGalaxy(glx);
                WaggleGalaxy(glx);
                WaggleGalaxy(glx);
            }

            return planet;
        }

        internal bool FindPlanetByName(string find_name)
        {
            bool found = false;
            GalaxySeed glx = new(_gameState.Cmdr.Galaxy);

            for (int i = 0; i < 256; i++)
            {
                string planet_name = NamePlanet(glx);

                if (planet_name == find_name)
                {
                    found = true;
                    _gameState.HyperspacePlanet = glx;
                    _gameState.PlanetName = planet_name;
                    break;
                }

                WaggleGalaxy(glx);
                WaggleGalaxy(glx);
                WaggleGalaxy(glx);
                WaggleGalaxy(glx);
            }

            return found;
        }

        internal int FindPlanetNumber(GalaxySeed galaxy, GalaxySeed planet)
        {
            GalaxySeed glx = new(galaxy);

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

                WaggleGalaxy(glx);
                WaggleGalaxy(glx);
                WaggleGalaxy(glx);
                WaggleGalaxy(glx);
            }

            return -1;
        }

        internal string NamePlanet(GalaxySeed galaxy)
        {
            GalaxySeed glx = new(galaxy);

            StringBuilder name = new();
            int size = (glx.A & 64) == 0 ? 3 : 4;

            for (int i = 0; i < size; i++)
            {
                int x = glx.F & 31;
                if (x != 0)
                {
                    x += 12;
                    x *= 2;
                    name.Append(Digrams[x]);
                    if (Digrams[x + 1] != '?')
                    {
                        name.Append(Digrams[x + 1]);
                    }
                }

                WaggleGalaxy(glx);
            }

            return name.ToString();
        }

        internal void WaggleGalaxy(GalaxySeed glx_ptr)
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
    }
}
