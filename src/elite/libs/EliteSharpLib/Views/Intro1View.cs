// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Diagnostics;
using System.Numerics;
using EliteSharpLib.Audio;
using EliteSharpLib.Conflict;
using EliteSharpLib.Graphics;
using EliteSharpLib.Ships;
using Useful.Audio;
using Useful.Controls;
using Useful.Maths;

namespace EliteSharpLib.Views;

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
    private readonly IEliteDraw _draw;
    private readonly IShipFactory _shipFactory;
    private readonly uint _colorGold;
    private readonly uint _colorWhite;

    internal Intro1View(
        GameState gameState,
        AudioController audio,
        IKeyboard keyboard,
        PlayerShip ship,
        Combat combat,
        Universe universe,
        IEliteDraw draw,
        IShipFactory shipFactory)
    {
        _gameState = gameState;
        _audio = audio;
        _keyboard = keyboard;
        _ship = ship;
        _combat = combat;
        _universe = universe;
        _draw = draw;
        _shipFactory = shipFactory;

        _colorGold = draw.Palette["Gold"];
        _colorWhite = draw.Palette["White"];
    }

    public void Draw()
    {
        _draw.Graphics.DrawImageCentre((int)ImageType.EliteText, _draw.Top + 10);

        _draw.Graphics
            .DrawTextCentre(_draw.ScannerTop - 90, "Original Game (C) I.Bell & D.Braben", (int)FontType.Small, _colorWhite);
        _draw.Graphics.DrawTextCentre(_draw.ScannerTop - 70, "The New Kind - Christian Pinder", (int)FontType.Small, _colorWhite);
        _draw.Graphics.DrawTextCentre(_draw.ScannerTop - 50, "The Sharp Kind - Andy Hawkins", (int)FontType.Small, _colorWhite);
        _draw.Graphics.DrawTextCentre(_draw.ScannerTop - 30, "Load New Commander (Y/N)?", (int)FontType.Large, _colorGold);
    }

    public void HandleInput()
    {
        if (_keyboard.IsPressed(ConsoleKey.Y))
        {
            _combat.Reset();
            _universe.ClearUniverse();
            _audio.StopMusic();
            _gameState.SetView(Screen.LoadCommander);
        }

        if (_keyboard.IsPressed(ConsoleKey.N))
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

        Vector4[] initMatrix = VectorMaths.GetLeftHandedBasisMatrix.ToVector4Array();

        // Ship faces away
        initMatrix[2].Z = 1;
        IShip cobraMk3 = _shipFactory.CreateShip("CobraMk3");
        if (!_universe.AddNewShip(cobraMk3, new(0, 0, 4500, 0), initMatrix.ToMatrix4x4(), -127, 127))
        {
            Debug.WriteLine("Failed to create CobraMk3");
        }

        _audio.PlayMusic((int)MusicType.EliteTheme, true);
    }

    public void UpdateUniverse()
    {
        _ship.Roll = 1;
        _universe.FirstShip!.Location =
            new(_universe.FirstShip!.Location.X, _universe.FirstShip!.Location.Y, _universe.FirstShip!.Location.Z - 100, 0);

        if (_universe.FirstShip!.Location.Z < 384)
        {
            _universe.FirstShip!.Location =
                new(_universe.FirstShip!.Location.X, _universe.FirstShip!.Location.Y, 384, 0);
        }
    }
}
