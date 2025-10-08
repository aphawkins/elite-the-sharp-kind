// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Diagnostics;
using System.Numerics;
using EliteSharp.Audio;
using EliteSharp.Conflict;
using EliteSharp.Graphics;
using EliteSharp.Ships;
using Useful.Audio;
using Useful.Controls;

namespace EliteSharp.Views;

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
    private int _direction;
    private Vector3[] _rotmat = new Vector3[3];
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
        IEliteDraw draw)
    {
        _gameState = gameStat;
        _audio = audio;
        _keyboard = keyboard;
        _stars = stars;
        _ship = ship;
        _combat = combat;
        _universe = universe;
        _draw = draw;
        _parade = new ShipFactory(_draw).CreateParade();
    }

    public void Draw()
    {
        _draw.Graphics.DrawImageCentre((int)ImageType.EliteText, _draw.Top + 10);

        _draw.Graphics.DrawTextCentre(_draw.ScannerTop - 30, "Press Fire or Space, Commander.", (int)FontType.Large, EliteColors.Gold);
        if (_universe.FirstShip != null)
        {
            _draw.Graphics
                .DrawTextCentre(_draw.ScannerTop - 60, ((IShip)_universe.FirstShip).Name, (int)FontType.Small, EliteColors.White);
        }
    }

    public void HandleInput()
    {
        if (_keyboard.IsKeyPressed(CommandKey.SpaceBar))
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
        _rotmat = VectorMaths.GetInitialMatrix();
        _audio.PlayMusic((int)MusicType.BlueDanube, true);

        AddNewShip();
    }

    public void UpdateUniverse()
    {
        _showTime++;

        if (_showTime >= 140 && _direction < 0)
        {
            _direction = -_direction;
        }

        if (_universe.FirstShip != null)
        {
            _universe.FirstShip.Location =
                new(_universe.FirstShip.Location.X, _universe.FirstShip.Location.Y, _universe.FirstShip.Location.Z + _direction);

            if (_universe.FirstShip.Location.Z < _parade[_shipNo].MinDistance)
            {
                _universe.FirstShip.Location =
                    new(_universe.FirstShip.Location.X, _universe.FirstShip.Location.Y, _parade[_shipNo].MinDistance);
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
        if (!_universe.AddNewShip(_parade[_shipNo], new(0, 0, 4500), _rotmat, -127, -127))
        {
            Debug.WriteLine("Failed to create first Parade ship");
        }
    }
}
