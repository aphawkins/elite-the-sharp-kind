// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Diagnostics;
using System.Numerics;
using EliteSharp.Assets.Fonts;
using EliteSharp.Audio;
using EliteSharp.Graphics;
using EliteSharp.Ships;
using EliteSharp.Trader;

namespace EliteSharp.Views;

internal sealed class EscapeCapsuleView : IView
{
    private readonly AudioController _audio;
    private readonly GameState _gameState;
    private readonly Pilot _pilot;
    private readonly PlayerShip _ship;
    private readonly Stars _stars;
    private readonly Trade _trade;
    private readonly Universe _universe;
    private readonly IDraw _draw;
    private int _i;
    private IShip _newship;

    internal EscapeCapsuleView(
        GameState gameState,
        AudioController audio,
        Stars stars,
        PlayerShip ship,
        Trade trade,
        Universe universe,
        Pilot pilot,
        IDraw draw)
    {
        _gameState = gameState;
        _audio = audio;
        _stars = stars;
        _ship = ship;
        _trade = trade;
        _universe = universe;
        _pilot = pilot;
        _draw = draw;
        _newship = new ShipBase(draw);
    }

    public void Draw()
    {
        if (_i < 90)
        {
            _draw.Graphics.DrawTextCentre(
                _draw.ScannerTop - 40,
                "Escape capsule launched - Ship auto-destuct initiated.",
                FontType.Small,
                EliteColors.White);
        }
    }

    public void HandleInput()
    {
    }

    public void Reset()
    {
        _ship.Speed = 1;
        _ship.Roll = 0;
        _ship.Climb = 0;
        Vector3[] rotmat = VectorMaths.GetInitialMatrix();
        rotmat[2].Z = 1;
        _newship = new CobraMk3(_draw);
        if (!_universe.AddNewShip(_newship, new(0, 0, 200), rotmat, -127, -127))
        {
            Debug.Fail("Failed to create CobraMk3");
        }

        _newship.Velocity = 7;
        _audio.PlayEffect(SoundEffect.Launch);
        _i = 0;
    }

    public void UpdateUniverse()
    {
        if (_i < 90)
        {
            if (_i == 40)
            {
                _newship.Flags |= ShipProperties.Dead;
                _audio.PlayEffect(SoundEffect.Explode);
            }

            _stars.FrontStarfield();
            _newship.Location = new(0, 0, _newship.Location.Z + 2);
            _i++;
        }
        else if (!_universe.IsStationPresent)
        {
            _pilot.AutoDock();

            if ((MathF.Abs(_ship.Roll) < 3) && (MathF.Abs(_ship.Climb) < 3))
            {
                foreach (IObject universeObj in _universe.GetAllObjects())
                {
                    if (universeObj.Type != 0)
                    {
                        universeObj.Location = new(universeObj.Location.X, universeObj.Location.Y, universeObj.Location.Z - 1500);
                    }
                }
            }

            _stars.WarpStars = true;
            _stars.FrontStarfield();
        }
        else
        {
            _ship.HasEscapeCapsule = false;
            _gameState.Cmdr.LegalStatus = 0;
            _ship.Fuel = _ship.MaxFuel;
            _trade.ClearCurrentCargo();
            _gameState.SetView(Screen.Docking);
        }
    }
}
