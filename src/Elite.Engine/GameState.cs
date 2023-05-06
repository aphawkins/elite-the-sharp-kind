/*
 * Elite - The New Kind.
 *
 * Reverse engineered from the BBC disk version of Elite.
 * Additional material by C.J.Pinder.
 *
 * The original Elite code is (C) I.Bell & D.Braben 1984.
 * This version re-engineered in C by C.J.Pinder 1999-2001.
 *
 * email: <christian@newkind.co.uk>
 *
 *
 */

using Elite.Engine.Enums;
using Elite.Engine.Types;

namespace Elite.Engine
{
    internal class GameState
    {
        private readonly IKeyboard _keyboard;
        private readonly Dictionary<SCR, IView> _views;

        internal bool IsGameOver { get; private set; } = false;
        internal bool IsInitialised { get; set; } = false;
        internal SCR currentScreen = SCR.SCR_NONE;
        internal IView? currentView;

        internal bool witchspace;
        internal Commander cmdr = new();
        internal GalaxySeed docked_planet = new();
        internal string planetName = string.Empty;
        internal GalaxySeed hyperspace_planet = new();
        internal PlanetData current_planet_data = new();
        internal int carry_flag = 0;

        internal GameState(IKeyboard keyboard, Dictionary<SCR, IView> views) 
        {
            _views = views;
            _keyboard = keyboard;

            // currentView = _views[SCR.SCR_CMDR_STATUS];
        }

        internal void Reset()
        {
            IsInitialised = true;
            IsGameOver = false;
            witchspace = false;
        }

        internal void SetView(SCR screen)
        {
            //lock (_state)
            //{
                currentScreen = screen;
                currentView = _views[screen];
                _keyboard.ClearKeyPressed();
                currentView.Reset();
            //}
        }

        /// <summary>
        /// Game Over...
        /// </summary>
        internal void GameOver()
        {
            if (!IsGameOver)
            {
                SetView(SCR.SCR_GAME_OVER);
            }

            IsGameOver = true;
        }
    }
}
