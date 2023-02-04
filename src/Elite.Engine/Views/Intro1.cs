namespace Elite.Engine.Views
{
    using System.Numerics;
    using Elite.Common.Enums;
    using Elite.Engine.Enums;

    /// <summary>
    /// Rolling Cobra MkIII.
    /// </summary>
    internal class Intro1 : IView
    {
        private readonly IGfx _gfx;
        private readonly Audio _audio;
        private readonly IKeyboard _keyboard;
        private Vector3[] intro_ship_matrix = new Vector3[3];

        internal Intro1(IGfx gfx, Audio audio, IKeyboard keyboard)
        {
            _gfx = gfx;
            _audio = audio;
            _keyboard = keyboard;
        }

        public void Reset()
        {
            swat.clear_universe();
            VectorMaths.set_init_matrix(ref intro_ship_matrix);
            swat.add_new_ship(SHIP.SHIP_COBRA3, new(0, 0, 4500), intro_ship_matrix, -127, -127);
            elite.flight_roll = 1;            

            _audio.PlayMusic(Music.EliteTheme, true);
        }

        public void UpdateUniverse()
        {
            space.universe[0].location.Z -= 100;

            if (space.universe[0].location.Z < 384)
            {
                space.universe[0].location.Z = 384;
            }
        }

        public void Draw()
        {
            _gfx.DrawImage(Image.EliteText, new(-1, 10));

            _gfx.DrawTextCentre(310, "Original Game (C) I.Bell & D.Braben.", 120, GFX_COL.GFX_COL_WHITE);
            _gfx.DrawTextCentre(330, "Re-engineered by C.J.Pinder.", 120, GFX_COL.GFX_COL_WHITE);
            _gfx.DrawTextCentre(360, "Load New Commander (Y/N)?", 140, GFX_COL.GFX_COL_GOLD);
        }

        public void HandleInput()
        {
            if (_keyboard.IsKeyPressed(CommandKey.Y))
            {
                _audio.StopMusic();
                elite.SetView(SCR.SCR_LOAD_CMDR);
            }
            else if (_keyboard.IsKeyPressed(CommandKey.N))
            {
                _audio.StopMusic();
                elite.SetView(SCR.SCR_INTRO_TWO);
            }
        }
    }
}