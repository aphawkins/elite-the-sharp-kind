namespace Elite.Engine.Views
{
    using System.Numerics;
    using Elite.Common.Enums;
    using Elite.Engine.Enums;

    internal class EscapePod : IView
    {
        private readonly IGfx _gfx;
        private readonly Audio _audio;
        private readonly Stars _stars;
        private int _newship;
        private int _i;

        internal EscapePod(IGfx gfx, Audio audio, Stars stars)
        {
            _gfx = gfx;
            _audio = audio;
            _stars = stars;
        }

        public void Draw()
        {
            if (_i < 90)
            {
                _gfx.DrawTextCentre(358, "Escape pod launched - Ship auto-destuct initiated.", 120, GFX_COL.GFX_COL_WHITE);
            }
        }

        public void HandleInput()
        {
        }

        public void Reset()
        {
            elite.flight_speed = 1;
            elite.flight_roll = 0;
            elite.flight_climb = 0;

            Vector3[] rotmat = new Vector3[3];
            VectorMaths.set_init_matrix(ref rotmat);
            rotmat[2].Z = 1;

            _newship = swat.add_new_ship(SHIP.SHIP_COBRA3, new(0, 0, 200), rotmat, -127, -127);
            space.universe[_newship].velocity = 7;
            _audio.PlayEffect(SoundEffect.Launch);

            _i = 0;
        }

        public void UpdateUniverse()
        {
            if (_i < 90)
            {
                if (_i == 40)
                {
                    space.universe[_newship].flags |= FLG.FLG_DEAD;
                    _audio.PlayEffect(SoundEffect.Explode);
                }
                
                _stars.front_starfield();
                space.universe[_newship].location.X = 0;
                space.universe[_newship].location.Y = 0;
                space.universe[_newship].location.Z += 2;
                _i++;
            }
            else if ((space.ship_count[SHIP.SHIP_CORIOLIS] == 0) && (space.ship_count[SHIP.SHIP_DODEC] == 0))
            {
                elite.auto_dock();

                if ((MathF.Abs(elite.flight_roll) < 3) && (MathF.Abs(elite.flight_climb) < 3))
                {
                    for (int i = 0; i < elite.MAX_UNIV_OBJECTS; i++)
                    {
                        if (space.universe[i].type != 0)
                        {
                            space.universe[i].location.Z -= 1500;
                        }
                    }
                }

                Stars.warp_stars = true;
                _stars.front_starfield();
            }
            else
            {
                elite.cmdr.escape_pod = false;
                elite.cmdr.legal_status = 0;
                elite.cmdr.fuel = elite.myship.max_fuel;

                for (int i = 0; i < trade.stock_market.Length; i++)
                {
                    elite.cmdr.current_cargo[i] = 0;
                }

                elite.SetView(SCR.SCR_DOCKING);
            }
        }
    }
}