// 'Stunt Car Racer - The Sharp Kind' - Andy Hawkins 2026.
// 'Stunt Car Racer Remake' - sourceforge.net/projects/stuntcarremake.
// Stunt Car Racer (C) Geoff Crammond / MicroStyle / MicroProse 1989.

using StuntCarRacerLib.Cars;
using StuntCarRacerLib.Fakes;
using StuntCarRacerLib.Rendering;
using StuntCarRacerLib.Tracks;
using Xunit;

namespace StuntCarRacerLib.Tests.Rendering;

public class TrackRendererTests
{
    [Theory]
    [InlineData(TrackId.LittleRamp)]
    [InlineData(TrackId.SteppingStones)]
    [InlineData(TrackId.HumpBack)]
    [InlineData(TrackId.BigRamp)]
    [InlineData(TrackId.SkiJump)]
    [InlineData(TrackId.DrawBridge)]
    [InlineData(TrackId.HighJump)]
    [InlineData(TrackId.RollerCoaster)]
    public void DrawsPolygonsFromRaceStartOnAllTracks(TrackId id)
    {
        Track track = Track.Load(id);
        CarPhysics car = new(track);
        car.StartRace();

        SceneCamera camera = new();
        camera.FollowCar(car);

        RecordingGraphics graphics = new(640, 400);
        TrackRenderer renderer = new(track, graphics);
        renderer.Draw(camera);

        // Track geometry ahead of the car must produce filled polygons.
        Assert.True(graphics.FilledPolygons.Count > 0);

        // Every drawn polygon has at least 3 points.
        Assert.All(graphics.FilledPolygons, p => Assert.True(p.Points.Length >= 3));
    }

    [Fact]
    public void DrawingIsDeterministic()
    {
        Track track = Track.Load(TrackId.LittleRamp);
        CarPhysics car = new(track);
        car.StartRace();

        SceneCamera camera = new();
        camera.FollowCar(car);

        RecordingGraphics graphics1 = new(640, 400);
        new TrackRenderer(track, graphics1).Draw(camera);

        RecordingGraphics graphics2 = new(640, 400);
        new TrackRenderer(track, graphics2).Draw(camera);

        Assert.Equal(graphics1.FilledPolygons.Count, graphics2.FilledPolygons.Count);
    }

    [Fact]
    public void DrawsWhileDrivingWithoutExceptions()
    {
        Track track = Track.Load(TrackId.LittleRamp);
        CarPhysics car = new(track);
        car.StartRace();

        SceneCamera camera = new();
        RecordingGraphics graphics = new(640, 400);
        TrackRenderer renderer = new(track, graphics);

        for (int frame = 0; frame < 200; frame++)
        {
            car.Update(CarInput.AccelBoost);
            camera.FollowCar(car);
            renderer.Draw(camera);
        }

        Assert.True(graphics.FilledPolygons.Count > 0);
    }
}
