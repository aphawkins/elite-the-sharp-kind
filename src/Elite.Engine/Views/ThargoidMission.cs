using Elite.Common.Enums;
using Elite.Engine.Enums;
using Elite.Engine.Ships;

namespace Elite.Engine.Views
{
    internal sealed class ThargoidMissionView : IView
    {
        private readonly GameState _gameState;
        private readonly IGfx _gfx;
        private readonly Draw _draw;
        private readonly IKeyboard _keyboard;
        private readonly PlayerShip _ship;

        private const string s_mission2BriefA =
            "Attention Commander, I am Captain Fortesque of Her Majesty's Space Navy. " +
            "We have need of your services again. If you would be so good as to go to " +
            "Ceerdi you will be briefed.If succesful, you will be rewarded." +
            "---MESSAGE ENDS.";
        private const string s_mission2BriefB =
            "Good Day Commander. I am Agent Blake of Naval Intelligence. As you know, " +
            "the Navy have been keeping the Thargoids off your ass out in deep space " +
            "for many years now. Well the situation has changed. Our boys are ready " +
            "for a push right to the home system of those murderers.";
        private const string s_mission2BriefC =
            "I have obtained the defence plans for their Hive Worlds. The beetles " +
            "know we've got something but not what. If I transmit the plans to our " +
            "base on Birera they'll intercept the transmission. I need a ship to " +
            "make the run. You're elected. The plans are unipulse coded within " +
            "this transmission. You will be paid. Good luck Commander. ---MESSAGE ENDS.";
        private const string s_mission2Debrief =
            "You have served us well and we shall remember. " +
            "We did not expect the Thargoids to find out about you." +
            "For the moment please accept this Navy Extra Energy Unit as payment. " +
            "---MESSAGE ENDS.";

        internal ThargoidMissionView(GameState gameState, IGfx gfx, Draw draw, IKeyboard keyboard, PlayerShip ship)
        {
            _gameState = gameState;
            _gfx = gfx;
            _draw = draw;
            _keyboard = keyboard;
            _ship = ship;
        }

        public void Reset()
        {
            if (_gameState.Cmdr.Mission == 3 && _gameState.Cmdr.Score >= 1280 && _gameState.Cmdr.GalaxyNumber == 2)
            {
                // First brief
                _gameState.Cmdr.Mission = 4;
            }
            else if (_gameState.Cmdr.Mission == 4 && _gameState.DockedPlanet.D == 215 && _gameState.DockedPlanet.B == 84)
            {
                // Second brief
                _gameState.Cmdr.Mission = 5;
            }
            else if (_gameState.Cmdr.Mission == 5 && _gameState.DockedPlanet.D == 63 && _gameState.DockedPlanet.B == 72)
            {
                // Debrief
                _gameState.Cmdr.Mission = 6;
                _gameState.Cmdr.Score += 256;
                _ship.EnergyUnit = EnergyUnit.Naval;
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
            if (_gameState.Cmdr.Mission == 4)
            {
                _draw.DrawViewHeader("INCOMING MESSAGE");
                _draw.DrawTextPretty(116, 132, 400, s_mission2BriefA);
                _gfx.DrawTextCentre(330, "Press space to continue.", 140, GFX_COL.GFX_COL_GOLD);
            }
            else if (_gameState.Cmdr.Mission == 5)
            {
                _draw.DrawViewHeader("INCOMING MESSAGE");
                _draw.DrawTextPretty(16, 50, 300, s_mission2BriefB);
                _draw.DrawTextPretty(16, 200, 470, s_mission2BriefC);
                _gfx.DrawImage(Image.Blake, new(352, 46));
                _gfx.DrawTextCentre(330, "Press space to continue.", 140, GFX_COL.GFX_COL_GOLD);
            }
            else if (_gameState.Cmdr.Mission == 6)
            {
                _draw.DrawViewHeader("INCOMING MESSAGE");
                _gfx.DrawTextCentre(100, "Well done Commander.", 140, GFX_COL.GFX_COL_GOLD);
                _draw.DrawTextPretty(116, 132, 400, s_mission2Debrief);
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
