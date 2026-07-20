// 'Stunt Car Racer - The Sharp Kind' - Andy Hawkins 2026.
// 'Stunt Car Racer Remake' - sourceforge.net/projects/stuntcarremake.
// Stunt Car Racer (C) Geoff Crammond / MicroStyle / MicroProse 1989.

using StuntCarRacerLib.Rendering;
using Xunit;

namespace StuntCarRacerLib.Tests.Rendering;

// Regression lock for the palette.json conversion: same 42 entries as the
// original hardcoded array, addressed positionally.
public class ScrPaletteTests
{
    [Theory]
    [InlineData(0, 0xff000000)]
    [InlineData(9, 0xff000000)]
    [InlineData(11, 0xff880022)]
    [InlineData(17, 0xff333333)]
    [InlineData(19, 0xff220088)]
    [InlineData(25, 0xff333333)]
    [InlineData(26, 0xff000000)]
    [InlineData(29, 0xffffff00)]
    [InlineData(41, 0xffffffff)]
    public void ColourReturnsTheOriginalArgbValue(int index, uint expected)
        => Assert.Equal(expected, (uint)new ScrPalette().Colour(index));
}
