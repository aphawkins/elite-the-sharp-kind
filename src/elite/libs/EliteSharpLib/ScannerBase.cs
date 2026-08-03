// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using EliteSharp.Abstractions.Assets;
using EliteSharpLib.Conflict;
using EliteSharpLib.Graphics;
using EliteSharpLib.Ships;
using Useful;
using Useful.Maths;

namespace EliteSharpLib;

/// <summary>
/// The HUD: dials, compass and the scanner's lollipops. Every position here is
/// an offset into the tier's scanner bitmap, and the two tiers' scanners are
/// different sizes (8-bit 320x56 against 16-bit 512x129), so the layout is
/// entirely the subclass's - this base holds only the logic that reads the
/// ship and universe and decides what to draw.
/// </summary>
internal abstract class ScannerBase(
    GameState gameState,
    IEliteDraw draw,
    Universe universe,
    PlayerShip ship,
    Combat combat)
{
    private readonly Combat _combat = combat;
    private readonly GameState _gameState = gameState;
    private readonly PlayerShip _ship = ship;
    private readonly Universe _universe = universe;

    protected IEliteDraw Draw { get; } = draw;

    /// <summary>
    /// Gets the scanner's centre in screen coordinates, which the lollipops
    /// plot around.
    /// </summary>
    protected abstract Vector2 ScannerCentre { get; }

    /// <summary>
    /// Gets the half-height and half-width of the lollipop area, in the
    /// scanner's own units.
    /// </summary>
    protected abstract (float Y, float X) ScannerExtent { get; }

    /// <summary>
    /// Gets the dial bar's height in pixels, including its top and bottom rules.
    /// </summary>
    protected abstract int DialBarHeight { get; }

    /// <summary>
    /// Gets the length in pixels of a full dial bar, which is what a dial's
    /// 0-to-1 reading is scaled by.
    /// </summary>
    protected abstract float DialBarWidth { get; }

    /// <summary>
    /// Gets how far in pixels the roll and climb indicators travel either side
    /// of their dial's centre.
    /// </summary>
    protected abstract float IndicatorTravel { get; }

    /// <summary>
    /// Gets the thickness in pixels of the roll and climb indicators.
    /// </summary>
    protected abstract int IndicatorWidth { get; }

    protected abstract Vector2 ShieldFrontPosition { get; }

    protected abstract Vector2 ShieldRearPosition { get; }

    protected abstract Vector2 FuelPosition { get; }

    protected abstract Vector2 CabinTempPosition { get; }

    protected abstract Vector2 LaserTempPosition { get; }

    protected abstract Vector2 AltitudePosition { get; }

    /// <summary>
    /// Gets the topmost energy bank's position; the other three follow below it.
    /// </summary>
    protected abstract Vector2 EnergyPosition { get; }

    protected abstract float EnergyBankSpacing { get; }

    protected abstract Vector2 SpeedPosition { get; }

    protected abstract float SpeedHeight { get; }

    protected abstract Vector2 RollPosition { get; }

    protected abstract Vector2 ClimbPosition { get; }

    protected abstract Vector2 CompassCentre { get; }

    protected abstract float CompassRadius { get; }

    protected abstract float CompassDotRadius { get; }

    protected abstract Vector2 MissilePosition { get; }

    protected abstract float MissileSpacing { get; }

    protected abstract Vector2 StationIndicatorPosition { get; }

    protected abstract Vector2 EcmIndicatorPosition { get; }

    /// <summary>
    /// Gets the dial's top rule colour, which the roll, climb and speed
    /// indicators also use.
    /// </summary>
    protected abstract FastColor DialTopColor { get; }

    protected abstract FastColor DialBodyColor { get; }

    protected abstract FastColor DialBottomColor { get; }

    protected abstract FastColor SpeedWarningColor { get; }

    protected abstract FastColor StationColor { get; }

    protected abstract FastColor MissileColor { get; }

    protected abstract FastColor PoliceColor { get; }

    protected abstract FastColor HostileColor { get; }

    protected abstract FastColor DefaultColor { get; }

    internal void DrawScanner()
        => Draw.Graphics.DrawImage(nameof(ImageType.Scanner), new(Draw.Layout.ViewportLeft, Draw.Layout.ViewportHeight));

    internal void UpdateConsole()
    {
        DrawScanner();
        DisplaySpeed();
        DisplayFlightClimb();
        DisplayFlightRoll();
        DisplayShields();
        DisplayAltitude();
        DisplayEnergy();
        DisplayCabinTemp();
        DisplayLaserTemp();
        DisplayFuel();
        DisplayMissiles();

        if (_gameState.IsDocked)
        {
            return;
        }

        UpdateScanner();
        UpdateCompass();

        if (_universe.IsStationPresent)
        {
            Draw.Graphics.DrawImage(nameof(ImageType.Station), ScannerRelative(StationIndicatorPosition));
        }

        if (_ship.EcmActive != 0)
        {
            Draw.Graphics.DrawImage(nameof(ImageType.ECM), ScannerRelative(EcmIndicatorPosition));
        }
    }

    /// <summary>
    /// Turns a scanner-relative position into a screen one.
    /// </summary>
    protected Vector2 ScannerRelative(Vector2 position)
        => new(Draw.Layout.ViewportLeft + position.X, Draw.Layout.ViewportHeight + position.Y);

    private void DisplayAltitude()
    {
        if (_ship.Altitude > PlayerShip.AltitudeMin)
        {
            DisplayDialBar(_ship.Altitude * DialBarWidth, AltitudePosition);
        }
    }

    private void DisplayCabinTemp()
    {
        if (_ship.CabinTemperature > PlayerShip.TemperatureMin)
        {
            DisplayDialBar(_ship.CabinTemperature * DialBarWidth, CabinTempPosition);
        }
    }

    /// <summary>
    /// Draw an indicator bar. Used for shields and energy banks.
    /// </summary>
    private void DisplayDialBar(float len, Vector2 position)
    {
        Vector2 origin = ScannerRelative(position);
        float x = origin.X;
        float y = origin.Y;
        int last = DialBarHeight - 1;

        Draw.Graphics.DrawLine(new(x, y), new(x + len - 1, y), DialTopColor);
        int i = 1;
        Draw.Graphics.DrawLine(new(x, y + i), new(x + len - 1, y + i), DialTopColor);

        for (i = 2; i < last; i++)
        {
            Draw.Graphics.DrawLine(new(x, y + i), new(x + len - 1, y + i), DialBodyColor);
        }

        Draw.Graphics.DrawLine(new(x, y + i), new(x + len - 1, y + i), DialBottomColor);
    }

    /// <summary>
    /// Display the energy banks. The ship's energy is a single fraction of
    /// maximum, split across four banks that fill from the bottom one up.
    /// </summary>
    private void DisplayEnergy()
    {
        const int bankCount = 4;
        Vector2 bank = EnergyPosition;

        for (int i = 0; i < bankCount; i++)
        {
            // Bank 0 is the topmost, so it holds the last quarter to fill.
            float fill = Math.Clamp((_ship.Energy * bankCount) - (bankCount - 1 - i), 0, 1);

            if (fill > 0)
            {
                DisplayDialBar(fill * DialBarWidth, bank with { Y = bank.Y + (i * EnergyBankSpacing) });
            }
        }
    }

    private void DisplayFlightClimb()
    {
        Vector2 origin = ScannerRelative(ClimbPosition);
        DisplayIndicator(_ship.Climb / _ship.MaxClimb, origin);
    }

    private void DisplayFlightRoll()
    {
        Vector2 origin = ScannerRelative(RollPosition);

        // Roll reads the other way round: a roll to starboard slides left.
        DisplayIndicator(-_ship.Roll / _ship.MaxRoll, origin);
    }

    /// <summary>
    /// Draw the roll or climb indicator: a short vertical block sliding either
    /// side of its dial's centre.
    /// </summary>
    /// <param name="offset">How far from centre, between -1 and 1.</param>
    /// <param name="origin">The dial's left-hand end, in screen coordinates.</param>
    private void DisplayIndicator(float offset, Vector2 origin)
    {
        float x = origin.X + (DialBarWidth / 2) + (offset * IndicatorTravel);

        for (int i = 0; i < IndicatorWidth; i++)
        {
            Draw.Graphics.DrawLine(new(x + i, origin.Y), new(x + i, origin.Y + DialBarHeight - 1), DialTopColor);
        }
    }

    private void DisplayFuel()
    {
        // Fuel is a real quantity in light years rather than a 0-to-1 reading,
        // so the dial takes its fraction of a full tank.
        if (_ship.Fuel > 0)
        {
            DisplayDialBar(_ship.Fuel / _ship.MaxFuel * DialBarWidth, FuelPosition);
        }
    }

    private void DisplayLaserTemp()
    {
        if (_gameState.LaserTemp > GameState.LaserTempMin)
        {
            DisplayDialBar(_gameState.LaserTemp * DialBarWidth, LaserTempPosition);
        }
    }

    private void DisplayMissiles()
    {
        if (_ship.MissileCount == 0)
        {
            return;
        }

        int missileCount = _ship.MissileCount > 4 ? 4 : _ship.MissileCount;
        Vector2 origin = MissilePosition;
        Vector2 location = ScannerRelative(origin with { X = origin.X + ((4 - missileCount) * MissileSpacing) });

        if (_combat.IsMissileArmed)
        {
            Draw.Graphics
                .DrawImage((_combat.MissileTarget == null) ? nameof(ImageType.MissileYellow) : nameof(ImageType.MissileRed), location);
            location.X += MissileSpacing;
            missileCount--;
        }

        for (; missileCount > 0; missileCount--)
        {
            Draw.Graphics.DrawImage(nameof(ImageType.MissileGreen), location);
            location.X += MissileSpacing;
        }
    }

    /// <summary>
    /// Display the current shield strengths.
    /// </summary>
    private void DisplayShields()
    {
        if (_ship.ShieldFront > PlayerShip.ShieldMin)
        {
            DisplayDialBar(_ship.ShieldFront * DialBarWidth, ShieldFrontPosition);
        }

        if (_ship.ShieldRear > PlayerShip.ShieldMin)
        {
            DisplayDialBar(_ship.ShieldRear * DialBarWidth, ShieldRearPosition);
        }
    }

    /// <summary>
    /// Display the speed bar.
    /// </summary>
    private void DisplaySpeed()
    {
        Vector2 origin = ScannerRelative(SpeedPosition);
        float length = _ship.Speed / _ship.MaxSpeed * DialBarWidth;
        FastColor color = (_ship.Speed > (_ship.MaxSpeed * 2 / 3)) ? SpeedWarningColor : DialTopColor;

        for (int i = 0; i < SpeedHeight; i++)
        {
            Draw.Graphics.DrawLine(new(origin.X, origin.Y + i), new(origin.X + length - 1, origin.Y + i), color);
        }
    }

    /// <summary>
    /// Update the compass which tracks the space station / planet.
    /// </summary>
    private void UpdateCompass()
    {
        if (_gameState.InWitchspace)
        {
            return;
        }

        IObject? obj = _universe.IsStationPresent ? _universe.StationOrSun : _universe.Planet;
        if (obj == null)
        {
            return;
        }

        Vector4 dest = VectorMaths.UnitVector(obj.Location);

        if (float.IsNaN(dest.X))
        {
            return;
        }

        Vector2 position = ScannerRelative(new(
            CompassCentre.X - CompassDotRadius + (dest.X * CompassRadius) + 1f,
            CompassCentre.Y - CompassDotRadius + (dest.Y * -CompassRadius) + 1f));

        if (dest.Z < 0)
        {
            Draw.Graphics.DrawImage(nameof(ImageType.CompassRed), position);
        }
        else
        {
            Draw.Graphics.DrawImage(nameof(ImageType.CompassGreen), position);
        }
    }

    /// <summary>
    /// Update the scanner and draw all the lollipops.
    /// </summary>
    private void UpdateScanner()
    {
        (float extentY, float extentX) = ScannerExtent;
        Vector2 centre = ScannerCentre;

        foreach (IObject obj in _universe.GetAllObjects())
        {
            if ((obj.Type <= 0) ||
                obj.Flags.HasFlag(ShipProperties.Dead) ||
                obj.Flags.HasFlag(ShipProperties.Cloaked))
            {
                continue;
            }

            float x = obj.Location.X / 256;
            float y1 = -obj.Location.Z / 1024;
            float y2 = y1 - (obj.Location.Y / 512);

            if ((y2 < -extentY)
                || (y2 > extentY) ||
                (x < -extentX)
                || (x > extentX))
            {
                continue;
            }

            x += centre.X;
            y1 += centre.Y;
            y2 += centre.Y;

            FastColor color = LollipopColor(obj);

            // ship
            Draw.Graphics.DrawRectangleFilled(new(x - 3, y2), 5, 3, color);

            // stick
            Draw.Graphics.DrawRectangleFilled(new(x, y2 < y1 ? y2 : y1), 2, MathF.Abs(y2 - y1), color);
        }
    }

    /// <summary>
    /// The lollipop colour for an object on the scanner.
    /// </summary>
    private FastColor LollipopColor(IObject obj)
        => obj.Flags.HasFlag(ShipProperties.Station)
            ? StationColor
            : obj.Type == ShipType.Missile
                ? MissileColor
                : obj.Flags.HasFlag(ShipProperties.Police)
                    ? PoliceColor
                    : obj.Flags.HasFlag(ShipProperties.Hostile) ? HostileColor : DefaultColor;
}
