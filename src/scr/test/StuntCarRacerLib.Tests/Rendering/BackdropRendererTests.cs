// 'Stunt Car Racer - The Sharp Kind' - Andy Hawkins 2026.
// 'Stunt Car Racer Remake' - sourceforge.net/projects/stuntcarremake.
// Stunt Car Racer (C) Geoff Crammond / MicroStyle / MicroProse 1989.

using System.Numerics;
using StuntCarRacerLib.Rendering;
using StuntCarRacerLib.Tracks;
using Useful.Fakes.Assets;
using Useful.Graphics;
using Xunit;

namespace StuntCarRacerLib.Tests.Rendering;

public class BackdropRendererTests
{
    private const int Width = 640;

    // The app window is 16:10, not the original's 4:3 - the aspect ratio at
    // which the horizon and track projections used to disagree.
    private const int Height = 400;

    // The horizon fill line is the projection of the ground plane (world
    // y = 0) at a horizontal distance of 2 * 0x10000 track units (the
    // original uses points at z = 0x10000 with the viewpoint height halved).
    private const int GroundLineDistance = 2 * 0x00010000;

    // The ground fill must sit exactly where the track projection puts the
    // ground plane, otherwise the track appears to float above the ground
    // when the camera pitches (regression test for the floating track bug).
    [Theory]
    [InlineData(500, 8000)] // shallow pitch down
    [InlineData(2000, 8000)] // steep pitch down
    [InlineData(1000, 20000)] // near-level, high up
    public void GroundLineMatchesTrackProjectionWhenPitchedDown(int cameraHeight, int targetDistance)
    {
        FastBitmap? frame = null;
        using SoftwareGraphics graphics = SoftwareGraphics.Create(Width, Height, b => frame = b, new FakeAssetLocator());
        BackdropRenderer backdrop = new(graphics);
        SceneCamera camera = new();

        // camera above the world centre looking down at a ground point ahead
        const long centre = (long)Track.TrackCubes * Track.CubeSize / 2;
        camera.LookAt(
            centre,
            -((long)cameraHeight << Track.LogPrecision),
            centre,
            centre,
            0,
            centre + ((long)targetDistance << Track.LogPrecision));

        graphics.Clear();
        backdrop.DrawHorizonOnly(camera);
        graphics.ScreenUpdate();
        Assert.NotNull(frame);

        // where the track renderer would project the ground plane at the
        // ground line's distance, directly ahead
        Scene3D scene = new();
        scene.SetView(camera, Width, Height);
        Vector2 projected = scene.ProjectPoint(
            scene.TransformPoint(camera.X, 0, camera.Z + GroundLineDistance));

        float groundLine = FindSkyToGroundBoundary(frame);
        Assert.True(
            Math.Abs(groundLine - projected.Y) <= 3,
            $"Ground line at y={groundLine} but the track projects the ground plane to y={projected.Y}.");
    }

    // Finds the sky/ground fill boundary: the topmost ground pixel in a
    // column whose pixel above is sky (skipping columns covered by scenery).
    private static float FindSkyToGroundBoundary(FastBitmap frame)
    {
        uint sky = ScrPalette.Colour(Track.ScrBaseColour + 7);
        uint ground = ScrPalette.Colour(Track.ScrBaseColour + 13);

        for (int x = 0; x < Width; x++)
        {
            for (int y = 1; y < Height; y++)
            {
                if (frame.GetPixel(x, y) == ground)
                {
                    if (frame.GetPixel(x, y - 1) == sky)
                    {
                        return y;
                    }

                    break; // scenery above the ground here; try the next column
                }
            }
        }

        Assert.Fail("No sky/ground boundary found in the frame.");
        return 0;
    }
}
