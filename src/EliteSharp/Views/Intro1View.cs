// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Diagnostics;
using System.Numerics;
using EliteSharp.Audio;
using EliteSharp.Conflict;
using EliteSharp.Controls;
using EliteSharp.Graphics;
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
        private readonly IKeyboard _keyboard;
        private readonly PlayerShip _ship;
        private readonly Universe _universe;
        private readonly IDraw _draw;

        internal Intro1View(GameState gameState, AudioController audio, IKeyboard keyboard, PlayerShip ship, Combat combat, Universe universe, IDraw draw)
        {
            _gameState = gameState;
            _audio = audio;
            _keyboard = keyboard;
            _ship = ship;
            _combat = combat;
            _universe = universe;
            _draw = draw;
        }

        public void Draw()
        {
            _draw.Graphics.DrawImageCentre(Image.EliteText, _draw.Top + 10);

            _draw.Graphics.DrawTextCentre(_draw.ScannerTop - 90, "Original Game (C) I.Bell & D.Braben.", FontSize.Small, Colour.White);
            _draw.Graphics.DrawTextCentre(_draw.ScannerTop - 70, "The New Kind - Christian Pinder.", FontSize.Small, Colour.White);
            _draw.Graphics.DrawTextCentre(_draw.ScannerTop - 50, "The Sharp Kind - Andy Hawkins.", FontSize.Small, Colour.White);
            _draw.Graphics.DrawTextCentre(_draw.ScannerTop - 30, "Load New Commander (Y/N)?", FontSize.Large, Colour.Gold);
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
            IShip cobraMk3 = new CobraMk3(_draw);
            if (!_universe.AddNewShip(cobraMk3, new(0, 0, 4500), initMatrix, -127, 127))
            {
                Debug.WriteLine("Failed to create CobraMk3");
            }

            _audio.PlayMusic(Music.EliteTheme, true);
        }

        public void UpdateUniverse()
        {
            _ship.Roll = 1;
            _universe.FirstShip!.Location =
                new(_universe.FirstShip!.Location.X, _universe.FirstShip!.Location.Y, _universe.FirstShip!.Location.Z - 100);

            if (_universe.FirstShip!.Location.Z < 384)
            {
                _universe.FirstShip!.Location =
                    new(_universe.FirstShip!.Location.X, _universe.FirstShip!.Location.Y, 384);
            }
        }
    }
}
