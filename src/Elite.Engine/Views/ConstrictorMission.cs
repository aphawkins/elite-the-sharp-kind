namespace Elite.Engine.Views
{
    using System.Numerics;
    using Elite.Engine.Enums;

    internal class ConstrictorMission : IView
    {
        private readonly GameState _gameState;
        private readonly IGfx _gfx;
        private readonly Draw _draw;
        private readonly IKeyboard _keyboard;

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

        internal ConstrictorMission(GameState gameState, IGfx gfx, Draw draw, IKeyboard keyboard)
        {
            _gameState = gameState;
            _gfx = gfx;
            _draw = draw;
            _keyboard = keyboard;
        }

        public void Reset()
        {
            if (elite.cmdr.mission == 0 && elite.cmdr.score >= 256 && elite.cmdr.galaxy_number < 2)
            {
                // Show brief
                elite.cmdr.mission = 1;

                swat.clear_universe();
                int i = swat.add_new_ship(SHIP.SHIP_CONSTRICTOR, new(200, 90, 600), VectorMaths.GetInitialMatrix(), -127, -127);
                space.universe[i].flags = FLG.FLG_NONE;
                _gameState.flight_roll = 0;
                _gameState.flight_climb = 0;
                elite.flight_speed = 0;
            }
            else if (elite.cmdr.mission == 2)
            {
                // Show debrief
                elite.cmdr.mission = 3;
                elite.cmdr.score += 256;
                elite.cmdr.credits += 5000;
            }
            else
            {
                _gameState.SetView(SCR.SCR_MISSION_2);
            }
        }

        public void UpdateUniverse()
        {
        }

        public void Draw()
        {
            if (elite.cmdr.mission == 1)
            {
                _draw.DrawViewHeader("INCOMING MESSAGE");

                _draw.DrawTextPretty(16, 50, 300, mission1_brief_a);
                _draw.DrawTextPretty(16, 200, 470, elite.cmdr.galaxy_number == 0 ? mission1_brief_b : mission1_brief_c);

                _gfx.DrawTextCentre(330, "Press space to continue.", 140, GFX_COL.GFX_COL_GOLD);
            }
            else if (elite.cmdr.mission == 3)
            {
                _draw.DrawViewHeader("INCOMING MESSAGE");

                _gfx.DrawTextCentre(100, "Congratulations Commander!", 140, GFX_COL.GFX_COL_GOLD);

                _draw.DrawTextPretty(116, 132, 400, mission1_debrief);

                _gfx.DrawTextCentre(330, "Press space to continue.", 140, GFX_COL.GFX_COL_GOLD);
            }
        }

        public void HandleInput()
        {
            if (_keyboard.IsKeyPressed(CommandKey.SpaceBar))
            {
                swat.clear_universe();
                _gameState.SetView(SCR.SCR_MISSION_2);
            }
        }
    }
}