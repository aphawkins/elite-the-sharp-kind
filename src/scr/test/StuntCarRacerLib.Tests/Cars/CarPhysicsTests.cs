// 'Stunt Car Racer - The Sharp Kind' - Andy Hawkins 2026.
// 'Stunt Car Racer Remake' - sourceforge.net/projects/stuntcarremake.
// Stunt Car Racer (C) Geoff Crammond / MicroStyle / MicroProse 1989.

using StuntCarRacerLib.Cars;
using StuntCarRacerLib.Tracks;
using Xunit;

namespace StuntCarRacerLib.Tests.Cars;

public class CarPhysicsTests
{
    [Fact]
    public void StartRacePositionsCarAboveStartPiece()
    {
        Track track = Track.Load(TrackId.LittleRamp);
        CarPhysics car = new(track);

        car.StartRace();

        // Little Ramp start piece is 15, a straight the car can be put on.
        Assert.Equal(15, car.CurrentPiece);
        Assert.False(car.DropStartDone);
        Assert.False(car.TouchingRoad);
        Assert.Equal(0, car.DisplaySpeed);
        Assert.Equal(0, car.LapNumber);
    }

    [Fact]
    public void CarDropsOntoRoadAtRaceStart()
    {
        Track track = Track.Load(TrackId.LittleRamp);
        CarPhysics car = new(track);
        car.StartRace();

        // With no input the car should fall under gravity and land on the road.
        bool landed = false;
        for (int frame = 0; frame < 100 && !landed; frame++)
        {
            car.Update(CarInput.None);
            landed = car.TouchingRoad;
        }

        Assert.True(landed);
        Assert.True(car.DropStartDone);
    }

    [Fact]
    public void AcceleratingIncreasesDisplaySpeed()
    {
        Track track = Track.Load(TrackId.LittleRamp);
        CarPhysics car = new(track);
        car.StartRace();

        // Land first, then accelerate.
        for (int frame = 0; frame < 100; frame++)
        {
            car.Update(CarInput.None);
        }

        for (int frame = 0; frame < 100; frame++)
        {
            car.Update(CarInput.AccelBoost);
        }

        Assert.True(car.TouchingRoad);
        Assert.True(car.DisplaySpeed > 0);
    }

    [Fact]
    public void CarDrivesAlongTrackThroughPieces()
    {
        Track track = Track.Load(TrackId.LittleRamp);
        CarPhysics car = new(track);
        car.StartRace();

        // Drive forward; the car should progress through several track pieces
        // and be on the road most of the time (it launches off the ramp and
        // is airborne for some frames, which is correct behaviour).
        HashSet<int> piecesVisited = [];
        int framesTouching = 0;
        for (int frame = 0; frame < 300; frame++)
        {
            car.Update(CarInput.AccelBoost);
            piecesVisited.Add(car.CurrentPiece);
            if (car.TouchingRoad)
            {
                framesTouching++;
            }
        }

        Assert.True(piecesVisited.Count >= 5);
        Assert.True(framesTouching > 150);
    }

    [Fact]
    public void BoostConsumesReserve()
    {
        Track track = Track.Load(TrackId.LittleRamp);
        CarPhysics car = new(track);
        car.StartRace();
        car.BoostReserve = track.StandardBoost;

        for (int frame = 0; frame < 100; frame++)
        {
            car.Update(CarInput.None);
        }

        for (int frame = 0; frame < 200; frame++)
        {
            car.Update(CarInput.AccelBoost);
        }

        Assert.True(car.BoostReserve < track.StandardBoost);
    }

    [Fact]
    public void SteeringChangesHeading()
    {
        Track track = Track.Load(TrackId.LittleRamp);
        CarPhysics car = new(track);
        car.StartRace();

        for (int frame = 0; frame < 100; frame++)
        {
            car.Update(CarInput.None);
        }

        int headingBefore = car.YAngle;

        // Get moving, then steer left.
        for (int frame = 0; frame < 30; frame++)
        {
            car.Update(CarInput.AccelBoost);
        }

        for (int frame = 0; frame < 30; frame++)
        {
            car.Update(CarInput.AccelBoost | CarInput.Left);
        }

        Assert.NotEqual(headingBefore, car.YAngle);
    }

    [Fact]
    public void PhysicsIsDeterministic()
    {
        Track track = Track.Load(TrackId.LittleRamp);
        CarPhysics car1 = new(track);
        CarPhysics car2 = new(track);
        car1.StartRace();
        car2.StartRace();

        for (int frame = 0; frame < 200; frame++)
        {
            CarInput input = frame < 100 ? CarInput.None : CarInput.AccelBoost;
            car1.Update(input);
            car2.Update(input);
        }

        Assert.Equal(car1.X, car2.X);
        Assert.Equal(car1.Y, car2.Y);
        Assert.Equal(car1.Z, car2.Z);
        Assert.Equal(car1.YAngle, car2.YAngle);
        Assert.Equal(car1.DisplaySpeed, car2.DisplaySpeed);
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
    public void CarCanDriveOnAllTracks(TrackId id)
    {
        Track track = Track.Load(id);
        CarPhysics car = new(track);
        car.StartRace();

        // Drop, then drive for a while - must not throw and must land.
        for (int frame = 0; frame < 100; frame++)
        {
            car.Update(CarInput.None);
        }

        for (int frame = 0; frame < 100; frame++)
        {
            car.Update(CarInput.AccelBoost);
        }

        Assert.True(car.DropStartDone);
    }
}
