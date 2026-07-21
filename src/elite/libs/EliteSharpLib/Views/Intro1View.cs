// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using EliteSharpLib.Audio;
using EliteSharpLib.Conflict;
using EliteSharpLib.Graphics;
using EliteSharpLib.Ships;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
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
    private readonly ILogger<Intro1View> _logger;
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
        IShipFactory shipFactory,
        ILogger<Intro1View>? logger = null)
    {
        _gameState = gameState;
        _audio = audio;
        _keyboard = keyboard;
        _ship = ship;
        _combat = combat;
        _universe = universe;
        _draw = draw;
        _shipFactory = shipFactory;
        _logger = logger ?? NullLogger<Intro1View>.Instance;

        _colorGold = draw.Palette["Gold"];
        _colorWhite = draw.Palette["White"];
    }

    public void Draw()
    {
        _draw.Graphics.DrawImageCentre(nameof(ImageType.EliteText), _draw.Top + 10);

        _draw.Graphics
            .DrawTextCentre(_draw.ScannerTop - 90, "Original Game (C) I.Bell & D.Braben", nameof(FontType.Small), _colorWhite);
        _draw.Graphics.DrawTextCentre(_draw.ScannerTop - 70, "The New Kind - Christian Pinder", nameof(FontType.Small), _colorWhite);
        _draw.Graphics.DrawTextCentre(_draw.ScannerTop - 50, "The Sharp Kind - Andy Hawkins", nameof(FontType.Small), _colorWhite);
        _draw.Graphics.DrawTextCentre(_draw.ScannerTop - 30, "Load New Commander (Y/N)?", nameof(FontType.Large), _colorGold);
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

        Matrix4x4 initMatrix = VectorMaths.GetLeftHandedBasisMatrix;

        // Ship faces away
        initMatrix.M33 = 1;
        IShip cobraMk3 = _shipFactory.CreateShip("CobraMk3");
        if (!_universe.AddNewShip(cobraMk3, new(0, 0, 4500, 0), initMatrix, -127, 127))
        {
            LogMessages.FailedToCreateShip(_logger, "CobraMk3");
        }

        _audio.PlayMusic(nameof(MusicType.EliteTheme), true);
    }

    public void Update()
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
