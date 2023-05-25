// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using Elite.Engine.Config;
using Elite.Engine.Enums;
using Elite.Engine.Types;

namespace Elite.Engine
{
    internal sealed class GameState
    {
        private readonly IKeyboard _keyboard;
        private readonly Dictionary<Screen, IView> _views;

        internal GameState(IKeyboard keyboard, Dictionary<Screen, IView> views)
        {
            _views = views;
            _keyboard = keyboard;

            // currentView = _views[SCR.SCR_CMDR_STATUS];
        }

        internal int CarryFlag { get; set; }

        internal Commander Cmdr { get; set; } = new();

        internal Vector2 CompassCentre { get; set; } = new(382, 22 + 385);

        internal ConfigSettings Config { get; set; } = new();

        internal Vector2 Cross { get; set; } = new(0, 0);

        internal PlanetData CurrentPlanetData { get; set; } = new();

        internal Screen CurrentScreen { get; set; } = Screen.None;

        internal IView? CurrentView { get; set; }

        internal bool DetonateBomb { get; set; }

        internal float DistanceToPlanet { get; set; }

        internal GalaxySeed DockedPlanet { get; set; } = new();

        internal bool DrawLasers { get; set; }

        internal bool ExitGame { get; set; }

        internal GalaxySeed HyperspacePlanet { get; set; } = new();

        internal bool InWitchspace { get; set; }

        internal bool IsDocked { get; set; } = true;

        internal bool IsGameOver { get; private set; }

        internal bool IsInitialised { get; set; }

        internal float LaserTemp { get; set; }

        internal int MCount { get; set; }

        internal int MessageCount { get; set; }

        internal string MessageString { get; set; } = string.Empty;

        internal string PlanetName { get; set; } = string.Empty;

        internal void DoExitGame() => ExitGame = true;

        /// <summary>
        /// Game Over...
        /// </summary>
        internal void GameOver()
        {
            if (!IsGameOver)
            {
                SetView(Screen.GameOver);
            }

            IsGameOver = true;
        }

        internal void InfoMessage(string message)
        {
            MessageString = message;
            MessageCount = 37;

            //  sound.snd_play_sample (SND_BEEP);
        }

        internal void Reset()
        {
            IsInitialised = true;
            IsGameOver = false;
            InWitchspace = false;
            IsDocked = true;
            Cross = new(-1, -1);
            DetonateBomb = false;
            DrawLasers = false;
            ExitGame = false;
            MCount = 0;
        }

        internal void SetView(Screen screen)
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
