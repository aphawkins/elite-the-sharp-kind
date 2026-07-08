// 'Stunt Car Racer - The Sharp Kind' - Andy Hawkins 2026.
// 'Stunt Car Racer Remake' - sourceforge.net/projects/stuntcarremake.
// Stunt Car Racer (C) Geoff Crammond / MicroStyle / MicroProse 1989.

using StuntCarRacerLib.Cars;
using Useful.Abstraction;

namespace StuntCarRacerLib.Screens;

// The race itself (original GAME_IN_PROGRESS): input, engine sound and race
// timing at the full tick rate, car physics every FrameGap ticks.
internal sealed class RaceScreen : IGameScreen
{
    private readonly StuntCarRacerMain _game;

    internal RaceScreen(StuntCarRacerMain game) => _game = game;

    public void Reset()
    {
        _game.Car.StartRace();
        _game.Car.BoostReserve = _game.Track.StandardBoost;
        _game.Opponent.StartRace();
        _game.Bridge.Reset(_game.Opponent);

        _game.RaceTick = 0;
        _game.RaceFinished = false;
        _game.RaceWon = false;
        _game.RaceFinishedTick = 0;
    }

    public void Update()
    {
        // The full-rate part of the race (the original FramesWheelsEngine
        // call plus the race-finished timing, which the original drove from
        // the wall clock).
        _game.RaceTick++;
        _game.Car.ApplyEngineRevs();
        _game.UpdateEngineSound();

        // show the race result for six seconds, then it is game over
        if (_game.RaceFinished && _game.RaceTick - _game.RaceFinishedTick > 6 * StuntCarRacerMain.TickRate)
        {
            _game.Screens.Set(GameMode.GameOver);
            return;
        }

        if (!_game.PhysicsDue())
        {
            return;
        }

        // One physics frame of the race (every FrameGap ticks).
        _game.FrameMoved = true;
        _game.Car.Update(ReadInput());
        _game.Opponent.Update();
        _game.Bridge.Move(_game.Car.CurrentPiece, _game.Opponent.CurrentPiece, _game.Opponent);
        _game.Car.UpdateLapData();
        _game.Opponent.UpdateLapData();
        _game.Car.UpdateDamage();
        _game.Camera.FollowCar(_game.Car);

        // the race finishes when either car completes the final lap
        if (!_game.RaceFinished && (_game.Car.RaceFinished || _game.Opponent.LapNumber >= 4))
        {
            _game.RaceFinished = true;
            _game.RaceWon = _game.Opponent.CalculateIfWinning() < 0;
            _game.RaceFinishedTick = _game.RaceTick;
        }

        _game.UpdateSounds();
    }

    public void Draw()
    {
        _game.DrawWorld(showOpponent: true);
        _game.DrawHud(gameOver: false);
    }

    // ptitSeb's stuntcarremake keyboard controls: Left/Right arrows =
    // steer, Up = accelerate, Down = brake, Space = boost (applies with
    // either accelerate or brake held).
    private CarInput ReadInput()
    {
        CarInput input = CarInput.None;

        if (_game.Keyboard.IsPressed(ConsoleKey.LeftArrow))
        {
            input |= CarInput.Left;
        }

        if (_game.Keyboard.IsPressed(ConsoleKey.RightArrow))
        {
            input |= CarInput.Right;
        }

        if (_game.Keyboard.IsPressed(ConsoleKey.UpArrow))
        {
            input |= CarInput.Accelerate;
        }

        if (_game.Keyboard.IsPressed(ConsoleKey.DownArrow))
        {
            input |= CarInput.Brake;
        }

        if (_game.Keyboard.IsPressed(ConsoleKey.Spacebar))
        {
            input |= CarInput.Boost;
        }

        return input;
    }
}
