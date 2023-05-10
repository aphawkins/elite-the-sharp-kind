// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Diagnostics;
using Elite.Engine.Enums;
using Elite.Engine.Missions;
using Elite.Engine.Types;

namespace Elite.Engine.Views
{
    internal sealed class PlanetDataView : IView
    {
        private readonly string[][] _descriptionList = new[]
        {
        new string[] {"fabled", "notable", "well known", "famous", "noted"},
        new string[] {"very", "mildly", "most", "reasonably", ""},
        new string[] {"ancient", "<20>", "great", "vast", "pink"},
        new string[] {"<29> <28> plantations", "mountains", "<27>", "<19> forests", "oceans"},
        new string[] {"shyness", "silliness", "mating traditions", "loathing of <5>", "love for <5>"},
        new string[] {"food blenders", "tourists", "poetry", "discos", "<13>"},
        new string[] {"talking tree", "crab", "bat", "lobst", "%R"},
        new string[] {"beset", "plagued", "ravaged", "cursed", "scourged"},
        new string[] {"<21> civil war", "<26> <23> <24>s", "a <26> disease", "<21> earthquakes", "<21> solar activity"},
        new string[] {"its <2> <3>", "the %I <23> <24>","its inhabitants' <25> <4>", "<32>", "its <12> <13>"},
        new string[] {"juice", "brandy", "water", "brew", "gargle blasters"},
        new string[] {"%R", "%I <24>", "%I %R", "%I <26>", "<26> %R"},
        new string[] {"fabulous", "exotic", "hoopy", "unusual", "exciting"},
        new string[] {"cuisine", "night life", "casinos", "sit coms", " <32> "},
        new string[] {"%H", "The planet %H", "The world %H", "This planet", "This world"},
        new string[] {"n unremarkable", " boring", " dull", " tedious", " revolting"},
        new string[] {"planet", "world", "place", "little planet", "dump"},
        new string[] {"wasp", "moth", "grub", "ant", "%R"},
        new string[] {"poet", "arts graduate", "yak", "snail", "slug"},
        new string[] {"tropical", "dense", "rain", "impenetrable", "exuberant"},
        new string[] {"funny", "wierd", "unusual", "strange", "peculiar"},
        new string[] {"frequent", "occasional", "unpredictable", "dreadful", "deadly"},
        new string[] {"<1> <0> for <9>", "<1> <0> for <9> and <9>", "<7> by <8>", "<1> <0> for <9> but <7> by <8>"," a<15> <16>"},
        new string[] {"<26>", "mountain", "edible", "tree", "spotted"},
        new string[] {"<30>", "<31>", "<6>oid", "<18>", "<17>"},
        new string[] {"ancient", "exceptional", "eccentric", "ingrained", "<20>"},
        new string[] {"killer", "deadly", "evil", "lethal", "vicious"},
        new string[] {"parking meters", "dust clouds", "ice bergs", "rock formations", "volcanoes"},
        new string[] {"plant", "tulip", "banana", "corn", "%Rweed"},
        new string[] {"%R", "%I %R", "%I <26>", "inhabitant", "%I %R"},
        new string[] {"shrew", "beast", "bison", "snake", "wolf"},
        new string[] {"leopard", "cat", "monkey", "goat", "fish"},
        new string[] {"<11> <10>", "%I <30> <33>","its <12> <31> <33>", "<34> <35>", "<11> <10>"},
        new string[] {"meat", "cutlet", "steak", "burgers", "soup"},
        new string[] {"ice", "mud", "Zero-G", "vacuum", "%I ultra"},
        new string[] {"hockey", "cricket", "karate", "polo", "tennis"}
        };

        private readonly Draw _draw;

        private readonly string[] _economyType = {"Rich Industrial",
                                "Average Industrial",
                                "Poor Industrial",
                                "Mainly Industrial",
                                "Mainly Agricultural",
                                "Rich Agricultural",
                                "Average Agricultural",
                                "Poor Agricultural"};

        private readonly GameState _gameState;
        private readonly IGfx _gfx;

        private readonly string[] _governmentType = { "Anarchy",
                                    "Feudal",
                                    "Multi-Government",
                                    "Dictatorship",
                                    "Communist",
                                    "Confederacy",
                                    "Democracy",
                                    "Corporate State"};

        private readonly Planet _planet;
        private float _distanceToPlanet;
        private PlanetData _hyperPlanetData = new();

        internal PlanetDataView(GameState gameState, IGfx gfx, Draw draw, Planet planet)
        {
            _gameState = gameState;
            _gfx = gfx;
            _draw = draw;
            _planet = planet;
        }

        public void Draw()
        {
            _draw.ClearDisplay();
            _draw.DrawViewHeader($"DATA ON {_planet.NamePlanet(_gameState.HyperspacePlanet, false)}");

            if (_distanceToPlanet > 0)
            {
                _gfx.DrawTextLeft(16, 42, "Distance:", Colour.Green1);
                _gfx.DrawTextLeft(140, 42, $"{_distanceToPlanet:N1} Light Years", Colour.White1);
            }
            _gfx.DrawTextLeft(16, 74, "Economy:", Colour.Green1);
            _gfx.DrawTextLeft(140, 74, _economyType[_hyperPlanetData.Economy], Colour.White1);
            _gfx.DrawTextLeft(16, 106, "Government:", Colour.Green1);
            _gfx.DrawTextLeft(140, 106, _governmentType[_hyperPlanetData.Government], Colour.White1);
            _gfx.DrawTextLeft(16, 138, "Tech Level:", Colour.Green1);
            _gfx.DrawTextLeft(140, 138, $"{_hyperPlanetData.TechLevel + 1}", Colour.White1);
            _gfx.DrawTextLeft(16, 170, "Population:", Colour.Green1);
            _gfx.DrawTextLeft(140, 170, $"{_hyperPlanetData.Population:N1} Billion {_planet.DescribeInhabitants(_gameState.HyperspacePlanet)}", Colour.White1);
            _gfx.DrawTextLeft(16, 202, "Gross Productivity:", Colour.Green1);
            _gfx.DrawTextLeft(140, 202, $"{_hyperPlanetData.Productivity} Million Credits", Colour.White1);
            _gfx.DrawTextLeft(16, 234, "Average Radius:", Colour.Green1);
            _gfx.DrawTextLeft(140, 234, $"{_hyperPlanetData.Radius} km", Colour.White1);
            _draw.DrawTextPretty(16, 266, 400, DescribePlanet(_gameState.HyperspacePlanet));
        }

        public void HandleInput()
        {
        }

        public void Reset()
        {
        }

        public void UpdateUniverse()
        {
            _distanceToPlanet = Planet.CalculateDistanceToPlanet(_gameState.DockedPlanet, _gameState.HyperspacePlanet);
            _hyperPlanetData = Planet.GeneratePlanetData(_gameState.HyperspacePlanet);
        }

        private string DescribePlanet(GalaxySeed planet)
        {
            if (_gameState.Cmdr.Mission == 1)
            {
                string? mission_text = new Mission(_planet).MissionPlanetDescription(_gameState, planet);
                if (!string.IsNullOrEmpty(mission_text))
                {
                    return mission_text;
                }
            }

            RNG.Seed.A = planet.C;
            RNG.Seed.B = planet.D;
            RNG.Seed.C = planet.E;
            RNG.Seed.D = planet.F;

            if (_gameState.Config.PlanetDescriptions == PlanetDescriptions.HoopyCasinos)
            {
                RNG.Seed.A ^= planet.A;
                RNG.Seed.B ^= planet.B;
                RNG.Seed.C ^= RNG.Seed.A;
                RNG.Seed.D ^= RNG.Seed.B;
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

                    if (_gameState.Config.PlanetDescriptions == PlanetDescriptions.HoopyCasinos)
                    {
                        option = RNG.GenMSXRandomNumber();
                    }
                    else
                    {
                        int rnd = RNG.GenerateRandomNumber();
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
                            temp = _planet.NamePlanet(_gameState.HyperspacePlanet, true);
                            planetDescription += temp;
                            break;

                        case 'I':
                            temp = _planet.NamePlanet(_gameState.HyperspacePlanet, true);
                            planetDescription += temp;
                            planetDescription += "ian";
                            break;

                        case 'R':
                            int len = RNG.GenerateRandomNumber() & 3;
                            for (int i = 0; i <= len; i++)
                            {
                                int x = RNG.GenerateRandomNumber() & 62;
                                if (i == 0)
                                {
                                    planetDescription += _planet.Digrams[x];
                                }
                                else
                                {
                                    planetDescription += char.ToLower(_planet.Digrams[x]);
                                }
                                planetDescription += char.ToLower(_planet.Digrams[x + 1]);
                            }
                            break;

                        default:
                            break;
                    }

                    continue;
                }

                planetDescription += source[j];
            }
        }
    }
}
