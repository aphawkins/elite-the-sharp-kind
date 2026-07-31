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
/// The 16-bit HUD, laid out against the 640x129 scanner bitmap.
/// </summary>
/// <remarks>
/// When the tier widened from 512 to 640 the art was re-laid-out rather than
/// stretched: the left dial panel stayed put (its edge is at x=97-100 in both),
/// the radar's centre line moved half the increase (253 to 317), and everything
/// from the compass rightwards moved the full 128 - the right panel's edge
/// 411 to 539, and the compass ring's centre 386.5 to 514.5. So the left
/// cluster's offsets below are unchanged and the right cluster's are +128.
/// </remarks>
internal sealed class Scanner16Bit : ScannerBase
{
    internal Scanner16Bit(GameState gameState, IEliteDraw draw, Universe universe, PlayerShip ship, Combat combat)
        : base(gameState, draw, universe, ship, combat)
    {
        ArgumentNullException.ThrowIfNull(draw);

        DialTopColor = draw.Palette["Goldenrod"];
        DialBodyColor = draw.Palette["DarkGoldenrod"];
        DialBottomColor = draw.Palette["DarkRed"];
        SpeedWarningColor = draw.Palette["DarkRed"];
        StationColor = draw.Palette["Green"];
        MissileColor = draw.Palette["Plum"];
        PoliceColor = draw.Palette["DarkSlateBlue"];
        HostileColor = draw.Palette["Yellow"];
        DefaultColor = draw.Palette["White"];
    }

    protected override Vector2 ScannerCentre => new(Draw.Layout.ViewportCentre.X - 3, Draw.Layout.ViewportHeight + 63);

    protected override (float Y, float X) ScannerExtent => (28, 50);

    protected override int DialBarHeight => 8;

    protected override float DialBarWidth => 64;

    protected override float IndicatorTravel => 28;

    protected override int IndicatorWidth => 4;

    protected override Vector2 ShieldFrontPosition => new(31, 7);

    protected override Vector2 ShieldRearPosition => new(31, 23);

    protected override Vector2 FuelPosition => new(31, 44);

    protected override Vector2 CabinTempPosition => new(31, 60);

    protected override Vector2 LaserTempPosition => new(31, 76);

    protected override Vector2 AltitudePosition => new(31, 92);

    protected override Vector2 EnergyPosition => new(544, 61);

    protected override float EnergyBankSpacing => 18;

    protected override Vector2 SpeedPosition => new(545, 9);

    protected override float SpeedHeight => 6;

    protected override Vector2 RollPosition => new(544, 9 + 14);

    protected override Vector2 ClimbPosition => new(544, 9 + 14 + 16);

    protected override Vector2 CompassCentre => new(514, 26);

    protected override float CompassRadius => 16;

    protected override float CompassDotRadius => 5;

    protected override Vector2 MissilePosition => new(35, 113);

    protected override float MissileSpacing => 16;

    protected override Vector2 StationIndicatorPosition => new(515, 105);

    protected override Vector2 EcmIndicatorPosition => new(115, 105);

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
