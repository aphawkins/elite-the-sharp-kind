namespace Elite.Engine.Views
{
    using Elite.Engine.Enums;
    using Elite.Engine.Save;
    using Elite.Engine.Types;

    internal class LoadCommander : IView
    {
        private readonly IGfx _gfx;
        private readonly IKeyboard _keyboard;
        private string _name = elite.cmdr.name;
        private Commander _cmdr = CommanderFactory.Jameson();

        internal LoadCommander(IGfx gfx, IKeyboard keyboard)
        {
            _gfx = gfx;
            _keyboard = keyboard;
        }

        public void Draw()
        {
            elite.draw.ClearDisplay();
            elite.draw.DrawViewHeader("LOAD COMMANDER");

            _gfx.DrawTextCentre(75, "Please enter commander name:", 120, GFX_COL.GFX_COL_WHITE);
            _gfx.DrawRectangle(100, 100, 312, 50, GFX_COL.GFX_COL_WHITE);
            _gfx.DrawTextCentre(125, _name, 140, GFX_COL.GFX_COL_WHITE);

            if (_cmdr == null)
            {
                _gfx.DrawTextCentre(175, "Error Loading Commander!", 140, GFX_COL.GFX_COL_GOLD);
                _gfx.DrawTextCentre(200, "Press SPACE to continue.", 120, GFX_COL.GFX_COL_WHITE);
            }
        }

        public void HandleInput()
        {
            if (_keyboard.IsKeyPressed(CommandKey.Backspace))
            {
                if (!string.IsNullOrEmpty(_name))
                {
                    _name = _name[..^1]; ;
                }
            }

            int key = _keyboard.GetKeyPressed();

            if (key is >= 'A' and <= 'Z')
            {
                _name += (char)key;
            }

            if (_keyboard.IsKeyPressed(CommandKey.Enter))
            {
                _cmdr = SaveFile.LoadCommanderAsync(_name).Result;
                if (_cmdr != null)
                {
                    elite.saved_cmdr = (Commander)_cmdr.Clone();
                    elite.restore_saved_commander();
                    elite.SetView(SCR.SCR_CMDR_STATUS);
                }
            }

            if (_keyboard.IsKeyPressed(CommandKey.SpaceBar))
            {
                elite.SetView(SCR.SCR_CMDR_STATUS);
            }
        }

        public void Reset()
        {
            _keyboard.ClearKeyPressed();
        }

        public void UpdateUniverse()
        {
        }
    }
}
