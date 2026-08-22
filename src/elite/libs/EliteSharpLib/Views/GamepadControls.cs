// 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using SharpKind.Input;

namespace EliteSharpLib.Views;

// Elite's flight controls are digital - each held key steps pitch or roll by
// a fixed amount - so an analog stick only has to say which way it is
// pushed. A digital HID stick sits at the ends of the range, so it passes
// the threshold whatever it is set to.
//
// Only flight and fire are mapped. Everything else Elite can do (docking,
// hyperspace, missiles, ECM, the market and charts) has no room on a
// one-stick joystick and stays on the keyboard.
internal static class GamepadControls
{
    private const float Threshold = 0.5f;

    // Negative rolls left, positive right, 0 is centred.
    internal static int Roll(IGamepad gamepad) => Direction(gamepad.Axis(GamepadAxis.LeftX));

    // Negative climbs, positive dives: SDL's Y axis is positive downwards,
    // and pulling the stick back to climb reads as negative, which is the
    // sense Elite's own "up" control has.
    internal static int Pitch(IGamepad gamepad) => Direction(gamepad.Axis(GamepadAxis.LeftY));

    // Buttons 1 and 3 as a joystick numbers them, or (A)/(X) on a pad - the
    // same pair Stunt Car Racer fires on, so one stick behaves the same way
    // in both games.
    internal static bool IsFiring(IGamepad gamepad)
        => gamepad.IsHeld(GamepadButton.A) || gamepad.IsHeld(GamepadButton.X);

    // Speed has no axis left on a one-stick joystick, so it needs the two
    // remaining buttons; a pad reaches it on the triggers as well.
    internal static bool IsAccelerating(IGamepad gamepad)
        => gamepad.IsHeld(GamepadButton.B) || gamepad.Axis(GamepadAxis.RightTrigger) >= Threshold;

    internal static bool IsDecelerating(IGamepad gamepad)
        => gamepad.IsHeld(GamepadButton.Y) || gamepad.Axis(GamepadAxis.LeftTrigger) >= Threshold;

    private static int Direction(float value) => value <= -Threshold ? -1 : value >= Threshold ? 1 : 0;
}
