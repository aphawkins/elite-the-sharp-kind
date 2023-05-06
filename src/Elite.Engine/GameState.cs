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

using System.Numerics;
using Elite.Engine.Config;
using Elite.Engine.Enums;
using Elite.Engine.Ships;
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
        internal int message_count;
        internal string message_string = string.Empty;

        internal bool witchspace;
        internal Commander cmdr = new();
        internal GalaxySeed docked_planet = new();
        internal string planetName = string.Empty;
        internal GalaxySeed hyperspace_planet = new();
        internal PlanetData current_planet_data = new();
        internal int carry_flag = 0;
        internal bool IsAutoPilotOn { get; set; }
        internal bool IsDocked { get; set; } = true;
        internal Vector2 CompassCentre { get; set; } = new(382, 22 + 385);
        internal Vector2 Cross { get; set; } = new(0, 0);
        internal bool DetonateBomb { get; set; }
        internal float DistanceToPlanet { get; set; }
        internal bool DrawLasers { get; set; }
        internal bool ExitGame { get; set; }
        internal float LaserTemp { get; set; }
        internal int mcount { get; set; }
        internal ConfigSettings config = new();

        internal ShipData[] ShipList { get; private set; } = new ShipData[Ship.NO_OF_SHIPS + 1]
        {
            new(),
            Ship.missile_data,
            Ship.coriolis_data,
            Ship.esccaps_data,
            Ship.alloy_data,
            Ship.cargo_data,
            Ship.boulder_data,
            Ship.asteroid_data,
            Ship.rock_data,
            Ship.orbit_data,
            Ship.transp_data,
            Ship.cobra3a_data,
            Ship.pythona_data,
            Ship.boa_data,
            Ship.anacnda_data,
            Ship.hermit_data,
            Ship.viper_data,
            Ship.sidewnd_data,
            Ship.mamba_data,
            Ship.krait_data,
            Ship.adder_data,
            Ship.gecko_data,
            Ship.cobra1_data,
            Ship.worm_data,
            Ship.cobra3b_data,
            Ship.asp2_data,
            Ship.pythonb_data,
            Ship.ferdlce_data,
            Ship.moray_data,
            Ship.thargoid_data,
            Ship.thargon_data,
            Ship.constrct_data,
            Ship.cougar_data,
            Ship.dodec_data
        };

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
            IsAutoPilotOn = false;
            IsDocked = true;
            Cross = new(-1, -1);
            DetonateBomb = false;
            DrawLasers = false;
            ExitGame = false;
            mcount = 0;
            SetView(SCR.SCR_INTRO_ONE);
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

        internal void DoExitGame() => ExitGame = true;

        internal void InfoMessage(string message)
        {
            message_string = message;
            message_count = 37;
            //	sound.snd_play_sample (SND_BEEP);
        }
    }
}
