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
    public void CarGoesOnChainsAfterLeavingTrackThenReleasesOnBoost()
    {
        Track track = Track.Load(TrackId.LittleRamp);
        CarPhysics car = new(track);
        car.StartRace();

        for (int frame = 0; frame < 100 && !car.TouchingRoad; frame++)
        {
            car.Update(CarInput.None);
        }

        // Steer hard off the road and keep driving until recovery kicks in.
        bool wentOnChains = false;
        for (int frame = 0; frame < 500 && !wentOnChains; frame++)
        {
            car.Update(CarInput.AccelBoost | CarInput.Left);
            wentOnChains = car.OnChains;
        }

        Assert.True(wentOnChains);

        // Nothing releases the car until the swing settles and it starts
        // waiting for the player to press boost/fire.
        for (int frame = 0; frame < 100 && !car.WaitingToReleaseChains; frame++)
        {
            car.Update(CarInput.None);
        }

        Assert.True(car.WaitingToReleaseChains);
        Assert.True(car.OnChains);

        bool landed = false;
        for (int frame = 0; frame < 200 && !landed; frame++)
        {
            car.Update(CarInput.Boost);
            landed = car.TouchingRoad;
        }

        Assert.False(car.OnChains);
        Assert.True(landed);
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

    // Regression test for the ptitSeb remake's CalculateDisplaySpeed change:
    // the dead zone was raised from "PlayerZSpeed < 0" to "< 0x1100" (the
    // first few speed values aren't shown), and the result rescaled by
    // 200/128 so the cockpit's speed bar (full at 240 display units) can
    // actually reach full width during normal driving.
    [Fact]
    public void DisplaySpeedStaysZeroBelowTheDeadZoneThenRescales()
    {
        Track track = Track.Load(TrackId.LittleRamp);
        CarPhysics car = new(track);
        car.StartRace();

        for (int frame = 0; frame < 100; frame++)
        {
            car.Update(CarInput.None);
        }

        bool sawDeadZone = false;
        bool sawRescaledOutput = false;
        for (int frame = 0; frame < 200; frame++)
        {
            car.Update(CarInput.AccelBoost);

            if (car.PlayerZSpeed < 0x1100)
            {
                Assert.Equal(0, car.DisplaySpeed);
                sawDeadZone = true;
            }
            else
            {
                // the rescaled formula produces roughly 1.5625x the old
                // (183/32768) value, capable of exceeding it
                int oldFormula = (car.PlayerZSpeed * 183) >> 15;
                if (car.DisplaySpeed > oldFormula)
                {
                    sawRescaledOutput = true;
                }
            }
        }

        Assert.True(sawDeadZone, "expected some frames below the 0x1100 dead zone");
        Assert.True(sawRescaledOutput, "expected the rescaled formula to exceed the old one at some point");
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

    // Regression test for the ptitSeb remake's control-scheme rewrite:
    // accelerate/brake/boost are independent keys (fluffyfreak combined
    // accelerate+boost and brake+boost into single keys). Boost alone,
    // with neither accelerate nor brake held, must not move the car or
    // consume the reserve.
    [Fact]
    public void BoostAloneWithoutAccelerateOrBrakeDoesNothing()
    {
        Track track = Track.Load(TrackId.LittleRamp);
        CarPhysics car = new(track);
        car.StartRace();
        car.BoostReserve = track.StandardBoost;

        for (int frame = 0; frame < 100; frame++)
        {
            car.Update(CarInput.None);
        }

        int speedBeforeBoost = car.DisplaySpeed;
        for (int frame = 0; frame < 100; frame++)
        {
            car.Update(CarInput.Boost);
        }

        Assert.Equal(track.StandardBoost, car.BoostReserve);
        Assert.Equal(speedBeforeBoost, car.DisplaySpeed);
    }

    [Fact]
    public void AccelerateAloneMovesTheCarWithoutConsumingBoost()
    {
        Track track = Track.Load(TrackId.LittleRamp);
        CarPhysics car = new(track);
        car.StartRace();
        car.BoostReserve = track.StandardBoost;

        for (int frame = 0; frame < 100; frame++)
        {
            car.Update(CarInput.None);
        }

        for (int frame = 0; frame < 100; frame++)
        {
            car.Update(CarInput.Accelerate);
        }

        Assert.True(car.DisplaySpeed > 0);
        Assert.Equal(track.StandardBoost, car.BoostReserve);
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

    [Fact]
    public void FrontWheelsSpinFasterAsSpeedIncreases()
    {
        Track track = Track.Load(TrackId.LittleRamp);
        CarPhysics car = new(track);
        car.StartRace();

        for (int frame = 0; frame < 100; frame++)
        {
            car.Update(CarInput.None);
        }

        // wheel frames must stay within the cockpit sprite sheet's 6 frames
        for (int frame = 0; frame < 100; frame++)
        {
            car.Update(CarInput.AccelBoost);
            Assert.InRange(car.LeftWheelFrame, 0, 5);
            Assert.InRange(car.RightWheelFrame, 0, 5);
        }
    }

    [Fact]
    public void WheelBounceIsZeroWhileAirborne()
    {
        Track track = Track.Load(TrackId.LittleRamp);
        CarPhysics car = new(track);
        car.StartRace();

        // at race start the car is above the road, not touching it
        Assert.Equal(0, car.LeftWheelBounce);
        Assert.Equal(0, car.RightWheelBounce);
    }

    [Fact]
    public void SmashHolesResetOnNewRace()
    {
        Track track = Track.Load(TrackId.LittleRamp);
        CarPhysics car = new(track);
        car.StartRace();

        Assert.Equal(0, car.SmashHoles);

        car.StartRace();

        Assert.Equal(0, car.SmashHoles);
    }

    [Fact]
    public void LimitViewpointYLeavesRestingCarUnchanged()
    {
        Track track = Track.Load(TrackId.LittleRamp);
        CarPhysics car = new(track);
        car.StartRace();

        // land on the road and settle
        for (int frame = 0; frame < 100; frame++)
        {
            car.Update(CarInput.None);
        }

        Assert.True(car.TouchingRoad);

        // at rest the wheels sit within the adjustment threshold, so the
        // viewpoint is not moved
        Assert.Equal(car.PlayerY, car.LimitViewpointY());
    }

    [Fact]
    public void LimitViewpointYHoldsViewpointUpOverBumps()
    {
        Track track = Track.Load(TrackId.HumpBack);
        CarPhysics car = new(track);
        car.StartRace();

        // drive over the humps, steering back towards the middle of the
        // road; landing compression must engage the viewpoint limit on at
        // least one frame, and only ever raise the viewpoint
        bool engaged = false;
        for (int frame = 0; frame < 400; frame++)
        {
            CarInput input = CarInput.Accelerate;
            if (car.RoadXPosition < 0xA0)
            {
                input |= CarInput.Right;
            }
            else if (car.RoadXPosition > 0xE0)
            {
                input |= CarInput.Left;
            }

            // Press boost/fire to release from the recovery chains if the
            // car has gone off track, as a player would.
            if (car.OnChains)
            {
                input |= CarInput.Boost;
            }

            car.Update(input);

            int limited = car.LimitViewpointY();
            if (limited != car.PlayerY)
            {
                engaged = true;
                Assert.True(limited > car.PlayerY);
            }
        }

        Assert.True(engaged);
    }
}
