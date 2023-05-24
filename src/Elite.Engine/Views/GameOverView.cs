// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using Elite.Engine.Audio;
using Elite.Engine.Conflict;
using Elite.Engine.Enums;
using Elite.Engine.Ships;

namespace Elite.Engine.Views
{
    internal sealed class GameOverView : IView
    {
        private readonly AudioController _audio;
        private readonly Combat _combat;
        private readonly GameState _gameState;
        private readonly IGraphics _graphics;
        private readonly PlayerShip _ship;
        private readonly Stars _stars;
        private readonly Universe _universe;
        private int _i;

        internal GameOverView(GameState gameState, IGraphics graphics, AudioController audio, Stars stars, PlayerShip ship, Combat combat, Universe universe)
        {
            _gameState = gameState;
            _graphics = graphics;
            _audio = audio;
            _stars = stars;
            _ship = ship;
            _combat = combat;
            _universe = universe;
        }

        public void Draw() => _graphics.DrawTextCentre(190, "GAME OVER", 140, Colour.Gold);

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
            _universe.Objects[newship].Flags |= ShipFlags.Dead;

            // Cargo
            for (int i = 0; i < 5; i++)
            {
                ShipType type = RNG.TrueOrFalse() ? ShipType.Cargo : ShipType.Alloy;
                newship = _combat.AddNewShip(type, new(RNG.Random(-32, 31), RNG.Random(-32, 31), -400), VectorMaths.GetInitialMatrix(), 0, 0);
                _universe.Objects[newship].RotZ = ((RNG.Random(255) * 2) & 255) - 128;
                _universe.Objects[newship].RotX = ((RNG.Random(255) * 2) & 255) - 128;
                _universe.Objects[newship].Velocity = RNG.Random(15);
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
