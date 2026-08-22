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

    // ptitSeb's pad mapping (Car_Behaviour.cpp:793-817): right trigger =
    // accelerate, (A) = boost, (B) or left trigger = brake, left stick =
    // steer. Brake wins over accelerate, as it does there.
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

        if (IsPulled(gamepad, GamepadAxis.RightTrigger))
        {
            input |= CarInput.Accelerate;
        }

        if (gamepad.IsHeld(GamepadButton.A))
        {
            input |= CarInput.Boost;
        }

        if (gamepad.IsHeld(GamepadButton.B) || IsPulled(gamepad, GamepadAxis.LeftTrigger))
        {
            input &= ~CarInput.Accelerate;
            input |= CarInput.Brake;
        }

        return input;
    }

    private static bool IsPulled(IGamepad gamepad, GamepadAxis axis) => gamepad.Axis(axis) >= Threshold;
}
