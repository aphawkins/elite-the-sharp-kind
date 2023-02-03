namespace Elite.Engine.Missions
{
    using System.Numerics;
    using Elite.Engine.Enums;

    internal class ConstrictorMission // : IView
    {
        IGfx _gfx;
        space _space;

        private static readonly string mission1_brief_a =
            "Greetings Commander, I am Captain Curruthers of " +
            "Her Majesty's Space Navy and I beg a moment of your " +
            "valuable time.  We would like you to do a little job " +
            "for us.  The ship you see here is a new model, the " +
            "Constrictor, equiped with a top secret new shield " +
            "generator.  Unfortunately it's been stolen.";

        private static readonly string mission1_brief_b =
            "It went missing from our ship yard on Xeer five months ago " +
            "and was last seen at Reesdice. Your mission should you decide " +
            "to accept it, is to seek and destroy this ship. You are " +
            "cautioned that only Military Lasers will get through the new " +
            "shields and that the Constrictor is fitted with an E.C.M. " +
            "System. Good Luck, Commander. ---MESSAGE ENDS.";

        private static readonly string mission1_brief_c =
            "It went missing from our ship yard on Xeer five months ago " +
            "and is believed to have jumped to this galaxy. " +
            "Your mission should you decide to accept it, is to seek and " +
            "destroy this ship. You are cautioned that only Military Lasers " +
            "will get through the new shields and that the Constrictor is " +
            "fitted with an E.C.M. System. Good Luck, Commander. ---MESSAGE ENDS.";

        private static readonly string mission1_debrief =
            "There will always be a place for you in Her Majesty's Space Navy. " +
            "And maybe sooner than you think... ---MESSAGE ENDS.";

        internal ConstrictorMission(IGfx gfx, space space)
        {
            _gfx = gfx;
            _space = space;
        }

        internal void check_mission_brief()
        {
            if ((elite.cmdr.mission == 0) && (elite.cmdr.score >= 256) && (elite.cmdr.galaxy_number < 2))
            {
                constrictor_mission_brief();
                return;
            }

            if (elite.cmdr.mission == 2)
            {
                constrictor_mission_debrief();
                return;
            }
        }

        private void constrictor_mission_brief()
        {
            Vector3[] rotmat = new Vector3[3];

            elite.cmdr.mission = 1;

            //elite.current_screen = SCR.SCR_FRONT_VIEW;

            elite.draw.ClearDisplay();
            _gfx.DrawTextCentre(20, "INCOMING MESSAGE", 140, GFX_COL.GFX_COL_GOLD);
            _gfx.DrawLine(new(0f, 36f), new(511f, 36f));

            elite.draw.DrawTextPretty(16, 50, 300, mission1_brief_a);
            elite.draw.DrawTextPretty(16, 200, 470, (elite.cmdr.galaxy_number == 0) ? mission1_brief_b : mission1_brief_c);

            _gfx.DrawTextCentre(330, "Press space to continue.", 140, GFX_COL.GFX_COL_GOLD);

            swat.clear_universe();
            VectorMaths.set_init_matrix(ref rotmat);
            swat.add_new_ship(SHIP.SHIP_CONSTRICTOR, new(200, 90, 600), rotmat, -127, -127);
            elite.flight_roll = 0;
            elite.flight_climb = 0;
            elite.flight_speed = 0;

            do
            {
                _gfx.ClearArea(310, 50, 510, 180);
                _space.update_universe();
                space.universe[0].location.Z = 600;
                _gfx.ScreenUpdate();
            } while (!elite.keyboard.IsKeyPressed(CommandKey.Space));
        }

        private void constrictor_mission_debrief()
        {
            elite.cmdr.mission = 3;
            elite.cmdr.score += 256;
            elite.cmdr.credits += 5000;

            elite.draw.ClearDisplay();
            _gfx.DrawTextCentre(20, "INCOMING MESSAGE", 140, GFX_COL.GFX_COL_GOLD);
            _gfx.DrawLine(new(0f, 36f), new(511f, 36f));

            _gfx.DrawTextCentre(100, "Congratulations Commander!", 140, GFX_COL.GFX_COL_GOLD);

            elite.draw.DrawTextPretty(116, 132, 400, mission1_debrief);

            _gfx.DrawTextCentre(330, "Press space to continue.", 140, GFX_COL.GFX_COL_GOLD);

            _gfx.ScreenUpdate();

            do
            {
            } while (!elite.keyboard.IsKeyPressed(CommandKey.Space));
        }
    }
}