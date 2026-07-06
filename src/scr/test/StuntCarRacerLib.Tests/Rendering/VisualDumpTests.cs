// 'Stunt Car Racer - The Sharp Kind' - Andy Hawkins 2026.
// 'Stunt Car Racer Remake' - sourceforge.net/projects/stuntcarremake.
// Stunt Car Racer (C) Geoff Crammond / MicroStyle / MicroProse 1989.

using StuntCarRacerLib.Cars;
using StuntCarRacerLib.Rendering;
using StuntCarRacerLib.Tracks;
using Useful.Assets;
using Useful.Graphics;
using Xunit;

namespace StuntCarRacerLib.Tests.Rendering;

// Renders frames through the real software rasterizer and saves BMP files
// for visual inspection (found under StuntCarRacerFrames in the temp dir).
public class VisualDumpTests
{
    [Fact]
    public void DumpFrames()
    {
        string outDir = Path.Combine(Path.GetTempPath(), "StuntCarRacerFrames");
        Directory.CreateDirectory(outDir);

        Track track = Track.Load(TrackId.LittleRamp);
        CarPhysics car = new(track);
        car.StartRace();
        OpponentPhysics opponent = new(track, car, new Random(3));
        opponent.StartRace();

        FastBitmap? lastFrame = null;
        using SoftwareGraphics graphics = SoftwareGraphics.Create(640, 400, b => lastFrame = b, AssetLocator.Create());
        TrackRenderer renderer = new(track, graphics);
        BackdropRenderer backdrop = new(graphics);
        OpponentRenderer opponentRenderer = new(opponent);
        SceneCamera camera = new();
        List<WorldPolygon> worldPolygons = [];

        void Step(CarInput input)
        {
            car.Update(input);
            opponent.Update();
        }

        void RenderAndSave(string name, bool followCar = true)
        {
            if (followCar)
            {
                camera.FollowCar(car);
            }

            graphics.Clear();
            backdrop.Draw(camera);
            worldPolygons.Clear();
            opponentRenderer.AppendWorldPolygons(worldPolygons);
            renderer.Draw(camera, worldPolygons, car.CurrentPiece, car.CurrentSegment);
            graphics.ScreenUpdate();
            Assert.NotNull(lastFrame);
            SaveBmp(lastFrame, Path.Combine(outDir, name));
        }

        // frame at race start (car in the air above the start piece)
        RenderAndSave("frame_start.bmp");

        // after dropping onto the road (the opponent should be just ahead)
        for (int i = 0; i < 12; i++)
        {
            Step(CarInput.None);
        }

        RenderAndSave("frame_landed.bmp");

        // a moment later the opponent has pulled ahead into view
        for (int i = 0; i < 10; i++)
        {
            Step(CarInput.None);
        }

        RenderAndSave("frame_opponent.bmp");

        // after accelerating down the straight
        for (int i = 0; i < 60; i++)
        {
            Step(CarInput.AccelBoost);
        }

        RenderAndSave("frame_driving.bmp");

        // approaching / on the ramp
        for (int i = 0; i < 60; i++)
        {
            Step(CarInput.AccelBoost);
        }

        RenderAndSave("frame_ramp.bmp");

        // the driving frame with the in-game dashboard over it: chassis
        // beam with a part-grown damage crack, speed bar and read-outs
        HudRenderer hud = new(graphics, new Random(5));
        camera.FollowCar(car);
        graphics.Clear();
        backdrop.Draw(camera);
        worldPolygons.Clear();
        opponentRenderer.AppendWorldPolygons(worldPolygons);
        renderer.Draw(camera, worldPolygons, car.CurrentPiece, car.CurrentSegment);
        hud.Draw(2, 15, 120, car.PlayerZSpeed, -340);
        graphics.ScreenUpdate();
        Assert.NotNull(lastFrame);
        SaveBmp(lastFrame, Path.Combine(outDir, "frame_hud.bmp"));

        // the track-preview viewpoint: high up, pitched down at the car -
        // the camera angle that exposed the floating-track bug (the ground
        // fill must stay attached to the track when the camera pitches)
        const long centre = (long)Track.TrackCubes * Track.CubeSize / 2;
        camera.LookAt(
            centre + ((car.X - centre) / 2),
            car.Y - ((long)Track.CubeSize / 2),
            centre + ((car.Z - centre) / 2),
            car.X,
            car.Y,
            car.Z);
        RenderAndSave("frame_preview.bmp", followCar: false);
    }

    private static void SaveBmp(FastBitmap bitmap, string path)
    {
        int w = bitmap.Width;
        int h = bitmap.Height;
        int rowSize = ((w * 3) + 3) & ~3;
        int dataSize = rowSize * h;

        using BinaryWriter writer = new(File.Create(path));
        writer.Write((byte)'B');
        writer.Write((byte)'M');
        writer.Write(54 + dataSize);
        writer.Write(0);
        writer.Write(54);
        writer.Write(40);
        writer.Write(w);
        writer.Write(h);
        writer.Write((short)1);
        writer.Write((short)24);
        writer.Write(0);
        writer.Write(dataSize);
        writer.Write(2835);
        writer.Write(2835);
        writer.Write(0);
        writer.Write(0);

        byte[] row = new byte[rowSize];
        for (int y = h - 1; y >= 0; y--)
        {
            for (int x = 0; x < w; x++)
            {
                uint argb = bitmap.GetPixel(x, y);
                row[x * 3] = (byte)(argb & 0xff);
                row[(x * 3) + 1] = (byte)((argb >> 8) & 0xff);
                row[(x * 3) + 2] = (byte)((argb >> 16) & 0xff);
            }

            writer.Write(row);
        }
    }
}
