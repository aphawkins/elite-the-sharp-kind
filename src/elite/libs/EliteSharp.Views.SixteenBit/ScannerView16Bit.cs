// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using EliteSharp.Abstractions.Views;
using Useful;

namespace EliteSharp.Views.SixteenBit;

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
internal sealed class ScannerView16Bit : ScannerViewBase
{
    internal ScannerView16Bit(IViewSurface surface)
        : base(surface)
    {
        DialTopColor = surface.Palette["Gold"];
        DialBodyColor = surface.Palette["DarkYellow"];
        DialBottomColor = surface.Palette["LightRed"];
        SpeedWarningColor = surface.Palette["LightRed"];
        Ships = new(
            Default: surface.Palette["White"],
            Station: surface.Palette["Green"],
            Missile: surface.Palette["Lilac"],
            Police: surface.Palette["Purple"],
            Hostile: surface.Palette["Yellow"]);
    }

    protected override Vector2 ScannerCentre => new(Surface.Layout.ViewportCentre.X - 3, Surface.Layout.ViewportHeight + 63);

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

    protected override ShipColours Ships { get; }
}
