using Elite.Engine.Enums;
using Elite.Engine.Save;
using Elite.Engine.Types;
using Elite.Engine.Views;

namespace Elite.Engine.Views
{
    using Elite.Engine.Enums;
    using Elite.Engine.Save;
    using Elite.Engine.Types;

    internal class SaveCommander : IView
    {
        private readonly IGfx _gfx;
        private readonly IKeyboard _keyboard;
        private string _name = elite.cmdr.name;
        private bool? _isSuccess = null;

        internal SaveCommander(IGfx gfx, IKeyboard keyboard)
        {
            _gfx = gfx;
            _keyboard = keyboard;
        }

        public void Draw()
        {
            elite.draw.ClearDisplay();
            elite.draw.DrawViewHeader("SAVE COMMANDER");

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
                elite.cmdr.name = _name;
                elite.cmdr.ShipLocationX = elite.docked_planet.d;
                elite.cmdr.ShipLocationY = elite.docked_planet.b;
                _isSuccess = SaveFile.SaveCommanderAsync(elite.cmdr).Result;
                
                if (_isSuccess.HasValue && _isSuccess.Value)
                {
                    elite.saved_cmdr = (Commander)elite.cmdr.Clone();
                }
                else
                {
                    elite.cmdr.name = elite.saved_cmdr.name;
                }
            }

            if (_keyboard.IsKeyPressed(CommandKey.SpaceBar))
            {
                elite.SetView(SCR.SCR_OPTIONS);
            }
        }

        public void Reset()
        {
            _isSuccess = null;
        }

        public void UpdateUniverse()
        {
        }
    }
}