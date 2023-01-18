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

namespace Elite
{
	using Elite.Enums;
	using Elite.Structs;
	using System.Diagnostics;
	using System.Text;

	internal static class Planet
	{
		struct random_seed
		{
			internal int a;
			internal int b;
			internal int c;
			internal int d;
		};

		static random_seed rnd_seed;

		static string digrams = "ABOUSEITILETSTONLONUTHNOALLEXEGEZACEBISOUSESARMAINDIREA?ERATENBERALAVETIEDORQUANTEISRION";

		static string[] inhabitant_desc1 = new string[] { "Large ", "Fierce ", "Small " };

		static string[] inhabitant_desc2 = new string[] { "Green ", "Red ", "Yellow ", "Blue ", "Black ", "Harmless " };

		static string[] inhabitant_desc3 = new string[] { "Slimy ", "Bug-Eyed ", "Horned ", "Bony ", "Fat ", "Furry " };

		static string[] inhabitant_desc4 = new string[] { "Rodent", "Frog", "Lizard", "Lobster", "Bird", "Humanoid", "Feline", "Insect" };

		static string[][] desc_list = new string[36][]
		{
		/*  0	*/	new string[] {"fabled", "notable", "well known", "famous", "noted"},
		/*  1	*/	new string[] {"very", "mildly", "most", "reasonably", ""},
		/*  2	*/	new string[] {"ancient", "<20>", "great", "vast", "pink"},
		/*  3	*/	new string[] {"<29> <28> plantations", "mountains", "<27>", "<19> forests", "oceans"},
		/*  4	*/	new string[] {"shyness", "silliness", "mating traditions", "loathing of <5>", "love for <5>"},
		/*  5	*/	new string[] {"food blenders", "tourists", "poetry", "discos", "<13>"},
		/*  6	*/	new string[] {"talking tree", "crab", "bat", "lobst", "%R"},
		/*  7	*/	new string[] {"beset", "plagued", "ravaged", "cursed", "scourged"},
		/*  8	*/	new string[] {"<21> civil war", "<26> <23> <24>s", "a <26> disease", "<21> earthquakes", "<21> solar activity"},
		/*  9	*/	new string[] {"its <2> <3>", "the %I <23> <24>","its inhabitants' <25> <4>", "<32>", "its <12> <13>"},
		/* 10	*/	new string[] {"juice", "brandy", "water", "brew", "gargle blasters"},
		/* 11	*/	new string[] {"%R", "%I <24>", "%I %R", "%I <26>", "<26> %R"},
		/* 12	*/	new string[] {"fabulous", "exotic", "hoopy", "unusual", "exciting"},
		/* 13	*/	new string[] {"cuisine", "night life", "casinos", "sit coms", " <32> "},
		/* 14	*/	new string[] {"%H", "The planet %H", "The world %H", "This planet", "This world"},
		/* 15	*/	new string[] {"n unremarkable", " boring", " dull", " tedious", " revolting"},
		/* 16	*/	new string[] {"planet", "world", "place", "little planet", "dump"},
		/* 17	*/	new string[] {"wasp", "moth", "grub", "ant", "%R"},
		/* 18	*/	new string[] {"poet", "arts graduate", "yak", "snail", "slug"},
		/* 19	*/	new string[] {"tropical", "dense", "rain", "impenetrable", "exuberant"},
		/* 20	*/	new string[] {"funny", "wierd", "unusual", "strange", "peculiar"},
		/* 21	*/	new string[] {"frequent", "occasional", "unpredictable", "dreadful", "deadly"},
		/* 22	*/	new string[] {"<1> <0> for <9>", "<1> <0> for <9> and <9>", "<7> by <8>", "<1> <0> for <9> but <7> by <8>"," a<15> <16>"},
		/* 23	*/	new string[] {"<26>", "mountain", "edible", "tree", "spotted"},
		/* 24	*/	new string[] {"<30>", "<31>", "<6>oid", "<18>", "<17>"},
		/* 25	*/	new string[] {"ancient", "exceptional", "eccentric", "ingrained", "<20>"},
		/* 26	*/	new string[] {"killer", "deadly", "evil", "lethal", "vicious"},
		/* 27	*/	new string[] {"parking meters", "dust clouds", "ice bergs", "rock formations", "volcanoes"},
		/* 28	*/	new string[] {"plant", "tulip", "banana", "corn", "%Rweed"},
		/* 29	*/	new string[] {"%R", "%I %R", "%I <26>", "inhabitant", "%I %R"},
		/* 30	*/	new string[] {"shrew", "beast", "bison", "snake", "wolf"},
		/* 31	*/	new string[] {"leopard", "cat", "monkey", "goat", "fish"},
		/* 32	*/	new string[] {"<11> <10>", "%I <30> <33>","its <12> <31> <33>", "<34> <35>", "<11> <10>"},
		/* 33	*/	new string[] {"meat", "cutlet", "steak", "burgers", "soup"},
		/* 34	*/	new string[] {"ice", "mud", "Zero-G", "vacuum", "%I ultra"},
		/* 35	*/	new string[] {"hockey", "cricket", "karate", "polo", "tennis"}
		};

		/*
		 * Generate a random number between 0 and 255.
		 * This is the version used in the 6502 Elites.
		 */
		static int gen_rnd_number()
		{
			int a, x;

			x = (rnd_seed.a * 2) & 0xFF;
			a = x + rnd_seed.c;
			if (rnd_seed.a > 127)
			{
				a++;
			}
			rnd_seed.a = a & 0xFF;
			rnd_seed.c = x;

			a /= 256;    /* a = any carry left from above */
			x = rnd_seed.b;
			a = (a + x + rnd_seed.d) & 0xFF;
			rnd_seed.b = a;
			rnd_seed.d = x;
			return a;
		}

		/*
		 * Generate a random number between 0 and 255.
		 * This is the version used in the MSX and 16bit Elites.
		 */
		static int gen_msx_rnd_number()
		{
			int a, b;

			a = rnd_seed.a;
			b = rnd_seed.b;

			rnd_seed.a = rnd_seed.c;
			rnd_seed.b = rnd_seed.d;

			a += rnd_seed.c;
			b = (b + rnd_seed.d) & 255;
			if (a > 255)
			{
				a &= 255;
				b++;
			}

			rnd_seed.c = a;
			rnd_seed.d = b;

			return rnd_seed.c / 0x34;
		}

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

		internal static galaxy_seed find_planet(int cx, int cy)
		{
			int min_dist = 10000;
			galaxy_seed planet = new();
			galaxy_seed glx = elite.cmdr.galaxy;

			for (int i = 0; i < 256; i++)
			{
				int dx = Math.Abs(cx - glx.d);
				int dy = Math.Abs(cy - glx.b);

				int distance = dx > dy ? (dx + dx + dy) / 2 : (dx + dy + dy) / 2;

                if (distance < min_dist)
				{
					min_dist = distance;
					planet = glx;
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
			galaxy_seed glx = elite.cmdr.galaxy;

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

		internal static string name_planet(galaxy_seed glx)
		{
			string gname = string.Empty;
			int size = (glx.a & 0x40) == 0 ? 3 : 4;

            for (int i = 0; i < size; i++)
			{
				int x = glx.f & 0x1F;
				if (x != 0)
				{
					x += 12;
					x *= 2;
					gname += digrams[x];
					if (digrams[x + 1] != '?')
					{
						gname += digrams[x + 1];
					}
				}

				waggle_galaxy(ref glx);
			}

			return gname;
		}

		internal static string capitalise_name(string name)
		{
			return char.ToUpper(name[0]) + name[1..].ToLower();
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

		static void expand_description(string source, ref string planet_description)
		{
			string temp = string.Empty;
			string expanded;
			int num;
			int rnd;
			int option;
			int i, len, x;

			for (int j = 0; j < source.Length; j++)
			{
				if (source[j] == '<')
				{
					j++;
                    temp = string.Empty;

                    while (source[j] != '>')
					{
						temp += source[j];
						j++;
					}

					num = Convert.ToInt32(temp);
					Debug.Assert(num < desc_list.Length);
					expanded = temp;

					if (elite.config.PlanetDescriptions == Enums.PlanetDescriptions.HoopyCasinos)
					{
						option = gen_msx_rnd_number();
					}
					else
					{
						rnd = gen_rnd_number();
						option = 0;
						if (rnd >= 0x33) option++;
						if (rnd >= 0x66) option++;
						if (rnd >= 0x99) option++;
						if (rnd >= 0xCC) option++;
					}

					expand_description(desc_list[num][option], ref planet_description);
					continue;
				}

				if (source[j] == '%')
				{
					j++;
					switch (source[j])
					{
						case 'H':
							temp = name_planet(elite.hyperspace_planet);
							temp = Planet.capitalise_name(temp);
							planet_description += temp;
							break;

						case 'I':
							temp = name_planet(elite.hyperspace_planet);
							temp = Planet.capitalise_name(temp);
							planet_description += temp;
							planet_description += "ian";
							break;

						case 'R':
							len = gen_rnd_number() & 3;
							for (i = 0; i <= len; i++)
							{
								x = gen_rnd_number() & 0x3e;
								if (i == 0)
								{
									planet_description += digrams[x];
								}
								else
								{
									planet_description += char.ToLower(digrams[x]);
								}
								planet_description += char.ToLower(digrams[x + 1]);
							}
							break;
					}

					continue;
				}

				planet_description += source[j];
			}

		}

		internal static string describe_planet(galaxy_seed planet)
		{
			string mission_text;

			if (elite.cmdr.mission == 1)
			{
				mission_text = missions.mission_planet_desc(planet);
				if (mission_text != null)
				{
					return mission_text;
				}
			}

			rnd_seed.a = planet.c;
			rnd_seed.b = planet.d;
			rnd_seed.c = planet.e;
			rnd_seed.d = planet.f;

			if (elite.config.PlanetDescriptions == PlanetDescriptions.HoopyCasinos)
			{
				rnd_seed.a ^= planet.a;
				rnd_seed.b ^= planet.b;
				rnd_seed.c ^= rnd_seed.a;
				rnd_seed.d ^= rnd_seed.b;
			}

			string planet_description = string.Empty;

            expand_description("<14> is <22>.", ref planet_description);

			return planet_description;
		}

		internal static void generate_planet_data(ref planet_data pl, galaxy_seed planet_seed)
		{

			pl.government = (planet_seed.c / 8) & 7;

			pl.economy = planet_seed.b & 7;

			if (pl.government < 2)
				pl.economy |= 2;

			pl.techlevel = pl.economy ^ 7;
			pl.techlevel += planet_seed.d & 3;
			pl.techlevel += (pl.government / 2) + (pl.government & 1);


			pl.population = pl.techlevel * 4;
			pl.population += pl.government;
			pl.population += pl.economy;
			pl.population++;

			pl.productivity = (pl.economy ^ 7) + 3;
			pl.productivity *= pl.government + 4;
			pl.productivity *= pl.population;
			pl.productivity *= 8;

			pl.radius = (((planet_seed.f & 15) + 11) * 256) + planet_seed.d;
		}
	}
}