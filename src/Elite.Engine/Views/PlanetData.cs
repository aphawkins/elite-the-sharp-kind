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

    internal class PlanetData : IView
	{
        private readonly IGfx _gfx;
        private readonly Mission _mission;
        private float _distanceToPlanet = 0;
        private planet_data _hyperPlanetData = new();

        private readonly string[] _economyType = {"Rich Industrial",
                                "Average Industrial",
                                "Poor Industrial",
                                "Mainly Industrial",
                                "Mainly Agricultural",
                                "Rich Agricultural",
                                "Average Agricultural",
                                "Poor Agricultural"};

        private readonly string[] _governmentType = { "Anarchy",
                                    "Feudal",
                                    "Multi-Government",
                                    "Dictatorship",
                                    "Communist",
                                    "Confederacy",
                                    "Democracy",
                                    "Corporate State"};

        private readonly string[][] _descriptionList = new[]
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

        internal PlanetData(IGfx gfx, Mission mission)
        {
            _gfx = gfx;
            _mission = mission;
        }

        private string DescribePlanet(galaxy_seed planet)
        {
            if (elite.cmdr.mission == 1)
            {
                string? mission_text = _mission.mission_planet_desc(planet);
                if (!string.IsNullOrEmpty(mission_text))
                {
                    return mission_text;
                }
            }

            RNG.Seed.a = planet.c;
            RNG.Seed.b = planet.d;
            RNG.Seed.c = planet.e;
            RNG.Seed.d = planet.f;

            if (elite.config.PlanetDescriptions == PlanetDescriptions.HoopyCasinos)
            {
                RNG.Seed.a ^= planet.a;
                RNG.Seed.b ^= planet.b;
                RNG.Seed.c ^= RNG.Seed.a;
                RNG.Seed.d ^= RNG.Seed.b;
            }

            string planet_description = string.Empty;

            ExpandDescription("<14> is <22>.", ref planet_description);

            return planet_description;
        }

        private void ExpandDescription(string source, ref string planetDescription)
        {
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

                    int num = Convert.ToInt32(temp);
                    Debug.Assert(num < _descriptionList.Length);
                    int option;

                    if (elite.config.PlanetDescriptions == PlanetDescriptions.HoopyCasinos)
                    {
                        option = RNG.gen_msx_rnd_number();
                    }
                    else
                    {
                        int rnd = RNG.gen_rnd_number();
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

                    ExpandDescription(_descriptionList[num][option], ref planetDescription);
                    continue;
                }

                if (source[j] == '%')
                {
                    j++;
                    switch (source[j])
                    {
                        case 'H':
                            temp = Planet.name_planet(elite.hyperspace_planet, true);
                            planetDescription += temp;
                            break;

                        case 'I':
                            temp = Planet.name_planet(elite.hyperspace_planet, true);
                            planetDescription += temp;
                            planetDescription += "ian";
                            break;

                        case 'R':
                            int len = RNG.gen_rnd_number() & 3;
                            for (int i = 0; i <= len; i++)
                            {
                                int x = RNG.gen_rnd_number() & 62;
                                if (i == 0)
                                {
                                    planetDescription += Planet.digrams[x];
                                }
                                else
                                {
                                    planetDescription += char.ToLower(Planet.digrams[x]);
                                }
                                planetDescription += char.ToLower(Planet.digrams[x + 1]);
                            }
                            break;
                    }

                    continue;
                }

                planetDescription += source[j];
            }
        }

        public void Reset()
        {
        }

        public void UpdateUniverse()
        {
            _distanceToPlanet = Planet.calc_distance_to_planet(elite.docked_planet, elite.hyperspace_planet);
            _hyperPlanetData = Planet.generate_planet_data(elite.hyperspace_planet);
        }

        public void Draw()
        {
            elite.draw.ClearDisplay();
            elite.draw.DrawViewHeader($"DATA ON {Planet.name_planet(elite.hyperspace_planet, false)}");

            if (_distanceToPlanet > 0)
            {
                _gfx.DrawTextLeft(16, 42, "Distance:", GFX_COL.GFX_COL_GREEN_1);
                _gfx.DrawTextLeft(140, 42, $"{_distanceToPlanet:N1} Light Years", GFX_COL.GFX_COL_WHITE);
            }
            _gfx.DrawTextLeft(16, 74, "Economy:", GFX_COL.GFX_COL_GREEN_1);
            _gfx.DrawTextLeft(140, 74, _economyType[_hyperPlanetData.economy], GFX_COL.GFX_COL_WHITE);
            _gfx.DrawTextLeft(16, 106, "Government:", GFX_COL.GFX_COL_GREEN_1);
            _gfx.DrawTextLeft(140, 106, _governmentType[_hyperPlanetData.government], GFX_COL.GFX_COL_WHITE);
            _gfx.DrawTextLeft(16, 138, "Tech Level:", GFX_COL.GFX_COL_GREEN_1);
            _gfx.DrawTextLeft(140, 138, $"{_hyperPlanetData.techlevel + 1}", GFX_COL.GFX_COL_WHITE);
            _gfx.DrawTextLeft(16, 170, "Population:", GFX_COL.GFX_COL_GREEN_1);
            _gfx.DrawTextLeft(140, 170, $"{_hyperPlanetData.population:N1} Billion {Planet.describe_inhabitants(elite.hyperspace_planet)}", GFX_COL.GFX_COL_WHITE);
            _gfx.DrawTextLeft(16, 202, "Gross Productivity:", GFX_COL.GFX_COL_GREEN_1);
            _gfx.DrawTextLeft(140, 202, $"{_hyperPlanetData.productivity} Million Credits", GFX_COL.GFX_COL_WHITE);
            _gfx.DrawTextLeft(16, 234, "Average Radius:", GFX_COL.GFX_COL_GREEN_1);
            _gfx.DrawTextLeft(140, 234, $"{_hyperPlanetData.radius} km", GFX_COL.GFX_COL_WHITE);
            elite.draw.DrawTextPretty(16, 266, 400, DescribePlanet(elite.hyperspace_planet));
        }

        public void HandleInput()
        {
        }
    }
}