// 'Useful Libraries' - Andy Hawkins 2025.

using System.Runtime.CompilerServices;

namespace Useful;

public static class Guard
{
    public static void ArgumentNull<T>(
        [ValidatedNotNull] T argument,
        [CallerArgumentExpression(nameof(argument))] string? callerArg = null)
        where T : class
    {
        if (argument == null)
        {
            throw new ArgumentNullException(callerArg);
        }
    }
}
