// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using EliteSharpLib.Conflict;

namespace EliteSharpLib.Tests;

/// <summary>
/// Which ships think on which count.
/// </summary>
/// <remarks>
/// The golden traces run at the game's own rate, where the count moves once
/// per update and a ship could never think twice on the same one - so they
/// hold whether or not the repeat is refused, and prove only that the phase
/// spread is untouched. The refusal is what stops a faster update rate
/// buying free reactions, and it is tested here.
/// </remarks>
public class TacticsScheduleTests
{
    [Fact]
    public void OneSlotInEightThinksOnAnyGivenCount()
    {
        TacticsSchedule schedule = new();

        int thinking = 0;
        for (int slot = 0; slot < 16; slot++)
        {
            if (schedule.ShouldThink(slot, 100))
            {
                thinking++;
            }
        }

        Assert.Equal(2, thinking);
    }

    [Fact]
    public void ASlotThinksOnceOnACountHoweverOftenItIsAsked()
    {
        // The whole point. At 60Hz the phase test below stays true for four
        // or five updates in a row, because the count only moves 13.5 times
        // a second.
        TacticsSchedule schedule = new();
        int slot = SlotsThinkingOn(new TacticsSchedule(), 100)[0];

        int thoughts = 0;
        for (int update = 0; update < 5; update++)
        {
            if (schedule.ShouldThink(slot, 100))
            {
                thoughts++;
            }
        }

        Assert.Equal(1, thoughts);
    }

    [Fact]
    public void TheNextCountLetsItThinkAgain()
    {
        TacticsSchedule schedule = new();
        int slot = SlotsThinkingOn(new TacticsSchedule(), 100)[0];

        Assert.True(schedule.ShouldThink(slot, 100));
        Assert.False(schedule.ShouldThink(slot, 100));

        // Its next turn is eight counts on, and the counter runs downwards.
        Assert.True(schedule.ShouldThink(slot, 92));
    }

    [Fact]
    public void OverAWholeTurnOfTheCounterEverySlotThinksThirtyTwoTimes()
    {
        // 256 counts, one slot in eight per count: every slot gets 32 turns,
        // and the memory never holds one back when the value comes round.
        TacticsSchedule schedule = new();

        Dictionary<int, int> thoughts = [];
        for (int count = 255; count >= 0; count--)
        {
            foreach (int slot in SlotsThinkingOn(schedule, count))
            {
                thoughts[slot] = thoughts.GetValueOrDefault(slot) + 1;
            }
        }

        Assert.Equal(16, thoughts.Count);
        Assert.All(thoughts.Values, count => Assert.Equal(32, count));
    }

    [Fact]
    public void ResettingForgetsEverySlot()
    {
        TacticsSchedule schedule = new();
        int slot = SlotsThinkingOn(new TacticsSchedule(), 100)[0];

        Assert.True(schedule.ShouldThink(slot, 100));
        schedule.Reset();

        Assert.True(schedule.ShouldThink(slot, 100));
    }

    private static List<int> SlotsThinkingOn(TacticsSchedule schedule, int count)
    {
        List<int> slots = [];
        for (int slot = 0; slot < 16; slot++)
        {
            if (schedule.ShouldThink(slot, count))
            {
                slots.Add(slot);
            }
        }

        return slots;
    }
}
