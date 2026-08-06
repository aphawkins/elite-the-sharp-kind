// 'Useful Libraries' - Andy Hawkins 2023-2026.

namespace Useful.Input;

// Whether a scripted key event presses and releases within its one tick, or
// starts/ends a hold spanning several ticks; SaveFrame carries no key and
// instead asks the host to dump the current framebuffer.
public enum KeyScriptAction
{
    Tap = 0,
    Hold = 1,
    Release = 2,
    SaveFrame = 3,
}
