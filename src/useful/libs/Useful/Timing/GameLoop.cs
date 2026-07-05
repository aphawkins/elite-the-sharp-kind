// 'Useful Libraries' - Andy Hawkins 2025.

namespace Useful.Timing;

/// <summary>
/// Accumulator-based fixed-timestep game loop. <c>update</c> runs at a
/// constant rate regardless of how long rendering takes; rendering runs
/// independently — capped at a maximum frame rate, unlimited, or absent
/// (when the game draws inside its update).
/// </summary>
public sealed class GameLoop
{
    // After a stall (debugger pause, window drag) drop the backlog rather
    // than bursting updates to catch up.
    private const int MaxPendingUpdates = 5;

    private readonly Action _update;
    private readonly Action? _render;
    private readonly Func<bool> _isRunning;
    private readonly TimeProvider _time;
    private readonly Action<TimeSpan> _wait;
    private readonly long _updateInterval;
    private readonly long _renderInterval;

    public GameLoop(double updatesPerSecond, Action update, Func<bool> isRunning)
        : this(updatesPerSecond, update, null, isRunning, 0, TimeProvider.System, Thread.Sleep)
    {
    }

    public GameLoop(double updatesPerSecond, Action update, Action render, Func<bool> isRunning, double maxFramesPerSecond)
        : this(updatesPerSecond, update, render, isRunning, maxFramesPerSecond, TimeProvider.System, Thread.Sleep)
        => Guard.ArgumentNull(render);

    internal GameLoop(
        double updatesPerSecond,
        Action update,
        Action? render,
        Func<bool> isRunning,
        double maxFramesPerSecond,
        TimeProvider time,
        Action<TimeSpan> wait)
    {
        Guard.ArgumentNull(update);
        Guard.ArgumentNull(isRunning);
        Guard.ArgumentNull(time);
        Guard.ArgumentNull(wait);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(updatesPerSecond);
        ArgumentOutOfRangeException.ThrowIfNegative(maxFramesPerSecond);

        _update = update;
        _render = render;
        _isRunning = isRunning;
        _time = time;
        _wait = wait;
        _updateInterval = (long)(time.TimestampFrequency / updatesPerSecond);
        _renderInterval = maxFramesPerSecond > 0 ? (long)(time.TimestampFrequency / maxFramesPerSecond) : 0;
    }

    public void Run()
    {
        long previous = _time.GetTimestamp();
        long accumulator = 0;
        long nextRenderDue = previous;

        while (_isRunning())
        {
            long now = _time.GetTimestamp();
            accumulator += now - previous;
            previous = now;

            if (accumulator > _updateInterval * MaxPendingUpdates)
            {
                accumulator = _updateInterval * MaxPendingUpdates;
            }

            while (accumulator >= _updateInterval)
            {
                _update();
                accumulator -= _updateInterval;

                if (!_isRunning())
                {
                    return;
                }
            }

            if (_render is not null && (_renderInterval == 0 || now >= nextRenderDue))
            {
                _render();

                // Schedule from the previous due time to keep the cadence,
                // but never build up a backlog of overdue renders.
                nextRenderDue = Math.Max(nextRenderDue + _renderInterval, _time.GetTimestamp());
            }

            WaitForNextDue(previous + _updateInterval - accumulator, nextRenderDue);
        }
    }

    private void WaitForNextDue(long updateDue, long renderDue)
    {
        if (_render is not null && _renderInterval == 0)
        {
            // Unlimited rendering: never wait.
            return;
        }

        long due = _render is not null ? Math.Min(updateDue, renderDue) : updateDue;
        long remaining = due - _time.GetTimestamp();
        if (remaining > 0)
        {
            _wait(TimeSpan.FromSeconds((double)remaining / _time.TimestampFrequency));
        }
    }
}
