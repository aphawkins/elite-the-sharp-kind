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
        private readonly IDraw _draw;
        private readonly GameState _gameState;
        private readonly IGraphics _graphics;
        private readonly IKeyboard _keyboard;
        private readonly PlayerShip _ship;
        private readonly List<(IShip Ship, int MinDistance)> _shipDistances = new()
        {
            { (new Missile(), 200) },
            { (new Coriolis(), 800) },
            { (new EscapeCapsule(), 200) },
            { (new Alloy(), 200) },
            { (new CargoCannister(), 200) },
            { (new Boulder(), 300) },
            { (new Asteroid(), 384) },
            { (new RockSplinter(), 200) },
            { (new Shuttle(), 200) },
            { (new Transporter(), 200) },
            { (new CobraMk3(), 420) },
            { (new Python(), 900) },
            { (new Boa(), 500) },
            { (new Anaconda(), 800) },
            { (new RockHermit(), 384) },
            { (new Viper(), 384) },
            { (new Sidewinder(), 384) },
            { (new Mamba(), 384) },
            { (new Krait(), 384) },
            { (new Adder(), 200) },
            { (new Gecko(), 384) },
            { (new CobraMk1(), 384) },
            { (new Worm(), 384) },
            { (new AspMk2(), 384) },
            { (new FerDeLance(), 384) },
            { (new Moray(), 384) },
            { (new Thargoid(), 700) },
            { (new Tharglet(), 384) },
            { (new DodecStation(), 900) },
        };

        private readonly Stars _stars;
        private readonly Universe _universe;
        private int _direction;
        private Vector3[] _rotmat = new Vector3[3];
        private int _shipNo;
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
            _ship.Speed = 3;
            _ship.Roll = 0;
            _ship.Climb = 0;
            _combat.Reset();
            _stars.CreateNewStars();
            _rotmat = VectorMaths.GetInitialMatrix();
            _audio.PlayMusic(Music.BlueDanube, true);

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

                if (_universe.FirstShip.Location.Z < _shipDistances[_shipNo].MinDistance)
                {
                    _universe.FirstShip.Location =
                        new(_universe.FirstShip.Location.X, _universe.FirstShip.Location.Y, _shipDistances[_shipNo].MinDistance);
                }

                if (_universe.FirstShip.Location.Z > 4500)
                {
                    _shipNo++;
                    if (_shipNo >= _shipDistances.Count)
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
            if (!_universe.AddNewShip(_shipDistances[_shipNo].Ship, new(0, 0, 4500), _rotmat, -127, -127))
            {
                Debug.WriteLine("Failed to create first Parade ship");
            }
        }
    }
}
