namespace Elite.Engine.Views
{
    using Elite.Engine.Enums;

    internal class Quit : IView
    {
        private readonly IGfx _gfx;
        private readonly IKeyboard _keyboard;

        internal Quit(IGfx gfx, IKeyboard keyboard) 
        {
            _gfx = gfx;
            _keyboard = keyboard;
        }

        public void Draw()
        {
            elite.draw.ClearDisplay();
            elite.draw.DrawViewHeader("GAME OPTIONS");

            _gfx.DrawTextCentre(175, "QUIT GAME (Y/N)?", 140, GFX_COL.GFX_COL_GOLD);
        }

        public void HandleInput()
        {
            if (_keyboard.IsKeyPressed(CommandKey.Y))
            {
                elite.FinishGame();
            }

            if (_keyboard.IsKeyPressed(CommandKey.N))
            {
                if (elite.docked)
                {
                    elite.SetView(SCR.SCR_CMDR_STATUS);
                }
                else
                {
                    elite.SetView(SCR.SCR_FRONT_VIEW);
                }
            }
        }

        public void Reset()
        {
        }

        public void UpdateUniverse()
        {
        }
    }
}
