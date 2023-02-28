namespace Elite.Engine.Views
{
    using Elite.Common.Enums;
    using Elite.Engine.Enums;

    internal class ThargoidMission : IView
    {
        private readonly GameState _gameState;
        private readonly IGfx _gfx;
        private readonly IKeyboard _keyboard;

        private static readonly string mission2_brief_a =
            "Attention Commander, I am Captain Fortesque of Her Majesty's Space Navy. " +
            "We have need of your services again. If you would be so good as to go to " +
            "Ceerdi you will be briefed.If succesful, you will be rewarded." +
            "---MESSAGE ENDS.";
        private static readonly string mission2_brief_b =
            "Good Day Commander. I am Agent Blake of Naval Intelligence. As you know, " +
            "the Navy have been keeping the Thargoids off your ass out in deep space " +
            "for many years now. Well the situation has changed. Our boys are ready " +
            "for a push right to the home system of those murderers.";
        private static readonly string mission2_brief_c =
            "I have obtained the defence plans for their Hive Worlds. The beetles " +
            "know we've got something but not what. If I transmit the plans to our " +
            "base on Birera they'll intercept the transmission. I need a ship to " +
            "make the run. You're elected. The plans are unipulse coded within " +
            "this transmission. You will be paid. Good luck Commander. ---MESSAGE ENDS.";
        private static readonly string mission2_debrief =
            "You have served us well and we shall remember. " +
            "We did not expect the Thargoids to find out about you." +
            "For the moment please accept this Navy Extra Energy Unit as payment. " +
            "---MESSAGE ENDS.";

        internal ThargoidMission(GameState gameState, IGfx gfx, IKeyboard keyboard)
        {
            _gameState = gameState;
            _gfx = gfx;
            _keyboard = keyboard;
        }

        public void Reset()
        {
            if (elite.cmdr.mission == 3 && elite.cmdr.score >= 1280 && elite.cmdr.galaxy_number == 2)
            {
                // First brief
                elite.cmdr.mission = 4;
            }
            else if (elite.cmdr.mission == 4 && elite.docked_planet.d == 215 && elite.docked_planet.b == 84)
            {
                // Second brief
                elite.cmdr.mission = 5;
            }
            else if (elite.cmdr.mission == 5 && elite.docked_planet.d == 63 && elite.docked_planet.b == 72)
            {
                // Debrief
                elite.cmdr.mission = 6;
                elite.cmdr.score += 256;
                elite.cmdr.energy_unit = EnergyUnit.Naval;
            }
            else
            {
                _gameState.SetView(SCR.SCR_CMDR_STATUS);
            }
        }

        public void UpdateUniverse()
        {
        }

        public void Draw()
        {
            if (elite.cmdr.mission == 4)
            {
                elite.draw.DrawViewHeader("INCOMING MESSAGE");
                elite.draw.DrawTextPretty(116, 132, 400, mission2_brief_a);
                _gfx.DrawTextCentre(330, "Press space to continue.", 140, GFX_COL.GFX_COL_GOLD);
            }
            else if (elite.cmdr.mission == 5)
            {
                elite.draw.DrawViewHeader("INCOMING MESSAGE");
                elite.draw.DrawTextPretty(16, 50, 300, mission2_brief_b);
                elite.draw.DrawTextPretty(16, 200, 470, mission2_brief_c);
                _gfx.DrawImage(Image.Blake, new(352, 46));
                _gfx.DrawTextCentre(330, "Press space to continue.", 140, GFX_COL.GFX_COL_GOLD);
            }
            else if (elite.cmdr.mission == 6)
            {
                elite.draw.DrawViewHeader("INCOMING MESSAGE");
                _gfx.DrawTextCentre(100, "Well done Commander.", 140, GFX_COL.GFX_COL_GOLD);
                elite.draw.DrawTextPretty(116, 132, 400, mission2_debrief);
                _gfx.DrawTextCentre(330, "Press space to continue.", 140, GFX_COL.GFX_COL_GOLD);
            }
        }

        public void HandleInput()
        {
            if (_keyboard.IsKeyPressed(CommandKey.SpaceBar))
            {
                _gameState.SetView(SCR.SCR_CMDR_STATUS);
            }
        }
    }
}