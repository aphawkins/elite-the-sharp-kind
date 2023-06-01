// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using EliteSharp.Audio;
using EliteSharp.Conflict;
using EliteSharp.Enums;
using EliteSharp.Ships;

namespace EliteSharp.Views
{
    /// <summary>
    /// Rolling Cobra MkIII.
    /// </summary>
    internal sealed class Intro1View : IView
    {
        private readonly AudioController _audio;
        private readonly Combat _combat;
        private readonly GameState _gameState;
        private readonly IGraphics _graphics;
        private readonly IKeyboard _keyboard;
        private readonly PlayerShip _ship;
        private readonly Universe _universe;

        internal Intro1View(GameState gameState, IGraphics graphics, AudioController audio, IKeyboard keyboard, PlayerShip ship, Combat combat, Universe universe)
        {
            _gameState = gameState;
            _graphics = graphics;
            _audio = audio;
            _keyboard = keyboard;
            _ship = ship;
            _combat = combat;
            _universe = universe;
        }

        public void Draw()
        {
            _graphics.DrawImage(Image.EliteText, new(-1, 10));

            _graphics.DrawTextCentre(310, "Original Game (C) I.Bell & D.Braben.", 120, Colour.White);
            _graphics.DrawTextCentre(330, "The New Kind - Christian Pinder.", 120, Colour.White);
            _graphics.DrawTextCentre(350, "The Sharp Kind - Andy Hawkins.", 120, Colour.White);
            _graphics.DrawTextCentre(370, "Load New Commander (Y/N)?", 140, Colour.Gold);
        }

        public void HandleInput()
        {
            if (_keyboard.IsKeyPressed(CommandKey.Yes))
            {
                _combat.Reset();
                _universe.ClearUniverse();
                _audio.StopMusic();
                _gameState.SetView(Screen.LoadCommander);
            }

            if (_keyboard.IsKeyPressed(CommandKey.No))
            {
                _combat.Reset();
                _universe.ClearUniverse();
                _audio.StopMusic();
                _gameState.SetView(Screen.IntroTwo);
            }
        }

        public void Reset()
        {
            _combat.Reset();
            _universe.ClearUniverse();

            Vector3[] initMatrix = VectorMaths.GetInitialMatrix();

            // Ship faces away
            initMatrix[2].Z = 1;
            _universe.AddNewShip(ShipType.CobraMk3, new(0, 0, 4500), initMatrix, -127, 127);
            _audio.PlayMusic(Music.EliteTheme, true);
        }

        public void UpdateUniverse()
        {
            _ship.Roll = 1;
            _universe.Planet.Location = new(_universe.Planet.Location.X, _universe.Planet.Location.Y, _universe.Planet.Location.Z - 100);

            if (_universe.Planet.Location.Z < 384)
            {
                _universe.Planet.Location = new(_universe.Planet.Location.X, _universe.Planet.Location.Y, 384);
            }
        }
    }
}
