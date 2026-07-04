// 'Stunt Car Racer - The Sharp Kind' - Andy Hawkins 2026.
// 'Stunt Car Racer Remake' - sourceforge.net/projects/stuntcarremake.
// Stunt Car Racer (C) Geoff Crammond / MicroStyle / MicroProse 1989.

using System.Numerics;
using StuntCarRacerLib.Cars;
using StuntCarRacerLib.Rendering;
using StuntCarRacerLib.Tracks;
using Xunit;

namespace StuntCarRacerLib.Tests.Rendering;

public class Scene3DTests
{
    [Fact]
    public void PointAheadProjectsToHorizontalCentre()
    {
        SceneCamera camera = CameraAtOrigin();
        Scene3D scene = new();
        scene.SetView(camera, 640, 400);

        // point straight ahead at camera height
        Coord3D cameraSpace = scene.TransformPoint(0, camera.Y, 1000);

        Assert.Equal(0, cameraSpace.X);
        Assert.Equal(0, cameraSpace.Y);
        Assert.Equal(1000, cameraSpace.Z);

        Vector2 screen = scene.ProjectPoint(cameraSpace);
        Assert.Equal(320, screen.X);
        Assert.Equal(200, screen.Y);
    }

    [Fact]
    public void PointToTheRightProjectsRightOfCentre()
    {
        SceneCamera camera = CameraAtOrigin();
        Scene3D scene = new();
        scene.SetView(camera, 640, 400);

        Coord3D cameraSpace = scene.TransformPoint(500, camera.Y, 1000);
        Vector2 screen = scene.ProjectPoint(cameraSpace);

        Assert.True(screen.X > 320);
    }

    [Fact]
    public void PointAboveProjectsAboveCentre()
    {
        SceneCamera camera = CameraAtOrigin();
        Scene3D scene = new();
        scene.SetView(camera, 640, 400);

        Coord3D cameraSpace = scene.TransformPoint(0, camera.Y + 500, 1000);
        Vector2 screen = scene.ProjectPoint(cameraSpace);

        // screen y grows downwards, so above centre means smaller y
        Assert.True(screen.Y < 200);
    }

    [Fact]
    public void PolygonBehindCameraIsFullyClipped()
    {
        Coord3D[] polygon =
        [
            new(-100, 0, -1000),
            new(100, 0, -1000),
            new(100, 0, -500),
            new(-100, 0, -500),
        ];

        Span<Coord3D> output = stackalloc Coord3D[5];
        int count = Scene3D.ClipPolygonToNearPlane(polygon, output);

        Assert.Equal(0, count);
    }

    [Fact]
    public void PolygonInFrontIsUnchanged()
    {
        Coord3D[] polygon =
        [
            new(-100, 0, 1000),
            new(100, 0, 1000),
            new(100, 0, 500),
            new(-100, 0, 500),
        ];

        Span<Coord3D> output = stackalloc Coord3D[5];
        int count = Scene3D.ClipPolygonToNearPlane(polygon, output);

        Assert.Equal(4, count);
        Assert.Equal(polygon[0], output[0]);
        Assert.Equal(polygon[3], output[3]);
    }

    [Fact]
    public void PolygonCrossingNearPlaneIsClippedToBoundary()
    {
        Coord3D[] polygon =
        [
            new(-100, 0, 1000),
            new(100, 0, 1000),
            new(100, 0, -1000),
            new(-100, 0, -1000),
        ];

        Span<Coord3D> output = stackalloc Coord3D[5];
        int count = Scene3D.ClipPolygonToNearPlane(polygon, output);

        Assert.Equal(4, count);
        for (int i = 0; i < count; i++)
        {
            Assert.True(output[i].Z >= Scene3D.ZClipBoundary);
        }
    }

    private static SceneCamera CameraAtOrigin()
    {
        // A camera following a freshly reset car sits at the origin
        // (raised by the cockpit height) with zero angles.
        Track track = Track.Load(TrackId.LittleRamp);
        CarPhysics car = new(track);
        car.Reset();

        SceneCamera camera = new();
        camera.FollowCar(car);
        return camera;
    }
}
