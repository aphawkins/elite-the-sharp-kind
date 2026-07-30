// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Numerics;
using EliteSharpLib.Conflict;
using EliteSharpLib.Graphics;
using EliteSharpLib.Ships;
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

    protected abstract Vector2 RollPosition { get; }

    protected abstract Vector2 ClimbPosition { get; }

    protected abstract Vector2 CompassPosition { get; }

    protected abstract float CompassRadius { get; }

    protected abstract Vector2 MissilePosition { get; }

    protected abstract float MissileSpacing { get; }

    protected abstract Vector2 StationIndicatorPosition { get; }

    protected abstract Vector2 EcmIndicatorPosition { get; }

    /// <summary>
    /// Gets the dial's top rule colour, which the roll, climb and speed
    /// indicators also use.
    /// </summary>
    protected abstract uint DialTopColor { get; }

    protected abstract uint DialBodyColor { get; }

    protected abstract uint DialBottomColor { get; }

    protected abstract uint SpeedWarningColor { get; }

    protected abstract uint StationColor { get; }

    protected abstract uint MissileColor { get; }

    protected abstract uint PoliceColor { get; }

    protected abstract uint HostileColor { get; }

    protected abstract uint DefaultColor { get; }

    internal void DrawScanner()
        => Draw.Graphics.DrawImage(nameof(ImageType.Scanner), new(Draw.Layout.ScannerLeft, Draw.Layout.ScannerTop));

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
            Draw.Graphics.DrawImage(nameof(ImageType.BigS), ScannerRelative(StationIndicatorPosition));
        }

        if (_ship.EcmActive != 0)
        {
            Draw.Graphics.DrawImage(nameof(ImageType.BigE), ScannerRelative(EcmIndicatorPosition));
        }
    }

    /// <summary>
    /// Turns a scanner-relative position into a screen one.
    /// </summary>
    protected Vector2 ScannerRelative(Vector2 position)
        => new(Draw.Layout.ScannerLeft + position.X, Draw.Layout.ScannerTop + position.Y);

    private void DisplayAltitude()
    {
        if (_ship.Altitude > 3)
        {
            DisplayDialBar(_ship.Altitude / 4, AltitudePosition);
        }
    }

    private void DisplayCabinTemp()
    {
        if (_ship.CabinTemperature > 3)
        {
            DisplayDialBar(_ship.CabinTemperature / 4, CabinTempPosition);
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

        Draw.Graphics.DrawLine(new(x, y), new(x + len, y), DialTopColor);
        int i = 1;
        Draw.Graphics.DrawLine(new(x, y + i), new(x + len, y + i), DialTopColor);

        for (i = 2; i < last; i++)
        {
            Draw.Graphics.DrawLine(new(x, y + i), new(x + len, y + i), DialBodyColor);
        }

        Draw.Graphics.DrawLine(new(x, y + i), new(x + len, y + i), DialBottomColor);
    }

    /// <summary>
    /// Display the energy banks.
    /// </summary>
    private void DisplayEnergy()
    {
        float e1 = _ship.Energy > 64 ? 64 : _ship.Energy;
        float e2 = _ship.Energy > 128 ? 64 : _ship.Energy - 64;
        float e3 = _ship.Energy > 192 ? 64 : _ship.Energy - 128;
        float e4 = _ship.Energy - 192;
        Vector2 bank = EnergyPosition;

        if (e4 > 0)
        {
            DisplayDialBar(e4, bank);
        }

        if (e3 > 0)
        {
            DisplayDialBar(e3, bank with { Y = bank.Y + EnergyBankSpacing });
        }

        if (e2 > 0)
        {
            DisplayDialBar(e2, bank with { Y = bank.Y + (2 * EnergyBankSpacing) });
        }

        if (e1 > 0)
        {
            DisplayDialBar(e1, bank with { Y = bank.Y + (3 * EnergyBankSpacing) });
        }
    }

    private void DisplayFlightClimb()
    {
        Vector2 origin = ScannerRelative(ClimbPosition);
        float position = origin.X + (_ship.Climb * 28 / _ship.MaxClimb) + 32;

        for (int i = 0; i < 4; i++)
        {
            Draw.Graphics.DrawLine(new(position + i, origin.Y), new(position + i, origin.Y + 7), DialTopColor);
        }
    }

    private void DisplayFlightRoll()
    {
        Vector2 origin = ScannerRelative(RollPosition);
        float position = origin.X - (_ship.Roll * 28 / _ship.MaxRoll) + 32;

        for (int i = 0; i < 4; i++)
        {
            Draw.Graphics.DrawLine(new(position + i, origin.Y), new(position + i, origin.Y + 7), DialTopColor);
        }
    }

    private void DisplayFuel()
    {
        if (_ship.Fuel > 0)
        {
            DisplayDialBar(_ship.Fuel * 64 / _ship.MaxFuel, FuelPosition);
        }
    }

    private void DisplayLaserTemp()
    {
        if (_gameState.LaserTemp > 0)
        {
            DisplayDialBar(_gameState.LaserTemp / 4, LaserTempPosition);
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
        if (_ship.ShieldFront > 3)
        {
            DisplayDialBar(_ship.ShieldFront / 4, ShieldFrontPosition);
        }

        if (_ship.ShieldRear > 3)
        {
            DisplayDialBar(_ship.ShieldRear / 4, ShieldRearPosition);
        }
    }

    /// <summary>
    /// Display the speed bar.
    /// </summary>
    private void DisplaySpeed()
    {
        Vector2 origin = ScannerRelative(SpeedPosition);
        float length = (_ship.Speed * 64 / _ship.MaxSpeed) - 1;
        uint color = (_ship.Speed > (_ship.MaxSpeed * 2 / 3)) ? SpeedWarningColor : DialTopColor;

        for (int i = 0; i < 6; i++)
        {
            Draw.Graphics.DrawLine(new(origin.X, origin.Y + i), new(origin.X + length, origin.Y + i), color);
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

        Vector2 origin = CompassPosition;
        Vector2 position = ScannerRelative(new(
            origin.X + (dest.X * CompassRadius),
            origin.Y + (dest.Y * -CompassRadius)));

        if (dest.Z < 0)
        {
            Draw.Graphics.DrawImage(nameof(ImageType.DotRed), position);
        }
        else
        {
            Draw.Graphics.DrawImage(nameof(ImageType.GreenDot), position);
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

            uint color = LollipopColor(obj);

            // ship
            Draw.Graphics.DrawRectangleFilled(new(x - 3, y2), 5, 3, color);

            // stick
            Draw.Graphics.DrawRectangleFilled(new(x, y2 < y1 ? y2 : y1), 2, MathF.Abs(y2 - y1), color);
        }
    }

    /// <summary>
    /// The lollipop colour for an object on the scanner.
    /// </summary>
    private uint LollipopColor(IObject obj)
        => obj.Flags.HasFlag(ShipProperties.Station)
            ? StationColor
            : obj.Type == ShipType.Missile
                ? MissileColor
                : obj.Flags.HasFlag(ShipProperties.Police)
                    ? PoliceColor
                    : obj.Flags.HasFlag(ShipProperties.Hostile) ? HostileColor : DefaultColor;
}
