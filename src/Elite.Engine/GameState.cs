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

    internal class GameState
    {
        private readonly IKeyboard _keyboard;
        private readonly Dictionary<SCR, IView> _views;
        internal bool IsGameOver { get; private set; } = false;
        internal bool IsInitialised { get; set; } = false;
        internal SCR currentScreen = SCR.SCR_NONE;
        internal IView currentView;
        internal float energy { get; set; } = 255;
        internal float fore_shield { get; private set; } = 255;
        internal float aft_shield { get; private set; } = 255;


        internal GameState(IKeyboard keyboard, Dictionary<SCR, IView> views) 
        {
            _views = views;
            _keyboard = keyboard;
        }

        internal void Reset()
        {
            fore_shield = 255;
            aft_shield = 255;
            energy = 255;
            IsInitialised = true;
            IsGameOver = false;
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

        /// <summary>
        /// Deplete the shields.  Drain the energy banks if the shields fail.
        /// </summary>
        /// <param name="damage"></param>
        /// <param name="front"></param>
        internal void damage_ship(int damage, bool front)
        {
            if (damage <= 0)    /* sanity check */
            {
                return;
            }

            float shield = front ? fore_shield : aft_shield;

            shield -= damage;
            if (shield < 0)
            {
                decrease_energy(shield);
                shield = 0;
            }

            if (front)
            {
                fore_shield = shield;
            }
            else
            {
                aft_shield = shield;
            }
        }

        internal void decrease_energy(float amount)
        {
            energy += amount;

            if (energy <= 0)
            {
                GameOver();
            }
        }

        internal bool IsEnergyLow()
        {
            return energy < 50;
        }

        /*
         * Regenerate the shields and the energy banks.
         */
        internal void regenerate_shields()
        {
            if (energy > 127)
            {
                if (fore_shield < 255)
                {
                    fore_shield++;
                    energy = Math.Clamp(energy - 1, 0, 255);
                }

                if (aft_shield < 255)
                {
                    aft_shield++;
                    energy = Math.Clamp(energy - 1, 0, 255);
                }
            }

            energy = Math.Clamp(energy + 1 + (int)elite.cmdr.energy_unit, 0, 255);
        }
    }
}