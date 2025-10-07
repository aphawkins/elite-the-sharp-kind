// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Diagnostics;
using EliteSharp.Audio;
using EliteSharp.Conflict;
using EliteSharp.Graphics;
using EliteSharp.Ships;
using Useful.Audio;

namespace EliteSharp.Views;

internal sealed class GameOverView : IView
{
    private readonly AudioController _audio;
    private readonly Combat _combat;
    private readonly GameState _gameState;
    private readonly PlayerShip _ship;
    private readonly Stars _stars;
    private readonly Universe _universe;
    private readonly IEliteDraw _draw;
    private int _i;

    internal GameOverView(
        GameState gameState,
        AudioController audio,
        Stars stars,
        PlayerShip ship,
        Combat combat,
        Universe universe,
        IEliteDraw draw)
    {
        _gameState = gameState;
        _audio = audio;
        _stars = stars;
        _ship = ship;
        _combat = combat;
        _universe = universe;
        _draw = draw;
    }

    public void Draw() => _draw.Graphics.DrawTextCentre(_draw.Centre.Y, "GAME OVER", nameof(FontType.Large), EliteColors.Gold);

    public void HandleInput()
    {
    }

    public void Reset()
    {
        _i = 0;
        _ship.Speed = 6;
        _ship.Roll = 0;
        _ship.Climb = 0;
        _combat.Reset();
        _universe.ClearUniverse();
        CobraMk3 cobraMk3 = new(_draw);
        if (!_universe.AddNewShip(cobraMk3, new(0, 0, -400), VectorMaths.GetInitialMatrix(), 0, 0))
        {
            Debug.WriteLine("Failed to create CobraMk3");
        }

        cobraMk3.Flags |= ShipProperties.Dead;

        // Cargo
        for (int i = 0; i < 5; i++)
        {
            IShip cargo = RNG.TrueOrFalse() ? new CargoCannister(_draw) : new Alloy(_draw);
            if (!_universe.AddNewShip(cargo, new(RNG.Random(-32, 32), RNG.Random(-32, 32), -400), VectorMaths.GetInitialMatrix(), 0, 0))
            {
                Debug.WriteLine("Failed to create Cargo");
            }

            cargo.RotZ = ((RNG.Random(256) * 2) & 255) - 128;
            cargo.RotX = ((RNG.Random(256) * 2) & 255) - 128;
            cargo.Velocity = RNG.Random(16);
        }

        _audio.PlayEffect(nameof(SoundEffect.Gameover));
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
