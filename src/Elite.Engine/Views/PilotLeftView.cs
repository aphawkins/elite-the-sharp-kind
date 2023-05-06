using Elite.Engine.Ships;

namespace Elite.Engine.Views
{
    internal class PilotLeftView : IView
    {
        private readonly PilotView _pilotView;
        private readonly Stars _stars;
        private readonly PlayerShip _ship;

        internal PilotLeftView(GameState gameState, IGfx gfx, IKeyboard keyboard, Stars stars, Pilot pilot, PlayerShip ship)
        {
            _pilotView = new(gameState, gfx, keyboard, pilot, ship);
            _stars = stars;
            _ship = ship;
        }

        public void Draw()
        {
            _pilotView.Draw();
            _pilotView.DrawViewName("Left View");
            _pilotView.DrawLaserSights(_ship.LaserFront.Type);
        }

        public void HandleInput() => _pilotView.HandleInput();

        public void Reset() => _pilotView.Reset();

        public void UpdateUniverse()
        {
            _pilotView.UpdateUniverse();
            _stars.LeftStarfield();
        }
    }
}
