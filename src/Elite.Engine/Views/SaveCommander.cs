namespace Elite.Engine.Views
{
    using Elite.Engine.Enums;
    using Elite.Engine.Save;
    using Elite.Engine.Types;

    internal class SaveCommander : IView
    {
        private readonly GameState _gameState;
        private readonly IGfx _gfx;
        private readonly Draw _draw;
        private readonly IKeyboard _keyboard;
        private string _name;
        private bool? _isSuccess = null;

        internal SaveCommander(GameState gameState, IGfx gfx, Draw draw, IKeyboard keyboard)
        {
            _gameState = gameState;
            _gfx = gfx;
            _draw = draw;
            _keyboard = keyboard;
        }

        public void Draw()
        {
            _draw.ClearDisplay();
            _draw.DrawViewHeader("SAVE COMMANDER");

            _gfx.DrawTextCentre(75, "Please enter commander name:", 120, GFX_COL.GFX_COL_WHITE);
            _gfx.DrawRectangle(100, 100, 312, 50, GFX_COL.GFX_COL_WHITE);
            _gfx.DrawTextCentre(125, _name, 140, GFX_COL.GFX_COL_WHITE);

            if (_isSuccess.HasValue)
            {
                if (_isSuccess.Value)
                {
                    _gfx.DrawTextCentre(175, "Commander Saved.", 140, GFX_COL.GFX_COL_GOLD);
                    _gfx.DrawTextCentre(200, "Press SPACE to continue.", 120, GFX_COL.GFX_COL_WHITE);
                }
                else
                {
                    _gfx.DrawTextCentre(175, "Error Saving Commander!", 140, GFX_COL.GFX_COL_GOLD);
                    _gfx.DrawTextCentre(200, "Press SPACE to continue.", 120, GFX_COL.GFX_COL_WHITE);
                }
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
                _gameState.cmdr.name = _name;
                _gameState.cmdr.ShipLocationX = _gameState.docked_planet.d;
                _gameState.cmdr.ShipLocationY = _gameState.docked_planet.b;
                _isSuccess = SaveFile.SaveCommanderAsync(_gameState.cmdr).Result;
                
                if (_isSuccess.HasValue && _isSuccess.Value)
                {
                    _gameState.saved_cmdr = (Commander)_gameState.cmdr.Clone();
                }
                else
                {
                    _gameState.cmdr.name = _gameState.saved_cmdr.name;
                }
            }

            if (_keyboard.IsKeyPressed(CommandKey.SpaceBar))
            {
                _gameState.SetView(SCR.SCR_OPTIONS);
            }
        }

        public void Reset()
        {
            _isSuccess = null;
            _name = _gameState.cmdr.name;
        }

        public void UpdateUniverse()
        {
        }
    }
}