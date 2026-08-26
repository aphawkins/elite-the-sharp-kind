// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

namespace EliteSharpLib;

/// <summary>
/// Paces Elite's housekeeping - the jobs the original hung off its
/// <c>MCount</c> counter - at a fixed rate in real time, whatever rate the
/// game is being updated at.
/// </summary>
/// <remarks>
/// <para>
/// The counter is a clock, not a tally, and that is the thing to keep hold
/// of. Its jobs are spread across it by residue rather than each carrying a
/// timer: shields regenerate on every eighth count, the altitude and the
/// cabin temperature on the tenth and twentieth of every thirty-two, an
/// encounter is rolled when it reaches zero. They share one phase, and
/// <c>Space.JumpWarp</c> re-phases the lot at once by masking the count down
/// to its low six bits. Give each job a timer of its own and that
/// relationship is gone - the jobs would drift apart, and a jump warp would
/// have nothing to re-phase.
/// </para>
/// <para>
/// So the count survives and this decides how often it moves. At the game's
/// own 13.5Hz one update is exactly one step, which is what it has always
/// been; at any other rate the housekeeping still happens 13.5 times a
/// second, because that is a fact about the game rather than about how often
/// it is drawn.
/// </para>
/// </remarks>
internal sealed class GameClock
{
    /// <summary>
    /// Housekeeping steps a second: Elite The New Kind's tick rate, which is
    /// what every one of these jobs was written against.
    /// </summary>
    internal const float StepsPerSecond = 13.5f;

    private const float SecondsPerStep = 1f / StepsPerSecond;

    private float _unspent;
    private float _elapsed = SecondsPerStep;

    /// <summary>
    /// Gets how much of one of the game's own ticks this update is worth:
    /// exactly one while the game updates at 13.5Hz, and a fraction of one
    /// when it updates faster.
    /// </summary>
    /// <remarks>
    /// Every rate the game was written as "per tick" is multiplied by this,
    /// which is what turns a step size into a speed. It is a division of one
    /// float by the identical float at the game's own rate, so it is exactly
    /// 1 there and the arithmetic is untouched - a rate conversion that
    /// changes nothing until the rate changes.
    /// </remarks>
    internal float Ticks { get; private set; } = 1f;

    /// <summary>
    /// Opens an update worth <paramref name="elapsedSeconds"/> of game time.
    /// </summary>
    internal void BeginUpdate(float elapsedSeconds)
    {
        _elapsed = elapsedSeconds;
        Ticks = elapsedSeconds / SecondsPerStep;
    }

    /// <summary>
    /// Adds this update's elapsed time to the clock and returns how many
    /// housekeeping steps are now due.
    /// </summary>
    /// <remarks>
    /// The remainder is kept rather than dropped, so a rate that does not
    /// divide into 13.5 still averages out instead of losing a step a second.
    /// Feeding it exactly <see cref="SecondsPerStep"/> takes the same float
    /// away as it added, leaving nothing behind to accumulate into drift.
    /// </remarks>
    internal int Advance() => Advance(_elapsed);

    /// <inheritdoc cref="Advance()"/>
    internal int Advance(float elapsedSeconds)
    {
        _unspent += elapsedSeconds;

        int due = 0;
        while (_unspent >= SecondsPerStep)
        {
            _unspent -= SecondsPerStep;
            due++;
        }

        return due;
    }
}
