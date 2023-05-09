using Elite.Common.Enums;
using Elite.Engine.Enums;
using Elite.Engine.Ships;

namespace Elite.Engine.Views
{
    internal sealed class GameOverView : IView
    {
        private readonly GameState _gameState;
        private readonly IGfx _gfx;
        private readonly Audio _audio;
        private readonly Stars _stars;
        private readonly PlayerShip _ship;
        private readonly Combat _combat;
        private int _i;

        internal GameOverView(GameState gameState, IGfx gfx, Audio audio, Stars stars, PlayerShip ship, Combat combat)
        {
            _gameState = gameState;
            _gfx = gfx;
            _audio = audio;
            _stars = stars;
            _ship = ship;
            _combat = combat;
        }

        public void Draw() => _gfx.DrawTextCentre(190, "GAME OVER", 140, GFX_COL.GFX_COL_GOLD);

        public void HandleInput()
        {
        }

        public void Reset()
        {
            _i = 0;
            _ship.Speed = 6;
            _ship.Roll = 0;
            _ship.Climb = 0;
            _combat.ClearUniverse();
            int newship = _combat.AddNewShip(ShipType.CobraMk3, new(0, 0, -400), VectorMaths.GetInitialMatrix(), 0, 0);
            Space.s_universe[newship].Flags |= ShipFlags.Dead;

            // Cargo
            for (int i = 0; i < 5; i++)
            {
                ShipType type = RNG.TrueOrFalse() ? ShipType.Cargo : ShipType.Alloy;
                newship = _combat.AddNewShip(type, new(RNG.Random(-32, 31), RNG.Random(-32, 31), -400), VectorMaths.GetInitialMatrix(), 0, 0);
                Space.s_universe[newship].RotZ = ((RNG.Random(255) * 2) & 255) - 128;
                Space.s_universe[newship].RotX = ((RNG.Random(255) * 2) & 255) - 128;
                Space.s_universe[newship].Velocity = RNG.Random(15);
            }

            _audio.PlayEffect(SoundEffect.Gameover);
        }

        public void UpdateUniverse()
        {
            if (_i >= 100)
            {
                _gameState.IsInitialised = false;
            }

            _stars.RearStarfield();
            _i++;
        }
    }
}
