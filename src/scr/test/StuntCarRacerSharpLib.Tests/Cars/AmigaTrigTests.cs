// 'Stunt Car Racer - The Sharp Kind' - Andy Hawkins 2026.
// 'Stunt Car Racer Remake' - sourceforge.net/projects/stuntcarremake.
// Stunt Car Racer (C) Geoff Crammond / MicroStyle / MicroProse 1989.

using StuntCarRacerSharpLib.Cars;
using Xunit;

namespace StuntCarRacerSharpLib.Tests.Cars;

public class AmigaTrigTests
{
    [Fact]
    public void SinCosOfZeroAngle()
    {
        Assert.Equal(0, AmigaTrig.Sin(0));
        Assert.Equal(AmigaTrig.Precision, AmigaTrig.Cos(0));
    }

    [Fact]
    public void SinOfNinetyDegreesIsPrecision()
        => Assert.Equal(AmigaTrig.Precision, AmigaTrig.Sin(AmigaTrig.Degrees90));

    [Fact]
    public void TrigCoefficientsAtZeroAnglesAreIdentity()
    {
        TrigCoefficients trig = new();
        trig.CalculateYXZ(0, 0, 0);

        Assert.Equal(AmigaTrig.Precision, trig.XX);
        Assert.Equal(0, trig.XY);
        Assert.Equal(0, trig.XZ);
        Assert.Equal(0, trig.YX);
        Assert.Equal(AmigaTrig.Precision, trig.YY);
        Assert.Equal(0, trig.YZ);
        Assert.Equal(0, trig.ZX);
        Assert.Equal(0, trig.ZY);
        Assert.Equal(AmigaTrig.Precision, trig.ZZ);
    }

    [Fact]
    public void TrigCoefficientsAtNinetyDegreesYRotation()
    {
        TrigCoefficients trig = new();
        trig.CalculateYXZ(0, AmigaTrig.Degrees90, 0);

        // Y rotation by 90 degrees maps x onto -z and z onto x.
        Assert.Equal(0, trig.XX);
        Assert.Equal(-AmigaTrig.Precision, trig.XZ);
        Assert.Equal(AmigaTrig.Precision, trig.ZX);
        Assert.Equal(0, trig.ZZ);
    }
}
