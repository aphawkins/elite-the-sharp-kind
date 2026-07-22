// 'Stunt Car Racer - The Sharp Kind' - Andy Hawkins 2026.
// 'Stunt Car Racer Remake' - sourceforge.net/projects/stuntcarremake.
// Stunt Car Racer (C) Geoff Crammond / MicroStyle / MicroProse 1989.

using Useful.Abstraction;
using Useful.Audio;
using Useful.Controls;

namespace StuntCarRacerSharpLib.Screens;

// The original freezes the action and silences the engine once the game is
// over; M returns to the track menu.
internal sealed class GameOverScreen : IGameScreen
{
    private readonly Race _race;
    private readonly IKeyboard _keyboard;
    private readonly ISound _sound;
    private readonly ScreenManager<GameMode, IGameScreen> _screens;

    internal GameOverScreen(Race race, IKeyboard keyboard, ISound sound, ScreenManager<GameMode, IGameScreen> screens)
    {
        _race = race;
        _keyboard = keyboard;
        _sound = sound;
        _screens = screens;
    }

    public void Reset() => _sound.StopLoop();

    public void Update()
    {
        if (_keyboard.IsPressed(ConsoleKey.M))
        {
            _screens.Set(GameMode.TrackMenu);
        }
    }

    public void Draw()
    {
        _race.DrawWorld(showOpponent: true);
        _race.DrawHud(gameOver: true);
    }
}
