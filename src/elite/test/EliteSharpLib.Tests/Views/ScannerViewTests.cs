// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Abstractions.Views;
using EliteSharp.Renditions.SixteenBit;
using EliteSharpLib.Fakes;
using SharpKind.Graphics.Fakes;

namespace EliteSharpLib.Tests.Views;

// The lollipops: a blip with a stick joining it to the scanner plane. The
// drawing floors a rectangle's position and its height separately, so a
// stick sized by its own fractional length can come up a row short of the
// blip - which reads as a lollipop broken off its stick.
public sealed class ScannerViewTests
{
    [Theory]

    // Stick above the blip, with fractional parts that sum past a whole
    // pixel: floor(stick) + floor(length) lands one row short of floor(blip).
    [InlineData(-10.6f, -3.9f)]
    [InlineData(-20.7f, -0.8f)]

    // Stick below the blip, the mirror case.
    [InlineData(9.9f, 2.6f)]
    public void StickReachesTheBlip(float stickY, float blipY)
    {
        // Arrange
        RecordingGraphics graphics = new(512, 512);
        FakeEliteDraw surface = new() { Graphics = graphics };
        ScannerView16Bit view = new(surface, surface.Ships);

        // Act
        view.Draw(Scanner(new(0, stickY, blipY, ShipClass.Default)));

        // Assert: the last two rectangles are the blip then its stick, and
        // the rows they cover must touch.
        (int blipTop, int blipBottom) = Rows(graphics.FilledRectangles[^2].Position.Y, graphics.FilledRectangles[^2].Height);
        (int stickTop, int stickBottom) = Rows(graphics.FilledRectangles[^1].Position.Y, graphics.FilledRectangles[^1].Height);

        Assert.True(
            stickTop <= blipBottom + 1 && stickBottom >= blipTop - 1,
            $"Stick rows {stickTop}-{stickBottom} do not meet blip rows {blipTop}-{blipBottom}.");
    }

    // The same flooring the software rasteriser applies, so these are the
    // rows that actually get painted.
    private static (int Top, int Bottom) Rows(float y, float height)
    {
        int top = (int)MathF.Floor(y);
        return (top, top + (int)MathF.Floor(height) - 1);
    }

    private static ScannerModel Scanner(in ScannerBlip blip) => new(
        IsDocked: false,
        ShieldFront: 0,
        ShieldRear: 0,
        Fuel: 0,
        CabinTemperature: 0,
        LaserTemperature: 0,
        Altitude: 0,
        EnergyBanks: [],
        Speed: 0,
        IsSpeedWarning: false,
        Roll: 0,
        Pitch: 0,
        Missiles: [],
        IsStationPresent: false,
        IsEcmActive: false,
        Compass: null,
        Blips: [blip]);
}
