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
    using Elite.Engine.Enums;

    internal static class CommanderStatus
    {
        internal static void display_commander_status()
        {
            elite.SetView(SCR.SCR_CMDR_STATUS);

            string dockedPlanetName = Planet.name_planet(elite.docked_planet, true);
            string hyperspacePlanetName = Planet.name_planet(elite.hyperspace_planet, true);
           
            int condition = 0;

            if (!elite.docked)
            {
                condition = 1;

                for (int i = 0; i < elite.MAX_UNIV_OBJECTS; i++)
                {
                    if (space.universe[i].type is SHIP.SHIP_MISSILE or (> SHIP.SHIP_ROCK and < SHIP.SHIP_DODEC))
                    {
                        condition = 2;
                        break;
                    }
                }

                if (condition == 2 && elite.energy < 128)
                {
                    condition = 3;
                }
            }

            elite.draw.DrawCommanderStatus(elite.cmdr.name,
                elite.witchspace,
                dockedPlanetName,
                hyperspacePlanetName,
                condition,
                elite.cmdr);
        }
    }
}