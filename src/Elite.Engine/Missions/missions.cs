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

/*
 * missions.c
 *
 * Code to handle the special missions.
 */

namespace Elite.Engine.Missions
{
    using Elite.Engine.Types;

    internal class Mission
    {
        private static readonly string[] mission1_pdesc =
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

        internal string? mission_planet_desc(galaxy_seed planet)
        {
            int pnum;

            if (!elite.docked)
            {
                return null;
            }

            if (planet.a != elite.docked_planet.a ||
                planet.b != elite.docked_planet.b ||
                planet.c != elite.docked_planet.c ||
                planet.d != elite.docked_planet.d ||
                planet.e != elite.docked_planet.e ||
                planet.f != elite.docked_planet.f)
            {
                return null;
            }

            pnum = Planet.find_planet_number(planet);

            if (elite.cmdr.galaxy_number == 0)
            {
                switch (pnum)
                {
                    case 150:
                        return mission1_pdesc[0];

                    case 36:
                        return mission1_pdesc[1];

                    case 28:
                        return mission1_pdesc[2];
                }
            }

            if (elite.cmdr.galaxy_number == 1)
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
                        return mission1_pdesc[3];

                    case 253:
                        return mission1_pdesc[4];

                    case 79:
                        return mission1_pdesc[5];

                    case 53:
                        return mission1_pdesc[6];

                    case 118:
                        return mission1_pdesc[7];

                    case 193:
                        return mission1_pdesc[8];
                }
            }

            if (elite.cmdr.galaxy_number == 2 && pnum == 101)
            {
                return mission1_pdesc[9];
            }

            return null;
        }
    }
}