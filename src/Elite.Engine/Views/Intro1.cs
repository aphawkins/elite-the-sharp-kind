// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using Elite.Common.Enums;
using Elite.Engine.Conflict;
using Elite.Engine.Enums;
using Elite.Engine.Ships;

namespace Elite.Engine.Views
{
    /// <summary>
    /// Rolling Cobra MkIII.
    /// </summary>
    internal sealed class Intro1View : IView
    {
        private readonly Audio _audio;
        private readonly Combat _combat;
        private readonly GameState _gameState;
        private readonly IGfx _gfx;
        private readonly IKeyboard _keyboard;
        private readonly PlayerShip _ship;

        internal Intro1View(GameState gameState, IGfx gfx, Audio audio, IKeyboard keyboard, PlayerShip ship, Combat combat)
        {
            _gameState = gameState;
            _gfx = gfx;
            _audio = audio;
            _keyboard = keyboard;
            _ship = ship;
            _combat = combat;
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

        public void Reset()
        {
            _combat.ClearUniverse();
            _combat.AddNewShip(ShipType.CobraMk3, new(0, 0, 4500), VectorMaths.GetInitialMatrix(), -127, -127);
            _ship.Roll = 1;
            _audio.PlayMusic(Music.EliteTheme, true);
        }

        public void UpdateUniverse()
        {
            Space.s_universe[0].Location = new(Space.s_universe[0].Location.X, Space.s_universe[0].Location.Y, Space.s_universe[0].Location.Z - 100);

            if (Space.s_universe[0].Location.Z < 384)
            {
                Space.s_universe[0].Location = new(Space.s_universe[0].Location.X, Space.s_universe[0].Location.Y, 384);
            }
        }
    }
}
