// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using Elite.Engine.Conflict;
using Elite.Engine.Enums;
using Elite.Engine.Ships;
using Elite.Engine.Trader;

namespace Elite.Engine.Views
{
    internal sealed class ConstrictorMissionView : IView
    {
        private readonly Combat _combat;
        private readonly Draw _draw;
        private readonly GameState _gameState;
        private readonly IGfx _gfx;
        private readonly IKeyboard _keyboard;

        private readonly string _mission1_brief_a =
            "Greetings Commander, I am Captain Curruthers of " +
            "Her Majesty's Space Navy and I beg a moment of your " +
            "valuable time.  We would like you to do a little job " +
            "for us.  The ship you see here is a new model, the " +
            "Constrictor, equiped with a top secret new shield " +
            "generator.  Unfortunately it's been stolen.";

        private readonly string _mission1_brief_b =
            "It went missing from our ship yard on Xeer five months ago " +
            "and was last seen at Reesdice. Your mission should you decide " +
            "to accept it, is to seek and destroy this ship. You are " +
            "cautioned that only Military Lasers will get through the new " +
            "shields and that the Constrictor is fitted with an E.C.M. " +
            "System. Good Luck, Commander. ---MESSAGE ENDS.";

        private readonly string _mission1_brief_c =
            "It went missing from our ship yard on Xeer five months ago " +
            "and is believed to have jumped to this galaxy. " +
            "Your mission should you decide to accept it, is to seek and " +
            "destroy this ship. You are cautioned that only Military Lasers " +
            "will get through the new shields and that the Constrictor is " +
            "fitted with an E.C.M. System. Good Luck, Commander. ---MESSAGE ENDS.";

        private readonly string _mission1_debrief =
            "There will always be a place for you in Her Majesty's Space Navy. " +
            "And maybe sooner than you think... ---MESSAGE ENDS.";

        private readonly PlayerShip _ship;
        private readonly Trade _trade;

        internal ConstrictorMissionView(GameState gameState, IGfx gfx, Draw draw, IKeyboard keyboard, PlayerShip ship, Trade trade, Combat combat)
        {
            _gameState = gameState;
            _gfx = gfx;
            _draw = draw;
            _keyboard = keyboard;
            _ship = ship;
            _trade = trade;
            _combat = combat;
        }

        public void Draw()
        {
            if (_gameState.Cmdr.Mission == 1)
            {
                _draw.DrawViewHeader("INCOMING MESSAGE");

                _draw.DrawTextPretty(16, 50, 300, _mission1_brief_a);
                _draw.DrawTextPretty(16, 200, 470, _gameState.Cmdr.GalaxyNumber == 0 ? _mission1_brief_b : _mission1_brief_c);

                _gfx.DrawTextCentre(330, "Press space to continue.", 140, GFX_COL.GFX_COL_GOLD);
            }
            else if (_gameState.Cmdr.Mission == 3)
            {
                _draw.DrawViewHeader("INCOMING MESSAGE");

                _gfx.DrawTextCentre(100, "Congratulations Commander!", 140, GFX_COL.GFX_COL_GOLD);

                _draw.DrawTextPretty(116, 132, 400, _mission1_debrief);

                _gfx.DrawTextCentre(330, "Press space to continue.", 140, GFX_COL.GFX_COL_GOLD);
            }
        }

        public void HandleInput()
        {
            if (_keyboard.IsKeyPressed(CommandKey.SpaceBar))
            {
                _combat.ClearUniverse();
                _gameState.SetView(SCR.SCR_MISSION_2);
            }
        }

        public void Reset()
        {
            if (_gameState.Cmdr.Mission == 0 && _gameState.Cmdr.Score >= 256 && _gameState.Cmdr.GalaxyNumber < 2)
            {
                // Show brief
                _gameState.Cmdr.Mission = 1;

                _combat.ClearUniverse();
                int i = _combat.AddNewShip(ShipType.Constrictor, new(200, 90, 600), VectorMaths.GetInitialMatrix(), -127, -127);
                Space.s_universe[i].Flags = ShipFlags.None;
                _ship.Roll = 0;
                _ship.Climb = 0;
                _ship.Speed = 0;
            }
            else if (_gameState.Cmdr.Mission == 2)
            {
                // Show debrief
                _gameState.Cmdr.Mission = 3;
                _gameState.Cmdr.Score += 256;
                _trade._credits += 5000;
            }
            else
            {
                _gameState.SetView(SCR.SCR_MISSION_2);
            }
        }

        public void UpdateUniverse()
        {
        }
    }
}
