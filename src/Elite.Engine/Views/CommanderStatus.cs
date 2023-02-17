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

    internal class CommanderStatus : IView
    {
        private readonly IGfx _gfx;
        private readonly string[] laserName = new string[] { "Pulse", "Beam", "Military", "Mining", "Custom" };
        int EQUIP_START_Y = 202;
        int Y_INC = 16;
        int EQUIP_MAX_Y = 290;
        int EQUIP_WIDTH = 200;

        string laser_type(int strength)
        {
            return strength switch
            {
                elite.PULSE_LASER => laserName[0],
                elite.BEAM_LASER => laserName[1],
                elite.MILITARY_LASER => laserName[2],
                elite.MINING_LASER => laserName[3],
                _ => laserName[4],
            };
        }

        string[] condition_txt = new string[]
{
                "Docked",
                "Green",
                "Yellow",
                "Red"
};

        (int score, string title)[] ratings = new (int score, string title)[]
        {
                new(0x0000, "Harmless"),
                new(0x0008, "Mostly Harmless"),
                new(0x0010, "Poor"),
                new(0x0020, "Average"),
                new(0x0040, "Above Average"),
                new(0x0080, "Competent"),
                new(0x0200, "Dangerous"),
                new(0x0A00, "Deadly"),
                new(0x1900, "- - - E L I T E - - -")
        };

        internal CommanderStatus(IGfx gfx)
        {
            _gfx = gfx;
        }

        public void Draw()
        {
            elite.draw.ClearDisplay();
            int x = 50;
            int y = EQUIP_START_Y;

            void IncrementPosition()
            {
                y += Y_INC;
                if (y > EQUIP_MAX_Y)
                {
                    y = EQUIP_START_Y;
                    x += EQUIP_WIDTH;
                }
            };

            string rating = string.Empty;
            foreach ((int score, string title) in ratings)
            {
                if (elite.cmdr.score >= score)
                {
                    rating = title;
                }
            }

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

            elite.draw.DrawViewHeader($"COMMANDER {elite.cmdr.name}");
            _gfx.DrawTextLeft(16, 58, "Present System:", GFX_COL.GFX_COL_GREEN_1);

            if (!elite.witchspace)
            {
                _gfx.DrawTextLeft(150, 58, dockedPlanetName, GFX_COL.GFX_COL_WHITE);
            }

            _gfx.DrawTextLeft(16, 74, "Hyperspace System:", GFX_COL.GFX_COL_GREEN_1);
            _gfx.DrawTextLeft(150, 74, hyperspacePlanetName, GFX_COL.GFX_COL_WHITE);

            _gfx.DrawTextLeft(16, 90, "Condition:", GFX_COL.GFX_COL_GREEN_1);
            _gfx.DrawTextLeft(150, 90, condition_txt[condition], GFX_COL.GFX_COL_WHITE);

            _gfx.DrawTextLeft(16, 106, "Fuel:", GFX_COL.GFX_COL_GREEN_1);
            _gfx.DrawTextLeft(150, 106, $"{elite.cmdr.fuel:N1} Light Years", GFX_COL.GFX_COL_WHITE);

            _gfx.DrawTextLeft(16, 122, "Cash:", GFX_COL.GFX_COL_GREEN_1);
            _gfx.DrawTextLeft(150, 122, $"{elite.cmdr.credits:N1} Credits", GFX_COL.GFX_COL_WHITE);

            _gfx.DrawTextLeft(16, 138, "Legal Status:", GFX_COL.GFX_COL_GREEN_1);
            _gfx.DrawTextLeft(150, 138, elite.cmdr.legal_status == 0 ? "Clean" : elite.cmdr.legal_status > 50 ? "Fugitive" : "Offender", GFX_COL.GFX_COL_WHITE);

            _gfx.DrawTextLeft(16, 154, "Rating:", GFX_COL.GFX_COL_GREEN_1);
            _gfx.DrawTextLeft(150, 154, rating, GFX_COL.GFX_COL_WHITE);

            _gfx.DrawTextLeft(16, 186, "EQUIPMENT:", GFX_COL.GFX_COL_GREEN_1);

            if (elite.cmdr.cargo_capacity > 20)
            {
                _gfx.DrawTextLeft(x, y, "Large Cargo Bay", GFX_COL.GFX_COL_WHITE);
                IncrementPosition();
            }

            if (elite.cmdr.escape_pod)
            {
                _gfx.DrawTextLeft(x, y, "Escape Pod", GFX_COL.GFX_COL_WHITE);
                IncrementPosition();
            }

            if (elite.cmdr.fuel_scoop)
            {
                _gfx.DrawTextLeft(x, y, "Fuel Scoops", GFX_COL.GFX_COL_WHITE);
                IncrementPosition();
            }

            if (elite.cmdr.ecm)
            {
                _gfx.DrawTextLeft(x, y, "E.C.M. System", GFX_COL.GFX_COL_WHITE);
                IncrementPosition();
            }

            if (elite.cmdr.energy_bomb)
            {
                _gfx.DrawTextLeft(x, y, "Energy Bomb", GFX_COL.GFX_COL_WHITE);
                IncrementPosition();
            }

            if (elite.cmdr.energy_unit != EnergyUnit.None)
            {
                _gfx.DrawTextLeft(x, y, elite.cmdr.energy_unit == EnergyUnit.Extra ? "Extra Energy Unit" : "Naval Energy Unit", GFX_COL.GFX_COL_WHITE);
                IncrementPosition();
            }

            if (elite.cmdr.docking_computer)
            {
                _gfx.DrawTextLeft(x, y, "Docking Computers", GFX_COL.GFX_COL_WHITE);
                IncrementPosition();
            }

            if (elite.cmdr.galactic_hyperdrive)
            {
                _gfx.DrawTextLeft(x, y, "Galactic Hyperspace", GFX_COL.GFX_COL_WHITE);
                IncrementPosition();
            }

            if (elite.cmdr.front_laser != 0)
            {
                _gfx.DrawTextLeft(x, y, $"Front {laser_type(elite.cmdr.front_laser)} Laser", GFX_COL.GFX_COL_WHITE);
                IncrementPosition();
            }

            if (elite.cmdr.rear_laser != 0)
            {
                _gfx.DrawTextLeft(x, y, $"Rear {laser_type(elite.cmdr.rear_laser)} Laser", GFX_COL.GFX_COL_WHITE);
                IncrementPosition();
            }

            if (elite.cmdr.left_laser != 0)
            {
                _gfx.DrawTextLeft(x, y, $"Left {laser_type(elite.cmdr.left_laser)} Laser", GFX_COL.GFX_COL_WHITE);
                IncrementPosition();
            }

            if (elite.cmdr.right_laser != 0)
            {
                _gfx.DrawTextLeft(x, y, $"Right {laser_type(elite.cmdr.right_laser)} Laser", GFX_COL.GFX_COL_WHITE);
            }
        }

        public void HandleInput()
        {
        }

        public void Reset()
        {
        }

        public void UpdateUniverse()
        {
        }
    }
}