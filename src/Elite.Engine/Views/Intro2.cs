namespace Elite.Engine.Views
{
    using System.Numerics;
    using Elite.Common.Enums;
    using Elite.Engine.Enums;
    using Elite.Engine.Ships;

    /// <summary>
    /// Parade of the various ships.
    /// </summary>
    internal class Intro2 : IView
    {
        private readonly IGfx _gfx;
        private readonly Audio _audio;
        private readonly IKeyboard _keyboard;
        private readonly Stars _stars;
        private SHIP _shipNo;
        private int _showTime;
        private int _direction;
        private readonly int[] _minDist = new int[]
        {
            0,
            200, 800, 200, 200, 200, 300, 384, 200,
            200, 200, 420, 900, 500, 800, 384, 384,
            384, 384, 384, 200, 384, 384, 384,   0,
            384,   0, 384, 384, 700, 384,   0,   0,
            900
        };
        private Vector3[] _rotmat = new Vector3[3];

        internal Intro2(IGfx gfx, Audio audio, IKeyboard keyboard, Stars stars)
        {
            _gfx = gfx;
            _audio = audio;
            _keyboard = keyboard;
            _stars = stars;
        }

        public void Reset()
        {
            _shipNo = 0;
            _showTime = 0;
            _direction = 100;

            swat.clear_universe();
            Stars.create_new_stars();
            _rotmat = VectorMaths.GetInitialMatrix();
            swat.add_new_ship(SHIP.SHIP_MISSILE, new(0, 0, 5000), _rotmat, -127, -127);
            _audio.PlayMusic(Music.BlueDanube, true);

            elite.flight_speed = 3;
            elite.flight_roll = 0;
            elite.flight_climb = 0;
        }

        public void UpdateUniverse()
        {
            _showTime++;

            if (_showTime >= 140 && _direction < 0)
            {
                _direction = -_direction;
            }

            space.universe[0].location.Z += _direction;

            if (space.universe[0].location.Z < _minDist[(int)_shipNo])
            {
                space.universe[0].location.Z = _minDist[(int)_shipNo];
            }

            if (space.universe[0].location.Z > 4500)
            {
                do
                {
                    _shipNo++;
                    if ((int)_shipNo > shipdata.NO_OF_SHIPS)
                    {
                        _shipNo = SHIP.SHIP_MISSILE;
                    }
                } while (_minDist[(int)_shipNo] == 0);

                _showTime = 0;
                _direction = -100;

                space.ship_count[space.universe[0].type] = 0;
                space.universe[0].type = SHIP.SHIP_NONE;

                swat.add_new_ship(_shipNo, new(0, 0, 4500), _rotmat, -127, -127);
            }

            _stars.front_starfield();
        }

        public void Draw()
        {
            _gfx.DrawImage(Image.EliteText, new(-1, 10));

            _gfx.DrawTextCentre(360, "Press Fire or Space, Commander.", 140, GFX_COL.GFX_COL_GOLD);
            _gfx.DrawTextCentre(330, elite.ship_list[(int)_shipNo].name, 120, GFX_COL.GFX_COL_WHITE);
        }

        public void HandleInput()
        {
            if (_keyboard.IsKeyPressed(CommandKey.Space))
            {
                swat.clear_universe();
                _audio.StopMusic();
                elite.SetView(SCR.SCR_CMDR_STATUS);
            }
        }
    }
}