// 'Stunt Car Racer - The Sharp Kind' - Andy Hawkins 2026.
// 'Stunt Car Racer Remake' - sourceforge.net/projects/stuntcarremake.
// Stunt Car Racer (C) Geoff Crammond / MicroStyle / MicroProse 1989.

using SharpKind.Input;
using StuntCarRacerSharpLib.Cars;

namespace StuntCarRacerSharpLib.Screens;

// Turns the analog readings of an IGamepad into the digital controls this
// game has: the physics only knows "left" and "right" (CarControl in
// ptitSeb's Car_Behaviour.cpp:812-815 sets a fixed +/-15 either way), so
// travel past a threshold is all that can be used. A digital HID stick sits
// at the ends of the range, so it passes the threshold whatever it is set to.
internal static class GamepadControls
{
    private const float Threshold = 0.5f;

    // -1 steers left, +1 right, 0 is centred.
    internal static int Steer(IGamepad gamepad)
    {
        float x = gamepad.Axis(GamepadAxis.LeftX);

        return x <= -Threshold ? -1 : x >= Threshold ? 1 : 0;
    }

    // Two layouts at once, because the two target devices have nothing in
    // common but the stick. An XInput pad drives as ptitSeb's remake maps it
    // (Car_Behaviour.cpp:793-817): right trigger = accelerate, (B) or left
    // trigger = brake, (A) = boost. A one-stick joystick has no triggers, so
    // it drives the arcade way instead: stick forward = accelerate, back =
    // brake, fire = boost. Neither layout can reach the other's controls, so
    // both are read unconditionally rather than guessing at the device.
    internal static CarInput ReadCarInput(IGamepad gamepad)
    {
        CarInput input = CarInput.None;

        switch (Steer(gamepad))
        {
            case -1:
                input |= CarInput.Left;
                break;

            case 1:
                input |= CarInput.Right;
                break;
        }

        // SDL's Y axis is positive downwards, so forward on the stick is
        // negative - the same convention the screen has.
        float y = gamepad.Axis(GamepadAxis.LeftY);

        if (IsPulled(gamepad, GamepadAxis.RightTrigger) || y <= -Threshold)
        {
            input |= CarInput.Accelerate;
        }

        // Buttons 1 and 4 as the device numbers them, which is what falls
        // under the thumb and the trigger finger on a Competition Pro.
        if (gamepad.IsHeld(GamepadButton.A) || gamepad.IsHeld(GamepadButton.Y))
        {
            input |= CarInput.Boost;
        }

        if (gamepad.IsHeld(GamepadButton.B) || IsPulled(gamepad, GamepadAxis.LeftTrigger) || y >= Threshold)
        {
            input &= ~CarInput.Accelerate;
            input |= CarInput.Brake;
        }

        return input;
    }

    private static bool IsPulled(IGamepad gamepad, GamepadAxis axis) => gamepad.Axis(axis) >= Threshold;
}
