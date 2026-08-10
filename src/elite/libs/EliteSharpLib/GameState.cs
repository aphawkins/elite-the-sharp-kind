// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Config;
using EliteSharpLib.Missions;
using EliteSharpLib.Types;
using EliteSharpLib.Views;
using SharpKind.Abstraction;

namespace EliteSharpLib;

internal sealed class GameState
{
    /// <summary>
    /// The bounds of <see cref="LaserTemp"/>.
    /// </summary>
    internal const float LaserTempMax = 1;

    /// <inheritdoc cref="LaserTempMax"/>
    internal const float LaserTempMin = 0;

    /// <summary>
    /// One unit of the original's 0-255 laser temperature, expressed as a
    /// fraction of <see cref="LaserTempMax"/>.
    /// </summary>
    internal const float LaserTempStep = 1f / 256f;

    /// <summary>
    /// The heat a single shot adds - the original's 8 out of 255.
    /// </summary>
    internal const float LaserTempPerShot = 8 * LaserTempStep;

    /// <summary>
    /// The temperature at or above which the laser is too hot to fire - the
    /// original's 242 out of 255.
    /// </summary>
    internal const float LaserTempOverheated = 242 * LaserTempStep;

    // The options menu and every screen reached from it. None of these is
    // somewhere the options can return to: they are all inside it.
    private static readonly Screen[] s_optionsFamily =
    [
        Screen.Options,
        Screen.Credits,
        Screen.Settings,
        Screen.EngineSettings,
        Screen.SaveCommander,
        Screen.LoadCommander,
        Screen.Quit,
    ];

    private readonly ScreenManager<Screen, IScreenController> _views;

    internal GameState(ScreenManager<Screen, IScreenController> views, MissionRegistry missions)
    {
        _views = views;
        Cmdr = new(new MissionProgress(missions));
    }

    internal int CarryFlag { get; set; }

    internal Commander Cmdr { get; set; }

    internal EliteConfig Config { get; set; } = new();

    internal PlanetData CurrentPlanetData { get; set; } = new();

    internal Screen CurrentScreen => _views.CurrentId;

    internal IScreenController CurrentView => _views.Current;

    internal bool DetonateBomb { get; set; }

    internal float DistanceToPlanet { get; set; }

    internal GalaxySeed DockedPlanet { get; set; } = new();

    internal bool DrawLasers { get; set; }

    internal bool ExitGame { get; set; }

    internal GalaxySeed HyperspacePlanet { get; set; } = new();

    internal bool InWitchspace { get; set; }

    internal bool IsDocked { get; set; } = true;

    internal bool IsGameOver { get; private set; }

    internal bool IsGamePaused { get; set; }

    internal bool IsInitialised { get; set; }

    /// <summary>
    /// Gets or sets the laser temperature, between <see cref="LaserTempMin"/>
    /// and <see cref="LaserTempMax"/>.
    /// </summary>
    internal float LaserTemp { get; set; }

    internal int MCount { get; set; }

    internal int MessageCount { get; set; }

    internal string MessageString { get; set; } = string.Empty;

    internal string PlanetName { get; set; } = string.Empty;

    /// <summary>
    /// Gets the screen the options menu's Back row returns to: the one the
    /// commander opened the options from. Only <see cref="EnterOptions"/>
    /// writes it, so a settings screen returning to the options with a plain
    /// <see cref="SetView"/> leaves it where it was and Back still goes all
    /// the way out.
    /// </summary>
    internal Screen OptionsReturn { get; private set; } = Screen.FrontView;

    internal void DoExitGame() => ExitGame = true;

    /// <summary>
    /// Opens the options menu, remembering the screen it was opened from so
    /// its Back row can return there. Pressing the key again on the options
    /// closes them: the same key in and out, and the same place either way.
    /// <para>
    /// The screen to return to is only taken from outside the options family.
    /// Taking it from a screen inside would make the way out lead back in - a
    /// second press on the options themselves would leave Back returning to
    /// the options - which is a menu with no way out of it at all.
    /// </para>
    /// </summary>
    internal void EnterOptions()
    {
        if (CurrentScreen == Screen.Options)
        {
            SetView(OptionsReturn);
            return;
        }

        if (!s_optionsFamily.Contains(CurrentScreen))
        {
            OptionsReturn = CurrentScreen;
        }

        SetView(Screen.Options);
    }

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
    }

    internal void Reset()
    {
        IsInitialised = true;
        IsGameOver = false;
        InWitchspace = false;
        IsDocked = true;
        DetonateBomb = false;
        DrawLasers = false;
        ExitGame = false;
        MCount = 0;
    }

    internal void SetView(Screen screen) => _views.Set(screen);
}
