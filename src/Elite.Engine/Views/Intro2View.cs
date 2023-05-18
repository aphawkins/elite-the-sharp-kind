// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using Elite.Common.Enums;
using Elite.Engine.Conflict;
using Elite.Engine.Enums;
using Elite.Engine.Ships;

namespace Elite.Engine.Views
{
    /// <summary>
    /// Parade of the various ships.
    /// </summary>
    internal sealed class Intro2View : IView
    {
        private readonly Audio _audio;
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
        private int _direction;
        private Vector3[] _rotmat = new Vector3[3];
        private ShipType _shipNo;
        private int _showTime;

        internal Intro2View(GameState gameStat, IGraphics graphics, Audio audio, IKeyboard keyboard, Stars stars, PlayerShip ship, Combat combat)
        {
            _gameState = gameStat;
            _graphics = graphics;
            _audio = audio;
            _keyboard = keyboard;
            _stars = stars;
            _ship = ship;
            _combat = combat;
        }

        public void Draw()
        {
            _graphics.DrawImage(Image.EliteText, new(-1, 10));

            _graphics.DrawTextCentre(360, "Press Fire or Space, Commander.", 140, Colour.Gold);
            _graphics.DrawTextCentre(330, _gameState.ShipList[_shipNo].Name, 120, Colour.White);
        }

        public void HandleInput()
        {
            if (_keyboard.IsKeyPressed(CommandKey.SpaceBar))
            {
                _combat.ClearUniverse();
                _audio.StopMusic();
                _gameState.SetView(Screen.CommanderStatus);
            }
        }

        public void Reset()
        {
            _shipNo = 0;
            _showTime = 0;
            _direction = 100;

            _combat.ClearUniverse();
            _stars.CreateNewStars();
            _rotmat = VectorMaths.GetInitialMatrix();
            _combat.AddNewShip(ShipType.Missile, new(0, 0, 5000), _rotmat, -127, -127);
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

            Space.s_universe[0].Location = new(Space.s_universe[0].Location.X, Space.s_universe[0].Location.Y, Space.s_universe[0].Location.Z + _direction);

            if (Space.s_universe[0].Location.Z < _minDist[(int)_shipNo])
            {
                Space.s_universe[0].Location = new(Space.s_universe[0].Location.X, Space.s_universe[0].Location.Y, _minDist[(int)_shipNo]);
            }

            if (Space.s_universe[0].Location.Z > 4500)
            {
                do
                {
                    _shipNo++;
                    if ((int)_shipNo > _gameState.ShipList.Count)
                    {
                        _shipNo = ShipType.Missile;
                    }
                }
                while (_minDist[(int)_shipNo] == 0);

                _showTime = 0;
                _direction = -100;

                Space.s_ship_count[Space.s_universe[0].Type] = 0;
                Space.s_universe[0].Type = ShipType.None;

                _combat.AddNewShip(_shipNo, new(0, 0, 4500), _rotmat, -127, -127);
            }

            _stars.FrontStarfield();
        }
    }
}
