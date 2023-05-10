// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using Elite.Engine.Types;

namespace Elite.Engine.Missions
{
    internal sealed class Mission
    {
        private readonly string[] _mission1_pdesc =
        {
            "THE CONSTRICTOR WAS LAST SEEN AT REESDICE, COMMANDER.",
            "A STRANGE LOOKING SHIP LEFT HERE A WHILE BACK. LOOKED BOUND FOR AREXE.",
            "YEP, AN UNUSUAL NEW SHIP HAD A GALACTIC HYPERDRIVE FITTED HERE, USED IT TOO.",
            "I HEAR A WEIRD LOOKING SHIP WAS SEEN AT ERRIUS.",
            "THIS STRANGE SHIP DEHYPED HERE FROM NOWHERE, SUN SKIMMED AND JUMPED. I HEAR IT WENT TO INBIBE.",
            "ROGUE SHIP WENT FOR ME AT AUSAR. MY LASERS DIDN'T EVEN SCRATCH ITS HULL.",
            "OH DEAR ME YES. A FRIGHTFUL ROGUE WITH WHAT I BELIEVE YOU PEOPLE CALL A LEAD " +
                "POSTERIOR SHOT UP LOTS OF THOSE BEASTLY PIRATES AND WENT TO USLERI.",
            "YOU CAN TACKLE THE VICIOUS SCOUNDREL IF YOU LIKE. HE'S AT ORARRA.",
            "THERE'S A REAL DEADLY PIRATE OUT THERE.",
            "BOY ARE YOU IN THE WRONG GALAXY!",
            "COMING SOON: ELITE - DARKNESS FALLS.",
        };

        private readonly Planet _planet;

        internal Mission(Planet planet) => _planet = planet;
        internal string? MissionPlanetDescription(GameState gameState, GalaxySeed planet)
        {
            if (!gameState.IsDocked)
            {
                return null;
            }

            if (planet.A != gameState.DockedPlanet.A ||
                planet.B != gameState.DockedPlanet.B ||
                planet.C != gameState.DockedPlanet.C ||
                planet.D != gameState.DockedPlanet.D ||
                planet.E != gameState.DockedPlanet.E ||
                planet.F != gameState.DockedPlanet.F)
            {
                return null;
            }

            int pnum = _planet.FindPlanetNumber(gameState.Cmdr.Galaxy, planet);

            if (gameState.Cmdr.GalaxyNumber == 0)
            {
                switch (pnum)
                {
                    case 150:
                        return _mission1_pdesc[0];

                    case 36:
                        return _mission1_pdesc[1];

                    case 28:
                        return _mission1_pdesc[2];
                    default:
                        break;
                }
            }

            if (gameState.Cmdr.GalaxyNumber == 1)
            {
                switch (pnum)
                {
                    case 32:
                    case 68:
                    case 164:
                    case 220:
                    case 106:
                    case 16:
                    case 162:
                    case 3:
                    case 107:
                    case 26:
                    case 192:
                    case 184:
                    case 5:
                        return _mission1_pdesc[3];

                    case 253:
                        return _mission1_pdesc[4];

                    case 79:
                        return _mission1_pdesc[5];

                    case 53:
                        return _mission1_pdesc[6];

                    case 118:
                        return _mission1_pdesc[7];

                    case 193:
                        return _mission1_pdesc[8];
                    default:
                        break;
                }
            }

            return gameState.Cmdr.GalaxyNumber == 2 && pnum == 101 ? _mission1_pdesc[9] : null;
        }
    }
}
