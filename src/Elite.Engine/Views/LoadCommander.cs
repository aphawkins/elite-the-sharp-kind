namespace Elite.Engine.Views
{
    using Elite.Engine.Enums;
    using Elite.Engine.Save;

    internal class LoadCommander : IView
    {
        private readonly GameState _gameState;
        private readonly IGfx _gfx;
        private readonly Draw _draw;
        private readonly IKeyboard _keyboard;
        private readonly SaveFile _save;
        private string _name;
        private bool isLoaded = true;

        internal LoadCommander(GameState gameState, IGfx gfx, Draw draw, IKeyboard keyboard, SaveFile save)
        {
            _gameState = gameState;
            _gfx = gfx;
            _draw = draw;
            _keyboard = keyboard;
            _save = save;
        }

        public void Draw()
        {
            _draw.ClearDisplay();
            _draw.DrawViewHeader("LOAD COMMANDER");

            _gfx.DrawTextCentre(75, "Please enter commander name:", 120, GFX_COL.GFX_COL_WHITE);
            _gfx.DrawRectangle(100, 100, 312, 50, GFX_COL.GFX_COL_WHITE);
            _gfx.DrawTextCentre(125, _name, 140, GFX_COL.GFX_COL_WHITE);

            if (!isLoaded)
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

            char key = (char)_keyboard.GetKeyPressed();

            if (key is >= 'A' and <= 'Z')
            {
                _name += key;
            }

            if (_keyboard.IsKeyPressed(CommandKey.Enter))
            {
                isLoaded = _save.LoadCommanderAsync(_name).Result;
                if (isLoaded)
                {
                    _save.GetLastSave();
                    _gameState.SetView(SCR.SCR_CMDR_STATUS);
                }
            }

            if (_keyboard.IsKeyPressed(CommandKey.SpaceBar))
            {
                _gameState.SetView(SCR.SCR_CMDR_STATUS);
            }
        }

        public void Reset()
        {
            _keyboard.ClearKeyPressed();
            _name = _gameState.cmdr.Name;
            isLoaded = true;
        }

        public void UpdateUniverse()
        {
        }
    }
}
