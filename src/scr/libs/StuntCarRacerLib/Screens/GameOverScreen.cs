// 'Stunt Car Racer - The Sharp Kind' - Andy Hawkins 2026.
// 'Stunt Car Racer Remake' - sourceforge.net/projects/stuntcarremake.
// Stunt Car Racer (C) Geoff Crammond / MicroStyle / MicroProse 1989.

using Useful.Abstraction;

namespace StuntCarRacerLib.Screens;

// The original freezes the action and silences the engine once the game is
// over; M returns to the track menu.
internal sealed class GameOverScreen : IGameScreen
{
    private readonly StuntCarRacerMain _game;

    internal GameOverScreen(StuntCarRacerMain game) => _game = game;

    public void Reset() => _game.Sound.StopLoop();

    public void Update()
    {
        if (_game.Keyboard.IsPressed(ConsoleKey.M))
        {
            _game.Screens.Set(GameMode.TrackMenu);
        }
    }

    public void Draw()
    {
        _game.DrawWorld(showOpponent: true);
        _game.DrawHud(gameOver: true);
    }
}
