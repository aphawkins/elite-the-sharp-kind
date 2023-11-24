// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Diagnostics;
using System.Globalization;
using System.Text;
using EliteSharp.Graphics;
using EliteSharp.Missions;
using EliteSharp.Planets;
using EliteSharp.Types;

namespace EliteSharp.Views
{
    internal sealed class PlanetDataView : IView
    {
        private readonly string[][] _descriptionList =
        [
            ["fabled", "notable", "well known", "famous", "noted"],
            ["very", "mildly", "most", "reasonably", string.Empty],
            ["ancient", "<20>", "great", "vast", "pink"],
            ["<29> <28> plantations", "mountains", "<27>", "<19> forests", "oceans"],
            ["shyness", "silliness", "mating traditions", "loathing of <5>", "love for <5>"],
            ["food blenders", "tourists", "poetry", "discos", "<13>"],
            ["talking tree", "crab", "bat", "lobst", "%R"],
            ["beset", "plagued", "ravaged", "cursed", "scourged"],
            ["<21> civil war", "<26> <23> <24>s", "a <26> disease", "<21> earthquakes", "<21> solar activity"],
            ["its <2> <3>", "the %I <23> <24>", "its inhabitants' <25> <4>", "<32>", "its <12> <13>"],
            ["juice", "brandy", "water", "brew", "gargle blasters"],
            ["%R", "%I <24>", "%I %R", "%I <26>", "<26> %R"],
            ["fabulous", "exotic", "hoopy", "unusual", "exciting"],
            ["cuisine", "night life", "casinos", "sit coms", " <32> "],
            ["%H", "The planet %H", "The world %H", "This planet", "This world"],
            ["n unremarkable", " boring", " dull", " tedious", " revolting"],
            ["planet", "world", "place", "little planet", "dump"],
            ["wasp", "moth", "grub", "ant", "%R"],
            ["poet", "arts graduate", "yak", "snail", "slug"],
            ["tropical", "dense", "rain", "impenetrable", "exuberant"],
            ["funny", "wierd", "unusual", "strange", "peculiar"],
            ["frequent", "occasional", "unpredictable", "dreadful", "deadly"],
            ["<1> <0> for <9>", "<1> <0> for <9> and <9>", "<7> by <8>", "<1> <0> for <9> but <7> by <8>", " a<15> <16>"],
            ["<26>", "mountain", "edible", "tree", "spotted"],
            ["<30>", "<31>", "<6>oid", "<18>", "<17>"],
            ["ancient", "exceptional", "eccentric", "ingrained", "<20>"],
            ["killer", "deadly", "evil", "lethal", "vicious"],
            ["parking meters", "dust clouds", "ice bergs", "rock formations", "volcanoes"],
            ["plant", "tulip", "banana", "corn", "%Rweed"],
            ["%R", "%I %R", "%I <26>", "inhabitant", "%I %R"],
            ["shrew", "beast", "bison", "snake", "wolf"],
            ["leopard", "cat", "monkey", "goat", "fish"],
            ["<11> <10>", "%I <30> <33>", "its <12> <31> <33>", "<34> <35>", "<11> <10>"],
            ["meat", "cutlet", "steak", "burgers", "soup"],
            ["ice", "mud", "Zero-G", "vacuum", "%I ultra"],
            ["hockey", "cricket", "karate", "polo", "tennis"],
        ];

        private readonly IDraw _draw;

        private readonly string[] _economyType =
        [
            "Rich Industrial",
            "Average Industrial",
            "Poor Industrial",
            "Mainly Industrial",
            "Mainly Agricultural",
            "Rich Agricultural",
            "Average Agricultural",
            "Poor Agricultural",
        ];

        private readonly GameState _gameState;

        private readonly string[] _governmentType =
        [
            "Anarchy",
            "Feudal",
            "Multi-Government",
            "Dictatorship",
            "Communist",
            "Confederacy",
            "Democracy",
            "Corporate State",
        ];

        private readonly PlanetController _planet;
        private float _distanceToPlanet;
        private PlanetData _hyperPlanetData = new();

        internal PlanetDataView(GameState gameState, IDraw draw, PlanetController planet)
        {
            _gameState = gameState;
            _draw = draw;
            _planet = planet;
        }

        public void Draw()
        {
            _draw.DrawViewHeader($"DATA ON {_planet.NamePlanet(_gameState.HyperspacePlanet)}");

            if (_distanceToPlanet > 0)
            {
                _draw.Graphics.DrawTextLeft(new(16 + _draw.Offset, 42), "Distance:", EliteColors.Green);
                _draw.Graphics.DrawTextLeft(new(140 + _draw.Offset, 42), $"{_distanceToPlanet:N1} Light Years", EliteColors.White);
            }

            _draw.Graphics.DrawTextLeft(new(16 + _draw.Offset, 74), "Economy:", EliteColors.Green);
            _draw.Graphics.DrawTextLeft(new(140 + _draw.Offset, 74), _economyType[_hyperPlanetData.Economy], EliteColors.White);
            _draw.Graphics.DrawTextLeft(new(16 + _draw.Offset, 106), "Government:", EliteColors.Green);
            _draw.Graphics.DrawTextLeft(new(140 + _draw.Offset, 106), _governmentType[_hyperPlanetData.Government], EliteColors.White);
            _draw.Graphics.DrawTextLeft(new(16 + _draw.Offset, 138), "Tech Level:", EliteColors.Green);
            _draw.Graphics.DrawTextLeft(new(140 + _draw.Offset, 138), $"{_hyperPlanetData.TechLevel + 1}", EliteColors.White);
            _draw.Graphics.DrawTextLeft(new(16 + _draw.Offset, 170), "Population:", EliteColors.Green);
            _draw.Graphics.DrawTextLeft(
                new(140 + _draw.Offset, 170),
                $"{_hyperPlanetData.Population:N1} Billion {_planet.DescribeInhabitants(_gameState.HyperspacePlanet)}",
                EliteColors.White);
            _draw.Graphics.DrawTextLeft(
                new(16 + _draw.Offset, 202),
                "Gross Productivity:",
                EliteColors.Green);
            _draw.Graphics.DrawTextLeft(
                new(140 + _draw.Offset, 202),
                $"{_hyperPlanetData.Productivity} Million Credits",
                EliteColors.White);
            _draw.Graphics.DrawTextLeft(
                new(16 + _draw.Offset, 234),
                "Average Radius:",
                EliteColors.Green);
            _draw.Graphics.DrawTextLeft(
                new(140 + _draw.Offset, 234),
                $"{_hyperPlanetData.Radius} km",
                EliteColors.White);
            _draw.DrawTextPretty(new(16 + _draw.Offset, 266), 400, DescribePlanet(_gameState.HyperspacePlanet));
        }

        public void HandleInput()
        {
        }

        public void Reset()
        {
        }

        public void UpdateUniverse()
        {
            _distanceToPlanet = PlanetController.CalculateDistanceToPlanet(_gameState.DockedPlanet, _gameState.HyperspacePlanet);
            _hyperPlanetData = PlanetController.GeneratePlanetData(_gameState.HyperspacePlanet);
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
            StringBuilder temp = new();

            for (int j = 0; j < source.Length; j++)
            {
                temp.Clear();

                if (source[j] == '<')
                {
                    j++;
                    temp.Clear();

                    while (source[j] != '>')
                    {
                        temp.Append(source[j]);
                        j++;
                    }

                    int num = Convert.ToInt32(temp.ToString(), CultureInfo.InvariantCulture);
                    Debug.Assert(num < _descriptionList.Length, "Number should be within the description range.");
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
                            temp = new(_planet.NamePlanet(_gameState.HyperspacePlanet).CapitaliseFirstLetter());
                            planetDescription += temp;
                            break;

                        case 'I':
                            temp = new(_planet.NamePlanet(_gameState.HyperspacePlanet).CapitaliseFirstLetter());
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
                                    planetDescription += char.ToLowerInvariant(_planet.Digrams[x]);
                                }

                                planetDescription += char.ToLowerInvariant(_planet.Digrams[x + 1]);
                            }

                            break;
                    }

                    continue;
                }

                planetDescription += source[j];
            }
        }
    }
}
