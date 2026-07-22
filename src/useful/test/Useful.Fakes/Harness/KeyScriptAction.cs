// 'Useful Libraries' - Andy Hawkins 2025.

namespace Useful.Fakes.Harness;

// Whether a scripted key event presses and releases within its one tick, or
// starts/ends a hold spanning several ticks.
public enum KeyScriptAction
{
    Tap = 0,
    Hold = 1,
    Release = 2,
}
