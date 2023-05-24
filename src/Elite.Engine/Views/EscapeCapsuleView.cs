// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using Elite.Engine.Conflict;
using Elite.Engine.Enums;
using Elite.Engine.Ships;
using Elite.Engine.Trader;

namespace Elite.Engine.Views
{
    internal sealed class EscapeCapsuleView : IView
    {
        private readonly Audio _audio;
        private readonly Combat _combat;
        private readonly GameState _gameState;
        private readonly IGraphics _graphics;
        private readonly Pilot _pilot;
        private readonly PlayerShip _ship;
        private readonly Stars _stars;
        private readonly Trade _trade;
        private readonly Universe _universe;
        private int _i;
        private int _newship;

        internal EscapeCapsuleView(
            GameState gameState,
            IGraphics graphics,
            Audio audio,
            Stars stars,
            PlayerShip ship,
            Trade trade,
            Combat combat,
            Universe universe,
            Pilot pilot)
        {
            _gameState = gameState;
            _graphics = graphics;
            _audio = audio;
            _stars = stars;
            _ship = ship;
            _trade = trade;
            _combat = combat;
            _universe = universe;
            _pilot = pilot;
        }

        public void Draw()
        {
            if (_i < 90)
            {
                _graphics.DrawTextCentre(358, "Escape capsule launched - Ship auto-destuct initiated.", 120, Colour.White);
            }
        }

        public void HandleInput()
        {
        }

        public void Reset()
        {
            _ship.Speed = 1;
            _ship.Roll = 0;
            _ship.Climb = 0;
            Vector3[] rotmat = VectorMaths.GetInitialMatrix();
            rotmat[2].Z = 1;
            _newship = _combat.AddNewShip(ShipType.CobraMk3, new(0, 0, 200), rotmat, -127, -127);
            _universe.Objects[_newship].Velocity = 7;
            _audio.PlayEffect(SoundEffect.Launch);
            _i = 0;
        }

        public void UpdateUniverse()
        {
            if (_i < 90)
            {
                if (_i == 40)
                {
                    _universe.Objects[_newship].Flags |= ShipFlags.Dead;
                    _audio.PlayEffect(SoundEffect.Explode);
                }

                _stars.FrontStarfield();
                _universe.Objects[_newship].Location = new(0, 0, _universe.Objects[_newship].Location.Z + 2);
                _i++;
            }
            else if ((_universe.ShipCount[ShipType.Coriolis] == 0) && (_universe.ShipCount[ShipType.Dodec] == 0))
            {
                _pilot.AutoDock();

                if ((MathF.Abs(_ship.Roll) < 3) && (MathF.Abs(_ship.Climb) < 3))
                {
                    for (int i = 0; i < EliteMain.MaxUniverseObjects; i++)
                    {
                        if (_universe.Objects[i].Type != 0)
                        {
                            _universe.Objects[i].Location = new(_universe.Objects[i].Location.X, _universe.Objects[i].Location.Y, _universe.Objects[i].Location.Z - 1500);
                        }
                    }
                }

                _stars.WarpStars = true;
                _stars.FrontStarfield();
            }
            else
            {
                _ship.HasEscapeCapsule = false;
                _gameState.Cmdr.LegalStatus = 0;
                _ship.Fuel = _ship.MaxFuel;
                _trade.ClearCurrentCargo();
                _gameState.SetView(Screen.Docking);
            }
        }
    }
}
