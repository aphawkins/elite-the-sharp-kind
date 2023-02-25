namespace Elite.Engine.Views
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Elite.Common.Enums;
    using Elite.Engine.Enums;
    using Elite.Engine.Types;

    internal class GameOverView : IView
    {
        private readonly IGfx _gfx;
        private readonly Audio _audio;
        private readonly Stars _stars;
        private int _i;

        internal GameOverView(IGfx gfx, Audio audio, Stars stars)
        {
            _gfx = gfx;
            _audio = audio;
            _stars = stars;
        }

        public void Draw()
        {
            _gfx.DrawTextCentre(190, "GAME OVER", 140, GFX_COL.GFX_COL_GOLD);
        }

        public void HandleInput()
        {
        }

        public void Reset()
        {
            _i = 0;
            elite.flight_speed = 6;
            elite.flight_roll = 0;
            elite.flight_climb = 0;
            swat.clear_universe();
            int newship = swat.add_new_ship(SHIP.SHIP_COBRA3, new(0, 0, -400), VectorMaths.GetInitialMatrix(), 0, 0);
            space.universe[newship].flags |= FLG.FLG_DEAD;

            // Cargo
            for (int i = 0; i < 5; i++)
            {
                SHIP type = RNG.TrueOrFalse() ? SHIP.SHIP_CARGO : SHIP.SHIP_ALLOY;
                newship = swat.add_new_ship(type, new(RNG.Random(-32, 31), RNG.Random(-32, 31), -400), VectorMaths.GetInitialMatrix(), 0, 0);
                space.universe[newship].rotz = ((RNG.Random(255) * 2) & 255) - 128;
                space.universe[newship].rotx = ((RNG.Random(255) * 2) & 255) - 128;
                space.universe[newship].velocity = RNG.Random(15);
            }

            _audio.PlayEffect(SoundEffect.Gameover);
        }

        public void UpdateUniverse()
        {
            if (_i >= 100)
            {
                elite._state.initialised = false;
            }

            _stars.rear_starfield();
            _i++;
        }
    }
}