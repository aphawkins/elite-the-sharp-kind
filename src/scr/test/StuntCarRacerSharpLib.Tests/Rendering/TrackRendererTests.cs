// 'Stunt Car Racer - The Sharp Kind' - Andy Hawkins 2026.
// 'Stunt Car Racer Remake' - sourceforge.net/projects/stuntcarremake.
// Stunt Car Racer (C) Geoff Crammond / MicroStyle / MicroProse 1989.

using System.Numerics;
using StuntCarRacerSharpLib.Cars;
using StuntCarRacerSharpLib.Fakes;
using StuntCarRacerSharpLib.Rendering;
using StuntCarRacerSharpLib.Tracks;
using Xunit;

namespace StuntCarRacerSharpLib.Tests.Rendering;

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

        ScrPalette palette = new();
        RecordingGraphics graphics = new(640, 400);
        TrackRenderer renderer = new(track, graphics, palette, new(palette));
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

        ScrPalette palette = new();
        RoadTextures roadTextures = new(palette);
        RecordingGraphics graphics1 = new(640, 400);
        new TrackRenderer(track, graphics1, palette, roadTextures).Draw(camera);

        RecordingGraphics graphics2 = new(640, 400);
        new TrackRenderer(track, graphics2, palette, roadTextures).Draw(camera);

        Assert.Equal(graphics1.FilledPolygons.Count, graphics2.FilledPolygons.Count);
    }

    [Fact]
    public void DrawsWhileDrivingWithoutExceptions()
    {
        Track track = Track.Load(TrackId.LittleRamp);
        CarPhysics car = new(track);
        car.StartRace();

        SceneCamera camera = new();
        ScrPalette palette = new();
        RecordingGraphics graphics = new(640, 400);
        TrackRenderer renderer = new(track, graphics, palette, new(palette));

        for (int frame = 0; frame < 200; frame++)
        {
            car.Update(CarInput.AccelBoost);
            camera.FollowCar(car);
            renderer.Draw(camera);
        }

        Assert.True(graphics.FilledPolygons.Count > 0);
    }

    [Fact]
    public void RoadLinesDrawOnlyWhenPlayerPositionIsGiven()
    {
        Track track = Track.Load(TrackId.LittleRamp);
        CarPhysics car = new(track);
        car.StartRace();

        SceneCamera camera = new();
        camera.FollowCar(car);

        ScrPalette palette = new();
        RoadTextures roadTextures = new(palette);

        // without a player position no road is textured
        RecordingGraphics untextured = new(640, 400);
        new TrackRenderer(track, untextured, palette, roadTextures).Draw(camera);
        Assert.Empty(untextured.TexturedPolygons);

        // with the player's position the nearby road draws textured
        RecordingGraphics textured = new(640, 400);
        new TrackRenderer(track, textured, palette, roadTextures).Draw(camera, null, car.CurrentPiece, car.CurrentSegment);
        Assert.NotEmpty(textured.TexturedPolygons);
        Assert.All(textured.TexturedPolygons, p => Assert.Contains(p.Texture, roadTextures.Textures));
        Assert.All(textured.TexturedPolygons, p => Assert.Equal(p.Points.Length, p.TextureCoords.Length));
    }

    [Fact]
    public void RoadLinesOnlyCoverTheSegmentsAroundThePlayer()
    {
        Track track = Track.Load(TrackId.LittleRamp);
        CarPhysics car = new(track);
        car.StartRace();

        SceneCamera camera = new();
        camera.FollowCar(car);

        ScrPalette palette = new();
        RecordingGraphics graphics = new(640, 400);
        new TrackRenderer(track, graphics, palette, new(palette)).Draw(camera, null, car.CurrentPiece, car.CurrentSegment);

        // 11 segments each side of the player plus the player's own, two
        // road triangles each (clipping can split a triangle once more)
        Assert.InRange(graphics.TexturedPolygons.Count, 1, 23 * 4);
    }

    // DrawPolygonFilled fills polygons as a triangle fan, which only fills
    // the intended shape when the outline is simple and the fan triangles
    // all wind the same way. Twisted quads straddling the near plane used to
    // reach it as self-intersecting outlines, painting large spurious
    // triangles on/beside the track (regression test for that bug).
    [Theory]
    [InlineData(TrackId.LittleRamp)]
    [InlineData(TrackId.SteppingStones)]
    [InlineData(TrackId.HumpBack)]
    [InlineData(TrackId.BigRamp)]
    [InlineData(TrackId.SkiJump)]
    [InlineData(TrackId.DrawBridge)]
    [InlineData(TrackId.HighJump)]
    [InlineData(TrackId.RollerCoaster)]
    public void NeverEmitsPolygonsATriangleFanCannotFill(TrackId id)
    {
        Track track = Track.Load(id);
        CarPhysics car = new(track);
        car.StartRace();

        SceneCamera camera = new();
        ScrPalette palette = new();
        RecordingGraphics graphics = new(640, 400);
        TrackRenderer renderer = new(track, graphics, palette, new(palette));

        for (int frame = 0; frame < 150; frame++)
        {
            car.Update(CarInput.AccelBoost);
            camera.FollowCar(car);
            graphics.FilledPolygons.Clear();
            graphics.TexturedPolygons.Clear();
            renderer.Draw(camera, null, car.CurrentPiece, car.CurrentSegment);

            foreach ((Vector2[] points, _) in graphics.FilledPolygons)
            {
                Assert.False(
                    IsMisfillable(points, out string why),
                    $"Frame {frame}: polygon is {why}: {string.Join(" ", points)}.");
            }

            foreach ((Vector2[] points, _, _) in graphics.TexturedPolygons)
            {
                Assert.False(
                    IsMisfillable(points, out string why),
                    $"Frame {frame}: textured polygon is {why}: {string.Join(" ", points)}.");
            }
        }
    }

    // A fan from point 0 fills exactly the polygon's shape only when the
    // outline is simple and every fan triangle winds the same way.
    private static bool IsMisfillable(in ReadOnlySpan<Vector2> p, out string why)
    {
        int n = p.Length;

        // any two non-adjacent edges crossing = self-intersecting outline
        for (int i = 0; i < n; i++)
        {
            for (int j = i + 2; j < n; j++)
            {
                if (i == 0 && j == n - 1)
                {
                    continue; // adjacent around the loop
                }

                if (SegmentsCross(p[i], p[(i + 1) % n], p[j], p[(j + 1) % n]))
                {
                    why = $"self-intersecting ({n} points, edges {i} and {j})";
                    return true;
                }
            }
        }

        // fan triangles with opposite windings double-paint or escape
        int sign = 0;
        for (int i = 1; i < n - 1; i++)
        {
            float cross = Cross(p[0], p[i], p[i + 1]);
            if (Math.Abs(cross) < 0.5f)
            {
                continue; // degenerate sliver
            }

            int currentSign = Math.Sign(cross);
            if (sign == 0)
            {
                sign = currentSign;
            }
            else if (currentSign != sign)
            {
                why = $"concave for a fan ({n} points, winding flips at {i})";
                return true;
            }
        }

        why = string.Empty;
        return false;
    }

    private static float Cross(Vector2 a, Vector2 b, Vector2 c)
        => ((b.X - a.X) * (c.Y - a.Y)) - ((b.Y - a.Y) * (c.X - a.X));

    private static bool SegmentsCross(Vector2 a, Vector2 b, Vector2 c, Vector2 d)
    {
        float d1 = Cross(c, d, a);
        float d2 = Cross(c, d, b);
        float d3 = Cross(a, b, c);
        float d4 = Cross(a, b, d);

        bool firstSplits = (d1 > 0 && d2 < 0) || (d1 < 0 && d2 > 0);
        bool secondSplits = (d3 > 0 && d4 < 0) || (d3 < 0 && d4 > 0);
        return firstSplits && secondSplits;
    }
}
