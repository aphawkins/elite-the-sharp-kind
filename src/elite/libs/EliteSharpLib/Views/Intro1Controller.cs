// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using EliteSharpLib.Audio;
using EliteSharpLib.Conflict;
using EliteSharpLib.Ships;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Useful.Audio;
using Useful.Controls;
using Useful.Maths;

namespace EliteSharpLib.Views;

/// <summary>
/// The title screen's behaviour: the rolling Cobra MkIII it puts in the
/// universe, and the Y/N choice of loading a commander. The screen's text is
/// fixed, so the view takes no model.
/// </summary>
internal sealed class Intro1Controller : IScreenController
{
    private readonly AudioController _audio;
    private readonly Combat _combat;
    private readonly GameState _gameState;
    private readonly IKeyboard _keyboard;
    private readonly PlayerShip _ship;
    private readonly Universe _universe;
    private readonly IShipFactory _shipFactory;
    private readonly ILogger<Intro1Controller> _logger;
    private readonly IView<Intro1Model> _view;

    internal Intro1Controller(
        GameState gameState,
        AudioController audio,
        IKeyboard keyboard,
        PlayerShip ship,
        Combat combat,
        Universe universe,
        IShipFactory shipFactory,
        IView<Intro1Model> view,
        ILogger<Intro1Controller>? logger = null)
    {
        _gameState = gameState;
        _audio = audio;
        _keyboard = keyboard;
        _ship = ship;
        _combat = combat;
        _universe = universe;
        _shipFactory = shipFactory;
        _view = view;
        _logger = logger ?? NullLogger<Intro1Controller>.Instance;
    }

    // The credits and prompt never change, so the model is built once.
    private static Intro1Model Model { get; } = new(
        [
            "Original Game (C) I.Bell & D.Braben",
            "The New Kind - Christian Pinder",
            "The Sharp Kind - Andy Hawkins",
        ],
        "Load New Commander (Y/N)?");

    public void Draw() => _view.Draw(Model);

    public void HandleInput()
    {
        if (_keyboard.IsPressed(ConsoleKey.Y))
        {
            LeaveIntro(Screen.LoadCommander);
        }

        if (_keyboard.IsPressed(ConsoleKey.N))
        {
            LeaveIntro(Screen.IntroTwo);
        }
    }

    public void Reset()
    {
        _combat.Reset();
        _universe.ClearUniverse();

        Matrix4x4 initMatrix = VectorMaths.GetLeftHandedBasisMatrix;

        // Ship faces away
        initMatrix.M33 = 1;
        IShip cobraMk3 = _shipFactory.CreateShip("CobraMk3");
        if (!_universe.AddNewShip(cobraMk3, new(0, 0, 4500, 0), initMatrix, -127, 127))
        {
            LogMessages.FailedToCreateShip(_logger, "CobraMk3");
        }

        _audio.PlayMusic(nameof(MusicType.EliteTheme), true);
    }

    public void Update()
    {
        _ship.Roll = 1;
        _universe.FirstShip!.Location =
            new(_universe.FirstShip.Location.X, _universe.FirstShip.Location.Y, _universe.FirstShip.Location.Z - 100, 0);

        if (_universe.FirstShip.Location.Z < 384)
        {
            _universe.FirstShip.Location =
                new(_universe.FirstShip.Location.X, _universe.FirstShip.Location.Y, 384, 0);
        }
    }

    private void LeaveIntro(Screen next)
    {
        _combat.Reset();
        _universe.ClearUniverse();
        _audio.StopMusic();
        _gameState.SetView(next);
    }
}
