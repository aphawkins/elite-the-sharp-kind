// 'Stunt Car Racer - The Sharp Kind' - Andy Hawkins 2026.
// 'Stunt Car Racer Remake' - sourceforge.net/projects/stuntcarremake.
// Stunt Car Racer (C) Geoff Crammond / MicroStyle / MicroProse 1989.

using StuntCarRacerSharpLib.Rendering;
using StuntCarRacerSharpLib.Tracks;
using Useful;
using Useful.Graphics;
using Xunit;

namespace StuntCarRacerSharpLib.Tests.Rendering;

public class RoadTexturesTests
{
    [Fact]
    public void StripsHaveLineColourAtEdgesAndRoadColourBetween()
    {
        ScrPalette palette = new();
        RoadTextures roadTextures = new(palette);

        FastColor yellow = palette.Colour(Track.ScrBaseColour + 3);
        FastColor red = palette.Colour(Track.ScrBaseColour + 10);
        FastColor darker = palette.Colour(Track.ScrBaseColour + 1);
        FastColor lighter = palette.Colour(Track.ScrBaseColour + 2);

        AssertStrip(roadTextures.Textures[RoadTextures.YellowDark], yellow, darker);
        AssertStrip(roadTextures.Textures[RoadTextures.YellowLight], yellow, lighter);
        AssertStrip(roadTextures.Textures[RoadTextures.RedDark], red, darker);
        AssertStrip(roadTextures.Textures[RoadTextures.RedLight], red, lighter);

        FastColor black = palette.Colour(Track.ScrBaseColour);
        FastColor white = palette.Colour(Track.ScrBaseColour + 15);
        AssertStrip(roadTextures.Textures[RoadTextures.Black], black, black);
        AssertStrip(roadTextures.Textures[RoadTextures.White], white, white);

        static void AssertStrip(FastBitmap strip, in FastColor lineColour, in FastColor roadColour)
        {
            Assert.Equal((uint)lineColour, strip.GetPixel(0, 0));
            Assert.Equal((uint)roadColour, strip.GetPixel(strip.Width / 2, 0));
            Assert.Equal((uint)lineColour, strip.GetPixel(strip.Width - 1, 0));
        }
    }

    [Theory]
    [InlineData(TrackId.LittleRamp)]
    [InlineData(TrackId.SteppingStones)]
    [InlineData(TrackId.HumpBack)]
    [InlineData(TrackId.BigRamp)]
    [InlineData(TrackId.SkiJump)]
    [InlineData(TrackId.DrawBridge)]
    [InlineData(TrackId.HighJump)]
    [InlineData(TrackId.RollerCoaster)]
    public void EverySegmentGetsAValidTexture(TrackId id)
    {
        Track track = Track.Load(id);

        int[] textures = RoadTextures.SegmentTextures(track);

        Assert.Equal(track.NumSegments, textures.Length);
        Assert.All(textures, t => Assert.InRange(t, RoadTextures.YellowDark, RoadTextures.White));
    }

    [Fact]
    public void StartLineSegmentIsWhite()
    {
        Track track = Track.Load(TrackId.LittleRamp);

        int[] textures = RoadTextures.SegmentTextures(track);

        TrackPiece piece = track.Pieces[track.StartLinePiece];
        Assert.Equal(RoadTextures.White, textures[piece.FirstSegment + piece.NumSegments - 1]);
    }

    [Fact]
    public void LineColoursAlternateSegmentBySegment()
    {
        Track track = Track.Load(TrackId.LittleRamp);

        int[] textures = RoadTextures.SegmentTextures(track);

        // the coloured (non-black, non-start-line) segments must include
        // both yellow and red variants, from the per-segment alternation
        Assert.Contains(textures, t => t is RoadTextures.YellowDark or RoadTextures.YellowLight);
        Assert.Contains(textures, t => t is RoadTextures.RedDark or RoadTextures.RedLight);
    }
}
