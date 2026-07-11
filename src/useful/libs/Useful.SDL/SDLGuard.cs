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
        => Execute(sdlMethod, zeroIndicatesError: false, callerArgument);

    // Most SDL/SDL_mixer int-returning functions signal failure with a
    // negative result. A handful (e.g. Mix_RegisterEffect, Mix_UnregisterEffect,
    // Mix_QuerySpec) instead use the inverted convention of zero-means-error,
    // nonzero-means-success; use this overload with zeroIndicatesError: true
    // for those.
    public static int Execute(
        Func<int> sdlMethod,
        bool zeroIndicatesError,
        [CallerArgumentExpression(nameof(sdlMethod))] string? callerArgument = null)
    {
        Debug.Assert(sdlMethod != null, "sdlMethod should not be null");

        int result = sdlMethod();
        bool isError = zeroIndicatesError ? result == 0 : result < 0;
        if (isError)
        {
            SDLHelper.Throw(
                callerArgument?.StartsWith("() => ", StringComparison.OrdinalIgnoreCase) == true
                    ? callerArgument[6..]
                    : callerArgument);
        }

        return result;
    }
}
