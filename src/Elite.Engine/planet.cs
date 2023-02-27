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

/*
 *
 * Handle the generation of planet info...
 */

namespace Elite.Engine
{
    using System.Diagnostics;
    using System.Numerics;
    using System.Text;
    using Elite.Engine.Enums;
    using Elite.Engine.Types;

    internal class Planet
	{
        internal static string digrams = "ABOUSEITILETSTONLONUTHNOALLEXEGEZACEBISOUSESARMAINDIREA?ERATENBERALAVETIEDORQUANTEISRION";
        private static readonly string[] inhabitant_desc1 = new string[] { "Large ", "Fierce ", "Small " };
        private static readonly string[] inhabitant_desc2 = new string[] { "Green ", "Red ", "Yellow ", "Blue ", "Black ", "Harmless " };
        private static readonly string[] inhabitant_desc3 = new string[] { "Slimy ", "Bug-Eyed ", "Horned ", "Bony ", "Fat ", "Furry " };
        private static readonly string[] inhabitant_desc4 = new string[] { "Rodent", "Frog", "Lizard", "Lobster", "Bird", "Humanoid", "Feline", "Insect" };

		internal static void waggle_galaxy(ref galaxy_seed glx_ptr)
		{
			int x;
			int y;
			int carry_flag;

			x = glx_ptr.a + glx_ptr.c;
			y = glx_ptr.b + glx_ptr.d;

			if (x > 0xFF)
			{
				y++;
			}

			x &= 0xFF;
			y &= 0xFF;

			glx_ptr.a = glx_ptr.c;
			glx_ptr.b = glx_ptr.d;
			glx_ptr.c = glx_ptr.e;
			glx_ptr.d = glx_ptr.f;

			x += glx_ptr.c;
			y += glx_ptr.d;

			if (x > 0xFF)
			{
				y++;
			}

			carry_flag = y > 0xFF ? 1 : 0;

            x &= 0xFF;
			y &= 0xFF;

			glx_ptr.e = x;
			glx_ptr.f = y;
		}

		internal static galaxy_seed find_planet(Vector2 centre)
		{
			float min_dist = 10000;
			galaxy_seed planet = new();
			galaxy_seed glx = (galaxy_seed)elite.cmdr.galaxy.Clone();

			for (int i = 0; i < 256; i++)
			{
				float dx = MathF.Abs(centre.X - glx.d);
				float dy = MathF.Abs(centre.Y - glx.b);

				float distance = dx > dy ? (dx + dx + dy) / 2 : (dx + dy + dy) / 2;

                if (distance < min_dist)
				{
					min_dist = distance;
					planet = (galaxy_seed)glx.Clone();
				}

				waggle_galaxy(ref glx);
				waggle_galaxy(ref glx);
				waggle_galaxy(ref glx);
				waggle_galaxy(ref glx);
			}

			return planet;
		}

		internal static int find_planet_number(galaxy_seed planet)
		{
			galaxy_seed glx = (galaxy_seed)elite.cmdr.galaxy.Clone();

			for (int i = 0; i < 256; i++)
			{

				if ((planet.a == glx.a) &&
					(planet.b == glx.b) &&
					(planet.c == glx.c) &&
					(planet.d == glx.d) &&
					(planet.e == glx.e) &&
					(planet.f == glx.f))
                {
                    return i;
                }

                waggle_galaxy(ref glx);
				waggle_galaxy(ref glx);
				waggle_galaxy(ref glx);
				waggle_galaxy(ref glx);
			}

			return -1;
		}

		internal static string name_planet(galaxy_seed galaxy, bool capitalise)
		{
			galaxy_seed glx = (galaxy_seed)galaxy.Clone();

            string name = string.Empty;
			int size = (glx.a & 0x40) == 0 ? 3 : 4;

            for (int i = 0; i < size; i++)
			{
				int x = glx.f & 0x1F;
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

				waggle_galaxy(ref glx);
			}

			if (capitalise)
			{
                return char.ToUpper(name[0]) + name[1..].ToLower();
            }

			return name;
		}

		internal static string describe_inhabitants(galaxy_seed planet)
		{
			StringBuilder sb = new("(");

			if (planet.e < 128)
			{
				sb.Append("Human Colonial");
			}
			else
			{
				int inhab = (planet.f / 4) & 7;
				if (inhab < 3)
				{
					sb.Append(inhabitant_desc1[inhab]);
				}

				inhab = planet.f / 32;
				if (inhab < 6)
				{
					sb.Append(inhabitant_desc2[inhab]);
				}

				inhab = (planet.d ^ planet.b) & 7;
				if (inhab < 6)
				{
					sb.Append(inhabitant_desc3[inhab]);
				}

				inhab = (inhab + (planet.f & 3)) & 7;
				sb.Append(inhabitant_desc4[inhab]);
			}

			sb.Append("s)");
			return sb.ToString();
		}


		internal static planet_data generate_planet_data(galaxy_seed planet_seed)
		{
			planet_data pl = new();
			pl.government = (planet_seed.c / 8) & 7;

			pl.economy = planet_seed.b & 7;

			if (pl.government < 2)
            {
                pl.economy |= 2;
            }

            pl.techlevel = pl.economy ^ 7;
			pl.techlevel += planet_seed.d & 3;
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
			pl.radius = (((planet_seed.f & 15) + 11) * 256) + planet_seed.d;

			return pl;
		}

        internal static bool find_planet_by_name(string find_name)
        {
            bool found = false;
            galaxy_seed glx = (galaxy_seed)elite.cmdr.galaxy.Clone();

            for (int i = 0; i < 256; i++)
            {
                string planet_name = Planet.name_planet(glx, false);

                if (planet_name == find_name)
                {
                    found = true;
					elite.hyperspace_planet = glx;
					elite.planetName = planet_name;
                    break;
                }

                Planet.waggle_galaxy(ref glx);
                Planet.waggle_galaxy(ref glx);
                Planet.waggle_galaxy(ref glx);
                Planet.waggle_galaxy(ref glx);
            }

			return found;
        }

        internal static float calc_distance_to_planet(galaxy_seed from_planet, galaxy_seed to_planet)
        {
            float dx = MathF.Abs(to_planet.d - from_planet.d);
            float dy = MathF.Abs(to_planet.b - from_planet.b);

            dx *= dx;
            dy /= 2;
            dy *= dy;

            float light_years = MathF.Sqrt(dx + dy);
            light_years *= 4f;

            return light_years / 10;
        }
    }
}