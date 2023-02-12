namespace Elite.Engine.Views
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    internal class GameOverView : IView
    {
        private readonly IGfx _gfx;
        private readonly Stars _stars;

        internal GameOverView(IGfx gfx, Stars stars)
        {
            _gfx = gfx;
            _stars = stars;
        }

        public void Draw()
        {
        }

        public void HandleInput()
        {
        }

        public void Reset()
        {
        }

        public void UpdateUniverse()
        {
            _stars.rear_starfield();
        }
    }
}
