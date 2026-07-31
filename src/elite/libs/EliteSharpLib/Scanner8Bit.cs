// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using EliteSharpLib.Conflict;
using EliteSharpLib.Graphics;
using EliteSharpLib.Ships;
using Useful;

namespace EliteSharpLib;

/// <summary>
/// The 8-bit HUD, laid out against the 320x56 scanner bitmap. Roughly the
/// 16-bit offsets halved, since the scanner is a little under half the size in
/// each axis - first drafts against the art rather than authored positions.
/// </summary>
internal sealed class Scanner8Bit : ScannerBase
{
    internal Scanner8Bit(GameState gameState, IEliteDraw draw, Universe universe, PlayerShip ship, Combat combat)
        : base(gameState, draw, universe, ship, combat)
    {
        ArgumentNullException.ThrowIfNull(draw);

        DialTopColor = draw.Palette["Yellow"];
        DialBodyColor = draw.Palette["Orange"];
        DialBottomColor = draw.Palette["Red"];
        SpeedWarningColor = draw.Palette["Red"];
        StationColor = draw.Palette["Green"];
        MissileColor = draw.Palette["Purple"];
        PoliceColor = draw.Palette["Cyan"];
        HostileColor = draw.Palette["Yellow"];
        DefaultColor = draw.Palette["White"];
    }

    protected override Vector2 ScannerCentre => new(Draw.Layout.ViewportCentre.X - 2, Draw.Layout.ScannerTop + 28);

    protected override (float Y, float X) ScannerExtent => (14, 25);

    protected override int DialBarHeight => 4;

    protected override float DialBarWidth => 32;

    protected override float IndicatorTravel => 14;

    protected override int IndicatorWidth => 2;

    protected override Vector2 ShieldFrontPosition => new(18, 2);

    protected override Vector2 ShieldRearPosition => new(18, 10);

    protected override Vector2 FuelPosition => new(18, 18);

    protected override Vector2 CabinTempPosition => new(18, 26);

    protected override Vector2 LaserTempPosition => new(18, 33);

    protected override Vector2 AltitudePosition => new(18, 42);

    protected override Vector2 EnergyPosition => new(270, 26);

    protected override float EnergyBankSpacing => 8;

    protected override Vector2 SpeedPosition => new(270, 2);

    protected override float SpeedHeight => 4;

    // Left-aligned with the speed and energy dials: the indicators centre on
    // half a dial's width in, which used to be baked in as a fixed 32.
    protected override Vector2 RollPosition => new(270, 10);

    protected override Vector2 ClimbPosition => new(270, 18);

    protected override Vector2 CompassCentre => new(257, 10);

    protected override float CompassRadius => 8;

    protected override float CompassDotRadius => 2;

    protected override Vector2 MissilePosition => new(18, 49);

    protected override float MissileSpacing => 8;

    protected override Vector2 StationIndicatorPosition => new(255, 35);

    protected override Vector2 EcmIndicatorPosition => new(72, 46);

    protected override FastColor DialTopColor { get; }

    protected override FastColor DialBodyColor { get; }

    protected override FastColor DialBottomColor { get; }

    protected override FastColor SpeedWarningColor { get; }

    protected override FastColor StationColor { get; }

    protected override FastColor MissileColor { get; }

    protected override FastColor PoliceColor { get; }

    protected override FastColor HostileColor { get; }

    protected override FastColor DefaultColor { get; }
}
