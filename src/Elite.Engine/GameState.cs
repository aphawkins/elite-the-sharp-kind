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

namespace Elite.Engine
{
    using Elite.Engine.Enums;
    using Elite.Engine.Types;

    internal class GameState
    {
        private readonly IKeyboard _keyboard;
        private readonly Dictionary<SCR, IView> _views;

        internal bool IsGameOver { get; private set; } = false;
        internal bool IsInitialised { get; set; } = false;
        internal SCR currentScreen = SCR.SCR_NONE;
        internal IView currentView;

        internal bool witchspace;
        internal Commander cmdr = new();
        internal galaxy_seed docked_planet = new();
        internal string planetName;
        internal galaxy_seed hyperspace_planet;
        internal planet_data current_planet_data = new();

        internal GameState(IKeyboard keyboard, Dictionary<SCR, IView> views) 
        {
            _views = views;
            _keyboard = keyboard;
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