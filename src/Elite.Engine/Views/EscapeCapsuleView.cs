// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using Elite.Common.Enums;
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
        private readonly IGfx _gfx;
        private readonly PlayerShip _ship;
        private readonly Stars _stars;
        private readonly Trade _trade;
        private int _i;
        private int _newship;

        internal EscapeCapsuleView(GameState gameState, IGfx gfx, Audio audio, Stars stars, PlayerShip ship, Trade trade, Combat combat)
        {
            _gameState = gameState;
            _gfx = gfx;
            _audio = audio;
            _stars = stars;
            _ship = ship;
            _trade = trade;
            _combat = combat;
        }

        public void Draw()
        {
            if (_i < 90)
            {
                _gfx.DrawTextCentre(358, "Escape capsule launched - Ship auto-destuct initiated.", 120, Colour.White1);
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
            Space.s_universe[_newship].Velocity = 7;
            _audio.PlayEffect(SoundEffect.Launch);
            _i = 0;
        }

        public void UpdateUniverse()
        {
            if (_i < 90)
            {
                if (_i == 40)
                {
                    Space.s_universe[_newship].Flags |= ShipFlags.Dead;
                    _audio.PlayEffect(SoundEffect.Explode);
                }

                _stars.FrontStarfield();
                Space.s_universe[_newship].Location = new(0, 0, Space.s_universe[_newship].Location.Z + 2);
                _i++;
            }
            else if ((Space.s_ship_count[ShipType.Coriolis] == 0) && (Space.s_ship_count[ShipType.Dodec] == 0))
            {
                _ship.AutoDock();

                if ((MathF.Abs(_ship.Roll) < 3) && (MathF.Abs(_ship.Climb) < 3))
                {
                    for (int i = 0; i < EliteMain.MAX_UNIV_OBJECTS; i++)
                    {
                        if (Space.s_universe[i].Type != 0)
                        {
                            Space.s_universe[i].Location = new(Space.s_universe[i].Location.X, Space.s_universe[i].Location.Y, Space.s_universe[i].Location.Z - 1500);
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
                _gameState.SetView(SCR.SCR_DOCKING);
            }
        }
    }
}
