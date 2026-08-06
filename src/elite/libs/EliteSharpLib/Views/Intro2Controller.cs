// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using EliteSharp.Abstractions.Views;
using EliteSharpLib.Audio;
using EliteSharpLib.Conflict;
using EliteSharpLib.Ships;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Useful.Audio;
using Useful.Input;
using Useful.Maths;

namespace EliteSharpLib.Views;

/// <summary>
/// The ship parade's behaviour: cycling through <see cref="IShipFactory.CreateParade"/>,
/// flying each ship past the camera in turn, on a timer or on request.
/// </summary>
internal sealed class Intro2Controller : IScreenController
{
    private const string Prompt = "Press Fire or Space, Commander.";

    private readonly AudioController _audio;
    private readonly Combat _combat;
    private readonly GameState _gameState;
    private readonly IKeyboard _keyboard;
    private readonly PlayerShip _ship;
    private readonly List<IShip> _parade;
    private readonly Stars _stars;
    private readonly Universe _universe;
    private readonly ILogger<Intro2Controller> _logger;
    private readonly IView<Intro2Model> _view;

    private int _direction;
    private Matrix4x4 _rotmat;
    private int _shipNo;
    private int _showTime;

    internal Intro2Controller(
        GameState gameState,
        AudioController audio,
        IKeyboard keyboard,
        Stars stars,
        PlayerShip ship,
        Combat combat,
        Universe universe,
        IShipFactory shipFactory,
        IView<Intro2Model> view,
        ILogger<Intro2Controller>? logger = null)
    {
        _gameState = gameState;
        _audio = audio;
        _keyboard = keyboard;
        _stars = stars;
        _ship = ship;
        _combat = combat;
        _universe = universe;
        _parade = shipFactory.CreateParade();
        _view = view;
        _logger = logger ?? NullLogger<Intro2Controller>.Instance;
    }

    public void Draw() => _view.Draw(BuildModel());

    public void HandleInput()
    {
        if (_keyboard.IsPressed(ConsoleKey.Spacebar))
        {
            _combat.Reset();
            _universe.ClearUniverse();
            _audio.StopMusic();
            _gameState.SetView(Screen.CommanderStatus);
            return;
        }

        if (_keyboard.IsPressed(ConsoleKey.LeftArrow))
        {
            _shipNo--;
            if (_shipNo < 0)
            {
                _shipNo = _parade.Count - 1;
            }

            AddNewShip();
        }
        else if (_keyboard.IsPressed(ConsoleKey.RightArrow))
        {
            _shipNo++;
            if (_shipNo >= _parade.Count)
            {
                _shipNo = 0;
            }

            AddNewShip();
        }
    }

    public void Reset()
    {
        _shipNo = 0;
        _ship.Speed = 3;
        _ship.Roll = 0;
        _ship.Climb = 0;
        _combat.Reset();
        _stars.CreateNewStars();
        _rotmat = VectorMaths.GetLeftHandedBasisMatrix;
        _audio.PlayMusic(nameof(MusicType.BlueDanube), true);

        AddNewShip();
    }

    public void Update()
    {
        _showTime++;

        if (_showTime >= 140 && _direction < 0)
        {
            _direction = -_direction;
        }

        if (_universe.FirstShip != null)
        {
            _universe.FirstShip.Location =
                new(_universe.FirstShip.Location.X, _universe.FirstShip.Location.Y, _universe.FirstShip.Location.Z + _direction, 0);

            if (_universe.FirstShip.Location.Z < _parade[_shipNo].MinDistance)
            {
                _universe.FirstShip.Location =
                    new(_universe.FirstShip.Location.X, _universe.FirstShip.Location.Y, _parade[_shipNo].MinDistance, 0);
            }

            if (_universe.FirstShip.Location.Z > 4500)
            {
                _shipNo++;
                if (_shipNo >= _parade.Count)
                {
                    _shipNo = 0;
                }

                AddNewShip();
            }
        }

        _stars.FrontStarfield();
    }

    // Exposed for tests: the prompt and the current ship's name.
    internal Intro2Model BuildModel() => new(Prompt, _universe.FirstShip != null ? ((IShip)_universe.FirstShip).Name : string.Empty);

    private void AddNewShip()
    {
        _showTime = 0;
        _direction = -100;
        _universe.ClearUniverse();
        if (!_universe.AddNewShip(_parade[_shipNo], new(0, 0, 4500, 0), _rotmat, -127, -127))
        {
            LogMessages.FailedToCreateShip(_logger, _parade[_shipNo].Name);
        }
    }
}
