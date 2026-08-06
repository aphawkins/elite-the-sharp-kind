// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Diagnostics;
using System.Numerics;
using EliteSharp.Abstractions.Ships;
using EliteSharp.Abstractions.Views;
using EliteSharpLib.Conflict;
using EliteSharpLib.Ships;
using Useful.Graphics.Rendering;
using Useful.Input;

namespace EliteSharpLib.Views;

/// <summary>
/// A cockpit window's behaviour: flight, docking and weapon controls, which
/// are identical looking front, rear, left or right, so one controller
/// serves all four - its <see cref="PilotDirection"/> only selects the view
/// name, the laser mount and the starfield to scroll.
/// </summary>
internal sealed class PilotController : IScreenController
{
    private readonly GameState _gameState;
    private readonly IKeyboard _keyboard;
    private readonly Pilot _pilot;
    private readonly PlayerShip _ship;
    private readonly Stars _stars;
    private readonly Space _space;
    private readonly Combat _combat;
    private readonly PilotDirection _direction;
    private readonly RNG _rng;
    private readonly IView<PilotModel> _view;

    private int _drawLaserFrames;

    internal PilotController(
        GameState gameState,
        IKeyboard keyboard,
        Pilot pilot,
        PlayerShip ship,
        Stars stars,
        Space space,
        Combat combat,
        PilotDirection direction,
        RNG rng,
        IView<PilotModel> view)
    {
        _gameState = gameState;
        _keyboard = keyboard;
        _pilot = pilot;
        _ship = ship;
        _stars = stars;
        _space = space;
        _combat = combat;
        _rng = rng;
        _direction = direction;
        _view = view;
    }

    public void Draw() => _view.Draw(BuildModel());

    // Continuous flight controls (pitch/roll/speed/fire) are polled every
    // frame and need IsHeld's non-consuming "is the key currently down"
    // state, not IsPressed's one-shot consumption - otherwise a held key
    // would go unresponsive as soon as a second key was also held (SDL/
    // Windows key-repeat only re-fires for the most recently pressed key).
    // One-shot commands below (docking, hyperspace, missiles, pause, etc.)
    // correctly keep using IsPressed.
    public void HandleInput()
    {
        HandleFlightControls();
        HandleNavigationCommands();
        HandleWeaponCommands();
    }

    public void Reset() => _stars.FlipStars();

    public void Update()
    {
        _drawLaserFrames = _gameState.DrawLasers ? 2 : Math.Clamp(_drawLaserFrames - 1, 0, _drawLaserFrames);

        switch (_direction)
        {
            case PilotDirection.Front:
                _stars.FrontStarfield();
                break;

            case PilotDirection.Rear:
                _stars.RearStarfield();
                break;

            case PilotDirection.Left:
                _stars.LeftStarfield();
                break;

            case PilotDirection.Right:
                _stars.RightStarfield();
                break;
        }
    }

    // Exposed for tests: the view name, the hyperspace status text and
    // this direction's laser state.
    internal PilotModel BuildModel()
    {
        string hyperspaceStatus = _space.HyperGalactic
            ? "Galactic Hyperspace"
            : _space.HyperCountdown > 0 ? $"Hyperspace - {_space.HyperName}" : string.Empty;

        (string viewName, LaserType laserType) = _direction switch
        {
            PilotDirection.Front => ("Front View", _ship.LaserFront.Type),
            PilotDirection.Rear => ("Rear View", _ship.LaserRear.Type),
            PilotDirection.Left => ("Left View", _ship.LaserLeft.Type),
            PilotDirection.Right => ("Right View", _ship.LaserRight.Type),
            _ => throw new UnreachableException(),
        };

        // The beams meet a pixel or two off centre, rolled fresh every frame -
        // that shimmer is the original's. The roll happens here because the
        // game owns the one source of entropy; a view that rolled its own
        // would not be reproducible.
        Vector2 laserAim = new(_rng.Random(0, 2), _rng.Random(0, 2));

        return new(
            viewName,
            hyperspaceStatus,
            laserType,
            _drawLaserFrames > 0,
            laserAim,
            _gameState.Config.Engine.Graphics.GraphicStyle == GraphicStyle.Wireframe);
    }

    private void HandleFlightControls()
    {
        if (_keyboard.IsHeld(ConsoleKey.A))
        {
            _gameState.DrawLasers = _combat.FireLaser();
        }

        if (_keyboard.IsHeld(ConsoleKey.S) || _keyboard.IsHeld(ConsoleKey.UpArrow))
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

        if (_keyboard.IsHeld(ConsoleKey.X) || _keyboard.IsHeld(ConsoleKey.DownArrow))
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

        HandleRollControls();

        if (_keyboard.IsHeld(ConsoleKey.Spacebar) &&
            !_gameState.IsDocked)
        {
            _ship.IncreaseSpeed();
        }

        if (_keyboard.IsHeld(ConsoleKey.Oem2) &&
            !_gameState.IsDocked)
        {
            _ship.DecreaseSpeed();
        }
    }

    // Roll left and right. A roll in the opposite direction to the current one
    // levels the ship out instead.
    private void HandleRollControls()
    {
        if (_keyboard.IsHeld(ConsoleKey.OemComma) || _keyboard.IsHeld(ConsoleKey.LeftArrow))
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

        if (_keyboard.IsHeld(ConsoleKey.OemPeriod) || _keyboard.IsHeld(ConsoleKey.RightArrow))
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
    }

    private void HandleNavigationCommands()
    {
        if (_keyboard.IsPressed(ConsoleKey.C) &&
            !_gameState.IsDocked
            && _ship.HasDockingComputer)
        {
            EngageDockingComputer();
        }

        if (_keyboard.IsPressed(ConsoleKey.D))
        {
            _pilot.DisengageAutoPilot();
        }

        if (_keyboard.IsPressed(ConsoleKey.H) && (!_gameState.IsDocked))
        {
            // Held, not pressed: Ctrl only picks which hyperspace this is, and
            // consuming it would take it from any other Ctrl combination read
            // later in the same tick.
            if (_keyboard.IsHeld(ConsoleModifiers.Control))
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

        if (_keyboard.IsPressed(ConsoleKey.P))
        {
            _gameState.IsGamePaused = true;
        }

        if (_keyboard.IsPressed(ConsoleKey.Escape) &&
            (!_gameState.IsDocked)
            && _ship.HasEscapeCapsule
            && (!_gameState.InWitchspace))
        {
            _gameState.SetView(Screen.EscapeCapsule);
        }
    }

    // Dock instantly if configured to, otherwise fly the ship in on autopilot.
    private void EngageDockingComputer()
    {
        if (_gameState.Config.Game.InstantDock)
        {
            _space.EngageDockingComputer();
        }
        else if (!_gameState.InWitchspace && !_space.IsHyperspaceReady)
        {
            _pilot.EngageAutoPilot();
        }
    }

    private void HandleWeaponCommands()
    {
        if (_keyboard.IsPressed(ConsoleKey.E) &&
            !_gameState.IsDocked
            && _ship.HasECM)
        {
            _combat.ActivateECM(true);
        }

        if (_keyboard.IsPressed(ConsoleKey.M) &&
            !_gameState.IsDocked)
        {
            _combat.FireMissile();
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
}
