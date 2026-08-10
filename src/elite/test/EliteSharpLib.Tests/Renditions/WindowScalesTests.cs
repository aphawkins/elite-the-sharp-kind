// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharp.Abstractions.Renditions;
using EliteSharp.Renditions.EightBit;
using EliteSharp.Renditions.SixteenBit;

namespace EliteSharpLib.Tests.Renditions;

// What a hand-edited windowScale becomes: the config file cannot judge one,
// because which scales exist is the chosen rendition's answer.
public class WindowScalesTests
{
    [Fact]
    public void AnUnchosenScaleTakesTheRenditionsDefault()
    {
        Assert.Equal(4, WindowScales.Resolve(new EightBitRendition(), null));
        Assert.Equal(2, WindowScales.Resolve(new SixteenBitRendition(), null));
    }

    [Theory]
    [InlineData(1, 1)]
    [InlineData(2, 2)]
    [InlineData(4, 4)]
    public void AScaleTheEightBitTierOffersIsKept(int configured, int expected)
        => Assert.Equal(expected, WindowScales.Resolve(new EightBitRendition(), configured));

    // 3 sits between the 2 and 4 this tier offers, so it is pegged rather than
    // thrown away: the commander loses the exact number, not the setting.
    [Theory]
    [InlineData(3, 4)]
    [InlineData(9, 4)]
    [InlineData(0, 1)]
    public void AScaleTheEightBitTierDoesNotOfferIsPeggedToTheNearest(int configured, int expected)
        => Assert.Equal(expected, WindowScales.Resolve(new EightBitRendition(), configured));

    // The 16-bit tier stops at 2, so the same 4 that suits the 8-bit tier
    // comes back as 2 here.
    [Theory]
    [InlineData(1, 1)]
    [InlineData(2, 2)]
    [InlineData(3, 2)]
    [InlineData(4, 2)]
    public void AScaleIsPeggedToWhatTheSixteenBitTierOffers(int configured, int expected)
        => Assert.Equal(expected, WindowScales.Resolve(new SixteenBitRendition(), configured));
}
