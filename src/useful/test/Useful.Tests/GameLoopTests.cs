// 'Useful Libraries' - Andy Hawkins 2025.

using Useful.Timing;
using Xunit;

namespace Useful.Tests;

public class GameLoopTests
{
    [Fact]
    public void UpdateRunsAtTheFixedRate()
    {
        // Arrange
        FakeTime time = new();
        int updates = 0;

        // 100 updates per second, one update per 10 fake ticks
        GameLoop loop = new(100, () => updates++, null, () => updates < 10, 0, time, d => time.Wait(d));

        // Act
        loop.Run();

        // Assert
        Assert.Equal(10, updates);
        Assert.Equal(100, time.Timestamp);
    }

    [Fact]
    public void RenderIsCappedIndependentlyOfUpdates()
    {
        // Arrange
        FakeTime time = new();
        int updates = 0;
        int renders = 0;

        // updates at 100Hz, renders capped at 50Hz: two updates per render
        GameLoop loop = new(100, () => updates++, () => renders++, () => updates < 10, 50, time, d => time.Wait(d));

        // Act
        loop.Run();

        // Assert
        Assert.Equal(10, updates);
        Assert.Equal(5, renders);
    }

    [Fact]
    public void UnlimitedRenderDoesNotWaitForUpdates()
    {
        // Arrange
        FakeTime time = new();
        int updates = 0;
        int renders = 0;

        GameLoop loop = new(100, () => updates++, () => renders++, () => renders < 100, 0, time, d => time.Wait(d));

        // Act
        loop.Run();

        // Assert: time never advances (no waiting), so no update falls due
        Assert.Equal(100, renders);
        Assert.Equal(0, updates);
        Assert.Equal(0, time.Timestamp);
    }

    [Fact]
    public void StallDropsBacklogInsteadOfBurstingUpdates()
    {
        // Arrange
        FakeTime time = new();
        List<long> updateTimes = [];

        // the third wait stalls for a whole fake second
        int waits = 0;
        void Wait(TimeSpan duration)
        {
            waits++;
            time.Wait(duration);
            if (waits == 3)
            {
                time.Timestamp += 1000;
            }
        }

        GameLoop loop = new(100, () => updateTimes.Add(time.Timestamp), null, () => updateTimes.Count < 20, 0, time, Wait);

        // Act
        loop.Run();

        // Assert: the burst after the stall is capped at five updates
        int maxBurst = updateTimes.GroupBy(t => t).Max(g => g.Count());
        Assert.Equal(5, maxBurst);
    }

    [Fact]
    public void ConstructWithNullUpdateThrows()
        => Assert.Throws<ArgumentNullException>(() => new GameLoop(50, null!, () => true));

    [Fact]
    public void ConstructWithNullRenderThrows()
        => Assert.Throws<ArgumentNullException>(() => new GameLoop(50, () => { }, null!, () => true, 50));

    [Fact]
    public void ConstructWithZeroUpdateRateThrows()
        => Assert.Throws<ArgumentOutOfRangeException>(() => new GameLoop(0, () => { }, () => true));

    [Fact]
    public void ConstructWithNegativeFrameCapThrows()
        => Assert.Throws<ArgumentOutOfRangeException>(() => new GameLoop(50, () => { }, () => { }, () => true, -1));

    // A fake clock with 1000 timestamp ticks per second; waiting advances it.
    private sealed class FakeTime : TimeProvider
    {
        public long Timestamp { get; set; }

        public override long TimestampFrequency => 1000;

        public override long GetTimestamp() => Timestamp;

        public void Wait(in TimeSpan duration) => Timestamp += (long)(duration.TotalSeconds * TimestampFrequency);
    }
}
