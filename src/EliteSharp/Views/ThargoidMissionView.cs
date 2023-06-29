// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Controls;
using EliteSharp.Equipment;
using EliteSharp.Graphics;
using EliteSharp.Ships;

namespace EliteSharp.Views
{
    internal sealed class ThargoidMissionView : IView
    {
        private const string Mission2BriefA =
            "Attention Commander, I am Captain Fortesque of Her Majesty's Space Navy. " +
            "We have need of your services again. If you would be so good as to go to " +
            "Ceerdi you will be briefed.If succesful, you will be rewarded." +
            "---MESSAGE ENDS.";

        private const string Mission2BriefB =
            "Good Day Commander. I am Agent Blake of Naval Intelligence. As you know, " +
            "the Navy have been keeping the Thargoids off your ass out in deep space " +
            "for many years now. Well the situation has changed. Our boys are ready " +
            "for a push right to the home system of those murderers.";

        private const string Mission2BriefC =
            "I have obtained the defence plans for their Hive Worlds. The beetles " +
            "know we've got something but not what. If I transmit the plans to our " +
            "base on Birera they'll intercept the transmission. I need a ship to " +
            "make the run. You're elected. The plans are unipulse coded within " +
            "this transmission. You will be paid. Good luck Commander. ---MESSAGE ENDS.";

        private const string Mission2Debrief =
            "You have served us well and we shall remember. " +
            "We did not expect the Thargoids to find out about you." +
            "For the moment please accept this Navy Extra Energy Unit as payment. " +
            "---MESSAGE ENDS.";

        private readonly IDraw _draw;
        private readonly GameState _gameState;
        private readonly IGraphics _graphics;
        private readonly IKeyboard _keyboard;
        private readonly PlayerShip _ship;

        internal ThargoidMissionView(GameState gameState, IGraphics graphics, IDraw draw, IKeyboard keyboard, PlayerShip ship)
        {
            _gameState = gameState;
            _graphics = graphics;
            _draw = draw;
            _keyboard = keyboard;
            _ship = ship;
        }

        public void Draw()
        {
            if (_gameState.Cmdr.Mission == 4)
            {
                _draw.DrawViewHeader("INCOMING MESSAGE");
                _draw.DrawTextPretty(new(116, 132), 400, Mission2BriefA);
                _graphics.DrawTextCentre(330, "Press space to continue.", FontSize.Large, Colour.Gold);
            }
            else if (_gameState.Cmdr.Mission == 5)
            {
                _draw.DrawViewHeader("INCOMING MESSAGE");
                _draw.DrawTextPretty(new(16, 50), 300, Mission2BriefB);
                _draw.DrawTextPretty(new(16, 200), 470, Mission2BriefC);
                _graphics.DrawImage(Image.Blake, new(352, 46));
                _graphics.DrawTextCentre(330, "Press space to continue.", FontSize.Large, Colour.Gold);
            }
            else if (_gameState.Cmdr.Mission == 6)
            {
                _draw.DrawViewHeader("INCOMING MESSAGE");
                _graphics.DrawTextCentre(100, "Well done Commander.", FontSize.Large, Colour.Gold);
                _draw.DrawTextPretty(new(116, 132), 400, Mission2Debrief);
                _graphics.DrawTextCentre(330, "Press space to continue.", FontSize.Large, Colour.Gold);
            }
        }

        public void HandleInput()
        {
            if (_keyboard.IsKeyPressed(CommandKey.SpaceBar))
            {
                _gameState.SetView(Screen.CommanderStatus);
            }
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
                _gameState.SetView(Screen.CommanderStatus);
            }
        }

        public void UpdateUniverse()
        {
        }
    }
}
