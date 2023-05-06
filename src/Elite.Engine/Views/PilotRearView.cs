using Elite.Engine.Ships;

namespace Elite.Engine.Views
{
    internal class PilotRearView : IView
    {
        private readonly PilotView _pilotView;
        private readonly Stars _stars;
        private readonly PlayerShip _ship;

        internal PilotRearView(GameState gameState, IGfx gfx, IKeyboard keyboard, Stars stars, Pilot pilot, PlayerShip ship)
        {
            _pilotView = new(gameState, gfx, keyboard, pilot, ship);
            _stars = stars;
            _ship = ship;
        }

        public void Draw()
        {
            _pilotView.Draw();
            _pilotView.DrawViewName("Rear View");
            _pilotView.DrawLaserSights(_ship.LaserFront.Type);
        }

        public void HandleInput() => _pilotView.HandleInput();

        public void Reset() => _pilotView.Reset();

        public void UpdateUniverse()
        {
            _pilotView.UpdateUniverse();
            _stars.RearStarfield();
        }
    }
}
