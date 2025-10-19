// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Conflict;
using EliteSharp.Graphics;
using EliteSharp.Ships;
using Useful.Controls;

namespace EliteSharp.Views;

internal sealed class PilotRearView : IView
{
    private readonly PilotView _pilotView;
    private readonly PlayerShip _ship;
    private readonly Stars _stars;

    internal PilotRearView(
        GameState gameState,
        IKeyboard keyboard,
        Stars stars,
        Pilot pilot,
        PlayerShip ship,
        Space space,
        IEliteDraw draw,
        Combat combat)
    {
        _pilotView = new(gameState, keyboard, pilot, ship, stars, space, draw, combat);
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
