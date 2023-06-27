// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Diagnostics;
using System.Numerics;
using EliteSharp.Audio;
using EliteSharp.Conflict;
using EliteSharp.Controls;
using EliteSharp.Graphics;
using EliteSharp.Ships;

namespace EliteSharp.Views
{
    /// <summary>
    /// Parade of the various ships.
    /// </summary>
    internal sealed class Intro2View : IView
    {
        private readonly AudioController _audio;
        private readonly Combat _combat;
        private readonly GameState _gameState;
        private readonly IGraphics _graphics;
        private readonly IKeyboard _keyboard;

        private readonly int[] _minDist = new int[]
        {
            0,
            200, 800, 200, 200, 200, 300, 384, 200,
            200, 200, 420, 900, 500, 800, 384, 384,
            384, 384, 384, 200, 384, 384, 384,   0,
            384,   0, 384, 384, 700, 384,   0,   0,
            900,
        };

        private readonly PlayerShip _ship;
        private readonly Stars _stars;
        private readonly Universe _universe;
        private readonly IDraw _draw;
        private int _direction;
        private Vector3[] _rotmat = new Vector3[3];
        private ShipType _shipNo;
        private int _showTime;

        internal Intro2View(
            GameState gameStat,
            IGraphics graphics,
            AudioController audio,
            IKeyboard keyboard,
            Stars stars,
            PlayerShip ship,
            Combat combat,
            Universe universe,
            IDraw draw)
        {
            _gameState = gameStat;
            _graphics = graphics;
            _audio = audio;
            _keyboard = keyboard;
            _stars = stars;
            _ship = ship;
            _combat = combat;
            _universe = universe;
            _draw = draw;
        }

        public void Draw()
        {
            _graphics.DrawImageCentre(Image.EliteText, _draw.Top + 10);

            _graphics.DrawTextCentre(_draw.ScannerTop - 30, "Press Fire or Space, Commander.", FontSize.Large, Colour.Gold);
            if (_universe.FirstShip != null)
            {
                _graphics.DrawTextCentre(_draw.ScannerTop - 60, _universe.FirstShip.Name, FontSize.Small, Colour.White);
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
            _showTime = 0;
            _direction = 100;

            _combat.Reset();
            _universe.ClearUniverse();
            _stars.CreateNewStars();
            _rotmat = VectorMaths.GetInitialMatrix();
            IShip missile = new Missile();
            if (!_universe.AddNewShip(missile, new(0, 0, 5000), _rotmat, -127, -127))
            {
                Debug.WriteLine("Failed to create Missile");
            }

            _audio.PlayMusic(Music.BlueDanube, true);

            _ship.Speed = 3;
            _ship.Roll = 0;
            _ship.Climb = 0;
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

                if (_universe.FirstShip.Location.Z < _minDist[(int)_shipNo])
                {
                    _universe.FirstShip.Location =
                        new(_universe.FirstShip.Location.X, _universe.FirstShip.Location.Y, _minDist[(int)_shipNo]);
                }

                if (_universe.FirstShip.Location.Z > 4500)
                {
                    do
                    {
                        _shipNo++;
                        if (_shipNo > ShipType.Dodec)
                        {
                            _shipNo = ShipType.Missile;
                        }
                    }
                    while (_minDist[(int)_shipNo] == 0);

                    _showTime = 0;
                    _direction = -100;
                    _universe.ClearUniverse();
                    if (!_universe.AddNewShip(ShipFactory.Create(_shipNo), new(0, 0, 4500), _rotmat, -127, -127))
                    {
                        Debug.WriteLine("Failed to create Parade ship");
                    }
                }
            }

            _stars.FrontStarfield();
        }
    }
}
