// 'SharpKind Libraries' - Andy Hawkins 2023-2026.

using SharpKind.Graphics.Rendering;

namespace SharpKind.Graphics.Tests;

public class VertexColoursTests
{
    private static readonly FastColor s_black = new(255, 0, 0, 0);
    private static readonly FastColor s_white = new(255, 255, 255, 255);

    [Fact]
    public void LerpAtTheEndsIsTheEnds()
    {
        Assert.Equal(s_black, VertexColours.Lerp(s_black, s_white, 0f));
        Assert.Equal(s_white, VertexColours.Lerp(s_black, s_white, 1f));
    }

    // Away from zero, so a channel does not round one way here and the other
    // when a rendition quantises it.
    [Fact]
    public void LerpHalfwayRoundsAwayFromZero()
        => Assert.Equal(new FastColor(255, 128, 128, 128), VertexColours.Lerp(s_black, s_white, 0.5f));

    [Fact]
    public void LerpBlendsEachChannelIndependently()
    {
        FastColor blended = VertexColours.Lerp(new(255, 100, 0, 200), new(255, 200, 100, 0), 0.5f);

        Assert.Equal(new FastColor(255, 150, 50, 100), blended);
    }

    // One face's corners carry one face's alpha, so there is nothing to blend
    // and inventing values between two identical ones would be noise.
    [Fact]
    public void LerpTakesAlphaFromTheStart()
        => Assert.Equal(64, VertexColours.Lerp(new(64, 0, 0, 0), new(255, 0, 0, 0), 0.75f).A);

    [Fact]
    public void MeanAveragesEveryCorner()
    {
        FastColor mean = VertexColours.Mean([new(255, 0, 30, 60), new(255, 60, 60, 60), new(255, 30, 0, 0)]);

        Assert.Equal(new FastColor(255, 30, 30, 40), mean);
    }

    [Fact]
    public void MeanOfNoCornersIsDefault() => Assert.Equal(default, VertexColours.Mean([]));

    // A flat colour is quantised once before it is submitted, so a face
    // standing down to one has to arrive the same way.
    [Fact]
    public void FlattenQuantisesWhenTheQuantiserAnswersPerFace()
    {
        FastColor flat = VertexColours.Flatten([s_black, s_white], new ChannelGridQuantiser(1));

        Assert.Equal(new ChannelGridQuantiser(1).Quantise(new(255, 128, 128, 128), 0, 0), flat);
    }

    // A dither can only be asked per pixel, so the fill keeps it and the
    // colour travels unreduced.
    [Fact]
    public void FlattenLeavesADitherToTheFill()
    {
        FastColor flat = VertexColours.Flatten([s_black, s_white], new OrderedDitherQuantiser(new ChannelGridQuantiser(1)));

        Assert.Equal(new FastColor(255, 128, 128, 128), flat);
    }

    [Fact]
    public void FlattenWithNoQuantiserIsJustTheMean()
        => Assert.Equal(new FastColor(255, 128, 128, 128), VertexColours.Flatten([s_black, s_white], quantiser: null));
}
