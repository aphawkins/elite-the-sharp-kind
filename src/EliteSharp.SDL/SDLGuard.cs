// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Runtime.CompilerServices;

namespace EliteSharp.SDL
{
    internal static class SDLGuard
    {
        internal static nint Execute(Func<nint> sdlMethod, [CallerArgumentExpression(nameof(sdlMethod))] string? callerArgument = null)
        {
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

        internal static int Execute(Func<int> sdlMethod, [CallerArgumentExpression(nameof(sdlMethod))] string? callerArgument = null)
        {
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
}
