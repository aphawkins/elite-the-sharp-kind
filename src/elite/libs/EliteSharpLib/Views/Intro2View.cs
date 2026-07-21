// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using EliteSharpLib.Audio;
using EliteSharpLib.Conflict;
using EliteSharpLib.Graphics;
using EliteSharpLib.Ships;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Useful.Audio;
using Useful.Controls;
using Useful.Maths;

namespace EliteSharpLib.Views;

/// <summary>
/// Parade of the various ships.
/// </summary>
internal sealed class Intro2View : IView
{
    private readonly AudioController _audio;
    private readonly Combat _combat;
    private readonly IEliteDraw _draw;
    private readonly GameState _gameState;
    private readonly IKeyboard _keyboard;
    private readonly PlayerShip _ship;
    private readonly List<IShip> _parade;
    private readonly Stars _stars;
    private readonly Universe _universe;
    private readonly ILogger<Intro2View> _logger;
    private readonly uint _colorGold;
    private readonly uint _colorWhite;

    private int _direction;
    private Matrix4x4 _rotmat;
    private int _shipNo;
    private int _showTime;

    internal Intro2View(
        GameState gameStat,
        AudioController audio,
        IKeyboard keyboard,
        Stars stars,
        PlayerShip ship,
        Combat combat,
        Universe universe,
        IEliteDraw draw,
        IShipFactory shipFactory,
        ILogger<Intro2View>? logger = null)
    {
        _gameState = gameStat;
        _audio = audio;
        _keyboard = keyboard;
        _stars = stars;
        _ship = ship;
        _combat = combat;
        _universe = universe;
        _draw = draw;
        _parade = shipFactory.CreateParade();
        _logger = logger ?? NullLogger<Intro2View>.Instance;

        _colorGold = draw.Palette["Gold"];
        _colorWhite = draw.Palette["White"];
    }

    public void Draw()
    {
        _draw.Graphics.DrawImageCentre(nameof(ImageType.EliteText), _draw.Top + 10);

        _draw.Graphics.DrawTextCentre(_draw.ScannerTop - 30, "Press Fire or Space, Commander.", nameof(FontType.Large), _colorGold);
        if (_universe.FirstShip != null)
        {
            _draw.Graphics
                .DrawTextCentre(_draw.ScannerTop - 60, ((IShip)_universe.FirstShip).Name, nameof(FontType.Small), _colorWhite);
        }
    }

    public void HandleInput()
    {
        if (_keyboard.IsPressed(ConsoleKey.Spacebar))
        {
            _combat.Reset();
            _universe.ClearUniverse();
            _audio.StopMusic();
            _gameState.SetView(Screen.CommanderStatus);
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
