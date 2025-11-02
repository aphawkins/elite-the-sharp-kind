// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Conflict;
using EliteSharpLib.Graphics;
using EliteSharpLib.Lasers;
using EliteSharpLib.Ships;
using Useful.Controls;

namespace EliteSharpLib.Views;

internal sealed class PilotView : IView
{
    private readonly GameState _gameState;
    private readonly IKeyboard _keyboard;
    private readonly LaserDraw _laser;
    private readonly Pilot _pilot;
    private readonly PlayerShip _ship;
    private readonly Stars _stars;
    private readonly Space _space;
    private readonly IEliteDraw _draw;
    private readonly Combat _combat;
    private int _drawLaserFrames;

    internal PilotView(
        GameState gameState,
        IKeyboard keyboard,
        Pilot pilot,
        PlayerShip ship,
        Stars stars,
        Space space,
        IEliteDraw draw,
        Combat combat)
    {
        _gameState = gameState;
        _keyboard = keyboard;
        _laser = new LaserDraw(_gameState, draw);
        _pilot = pilot;
        _ship = ship;
        _stars = stars;
        _space = space;
        _draw = draw;
        _combat = combat;
    }

    public void Draw()
    {
        if (_drawLaserFrames > 0)
        {
            _laser.DrawLaserLines();
        }

        if (_space.HyperGalactic)
        {
            _draw.Graphics.DrawTextCentre(358, "Galactic Hyperspace", (int)FontType.Small, EliteColors.White);
        }
        else if (_space.HyperCountdown > 0)
        {
            _draw.Graphics.DrawTextCentre(358, $"Hyperspace - {_space.HyperName}", (int)FontType.Small, EliteColors.White);
        }
    }

    public void HandleInput()
    {
        if (_keyboard.IsPressed(ConsoleKey.A))
        {
            _gameState.DrawLasers = _combat.FireLaser();
        }

        if (_keyboard.IsPressed(ConsoleKey.C) &&
            !_gameState.IsDocked
            && _ship.HasDockingComputer)
        {
            if (_gameState.Config.InstantDock)
            {
                _space.EngageDockingComputer();
            }
            else if (!_gameState.InWitchspace && !_space.IsHyperspaceReady)
            {
                _pilot.EngageAutoPilot();
            }
        }

        if (_keyboard.IsPressed(ConsoleKey.D))
        {
            _pilot.DisengageAutoPilot();
        }

        if (_keyboard.IsPressed(ConsoleKey.E) &&
            !_gameState.IsDocked
            && _ship.HasECM)
        {
            _combat.ActivateECM(true);
        }

        if (_keyboard.IsPressed(ConsoleKey.H) && (!_gameState.IsDocked))
        {
            if (_keyboard.IsPressed(ConsoleModifiers.Control))
            {
                _space.StartGalacticHyperspace();
            }
            else
            {
                _space.StartHyperspace();
            }
        }

        if (_keyboard.IsPressed(ConsoleKey.J) &&
            (!_gameState.IsDocked)
            && (!_gameState.InWitchspace))
        {
            _space.JumpWarp();
        }

        if (_keyboard.IsPressed(ConsoleKey.M) &&
            !_gameState.IsDocked)
        {
            _combat.FireMissile();
        }

        if (_keyboard.IsPressed(ConsoleKey.P))
        {
            _gameState.IsGamePaused = true;
        }

        if (_keyboard.IsPressed(ConsoleKey.S) || _keyboard.IsPressed(ConsoleKey.UpArrow))
        {
            if (_ship.Climb > 0)
            {
                _ship.Climb = 0;
            }
            else
            {
                _ship.DecreaseClimb();
                _ship.DecreaseClimb();
            }

            _ship.IsClimbing = true;
        }

        if (_keyboard.IsPressed(ConsoleKey.X) || _keyboard.IsPressed(ConsoleKey.DownArrow))
        {
            if (_ship.Climb < 0)
            {
                _ship.Climb = 0;
            }
            else
            {
                _ship.IncreaseClimb();
                _ship.IncreaseClimb();
            }

            _ship.IsClimbing = true;
        }

        if (_keyboard.IsPressed(ConsoleKey.OemComma) || _keyboard.IsPressed(ConsoleKey.LeftArrow))
        {
            if (_ship.Roll < 0)
            {
                _ship.Roll = 0;
            }
            else
            {
                _ship.IncreaseRoll();
                _ship.IncreaseRoll();
                _ship.IsRolling = true;
            }
        }

        if (_keyboard.IsPressed(ConsoleKey.OemPeriod) || _keyboard.IsPressed(ConsoleKey.RightArrow))
        {
            if (_ship.Roll > 0)
            {
                _ship.Roll = 0;
            }
            else
            {
                _ship.DecreaseRoll();
                _ship.DecreaseRoll();
                _ship.IsRolling = true;
            }
        }

        if (_keyboard.IsPressed(ConsoleKey.T) &&
            !_gameState.IsDocked)
        {
            _combat.ArmMissile();
        }

        if (_keyboard.IsPressed(ConsoleKey.U) &&
            !_gameState.IsDocked)
        {
            _combat.UnarmMissile();
        }

        if (_keyboard.IsPressed(ConsoleKey.Spacebar) &&
            !_gameState.IsDocked)
        {
            _ship.IncreaseSpeed();
        }

        if (_keyboard.IsPressed(ConsoleKey.Oem2) &&
            !_gameState.IsDocked)
        {
            _ship.DecreaseSpeed();
        }

        if (_keyboard.IsPressed(ConsoleKey.Tab) &&
            (!_gameState.IsDocked)
            && _ship.HasEnergyBomb)
        {
            _gameState.DetonateBomb = true;
            _ship.HasEnergyBomb = false;
        }

        if (_keyboard.IsPressed(ConsoleKey.Escape) &&
            (!_gameState.IsDocked)
            && _ship.HasEscapeCapsule
            && (!_gameState.InWitchspace))
        {
            _gameState.SetView(Screen.EscapeCapsule);
        }
    }

    public void Reset() => _stars.FlipStars();

    public void UpdateUniverse()
        => _drawLaserFrames = _gameState.DrawLasers ? 2 : Math.Clamp(_drawLaserFrames - 1, 0, _drawLaserFrames);

    internal void DrawLaserSights(LaserType laserType) => _laser.DrawLaserSights(laserType);

    internal void DrawViewName(string name)
        => _draw.Graphics.DrawTextCentre(_draw.Top + 10, name, (int)FontType.Small, EliteColors.White);
}
