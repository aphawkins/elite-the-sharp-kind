// 'Stunt Car Racer - The Sharp Kind' - Andy Hawkins 2026.
// 'Stunt Car Racer Remake' - sourceforge.net/projects/stuntcarremake.
// Stunt Car Racer (C) Geoff Crammond / MicroStyle / MicroProse 1989.

using StuntCarRacerLib.Rendering;
using StuntCarRacerLib.Tracks;
using Useful.Graphics;
using Xunit;

namespace StuntCarRacerLib.Tests.Rendering;

public class RoadTexturesTests
{
    [Fact]
    public void StripsHaveLineColourAtEdgesAndRoadColourBetween()
    {
        uint yellow = ScrPalette.Colour(Track.ScrBaseColour + 3);
        uint red = ScrPalette.Colour(Track.ScrBaseColour + 10);
        uint darker = ScrPalette.Colour(Track.ScrBaseColour + 1);
        uint lighter = ScrPalette.Colour(Track.ScrBaseColour + 2);

        AssertStrip(RoadTextures.Textures[RoadTextures.YellowDark], yellow, darker);
        AssertStrip(RoadTextures.Textures[RoadTextures.YellowLight], yellow, lighter);
        AssertStrip(RoadTextures.Textures[RoadTextures.RedDark], red, darker);
        AssertStrip(RoadTextures.Textures[RoadTextures.RedLight], red, lighter);

        uint black = ScrPalette.Colour(Track.ScrBaseColour);
        uint white = ScrPalette.Colour(Track.ScrBaseColour + 15);
        AssertStrip(RoadTextures.Textures[RoadTextures.Black], black, black);
        AssertStrip(RoadTextures.Textures[RoadTextures.White], white, white);

        static void AssertStrip(FastBitmap strip, uint lineColour, uint roadColour)
        {
            Assert.Equal(lineColour, strip.GetPixel(0, 0));
            Assert.Equal(roadColour, strip.GetPixel(strip.Width / 2, 0));
            Assert.Equal(lineColour, strip.GetPixel(strip.Width - 1, 0));
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
