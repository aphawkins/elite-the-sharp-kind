// 'SharpKind Libraries' - Andy Hawkins 2023-2026.

using SDL;
using SharpKind.Input;
using static SDL.SDL3;

namespace SharpKind.SDL;

public sealed unsafe class SDLInput : IInput, IDisposable
{
    // The two device classes are kept apart because they are opened and
    // closed by different SDL calls. A device with a gamepad mapping appears
    // in both event streams, so it is only ever opened as a gamepad.
    private readonly Dictionary<SDL_JoystickID, nint> _gamepads = [];
    private readonly Dictionary<SDL_JoystickID, nint> _joysticks = [];
    private IKeyboardSink? _keyboard;
    private IGamepadSink? _gamepad;
    private bool _isDisposed;

    public void Register(IKeyboardSink keyboard) => _keyboard = keyboard;

    public void Register(IGamepadSink gamepad)
    {
        SDLGuard.Execute(() => SDL_Init(SDL_InitFlags.SDL_INIT_GAMEPAD | SDL_InitFlags.SDL_INIT_JOYSTICK));
        _gamepad = gamepad;
    }

    public void Poll()
    {
        while (PollEvent(out SDL_Event sdlEvent) &&
            sdlEvent.type != (uint)SDL_EventType.SDL_EVENT_POLL_SENTINEL &&
            _keyboard?.Close != true)
        {
            switch ((SDL_EventType)sdlEvent.type)
            {
                case SDL_EventType.SDL_EVENT_KEY_DOWN:
                    (ConsoleKey key, ConsoleModifiers modifiers) = SDLHelper.KeyConverter(sdlEvent.key.key);
                    _keyboard?.KeyDown(key, modifiers);
                    break;

                case SDL_EventType.SDL_EVENT_KEY_UP:
                    (ConsoleKey key1, ConsoleModifiers modifiers1) = SDLHelper.KeyConverter(sdlEvent.key.key);
                    _keyboard?.KeyUp(key1, modifiers1);
                    break;

                case SDL_EventType.SDL_EVENT_QUIT:
                    _keyboard?.Close = true;
                    break;

                default:
                    if (_gamepad is not null)
                    {
                        HandleDeviceEvent(sdlEvent, _gamepad);
                    }

                    break;
            }
        }
    }

    public void Dispose()
    {
        if (_isDisposed)
        {
            return;
        }

        foreach (nint gamepad in _gamepads.Values)
        {
            SDL_CloseGamepad((SDL_Gamepad*)gamepad);
        }

        foreach (nint joystick in _joysticks.Values)
        {
            SDL_CloseJoystick((SDL_Joystick*)joystick);
        }

        _gamepads.Clear();
        _joysticks.Clear();
        _isDisposed = true;
    }

    // SDL reports stick axes over the full signed 16-bit range and triggers
    // over the positive half, so both normalise the same way. A digital HID
    // stick sits at the ends of that range, which lands on exactly -1/0/+1.
    private static float Normalise(short value) => Math.Clamp(value / 32767f, -1f, 1f);

    private static GamepadButton ConvertGamepadButton(SDL_GamepadButton button) => button switch
    {
        SDL_GamepadButton.SDL_GAMEPAD_BUTTON_SOUTH => GamepadButton.A,
        SDL_GamepadButton.SDL_GAMEPAD_BUTTON_EAST => GamepadButton.B,
        SDL_GamepadButton.SDL_GAMEPAD_BUTTON_WEST => GamepadButton.X,
        SDL_GamepadButton.SDL_GAMEPAD_BUTTON_NORTH => GamepadButton.Y,
        SDL_GamepadButton.SDL_GAMEPAD_BUTTON_LEFT_SHOULDER => GamepadButton.LeftShoulder,
        SDL_GamepadButton.SDL_GAMEPAD_BUTTON_RIGHT_SHOULDER => GamepadButton.RightShoulder,
        SDL_GamepadButton.SDL_GAMEPAD_BUTTON_BACK => GamepadButton.Back,
        SDL_GamepadButton.SDL_GAMEPAD_BUTTON_START => GamepadButton.Start,
        _ => GamepadButton.None,
    };

    private static GamepadAxis? ConvertGamepadAxis(SDL_GamepadAxis axis) => axis switch
    {
        SDL_GamepadAxis.SDL_GAMEPAD_AXIS_LEFTX => GamepadAxis.LeftX,
        SDL_GamepadAxis.SDL_GAMEPAD_AXIS_LEFTY => GamepadAxis.LeftY,
        SDL_GamepadAxis.SDL_GAMEPAD_AXIS_RIGHTX => GamepadAxis.RightX,
        SDL_GamepadAxis.SDL_GAMEPAD_AXIS_RIGHTY => GamepadAxis.RightY,
        SDL_GamepadAxis.SDL_GAMEPAD_AXIS_LEFT_TRIGGER => GamepadAxis.LeftTrigger,
        SDL_GamepadAxis.SDL_GAMEPAD_AXIS_RIGHT_TRIGGER => GamepadAxis.RightTrigger,
        _ => null,
    };

    // A device with no gamepad mapping numbers its buttons and axes and says
    // nothing about what they are, so index order is the only mapping there
    // is: the Competition Pro fire buttons come out as A, B, X, Y.
    private static GamepadButton ConvertJoystickButton(byte index) => index switch
    {
        0 => GamepadButton.A,
        1 => GamepadButton.B,
        2 => GamepadButton.X,
        3 => GamepadButton.Y,
        4 => GamepadButton.LeftShoulder,
        5 => GamepadButton.RightShoulder,
        6 => GamepadButton.Back,
        7 => GamepadButton.Start,
        _ => GamepadButton.None,
    };

    private static GamepadAxis? ConvertJoystickAxis(byte index) => index switch
    {
        0 => GamepadAxis.LeftX,
        1 => GamepadAxis.LeftY,
        2 => GamepadAxis.RightX,
        3 => GamepadAxis.RightY,
        _ => null,
    };

    private static bool PollEvent(out SDL_Event sdlEvent)
    {
        // SDL3's bool result signals whether an event was returned, not
        // failure - the end of the queue is instead detected via the
        // SDL_EVENT_POLL_SENTINEL check in the caller's loop condition, so
        // there is no error case to guard here.
        sdlEvent = default;

        fixed (SDL_Event* eventPtr = &sdlEvent)
        {
            _ = SDL_PollEvent(eventPtr);
        }

        return true;
    }

    private static void ButtonChanged(GamepadButton button, bool down, IGamepadSink gamepad)
    {
        if (down)
        {
            gamepad.ButtonDown(button);
        }
        else
        {
            gamepad.ButtonUp(button);
        }
    }

    private static void AxisMoved(GamepadAxis? axis, short value, IGamepadSink gamepad)
    {
        if (axis.HasValue)
        {
            gamepad.AxisMoved(axis.Value, Normalise(value));
        }
    }

    // A hat is a second way of expressing the same 8-way direction the stick
    // gives, so it feeds the left stick axes rather than a separate control.
    private static void HatMoved(byte hat, IGamepadSink gamepad)
    {
        float x = 0f;
        if ((hat & SDL_HAT_LEFT) != 0)
        {
            x = -1f;
        }
        else if ((hat & SDL_HAT_RIGHT) != 0)
        {
            x = 1f;
        }

        float y = 0f;
        if ((hat & SDL_HAT_UP) != 0)
        {
            y = -1f;
        }
        else if ((hat & SDL_HAT_DOWN) != 0)
        {
            y = 1f;
        }

        gamepad.AxisMoved(GamepadAxis.LeftX, x);
        gamepad.AxisMoved(GamepadAxis.LeftY, y);
    }

    private void HandleDeviceEvent(SDL_Event sdlEvent, IGamepadSink gamepad)
    {
        switch ((SDL_EventType)sdlEvent.type)
        {
            case SDL_EventType.SDL_EVENT_GAMEPAD_ADDED:
                OpenGamepad(sdlEvent.gdevice.which, gamepad);
                break;

            case SDL_EventType.SDL_EVENT_GAMEPAD_REMOVED:
                CloseGamepad(sdlEvent.gdevice.which, gamepad);
                break;

            case SDL_EventType.SDL_EVENT_GAMEPAD_BUTTON_DOWN:
                ButtonChanged(ConvertGamepadButton((SDL_GamepadButton)sdlEvent.gbutton.button), down: true, gamepad);
                break;

            case SDL_EventType.SDL_EVENT_GAMEPAD_BUTTON_UP:
                ButtonChanged(ConvertGamepadButton((SDL_GamepadButton)sdlEvent.gbutton.button), down: false, gamepad);
                break;

            case SDL_EventType.SDL_EVENT_GAMEPAD_AXIS_MOTION:
                AxisMoved(ConvertGamepadAxis((SDL_GamepadAxis)sdlEvent.gaxis.axis), sdlEvent.gaxis.value, gamepad);
                break;

            case SDL_EventType.SDL_EVENT_JOYSTICK_ADDED:
                OpenJoystick(sdlEvent.jdevice.which, gamepad);
                break;

            case SDL_EventType.SDL_EVENT_JOYSTICK_REMOVED:
                CloseJoystick(sdlEvent.jdevice.which, gamepad);
                break;

            case SDL_EventType.SDL_EVENT_JOYSTICK_BUTTON_DOWN:
                if (_joysticks.ContainsKey(sdlEvent.jbutton.which))
                {
                    ButtonChanged(ConvertJoystickButton(sdlEvent.jbutton.button), down: true, gamepad);
                }

                break;

            case SDL_EventType.SDL_EVENT_JOYSTICK_BUTTON_UP:
                if (_joysticks.ContainsKey(sdlEvent.jbutton.which))
                {
                    ButtonChanged(ConvertJoystickButton(sdlEvent.jbutton.button), down: false, gamepad);
                }

                break;

            case SDL_EventType.SDL_EVENT_JOYSTICK_AXIS_MOTION:
                if (_joysticks.ContainsKey(sdlEvent.jaxis.which))
                {
                    AxisMoved(ConvertJoystickAxis(sdlEvent.jaxis.axis), sdlEvent.jaxis.value, gamepad);
                }

                break;

            case SDL_EventType.SDL_EVENT_JOYSTICK_HAT_MOTION:
                if (_joysticks.ContainsKey(sdlEvent.jhat.which))
                {
                    HatMoved(sdlEvent.jhat.value, gamepad);
                }

                break;
        }
    }

    private void OpenGamepad(SDL_JoystickID which, IGamepadSink gamepad)
    {
        if (_gamepads.ContainsKey(which))
        {
            return;
        }

        nint handle = (nint)SDL_OpenGamepad(which);
        if (handle == nint.Zero)
        {
            // A device that will not open is simply not available to play
            // with; failing the frame over it would be worse than ignoring it.
            return;
        }

        _gamepads[which] = handle;
        gamepad.Connected();
    }

    private void CloseGamepad(SDL_JoystickID which, IGamepadSink gamepad)
    {
        if (!_gamepads.Remove(which, out nint handle))
        {
            return;
        }

        SDL_CloseGamepad((SDL_Gamepad*)handle);
        gamepad.Disconnected();
    }

    private void OpenJoystick(SDL_JoystickID which, IGamepadSink gamepad)
    {
        // A mapped device is handled through the gamepad events, which name
        // their buttons; opening it here as well would double-count it.
        if (SDL_IsGamepad(which) || _joysticks.ContainsKey(which))
        {
            return;
        }

        nint handle = (nint)SDL_OpenJoystick(which);
        if (handle == nint.Zero)
        {
            return;
        }

        _joysticks[which] = handle;
        gamepad.Connected();
    }

    private void CloseJoystick(SDL_JoystickID which, IGamepadSink gamepad)
    {
        if (!_joysticks.Remove(which, out nint handle))
        {
            return;
        }

        SDL_CloseJoystick((SDL_Joystick*)handle);
        gamepad.Disconnected();
    }
}
