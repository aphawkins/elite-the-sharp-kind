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
    internal sealed class GameState
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
        internal Dictionary<ShipType, IShip> ShipList { get; private set; } = new()
        {
            { ShipType.None, new NoShip() },
            { ShipType.Missile, new Missile() },
            { ShipType.Coriolis, new Coriolis() },
            { ShipType.EscapeCapsule, new EscapeCapsule() },
            { ShipType.Alloy, new Alloy() },
            { ShipType.Cargo, new CargoCannister() },
            { ShipType.Boulder, new Boulder() },
            { ShipType.Asteroid, new Asteroid() },
            { ShipType.Rock, new RockSplinter() },
            { ShipType.Shuttle, new Shuttle() },
            { ShipType.Transporter, new Transporter() },
            { ShipType.CobraMk3, new CobraMk3() },
            { ShipType.Python, new Python() },
            { ShipType.Boa, new Boa() },
            { ShipType.Anaconda, new Anaconda() },
            { ShipType.Hermit, new RockHermit() },
            { ShipType.Viper, new Viper() },
            { ShipType.Sidewinder, new Sidewinder() },
            { ShipType.Mamba, new Mamba() },
            { ShipType.Krait, new Krait() },
            { ShipType.Adder, new Adder() },
            { ShipType.Gecko, new Gecko() },
            { ShipType.CobraMk1, new CobraMk1() },
            { ShipType.Worm, new Worm() },
            { ShipType.CobraMk3Lone, new CobraMk3Lone() },
            { ShipType.Asp2, new AspMk2() },
            { ShipType.PythonLone, new PythonLone() },
            { ShipType.FerDeLance, new FerDeLance() },
            { ShipType.Moray, new Moray() },
            { ShipType.Thargoid, new Thargoid() },
            { ShipType.Tharglet, new Tharglet() },
            { ShipType.Constrictor, new Constrictor() },
            { ShipType.Cougar, new Cougar() },
            { ShipType.Dodec, new DodecStation() }
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
