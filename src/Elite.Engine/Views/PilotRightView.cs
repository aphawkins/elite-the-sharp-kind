namespace Elite.Engine.Views
{
    internal class PilotRightView : IView
    {
        private readonly PilotView _pilotView;
        private readonly GameState _gameState;
        private readonly Stars _stars;

        internal PilotRightView(GameState gameState, IGfx gfx, IKeyboard keyboard, Stars stars, pilot pilot)
        {
            _pilotView = new(gameState, gfx, keyboard, pilot);
            _gameState = gameState;
            _stars = stars;
        }

        public void Draw()
        {
            _pilotView.Draw();
            _pilotView.DrawViewName("Right View");
            _pilotView.DrawLaserSights(_gameState.cmdr.front_laser.Type);
        }

        public void HandleInput()
        {
            _pilotView.HandleInput();
        }

        public void Reset()
        {
            _pilotView.Reset();
        }

        public void UpdateUniverse()
        {
            _pilotView.UpdateUniverse();
            _stars.RightStarfield();
        }
    }
}
