using Elite.Common.Enums;
using Elite.Engine.Enums;
using Elite.Engine.Ships;

namespace Elite.Engine.Views
{
    /// <summary>
    /// Rolling Cobra MkIII.
    /// </summary>
    internal class Intro1View : IView
    {
        private readonly GameState _gameState;
        private readonly IGfx _gfx;
        private readonly Audio _audio;
        private readonly IKeyboard _keyboard;
        private readonly PlayerShip _ship;
        private readonly Combat _combat;

        internal Intro1View(GameState gameState, IGfx gfx, Audio audio, IKeyboard keyboard, PlayerShip ship, Combat combat)
        {
            _gameState = gameState;
            _gfx = gfx;
            _audio = audio;
            _keyboard = keyboard;
            _ship = ship;
            _combat = combat;
        }

        public void Reset()
        {
            _combat.ClearUniverse();
            _combat.AddNewShip(ShipType.CobraMk3, new(0, 0, 4500), VectorMaths.GetInitialMatrix(), -127, -127);
            _ship.Roll = 1;
            _audio.PlayMusic(Music.EliteTheme, true);
        }

        public void UpdateUniverse()
        {
            Space.universe[0].Location = new(Space.universe[0].Location.X, Space.universe[0].Location.Y, Space.universe[0].Location.Z - 100);

            if (Space.universe[0].Location.Z < 384)
            {
                Space.universe[0].Location = new(Space.universe[0].Location.X, Space.universe[0].Location.Y, 384);
            }
        }

        public void Draw()
        {
            _gfx.DrawImage(Image.EliteText, new(-1, 10));

            _gfx.DrawTextCentre(310, "Original Game (C) I.Bell & D.Braben.", 120, GFX_COL.GFX_COL_WHITE);
            _gfx.DrawTextCentre(330, "The New Kind - Christian Pinder.", 120, GFX_COL.GFX_COL_WHITE);
            _gfx.DrawTextCentre(350, "The Sharp Kind - Andy Hawkins.", 120, GFX_COL.GFX_COL_WHITE);
            _gfx.DrawTextCentre(370, "Load New Commander (Y/N)?", 140, GFX_COL.GFX_COL_GOLD);
        }

        public void HandleInput()
        {
            if (_keyboard.IsKeyPressed(CommandKey.Yes))
            {
                _combat.ClearUniverse();
                _audio.StopMusic();
                _gameState.SetView(SCR.SCR_LOAD_CMDR);
            }
            if (_keyboard.IsKeyPressed(CommandKey.No))
            {
                _combat.ClearUniverse();
                _audio.StopMusic();
                _gameState.SetView(SCR.SCR_INTRO_TWO);
            }
        }
    }
}
