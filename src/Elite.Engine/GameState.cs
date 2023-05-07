// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

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

        internal GameState(IKeyboard keyboard, Dictionary<SCR, IView> views)
        {
            _views = views;
            _keyboard = keyboard;

            // currentView = _views[SCR.SCR_CMDR_STATUS];
        }

        internal int CarryFlag { get; set; } = 0;
        internal Commander Cmdr { get; set; } = new();
        internal Vector2 CompassCentre { get; set; } = new(382, 22 + 385);
        internal ConfigSettings Config { get; set; } = new();
        internal Vector2 Cross { get; set; } = new(0, 0);
        internal PlanetData CurrentPlanetData { get; set; } = new();
        internal SCR CurrentScreen { get; set; } = SCR.SCR_NONE;
        internal IView? CurrentView { get; set; }
        internal bool DetonateBomb { get; set; }
        internal float DistanceToPlanet { get; set; }
        internal GalaxySeed DockedPlanet { get; set; } = new();
        internal bool DrawLasers { get; set; }
        internal bool ExitGame { get; set; }
        internal GalaxySeed HyperspacePlanet { get; set; } = new();
        internal bool InWitchspace { get; set; }
        internal bool IsAutoPilotOn { get; set; }
        internal bool IsDocked { get; set; } = true;
        internal bool IsGameOver { get; private set; } = false;
        internal bool IsInitialised { get; set; } = false;
        internal float LaserTemp { get; set; }
        internal int mcount { get; set; }
        internal int MessageCount { get; set; }
        internal string MessageString { get; set; } = string.Empty;
        internal string PlanetName { get; set; } = string.Empty;
        internal Dictionary<ShipType, ShipData> ShipList { get; private set; } = new()
        {
            { ShipType.None, new() },
            { ShipType.Missile, Ship.missile_data },
            { ShipType.Coriolis, Ship.coriolis_data },
            { ShipType.EscapePod, Ship.esccaps_data },
            { ShipType.Alloy, new Alloy() },
            { ShipType.Cargo, Ship.cargo_data },
            { ShipType.Boulder, Ship.boulder_data },
            { ShipType.Asteroid, Ship.asteroid_data },
            { ShipType.Rock, Ship.rock_data },
            { ShipType.Shuttle, Ship.orbit_data },
            { ShipType.Transporter, Ship.transp_data },
            { ShipType.CobraMk3, Ship.cobra3a_data },
            { ShipType.Python, Ship.pythona_data },
            { ShipType.Boa, Ship.boa_data },
            { ShipType.Anaconda, new Anaconda() },
            { ShipType.Hermit, Ship.hermit_data },
            { ShipType.Viper, Ship.viper_data },
            { ShipType.Sidewinder, Ship.sidewnd_data },
            { ShipType.Mamba, Ship.mamba_data },
            { ShipType.Krait, Ship.krait_data },
            { ShipType.Adder, new Adder() },
            { ShipType.Gecko, Ship.gecko_data },
            { ShipType.CobraMk1, Ship.cobra1_data },
            { ShipType.Worm, Ship.worm_data },
            { ShipType.CobraMk3Lone, Ship.cobra3b_data },
            { ShipType.Asp2, Ship.asp2_data },
            { ShipType.PythonLone, Ship.pythonb_data },
            { ShipType.FerDeLance, Ship.ferdlce_data },
            { ShipType.Moray, Ship.moray_data },
            { ShipType.Thargoid, Ship.thargoid_data },
            { ShipType.Tharglet, Ship.thargon_data },
            { ShipType.Constrictor, Ship.constrct_data },
            { ShipType.Cougar, Ship.cougar_data },
            { ShipType.Dodec, Ship.dodec_data }
        };
        internal void DoExitGame() => ExitGame = true;

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

        internal void InfoMessage(string message)
        {
            MessageString = message;
            MessageCount = 37;
            //	sound.snd_play_sample (SND_BEEP);
        }

        internal void Reset()
        {
            IsInitialised = true;
            IsGameOver = false;
            InWitchspace = false;
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
            CurrentScreen = screen;
            CurrentView = _views[screen];
            _keyboard.ClearKeyPressed();
            CurrentView.Reset();
            //}
        }
    }
}
