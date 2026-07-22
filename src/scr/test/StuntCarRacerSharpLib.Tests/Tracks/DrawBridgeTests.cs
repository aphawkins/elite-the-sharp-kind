// 'Stunt Car Racer - The Sharp Kind' - Andy Hawkins 2026.
// 'Stunt Car Racer Remake' - sourceforge.net/projects/stuntcarremake.
// Stunt Car Racer (C) Geoff Crammond / MicroStyle / MicroProse 1989.

using StuntCarRacerSharpLib.Cars;
using StuntCarRacerSharpLib.Tracks;
using Useful;
using Xunit;

namespace StuntCarRacerSharpLib.Tests.Tracks;

public class DrawBridgeTests
{
    [Fact]
    public void BridgeMovesWhileCarsAreAway()
    {
        Track track = Track.Load(TrackId.DrawBridge);
        CarPhysics player = new(track);
        OpponentPhysics opponent = new(track, player, new RandomSource(new Random(1)));
        DrawBridge bridge = new(track);
        bridge.Reset(opponent);

        int before = track.Pieces[51].Coords[8].Y; // a bridge top coordinate

        // both cars far from the bridge - the bridge should animate
        HashSet<int> heights = [];
        for (int frame = 0; frame < 40; frame++)
        {
            bridge.Move(0, 0, opponent);
            heights.Add(track.Pieces[51].Coords[8].Y);
        }

        Assert.True(heights.Count > 5);
        _ = before;
    }

    [Fact]
    public void BridgeFreezesWhilePlayerIsOnIt()
    {
        Track track = Track.Load(TrackId.DrawBridge);
        CarPhysics player = new(track);
        OpponentPhysics opponent = new(track, player, new RandomSource(new Random(1)));
        DrawBridge bridge = new(track);
        bridge.Reset(opponent);

        // player on the bridge - the coordinates must not change
        int before = track.Pieces[51].Coords[8].Y;
        for (int frame = 0; frame < 40; frame++)
        {
            bridge.Move(52, 0, opponent);
        }

        Assert.Equal(before, track.Pieces[51].Coords[8].Y);
    }

    [Fact]
    public void BridgeDoesNothingOnOtherTracks()
    {
        Track track = Track.Load(TrackId.LittleRamp);
        CarPhysics player = new(track);
        OpponentPhysics opponent = new(track, player, new RandomSource(new Random(1)));
        DrawBridge bridge = new(track);
        bridge.Reset(opponent);

        int before = track.Pieces[20].Coords[8].Y;
        for (int frame = 0; frame < 40; frame++)
        {
            bridge.Move(0, 0, opponent);
        }

        Assert.Equal(before, track.Pieces[20].Coords[8].Y);
    }

    [Fact]
    public void BridgeHeightFollowsTriangleWave()
    {
        Track track = Track.Load(TrackId.DrawBridge);
        CarPhysics player = new(track);
        OpponentPhysics opponent = new(track, player, new RandomSource(new Random(1)));
        DrawBridge bridge = new(track);
        bridge.Reset(opponent);

        // sample one full 32-frame cycle; the coordinate should rise and fall
        List<int> heights = [];
        for (int frame = 0; frame < 32; frame++)
        {
            bridge.Move(0, 0, opponent);
            heights.Add(track.Pieces[51].Coords[8].Y);
        }

        Assert.True(heights.Max() > heights.Min());

        // the same height repeats one cycle later
        bridge.Move(0, 0, opponent);
        int nextCycle = track.Pieces[51].Coords[8].Y;
        Assert.Equal(heights[0], nextCycle);
    }
}
