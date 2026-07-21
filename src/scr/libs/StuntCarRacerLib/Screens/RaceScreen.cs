// 'Stunt Car Racer - The Sharp Kind' - Andy Hawkins 2026.
// 'Stunt Car Racer Remake' - sourceforge.net/projects/stuntcarremake.
// Stunt Car Racer (C) Geoff Crammond / MicroStyle / MicroProse 1989.

using StuntCarRacerLib.Cars;
using Useful.Abstraction;
using Useful.Controls;

namespace StuntCarRacerLib.Screens;

// The race itself (original GAME_IN_PROGRESS): input, engine sound and race
// timing at the full tick rate, car physics every FrameGap ticks.
internal sealed class RaceScreen : IGameScreen
{
    private readonly Race _race;
    private readonly IKeyboard _keyboard;
    private readonly ScreenManager<GameMode, IGameScreen> _screens;

    internal RaceScreen(Race race, IKeyboard keyboard, ScreenManager<GameMode, IGameScreen> screens)
    {
        _race = race;
        _keyboard = keyboard;
        _screens = screens;
    }

    public void Reset()
    {
        _race.Car.StartRace();
        _race.Car.BoostReserve = _race.Track.StandardBoost;
        _race.Opponent.StartRace();
        _race.Bridge.Reset(_race.Opponent);

        _race.RaceTick = 0;
        _race.RaceFinished = false;
        _race.RaceWon = false;
        _race.RaceFinishedTick = 0;
    }

    public void Update()
    {
        // The full-rate part of the race (the original FramesWheelsEngine
        // call plus the race-finished timing, which the original drove from
        // the wall clock).
        _race.RaceTick++;
        _race.Car.ApplyEngineRevs();
        _race.UpdateEngineSound();

        // show the race result for six seconds, then it is game over
        if (_race.RaceFinished && _race.RaceTick - _race.RaceFinishedTick > 6 * StuntCarRacerMain.TickRate)
        {
            _screens.Set(GameMode.GameOver);
            return;
        }

        if (!_race.PhysicsDue())
        {
            return;
        }

        // One physics frame of the race (every FrameGap ticks).
        _race.FrameMoved = true;
        _race.Car.Update(ReadInput());
        _race.Opponent.Update();
        _race.Bridge.Move(_race.Car.CurrentPiece, _race.Opponent.CurrentPiece, _race.Opponent);
        _race.Car.UpdateLapData();
        _race.Opponent.UpdateLapData();
        _race.Car.UpdateDamage();
        _race.Camera.FollowCar(_race.Car);

        // the race finishes when either car completes the final lap
        if (!_race.RaceFinished && (_race.Car.RaceFinished || _race.Opponent.LapNumber >= 4))
        {
            _race.RaceFinished = true;
            _race.RaceWon = _race.Opponent.CalculateIfWinning() < 0;
            _race.RaceFinishedTick = _race.RaceTick;
        }

        _race.UpdateSounds();
    }

    public void Draw()
    {
        _race.DrawWorld(showOpponent: true);
        _race.DrawHud(gameOver: false);
    }

    // ptitSeb's stuntcarremake keyboard controls: Left/Right arrows =
    // steer, Up = accelerate, Down = brake, Space = boost (applies with
    // either accelerate or brake held). Uses IsHeld rather than IsPressed:
    // these are continuous controls polled every physics tick, not
    // one-shot menu actions, so they must reflect whether the key is
    // physically down rather than being consumed after the first read
    // (IsPressed's one-shot consumption meant driving felt "stuck" as
    // soon as a second key was held, since a held key's state was cleared
    // on the previous tick and nothing but a fresh SDL key-repeat event —
    // which the OS doesn't reliably send per-key once several keys are
    // down at once — would set it again).
    private CarInput ReadInput()
    {
        CarInput input = CarInput.None;

        if (_keyboard.IsHeld(ConsoleKey.LeftArrow))
        {
            input |= CarInput.Left;
        }

        if (_keyboard.IsHeld(ConsoleKey.RightArrow))
        {
            input |= CarInput.Right;
        }

        if (_keyboard.IsHeld(ConsoleKey.UpArrow))
        {
            input |= CarInput.Accelerate;
        }

        if (_keyboard.IsHeld(ConsoleKey.DownArrow))
        {
            input |= CarInput.Brake;
        }

        if (_keyboard.IsHeld(ConsoleKey.Spacebar))
        {
            input |= CarInput.Boost;
        }

        return input;
    }
}
