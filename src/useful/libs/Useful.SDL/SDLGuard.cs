// 'Useful Libraries' - Andy Hawkins 2025.

using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Useful.SDL;

public static class SDLGuard
{
    public static nint Execute(Func<nint> sdlMethod, [CallerArgumentExpression(nameof(sdlMethod))] string? callerArgument = null)
    {
        Debug.Assert(sdlMethod != null, "sdlMethod should not be null");

        nint result = sdlMethod();
        if (result == nint.Zero)
        {
            SDLHelper.Throw(
                callerArgument?.StartsWith("() => ", StringComparison.OrdinalIgnoreCase) == true
                ? callerArgument[6..]
                : callerArgument);
        }

        return result;
    }

    public static int Execute(Func<int> sdlMethod, [CallerArgumentExpression(nameof(sdlMethod))] string? callerArgument = null)
    {
        Debug.Assert(sdlMethod != null, "sdlMethod should not be null");

        int result = sdlMethod();
        if (result < 0)
        {
            SDLHelper.Throw(
                callerArgument?.StartsWith("() => ", StringComparison.OrdinalIgnoreCase) == true
                    ? callerArgument[6..]
                    : callerArgument);
        }

        return result;
    }
}
