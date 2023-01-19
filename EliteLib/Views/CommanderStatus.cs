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

namespace Elite.Views
{
    using Elite;
    using Elite.Enums;

    internal static class CommanderStatus
    {
        private static string[] condition_txt = new string[]
        {
            "Docked",
            "Green",
            "Yellow",
            "Red"
        };
        private static (int score, string title)[] ratings = new (int score, string title)[]
        {
            new(0x0000, "Harmless"),
            new(0x0008, "Mostly Harmless"),
            new(0x0010, "Poor"),
            new(0x0020, "Average"),
            new(0x0040, "Above Average"),
            new(0x0080, "Competent"),
            new(0x0200, "Dangerous"),
            new(0x0A00, "Deadly"),
            new(0x1900, "---- E L I T E ---")
        };
        private const int EQUIP_START_Y = 202;
        private const int EQUIP_START_X = 50;
        private const int EQUIP_MAX_Y = 290;
        private const int EQUIP_WIDTH = 200;
        private const int Y_INC = 16;
        private static string[] laser_name = new string[5] { "Pulse", "Beam", "Military", "Mining", "Custom" };

        private static string laser_type(int strength)
        {
            return strength switch
            {
                elite.PULSE_LASER => laser_name[0],
                elite.BEAM_LASER => laser_name[1],
                elite.MILITARY_LASER => laser_name[2],
                elite.MINING_LASER => laser_name[3],
                _ => laser_name[4],
            };
        }

        internal static void display_commander_status()
        {
            string planet_name;
            string str;
            int i;
            int x, y;
            int condition;
            SHIP type;

            elite.current_screen = SCR.SCR_CMDR_STATUS;

            elite.alg_gfx.ClearDisplay();

            str = "COMMANDER " + elite.cmdr.name;

            elite.alg_gfx.DrawTextCentre(20, str, 140, GFX_COL.GFX_COL_GOLD);

            elite.alg_gfx.DrawLine(0, 36, 511, 36);

            elite.alg_gfx.DrawTextLeft(16, 58, "Present System:", GFX_COL.GFX_COL_GREEN_1);

            if (!elite.witchspace)
            {
                planet_name = Planet.name_planet(elite.docked_planet);
                planet_name = Planet.capitalise_name(planet_name);
                str = planet_name;
                elite.alg_gfx.DrawTextLeft(190, 58, str, GFX_COL.GFX_COL_WHITE);
            }

            elite.alg_gfx.DrawTextLeft(16, 74, "Hyperspace System:", GFX_COL.GFX_COL_GREEN_1);
            planet_name = Planet.name_planet(elite.hyperspace_planet);
            planet_name = Planet.capitalise_name(planet_name);
            str = planet_name;
            elite.alg_gfx.DrawTextLeft(190, 74, str, GFX_COL.GFX_COL_WHITE);

            if (elite.docked)
            {
                condition = 0;
            }
            else
            {
                condition = 1;

                for (i = 0; i < elite.MAX_UNIV_OBJECTS; i++)
                {
                    type = space.universe[i].type;

                    if (type is SHIP.SHIP_MISSILE or (> SHIP.SHIP_ROCK and < SHIP.SHIP_DODEC))
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

            elite.alg_gfx.DrawTextLeft(16, 90, "Condition:", GFX_COL.GFX_COL_GREEN_1);
            elite.alg_gfx.DrawTextLeft(190, 90, condition_txt[condition], GFX_COL.GFX_COL_WHITE);

            str = $"{elite.cmdr.fuel / 10:D}.{elite.cmdr.fuel % 10:D} Light Years";
            elite.alg_gfx.DrawTextLeft(16, 106, "Fuel:", GFX_COL.GFX_COL_GREEN_1);
            elite.alg_gfx.DrawTextLeft(70, 106, str, GFX_COL.GFX_COL_WHITE);

            str = $"{elite.cmdr.credits / 10:D}.{elite.cmdr.credits % 10:D} Cr";
            elite.alg_gfx.DrawTextLeft(16, 122, "Cash:", GFX_COL.GFX_COL_GREEN_1);
            elite.alg_gfx.DrawTextLeft(70, 122, str, GFX_COL.GFX_COL_WHITE);

            str = elite.cmdr.legal_status == 0 ? "Clean" : elite.cmdr.legal_status > 50 ? "Fugitive" : "Offender";

            elite.alg_gfx.DrawTextLeft(16, 138, "Legal Status:", GFX_COL.GFX_COL_GREEN_1);
            elite.alg_gfx.DrawTextLeft(128, 138, str, GFX_COL.GFX_COL_WHITE);

            foreach ((int score, string title) in ratings)
            {
                if (elite.cmdr.score >= score)
                {
                    str = title;
                }
            }

            elite.alg_gfx.DrawTextLeft(16, 154, "Rating:", GFX_COL.GFX_COL_GREEN_1);
            elite.alg_gfx.DrawTextLeft(80, 154, str, GFX_COL.GFX_COL_WHITE);

            elite.alg_gfx.DrawTextLeft(16, 186, "EQUIPMENT:", GFX_COL.GFX_COL_GREEN_1);

            x = EQUIP_START_X;
            y = EQUIP_START_Y;

            if (elite.cmdr.cargo_capacity > 20)
            {
                elite.alg_gfx.DrawTextLeft(x, y, "Large Cargo Bay", GFX_COL.GFX_COL_WHITE);
                y += Y_INC;
            }

            if (elite.cmdr.escape_pod)
            {
                elite.alg_gfx.DrawTextLeft(x, y, "Escape Pod", GFX_COL.GFX_COL_WHITE);
                y += Y_INC;
            }

            if (elite.cmdr.fuel_scoop)
            {
                elite.alg_gfx.DrawTextLeft(x, y, "Fuel Scoops", GFX_COL.GFX_COL_WHITE);
                y += Y_INC;
            }

            if (elite.cmdr.ecm)
            {
                elite.alg_gfx.DrawTextLeft(x, y, "E.C.M. System", GFX_COL.GFX_COL_WHITE);
                y += Y_INC;
            }

            if (elite.cmdr.energy_bomb)
            {
                elite.alg_gfx.DrawTextLeft(x, y, "Energy Bomb", GFX_COL.GFX_COL_WHITE);
                y += Y_INC;
            }

            if (elite.cmdr.energy_unit != 0)
            {
                elite.alg_gfx.DrawTextLeft(x, y, elite.cmdr.energy_unit == 1 ? "Extra Energy Unit" : "Naval Energy Unit", GFX_COL.GFX_COL_WHITE);
                y += Y_INC;
                if (y > EQUIP_MAX_Y)
                {
                    y = EQUIP_START_Y;
                    x += EQUIP_WIDTH;
                }
            }

            if (elite.cmdr.docking_computer)
            {
                elite.alg_gfx.DrawTextLeft(x, y, "Docking Computers", GFX_COL.GFX_COL_WHITE);
                y += Y_INC;
                if (y > EQUIP_MAX_Y)
                {
                    y = EQUIP_START_Y;
                    x += EQUIP_WIDTH;
                }
            }


            if (elite.cmdr.galactic_hyperdrive)
            {
                elite.alg_gfx.DrawTextLeft(x, y, "Galactic Hyperspace", GFX_COL.GFX_COL_WHITE);
                y += Y_INC;
                if (y > EQUIP_MAX_Y)
                {
                    y = EQUIP_START_Y;
                    x += EQUIP_WIDTH;
                }
            }

            if (elite.cmdr.front_laser != 0)
            {
                str = $"Front {laser_type(elite.cmdr.front_laser)} Laser";
                elite.alg_gfx.DrawTextLeft(x, y, str, GFX_COL.GFX_COL_WHITE);
                y += Y_INC;
                if (y > EQUIP_MAX_Y)
                {
                    y = EQUIP_START_Y;
                    x += EQUIP_WIDTH;
                }
            }

            if (elite.cmdr.rear_laser != 0)
            {
                str = $"Rear {laser_type(elite.cmdr.rear_laser)} Laser";
                elite.alg_gfx.DrawTextLeft(x, y, str, GFX_COL.GFX_COL_WHITE);
                y += Y_INC;
                if (y > EQUIP_MAX_Y)
                {
                    y = EQUIP_START_Y;
                    x += EQUIP_WIDTH;
                }
            }

            if (elite.cmdr.left_laser != 0)
            {
                str = $"Left {laser_type(elite.cmdr.left_laser)} Laser";
                elite.alg_gfx.DrawTextLeft(x, y, str, GFX_COL.GFX_COL_WHITE);
                y += Y_INC;
                if (y > EQUIP_MAX_Y)
                {
                    y = EQUIP_START_Y;
                    x += EQUIP_WIDTH;
                }
            }

            if (elite.cmdr.right_laser != 0)
            {
                str = $"Right {laser_type(elite.cmdr.right_laser)} Laser";
                elite.alg_gfx.DrawTextLeft(x, y, str, GFX_COL.GFX_COL_WHITE);
            }
        }
    }
}