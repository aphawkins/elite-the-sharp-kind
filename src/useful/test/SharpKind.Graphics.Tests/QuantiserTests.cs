// 'SharpKind Libraries' - Andy Hawkins 2023-2026.

using SharpKind.Graphics.Rendering;

namespace SharpKind.Graphics.Tests;

public class QuantiserTests
{
    // The 8-bit tier's greys, evenly spaced.
    private static readonly FastColor[] s_greys =
    [
        new(0xFF000000), new(0xFF333333), new(0xFF666666),
        new(0xFF999999), new(0xFFCCCCCC), new(0xFFFFFFFF),
    ];

    [Fact]
    public void AChannelGridTakesTheNearestLevelItsDacDrives()
    {
        ChannelGridQuantiser quantiser = new(channelBits: 4);

        // 0x7A sits between the 0x77 and 0x88 levels, nearer the former.
        Assert.Equal(new FastColor(0xFF777777), quantiser.Quantise(new(0xFF7A7A7A), 0, 0));
        Assert.Equal(17f, quantiser.LevelGap);
    }

    [Fact]
    public void EightBitsAChannelIsEveryLevelThereIs()
    {
        ChannelGridQuantiser quantiser = new(channelBits: 8);

        Assert.Equal(new FastColor(0xFF7A7A7A), quantiser.Quantise(new(0xFF7A7A7A), 0, 0));
    }

    [Fact]
    public void APaletteTakesTheNearestEntryItNames()
    {
        PaletteQuantiser quantiser = new(s_greys);

        Assert.Equal(new FastColor(0xFF666666), quantiser.Quantise(new(0xFF6A6A6A), 0, 0));
    }

    // Evenly spaced 0x33 apart, so that is the gap a dither has to span.
    [Fact]
    public void APalettesLevelGapIsMeasuredFromItsEntries()
        => Assert.Equal(0x33, new PaletteQuantiser(s_greys).LevelGap, 0);

    [Fact]
    public void APaletteNeedsAtLeastOneEntry()
        => Assert.Throws<ArgumentException>(() => new PaletteQuantiser([]));

    // The point of dithering: one colour between two displayable ones resolves
    // to a mix of both rather than banding to whichever is nearer.
    [Fact]
    public void ADitherSpreadsOneColourAcrossTheTwoEitherSideOfIt()
    {
        OrderedDitherQuantiser quantiser = new(new PaletteQuantiser(s_greys));
        HashSet<uint> produced = [];

        for (int y = 0; y < 4; y++)
        {
            for (int x = 0; x < 4; x++)
            {
                produced.Add(quantiser.Quantise(new(0xFF4C4C4C), x, y).Argb);
            }
        }

        Assert.Equal([0xFF333333u, 0xFF666666u], produced.Order().ToHashSet());
    }

    // Halfway between two entries, the 4x4 matrix should land eight pixels on
    // each - a dither that shifted the average would darken or lighten a face.
    [Fact]
    public void ADitherKeepsTheAverageWhereTheColourWas()
    {
        OrderedDitherQuantiser quantiser = new(new PaletteQuantiser(s_greys));
        int lighter = 0;

        for (int y = 0; y < 4; y++)
        {
            for (int x = 0; x < 4; x++)
            {
                if (quantiser.Quantise(new(0xFF4C4C4C), x, y).Argb == 0xFF666666)
                {
                    lighter++;
                }
            }
        }

        Assert.Equal(8, lighter);
    }

    [Fact]
    public void ADitherLeavesAnExactlyDisplayableColourAlone()
    {
        OrderedDitherQuantiser quantiser = new(new PaletteQuantiser(s_greys));

        for (int y = 0; y < 4; y++)
        {
            for (int x = 0; x < 4; x++)
            {
                Assert.Equal(new FastColor(0xFF666666), quantiser.Quantise(new(0xFF666666), x, y));
            }
        }
    }

    // Only a dither needs the pixel; the fill checks this to decide whether it
    // has to ask per pixel or can take one answer for the whole face.
    [Fact]
    public void OnlyADitherDependsOnThePixelPosition()
    {
        Assert.False(new ChannelGridQuantiser(4).IsPositionDependent);
        Assert.False(new PaletteQuantiser(s_greys).IsPositionDependent);
        Assert.True(new OrderedDitherQuantiser(new PaletteQuantiser(s_greys)).IsPositionDependent);
    }
}
