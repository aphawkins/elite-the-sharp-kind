// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Config;
using EliteSharpLib.Types;
using EliteSharpLib.Views;
using Useful.Abstraction;

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

    private readonly ScreenManager<Screen, IScreenController> _views;

    internal GameState(ScreenManager<Screen, IScreenController> views) => _views = views;

    internal int CarryFlag { get; set; }

    internal Commander Cmdr { get; set; } = new();

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
