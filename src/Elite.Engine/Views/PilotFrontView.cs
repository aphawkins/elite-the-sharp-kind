// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using Elite.Engine.Ships;

namespace Elite.Engine.Views
{
    internal sealed class PilotFrontView : IView
    {
        private readonly PilotView _pilotView;
        private readonly PlayerShip _ship;
        private readonly Stars _stars;

        internal PilotFrontView(GameState gameState, IGfx gfx, IKeyboard keyboard, Stars stars, Pilot pilot, PlayerShip ship)
        {
            _pilotView = new(gameState, gfx, keyboard, pilot, ship, stars);
            _stars = stars;
            _ship = ship;
        }

        public void Draw()
        {
            _pilotView.Draw();
            _pilotView.DrawViewName("Front View");
            _pilotView.DrawLaserSights(_ship.LaserFront.Type);
        }

        public void HandleInput() => _pilotView.HandleInput();

        public void Reset() => _pilotView.Reset();

        public void UpdateUniverse()
        {
            _pilotView.UpdateUniverse();
            _stars.FrontStarfield();
        }
    }
}
