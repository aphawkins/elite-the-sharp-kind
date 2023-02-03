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
    using System.Diagnostics;
    using Elite.Engine.Enums;
    using Elite.Engine.Missions;
    using Elite.Engine.Types;

    internal class PlanetData
	{
        private Mission _mission;
        private random_seed _rnd_seed = new();

        private static readonly string[] economy_type = {"Rich Industrial",
                                "Average Industrial",
                                "Poor Industrial",
                                "Mainly Industrial",
                                "Mainly Agricultural",
                                "Rich Agricultural",
                                "Average Agricultural",
                                "Poor Agricultural"};

        private static readonly string[] government_type = { "Anarchy",
                                    "Feudal",
                                    "Multi-Government",
                                    "Dictatorship",
                                    "Communist",
                                    "Confederacy",
                                    "Democracy",
                                    "Corporate State"};

        private static readonly string[][] desc_list = new string[36][]
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

        internal PlanetData(Mission mission)
        {
            _mission = mission;
        }

        /// <summary>
        /// Displays data on the currently selected Hyperspace Planet.
        /// </summary>
        internal void display_data_on_planet()
		{
			planet_data hyper_planet_data = new();

			elite.SetView(SCR.SCR_PLANET_DATA);

			string planetName = Planet.name_planet(elite.hyperspace_planet, false);
            float lightYears = GalacticChart.calc_distance_to_planet(elite.docked_planet, elite.hyperspace_planet);
            Planet.generate_planet_data(ref hyper_planet_data, elite.hyperspace_planet);

            elite.draw.DrawDataOnPlanet(planetName, lightYears,
                economy_type[hyper_planet_data.economy],
                government_type[hyper_planet_data.government],
                hyper_planet_data.techlevel + 1,
				hyper_planet_data.population,
				Planet.describe_inhabitants(elite.hyperspace_planet),
                hyper_planet_data.productivity,
                hyper_planet_data.radius,
                describe_planet(elite.hyperspace_planet)
                );
		}

        internal string describe_planet(galaxy_seed planet)
        {
            if (elite.cmdr.mission == 1)
            {
                string? mission_text = _mission.mission_planet_desc(planet);
                if (!string.IsNullOrEmpty(mission_text))
                {
                    return mission_text;
                }
            }

            random_seed rnd_seed = new()
            {
                a = planet.c,
                b = planet.d,
                c = planet.e,
                d = planet.f,
            };

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

        private void expand_description(string source, ref string planet_description)
        {
            int num;
            int rnd;
            int option;
            int i;
            int len;
            int x;

            for (int j = 0; j < source.Length; j++)
            {
                string temp;
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

                    if (elite.config.PlanetDescriptions == PlanetDescriptions.HoopyCasinos)
                    {
                        option = gen_msx_rnd_number();
                    }
                    else
                    {
                        rnd = gen_rnd_number();
                        option = 0;
                        if (rnd >= 0x33)
                        {
                            option++;
                        }

                        if (rnd >= 0x66)
                        {
                            option++;
                        }

                        if (rnd >= 0x99)
                        {
                            option++;
                        }

                        if (rnd >= 0xCC)
                        {
                            option++;
                        }
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
                            temp = Planet.name_planet(elite.hyperspace_planet, true);
                            planet_description += temp;
                            break;

                        case 'I':
                            temp = Planet.name_planet(elite.hyperspace_planet, true);
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
                                    planet_description += Planet.digrams[x];
                                }
                                else
                                {
                                    planet_description += char.ToLower(Planet.digrams[x]);
                                }
                                planet_description += char.ToLower(Planet.digrams[x + 1]);
                            }
                            break;
                    }

                    continue;
                }

                planet_description += source[j];
            }
        }

        /// <summary>
        /// Generate a random number between 0 and 255.
        /// This is the version used in the 6502 Elites.
        /// </summary>
        /// <returns>A random number between 0 and 255.</returns>
        private int gen_rnd_number()
        {
            int a, x;

            x = (_rnd_seed.a * 2) & 0xFF;
            a = x + _rnd_seed.c;
            if (_rnd_seed.a > 127)
            {
                a++;
            }

            _rnd_seed.a = a & 0xFF;
            _rnd_seed.c = x;

            a /= 256;    /* a = any carry left from above */
            x = _rnd_seed.b;
            a = (a + x + _rnd_seed.d) & 0xFF;
            _rnd_seed.b = a;
            _rnd_seed.d = x;
            return a;
        }

        /// <summary>
        /// Generate a random number between 0 and 255.
        /// This is the version used in the MSX and 16bit Elites.
        /// </summary>
        /// <returns>A random number between 0 and 255.</returns>
        private int gen_msx_rnd_number()
        {
            int a = _rnd_seed.a;
            int b = _rnd_seed.b;

            _rnd_seed.a = _rnd_seed.c;
            _rnd_seed.b = _rnd_seed.d;

            a += _rnd_seed.c;
            b = (b + _rnd_seed.d) & 255;
            if (a > 255)
            {
                a &= 255;
                b++;
            }

            _rnd_seed.c = a;
            _rnd_seed.d = b;

            return _rnd_seed.c / 0x34;
        }
    }
}