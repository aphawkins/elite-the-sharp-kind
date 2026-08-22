// 'SharpKind Libraries' - Andy Hawkins 2023-2026.

namespace SharpKind.Input;

/// <summary>
/// The axes a game can ask about. Triggers are axes rather than buttons so
/// that an analog pad keeps its travel; a device without them reports 0.
/// </summary>
public enum GamepadAxis
{
    LeftX = 0,
    LeftY = 1,
    RightX = 2,
    RightY = 3,
    LeftTrigger = 4,
    RightTrigger = 5,
}
